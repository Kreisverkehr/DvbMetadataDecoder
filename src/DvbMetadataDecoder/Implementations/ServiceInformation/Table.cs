using System;
using Kreisverkehr.DvbMetadataDecoder;

namespace DvbMetadataDecoder.Implementations.ServiceInformation;

internal class Table : IServiceInformationTable
{
    internal static Table CreateTable(ushort pid, ReadOnlySpan<byte> data)
    {
        return new Table()
        {
            TableId = data[0]
        };
    }

    public byte TableId {get; private set;}

    public IList<IDescriptor> Descriptors => throw new NotImplementedException();
}
