namespace Kreisverkehr.DvbMetadataDecoder;

public interface IDvbMetadataRepository
{
    IProgramAssociationTable ProgramAssociationTable { get; }
}

public class DvbMetadataRepository : IDvbMetadataRepository
{
    private readonly IProgramAssociationTable _pat;

    public DvbMetadataRepository(IProgramAssociationTable pat)
    {
        _pat = pat;
    }

    public IProgramAssociationTable ProgramAssociationTable => _pat;
}