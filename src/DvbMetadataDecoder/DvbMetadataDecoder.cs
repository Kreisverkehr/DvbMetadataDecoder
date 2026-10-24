namespace Kreisverkehr.DvbMetadataDecoder;

public interface IDvbMetadataDecoder
{
    ValueTask FeedAsync(ReadOnlyMemory<byte> data);
}

public class DvbMetadataDecoder : IDvbMetadataDecoder
{
    public async ValueTask FeedAsync(ReadOnlyMemory<byte> data)
    {
        throw new NotImplementedException();
    }
}