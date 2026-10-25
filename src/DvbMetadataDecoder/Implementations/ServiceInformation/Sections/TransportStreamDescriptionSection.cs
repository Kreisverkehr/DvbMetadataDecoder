namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class TransportStreamDescriptionSection : Section, ITransportStreamDescriptionSection
{
    private readonly IList<Descriptor> _descriptors = [];
    public TransportStreamDescriptionSection(ReadOnlySpan<byte> data) : base(data)
    {
        Descriptor.AddDescriptors(GetDataPart(data), _descriptors);
    }

    public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();
}