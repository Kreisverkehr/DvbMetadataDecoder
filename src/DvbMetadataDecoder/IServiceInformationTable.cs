namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationTable;

public interface IProgramAssociationTable : IServiceInformationTable
{
    ushort TransportStreamId { get; }
    byte VersionNumber { get; }
    IReadOnlyCollection<ushort> NetworkPids { get; }
    IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations { get; }
    IEnumerable<ushort> ProgramMapPids { get; }
}

public interface IProgramMapTable : IServiceInformationTable
{
    ushort ProgramMapPid { get; }
    ushort ProgramNumber { get; }
    byte VersionNumber { get; }
    ushort PcrPid { get; }
    IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    IReadOnlyCollection<IProgramMapSection.IEsInfo> ElementaryStreams { get; }
}

public interface IEventInformationTable : IServiceInformationTable
{
    ushort ServiceId { get; }
    ushort TransportStreamId { get; }
    ushort OriginalNetworkId { get; }
    IReadOnlyCollection<IEventInformationSubtable> Subtables { get; }
    IReadOnlyCollection<IEventInformationSection.IEventInformation> Events { get; }
}

public interface IEventInformationSubtable
{
    byte TableId { get; }
    byte VersionNumber { get; }
    byte LastSectionNumber { get; }
    byte LastTableId { get; }
    IReadOnlyCollection<IEventInformationSegment> Segments { get; }
    IReadOnlyCollection<IEventInformationSection.IEventInformation> Events { get; }
}

public interface IEventInformationSegment
{
    byte FirstSectionNumber { get; }
    byte LastSectionNumber { get; }
    IReadOnlyCollection<IEventInformationSection.IEventInformation> Events { get; }
}