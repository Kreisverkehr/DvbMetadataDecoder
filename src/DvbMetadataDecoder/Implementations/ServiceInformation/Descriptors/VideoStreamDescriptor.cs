namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class VideoStreamDescriptor : Descriptor, IVideoStreamDescriptor
{
    public VideoStreamDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        MultipleFramerate = (data[2] & 0b10000000) != 0;
        FrameRateCode = (byte)((data[2] & 0b01111000) >> 3);
        Mpeg1Only = (data[2] & 0b00000100) != 0;
        ConstrainedParameter = (data[2] & 0b00000010) != 0;
        StillPicture = (data[2] & 0b00000001) != 0;
        if(!Mpeg1Only)
        {
            ProfileAndLevelIndication = data[3];
            ChromaFormat = (byte)((data[4] & 0b11000000) >> 6);
            FramerateExtension = (data[4] & 0b00100000) != 0;
        }
    }

    public bool MultipleFramerate { get; private set; }

    public byte FrameRateCode { get; private set; }

    public bool Mpeg1Only { get; private set; }

    public bool ConstrainedParameter { get; private set; }

    public bool StillPicture { get; private set; }

    public byte? ProfileAndLevelIndication { get; private set; }

    public byte? ChromaFormat { get; private set; }

    public bool? FramerateExtension { get; private set; }
}