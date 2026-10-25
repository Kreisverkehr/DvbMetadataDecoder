using System.Dynamic;

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

public interface IProgramMapSection : IServiceInformationSection
{
    ushort ProgramNumber { get; }
    ushort PcrPid { get; }
    IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    IReadOnlyCollection<IEntry> Entries { get; }
    public interface IEntry
    {
        byte StreamType { get; }
        ushort ElementaryPid { get; }
        IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }
    }
}