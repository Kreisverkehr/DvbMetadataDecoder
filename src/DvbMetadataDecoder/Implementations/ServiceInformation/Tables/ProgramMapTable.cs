namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

internal sealed class ProgramMapTable : IProgramMapTable
{
    public ProgramMapTable(
        ushort programMapPid,
        ushort programNumber,
        byte versionNumber,
        ushort pcrPid,
        IReadOnlyCollection<IServiceInformationDescriptor> descriptors,
        IReadOnlyCollection<IProgramMapSection.IEsInfo> elementaryStreams)
    {
        ProgramMapPid = programMapPid;
        ProgramNumber = programNumber;
        VersionNumber = versionNumber;
        PcrPid = pcrPid;
        Descriptors = descriptors;
        ElementaryStreams = elementaryStreams;
    }

    public ushort ProgramMapPid { get; }

    public ushort ProgramNumber { get; }

    public byte VersionNumber { get; }

    public ushort PcrPid { get; }

    public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors { get; }

    public IReadOnlyCollection<IProgramMapSection.IEsInfo> ElementaryStreams { get; }
}