namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class StdDescriptor : Descriptor, IStdDescriptor
{
    public StdDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        LeakValid = (data[2] & 0b00000001) != 0;
    }

    public bool LeakValid { get; private set; }
}