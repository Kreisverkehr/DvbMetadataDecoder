namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class RegistrationDescriptor : Descriptor, IRegistrationDescriptor
{
    public RegistrationDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        FormatIdentifier = (uint)((data[2] << 24) | (data[3] << 16) | (data[4] << 8) | data[5]);
        AdditionalIdentificationInfo = data[6..].ToArray();
    }

    public uint FormatIdentifier { get; private set; }

    public ReadOnlyMemory<byte> AdditionalIdentificationInfo { get; private set; }
}
