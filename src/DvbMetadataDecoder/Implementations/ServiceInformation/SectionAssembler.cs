namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;

internal sealed class SectionAssembler
{
    private readonly List<byte> _partialSection = [];

    private readonly Action<byte[]> _onSection;
    private byte? _lastPayloadContinuityCounter;

    internal SectionAssembler(Action<byte[]> onSection)
    {
        _onSection = onSection;
    }

    public void Process(ITransportStreamPackage package)
    {
        if (package.TransportErrorIndicator || package.IsScrambled)
        {
            Reset();
            return;
        }

        if (package.AdaptationField?.DiscontinuityIndicator == true)
        {
            Reset();
        }

        if(package.Payload?.IsEmpty ?? true)
            return;

        var continuityCounter = package.ContinuityCounter;
        if (_lastPayloadContinuityCounter is byte previousCounter)
        {
            if (continuityCounter == previousCounter)
                return;

            if (continuityCounter != ((previousCounter + 1) & 0x0f))
            {
                _partialSection.Clear();
                if (!package.PayloadUnitStartIndicator)
                {
                    _lastPayloadContinuityCounter = continuityCounter;
                    return;
                }
            }
        }

        _lastPayloadContinuityCounter = continuityCounter;

        var payload = package.Payload.Value.Span;

        if (package.PayloadUnitStartIndicator)
        {
            var pointerLength = payload[0];
            if (pointerLength + 1 > payload.Length)
                return;

            CompletePartialSection(payload.Slice(1, pointerLength));
            _partialSection.Clear();
            ProcessNewSections(payload[(pointerLength + 1)..]);
            return;
        }

        CompletePartialSection(payload);
    }

    private void CompletePartialSection(ReadOnlySpan<byte> continuation)
    {
        if (_partialSection.Count == 0)
            return;

        for (var index = 0; index < continuation.Length; index++)
        {
            _partialSection.Add(continuation[index]);
            if (_partialSection.Count == 3)
            {
                var sectionLength = 3 + (((_partialSection[1] & 0x0f) << 8) | _partialSection[2]);
                if (sectionLength > 4096)
                {
                    _partialSection.Clear();
                    return;
                }
            }

            if (_partialSection.Count >= 3)
            {
                var expectedLength = 3 + (((_partialSection[1] & 0x0f) << 8) | _partialSection[2]);
                if (_partialSection.Count == expectedLength)
                {
                    EmitSection(_partialSection.ToArray());
                    _partialSection.Clear();
                    return;
                }
            }
        }
    }

    private void ProcessNewSections(ReadOnlySpan<byte> payload)
    {
        var offset = 0;
        while (offset < payload.Length && payload[offset] != 0xff)
        {
            if (payload.Length - offset < 3)
            {
                _partialSection.AddRange(payload[offset..].ToArray());
                return;
            }

            var sectionLength = 3 + (((payload[offset + 1] & 0x0f) << 8) | payload[offset + 2]);
            if (sectionLength > 4096)
                return;

            if (offset + sectionLength > payload.Length)
            {
                _partialSection.AddRange(payload[offset..].ToArray());
                return;
            }

            EmitSection(payload.Slice(offset, sectionLength).ToArray());
            offset += sectionLength;
        }
    }

    private void EmitSection(byte[] section)
    {
        if (HasValidCrc(section))
            _onSection(section);
    }

    private static bool HasValidCrc(ReadOnlySpan<byte> section)
    {
        if (section.Length < 3)
            return false;

        var tableHasCrc = (section[1] & 0x80) != 0 || section[0] == 0x73;
        if (!tableHasCrc)
            return true;

        if (section.Length < 7)
            return false;

        uint crc = 0xffffffff;
        foreach (var value in section)
        {
            crc ^= (uint)value << 24;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & 0x80000000) != 0
                    ? (crc << 1) ^ 0x04c11db7
                    : crc << 1;
        }

        return crc == 0;
    }

    private void Reset()
    {
        _partialSection.Clear();
        _lastPayloadContinuityCounter = null;
    }
}