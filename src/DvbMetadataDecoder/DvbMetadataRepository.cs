using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Tables;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IDvbMetadataRepository
{
    IProgramAssociationTable ProgramAssociationTable { get; }
    IReadOnlyCollection<IProgramMapTable> ProgramMapTables { get; }
    IReadOnlyCollection<IEventInformationTable> ActualTransportStreamPresentFollowingEventInformationTables { get; }
    IReadOnlyCollection<IEventInformationTable> OtherTransportStreamPresentFollowingEventInformationTables { get; }
    IReadOnlyCollection<IEventInformationTable> ActualTransportStreamScheduleEventInformationTables { get; }
    IReadOnlyCollection<IEventInformationTable> OtherTransportStreamScheduleEventInformationTables { get; }
    IReadOnlyCollection<IServiceDescriptionTable> ActualTransportStreamServiceDescriptionTables { get; }
    IReadOnlyCollection<IServiceDescriptionTable> OtherTransportStreamServiceDescriptionTables { get; }
}

public class DvbMetadataRepository : IDvbMetadataRepository, IAsyncDisposable
{
    private readonly IProgramAssociationTable _pat;
    private readonly ProgramMapTableCollector _programMapTableCollector;
    private readonly EventInformationTableCollector _eventInformationTableCollector;
    private readonly ServiceDescriptionTableCollector _serviceDescriptionTableCollector;

    public DvbMetadataRepository(IProgramAssociationTable pat, IServiceInformationSectionFactory sectionFactory)
    {
        _pat = pat;
        _programMapTableCollector = new(sectionFactory, pat);
        _eventInformationTableCollector = new(sectionFactory);
        _serviceDescriptionTableCollector = new(sectionFactory);
    }

    public IProgramAssociationTable ProgramAssociationTable => _pat;

    public IReadOnlyCollection<IProgramMapTable> ProgramMapTables => _programMapTableCollector.ProgramMapTables;

    public IReadOnlyCollection<IEventInformationTable> ActualTransportStreamPresentFollowingEventInformationTables =>
        _eventInformationTableCollector.ActualTransportStreamPresentFollowingEventInformationTables;

    public IReadOnlyCollection<IEventInformationTable> OtherTransportStreamPresentFollowingEventInformationTables =>
        _eventInformationTableCollector.OtherTransportStreamPresentFollowingEventInformationTables;

    public IReadOnlyCollection<IEventInformationTable> ActualTransportStreamScheduleEventInformationTables =>
        _eventInformationTableCollector.ActualTransportStreamScheduleEventInformationTables;

    public IReadOnlyCollection<IEventInformationTable> OtherTransportStreamScheduleEventInformationTables =>
        _eventInformationTableCollector.OtherTransportStreamScheduleEventInformationTables;

    public IReadOnlyCollection<IServiceDescriptionTable> ActualTransportStreamServiceDescriptionTables =>
        _serviceDescriptionTableCollector.ActualTransportStreamServiceDescriptionTables;

    public IReadOnlyCollection<IServiceDescriptionTable> OtherTransportStreamServiceDescriptionTables =>
        _serviceDescriptionTableCollector.OtherTransportStreamServiceDescriptionTables;

    public async ValueTask DisposeAsync()
    {
        await _programMapTableCollector.DisposeAsync().ConfigureAwait(false);
        await _eventInformationTableCollector.DisposeAsync().ConfigureAwait(false);
        await _serviceDescriptionTableCollector.DisposeAsync().ConfigureAwait(false);
    }
}