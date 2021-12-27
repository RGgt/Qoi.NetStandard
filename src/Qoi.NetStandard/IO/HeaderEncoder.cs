using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO
{
    internal static class HeaderEncoder
    {
        public static QoiHeader ReadHeader(QoiBytesReader binaryReader)
        {
            int q = binaryReader.ReadByte();
            int o = binaryReader.ReadByte();
            int i = binaryReader.ReadByte();
            int f = binaryReader.ReadByte();
            if (q != QoiHeader.MAGIC_CODE[0] || o != QoiHeader.MAGIC_CODE[1] || i != QoiHeader.MAGIC_CODE[2] || f != QoiHeader.MAGIC_CODE[3])
                throw new Exceptions.QoiDencodingException("This is not a QOI header!");
            QoiHeader qoiHeader = new QoiHeader()
            {
                Width = UintFromBigEndian(binaryReader.ReadBytes(4)),
                Height = UintFromBigEndian(binaryReader.ReadBytes(4)),
                Channels = binaryReader.ReadByte(),
                ColorSpace = binaryReader.ReadByte()
            };
            if (qoiHeader.Channels == (int)EQoiChannels.Unknown)
                qoiHeader.Channels = (int)EQoiChannels.RGB;
            return qoiHeader;
        }
        private static uint UintFromBigEndian(byte[] bytes)
        {
            EncodingHelper.ToBigEndian(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static void AppendHeader(BinaryWriter binaryWriter, QoiHeader header)
        {
            binaryWriter.Write(QoiHeader.MAGIC_CODE);
            binaryWriter.Write(ToBigEndian(header.Width));
            binaryWriter.Write(ToBigEndian(header.Height));
            binaryWriter.Write(header.Channels);
            binaryWriter.Write(header.ColorSpace);
        }
        private static byte[] ToBigEndian(uint integer)
        {
            byte[] bytes = BitConverter.GetBytes(integer);
            EncodingHelper.ToBigEndian(bytes);
            return bytes;
        }
    }
}
