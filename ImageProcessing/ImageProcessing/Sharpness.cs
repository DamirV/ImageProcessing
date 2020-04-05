﻿using System;
using System.Drawing;

namespace ImageProcessing
{
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
            r = Clamp(neighborColor.R + (int)(_multiplier * (neighborColor.R - r)), 0, 255);
            g = Clamp(neighborColor.G + (int)(_multiplier * (neighborColor.G - g)), 0, 255);
            b = Clamp(neighborColor.B + (int)(_multiplier * (neighborColor.B - b)), 0, 255);


            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}