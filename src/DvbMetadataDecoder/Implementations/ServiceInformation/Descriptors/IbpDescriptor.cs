namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class IbpDescriptor : Descriptor, IIbpDescriptor
{
    public IbpDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        GopClosed = (data[2] & 0b10000000) != 0;
        GopIdentical = (data[2] & 0b01000000) != 0;
        MaxGopLength = (uint)((data[2] & 0b00111111) << 8 | data[3]);
    }

    public bool GopClosed { get; private set; }
    public bool GopIdentical { get; private set; }
    public uint MaxGopLength { get; private set; }
}