namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationTable;

public interface IProgramAssociationTable : IServiceInformationTable
{
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