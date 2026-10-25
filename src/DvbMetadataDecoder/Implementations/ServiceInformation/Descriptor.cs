using Kreisverkehr.DvbMetadataDecoder;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation;

internal abstract class Descriptor : IServiceInformationDescriptor
{
    private ushort _tag;
    private ushort _length;

    protected Descriptor(ReadOnlySpan<byte> data)
    {
        _tag = data[0];
        _length = data[1];
    }

    public ushort Tag => _tag;

    internal static void AddDescriptors(ReadOnlySpan<byte> data, IList<Descriptor> descriptors)
    {
        if (data.IsEmpty || data.Length < 2)
            return;

        int descriptorStart = 0;
        do
        {
            byte descriptorLength = data[descriptorStart + 1];
            descriptors.Add(CreateDescriptor(data.Slice(descriptorStart, descriptorLength + 2)));
            descriptorStart += descriptorLength + 2;

        } while (descriptorStart < data.Length);
    }

    internal static Descriptor CreateDescriptor(ReadOnlySpan<byte> data) => data[0] switch
    {
        _ => new UnknownDescriptor(data)
    };
}

internal class UnknownDescriptor : Descriptor, IUnknownDescriptor
{
    public UnknownDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        Data = data[2..].ToArray();
    }

    public ReadOnlyMemory<byte> Data { get; private set; }
}