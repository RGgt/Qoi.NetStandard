using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO.DecodedData
{
    internal interface IDecodedDataNChannels
    {
        QoiPixelData ReadFromBytes(byte[] decoded, int px_pos);
        void WriteWriteToStream(QoiPixelData px, MemoryStream ms);
    }
}
