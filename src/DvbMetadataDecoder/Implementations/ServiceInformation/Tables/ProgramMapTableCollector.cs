using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal sealed class ProgramMapTableCollector : IAsyncDisposable
{
    private readonly IProgramAssociationTable _programAssociationTable;
    private readonly ChannelReader<(ushort Pid, IServiceInformationSection Section)> _subscription;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Task _worker;
    private readonly Dictionary<(ushort Pid, ushort ProgramNumber), Candidate> _candidates = [];
    private volatile IProgramMapTable[] _published = [];

    public ProgramMapTableCollector(
        IServiceInformationSectionFactory sectionFactory,
        IProgramAssociationTable programAssociationTable)
    {
        _programAssociationTable = programAssociationTable;
        _subscription = sectionFactory.Subscribe();
        _worker = ProcessSections(_shutdown.Token);
    }

    public IReadOnlyCollection<IProgramMapTable> ProgramMapTables
    {
        get
        {
            var associations = _programAssociationTable.PidAssociations.ToHashSet();
            var currentTables = _published
                .Where(table => associations.Contains((table.ProgramNumber, table.ProgramMapPid)))
                .ToArray();
            return Array.AsReadOnly(currentTables);
        }
    }

    private async Task ProcessSections(CancellationToken cancellationToken)
    {
        while (await _subscription.WaitToReadAsync(cancellationToken))
        {
            while (!cancellationToken.IsCancellationRequested && _subscription.TryRead(out var sectionData))
            {
                var (pid, section) = sectionData;
                if (section is not IProgramMapSection programMapSection ||
                    !programMapSection.IsCurrent ||
                    programMapSection.SectionNumber > programMapSection.LastSectionNumber)
                {
                    continue;
                }

                var associations = _programAssociationTable.PidAssociations;
                if (!associations.Contains((programMapSection.ProgramNumber, pid)))
                {
                    continue;
                }

                RemoveCandidatesForInactiveAssociations(associations);

                var key = (Pid: pid, programMapSection.ProgramNumber);
                if (!_candidates.TryGetValue(key, out var candidate) ||
                    candidate.VersionNumber != programMapSection.VersionNumber ||
                    candidate.LastSectionNumber != programMapSection.LastSectionNumber)
                {
                    candidate = new(programMapSection.VersionNumber, programMapSection.LastSectionNumber);
                    _candidates[key] = candidate;
                }

                candidate.Sections[programMapSection.SectionNumber] = programMapSection;
                if (candidate.Sections.Count != candidate.LastSectionNumber + 1)
                    continue;

                var sections = Enumerable.Range(0, candidate.LastSectionNumber + 1)
                    .Select(sectionNumber => candidate.Sections[(byte)sectionNumber])
                    .ToArray();
                var pcrPid = sections[0].PcrPid;
                if (sections.Any(candidateSection => candidateSection.PcrPid != pcrPid))
                {
                    _candidates.Remove(key);
                    continue;
                }

                var descriptors = sections.SelectMany(candidateSection => candidateSection.Descriptors).ToArray();
                var elementaryStreams = sections.SelectMany(candidateSection => candidateSection.EsInfos).ToArray();
                var snapshot = new ProgramMapTable(
                    pid,
                    programMapSection.ProgramNumber,
                    programMapSection.VersionNumber,
                    pcrPid,
                    Array.AsReadOnly(descriptors),
                    Array.AsReadOnly(elementaryStreams));

                var publishedTables = _published.ToDictionary(
                    table => (table.ProgramMapPid, table.ProgramNumber));
                publishedTables[key] = snapshot;
                _published = publishedTables.Values.ToArray();
            }
        }
    }

    private void RemoveCandidatesForInactiveAssociations(
        IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> associations)
    {
        var activeAssociations = associations.ToHashSet();
        foreach (var key in _candidates.Keys
                     .Where(key => !activeAssociations.Contains((key.ProgramNumber, key.Pid)))
                     .ToArray())
        {
            _candidates.Remove(key);
        }
    }

    public async ValueTask DisposeAsync()
    {
        _shutdown.Cancel();
        try
        {
            await _worker.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested)
        {
        }
        finally
        {
            _shutdown.Dispose();
        }
    }

    private sealed class Candidate(byte versionNumber, byte lastSectionNumber)
    {
        public byte VersionNumber { get; } = versionNumber;

        public byte LastSectionNumber { get; } = lastSectionNumber;

        public Dictionary<byte, IProgramMapSection> Sections { get; } = [];
    }
}