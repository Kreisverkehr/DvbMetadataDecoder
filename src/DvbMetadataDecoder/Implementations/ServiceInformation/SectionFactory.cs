using System.Threading.Channels;
using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;
using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Sections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;

internal class SectionFactory : IServiceInformationSectionFactory, IAsyncDisposable
{
    private readonly List<Channel<(ushort Pid, IServiceInformationSection Section)>> _subscribers = [];
    private readonly ChannelReader<ITransportStreamPackage> _packageSubscription;
    private readonly Lazy<IDvbMetadataRepository> _respository;
    private readonly ITransportStreamPackageFactory _packageFactory;
    private readonly Task _worker;
    private readonly Dictionary<ushort, SectionAssembler> _assemblers = [];

    private IEnumerable<ushort> ProgramMapPids => _respository.Value.ProgramAssociationTable.ProgramMapPids;

    public SectionFactory(ITransportStreamPackageFactory packageFactory, IServiceProvider provider)
    {
        _respository = new(provider.GetRequiredService<IDvbMetadataRepository>);
        _packageFactory = packageFactory;
        _packageSubscription = _packageFactory.Subscribe();
        _worker = ProcessPackages();
    }

    private async Task ProcessPackages(CancellationToken cancellationToken = default)
    {
        while (await _packageSubscription.WaitToReadAsync(cancellationToken) && _packageSubscription.TryRead(out var pkg))
        {
            if ((pkg.PacketIdentifier >= 0x10 && pkg.PacketIdentifier <= 0x1F) || pkg.PacketIdentifier <= 0x3 || ProgramMapPids.Contains(pkg.PacketIdentifier))
                GetAssembler(pkg.PacketIdentifier).Process(pkg);
        }
    }

    private async Task ProcessSection(ushort pid, ReadOnlyMemory<byte> sectionData)
    {
        var tableId = sectionData.Span[0];

        if(pid == 0x00 && tableId == 0x00)
        {
            await BroadcastNewSection(pid, new ProgramAssociationSection(sectionData.Span));
            return;
        }

        if(tableId == 0x02 && ProgramMapPids.Contains(pid))
        {
            await BroadcastNewSection(pid, new ProgramMapSection(sectionData.Span));
            return;
        }

        if(pid == 0x01 && tableId == 0x01)
        {
            await BroadcastNewSection(pid, new ConditionalAccessSection(sectionData.Span));
            return;
        }

        if(pid == 0x02 && tableId == 0x03)
        {
            await BroadcastNewSection(pid, new TransportStreamDescriptionSection(sectionData.Span));
            return;
        }

        if(pid == 0x10 && (tableId == 0x40 || tableId == 0x41))
        {
            await BroadcastNewSection(pid, new NetworkInformationSection(sectionData.Span));
            return;
        }

        if(pid == 0x11 && (tableId == 0x42 || tableId == 0x46))
        {
            await BroadcastNewSection(pid, new ServiceDescriptionSection(sectionData.Span));
            return;
        }

        if(pid == 0x12 && tableId >= 0x4E && tableId <= 0x6F)
        {
            await BroadcastNewSection(pid, new EventInformationSection(sectionData.Span));
            return;            
        }

        await BroadcastNewSection(pid, new UnknownSection(sectionData.Span));
    }

    private async Task BroadcastNewSection(ushort pid, Section section)
    {
        foreach(var subscriber in _subscribers)
            await subscriber.Writer.WriteAsync((pid, section));
    }

    private SectionAssembler GetAssembler(ushort pid)
    {
        if (!_assemblers.ContainsKey(pid))
            _assemblers.Add(pid, new(async sectionData => await ProcessSection(pid, sectionData)));

        return _assemblers[pid];
    }

    public ValueTask DisposeAsync()
    {
        foreach (var subscriber in _subscribers)
            subscriber.Writer.Complete();

        return ValueTask.CompletedTask;
    }

    public ChannelReader<(ushort Pid, IServiceInformationSection Section)> Subscribe()
    {
        var subscriber = Channel.CreateUnbounded<(ushort Pid, IServiceInformationSection Section)>(new()
        {
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });
        _subscribers.Add(subscriber);
        return subscriber.Reader;
    }
}