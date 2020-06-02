using System;
using System.Drawing;

namespace ResearchWork
{
    class Sobel : Filter
    {
        private  double[,] kernelX;
        private  double[,] kernelY;

        public Sobel()
        {
            this.diameter = 3;
            this.radius = diameter / 2;
            this.kernelX = new double[,] { 
                { -1, 0, 1 },
                { -2, 0, 2 }, 
                { -1, 0, 1 } };

            this.kernelY = new double[,]
            {
                { -1, -2, -1 }, 
                { 0, 0, 0 }, 
                { 1, 2, 1 }
            };
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            double redX = 0, greenX = 0, blueX = 0;
            double redY = 0, greenY = 0, blueY = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    redX += wrapImage[idX, idY].R * kernelX[j + radius, i + radius];
                    greenX += wrapImage[idX, idY].G * kernelX[j + radius, i + radius];
                    blueX += wrapImage[idX, idY].B * kernelX[j + radius, i + radius];

                    redY += wrapImage[idX, idY].R * kernelY[j + radius, i + radius];
                    greenY += wrapImage[idX, idY].G * kernelY[j + radius, i + radius];
                    blueY += wrapImage[idX, idY].B * kernelY[j + radius, i + radius];
                }
            }

            int red = Clamp((int)Math.Sqrt(redX * redX + redY * redY), 0, 255);
            int green = Clamp((int)Math.Sqrt(greenX * greenX + greenY * greenY), 0, 255);
            int blue = Clamp((int)Math.Sqrt(blueX * blueX + blueY * blueY), 0, 255);

