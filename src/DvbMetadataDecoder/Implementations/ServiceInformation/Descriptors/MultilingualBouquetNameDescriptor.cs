using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class MultilingualBouquetNameDescriptor : Descriptor, IMultilingualBouquetNameDescriptor
{
    private readonly List<MultilingualBouquetName> _names = [];

    public MultilingualBouquetNameDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        int entryStart = 2;
        while(entryStart < data.Length - 2)
        {
            int entryLength = data[entryStart + 3];
            _names.Add(new(data.Slice(entryStart, entryLength + 4)));
            entryStart += entryLength + 4;
        }
    }

    public IReadOnlyCollection<IMultilingualBouquetNameDescriptor.IMultilingualBouquetName> BouquetNames => _names.AsReadOnly();

    private class MultilingualBouquetName : IMultilingualBouquetNameDescriptor.IMultilingualBouquetName
    {
        internal MultilingualBouquetName(ReadOnlySpan<byte> data)
        {
            Iso639Code = Encoding.ASCII.GetString(data[..3]);
            Text = Encoding.DvbText.GetString(data[4..]);
        }

        public string Iso639Code { get; private set; }

        public string Text { get; private set; }
    }
}
