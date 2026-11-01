namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class TargetBackgroundGridDescriptor : Descriptor, ITargetBackgroundGridDescriptor
{
    public TargetBackgroundGridDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        HorizontalSize = (ushort)((data[2] << 6) | (data[3] >> 2));
        VerticalSize = (ushort)(((data[3] & 0b00000011) << 14) | (data[4] << 6) | (data[5] >> 2));
        AspectRatioInformation = (byte)(data[5] & 0b00000011);
    }

    public ushort HorizontalSize { get; private set; }

    public ushort VerticalSize { get; private set; }

    public byte AspectRatioInformation { get; private set; }
}