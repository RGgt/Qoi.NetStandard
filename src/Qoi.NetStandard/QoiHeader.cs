using System;

namespace Qoi.NetStandard
{
    public class QoiHeader
    {
        /// <summary>
        /// Size could be determined at run-time, but is more efficient to have it as a constant.
        /// It is the size of the QuiHeader when serialized, in bytes.
        /// </summary>
        public const int SIZE_IN_BYTES = 14;

        /// <summary>
        /// The magic bytes: "qoif"
        /// </summary>
        public static readonly byte[] MAGIC_CODE = {
            113 // 'q'
            , 111 // 'o'
            , 105 // 'i'
            , 102 // 'f'
        };

        /// <summary>
        /// Image width in pixels (BE).
        /// 4 bytes.
        /// </summary>
        public uint Width { get; set; }
        /// <summary>
        /// Image height in pixels (BE)
        /// 4 bytes.
        /// </summary>
        public uint Height { get; set; }
        /// <summary>
        /// Only 2 possible values: 3 meaningRGB, and 4 meaning RGBA
        /// </summary>
        public byte Channels { get; set; }
        /// <summary>
        /// This is purely informative. It does not change the behavior of the
        /// encoder/decoder.
        /// 0 = sRGB with linear alpha
        /// 1 = all channels linear
        /// </summary>
        public byte ColorSpace { get; set; }
    }
}
