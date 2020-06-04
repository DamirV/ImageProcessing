using System;
using System.ComponentModel;
using System.Drawing;

namespace ResearchWork
{
    class GeometricMean : Filter
    {
        public GeometricMean(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double red = 1.0;
            double green = 1.0;
            double blue = 1.0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    red *= wrapImage[idX, idY].R;
                    green *= wrapImage[idX, idY].G;
                    blue *= wrapImage[idX, idY].B;
                }
            }

            red = Math.Pow(red, 1.0 / (diameter * diameter));
            green = Math.Pow(green, 1.0 / (diameter * diameter));
            blue = Math.Pow(blue, 1.0 / (diameter * diameter));

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }
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

    class LinearSmoothing : Filter
    {
        public LinearSmoothing(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
            this.kernel = new double[diameter, diameter];

            for (int i = 0; i < diameter; ++i)
            {
                for (int j = 0; j < diameter; ++j)
                {
                    this.kernel[i, j] = 1;
                }
            }

        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double red = 0.0;
            double green = 0.0;
            double blue = 0.0;
            double length = 0.0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    red += wrapImage[idX, idY].R * kernel[j + radius, i + radius];
                    green += wrapImage[idX, idY].G * kernel[j + radius, i + radius];
                    blue += wrapImage[idX, idY].B * kernel[j + radius, i + radius];

                    length += kernel[j + radius, i + radius];
                }
            }

            red /= length;
            green /= length;
            blue /= length;

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class ExtendedLinearSmoothing : Filter
    {
        public ExtendedLinearSmoothing(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;

            switch (diameter)
            {
                case 3:
                    this.kernel = new double[,] {
                            { 1, 2, 1 },
                            { 2, 4, 2 },
                            { 1, 2, 1 }
                        };
                    break;

                case 5:
                    this.kernel = new double[,] {
                            { 1, 2, 4, 2, 1 },
                            { 2, 4, 8, 4, 2 },
                            { 4, 8, 16, 8, 4 },
                            { 2, 4, 8, 4, 2 },
                            { 1, 2, 4, 2, 1 }
                        };
                    break;

                case 7:
                    this.kernel = new double[,] {
                            { 1, 2, 4, 8, 4, 2, 1},
                            { 2, 4, 8, 16, 8, 4, 2},
                            { 4, 8, 16, 32, 16, 8, 4},
                            { 8, 16, 32, 64, 32, 16, 8},
                            { 4, 8, 16, 32, 16, 8, 4},
                            { 2, 4, 8, 16, 8, 4, 2},
                            { 1, 2, 4, 8, 4, 2, 1},
                        };
                    break;
            }

        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double red = 0.0;
            double green = 0.0;
            double blue = 0.0;
            double length = 0.0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    red += wrapImage[idX, idY].R * kernel[j + radius, i + radius];
                    green += wrapImage[idX, idY].G * kernel[j + radius, i + radius];
                    blue += wrapImage[idX, idY].B * kernel[j + radius, i + radius];

                    length += kernel[j + radius, i + radius];
                }
            }

            red /= length;
            green /= length;
            blue /= length;

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class Mediana : Filter
    {
        public Mediana(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] red = new int[diameter * diameter];
            int[] green = new int[diameter * diameter];
            int[] blue = new int[diameter * diameter];
            int middle = (diameter * diameter + 1) / 2;
            int currentIndex = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    red[currentIndex] = wrapImage[idX, idY].R;
                    green[currentIndex] = wrapImage[idX, idY].G;
                    blue[currentIndex] = wrapImage[idX, idY].B;

                    ++currentIndex;
                }
            }

            Array.Sort(red);
            Array.Sort(green);
            Array.Sort(blue);

            return Color.FromArgb(red[middle], green[middle], blue[middle]);
        }
    }
}
