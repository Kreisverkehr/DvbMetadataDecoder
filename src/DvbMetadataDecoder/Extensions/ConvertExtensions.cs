namespace Kreisverkehr.DvbMetadataDecoder.Extensions;

internal static class ConvertExtensions
{
    extension(Convert)
    {
        /// <summary>
        /// Converts a binary coded decimal to an actual 64-bit integer.
        /// </summary>
        /// <remarks>It shifts through each nibble, calculates the corresponding digit and adds them up to the final value.</remarks>
        /// <param name="bcd">value in BCD format</param>
        /// <returns>A valid 64-bit integer</returns>
        /// <exception cref="FormatException">Thrown if it encounters a digit larger than 9</exception>
        internal static ulong BcdToUInt64(ulong bcd)
        {
            ulong result = 0;

            for (var shift = 60; shift >= 0; shift -= 4)
            {
                var digit = (bcd >> shift) & 0xF;
                if (digit > 9)
                    throw new FormatException("Invalid BCD-Digit.");

                result = checked(result * 10 + digit);
            }

            return result;
        }
    }
}