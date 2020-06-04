using System;
using System.ComponentModel;
using System.Data.Common;
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
            double blue = 0.0;

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

    class CounterHarmonicMean : Filter
    {
        private double order;
        public CounterHarmonicMean(int diameter, double order)
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

    class Maximum : Filter
    {
        public Maximum(int diameter)
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

    class Minimum : Filter
    {
        public Minimum(int diameter)
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

    class MidPoint : Filter
    {
        public MidPoint(int diameter)
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
