using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IDvbMetadataRepository
{
    IProgramAssociationTable ProgramAssociationTable { get; }
    IReadOnlyCollection<IProgramMapTable> ProgramMapTables { get; }
    IReadOnlyCollection<IEventInformationTable> PresentFollowingActualTransportStreamTables { get; }
    IReadOnlyCollection<IEventInformationTable> PresentFollowingOtherTransportStreamTables { get; }
    IReadOnlyCollection<IEventInformationTable> ScheduleActualTransportStreamTables { get; }
    IReadOnlyCollection<IEventInformationTable> ScheduleOtherTransportStreamTables { get; }
}

public class DvbMetadataRepository : IDvbMetadataRepository, IAsyncDisposable
{
    private readonly IProgramAssociationTable _pat;
    private readonly ProgramMapTableCollector _programMapTableCollector;
    private readonly EventInformationTableCollector _eventInformationTableCollector;

    public DvbMetadataRepository(IProgramAssociationTable pat, IServiceInformationSectionFactory sectionFactory)
    {
        _pat = pat;
        _programMapTableCollector = new(sectionFactory, pat);
        _eventInformationTableCollector = new(sectionFactory);
    }

    public IProgramAssociationTable ProgramAssociationTable => _pat;

    public IReadOnlyCollection<IProgramMapTable> ProgramMapTables => _programMapTableCollector.ProgramMapTables;

    public IReadOnlyCollection<IEventInformationTable> PresentFollowingActualTransportStreamTables =>
        _eventInformationTableCollector.PresentFollowingActualTransportStreamTables;

    public IReadOnlyCollection<IEventInformationTable> PresentFollowingOtherTransportStreamTables =>
        _eventInformationTableCollector.PresentFollowingOtherTransportStreamTables;

    public IReadOnlyCollection<IEventInformationTable> ScheduleActualTransportStreamTables =>
        _eventInformationTableCollector.ScheduleActualTransportStreamTables;

    public IReadOnlyCollection<IEventInformationTable> ScheduleOtherTransportStreamTables =>
        _eventInformationTableCollector.ScheduleOtherTransportStreamTables;

    public async ValueTask DisposeAsync()
    {
        await _programMapTableCollector.DisposeAsync().ConfigureAwait(false);
        await _eventInformationTableCollector.DisposeAsync().ConfigureAwait(false);
    }
}