namespace Kreisverkehr.DvbMetadataDecoder;

public interface IDvbMetadataDecoder
{
    ValueTask FeedAsync(ReadOnlyMemory<byte> data);
}

public class DvbMetadataDecoder : IDvbMetadataDecoder
{
    private readonly ITransportStreamPackageFactory _pkgFactory;

    public DvbMetadataDecoder(ITransportStreamPackageFactory pkgFactory)
    {
        _pkgFactory = pkgFactory;
    }

    public ValueTask FeedAsync(ReadOnlyMemory<byte> data) =>
        _pkgFactory.FeedAsync(data);
}