namespace Kreisverkehr.DvbMetadataDecoder;

public enum RunningStatus : byte
{
    Undefined = 0b000,
    NotRunning = 0b001,
    StartsInAFewSeconds = 0b010,
    Pausing = 0b011,
    Running = 0b100,
    ServiceOffAir = 0b101,
    Reserved1 = 0b110,
    Reserved2 = 0b111,
}