﻿using System;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;

namespace ImageProcessing
{
    class LinearSmoothing : Filter
    {
        public LinearSmoothing(int diameter, bool extendedMask)
        {
            Diameter = diameter;
            Radius = Diameter / 2;

            if (!extendedMask)
            {
                switch (Diameter)
                {
                    case 3:
                        Kernel = new float[,] {
                            { 1, 1, 1 },
                            { 1, 1, 1 },
                            { 1, 1, 1 }
                        };
                        break;

                    case 5:
                        Kernel = new float[,] {
                            {1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1 },
                            {1, 1, 1, 1, 1 },
                            {1, 1, 1, 1, 1 },
                            {1, 1, 1, 1, 1 }
                        };
                        break;

                    case 7:
                        Kernel = new float[,] {
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                        };
                        break;

                    default:
                        Kernel = new float[,] {
                            { 1, 1, 1 },
                            { 1, 1, 1 },
                            { 1, 1, 1 }
                        };
                        Diameter = 3;
                        Radius = Diameter / 2;
                        break;
                }
            }
            else
            {
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
                            {2, 4, 8, 4, 2 },
                            {4, 8, 16, 8, 4 },
                            {2, 4, 8, 4, 2 },
                            { 1, 2, 4, 2, 1 }
                        };
                        break;

                    case 7:
                        Kernel = new float[,] {
                            {1, 2, 4, 8, 4, 2, 1},
                            {2, 4, 8, 16, 8, 4, 2},
                            {4, 8, 16, 32, 16, 8, 4},
                            {8, 16, 32, 64, 32, 16, 8},
                            {4, 8, 16, 32, 16, 8, 4},
                            {2, 4, 8, 16, 8, 4, 2},
                            {1, 2, 4, 8, 4, 2, 1},
                        };
                        break;

                    default:
                        Kernel = new float[,] {
                            { 1, 2, 1 },
                            { 2, 4, 2 },
                            { 1, 2, 1 }
                        };
                        Diameter = 3;
                        Radius = Diameter / 2;
                        break;
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

                    r += (double)1/neighborColor.R;
                    g += (double)1/neighborColor.G;
                    b += (double)1/neighborColor.B;
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

            double numeratorR = 0;
            double numeratorG = 0;
            double numeratorB = 0;

            double denomiratorR = 0;
            double denomiratorG = 0;
            double denomiratorB = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];

                    numeratorR += Math.Pow(neighborColor.R, _order + 1);
                    numeratorG += Math.Pow(neighborColor.G, _order + 1);
                    numeratorB += Math.Pow(neighborColor.B, _order + 1);

                    denomiratorR += Math.Pow(neighborColor.R, _order);
                    denomiratorG += Math.Pow(neighborColor.G, _order);
                    denomiratorB += Math.Pow(neighborColor.B, _order);
                }
            }

            r = numeratorR / denomiratorR;
            g = numeratorG / denomiratorG;
            b = numeratorB / denomiratorB;

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
