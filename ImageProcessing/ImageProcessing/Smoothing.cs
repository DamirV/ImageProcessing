using System;
using System.Drawing;

namespace ImageProcessing
{
    class GaussianFilter : MatrixFilter
    {
        private readonly float _sigma;
        public GaussianFilter(int diameter, float sigma)
        {
            _sigma = sigma;
            Diameter = diameter;
            Radius = Diameter / 2;

            CreateGaussianKernel();
        }

        public void CreateGaussianKernel()
        {
            float constant = (float)(1 / (2 * Math.PI * _sigma * _sigma));

            Kernel = new float[Diameter, Diameter];

            float norm = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    float distance = (i * i + j * j) / (_sigma * _sigma);
                    Kernel[i + Radius, j + Radius] = constant * (float)(Math.Exp(-distance));
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

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radius = Diameter / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for (int l = -radius; l <= radius; l++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    int idX = BorderProcessing(x + k, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * Kernel[k + radius, l + radius];
                    resultG += neighborColor.G * Kernel[k + radius, l + radius];
                    resultB += neighborColor.B * Kernel[k + radius, l + radius];
                }
            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }
    }

    class LinearSmoothing : MatrixFilter
    {
        public LinearSmoothing(int diameter, bool extendedMask)
        {
            Diameter = diameter;
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

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            float r = 0;
            float g = 0;
            float b = 0;
            int radius = Diameter / 2;
            float length = 0;

            for (int i = -radius; i <= radius; ++i)
                for (int j = -radius; j <= radius; ++j)
                {

                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    r += neighborColor.R * Kernel[i + radius, j + radius];
                    g += neighborColor.G * Kernel[i + radius, j + radius];
                    b += neighborColor.B * Kernel[i + radius, j + radius];
                    length += Kernel[i + radius, j + radius];
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

    class Mediana : MatrixFilter
    {
        public Mediana(int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
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
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

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
}
