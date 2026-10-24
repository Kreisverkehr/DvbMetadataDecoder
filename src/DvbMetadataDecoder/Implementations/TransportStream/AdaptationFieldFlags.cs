namespace Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

[Flags]
internal enum AdaptationFieldFlags : byte
{
    DiscontinuityIndicator = 0b10000000,
    RandomAccessIndicator = 0b01000000,
    ElementaryStreamPriorityIndicator = 0b00100000,
    Pcr = 0b00010000,
    Opcr = 0b00001000,
    SplicingPoint = 0b00000100,
    TransportPrivateData = 0b00000010,
    AdaptationFieldExtension = 0b00000001,
}