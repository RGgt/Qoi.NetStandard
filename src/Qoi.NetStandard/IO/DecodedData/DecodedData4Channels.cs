using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.DecodedData
{
    internal class DecodedData4Channels : IDecodedDataNChannels
    {
        public QoiPixelData ReadFromBytes(byte[] decoded, int px_pos)
        {
            byte[] bytes = new byte[4] { decoded[px_pos + 0], decoded[px_pos + 1], decoded[px_pos + 2], decoded[px_pos + 3] };
            //if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            System.Drawing.Color c = System.Drawing.Color.FromArgb(BitConverter.ToInt32(bytes, 0));
            return new QoiPixelData(c.A, c.R, c.G, c.B);
        }
        public void WriteWriteToStream(QoiPixelData px, MemoryStream ms)
        {
            System.Drawing.Color c = System.Drawing.Color.FromArgb(px.A, px.R, px.G, px.B);
            byte[] bytes = BitConverter.GetBytes(c.ToArgb());
            //if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            ms.Write(bytes, 0, bytes.Length);
        }
    }
}