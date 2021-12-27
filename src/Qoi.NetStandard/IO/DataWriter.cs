using Qoi.NetStandard.IO.Encodings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace Qoi.NetStandard.IO
{
    internal static class DataWriter
    {
        public static void AppendDataAndEndOfStream(BinaryWriter sw, QoiHeader header, byte[] decoded)
        {
            int consecutiveRepetitions;
            int totalNumberOfBytes;
            int indexOfLastPixelFirstByte;
            int indexOfCurrentPixelFirstByte;
            QoiPixelData currentPixel, previousPixel;
            QoiPixelData[] last64ColorValues = new QoiPixelData[64];
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

            consecutiveRepetitions = 0;
            previousPixel = new QoiPixelData(255, 0, 0, 0);
            totalNumberOfBytes = (int)(header.Width * header.Height * header.Channels);
            indexOfLastPixelFirstByte = totalNumberOfBytes - header.Channels;

            for (indexOfCurrentPixelFirstByte = 0; indexOfCurrentPixelFirstByte < totalNumberOfBytes; indexOfCurrentPixelFirstByte += header.Channels)
            {
                currentPixel = decodedDataReader.ReadFromBytes(decoded, indexOfCurrentPixelFirstByte);

                currentPixel = EncodePixel(sw, ref consecutiveRepetitions, indexOfLastPixelFirstByte, indexOfCurrentPixelFirstByte, currentPixel, previousPixel, last64ColorValues);
                previousPixel = currentPixel;
            }

            StreamEndEncoder.Write(sw);
        }
        private static QoiPixelData EncodePixel(BinaryWriter sw, ref int consecutiveRepetitions, int indexOfLastPixelFirstByte, int indexOfCurrentPixelFirstByte,  QoiPixelData currentPixel, QoiPixelData previousPixel, QoiPixelData[] last64ColorValues)
        {
            if (currentPixel.Value == previousPixel.Value)
            {
                // If is a repetition, write it down
                consecutiveRepetitions++;
                if (RunEncoding.IsMuximumRuns(consecutiveRepetitions)
                    || IsLastPixel(indexOfLastPixelFirstByte, indexOfCurrentPixelFirstByte))
                {
                    RunEncoding.Encode(sw, consecutiveRepetitions);
                    consecutiveRepetitions = 0;
                }
            }
            else
            {
                if (consecutiveRepetitions > 0)
                {
                    // If it is not a repetition, but this is the first pixel after one, write this down.
                    RunEncoding.Encode(sw, consecutiveRepetitions);
                    consecutiveRepetitions = 0;
                }

                int indexInLast64ColorValues = IndexEncoding.ComputeIndexPosition(currentPixel);
                if (last64ColorValues[indexInLast64ColorValues].Value == currentPixel.Value)
                {
                    // If it is the same value as at the calculated index, then write it as QOI_OP_INDEX
                    IndexEncoding.Encode(sw, indexInLast64ColorValues);
                }
                else
                {
                    last64ColorValues[indexInLast64ColorValues] = currentPixel;

                    if (currentPixel.A == previousPixel.A)
                    {
                        // If is same alpha, use Diff, Luna or RGB and copy alpha from
                        // previous pixel while decoding.

                        int vr = currentPixel.R - previousPixel.R;
                        int vg = currentPixel.G - previousPixel.G;
                        int vb = currentPixel.B - previousPixel.B;

                        int vg_r = vr - vg;
                        int vg_b = vb - vg;

                        if (DiffEncoding.CanEncode(vr, vg, vb))
                        {
                            DiffEncoding.Encode(sw, vr, vg, vb);
                        }
                        else if (LunaEncoding.CanEncode(vg, vg_r, vg_b))
                        {
                            LunaEncoding.Encode(sw, vg, vg_r, vg_b);
                        }
                        else
                        {
                            RgbEncoding.Encode(sw, currentPixel);
                        }
                    }
                    else
                    {
                        ArgbEncodingc.Encode(sw, currentPixel);
                    }
                }
            }
            return currentPixel;
        }
        private static bool IsLastPixel(int indexOfLastPixelFirstByte, int indexOfCurrentPixelFirstByte)
        {
            return indexOfCurrentPixelFirstByte == indexOfLastPixelFirstByte;
        }
    }
}
