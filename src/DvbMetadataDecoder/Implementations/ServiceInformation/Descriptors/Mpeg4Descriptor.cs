namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class Mpeg4Descriptor : Descriptor, IMpeg4Descriptor
{
    public Mpeg4Descriptor(ReadOnlySpan<byte> data) : base(data)
    {
        ProfileAndLevel = data[2];
    }

    public byte ProfileAndLevel { get; private set; }
}

internal class Mpeg4VideoDescriptor : Mpeg4Descriptor, IMpeg4VideoDescriptor
{
    public Mpeg4VideoDescriptor(ReadOnlySpan<byte> data) : base(data) { }

    public byte VisualProfileAndLevel => ProfileAndLevel;
}

internal class Mpeg4AudioDescriptor : Mpeg4Descriptor, IMpeg4AudioDescriptor
{
    public Mpeg4AudioDescriptor(ReadOnlySpan<byte> data) : base(data) { }

    public byte AudioProfileAndLevel => ProfileAndLevel;
}