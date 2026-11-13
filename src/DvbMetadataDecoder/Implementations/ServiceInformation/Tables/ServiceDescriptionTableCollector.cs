using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal sealed class ServiceDescriptionTableCollector : IAsyncDisposable
{
    private readonly ChannelReader<(ushort Pid, IServiceInformationSection Section)> _subscription;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Task _worker;
    private readonly Dictionary<TableKey, Candidate> _candidates = [];
    private readonly Dictionary<TableKey, IServiceDescriptionTable> _publishedTables = [];
    private volatile PublishedTables _published = new([], []);

    public ServiceDescriptionTableCollector(IServiceInformationSectionFactory sectionFactory)
    {
        _subscription = sectionFactory.Subscribe();
        _worker = ProcessSections(_shutdown.Token);
    }

    public IReadOnlyCollection<IServiceDescriptionTable> ActualTransportStreamServiceDescriptionTables =>
        _published.ActualTransportStreamTables;

    public IReadOnlyCollection<IServiceDescriptionTable> OtherTransportStreamServiceDescriptionTables =>
        _published.OtherTransportStreamTables;

    private async Task ProcessSections(CancellationToken cancellationToken)
    {
        while (await _subscription.WaitToReadAsync(cancellationToken))
        {
            while (!cancellationToken.IsCancellationRequested && _subscription.TryRead(out var sectionData))
            {
                var (pid, section) = sectionData;
                if (pid != 0x11 || section is not IServiceDescriptionSection serviceDescriptionSection ||
                    !IsValid(serviceDescriptionSection))
                    continue;

                ProcessSection(serviceDescriptionSection);
            }
        }
    }

    private void ProcessSection(IServiceDescriptionSection section)
    {
        var key = new TableKey(section.TableId, section.TransportStreamId, section.OriginalNetworkId);
        if (!_candidates.TryGetValue(key, out var candidate) ||
            candidate.VersionNumber != section.VersionNumber ||
            candidate.LastSectionNumber != section.LastSectionNumber)
        {
            candidate = new(section.VersionNumber, section.LastSectionNumber);
            _candidates[key] = candidate;
        }

        candidate.Sections[section.SectionNumber] = section;
        if (candidate.IsPublished || candidate.Sections.Count != candidate.LastSectionNumber + 1)
            return;

        var services = Enumerable.Range(0, candidate.LastSectionNumber + 1)
            .Select(sectionNumber => candidate.Sections[(byte)sectionNumber])
            .SelectMany(candidateSection => candidateSection.Services);
        var table = new ServiceDescriptionTable(
            key.TableId,
            key.TransportStreamId,
            key.OriginalNetworkId,
            candidate.VersionNumber,
            services);

        candidate.IsPublished = true;
        Publish(key, table);
    }

    private static bool IsValid(IServiceDescriptionSection section) =>
        section.IsCurrent &&
        section.TableId is 0x42 or 0x46 &&
        section.SectionNumber <= section.LastSectionNumber;

    private void Publish(TableKey key, IServiceDescriptionTable table)
    {
        _publishedTables[key] = table;
        var entries = _publishedTables.ToArray();
        _published = new(
            BuildTables(entries, 0x42),
            BuildTables(entries, 0x46));
    }

    private static IReadOnlyCollection<IServiceDescriptionTable> BuildTables(
        IEnumerable<KeyValuePair<TableKey, IServiceDescriptionTable>> entries,
        byte tableId)
    {
        var tables = entries
            .Where(entry => entry.Key.TableId == tableId)
            .OrderBy(entry => entry.Key.TransportStreamId)
            .ThenBy(entry => entry.Key.OriginalNetworkId)
            .Select(entry => entry.Value)
            .ToArray();

        return Array.AsReadOnly(tables);
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

    private readonly record struct TableKey(byte TableId, ushort TransportStreamId, ushort OriginalNetworkId);

    private sealed class Candidate(byte versionNumber, byte lastSectionNumber)
    {
        public byte VersionNumber { get; } = versionNumber;

        public byte LastSectionNumber { get; } = lastSectionNumber;

        public Dictionary<byte, IServiceDescriptionSection> Sections { get; } = [];

        public bool IsPublished { get; set; }
    }

    private sealed record PublishedTables(
        IReadOnlyCollection<IServiceDescriptionTable> ActualTransportStreamTables,
        IReadOnlyCollection<IServiceDescriptionTable> OtherTransportStreamTables);
}