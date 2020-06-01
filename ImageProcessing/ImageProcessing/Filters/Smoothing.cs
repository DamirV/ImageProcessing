using System;
using System.ComponentModel;
using System.Drawing;

namespace ImageProcessing
{
    class Gaussian : Filter
    {
        private double sigma;
        public Gaussian(int diameter, double sigma)
        {
            this.sigma = sigma;
            this.diameter = diameter;
            this.radius = diameter / 2;
            this.kernel = new double[diameter, diameter];
            CreateGaussiankernel();
        }

        public void CreateGaussiankernel()
        {
            double constant = 1.0 / (2 * Math.PI * sigma * sigma);
            double norm = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    double distance = (i * i + j * j) / (sigma * sigma);
                    kernel[j + radius, i + radius] = constant * Math.Exp(-distance);
                    norm += kernel[j + radius, i + radius];
                }
            }

            for (int i = 0; i < diameter; ++i)
            {
                for (int j = 0; j < diameter; ++j)
                {
                    kernel[j, i] /= norm;
                }
            }
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double red = 0.0;
            double green = 0.0;
            double blue = 0.0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    red += wrapImage[idX, idY].R * kernel[j + radius, i + radius];
                    green += wrapImage[idX, idY].G * kernel[j + radius, i + radius];
                    blue += wrapImage[idX, idY].B * kernel[j + radius, i + radius];
                }
            }

            red = Clamp((int)red, 0, 255);
            green = Clamp((int)green, 0, 255);
            blue = Clamp((int)blue, 0, 255);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }
}
