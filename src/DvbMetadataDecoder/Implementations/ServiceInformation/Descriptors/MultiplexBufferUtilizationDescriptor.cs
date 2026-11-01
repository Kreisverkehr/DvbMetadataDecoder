namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class MultiplexBufferUtilizationDescriptor : Descriptor, IMultiplexBufferUtilizationDescriptor
{
    public MultiplexBufferUtilizationDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        BoundValid = (data[2] & 0b10000000) != 0;
        LtwOffsetLowerBound = (ushort)((data[2] & 0b01111111) << 8 | data[3]);
        LtwOffsetUpperBound = (ushort)(((data[4] & 0b01111111) << 8) | data[5]);
    }

    public bool BoundValid {get; private set; }

    public ushort LtwOffsetLowerBound {get; private set; }

    public ushort LtwOffsetUpperBound {get; private set; }
}