            return Color.FromArgb(red, green, blue);
        }
    }

    class SobelMean : Filter
    {
        private double[,] kernelX;
        private double[,] kernelY;

        public SobelMean()
        {
            this.diameter = 3;
            this.radius = diameter / 2;
            this.kernelX = new double[,] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 } };
            this.kernelY = new double[,]
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            double redX = 0, greenX = 0, blueX = 0;
            double redY = 0, greenY = 0, blueY = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);
                   
                    redX += wrapImage[idX, idY].R * kernelX[j + radius, i + radius];
                    greenX += wrapImage[idX, idY].G * kernelX[j + radius, i + radius];
                    blueX += wrapImage[idX, idY].B * kernelX[j + radius, i + radius];

                    redY += wrapImage[idX, idY].R * kernelY[j + radius, i + radius];
                    greenY += wrapImage[idX, idY].G * kernelY[j + radius, i + radius];
                    blueY += wrapImage[idX, idY].B * kernelY[j + radius, i + radius];
                }
            }
            int result  = Clamp((int)(
                (Math.Sqrt(redX * redX + redY * redY) + 
                Math.Sqrt(greenX * greenX + greenY * greenY) +
                Math.Sqrt(blueX * blueX + blueY * blueY))/3), 0, 255);
            return Color.FromArgb(result, result, result);
        }
    }

    class DiZenzo : Filter
    {
        private  double[,] kernelX;
        private double[,] kernelY;

        public DiZenzo()
        {
            this.diameter = 3;
            this.radius = diameter / 2;
            this.kernelX = new double[,] {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };
            this.kernelY = new double[,]
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            double redX = 0, greenX = 0, blueX = 0;
            double redY = 0, greenY = 0, blueY = 0;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    redX += wrapImage[idX, idY].R * kernelX[j + radius, i + radius];
                    greenX += wrapImage[idX, idY].G * kernelX[j + radius, i + radius];
                    blueX += wrapImage[idX, idY].B * kernelX[j + radius, i + radius];

                    redY += wrapImage[idX, idY].R * kernelY[j + radius, i + radius];
                    greenY += wrapImage[idX, idY].G * kernelY[j + radius, i + radius];
                    blueY += wrapImage[idX, idY].B * kernelY[j + radius, i + radius];
                }
            }

            double gxx, gyy, gxy;

            gxx = Math.Pow(redX, 2) + Math.Pow(greenX, 2) + Math.Pow(blueX, 2);
            gyy = Math.Pow(redY, 2) + Math.Pow(greenY, 2) + Math.Pow(blueY, 2);
            gxy = redX * redY +  greenX * greenY + blueX * blueY;
            double angle = Math.Atan(2 * gxy / Math.Abs(gxx - gyy))/2;

            int result = Clamp((int)Math.Sqrt(0.5 * (gxx + gyy + (gxx - gyy)*Math.Cos(2*angle) + 2 * gxy * Math.Sin(2*angle))), 0, 255);
            
            return Color.FromArgb(result, result, result);
        }
    }

    class Laplace : Filter
    {
        private  double multiplier;
        public Laplace(double multiplier)
        {
            this.multiplier = multiplier;
            this.kernel = new double[,]
                {
                    { 0, 1, 0 },
                    { 1, -4, 1 },
                    { 0, 1, 0 }
                };
            this.diameter = 3;
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

                    red += kernel[j + radius, i + radius] * wrapImage[idX, idY].R * multiplier;
                    green += kernel[j + radius, i + radius] * wrapImage[idX, idY].G * multiplier;
                    blue += kernel[j + radius, i + radius] * wrapImage[idX, idY].B * multiplier;
                }
            }

            red = Clamp((int)red, 0, 255);
            green = Clamp((int)green, 0, 255);
            blue = Clamp((int)blue, 0, 255);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class RestoredLaplace : Filter
    {
        private double multiplier;
        public RestoredLaplace(double multiplier)
        {
            this.multiplier = multiplier;
            this.kernel = new double[,]
            {
                { 0, 1, 0 },
                { 1, -4, 1 },
                { 0, 1, 0 }
            };
            this.diameter = 3;
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

                    red += kernel[j + radius, i + radius] * wrapImage[idX, idY].R * multiplier;
                    green += kernel[j + radius, i + radius] * wrapImage[idX, idY].G * multiplier;
                    blue += kernel[j + radius, i + radius] * wrapImage[idX, idY].B * multiplier;
                }
            }
            red = wrapImage[x, y].R + (int)(-red);
            green = wrapImage[x, y].G + (int)(-green);
            blue = wrapImage[x, y].B + (int)(-blue);


            red = Clamp((int)red, 0, 255);
            green = Clamp((int)green, 0, 255);
            blue = Clamp((int)blue, 0, 255);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class ExtendedLaplace : Filter
    {
        private double multiplier;
        public ExtendedLaplace(double multiplier)
        {
            this.multiplier = multiplier;

            this.kernel = new double[,]
                {
                    { 1, 1, 1 },
                    { 1, -8, 1 },
                    { 1, 1, 1 }
                };

            this.diameter = 3;
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

                    red += kernel[j + radius, i + radius] * wrapImage[idX, idY].R * multiplier;
                    green += kernel[j + radius, i + radius] * wrapImage[idX, idY].G * multiplier;
                    blue += kernel[j + radius, i + radius] * wrapImage[idX, idY].B * multiplier;
                }
            }

            red = Clamp((int)red, 0, 255);
            green = Clamp((int)green, 0, 255);
            blue = Clamp((int)blue, 0, 255);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class RestoredExtendedLaplace : Filter
    {
        private double multiplier;
        public RestoredExtendedLaplace(double multiplier)
        {
            this.multiplier = multiplier;

            this.kernel = new double[,]
            {
                { 1, 1, 1 },
                { 1, -8, 1 },
                { 1, 1, 1 }
            };

            this.diameter = 3;
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

                    red += kernel[j + radius, i + radius] * wrapImage[idX, idY].R * multiplier;
                    green += kernel[j + radius, i + radius] * wrapImage[idX, idY].G * multiplier;
                    blue += kernel[j + radius, i + radius] * wrapImage[idX, idY].B * multiplier;
                }
            }

            red = wrapImage[x, y].R + -red;
            green = wrapImage[x, y].G + -green;
            blue = wrapImage[x, y].B + -blue;
            

            red = Clamp((int)red, 0, 255);
            green = Clamp((int)green, 0, 255);
            blue = Clamp((int)blue, 0, 255);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }

    class FrequencyIncrease : Filter
    {
        private double multiplier;
        private double sigma;

        public FrequencyIncrease(double multiplier, int diameter, double sigma)
        {
            this.multiplier = multiplier;
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

            red = Clamp(wrapImage[x, y].R + (int)(multiplier * (wrapImage[x, y].R - red)), 0, 255);
            green = Clamp(wrapImage[x, y].G + (int)(multiplier * (wrapImage[x, y].G - green)), 0, 255);
            blue = Clamp(wrapImage[x, y].B + (int)(multiplier * (wrapImage[x, y].B - blue)), 0, 255);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }
    }
}
