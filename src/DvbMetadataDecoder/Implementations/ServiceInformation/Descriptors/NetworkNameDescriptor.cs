using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class NetworkNameDescriptor : Descriptor, INetworkNameDescriptor
{
    public NetworkNameDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        Text = Encoding.DvbText.GetString(data[2..]);
    }

    public string Text { get; private set; }
}