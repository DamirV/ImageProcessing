using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace ImageProcessing
{
    abstract class Filters
    {
        protected int Width;
        protected int Height;
        protected abstract Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y);
     
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public int BorderProcessing(int value, int min, int max)
        {
            if ((value >= min) && (value <= max))
            {
                return value;
            }
            else if (value < min)
            {
                return min + Math.Abs(min - value) - 1;
            }
            else
            {
                return max - Math.Abs(max - value) + 1;
            }
        }

        public virtual Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            //DateTime start = DateTime.Now;

            Bitmap resultImage = new Bitmap(sourceImage);
            Width = resultImage.Width;
            Height = resultImage.Height;
            int checkProgress = -1;

            using (ImageWrapper wrapImage = new ImageWrapper(resultImage))
            {
                for (int i = 0; i < Height; ++i)
                {
                    if (i > checkProgress)
                    {
                        worker.ReportProgress((int)((float)i / resultImage.Height * 100));
                        if (worker.CancellationPending)
                        {
                            return null;
                        }

                        checkProgress += 100;
                    }

                    for (int j = 0; j < Width; ++j)
                    {
                        wrapImage[j, i] = CalculateNewPixelColor(wrapImage, j, i);
                    }
                }
            }

            //TimeSpan result = DateTime.Now - start;
            //MessageBox.Show(result.TotalSeconds.ToString());
            return resultImage;
        }
    }

    class MatrixFilter : Filters
    {
        protected float[,] Kernel;
        protected int Diameter;
        protected int Radius;
        protected MatrixFilter() { }
        public MatrixFilter(float[,] kernel)
        {
            Kernel = kernel;
            Diameter = Kernel.GetLength(0);
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    Color neighborColor = wrapImage[idX, idY];

                    r += neighborColor.R * Kernel[j + Radius, i + Radius];
                    g += neighborColor.G * Kernel[j + Radius, i + Radius];
                    b += neighborColor.B * Kernel[j + Radius, i + Radius];
                }
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
