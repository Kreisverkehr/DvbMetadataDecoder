using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

internal class PackageFactory : ITransportStreamPackageFactory, IAsyncDisposable
{
    private readonly Channel<ITransportStreamPackage> _pkgChannel = Channel.CreateUnbounded<ITransportStreamPackage>(new()
    {
        AllowSynchronousContinuations = true,
        SingleReader = true,
        SingleWriter = true
    });

    public ValueTask DisposeAsync()
    {
        _pkgChannel.Writer.Complete();
        return ValueTask.CompletedTask;
    }

    public async ValueTask FeedAsync(ReadOnlyMemory<byte> data)
    {
        // TODO: Implement sync finding and continuation of left over bytes. We'll trust
        // that we get complete, perfectly aligned 188-byte packets here for now.

        for (var offset = 0; offset + 188 <= data.Length; offset += 188)
            await _pkgChannel.Writer.WriteAsync(new Package(data.Slice(offset, 188).Span));
    }

    public ValueTask<ITransportStreamPackage> ReadAsync(CancellationToken cancellationToken = default)
    {
        return _pkgChannel.Reader.ReadAsync(cancellationToken);
    }
}