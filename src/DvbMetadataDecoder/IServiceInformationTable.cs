namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationTable;

public interface IProgramAssociationTable : IServiceInformationTable
{
    byte VersionNumber { get; }
    IReadOnlyCollection<ushort> NetworkPids { get; }
    IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations { get; }
    IEnumerable<ushort> ProgramMapPids { get; }
}