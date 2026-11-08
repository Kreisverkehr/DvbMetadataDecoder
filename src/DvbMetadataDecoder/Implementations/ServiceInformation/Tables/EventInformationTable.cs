namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal sealed class EventInformationTable : IEventInformationTable
{
    public EventInformationTable(ushort serviceId, ushort transportStreamId, ushort originalNetworkId, IEnumerable<IEventInformationSubtable> subtables)
    {
        ServiceId = serviceId;
        TransportStreamId = transportStreamId;
        OriginalNetworkId = originalNetworkId;
        var subtableSnapshots = subtables.OrderBy(subtable => subtable.TableId).ToArray();
        Subtables = Array.AsReadOnly(subtableSnapshots);
        Events = Array.AsReadOnly(subtableSnapshots.SelectMany(subtable => subtable.Events).ToArray());
    }

    public ushort ServiceId { get; }

    public ushort TransportStreamId { get; }

    public ushort OriginalNetworkId { get; }

    public IReadOnlyCollection<IEventInformationSubtable> Subtables { get; }

    public IReadOnlyCollection<IEventInformationSection.IEventInformation> Events { get; }
}

internal sealed class EventInformationSubtable : IEventInformationSubtable
{
    public EventInformationSubtable(byte tableId, byte versionNumber, byte lastSectionNumber, byte lastTableId, IEnumerable<IEventInformationSegment> segments)
    {
        TableId = tableId;
        VersionNumber = versionNumber;
        LastSectionNumber = lastSectionNumber;
        LastTableId = lastTableId;
        var segmentSnapshots = segments.OrderBy(segment => segment.FirstSectionNumber).ToArray();
        Segments = Array.AsReadOnly(segmentSnapshots);
        Events = Array.AsReadOnly(segmentSnapshots.SelectMany(segment => segment.Events).ToArray());
    }

    public byte TableId { get; }

    public byte VersionNumber { get; }

    public byte LastSectionNumber { get; }

    public byte LastTableId { get; }

    public IReadOnlyCollection<IEventInformationSegment> Segments { get; }

    public IReadOnlyCollection<IEventInformationSection.IEventInformation> Events { get; }
}

internal sealed class EventInformationSegment : IEventInformationSegment
{
    public EventInformationSegment(byte firstSectionNumber, byte lastSectionNumber, IEnumerable<IEventInformationSection.IEventInformation> events)
    {
        FirstSectionNumber = firstSectionNumber;
        LastSectionNumber = lastSectionNumber;
        Events = Array.AsReadOnly(events.ToArray());
    }

    public byte FirstSectionNumber { get; }

    public byte LastSectionNumber { get; }

    public IReadOnlyCollection<IEventInformationSection.IEventInformation> Events { get; }
}