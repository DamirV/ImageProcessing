using System;
using System.Drawing;

namespace ImageProcessing
{
    class SobelFilter : Filter
    {
        private readonly double[,] _kernelX;
        private readonly double[,] _kernelY;

        public SobelFilter()
        {
            Diameter = 3;
            Radius = Diameter / 2;
            _kernelX = new double[,] { 
                { -1, 0, 1 },
                { -2, 0, 2 }, 
                { -1, 0, 1 } };
            _kernelY = new double[,]
            {
                { -1, -2, -1 }, 
                { 0, 0, 0 }, 
                { 1, 2, 1 }
            };
        }

        public SobelFilter(double[,] kernel, int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            _kernelX = new double[Diameter, Diameter];
            _kernelY = new double[Diameter, Diameter];
            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    _kernelX[i, j] = kernel[i, j];
                    _kernelY[i, j] = kernel[j, i];
                }
            }
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            double xR = 0, xG = 0, xB = 0;
            double yR = 0, yG = 0, yB = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    Color neighborColor = wrapImage[idX, idY];

                    xR += neighborColor.R * _kernelX[j + Radius, i + Radius];
                    xG += neighborColor.G * _kernelX[j + Radius, i + Radius];
                    xB += neighborColor.B * _kernelX[j + Radius, i + Radius];

