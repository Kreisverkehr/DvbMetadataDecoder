using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class EventInformationSection : Section, IEventInformationSection
{
    private const int SECTION_HEADER_LEN = 6;
    private const int ENTRY_HEADER_LEN = 12;

    private List<EventInformation> _events = [];

    public EventInformationSection(ReadOnlySpan<byte> data) : base(data)
    {
        ServiceId = (ushort)((data[3] << 8) | data[4]);
        var bodyData = GetDataPart(data);
        TransportStreamId = (ushort)((bodyData[0] << 8) | bodyData[1]);
        OriginalNetworkId = (ushort)((bodyData[2] << 8) | bodyData[3]);
        SegmentLastSectionNumber = bodyData[4];
        LastTableId = bodyData[5];

        int eventStart = SECTION_HEADER_LEN;
        while (eventStart + ENTRY_HEADER_LEN < bodyData.Length)
        {
            int entryDescriptorsLen = ((bodyData[eventStart + 10] & 0b00001111) << 8) | bodyData[eventStart + 11];
            var entrydata = bodyData.Slice(eventStart, ENTRY_HEADER_LEN + entryDescriptorsLen);
            _events.Add(new(entrydata));
            eventStart += entrydata.Length;
        }
    }

    public ushort ServiceId { get; private set; }

    public ushort TransportStreamId { get; private set; }

    public ushort OriginalNetworkId { get; private set; }

    public byte SegmentLastSectionNumber { get; private set; }

    public byte LastTableId { get; private set; }

    public IReadOnlyCollection<IEventInformationSection.IEventInformation> Events => _events.AsReadOnly();

    private class EventInformation : IEventInformationSection.IEventInformation
    {
        private List<Descriptor> _descriptors = [];
        internal EventInformation(ReadOnlySpan<byte> data)
        {
            EventId = (ushort)((data[0] << 8) | data[1]);
            ulong tsStartTime = ((ulong)data[2] << 32) | ((ulong)data[3] << 24) | ((ulong)data[4] << 16) | ((ulong)data[5] << 8) | data[6];
            StartTime = DateTime.FromTsTimestamp(tsStartTime);
            uint tsDuration = ((uint)data[7] << 16) | ((uint)data[8] << 8) | data[9];
            Duration = TimeSpan.FromTsDuration(tsDuration);
            RunningStatus = (RunningStatus)(data[10] >> 5);
            FreeCaMode = (data[10] & 0b00010000) != 0;
            Descriptor.AddDescriptors(data[ENTRY_HEADER_LEN..], _descriptors);
        }

        public ushort EventId { get; private set; }

        public DateTime? StartTime { get; private set; }

        public TimeSpan Duration { get; private set; }

        public RunningStatus RunningStatus { get; private set; }

        public bool FreeCaMode { get; private set; }

        public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();
    }
}