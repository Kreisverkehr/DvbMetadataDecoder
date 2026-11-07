using System.Text;
using Kreisverkehr.DvbMetadataDecoder.Text;

namespace Kreisverkehr.DvbMetadataDecoder.Extensions;

public static class EncodingExtensions
{
    extension(Encoding)
    {
        public static Encoding DvbText => DvbEncoding.Instance;
    }
}
