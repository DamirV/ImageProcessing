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
            Diameter = diameter;
            Radius = Diameter / 2;

            CreateGaussianKernel();
        }

        public void CreateGaussianKernel()
        {
            double constant = (double)(1 / (2 * Math.PI * _sigma * _sigma));

            Kernel = new double[Diameter, Diameter];

            double norm = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    double distance = (i * i + j * j) / (_sigma * _sigma);
                    Kernel[i + Radius, j + Radius] = constant * (double)(Math.Exp(-distance));
                    norm += Kernel[i + Radius, j + Radius];
                }
            }

            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    Kernel[i, j] /= norm;
                }
            }
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double r = 0;
            double g = 0;
            double b = 0;

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
