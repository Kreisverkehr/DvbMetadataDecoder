using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal sealed class EventInformationTableCollector : IAsyncDisposable
{
    private readonly ChannelReader<(ushort Pid, IServiceInformationSection Section)> _subscription;
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Task _worker;
    private readonly Dictionary<SubtableKey, PresentFollowingCandidate> _presentFollowingCandidates = [];
    private readonly Dictionary<SubtableKey, ScheduleCandidate> _scheduleCandidates = [];
    private readonly Dictionary<SubtableKey, IEventInformationSubtable> _publishedSubtables = [];
    private volatile PublishedTables _published = new([], [], [], []);

    public EventInformationTableCollector(IServiceInformationSectionFactory sectionFactory)
    {
        _subscription = sectionFactory.Subscribe();
        _worker = ProcessSections(_shutdown.Token);
    }

    public IReadOnlyCollection<IEventInformationTable> ActualTransportStreamPresentFollowingEventInformationTables =>
        _published.ActualTransportStreamPresentFollowingEventInformationTables;

    public IReadOnlyCollection<IEventInformationTable> OtherTransportStreamPresentFollowingEventInformationTables =>
        _published.OtherTransportStreamPresentFollowingEventInformationTables;

    public IReadOnlyCollection<IEventInformationTable> ActualTransportStreamScheduleEventInformationTables =>
        _published.ActualTransportStreamScheduleEventInformationTables;

    public IReadOnlyCollection<IEventInformationTable> OtherTransportStreamScheduleEventInformationTables =>
        _published.OtherTransportStreamScheduleEventInformationTables;

    private async Task ProcessSections(CancellationToken cancellationToken)
    {
        while (await _subscription.WaitToReadAsync(cancellationToken))
        {
            while (!cancellationToken.IsCancellationRequested && _subscription.TryRead(out var sectionData))
            {
                var (pid, section) = sectionData;
                if (pid != 0x12 || section is not IEventInformationSection eventSection || !IsValid(eventSection))
                    continue;

                var key = new SubtableKey(
                    eventSection.ServiceId,
                    eventSection.TransportStreamId,
                    eventSection.OriginalNetworkId,
                    eventSection.TableId);

                if (eventSection.TableId is 0x4E or 0x4F)
                    ProcessPresentFollowingSection(key, eventSection);
                else if (eventSection.TableId is >= 0x50 and <= 0x6F)
                    ProcessScheduleSection(key, eventSection);
            }
        }
    }

    private void ProcessPresentFollowingSection(SubtableKey key, IEventInformationSection section)
    {
        if (!_presentFollowingCandidates.TryGetValue(key, out var candidate) ||
            candidate.VersionNumber != section.VersionNumber ||
            candidate.LastSectionNumber != section.LastSectionNumber ||
            candidate.LastTableId != section.LastTableId)
        {
            candidate = new(section.VersionNumber, section.LastSectionNumber, section.LastTableId);
            _presentFollowingCandidates[key] = candidate;
        }

        candidate.Sections[section.SectionNumber] = section;
        if (candidate.Sections.Count != candidate.LastSectionNumber + 1)
            return;

        var sections = Enumerable.Range(0, candidate.LastSectionNumber + 1)
            .Select(sectionNumber => candidate.Sections[(byte)sectionNumber])
            .ToArray();
        var segments = sections
            .GroupBy(candidateSection => (byte)(candidateSection.SectionNumber / 8 * 8))
            .Select(group => new EventInformationSegment(
                group.Key,
                group.First().SegmentLastSectionNumber,
                group.OrderBy(candidateSection => candidateSection.SectionNumber)
                    .SelectMany(candidateSection => candidateSection.Events)));

        Publish(key, new EventInformationSubtable(
            key.TableId,
            candidate.VersionNumber,
            candidate.LastSectionNumber,
            candidate.LastTableId,
            segments));
    }

    private void ProcessScheduleSection(SubtableKey key, IEventInformationSection section)
    {
        if (!_scheduleCandidates.TryGetValue(key, out var candidate) ||
            candidate.VersionNumber != section.VersionNumber ||
            candidate.LastSectionNumber != section.LastSectionNumber ||
            candidate.LastTableId != section.LastTableId)
        {
            candidate = new(section.VersionNumber, section.LastSectionNumber, section.LastTableId);
            _scheduleCandidates[key] = candidate;
        }

        if (section.SectionNumber == 0 && candidate.HasSeenSectionZero)
        {
            if (candidate.IsComplete)
                Publish(key, CreateScheduleSnapshot(key, candidate));
        }

        if (!candidate.HasSeenSectionZero)
        {
            if (section.SectionNumber != 0)
                return;

            candidate.HasSeenSectionZero = true;
        }

        var segmentStart = (byte)(section.SectionNumber / 8 * 8);
        if (!candidate.Segments.TryGetValue(segmentStart, out var segment) ||
            segment.LastSectionNumber != section.SegmentLastSectionNumber)
        {
            segment = new(segmentStart, section.SegmentLastSectionNumber);
            candidate.Segments[segmentStart] = segment;
        }

        segment.Sections[section.SectionNumber] = section;
    }

    private static IEventInformationSubtable CreateScheduleSnapshot(SubtableKey key, ScheduleCandidate candidate)
    {
        var segments = candidate.Segments.Values
            .OrderBy(segment => segment.FirstSectionNumber)
            .Select(segment => new EventInformationSegment(
                segment.FirstSectionNumber,
                segment.LastSectionNumber,
                Enumerable.Range(segment.FirstSectionNumber, segment.LastSectionNumber - segment.FirstSectionNumber + 1)
                    .Select(sectionNumber => segment.Sections[(byte)sectionNumber])
                    .SelectMany(section => section.Events)));

        return new EventInformationSubtable(
            key.TableId,
            candidate.VersionNumber,
            candidate.LastSectionNumber,
            candidate.LastTableId,
            segments);
    }

    private static bool IsValid(IEventInformationSection section)
    {
        if (!section.IsCurrent || section.SectionNumber > section.LastSectionNumber)
            return false;

        var segmentStart = section.SectionNumber / 8 * 8;
        return section.SegmentLastSectionNumber >= section.SectionNumber &&
               section.SegmentLastSectionNumber <= segmentStart + 7 &&
               section.SegmentLastSectionNumber <= section.LastSectionNumber;
    }

    private void Publish(SubtableKey key, IEventInformationSubtable subtable)
    {
        _publishedSubtables[key] = subtable;
        var entries = _publishedSubtables.ToArray();
        _published = new(
            BuildTables(entries, tableId => tableId == 0x4E),
            BuildTables(entries, tableId => tableId == 0x4F),
            BuildTables(entries, tableId => tableId is >= 0x50 and <= 0x5F),
            BuildTables(entries, tableId => tableId is >= 0x60 and <= 0x6F));
    }

    private static IReadOnlyCollection<IEventInformationTable> BuildTables(IEnumerable<KeyValuePair<SubtableKey, IEventInformationSubtable>> entries, Func<byte, bool> tableIdPredicate)
    {
        var tables = entries
            .Where(entry => tableIdPredicate(entry.Key.TableId))
            .GroupBy(entry => (entry.Key.ServiceId, entry.Key.TransportStreamId, entry.Key.OriginalNetworkId))
            .Select(group => (IEventInformationTable)new EventInformationTable(
                group.Key.ServiceId,
                group.Key.TransportStreamId,
                group.Key.OriginalNetworkId,
                group.Select(entry => entry.Value)))
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

    private readonly record struct SubtableKey(ushort ServiceId, ushort TransportStreamId, ushort OriginalNetworkId, byte TableId);

    private sealed class PresentFollowingCandidate(byte versionNumber, byte lastSectionNumber, byte lastTableId)
    {
        public byte VersionNumber { get; } = versionNumber;

        public byte LastSectionNumber { get; } = lastSectionNumber;

        public byte LastTableId { get; } = lastTableId;

        public Dictionary<byte, IEventInformationSection> Sections { get; } = [];
    }

    private sealed class ScheduleCandidate(byte versionNumber, byte lastSectionNumber, byte lastTableId)
    {
        public byte VersionNumber { get; } = versionNumber;

        public byte LastSectionNumber { get; } = lastSectionNumber;

        public byte LastTableId { get; } = lastTableId;

        public bool HasSeenSectionZero { get; set; }

        public Dictionary<byte, ScheduleSegmentCandidate> Segments { get; } = [];

        public bool IsComplete => Segments.Count > 0 && Segments.Values.All(segment => segment.IsComplete);
    }

    private sealed class ScheduleSegmentCandidate(byte firstSectionNumber, byte lastSectionNumber)
    {
        public byte FirstSectionNumber { get; } = firstSectionNumber;

        public byte LastSectionNumber { get; } = lastSectionNumber;

        public Dictionary<byte, IEventInformationSection> Sections { get; } = [];

        public bool IsComplete => Sections.Count == LastSectionNumber - FirstSectionNumber + 1;
    }

    private sealed record PublishedTables(
        IReadOnlyCollection<IEventInformationTable> ActualTransportStreamPresentFollowingEventInformationTables,
        IReadOnlyCollection<IEventInformationTable> OtherTransportStreamPresentFollowingEventInformationTables,
        IReadOnlyCollection<IEventInformationTable> ActualTransportStreamScheduleEventInformationTables,
        IReadOnlyCollection<IEventInformationTable> OtherTransportStreamScheduleEventInformationTables
    );
}