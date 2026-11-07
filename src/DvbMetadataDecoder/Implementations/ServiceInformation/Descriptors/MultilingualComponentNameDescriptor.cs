using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class MultilingualComponentNameDescriptor : Descriptor, IMultilingualComponentNameDescriptor
{
    private readonly List<MultilingualComponentName> _names = [];

    public MultilingualComponentNameDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        int entryStart = 2;
        while(entryStart < data.Length - 2)
        {
            int entryLength = data[entryStart + 3];
            _names.Add(new(data.Slice(entryStart, entryLength + 4)));
            entryStart += entryLength + 4;
        }
    }

    public IReadOnlyCollection<IMultilingualComponentNameDescriptor.IMultilingualComponentName> ComponentNames => _names.AsReadOnly();

    private class MultilingualComponentName : IMultilingualComponentNameDescriptor.IMultilingualComponentName
    {
        internal MultilingualComponentName(ReadOnlySpan<byte> data)
        {
            Iso639Code = Encoding.ASCII.GetString(data[..3]);
            Text = Encoding.DvbText.GetString(data[4..]);
        }

        public string Iso639Code { get; private set; }

        public string Text { get; private set; }
    }
}
