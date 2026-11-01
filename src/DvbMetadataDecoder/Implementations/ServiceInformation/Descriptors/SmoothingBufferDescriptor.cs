namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class SmoothingBufferDescriptor : Descriptor, ISmoothingBufferDescriptor
{
    public SmoothingBufferDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        SbLeakRate = (uint)(data[2] & 0b00111111) << 16 | (uint)(data[3] << 8) | data[4];
        SbSize = (uint)(data[5] & 0b00111111) << 16 | (uint)(data[6] << 8) | data[7];
    }

    public uint SbLeakRate { get; private set; }

    public uint SbSize { get; private set; }
}