using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace QoiImageViewer.NetCore31LTS
{
    public partial class FormViewQoi : Form
    {
        public FormViewQoi()
        {
            InitializeComponent();
        }

        private void tsbOpenQoiImage_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdOpenQoiImage.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            string fileName = ofdOpenQoiImage.FileName;
            pictureBox1.BackgroundImage = DecodeQoi(fileName);
        }

        private static Image DecodeQoi(string fileName)
        {
            // Use DecodeQoi from Qoi.NetStandard library
            byte[] colorBytes = Qoi.NetStandard.QoiEncoder.DecodeQoi(fileName, out Qoi.NetStandard.QoiHeader header);

            // Copy decoded pixels into a standard bitmap image
            // that can be displayed anywhere
            return BitmapFromPixels(colorBytes, header);
        }
        private static Bitmap BitmapFromPixels(byte[] colorBytes, Qoi.NetStandard.QoiHeader header)
        {
            PixelFormat pixelFormat = header.Channels == 3 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb;
            Bitmap bitmap = new Bitmap((int) header.Width, (int) header.Height, pixelFormat);
            SetBitmpaPixels(bitmap, colorBytes);
            return bitmap;
        }
        private static void SetBitmpaPixels(Bitmap bitmap, byte[] colorBytes)
        {
            // We could set each pixel using [bitmap.SetPixel]
            // but bulk-setting all bytes is more efficient
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr iptr = bitmapData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(colorBytes, 0, iptr, colorBytes.Length);
            bitmap.UnlockBits(bitmapData);
        }
    }
}
