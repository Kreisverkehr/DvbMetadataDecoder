using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IDvbMetadataRepository
{
    IProgramAssociationTable ProgramAssociationTable { get; }
    IReadOnlyCollection<IProgramMapTable> ProgramMapTables { get; }
}

public class DvbMetadataRepository : IDvbMetadataRepository, IAsyncDisposable
{
    private readonly IProgramAssociationTable _pat;
    private readonly ProgramMapTableCollector _programMapTableCollector;

    public DvbMetadataRepository(IProgramAssociationTable pat, IServiceInformationSectionFactory sectionFactory)
    {
        _pat = pat;
        _programMapTableCollector = new(sectionFactory, pat);
    }

    public IProgramAssociationTable ProgramAssociationTable => _pat;

    public IReadOnlyCollection<IProgramMapTable> ProgramMapTables => _programMapTableCollector.ProgramMapTables;

    public ValueTask DisposeAsync() => _programMapTableCollector.DisposeAsync();
}