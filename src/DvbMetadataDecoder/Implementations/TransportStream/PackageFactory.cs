using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

internal class PackageFactory : ITransportStreamPackageFactory, IAsyncDisposable
{
    private readonly List<Channel<ITransportStreamPackage>> _subscriber = [];

    public ValueTask DisposeAsync()
    {
        foreach (var subscriberChannel in _subscriber)
            subscriberChannel.Writer.Complete();
        return ValueTask.CompletedTask;
    }

    public async ValueTask FeedAsync(ReadOnlyMemory<byte> data)
    {
        // TODO: Implement sync finding and continuation of left over bytes. We'll trust
        // that we get complete, perfectly aligned 188-byte packets here for now.

        for (var offset = 0; offset + 188 <= data.Length; offset += 188)
            foreach (var subscriberChannel in _subscriber)
                await subscriberChannel.Writer.WriteAsync(new Package(data.Slice(offset, 188).Span));
    }

    public ChannelReader<ITransportStreamPackage> Subscribe()
    {
        var subscription = Channel.CreateUnbounded<ITransportStreamPackage>(new()
        {
            SingleWriter = true,
            AllowSynchronousContinuations = true
        });
        _subscriber.Add(subscription);
        return subscription.Reader;
    }
}