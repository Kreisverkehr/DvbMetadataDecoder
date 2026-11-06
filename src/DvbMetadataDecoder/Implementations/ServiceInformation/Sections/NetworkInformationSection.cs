namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class NetworkInformationSection : Section, INetworkInformationSection
{
    private List<Descriptor> _descriptors = [];
    private List<NetworkInfoEntry> _entries = [];

    public NetworkInformationSection(ReadOnlySpan<byte> data) : base(data)
    {
        NetworkId = (ushort)((data[3] << 8) | data[4]);
        var bodyData = GetDataPart(data);

        ushort descriptorLength = (ushort)(((bodyData[0] & 0b00001111) << 8) | bodyData[1]);
        Descriptor.AddDescriptors(bodyData.Slice(2, descriptorLength), _descriptors);

        int entryStart = descriptorLength + 4;
        ushort entriesLength = (ushort)(((bodyData[2 + descriptorLength] & 0b00001111) << 8) | bodyData[3 + descriptorLength]);

        while (entryStart < entriesLength + descriptorLength + 4)
        {
            ushort currentEntryLength = (ushort)(((bodyData[entryStart + 4] & 0b00001111) << 8) | bodyData[entryStart + 5]);
            _entries.Add(new NetworkInfoEntry(bodyData.Slice(entryStart, currentEntryLength + 6)));
            entryStart += currentEntryLength + 6;
        }

    }

    public ushort NetworkId { get; private set; }

    public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();

    public IReadOnlyCollection<INetworkInformationSection.INetworkInfo> NetworkInfos => _entries.AsReadOnly();
}

internal class NetworkInfoEntry : INetworkInformationSection.INetworkInfo
{
    private List<Descriptor> _descriptors = [];
    internal NetworkInfoEntry(ReadOnlySpan<byte> data)
    {
        TransportStreamId = (ushort)((data[0] << 8) | data[1]);
        OriginalNetworkId = (ushort)((data[2] << 8) | data[3]);
        Descriptor.AddDescriptors(data[6..], _descriptors);
    }

    public ushort TransportStreamId {get;private set;}

    public ushort OriginalNetworkId {get;private set;}

    public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();
}