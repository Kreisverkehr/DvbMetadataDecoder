using System.Diagnostics;
using Kreisverkehr.DvbMetadataDecoder;
using Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

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
        0x02 => new VideoStreamDescriptor(data),
        0x03 => new AudioStreamDescriptor(data),
        0x04 => new HierarchyDescriptor(data),
        0x05 => new RegistrationDescriptor(data),
        0x06 => new DataStreamAlignmentDescriptor(data),
        0x07 => new TargetBackgroundGridDescriptor(data),
        0x08 => new VideoWindowDescriptor(data),
        0x09 => new ConditionalAccessDescriptor(data),
        0x0A => new Iso639LanguageDescriptor(data),
        0x0B => new SystemClockDescriptor(data),
        0x0C => new MultiplexBufferUtilizationDescriptor(data),
        0x0D => new CopyrightDescriptor(data),
        0x0E => new MaximumBitrateDescriptor(data),
        0x0F => new PrivateDataIndicatorDescriptor(data),
        0x10 => new SmoothingBufferDescriptor(data),
        0x11 => new StdDescriptor(data),
        0x12 => new IbpDescriptor(data),
        0x1B => new Mpeg4VideoDescriptor(data),
        0x1C => new Mpeg4AudioDescriptor(data),

        0x40 => new NetworkNameDescriptor(data),
        0x41 => new ServiceListDescriptor(data),
        0x48 => new ServiceDescriptor(data),
        0x4D => new ShortEventDescriptor(data),
        0x4E => new ExtendedEventDescriptor(data),
        0x50 => new ComponentDescriptor(data),
        0x5B => new MultilingualNetworkNameDescriptor(data),
        0x5C => new MultilingualBouquetNameDescriptor(data),
        0x5D => new MultilingualServiceNameDescriptor(data),
        0x5E => new MultilingualComponentNameDescriptor(data),

        _ => new UnknownDescriptor(data)
    };
}

[DebuggerDisplay("Tag = {Tag}, Length = {Data.Length}")]
internal class UnknownDescriptor : Descriptor, IUnknownDescriptor
{
    public UnknownDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        Data = data[2..].ToArray();
    }

    public ReadOnlyMemory<byte> Data { get; private set; }
}