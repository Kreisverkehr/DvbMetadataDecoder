namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class CopyrightDescriptor : Descriptor, ICopyrightDescriptor
{
    public CopyrightDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        CopyrightIdentifier = (uint)((data[2] << 24) | (data[3] << 16) | (data[4] << 8) | data[5]);
        AdditionalCopyrightInfo = data[6..].ToArray();
    }

    public uint CopyrightIdentifier { get; private set; }

    public ReadOnlyMemory<byte> AdditionalCopyrightInfo { get; private set; }
}