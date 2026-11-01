namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class VideoWindowDescriptor : Descriptor, IVideoWindowDescriptor
{
    public VideoWindowDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        HorizontalOffset = (ushort)((data[2] << 6) | (data[3] >> 2));
        VerticalOffset = (ushort)(((data[3] & 0b00000011) << 14) | (data[4] << 6) | (data[5] >> 2));
        WindowPriority = (byte)(data[5] & 0b00000011);
    }

    public ushort HorizontalOffset { get; private set; }

    public ushort VerticalOffset { get; private set; }

    public byte WindowPriority { get; private set; }
}