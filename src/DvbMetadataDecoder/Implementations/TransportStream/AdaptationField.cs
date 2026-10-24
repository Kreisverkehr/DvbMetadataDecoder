namespace Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

internal class AdaptationField : ITransportStreamPackageAdaptationField
{
    private const int PCR_FIELD_LENGTH = 6;

    private AdaptationFieldFlags _flags;

    internal AdaptationField(ReadOnlySpan<byte> data)
    {
        _flags = (AdaptationFieldFlags)data[0];

        int nextFieldIndex = 1;

        if (_flags.HasFlag(AdaptationFieldFlags.Pcr))
        {
            ProgramClockReference = ParsePcr(data.Slice(nextFieldIndex, PCR_FIELD_LENGTH));
            nextFieldIndex += PCR_FIELD_LENGTH;
        }

        if (_flags.HasFlag(AdaptationFieldFlags.Opcr))
        {
            OriginalProgramClockReference = ParsePcr(data.Slice(nextFieldIndex, PCR_FIELD_LENGTH));
            nextFieldIndex += PCR_FIELD_LENGTH;
        }

        if(_flags.HasFlag(AdaptationFieldFlags.SplicingPoint))
        {
            SpliceCountdown = (sbyte)data[nextFieldIndex];
            nextFieldIndex++;
        }

        if(_flags.HasFlag(AdaptationFieldFlags.TransportPrivateData))
        {
            TransportPrivateData = data.Slice(nextFieldIndex + 1, data[nextFieldIndex]).ToArray();
            nextFieldIndex += data[nextFieldIndex] + 1;
        }

        if(_flags.HasFlag(AdaptationFieldFlags.AdaptationFieldExtension))
        {
            Extension = new AdaptationFieldExtension(data.Slice(nextFieldIndex + 1, data[nextFieldIndex]));
        }
    }

    private ulong ParsePcr(ReadOnlySpan<byte> data)
    {
        ulong pcr = 0;
        pcr += (uint)data[0] << 25;
        pcr += (uint)data[1] << 17;
        pcr += (uint)data[2] << 9;
        pcr += (uint)data[3] << 1;
        pcr += (uint)data[4] >> 7;

        pcr *= 300;

        pcr += (uint)((data[4] & 0b00000001) << 8) + data[5];

        return pcr;
    }

    public bool DiscontinuityIndicator => _flags.HasFlag(AdaptationFieldFlags.DiscontinuityIndicator);

    public bool RandomAccessIndicator => _flags.HasFlag(AdaptationFieldFlags.RandomAccessIndicator);

    public bool ElementaryStreamPriorityIndicator => _flags.HasFlag(AdaptationFieldFlags.ElementaryStreamPriorityIndicator);

    public ulong? ProgramClockReference { get; private set; } = null;

    public ulong? OriginalProgramClockReference { get; private set; } = null;

    public sbyte? SpliceCountdown { get; private set; } = null;

    public ReadOnlyMemory<byte>? TransportPrivateData { get; private set; } = null;

    public ITransportStreamPackageAdaptationFieldExtension? Extension { get; private set; } = null;
}
