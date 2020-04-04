using System;
using System.Drawing;
using System.ComponentModel;

namespace ImageProcessing
{
    abstract class Filters
    {
        protected abstract Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y);

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
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; ++i)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                {
                    return null;
                }

                for (int j = 0; j < sourceImage.Height; ++j)
                {
                    resultImage.SetPixel(i, j, CalculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    }
    class BlackAndWhite : Filters
    {
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int r = sourceImage.GetPixel(x, y).R;
            int g = sourceImage.GetPixel(x, y).G;
            int b = sourceImage.GetPixel(x, y).B;

            int result = (r + g + b) / 3;

            return Color.FromArgb(result, result, result);
        }
    }

    class SaltPepper : Filters
    {
        private readonly int[,] _matrixOfRandomNumbers;
        private readonly int _saltPercent;
        private readonly int _pepperPercent;

        public SaltPepper(int width, int height, int saltPercent, int pepperPercent)
        {
            _matrixOfRandomNumbers = new int[width, height];
            _saltPercent = saltPercent;
            _pepperPercent = pepperPercent;
            Random rnd = new Random();

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    _matrixOfRandomNumbers[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            if (_matrixOfRandomNumbers[x, y] <= _saltPercent)
            {
                return Color.FromArgb(255, 255, 255);
            }
            if (_matrixOfRandomNumbers[x, y] > 100 - _pepperPercent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return sourceImage.GetPixel(x, y);
        }
    }

    class SaltAndPepper : Filters
    {
        private readonly int[,] _matrixOfRandomNumbers;
        private readonly int _percent;
        private readonly int _mod;
        public SaltAndPepper(int width, int height, int percent, int mod)
        {
            _percent = percent;
            _mod = mod;

            _matrixOfRandomNumbers = new int[width, height];
            Random rnd = new Random();

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                   _matrixOfRandomNumbers[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            switch (_mod)
            {
                case 1:
                    return CalculateSalt(sourceImage, x, y);

                case 2:
                    return CalculatePepper(sourceImage, x, y);

                case 3:
                    return CalculateSaltAndPepper(sourceImage, x, y);

                default:
                    return sourceImage.GetPixel(x, y);
            }
        }

        private Color CalculateSalt(Bitmap sourceImage, int x, int y)
        {
            if (_matrixOfRandomNumbers[x, y] < _percent)
            {
                return Color.FromArgb(255, 255, 255);
            }

            return sourceImage.GetPixel(x, y);
        }
        private Color CalculatePepper(Bitmap sourceImage, int x, int y)
        {
            if (_matrixOfRandomNumbers[x, y] < _percent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return sourceImage.GetPixel(x, y);
        }
        private Color CalculateSaltAndPepper(Bitmap sourceImage, int x, int y)
        {
            if (_matrixOfRandomNumbers[x, y] <= _percent / 2)
            {
                return Color.FromArgb(255, 255, 255);
            }
            if (_matrixOfRandomNumbers[x, y] > 100 - _percent / 2)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return sourceImage.GetPixel(x, y);
        }
    }

    class Salt : Filters
    {
        private readonly int[,] _matrixOfRandomNumbers;
        private readonly int _percent;
        public Salt(int width, int height, int percent)
        {
            _matrixOfRandomNumbers = new int[width, height];
            Random rnd = new Random();
            _percent = percent;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    _matrixOfRandomNumbers[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            if (_matrixOfRandomNumbers[x, y] < _percent)
            {
                return Color.FromArgb(255, 255, 255);
            }

            return sourceImage.GetPixel(x, y);
        }
    }

    class Pepper : Filters
    {
        private readonly int[,] _matrixOfRandomNumbers;
        private readonly int _percent;
        public Pepper(int width, int height, int percent)
        {
            _matrixOfRandomNumbers = new int[width, height];
            Random rnd = new Random();
            _percent = percent;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    _matrixOfRandomNumbers[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
          
            if (_matrixOfRandomNumbers[x, y] < _percent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return sourceImage.GetPixel(x, y);
        }
    }
    internal class MatrixFilter : Filters
    {
        protected float[,] Kernel;
        protected int Diameter;
        protected int Radius;
        protected MatrixFilter()
        {

        }

        public MatrixFilter(float[,] kernel)
        {
            Kernel = kernel;
            Diameter = Kernel.GetLength(0);
            Radius = Diameter / 2;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    r += neighborColor.R * Kernel[j + Radius, i + Radius];
                    g += neighborColor.G * Kernel[j + Radius, i + Radius];
                    b += neighborColor.B * Kernel[j + Radius, i + Radius];
                }
            }

            r = Clamp((int) r, 0, 255);
            g = Clamp((int) g, 0, 255);
            b = Clamp((int) b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }

    }

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

    class SobelFilter : MatrixFilter
    {
        private readonly float[,] _kernelX;
        private readonly float[,] _kernelY;

        public SobelFilter()
        {
            Diameter = 3;
            Radius = Diameter / 2;
            _kernelX = new float[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            _kernelY = new float[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        }

        public SobelFilter(float[,] kernel, int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            _kernelX = new float[Diameter, Diameter];
            _kernelY = new float[Diameter, Diameter];
            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    _kernelX[i, j] = kernel[i, j];
                    _kernelY[i, j] = kernel[j, i];
                }
            }
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            float xR = 0, xG = 0, xB = 0;
            float yR = 0, yG = 0, yB = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    xR += neighborColor.R * _kernelX[i + Radius, j + Radius];
                    xG += neighborColor.G * _kernelX[i + Radius, j + Radius];
                    xB += neighborColor.B * _kernelX[i + Radius, j + Radius];

                    yR += neighborColor.R * _kernelY[i + Radius, j + Radius];
                    yG += neighborColor.G * _kernelY[i + Radius, j + Radius];
                    yB += neighborColor.B * _kernelY[i + Radius, j + Radius];
                }
            }

            int r = Clamp((int)Math.Sqrt(xR * xR + yR * yR), 0, 255);
            int g = Clamp((int)Math.Sqrt(xG * xG + yG * yG), 0, 255);
            int b = Clamp((int)Math.Sqrt(xB * xB + yB * yB), 0, 255);

            return Color.FromArgb(r, g, b);
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

            return Color.FromArgb((int) r, (int) g, (int) b);
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

    class Laplass : MatrixFilter
    {
        private readonly bool _restoredBackground;
        private readonly float _multiplier;
        public Laplass(bool extendedMask, bool restoredBackground, float multiplier = 1)
        {
            _restoredBackground = restoredBackground;
            _multiplier = multiplier;

            if (!extendedMask)
            {
                Kernel = new float[,]
                {
                    { 0, 1, 0 },
                    { 1, -4, 1 },
                    { 0, 1, 0 }
                };
            }
            else
            {
                Kernel = new float[,]
                {
                    { 1, 1, 1 },
                    { 1, -8, 1 },
                    { 1, 1, 1 }
                };
            }

            Diameter = 3;
            Radius = Diameter / 2;
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;
            Color neighborColor;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);
                    neighborColor = sourceImage.GetPixel(idX, idY);

                    r += Kernel[i + Radius, j + Radius] * neighborColor.R;
                    g += Kernel[i + Radius, j + Radius] * neighborColor.G;
                    b += Kernel[i + Radius, j + Radius] * neighborColor.B;
                }
            }

            if (_restoredBackground)
            {
                neighborColor = sourceImage.GetPixel(x, y);
                r = neighborColor.R + (int)(-_multiplier * r);
                g = neighborColor.G + (int)(-_multiplier * g);
                b = neighborColor.B + (int)(-_multiplier * b);
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class FrequencyIncrease : MatrixFilter
    {
        private readonly float _multiplier;
        private readonly float _sigma;

        public FrequencyIncrease(float multiplier, int diameter, float sigma)
        {
            _multiplier = multiplier;
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
            float r = 0;
            float g = 0;
            float b = 0;
            Color neighborColor;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);
                    neighborColor = sourceImage.GetPixel(idX, idY);

                    r += neighborColor.R * Kernel[j + Radius, i + Radius];
                    g += neighborColor.G * Kernel[j + Radius, i + Radius];
                    b += neighborColor.B * Kernel[j + Radius, i + Radius];
                }

            }

            neighborColor = sourceImage.GetPixel(x, y);
            r = Clamp(neighborColor.R + (int) (_multiplier * (neighborColor.R - r)), 0, 255);
            g = Clamp(neighborColor.G + (int) (_multiplier * (neighborColor.G - g)), 0, 255);
            b = Clamp(neighborColor.B + (int) (_multiplier * (neighborColor.B - b)), 0, 255);


            return Color.FromArgb((int) r, (int) g, (int) b);
        }


    }

    class Dilation : MatrixFilter
    {
        public Dilation(int diameter)
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

        public Dilation(int diameter, float[,] kernel)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            Kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int max = 0;

            Color resultColor = Color.Black;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    int intensity = neighborColor.R;

                    if ((Kernel[j + Radius, i + Radius] > 0) && (intensity > max))
                    {
                        max = intensity;
                        resultColor = neighborColor;
                    }
                }
            }

            return resultColor;
        }
    }

    class Erosion : MatrixFilter
    {

        public Erosion(int diameter)
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

        public Erosion(int diameter, float[,] kernel)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            Kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int min = 255;

            Color resultColor = Color.White;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    int intensity = neighborColor.R;

                    if ((Kernel[j + Radius, i + Radius] > 0) && (intensity < min))
                    {
                        min = intensity;
                        resultColor = neighborColor;
                    }
                }
            }

            return resultColor;
        }
    }

    class Opening : MatrixFilter
    {

        public Opening(int diameter)
        {
            Diameter = diameter;
            Kernel = new float[Diameter, Diameter];

            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    Kernel[i, j] = 1;
                }
            }
        }

        public Opening()
        {

        }


        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {

            Erosion er = new Erosion(Diameter);
            Dilation di = new Dilation(Diameter);

            return di.ProcessImage(er.ProcessImage(sourceImage, worker), worker);

        }
    }

    class Closing : MatrixFilter
    {

        public Closing()
        {

        }

        public Closing(int diameter)
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

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation di = new Dilation(Diameter);
            Erosion er = new Erosion(Diameter);
            return er.ProcessImage(di.ProcessImage(sourceImage, worker), worker);
        }
    }

    class BlackHoles : MatrixFilter
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _percent;
        public BlackHoles()
        {

        }

        public BlackHoles(int diameter, int width, int height, int percent)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            _width = width;
            _height = height;
            _percent = percent;

            Kernel = new float[Diameter, Diameter];

            switch (Diameter)
            {
                case 3:
                    Kernel = new float[,]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case 5:
                    Kernel = new float[,]
                    {
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0}
                    };
                    break;

                case 7:
                    Kernel = new float[,]
                    {
                        {0, 0, 1, 1, 1, 0, 0},
                        {0, 1, 1, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 1, 1, 0},
                        {0, 0, 1, 1, 1, 0, 0},
                    };
                    break;

                default:
                    Kernel = new float[,]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    Diameter = 3;
                    Radius = Diameter / 2;
                    break;
            }
        }

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap tempBitmap = new Bitmap(_width, _height);

            for (int i = 0; i < _width; ++i)
            {
                for (int j = 0; j < _height; ++j)
                {
                    tempBitmap.SetPixel(i, j, Color.White);
                }
            }

            Pepper pe = new Pepper(_width, _height, _percent);
            Erosion er = new Erosion(Diameter, Kernel);
            tempBitmap = er.ProcessImage(pe.ProcessImage(tempBitmap, worker), worker);

            for (int i = 0; i < _width; ++i)
            {
                for (int j = 0; j < _height; ++j)
                {
                    Color neighborColor = tempBitmap.GetPixel(i, j);
                    if (neighborColor.R != 0 && neighborColor.G != 0 && neighborColor.B != 0)
                    {
                        tempBitmap.SetPixel(i, j, sourceImage.GetPixel(i, j));
                    }
                }
            }

            return tempBitmap;
        }
    }

    class WhiteHoles : MatrixFilter
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _percent;
        public WhiteHoles()
        {

        }

        public WhiteHoles(int diameter, int width, int height, int percent)
        {
            Diameter = diameter;
            _width = width;
            _height = height;
            _percent = percent;

            Kernel = new float[Diameter, Diameter];

            switch (Diameter)
            {
                case 3:
                    Kernel = new float[,]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case 5:
                    Kernel = new float[,]
                    {
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0}
                    };
                    break;

                case 7:
                    Kernel = new float[,]
                    {
                        {0, 0, 1, 1, 1, 0, 0},
                        {0, 1, 1, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 1, 1, 0},
                        {0, 0, 1, 1, 1, 0, 0},
                    };
                    break;

                default:
                    Kernel = new float[,]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    Diameter = 3;
                    Radius = Diameter / 2;
                    break;
            }
        }

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap tempBitmap = new Bitmap(_width, _height);

            for (int i = 0; i < _width; ++i)
            {
                for (int j = 0; j < _height; ++j)
                {
                    tempBitmap.SetPixel(i, j, Color.Black);
                }
            }

            Salt sa = new Salt(_width, _height, _percent);
            Dilation di = new Dilation(Diameter, Kernel);
            tempBitmap = di.ProcessImage(sa.ProcessImage(tempBitmap, worker), worker);

            for (int i = 0; i < _width; ++i)
            {
                for (int j = 0; j < _height; ++j)
                {
                    Color neighborColor = tempBitmap.GetPixel(i, j);
                    if (neighborColor.R != 255 && neighborColor.G != 255 && neighborColor.B != 255)
                    {
                        tempBitmap.SetPixel(i, j, sourceImage.GetPixel(i, j));
                    }
                }
            }

            return tempBitmap;
        }
    }

}
