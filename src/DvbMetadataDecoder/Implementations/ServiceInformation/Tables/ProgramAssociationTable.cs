using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal class ProgramAssociationTable : IProgramAssociationTable, IAsyncDisposable
{
    private readonly IServiceInformationSectionFactory _sectionFactory;
    private readonly ChannelReader<(ushort Pid, IServiceInformationSection Section)> _subscription;
    private readonly Task _subscriptionWorkerTask;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Dictionary<byte, IProgramAssociationSection> _candidateSections = [];
    private (ushort TransportStreamId, byte VersionNumber, byte LastSectionNumber)? _candidate;
    private volatile PublishedPat _published = new(0, [], []);

    public ProgramAssociationTable(IServiceInformationSectionFactory sectionFactory)
    {
        _sectionFactory = sectionFactory;
        _subscription = _sectionFactory.Subscribe();
        _subscriptionWorkerTask = ProcessSections(_shutdown.Token);
    }

    private async Task ProcessSections(CancellationToken cancellationToken)
    {
        while (await _subscription.WaitToReadAsync(cancellationToken))
        {
            while (!cancellationToken.IsCancellationRequested && _subscription.TryRead(out var sectionData))
            {
                (var pid, var section) = sectionData;
                if (pid != 0x00 || section is not IProgramAssociationSection patSection || !patSection.IsCurrent)
                {
                    continue;
                }

                if (patSection.SectionNumber > patSection.LastSectionNumber)
                    continue;

                var candidate = (patSection.TransportStreamId, patSection.VersionNumber, patSection.LastSectionNumber);
                if (_candidate != candidate)
                {
                    _candidate = candidate;
                    _candidateSections.Clear();
                }

                _candidateSections[patSection.SectionNumber] = patSection;
                if (_candidateSections.Count != patSection.LastSectionNumber + 1)
                    continue;

                var sections = Enumerable.Range(0, patSection.LastSectionNumber + 1)
                    .Select(sectionNumber => _candidateSections[(byte)sectionNumber])
                    .ToArray();
                var networkPids = sections.SelectMany(candidateSection => candidateSection.NetworkPids).ToArray();
                var pidAssociations = sections.SelectMany(candidateSection => candidateSection.PidAssociations).ToArray();

                _published = new(
                    patSection.VersionNumber,
                    Array.AsReadOnly(networkPids),
                    Array.AsReadOnly(pidAssociations));
            }
        }
    }

    public byte VersionNumber => _published.VersionNumber;

    public IReadOnlyCollection<ushort> NetworkPids => _published.NetworkPids;

    public IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations => _published.PidAssociations;

    public IEnumerable<ushort> ProgramMapPids => PidAssociations.Select(a => a.ProgramMapPid);

    public async ValueTask DisposeAsync()
    {
        _shutdown.Cancel();
        try
        {
            await _subscriptionWorkerTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_shutdown.IsCancellationRequested)
        {
        }
        finally
        {
            _shutdown.Dispose();
        }
    }

    private sealed record PublishedPat(byte VersionNumber, IReadOnlyCollection<ushort> NetworkPids, IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations);
}