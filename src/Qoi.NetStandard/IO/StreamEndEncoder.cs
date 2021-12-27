using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO
{
    internal static class StreamEndEncoder
    {
        /// <summary>
        /// Specs: https://qoiformat.org/qoi-specification.pdf
        /// The byte stream's end is marked with 7 0x00 bytes followed by a
        /// single 0x01 byte.
        /// </summary>
        public static readonly byte[] STREAM_END_MARKER_BYTES = { 0, 0, 0, 0, 0, 0, 0, 1 };
        public const int STREAM_END_MARKER_SIZE = 8;
        public static void Write(BinaryWriter sw) {
            for (int i = 0; i < STREAM_END_MARKER_SIZE; i++)
            {
                sw.Write(STREAM_END_MARKER_BYTES[i]);
            }
        }
    }
}
