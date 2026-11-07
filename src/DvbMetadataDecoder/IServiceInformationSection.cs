using System.Dynamic;
using Kreisverkehr.DvbMetadataDecoder.Lookups;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationSection
{
    byte TableId { get; }
    byte VersionNumber { get; }
    bool IsCurrent { get; }
    byte SectionNumber { get; }
    byte LastSectionNumber { get; }
}

public interface IUnknownSection : IServiceInformationSection
{
    ReadOnlyMemory<byte> SectionData { get; }
}

public interface IProgramAssociationSection : IServiceInformationSection
{
    ushort TransportStreamId { get; }
    IReadOnlyCollection<ushort> NetworkPids { get; }
    IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations { get; }
}

public interface IConditionalAccessSection : IServiceInformationSection
{
    IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
}

public interface IProgramMapSection : IServiceInformationSection
{
    ushort ProgramNumber { get; }
    ushort PcrPid { get; }
    IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    IReadOnlyCollection<IEsInfo> EsInfos { get; }
    public interface IEsInfo
    {
        StreamTypeInfo StreamType { get; }
        ushort ElementaryPid { get; }
        IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    }
}

public interface ITransportStreamDescriptionSection : IServiceInformationSection
{
    IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
}

public interface INetworkInformationSection : IServiceInformationSection
{
    ushort NetworkId { get; }
    IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    IReadOnlyCollection<INetworkInfo> NetworkInfos { get; }
    public interface INetworkInfo
    {
        ushort TransportStreamId { get; }
        ushort OriginalNetworkId { get; }
        IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    }
}

public interface IEventInformationSection : IServiceInformationSection
{
    ushort ServiceId { get; }
    ushort TransportStreamId { get; }
    ushort OriginalNetworkId { get; }
    byte SegmentLastSectionNumber { get; }
    byte LastTableId { get; }
    IReadOnlyCollection<IEventInformation> Events { get; }
    public interface IEventInformation
    {
        ushort EventId { get; }
        DateTime? StartTime { get; }
        TimeSpan Duration { get; }
        RunningStatus RunningStatus { get; }
        bool FreeCaMode { get; }
        IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    }
}