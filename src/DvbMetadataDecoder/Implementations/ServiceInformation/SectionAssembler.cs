namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;

internal sealed class SectionAssembler
{
    private readonly List<byte> _partialSection = [];

    private readonly Action<byte[]> _onSection;

    internal SectionAssembler(Action<byte[]> onSection)
    {
        _onSection = onSection;
    }

    public void Process(ITransportStreamPackage package)
    {
        if(package.Payload?.IsEmpty ?? true)
            return;

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
                    _onSection(_partialSection.ToArray());
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

            _onSection(payload.Slice(offset, sectionLength).ToArray());
            offset += sectionLength;
        }
    }
}