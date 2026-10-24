using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface ITransportStreamPackageFactory
{
    ValueTask FeedAsync(ReadOnlyMemory<byte> data);

    ChannelReader<ITransportStreamPackage> Subscribe();
}
