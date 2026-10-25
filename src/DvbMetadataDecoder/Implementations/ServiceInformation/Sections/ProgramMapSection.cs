using Kreisverkehr.DvbMetadataDecoder;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class ProgramMapSection : Section, IProgramMapSection
{
    private readonly ushort _pcrPid;
    private readonly ushort _programInfoLength;
    private readonly List<Descriptor> _descriptors = [];
    private readonly List<Entry> _entries = [];

    public ProgramMapSection(ReadOnlySpan<byte> data) : base(data)
    {
        var bodyData = GetDataPart(data);

        _pcrPid = (ushort)(((bodyData[0] & 0b00011111) << 8) | bodyData[1]);
        _programInfoLength = (ushort)(((bodyData[2] & 0b00001111) << 8) | bodyData[3]);
        Descriptor.AddDescriptors(bodyData.Slice(4, _programInfoLength), _descriptors);

        int startOfItem = _programInfoLength + 4;
        while(startOfItem < bodyData.Length)
        {
            int itemLength = (ushort)(((bodyData[startOfItem + 3] & 0b00001111) << 8) | bodyData[startOfItem + 4]) + 5;
            _entries.Add(new Entry(bodyData.Slice(startOfItem, itemLength)));
            startOfItem += itemLength;
        }
    }

    public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();
    public IReadOnlyCollection<IProgramMapSection.IEntry> Entries => _entries.AsReadOnly();

    private class Entry : IProgramMapSection.IEntry
    {
        private readonly byte _streamType;
        private readonly ushort _elementaryPid;
        private readonly List<Descriptor> _descriptors = [];

        public Entry(ReadOnlySpan<byte> data)
        {
            _streamType = data[0];
            _elementaryPid = (ushort)(((data[1] & 0b00011111) << 8) | data[2]);

            var esInfoLength = (ushort)(((data[3] & 0b00001111) << 8) | data[4]);
            Descriptor.AddDescriptors(data.Slice(5, esInfoLength), _descriptors);
        }

        public byte StreamType => _streamType;
        public ushort ElementaryPid => _elementaryPid;
        public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();
    }
}
