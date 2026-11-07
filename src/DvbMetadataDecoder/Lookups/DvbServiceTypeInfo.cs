namespace Kreisverkehr.DvbMetadataDecoder.Lookups;

public sealed record DvbServiceTypeInfo(
    string Description,
    bool HasVideo = false,
    bool HasAudio = false,
    bool IsHD = false,
    bool IsUHD = false,
    bool IsRadio = false,
    bool IsTeletext = false,
    string? VideoCodec = null
);
