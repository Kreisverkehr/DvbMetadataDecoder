namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class AudioStreamDescriptor : Descriptor, IAudioStreamDescriptor
{
    public AudioStreamDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        FreeFormat = (data[2] & 0b10000000) != 0;
        Id = (data[2] & 0b01000000) != 0;
        Layer = (byte)((data[2] & 0b00110000) >> 4);
        VariableRateAudio = (data[2] & 0b00001000) != 0;
    }

    public bool FreeFormat { get; private set; }

    public bool Id { get; private set; }

    public byte Layer { get; private set; }

    public bool VariableRateAudio { get; private set; }
}