using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.Encodings
{
    /// <summary>
    /// Specs: https://qoiformat.org/qoi-specification.pdf
    /// ┌─ QOI_OP_RGBA ───────────┬─────────┬─────────┬─────────┬─────────┐
    /// │ Byte[0]                 │ Byte[1] │ Byte[2] │ Byte[3] │ Byte[4] │
    /// │ 7 6 5 4 3 2 1 0         │    7..0 │    7..0 │    7..0 │    7..0 │
    /// │─────────────────────────┼─────────┼─────────┼─────────┼─────────│
    /// │ 1 1 1 1 1 1 1 1         │     red │   green │    blue │   alpha │
    /// └─────────────────────────┴─────────┴─────────┴─────────┴─────────┘
    /// 8-bit tag b11111111
    /// 8-bit red channel value
    /// 8-bit green channel value
    /// 8-bit blue channel value
    /// 8-bit alpha channel value
    /// </summary>
    internal static class ArgbEncodingc
    {
        public const byte QOI_OP_RGBA = 0b11111111;
        public static bool CanDecode(byte marker) {
            return QOI_OP_RGBA == marker;
        }
        public static QoiPixelData Decode(QoiBytesReader sr, out int bytesAdvanced)
        {
            QoiPixelData pixelData;
            byte r = sr.ReadByte();
            byte g = sr.ReadByte();
            byte b = sr.ReadByte();
            byte a = sr.ReadByte();
            pixelData = new QoiPixelData(a, r, g, b);
            bytesAdvanced = 4;
            return pixelData;
        }
        public static void Encode(BinaryWriter sw, QoiPixelData pixelData)
        {
            sw.Write(QOI_OP_RGBA);
            sw.Write(pixelData.R);
            sw.Write(pixelData.G);
            sw.Write(pixelData.B);
            sw.Write(pixelData.A);
        }
    }
}
