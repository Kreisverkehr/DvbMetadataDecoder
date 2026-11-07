using System.Diagnostics;
using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Extensions;

namespace Kreisverkehr.DvbMetadataDecoder.Implementations.ServiceInformation.Descriptors;

[DebuggerDisplay("({Iso639Code}) {Name} - {Text}")]
internal class ShortEventDescriptor : Descriptor, IShortEventDescriptor
{
    public ShortEventDescriptor(ReadOnlySpan<byte> data) : base(data)
    {
        Iso639Code = Encoding.ASCII.GetString(data[2..5]);
        int nameLen = data[5];
        Name = Encoding.DvbText.GetString(data.Slice(6, nameLen));
        int textLen = data[6 + nameLen];
        Text = Encoding.DvbText.GetString(data.Slice(7+nameLen, textLen));
    }

    public string Iso639Code {get;private set;}

    public string Name {get;private set;}

    public string Text {get;private set;}
}