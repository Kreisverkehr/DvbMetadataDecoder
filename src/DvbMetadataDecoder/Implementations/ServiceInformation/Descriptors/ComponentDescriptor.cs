using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class ComponentDescriptor : Descriptor, IComponentDescriptor
{
    public ComponentDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        StreamContentExt = (byte)(data[2] >> 4);
        StreamContent = (byte)(data[2] & 0b00001111 );
        ComponentType = data[3];
        ComponentTag = data[4];
        Iso639Code = Encoding.ASCII.GetString(data[5..8]);
        Description = Encoding.DvbText.GetString(data[8..]);
    }

    public byte StreamContentExt { get; private set; }

    public byte StreamContent { get; private set; }

    public byte ComponentType { get; private set; }

    public byte ComponentTag { get; private set; }

    public string Iso639Code { get; private set; }

    public string Description { get; private set; }
}