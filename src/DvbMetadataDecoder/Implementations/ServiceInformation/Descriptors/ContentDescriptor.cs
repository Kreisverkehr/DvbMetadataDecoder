using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class ContentDescriptor : Descriptor, IContentDescriptor
{
    private readonly List<ContentType> _contentTypes = [];
    public ContentDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        foreach (var entryData in data[2..].Chunk(2))
        {
            _contentTypes.Add(new(entryData));
        }
    }

    public IReadOnlyCollection<IContentDescriptor.IContentType> ContentTypes => _contentTypes.AsReadOnly();

    private class ContentType : IContentDescriptor.IContentType
    {
        public ContentType(ReadOnlySpan<byte> data)
        {
            Level1Type = (byte)(data[0] >> 4);
            Level2Type = (byte)(data[0] & 0x0F);
            UserByte = data[1];
        }

        public byte Level1Type { get; private set; }

        public byte Level2Type { get; private set; }

        public byte UserByte { get; private set; }
    }
}