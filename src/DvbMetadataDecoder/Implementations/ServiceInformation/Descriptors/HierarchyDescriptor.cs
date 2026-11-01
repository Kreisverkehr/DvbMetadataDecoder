using Kreisverkehr.DvbMetadataDecoder.Lookups;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class HierarchyDescriptor : Descriptor, IHierarchyDescriptor
{
    public HierarchyDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        ViewScalability = (data[2] & 0b10000000) != 1;
        TemporalScalability = (data[2] & 0b01000000) != 1;
        SpatialScalability = (data[2] & 0b00100000) != 1;
        QualityScalability = (data[2] & 0b00010000) != 1;
        HierarchyType = (byte)(data[2] & 0b00001111);
        HierarchyLayerIndex = (byte)(data[3] & 0b00111111);
        TrefPresent = (data[3] & 0b10000000) != 0;
        HierarchyEmbeddedLayerIndex = (byte)(data[4] & 0b00111111);
        HierarchyChannel = (byte)(data[5] & 0b00111111);
    }

    public bool ViewScalability { get; private set; }

    public bool TemporalScalability { get; private set; }

    public bool SpatialScalability { get; private set; }

    public bool QualityScalability { get; private set; }

    public byte HierarchyType { get; private set; }

    public string HierarchyTypeDescription => StaticValues.HierarchyTypes[HierarchyType];

    public byte HierarchyLayerIndex { get; private set; }

    public bool TrefPresent { get; private set; }

    public byte HierarchyEmbeddedLayerIndex { get; private set; }

    public byte HierarchyChannel { get; private set; }
}