namespace Kreisverkehr.DvbMetadataDecoder;

public interface ITransportStreamPackageFactory
{
    ValueTask FeedAsync(ReadOnlyMemory<byte> data);

    ValueTask<ITransportStreamPackage> ReadAsync(CancellationToken cancellationToken = default);
}
