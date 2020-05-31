using System;
using System.ComponentModel;
using System.Drawing;

namespace ImageProcessing
{
    class GaussianFilter : Filter
    {
        private readonly double _sigma;
        public GaussianFilter(int diameter, double sigma)
        {
            _sigma = sigma;
            diameter = diameter;
            radius = diameter / 2;

            CreateGaussiankernel();
        }

        public void CreateGaussiankernel()
        {
            double constant = (double)(1 / (2 * Math.PI * _sigma * _sigma));

            kernel = new double[diameter, diameter];

            double norm = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    double distance = (i * i + j * j) / (_sigma * _sigma);
                    kernel[i + radius, j + radius] = constant * (double)(Math.Exp(-distance));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for (int i = 0; i < diameter; ++i)
            {
                for (int j = 0; j < diameter; ++j)
                {
                    kernel[i, j] /= norm;
                }
            }
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);
                    Color neighborColor = wrapImage[idX, idY];

                    r += neighborColor.R * kernel[j + radius, i + radius];
                    g += neighborColor.G * kernel[j + radius, i + radius];
                    b += neighborColor.B * kernel[j + radius, i + radius];
                }
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
