using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

internal class ExtendedEventDescriptor : Descriptor, IExtendedEventDescriptor
{
    private readonly List<IExtendedEventDescriptor.IExtendedEventItem> _items = [];

    public ExtendedEventDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        DescriptorNo = (ushort)((data[2] & 0b11110000) >> 4);
        LastDescriptorNo = (ushort)(data[2] & 0b00001111);
        Iso639Code = Encoding.ASCII.GetString(data.Slice(3, 3));

        int itemOffset = 7;
        int itemsEnd = itemOffset + data[6];
        while (itemOffset < itemsEnd)
        {
            int descriptionLength = data[itemOffset++];
            string description = Encoding.DvbText.GetString(data.Slice(itemOffset, descriptionLength));
            itemOffset += descriptionLength;

            int textLength = data[itemOffset++];
            string text = Encoding.DvbText.GetString(data.Slice(itemOffset, textLength));
            itemOffset += textLength;

            _items.Add(new ExtendedEventItem(description, text));
        }

        int eventTextLength = data[itemsEnd];
        Text = Encoding.DvbText.GetString(data.Slice(itemsEnd + 1, eventTextLength));
    }

    public ushort DescriptorNo { get; private set; }

    public ushort LastDescriptorNo { get; private set; }

    public string Iso639Code { get; private set; }

    public IReadOnlyCollection<IExtendedEventDescriptor.IExtendedEventItem> Items => _items.AsReadOnly();

    public string Text { get; private set; }

    private class ExtendedEventItem : IExtendedEventDescriptor.IExtendedEventItem
    {
        internal ExtendedEventItem(string description, string text)
        {
            Description = description;
            Text = text;
        }

        public string Description { get; private set; }

        public string Text { get; private set; }
    }
}