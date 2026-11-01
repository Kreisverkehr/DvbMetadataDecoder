namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class ConditionalAccessDescriptor : Descriptor, IConditionalAccessDescriptor
{
    public ConditionalAccessDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        CaSystemId = (ushort)((data[2] << 8) | data[3]);
        CaPid = (ushort)(((data[4] & 0b00011111) << 8) | data[5]);
        PrivateData = data[6..].ToArray();
    }

    public ushort CaSystemId { get; private set; }

    public ushort CaPid { get; private set; }

    public ReadOnlyMemory<byte> PrivateData { get; private set; }
}