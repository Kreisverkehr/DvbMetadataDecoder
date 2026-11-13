using System.Diagnostics;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class ServiceDescriptionSection : Section, IServiceDescriptionSection
{
    internal const int DESC_HEADER_LEN = 5;
    private readonly List<ServiceDescription> _serviceDescriptions = [];
    public ServiceDescriptionSection(ReadOnlySpan<byte> data) : base(data)
    {
        TransportStreamId = (ushort)((data[3] << 8) | data[4]);
        var bodyData = GetDataPart(data);
        OriginalNetworkId = (ushort)((bodyData[0] << 8) | bodyData[1]);

        int nextStart = 3;
        while (nextStart < bodyData.Length)
        {
            int descLen = DESC_HEADER_LEN + (((bodyData[nextStart + 3] & 0b00001111) << 8) | bodyData[nextStart + 4]);
            _serviceDescriptions.Add(new(bodyData.Slice(nextStart, descLen)));
            nextStart += descLen;
        }
    }

    public ushort TransportStreamId { get; private set; }

    public ushort OriginalNetworkId { get; private set; }

    public IReadOnlyCollection<IServiceDescriptionSection.IServiceDescription> Services => _serviceDescriptions.AsReadOnly();

    [DebuggerDisplay("{ServiceDescriptor.ServiceName}")]
    internal class ServiceDescription : IServiceDescriptionSection.IServiceDescription
    {
        private readonly List<Descriptor> _descriptors = [];
        public ServiceDescription(ReadOnlySpan<byte> data)
        {
            ServiceId = (ushort)((data[0] << 8) | data[1]);
            HasEitSchedule = (data[2] & 0b00000010) != 0;
            HasEitPresentFollowing = (data[2] & 0b00000001) != 0;
            RunningStatus = (RunningStatus)(data[3] >> 5);
            FreeCaMode = (data[2] & 0b00010000) != 0;
            Descriptor.AddDescriptors(data[DESC_HEADER_LEN..], _descriptors);
        }
        public ushort ServiceId { get; private set; }

        public bool HasEitSchedule { get; private set; }

        public bool HasEitPresentFollowing { get; private set; }

        public RunningStatus RunningStatus { get; private set; }

        public bool FreeCaMode { get; private set; }

        public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();

        internal IServiceDescriptor? ServiceDescriptor => _descriptors.OfType<IServiceDescriptor>().FirstOrDefault();
    }
}