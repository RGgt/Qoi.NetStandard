using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.Encodings
{
    /// <summary>
    /// Specs: https://qoiformat.org/qoi-specification.pdf
    /// ┌─ QOI_OP_DIFF ───────────┐
    /// │ Byte[0] 7 6 5 4 3 2 1 0 │
    /// │───────┼─────┼─────┼─────│
    /// │ 0 1   │ dr  │ dg  │ db  │
    /// └───────┴─────┴─────┴─────┘
    /// 2 - bit tag b01
    /// 2 - bit red channel difference from the previous pixel - 2..1
    /// 2 - bit green channel difference from the previous pixel - 2..1
    /// 2 - bit blue channel difference from the previous pixel - 2..1
    ///
    /// The difference to the current channel values are using a wraparound
    /// operation, so 1 - 2 will result in 255, while 255 + 1 will result
    /// in 0.
    /// Values are stored as unsigned integers with a bias of 2.E.g. - 2
    /// is stored as 0(b00). 1 is stored as 3(b11).
    /// The alpha value remains unchanged from the previous pixel.
    /// </summary>
    internal static class DiffEncoding
    {
        public const byte QOI_OP_DIFF = 0b01000000;
        public static bool CanDecode(byte marker) {
            return QOI_OP_DIFF == (marker & EncodingHelper.QOI_MASK_2_BITS);
        }
        /// <summary>
        /// The alpha value remains unchanged from the previous pixel.
        /// </summary>
        /// <param name="previousPixelData"></param>
        /// <param name="marker"></param>
        /// <returns></returns>
        public static QoiPixelData Decode(QoiPixelData previousPixelData, byte marker)
        {
            // Values are stored with a bias of 2. E.g. -2
            // is stored as 0(b00). 1 is stored as 3(b11).

            byte r = (byte)(previousPixelData.R + (byte)(((marker >> 4) & 0b00000011) - 2 /* bias */));
            byte g = (byte)(previousPixelData.G + (byte)(((marker >> 2) & 0b00000011) - 2 /* bias */));
            byte b = (byte)(previousPixelData.B + (byte)(((marker >> 0) & 0b00000011) - 2 /* bias */));
            return new QoiPixelData(previousPixelData.A, r, g, b);
        }
        public static bool CanEncode(int vr, int vg, int vb)
        {
            return vr > -3 && vr < 2 && vg > -3 && vg < 2 && vb > -3 && vb < 2;
        }
        public static void Encode(BinaryWriter sw, int vr, int vg, int vb)
        {
            byte tempValue = (byte)(QOI_OP_DIFF + ((vr + 2 /* bias */) << 4) + ((vg + 2 /* bias */) << 2) + ((vb + 2 /* bias */) << 0));
            sw.Write(tempValue);
        }
    }
}
