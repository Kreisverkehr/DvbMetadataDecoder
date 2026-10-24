using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationSectionFactory
{
    ChannelReader<IServiceInformationSection> Subscribe();
}