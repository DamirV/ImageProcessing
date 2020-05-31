using System;
using System.ComponentModel;
using System.Drawing;

namespace ImageProcessing
{
    class Dilation : Filter
    {
        public Dilation(int diameter)
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

        public Dilation(int diameter, double[,] kernel)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
            this.kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int max = 0;

            Color resultColor = Color.Black;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    Color neighborColor = wrapImage[idX, idY];
                    int intensity = neighborColor.R;

                    if ((kernel[j + radius, i + radius] > 0) && (intensity > max))
                    {
                        max = intensity;
                        resultColor = neighborColor;
                    }
                }
            }

            return resultColor;
        }
    }

    class Erosion : Filter
    {
        public Erosion(int diameter)
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

        public Erosion(int diameter, double[,] kernel)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
            this.kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int min = 255;

            Color resultColor = Color.White;

            for (int i = -radius; i <= radius; ++i)
            {
                for (int j = -radius; j <= radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, width - 1);
                    int idY = BorderProcessing(y + i, 0, height - 1);

                    Color neighborColor = wrapImage[idX, idY];
                    int intensity = neighborColor.R;

                    if ((kernel[j + radius, i + radius] > 0) && (intensity < min))
                    {
                        min = intensity;
                        resultColor = neighborColor;
                    }
                }
            }

            return resultColor;
        }
    }

    class Opening : Filter
    {
        public Opening(int diameter)
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

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {

            Erosion er = new Erosion(diameter);
            Dilation di = new Dilation(diameter);

            return di.ProcessImage(er.ProcessImage(sourceImage, worker), worker);
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

    class Closing : Filter
    {
        public Closing(int diameter)
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

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation di = new Dilation(diameter);
            Erosion er = new Erosion(diameter);
            return er.ProcessImage(di.ProcessImage(sourceImage, worker), worker);
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
}
