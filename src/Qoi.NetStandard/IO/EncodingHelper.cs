using System;
using System.Collections.Generic;
using System.Text;

namespace Qoi.NetStandard.IO
{
    internal static class EncodingHelper
    {
        /// <summary>
        /// This is the max for the 2-bits tag used for some encodings.
        /// Specs: https://qoiformat.org/qoi-specification.pdf
        /// Each chunk starts with a 2- or 8-bit tag, followed by a number of
        /// data bits.The bit length of chunks is divisible by 8 - i.e.all
        /// chunks are byte aligned. All values encoded in these data bits have
        /// the most significant bit on the left.The 8-bit tags have
        /// precedence over the 2-bit tags. A decoder must check for the
        /// presence of an 8-bit tag first.
        /// </summary>
        public const byte QOI_MASK_2_BITS = 0b11000000; /* 11000000 */

        public static void ToBigEndian(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
        }
    }
}
