using System;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;

namespace ImageProcessing
{
    class LinearSmoothingFilter : Filter
    {
        public LinearSmoothingFilter(int diameter)
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

    class ExtendedLinearSmoothingFilter : Filter
    {
        public ExtendedLinearSmoothingFilter(int diameter)
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

    class MedianaFilter : Filter
    {
        public MedianaFilter(int diameter)
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

    class GeometricMeanFilter : Filter
    {
        public GeometricMeanFilter(int diameter)
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

    class HarmonicMean : Filter
    {
        public HarmonicMean(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double red = 0.0;
            double green = 0.0;
            double  blue = 0.0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    red += 1.0 / wrapImage[idX, idY].R;
                    green += 1.0 / wrapImage[idX, idY].G;
                    blue += 1.0 / wrapImage[idX, idY].B;
                }
            }

            red = (diameter * diameter) / red;
            green = (diameter * diameter) / green;
            blue = (diameter * diameter) / blue;

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class CounterHarmonicMeanFilter : Filter
    {
        private int order;
        public CounterHarmonicMeanFilter(int diameter, int order)
        {
            this.order = order;
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double red= 0.0;
            double green = 0.0;
            double blue = 0.0;

            double numeratorR = 0.0;
            double numeratorG = 0.0;
            double numeratorB = 0.0;

            double denomiratorR = 0.0;
            double denomiratorG = 0.0;
            double denomiratorB = 0.0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    numeratorR += Math.Pow(wrapImage[idX, idY].R, order + 1);
                    numeratorG += Math.Pow(wrapImage[idX, idY].G, order + 1);
                    numeratorB += Math.Pow(wrapImage[idX, idY].B, order + 1);

                    denomiratorR += Math.Pow(wrapImage[idX, idY].R, order);
                    denomiratorG += Math.Pow(wrapImage[idX, idY].G, order);
                    denomiratorB += Math.Pow(wrapImage[idX, idY].B, order);
                }
            }

            red = Clamp((int)(numeratorR / denomiratorR), 0, 255);
            green = Clamp((int)(numeratorG / denomiratorG), 0, 255);
            blue = Clamp((int)(numeratorB / denomiratorB), 0, 255);


            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class MaximumFilter : Filter
    {
        public MaximumFilter(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] red = new int[diameter * diameter];
            int[] green = new int[diameter * diameter];
            int[] blue = new int[diameter * diameter];
            int lastIndex = diameter * diameter - 1;
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

            return Color.FromArgb(red[lastIndex], green[lastIndex], blue[lastIndex]);
        }
    }

    class MinimumFilter : Filter
    {
        public MinimumFilter(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] red= new int[diameter * diameter];
            int[] green = new int[diameter * diameter];
            int[]  blue = new int[diameter * diameter];
            int currentIndex = 0;
            int firstIndex = 0;

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

            return Color.FromArgb(red[firstIndex], green[firstIndex], blue[firstIndex]);
        }
    }

    class MidPointFilter : Filter
    {
        public MidPointFilter(int diameter)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] red= new int[diameter * diameter];
            int[] green = new int[diameter * diameter];
            int[]  blue = new int[diameter * diameter];
            int firstIndex = 0;
            int lastIndex = diameter * diameter - 1;
            int currentIndex = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    red[currentIndex] = neighborColor.R;
                    green[currentIndex] = neighborColor.G;
                    blue[currentIndex] = neighborColor.B;

                    ++currentIndex;
                }
            }

            Array.Sort(red);
            Array.Sort(green);
            Array.Sort(blue);

            int resultR = (red[firstIndex] + red[lastIndex]) / 2;
            int resultG = (green[firstIndex] + green[lastIndex]) / 2;
            int resultB = (blue[firstIndex] + blue[lastIndex]) / 2;

            return Color.FromArgb(resultR, resultG, resultB);
        }
    }


}
