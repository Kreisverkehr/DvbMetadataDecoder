namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class PrivateDataIndicatorDescriptor : Descriptor, IPrivateDataIndicatorDescriptor
{
    public PrivateDataIndicatorDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        PrivateDataIndicator = (uint)(data[2] << 24) | (uint)(data[3] << 16) | (uint)(data[4] << 8) | data[5];
    }

    public uint PrivateDataIndicator { get; private set; }
}