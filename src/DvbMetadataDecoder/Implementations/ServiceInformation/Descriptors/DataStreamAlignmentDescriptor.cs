namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class DataStreamAlignmentDescriptor : Descriptor, IDataStreamAlignmentDescriptor
{
    public DataStreamAlignmentDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        AlignmentType = data[2];
    }

    public byte AlignmentType { get; private set; }
}