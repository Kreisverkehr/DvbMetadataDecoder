using System.Threading.Channels;
using DvbMetadataDecoder.Implementations.ServiceInformation;
using Microsoft.VisualBasic;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;

internal class TableFactory : IServiceInformationTableFactory, IAsyncDisposable
{
    private readonly List<Channel<IServiceInformationTable>> _subscribers = [];
    private readonly ChannelReader<ITransportStreamPackage> _packageSubscription;
    private readonly ITransportStreamPackageFactory _packageFactory;
    private readonly Task _worker;
    private readonly Dictionary<ushort, SectionAssembler> _assemblers = [];

    public TableFactory(ITransportStreamPackageFactory packageFactory)
    {
        _packageFactory = packageFactory;
        _packageSubscription = _packageFactory.Subscribe();
        _worker = ProcessPackages();
    }

    private async Task ProcessPackages(CancellationToken cancellationToken = default)
    {
        while (await _packageSubscription.WaitToReadAsync(cancellationToken) && _packageSubscription.TryRead(out var pkg))
        {
            if ((pkg.PacketIdentifier >= 0x10 && pkg.PacketIdentifier <= 0x1F) || pkg.PacketIdentifier <= 0x3)
                GetAssembler(pkg.PacketIdentifier).Process(pkg);
        }
    }

    private async Task ProcessSection(ushort pid, ReadOnlyMemory<byte> sectionData)
    {
        Table table = Table.CreateTable(pid, sectionData.Span);
        foreach(var subscriber in _subscribers)
            await subscriber.Writer.WriteAsync(table);
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

    public ChannelReader<IServiceInformationTable> Subscribe()
    {
        var subscriber = Channel.CreateUnbounded<IServiceInformationTable>(new()
        {
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });
        _subscribers.Add(subscriber);
        return subscriber.Reader;
    }
}