using System.Threading.Channels;

namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationSectionFactory
{
    ChannelReader<(ushort Pid, IServiceInformationSection Section)> Subscribe();
}