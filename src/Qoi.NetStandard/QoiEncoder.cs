using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Qoi.NetStandard.IO;

[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute("QoiTest")]
namespace Qoi.NetStandard
{
    public static class QoiEncoder
    {
        /// <summary>
        /// Encode an array of pixels (ARGB/RGB bytes) as .qui.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelsData"></param>
        /// <param name="hasAlpha"></param>
        /// <param name="linearAlpha"></param>
        /// <returns></returns>
        public static byte[] EncodeToQoi(int width, int height, byte[] pixelsData, bool hasAlpha, bool linearAlpha)
        {
            byte[] qoiFileBytes;
            int channels = hasAlpha ? (int)EQoiChannels.ARGB : (int)EQoiChannels.RGB;
            int expectedDecodedPixelsDataLenght = width * height * channels;
            if (pixelsData == null || pixelsData.Length < expectedDecodedPixelsDataLenght)
                throw new Exceptions.QoiEncodingException("Missing (some) color data!");
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    QoiHeader header = new QoiHeader()
                    {
                        Width = (uint)width,
                        Height = (uint)height,
                        Channels = (byte)channels,
                        ColorSpace = (byte)(linearAlpha ? 0 : 1)
                    };
                    HeaderEncoder.AppendHeader(bw, header);
                    DataWriter.AppendDataAndEndOfStream(bw, header, pixelsData);
                    qoiFileBytes = ms.ToArray();
                }
            }
            return qoiFileBytes;
        }

        /// <summary>
        /// Decode an array of bytes representing the content of a .qui file. Returns the
        /// decoded pixels as ARGB bytes.
        /// </summary>
        /// <param name="qoiFileBytes"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static byte[] DecodeQoi(byte[] qoiFileBytes, out QoiHeader header)
        {
            if (qoiFileBytes==null || qoiFileBytes.Length<QoiHeader.SIZE_IN_BYTES)
                throw new Exceptions.QoiDencodingException("Missing header data!");
            using (MemoryStream stream = new MemoryStream(qoiFileBytes))
            {
                return DecodeQoi(stream, out header);
            }
        }

        /// <summary>
        /// Decodes a stream containing the .qui file bytes. It can be a stream composed in
        /// memory, through unpacking, for example. Returns the decoded pixels as ARGB bytes.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static byte[] DecodeQoi(Stream stream, out QoiHeader header)
        {
            byte[] pixelsData;
            using (QoiBytesReader br = new QoiBytesReader(stream))
            {
                header = HeaderEncoder.ReadHeader(br);
                pixelsData = DataReader.ReadData(br, header);
            }
            return pixelsData;
        }

        /// <summary>
        /// Reads a .qoi file and returns decoded pixels (ARGB bytes).
        /// Not validations or checks are made. Caller code should validate permissions, file
        /// existence and size and the rest.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static byte[] DecodeQoi(String filePath, out QoiHeader header)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return DecodeQoi(stream, out header);
            }
        }
    }
}
