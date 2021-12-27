using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.Encodings
{
    /// <summary>
    /// Specs: https://qoiformat.org/qoi-specification.pdf
    /// ┌─ QOI_OP_RUN ────────────┐
    /// │ Byte[0]                 │
    /// │ 7 6   |     5 4 3 2 1 0 │
    /// │───────┼─────────────────│
    /// │ 1 1   │             run │
    /// └───────┴─────────────────┘
    /// 2 - bit tag b11
    /// 6 - bit run - length repeating the previous pixel: 1..62
    /// The run-length is stored with a bias of - 1.Note that the run -
    /// lengths 63 and 64(b111110 and b111111) are illegal as they are
    /// occupied by the QOI_OP_RGB and QOI_OP_RGBA tags.
    /// ---
    /// The value itself was already written at the initial occurrence,
    /// so only log the number of repetitions encountered so far.
    /// </summary>
    internal static class RunEncoding
    {
        public const byte QOI_OP_RUN = 0b11000000;
        public static bool CanDecode(byte marker)
        {
            return QOI_OP_RUN == (marker & EncodingHelper.QOI_MASK_2_BITS);
        }
        public static bool IsMuximumRuns(int consecutiveRepetitions)
        {
            // Number of repetitions is saved on 6 bits prefixed by two bits with the value b11.
            // Even if the same value keeps repeating, we should stop at the re-occurrence
            // 62, as values 63 and 64 (b111110 and b111111) are illegal, being occupied
            // by QOI_OP_RGB and QOI_OP_RGBA tags (b11111110 and b11111111).
            return consecutiveRepetitions == 62;
        }
        public static int DecodeLength( byte marker)
        {
            return marker & 0b00111111;
        }
        public static void Encode(BinaryWriter sw, int consecutiveRepetitions)
        {
            sw.Write((byte)(QOI_OP_RUN + consecutiveRepetitions - 1 /* bias */));
        }
    }
}
