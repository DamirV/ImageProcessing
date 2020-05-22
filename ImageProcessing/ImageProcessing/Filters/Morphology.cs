using System;
using System.ComponentModel;
using System.Drawing;

namespace ImageProcessing
{
    class Dilation : Filter
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

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int max = 0;

            Color resultColor = Color.Black;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];
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

    class Erosion : Filter
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

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int min = 255;

            Color resultColor = Color.White;

            for (int i = -Radius; i <= Radius; ++i)
            {
                for (int j = -Radius; j <= Radius; ++j)
                {
                    int idX = BorderProcessing(x + j, 0, Width - 1);
                    int idY = BorderProcessing(y + i, 0, Height - 1);

                    Color neighborColor = wrapImage[idX, idY];
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

    class Opening : Filter
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

        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;

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
                }
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }

    class Closing : Filter
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
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            float r = 0;
            float g = 0;
            float b = 0;

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
                }
            }

            r = Clamp((int)r, 0, 255);
            g = Clamp((int)g, 0, 255);
            b = Clamp((int)b, 0, 255);

            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