                    yR += neighborColor.R * _kernelY[j + Radius, i + Radius];
                    yG += neighborColor.G * _kernelY[j + Radius, i + Radius];
                    yB += neighborColor.B * _kernelY[j + Radius, i + Radius];
                }
            }

            int r = Clamp((int)Math.Sqrt(xR * xR + yR * yR), 0, 255);
            int g = Clamp((int)Math.Sqrt(xG * xG + yG * yG), 0, 255);
            int b = Clamp((int)Math.Sqrt(xB * xB + yB * yB), 0, 255);

            return Color.FromArgb(r, g, b);
        }
    }

    class SobelFilterRGB : Filter
    {
        private readonly double[,] _kernelX;
        private readonly double[,] _kernelY;

        public SobelFilterRGB()
        {
            Diameter = 3;
            Radius = Diameter / 2;
            _kernelX = new double[,] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 } };
            _kernelY = new double[,]
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };
        }

        public SobelFilterRGB(double[,] kernel, int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            _kernelX = new double[Diameter, Diameter];
            _kernelY = new double[Diameter, Diameter];
            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    _kernelX[i, j] = kernel[i, j];
                    _kernelY[i, j] = kernel[j, i];
                }
            }
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            double xR = 0, xG = 0, xB = 0;
            double yR = 0, yG = 0, yB = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    Color neighborColor = wrapImage[idX, idY];

                    xR += neighborColor.R * _kernelX[j + Radius, i + Radius];
                    xG += neighborColor.G * _kernelX[j + Radius, i + Radius];
                    xB += neighborColor.B * _kernelX[j + Radius, i + Radius];

                    yR += neighborColor.R * _kernelY[j + Radius, i + Radius];
                    yG += neighborColor.G * _kernelY[j + Radius, i + Radius];
                    yB += neighborColor.B * _kernelY[j + Radius, i + Radius];
                }
            }
            int result  = Clamp((int)((Math.Sqrt(xR * xR + yR * yR) + Math.Sqrt(xG * xG + yG * yG) + Math.Sqrt(xB * xB + yB * yB))/3), 0, 255);
            return Color.FromArgb(result, result, result);
        }
    }

    class SobelFilterColored : Filter
    {
        private readonly double[,] _kernelX;
        private readonly double[,] _kernelY;

        public SobelFilterColored()
        {
            Diameter = 3;
            Radius = Diameter / 2;
            _kernelX = new double[,] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };
            _kernelY = new double[,]
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };
        }

        public SobelFilterColored(double[,] kernel, int diameter)
        {
            Diameter = diameter;
            Radius = Diameter / 2;
            _kernelX = new double[Diameter, Diameter];
            _kernelY = new double[Diameter, Diameter];
            for (int i = 0; i < Diameter; ++i)
            {
                for (int j = 0; j < Diameter; ++j)
                {
                    _kernelX[i, j] = kernel[i, j];
                    _kernelY[i, j] = kernel[j, i];
                }
            }
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            double xR = 0, xG = 0, xB = 0;
            double yR = 0, yG = 0, yB = 0;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    Color neighborColor = wrapImage[idX, idY];

                    xR += neighborColor.R * _kernelX[j + Radius, i + Radius];
                    xG += neighborColor.G * _kernelX[j + Radius, i + Radius];
                    xB += neighborColor.B * _kernelX[j + Radius, i + Radius];

                    yR += neighborColor.R * _kernelY[j + Radius, i + Radius];
                    yG += neighborColor.G * _kernelY[j + Radius, i + Radius];
                    yB += neighborColor.B * _kernelY[j + Radius, i + Radius];
                }
            }

            double gxx, gyy, gxy;

            gxx = Math.Pow(xR, 2) + Math.Pow(xG, 2) + Math.Pow(xB, 2);
            gyy = Math.Pow(yR, 2) + Math.Pow(yG, 2) + Math.Pow(yB, 2);
            gxy = xR * yR +  xG * yG + xB * yB;
            double angle = Math.Atan(2 * gxy / Math.Abs(gxx - gyy))/2;

            int result = Clamp((int)Math.Sqrt(0.5 * (gxx + gyy + (gxx - gyy)*Math.Cos(2*angle) + 2 * gxy * Math.Sin(2*angle))), 0, 255);
            
            return Color.FromArgb(result, result, result);
        }
    }

    class Laplass : Filter
    {
        private readonly bool _restoredBackground;
        private readonly double _multiplier;
        public Laplass(double multiplier, bool restoredBackground)
        {
            _restoredBackground = restoredBackground;
            _multiplier = multiplier;
            Kernel = new double[,]
                {
                    { 0, 1, 0 },
                    { 1, -4, 1 },
                    { 0, 1, 0 }
                };
            Diameter = 3;
            Radius = Diameter / 2;
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double r = 0;
            double g = 0;
            double b = 0;
            Color neighborColor;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    neighborColor = wrapImage[idX, idY];

                    r += Kernel[j + Radius, i + Radius] * neighborColor.R * _multiplier;
                    g += Kernel[j + Radius, i + Radius] * neighborColor.G * _multiplier;
                    b += Kernel[j + Radius, i + Radius] * neighborColor.B * _multiplier;
                }
            }

            if (_restoredBackground)
            {
                neighborColor = wrapImage[x, y];
                r = neighborColor.R + (int)(-r);
                g = neighborColor.G + (int)(-g);
                b = neighborColor.B + (int)(-b);
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class ExtendedLaplass : Filter
    {
        private readonly bool _restoredBackground;
        private readonly double _multiplier;
        public ExtendedLaplass(double multiplier, bool restoredBackground)
        {
            _restoredBackground = restoredBackground;
            _multiplier = multiplier;

            Kernel = new double[,]
                {
                    { 1, 1, 1 },
                    { 1, -8, 1 },
                    { 1, 1, 1 }
                };

                Diameter = 3;
            Radius = Diameter / 2;
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double r = 0;
            double g = 0;
            double b = 0;
            Color neighborColor;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    neighborColor = wrapImage[idX, idY];

                    r += Kernel[j + Radius, i + Radius] * neighborColor.R * _multiplier;
                    g += Kernel[j + Radius, i + Radius] * neighborColor.G * _multiplier;
                    b += Kernel[j + Radius, i + Radius] * neighborColor.B * _multiplier;
                }
            }

            if (_restoredBackground)
            {
                neighborColor = wrapImage[x, y];
                r = neighborColor.R + -r;
                g = neighborColor.G + -g;
                b = neighborColor.B + -b;
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class FrequencyIncrease : Filter
    {
        private readonly double _multiplier;
        private readonly double _sigma;

        public FrequencyIncrease(double multiplier, int diameter, double sigma)
        {
            _multiplier = multiplier;
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
            Color neighborColor;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);
                    neighborColor = wrapImage[idX, idY];

                    r += neighborColor.R * Kernel[j + Radius, i + Radius];
                    g += neighborColor.G * Kernel[j + Radius, i + Radius];
                    b += neighborColor.B * Kernel[j + Radius, i + Radius];
                }

            }

            neighborColor = wrapImage[x, y];
            r = Clamp(neighborColor.R + (int)(_multiplier * (neighborColor.R - r)), 0, 255);
            g = Clamp(neighborColor.G + (int)(_multiplier * (neighborColor.G - g)), 0, 255);
            b = Clamp(neighborColor.B + (int)(_multiplier * (neighborColor.B - b)), 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
