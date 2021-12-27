using Qoi.NetStandard.IO.Encodings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qoi.NetStandard.IO
{
    internal static class DataReader
    {
        public static byte[] ReadData(QoiBytesReader sr, QoiHeader header)
        {
            QoiPixelData[] last64ColorValues = new QoiPixelData[64];
            QoiPixelData currentPixel;
            int totalNumberOfBytes, indexOfCurrentPixelFirstByte;
            // TODO: Remove this as it is now useless
            int bytesProcessed = 0;
            int consecutiveRepetitions = 0;
            DecodedData.IDecodedDataNChannels decodedDataReader;
            switch (header.Channels)
            {
                case (int)EQoiChannels.Unknown:
                    throw new Exceptions.QoiEncodingException("Invalid number of channels!");
                case (int)EQoiChannels.RGB:
                    decodedDataReader = new DecodedData.DecodedData3Channels();
                    break;
                case (int)EQoiChannels.ARGB:
                    decodedDataReader = new DecodedData.DecodedData4Channels();
                    break;
                default:
                    throw new Exceptions.QoiEncodingException("Invalid number of channels!");
            }
            totalNumberOfBytes = (int)(header.Width * header.Height * header.Channels);
            currentPixel = new QoiPixelData(255, 0, 0, 0);

            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                for (indexOfCurrentPixelFirstByte = 0; indexOfCurrentPixelFirstByte < totalNumberOfBytes; indexOfCurrentPixelFirstByte += header.Channels)
                {
                    if (consecutiveRepetitions > 0)
                    {
                        consecutiveRepetitions--;
                    }
                    else if (!sr.EndOfDataReached)
                    {
                        currentPixel = DecodePixel(sr, last64ColorValues, currentPixel, ref bytesProcessed, ref consecutiveRepetitions);
                        last64ColorValues[IndexEncoding.ComputeIndexPosition(currentPixel)] = currentPixel;
                    }

                    decodedDataReader.WriteWriteToStream(currentPixel, ms);
                }
                result = ms.ToArray();
            }
            return result;
        }

        private static QoiPixelData DecodePixel(QoiBytesReader sr, QoiPixelData[] last64ColorValues, QoiPixelData previousPixel, ref int bytesProcessed, ref int consecutiveRepetitions)
        {
            // First byte specifies the formatting used
            byte valueOfFirstByte = sr.ReadByte();
            bytesProcessed++;
            int bytesAdvancedInDecoder = 0;
            QoiPixelData currentPixel;
            if (RgbEncoding.CanDecode(valueOfFirstByte))
            {
                currentPixel = RgbEncoding.Decode(sr, previousPixel, out bytesAdvancedInDecoder);
            }
            else if (ArgbEncodingc.CanDecode(valueOfFirstByte))
            {
                currentPixel = ArgbEncodingc.Decode(sr, out bytesAdvancedInDecoder);
            }
            else if (IndexEncoding.CanDecode(valueOfFirstByte))
            {
                currentPixel = IndexEncoding.Decode(last64ColorValues, valueOfFirstByte);
            }
            else if (DiffEncoding.CanDecode(valueOfFirstByte))
            {
                currentPixel = DiffEncoding.Decode(previousPixel, valueOfFirstByte);
            }
            else if (LunaEncoding.CanDecode(valueOfFirstByte))
            {
                currentPixel = LunaEncoding.Decode(sr, previousPixel, valueOfFirstByte, out bytesAdvancedInDecoder);
            }
            else if (RunEncoding.CanDecode(valueOfFirstByte))
            {
                consecutiveRepetitions = RunEncoding.DecodeLength(valueOfFirstByte);
                currentPixel = previousPixel;
            }
            else
            {
                throw new Exceptions.QoiDencodingException("Unknown encoding flag!");
            }
            bytesProcessed += bytesAdvancedInDecoder;
            return currentPixel;
        }
    }
}
