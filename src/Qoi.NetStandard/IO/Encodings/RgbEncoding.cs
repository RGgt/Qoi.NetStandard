using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.Encodings
{
    /// <summary>
    /// Specs: https://qoiformat.org/qoi-specification.pdf
    /// ┌─ QOI_OP_RGB ────────────┬─────────┬─────────┬─────────┐
    /// │ Byte[0]                 │ Byte[1] │ Byte[2] │ Byte[3] │
    /// │ 7 6 5 4 3 2 1 0         │    7..0 │    7..0 │    7..0 │
    /// │─────────────────────────┼─────────┼─────────┼─────────│
    /// │ 1 1 1 1 1 1 1 0         │     red │   green │    blue │
    /// └─────────────────────────┴─────────┴─────────┴─────────┘
    /// 8 - bit tag b11111110
    /// 8 - bit red channel value
    /// 8 - bit green channel value
    /// 8 - bit blue channel value
    /// The alpha value remains unchanged from the previous pixel.
    /// </summary>
    internal static class RgbEncoding
    {
        public const byte QOI_OP_RGB = 0b11111110;
        public static bool CanDecode(byte marker) {
            return QOI_OP_RGB == marker;
        }
        /// <summary>
        /// The alpha value remains unchanged from the previous pixel.
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="previousPixel"></param>
        /// <param name="bytesAdvanced"></param>
        /// <returns></returns>
        public static QoiPixelData Decode(QoiBytesReader sr, QoiPixelData previousPixel, out int bytesAdvanced)
        {
            QoiPixelData pixelData;
            byte r = sr.ReadByte();
            byte g = sr.ReadByte();
            byte b = sr.ReadByte();
            pixelData = new QoiPixelData(previousPixel.A, r, g, b);
            bytesAdvanced = 3;
            return pixelData;
        }
        public static void Encode(BinaryWriter sw, QoiPixelData pixelData)
        {
            sw.Write(QOI_OP_RGB);
            sw.Write(pixelData.R);
            sw.Write(pixelData.G);
            sw.Write(pixelData.B);
        }
    }
}
