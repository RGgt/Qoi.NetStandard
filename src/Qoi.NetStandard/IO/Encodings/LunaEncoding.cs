using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.Encodings
{
    /// <summary>
    /// Specs: https://qoiformat.org/qoi-specification.pdf
    /// ┌─ QOI_OP_LUMA ───────────┬─────────────────────────┐
    /// | Byte[0]                 │ Byte[1]                 │
    /// │  7 6  | 5 4 3 2 1 0     │ 7 6 5 4     |3 2 1 0    │
    /// │───────┼─────────────────┼─────────────┼───────────│
    /// │ 1 0   │ diff green      │ dr - dg     │ db - dg   │
    /// └───────┴─────────────────┴─────────────┴───────────┘
    /// 2 - bit tag b10
    /// 6 - bit green channel difference from the previous pixel - 32..31
    /// 4 - bit red channel difference minus green channel difference - 8..7
    /// 4 - bit blue channel difference minus green channel difference - 8..7
    /// The green channel is used to indicate the general direction of
    /// change and is encoded in 6 bits.The red and blue channels(dr
    /// and db) base their diffs off of the green channel difference. I.e.:
    /// dr_dg = (last_px.r - cur_px.r) - (last_px.g - cur_px.g)
    /// db_dg = (last_px.b - cur_px.b) - (last_px.g - cur_px.g)
    /// The difference to the current channel values are using a wraparound
    /// operation, so 10 - 13 will result in 253, while 250 + 7 will result
    /// in 1.
    /// Values are stored as unsigned integers with a bias of 32 for the
    /// green channel and a bias of 8 for the red and blue channel.
    /// The alpha value remains unchanged from the previous pixel.
    /// </summary>
    internal static class LunaEncoding
    {
        public const byte QOI_OP_LUMA = 0b10000000;
        public static bool CanDecode(byte marker)
        {
            return QOI_OP_LUMA == (marker & EncodingHelper.QOI_MASK_2_BITS);
        }
        /// <summary>
        /// The alpha value remains unchanged from the previous pixel.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="previousPixelData"></param>
        /// <param name="marker"></param>
        /// <param name="bytesAdvanced"></param>
        /// <returns></returns>
        public static QoiPixelData Decode(QoiBytesReader sr, QoiPixelData previousPixelData, byte marker, out int bytesAdvanced)
        {
            byte valueOfSecondtByte = sr.ReadByte();
            bytesAdvanced = 1;

            // Values are stored with a bias of 32 for the green channel
            // and a bias of 8 for the red and blue channel.

            int vg = (marker & 0b00111111) - 32 /* bias */;
            byte r = (byte)(previousPixelData.R + (byte)(vg - 8 /* bias */ + ((valueOfSecondtByte >> 4) & 0b00001111)));
            byte g = (byte)(previousPixelData.G + (byte)(vg));
            byte b = (byte)(previousPixelData.B + (byte)(vg - 8 /* bias */ + (valueOfSecondtByte & 0b00001111)));
            return new QoiPixelData(previousPixelData.A, r, g, b);
        }
        public static bool CanEncode(int vg, int vg_r, int vg_b)
        {
            return vg_r > -9 && vg_r < 8 && vg > -33 && vg < 32 && vg_b > -9 && vg_b < 8;
        }
        public static void Encode(BinaryWriter sw, int vg, int vg_r, int vg_b)
        {
            byte tempValue = (byte)(QOI_OP_LUMA + vg + 32 /* bias */);
            sw.Write(tempValue);

            tempValue = (byte)(((vg_r + 8) << 4) + vg_b + 8 /* bias */);
            sw.Write(tempValue);
        }
    }
}
