using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.Encodings
{
    /// <summary>
    /// Specs: https://qoiformat.org/qoi-specification.pdf
    /// ┌─ QOI_OP_INDEX ──────────┐
    /// │ Byte[0]                 │
    /// │ 7 6   | 5 4 3 2 1 0     │
    /// │───────┼─────────────────│
    /// │ 0 0   │ index           │
    /// └───────┴─────────────────┘
    /// 2 - bit tag b00
    /// 6 - bit index into the color index array: 0..63
    /// A valid encoder must not issue 7 or more consecutive QOI_OP_INDEX
    /// chunks to the index 0, to avoid confusion with the end marker.
    /// </summary>
    internal static class IndexEncoding
    {
        public const byte QOI_OP_INDEX = 0b00000000;
        public static bool CanDecode(byte marker)
        {
            return QOI_OP_INDEX == (marker & EncodingHelper.QOI_MASK_2_BITS);
        }
        public static QoiPixelData Decode(QoiPixelData[] last64ColorValues, byte marker)
        {
            return last64ColorValues[marker];
        }
        /// <summary>
        /// A running array[64] (zero-initialized) of previously seen pixel
        /// values is maintained by the encoder and decoder.Each pixel that is
        /// seen by the encoder and decoder is put into this array at the
        /// position formed by a hash function of the color value.In the
        /// encoder, if the pixel value at the index matches the current pixel,
        /// this index position is written to the stream as QOI_OP_INDEX.The
        /// hash function for the index is:
        ///     index_position = (r * 3 + g * 5 + b * 7 + a * 11) % 64
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        public static int ComputeIndexPosition(QoiPixelData pixel)
        {
            return ((pixel.R * 3) + (pixel.G * 5) + (pixel.B * 7) + (pixel.A * 11)) % 64;
        }
        public static void Encode(BinaryWriter sw, int indexInLast64ColorValues)
        {
            sw.Write((byte)indexInLast64ColorValues);
        }
    }
}
