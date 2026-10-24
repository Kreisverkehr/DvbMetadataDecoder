namespace Kreisverkehr.DvbMetadataDecoder;

public interface IServiceInformationTable
{
    byte TableId { get; }
    IList<IDescriptor> Descriptors { get; }
}

public interface IDescriptor
{
}