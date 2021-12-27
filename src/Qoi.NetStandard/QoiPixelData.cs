using System;
using System.Collections.Generic;
using System.Text;

namespace Qoi.NetStandard
{
    internal readonly struct QoiPixelData
    {
        public QoiPixelData(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
        public byte A { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public int Value { get { return System.Drawing.Color.FromArgb(A, R, G, B).ToArgb(); } }
    }
}
