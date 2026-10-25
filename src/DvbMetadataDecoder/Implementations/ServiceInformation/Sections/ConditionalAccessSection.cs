namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Sections;

internal class ConditionalAccessSection : Section, IConditionalAccessSection
{
    private readonly IList<Descriptor> _descriptors = [];
    public ConditionalAccessSection(ReadOnlySpan<byte> data) : base(data)
    {
        Descriptor.AddDescriptors(GetDataPart(data), _descriptors);
    }

    public IReadOnlyCollection<IServiceInformationDescriptor> Descriptors => _descriptors.AsReadOnly();
}