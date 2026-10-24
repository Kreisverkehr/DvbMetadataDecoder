namespace Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

internal class AdaptationFieldExtension : ITransportStreamPackageAdaptationFieldExtension
{
    private AdaptationFieldExtensionFlags _flags;
    internal AdaptationFieldExtension(ReadOnlySpan<byte> data)
    {
        _flags = (AdaptationFieldExtensionFlags)data[0];

        int nextSetStart = 1;

        if(_flags.HasFlag(AdaptationFieldExtensionFlags.LegalTimeWindow))
        {
            LegalTimeWindowValid = (data[nextSetStart] & 0b10000000) != 0;
            LegalTimeWindowOffset = (ushort)(((data[nextSetStart] & 0b01111111) << 8) + data[nextSetStart]);
            nextSetStart += 2;
        }

        if(_flags.HasFlag(AdaptationFieldExtensionFlags.Piecewise))
        {
            PiecewiseRate = (uint)((data[nextSetStart] & 0b00111111) << 16) + data[nextSetStart + 1] << 8 + data[nextSetStart + 2];
            nextSetStart += 3;
        }

        if(_flags.HasFlag(AdaptationFieldExtensionFlags.SeamlessSplice))
        {
            SpliceType = (byte)(data[nextSetStart] & 0b11000000);
            DtsNextAccessUnit = (uint)((data[nextSetStart] & 0b00111111) << 32) + 
                data[nextSetStart + 1] << 24 + 
                data[nextSetStart + 2] << 16 +
                data[nextSetStart + 3] << 8 +
                data[nextSetStart + 1];
        }
    }

    public bool? LegalTimeWindowValid { get; private set; } = null;
    public ushort? LegalTimeWindowOffset { get; private set; } = null;
    public uint? PiecewiseRate { get; private set; } = null;
    public byte? SpliceType { get; private set; } = null;
    public ulong? DtsNextAccessUnit { get; private set; } = null;
}

[Flags]
internal enum AdaptationFieldExtensionFlags : byte
{
    LegalTimeWindow = 0b10000000,
    Piecewise = 0b01000000,
    SeamlessSplice = 0b00100000
}