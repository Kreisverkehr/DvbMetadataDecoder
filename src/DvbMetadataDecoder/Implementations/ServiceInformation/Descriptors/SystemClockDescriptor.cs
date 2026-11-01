namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class SystemClockDescriptor : Descriptor, ISystemClockDescriptor
{
    public SystemClockDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        ExternalClockReference = (data[2] & 0b10000000) != 0;
        ClockAccuracyInteger = (byte)(data[2] & 0b00111111);
        ClockAccuracyExponent = (byte)((data[3] & 0b11100000) >> 5);
    }

    public bool ExternalClockReference { get; private set; }

    public byte ClockAccuracyInteger { get; private set; }

    public byte ClockAccuracyExponent { get; private set; }
}