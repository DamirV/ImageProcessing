using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace ResearchWork
{
    public class ImageWrapper : IDisposable, IEnumerable<Point>
    {
        public int width { get; private set; }
        public int height { get; private set; }
        public Color defaultColor { get; set; }

        private byte[] data;
        private byte[] outData;
        private int stride;
        private BitmapData imageData;
        private Bitmap image;

        public ImageWrapper(Bitmap image, bool copySourceToOutput = false)
        {
            width = image.Width;
            height = image.Height;
            this.image = image;

            imageData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            stride = imageData.Stride;

            data = new byte[stride * height];
            System.Runtime.InteropServices.Marshal.Copy(imageData.Scan0, data, 0, data.Length);

            outData = copySourceToOutput ? (byte[])data.Clone() : new byte[stride * height];
        }

        public Color this[int x, int y]
        {
            get
            {
                var i = GetIndex(x, y);
                return i < 0 ? defaultColor : Color.FromArgb(data[i + 3], data[i + 2], data[i + 1], data[i]);
            }
            set
            {
                var i = GetIndex(x, y);
                if (i >= 0)
                {
                    outData[i] = value.B;
                    outData[i + 1] = value.G;
                    outData[i + 2] = value.R;
                    outData[i + 3] = value.A;
                };
            }
        }
        public Color this[Point point]
        {
            get { return this[point.X, point.Y]; }
            set { this[point.X, point.Y] = value; }
        }

        public void SetPixel(Point point, double red, double green, double blue)
        {
            if (red < 0) red = 0;
            if (red >= 256) red = 255;
            if (green < 0) green = 0;
            if (green >= 256) green = 255;
            if (blue < 0) blue = 0;
            if (blue >= 256) blue = 255;

            this[point.X, point.Y] = Color.FromArgb((int)red, (int)green, (int)blue);
        }

        public void SetPixel(Point point, Color color)
        {
            this[point.X, point.Y] = color;
        }

        int GetIndex(int x, int y)
        {
            return (x < 0 || x >= width || y < 0 || y >= height) ? -1 : x * 4 + y * stride;
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.Copy(outData, 0, imageData.Scan0, outData.Length);
            image.UnlockBits(imageData);
        }

        public IEnumerator<Point> GetEnumerator()
        {
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    yield return new Point(x, y);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SwapBuffers()
        {
            var temp = data;
            data = outData;
            outData = temp;
        }
    }
}
