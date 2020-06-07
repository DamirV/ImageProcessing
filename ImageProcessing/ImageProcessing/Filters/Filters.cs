using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace ResearchWork
{
    abstract class Filter
    {
        protected int width;
        protected int height;
        protected double[,] kernel;
        protected int diameter;
        protected int radius;

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
            Bitmap resultImage = new Bitmap(sourceImage);
            width = resultImage.Width;
            height = resultImage.Height;

            int checkProgress = -1;

            using (ImageWrapper wrapImage = new ImageWrapper(resultImage))
            {
                for (int i = 0; i < height; ++i)
                {
                    if (i > checkProgress)
                    {
                        worker.ReportProgress((int)((double)i / height * 100));
                        if (worker.CancellationPending)
                        {
                            return null;
                        }

                        checkProgress += 100;
                    }

                    for (int j = 0; j < width; ++j)
                    {
                        wrapImage[j, i] = CalculateNewPixelColor(wrapImage, j, i);
                    }
                }
            }

            return resultImage;
        }
    }

}
