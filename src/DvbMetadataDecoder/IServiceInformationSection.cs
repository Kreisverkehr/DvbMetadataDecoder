namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationSection
{
    byte TableId { get; }
    ushort TransportStreamId { get; }
    byte VersionNumber { get; }
    bool IsCurrent { get; }
    byte SectionNumber { get; }
    byte LastSectionNumber { get; }
}

public interface IProgramAssociationSection : IServiceInformationSection
{
    IReadOnlyCollection<ushort> NetworkPids { get; }
    IReadOnlyCollection<(ushort ProgramNumber, ushort ProgramMapPid)> PidAssociations { get; }
}