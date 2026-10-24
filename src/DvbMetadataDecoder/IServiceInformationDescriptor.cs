namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationDescriptor
{
    public ushort Tag { get; }
}

public interface IUnknownDescriptor
{
    public ReadOnlyMemory<byte> Data { get; }
}