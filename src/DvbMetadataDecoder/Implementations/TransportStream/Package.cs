namespace Kreisverkehr.DvbMetadataDecoder.Implementations.TransportStream;

internal class Package : ITransportStreamPackage
{
    private const ushort TS_PACKAGE_LEN = 188;
    private const byte TS_SYNC_BYTE = 0x47;

    private bool _tei;
    private bool _pusi;
    private bool _transportPriority;
    private ushort _pid;
    private byte _tsc;
    private byte _adaptationFieldControl;
    private byte _continuityCounter;

    internal Package(ReadOnlySpan<byte> data)
    {
        if (data.Length != TS_PACKAGE_LEN)
            throw new ArgumentException($"Package data is {data.Length} bytes. Expected exactly {TS_PACKAGE_LEN}", nameof(data));

        if (data[0] != TS_SYNC_BYTE)
            throw new FormatException($"Package must start with sync byte. Found {data[0]:X2}. Expected: {TS_SYNC_BYTE:X2}");

        _tei = (data[1] & 0b10000000) != 0;
        _pusi = (data[1] & 0b01000000) != 0;
        _transportPriority = (data[1] & 0b00100000) != 0;
        _pid = (ushort)(((data[1] & 0b00011111) << 8) + data[2]);
        _tsc = (byte)(data[3] >> 6);
        _adaptationFieldControl = (byte)((data[3] >> 4) & 0b00000011);
        _continuityCounter = (byte)(data[3] & 0b00001111);

        int payloadStartIndex = 4;
        bool hasPayload = (_adaptationFieldControl & 0b01) != 0;
        bool hasAdaptationField = (_adaptationFieldControl & 0b10) != 0;
        if (hasAdaptationField)
        {
            payloadStartIndex += data[4] + 1;
            AdaptationField = new AdaptationField(data.Slice(5, data[4]));
        }

        if (hasPayload)
        {
            Payload = data.Slice(payloadStartIndex).ToArray();

        }
    }

    public bool TransportErrorIndicator => _tei;
    public bool PayloadUnitStartIndicator => _pusi;
    public bool TransportPriority => _transportPriority;
    public ushort PacketIdentifier => _pid;
    public bool IsScrambled => !(_tsc == 0b00);
    public bool IsScrambledWithEvenKey => _tsc == 0b10;
    public bool IsScrambledWithOddKey => _tsc == 0b11;
    public byte ContinuityCounter => _continuityCounter;
    public ITransportStreamPackageAdaptationField? AdaptationField { get; private set; }
    public ReadOnlyMemory<byte>? Payload { get; private set; }
}