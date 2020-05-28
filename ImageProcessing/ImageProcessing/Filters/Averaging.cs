using System;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;

namespace ImageProcessing
{
    class LinearSmoothing : Filter
    {
        public LinearSmoothing(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;


            Kernel = new float[Diameter, Diameter];
            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    Kernel[i, j] = 1;
                }
            }

        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;
            float length = 0;

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
                    length += Kernel[j + Radius, i + Radius];
                }
            }

            r /= length;
            g /= length;
            b /= length;

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }


    }

    class ExtendedLinearSmoothing : Filter
    {
        public ExtendedLinearSmoothing(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;

            switch (Diameter)
            {
                case 3:
                    Kernel = new float[,] {
                            { 1, 2, 1 },
                            { 2, 4, 2 },
                            { 1, 2, 1 }
                        };
                    break;

                case 5:
                    Kernel = new float[,] {
                            { 1, 2, 4, 2, 1 },
                            { 2, 4, 8, 4, 2 },
                            { 4, 8, 16, 8, 4 },
                            { 2, 4, 8, 4, 2 },
                            { 1, 2, 4, 2, 1 }
                        };
                    break;

                case 7:
                    Kernel = new float[,] {
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
            float r = 0;
            float g = 0;
            float b = 0;
            float length = 0;

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
                    length += Kernel[j + Radius, i + Radius];
                }
            }

            r /= length;
            g /= length;
            b /= length;

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }


    }

    class Mediana : Filter
    {
        public Mediana(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] r = new int[Diameter * Diameter];
            int[] g = new int[Diameter * Diameter];
            int[] b = new int[Diameter * Diameter];
            int middle = (Diameter * Diameter + 1) / 2;
            int indexNum = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    r[indexNum] = neighborColor.R;
                    g[indexNum] = neighborColor.G;
                    b[indexNum] = neighborColor.B;

                    ++indexNum;
                }
            }

            Array.Sort(r);
            Array.Sort(g);
            Array.Sort(b);

            return Color.FromArgb(r[middle], g[middle], b[middle]);
        }
    }

    class GeometricMean : Filter
    {
        public GeometricMean(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double r = 1;
            double g = 1;
            double b = 1;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    r *= neighborColor.R;
                    g *= neighborColor.G;
                    b *= neighborColor.B;
                }
            }

            r = Math.Pow(r, (double)1 / (Diameter * Diameter));
            g = Math.Pow(g, (double)1 / (Diameter * Diameter));
            b = Math.Pow(b, (double)1 / (Diameter * Diameter));

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class HarmonicMean : Filter
    {
        public HarmonicMean(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
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

                    r += (double)1 / neighborColor.R;
                    g += (double)1 / neighborColor.G;
                    b += (double)1 / neighborColor.B;
                }
            }

            r = (double)(Diameter * Diameter) / r;
            g = (double)(Diameter * Diameter) / g;
            b = (double)(Diameter * Diameter) / b;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class CounterHarmonicMean : Filter
    {
        private readonly int _order;
        public CounterHarmonicMean(int diameter, int order)
        {
            this._order = order;
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double r = 0;
            double g = 0;
            double b = 0;

            long numeratorR = 0;
            long numeratorG = 0;
            long numeratorB = 0;

            long denomiratorR = 0;
            long denomiratorG = 0;
            long denomiratorB = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    numeratorR += (long)Math.Pow(neighborColor.R, _order + 1);
                    numeratorG += (long)Math.Pow(neighborColor.G, _order + 1);
                    numeratorB += (long)Math.Pow(neighborColor.B, _order + 1);

                    denomiratorR += (long)Math.Pow(neighborColor.R, _order);
                    denomiratorG += (long)Math.Pow(neighborColor.G, _order);
                    denomiratorB += (long)Math.Pow(neighborColor.B, _order);
                }
            }

            r = (long)(numeratorR / denomiratorR);
            g = (long)(numeratorG / denomiratorG);
            b = (long)(numeratorB / denomiratorB);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class MaximumFilter : Filter
    {
        public MaximumFilter(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] r = new int[Diameter * Diameter];
            int[] g = new int[Diameter * Diameter];
            int[] b = new int[Diameter * Diameter];
            int indexNum = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    r[indexNum] = neighborColor.R;
                    g[indexNum] = neighborColor.G;
                    b[indexNum] = neighborColor.B;

                    ++indexNum;
                }
            }

            Array.Sort(r);
            Array.Sort(g);
            Array.Sort(b);

            return Color.FromArgb(r[Diameter * Diameter - 1], g[Diameter * Diameter - 1], b[Diameter * Diameter - 1]);
        }
    }

    class MinimumFilter : Filter
    {
        public MinimumFilter(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] r = new int[Diameter * Diameter];
            int[] g = new int[Diameter * Diameter];
            int[] b = new int[Diameter * Diameter];
            int indexNum = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    r[indexNum] = neighborColor.R;
                    g[indexNum] = neighborColor.G;
                    b[indexNum] = neighborColor.B;

                    ++indexNum;
                }
            }

            Array.Sort(r);
            Array.Sort(g);
            Array.Sort(b);

            return Color.FromArgb(r[0], g[0], b[0]);
        }
    }

    class MidpointFilter : Filter
    {
        public MidpointFilter(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int[] r = new int[Diameter * Diameter];
            int[] g = new int[Diameter * Diameter];
            int[] b = new int[Diameter * Diameter];
            int indexNum = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    r[indexNum] = neighborColor.R;
                    g[indexNum] = neighborColor.G;
                    b[indexNum] = neighborColor.B;

                    ++indexNum;
                }
            }

            Array.Sort(r);
            Array.Sort(g);
            Array.Sort(b);

            int resultR = (r[0] + r[Diameter * Diameter - 1]) / 2;
            int resultG = (g[0] + g[Diameter * Diameter - 1]) / 2;
            int resultB = (b[0] + b[Diameter * Diameter - 1]) / 2;

            return Color.FromArgb(resultR, resultG, resultB);
        }
    }


}
