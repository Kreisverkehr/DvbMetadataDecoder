using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationTableFactory
{
    ChannelReader<IServiceInformationTable> Subscribe();
}