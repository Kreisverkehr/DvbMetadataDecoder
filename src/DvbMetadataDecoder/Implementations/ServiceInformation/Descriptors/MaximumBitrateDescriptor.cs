namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class MaximumBitrateDescriptor : Descriptor, IMaximumBitrateDescriptor
{
    public MaximumBitrateDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        MaximumBitrate = (uint)(data[2] & 0b00111111) << 16 | (uint)(data[3] << 8) | data[4];
    }

    public uint MaximumBitrate { get; private set; }
}