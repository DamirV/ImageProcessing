using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        virtual public Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, CalculateNewPixelColor(sourceImage, i, j));
                }
            }

            return resultImage;
        }
    }

    class BlackAndWhite : Filters
    {
        public BlackAndWhite()
        {

        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int R = sourceImage.GetPixel(x, y).R;
            int G = sourceImage.GetPixel(x, y).G;
            int B = sourceImage.GetPixel(x, y).B;

            R = G = B = (R + G + B) / 3;

            return Color.FromArgb(R, G, B);
        }
    }

    class SaltPepper : Filters
    {
        int[,] matrix;
        public SaltPepper(int width, int height)
        {
            matrix = new int[width, height];
            Random rnd = new Random();


            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    matrix[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int R = sourceImage.GetPixel(x, y).R;
            int G = sourceImage.GetPixel(x, y).G;
            int B = sourceImage.GetPixel(x, y).B;

            if (matrix[x, y] < 10)
            {
                return Color.FromArgb(255, 255, 255);
            }
            if (matrix[x, y] > 95)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return Color.FromArgb(R, G, B);
        }
    }

    class Salt : Filters
    {
        int[,] matrix;
        private int percent;
        public Salt(int width, int height, int percent)
        {
            matrix = new int[width, height];
            Random rnd = new Random();
            this.percent = percent;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    matrix[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int R = sourceImage.GetPixel(x, y).R;
            int G = sourceImage.GetPixel(x, y).G;
            int B = sourceImage.GetPixel(x, y).B;

            if (matrix[x, y] < percent)
            {
                return Color.FromArgb(255, 255, 255);
            }

            return Color.FromArgb(R, G, B);
        }
    }

    class Pepper : Filters
    {
        int[,] matrix;
        private int percent;
        public Pepper(int width, int height, int percent)
        {
            matrix = new int[width, height];
            Random rnd = new Random();
            this.percent = percent;

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    matrix[i, j] = rnd.Next(0, 100);
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int R = sourceImage.GetPixel(x, y).R;
            int G = sourceImage.GetPixel(x, y).G;
            int B = sourceImage.GetPixel(x, y).B;

            if (matrix[x, y] < percent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return Color.FromArgb(R, G, B);
        }
    }





    class MatrixFilter : Filters
    {
        protected float[,] kernel = null;
        protected int size;
        protected MatrixFilter()
        {

        }

        public MatrixFilter(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = BorderProcessing(x + k, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                }

            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }

    }

    class GaussianFilter : MatrixFilter
    {
        float sigma;
        public GaussianFilter(int size, float sigma)
        {
            this.sigma = sigma;
            this.size = size;
            CreateGaussianKernel();
        }

        public void CreateGaussianKernel()
        {
            int radius = size / 2;
            float distance = 0;
            float constant = (float)(1 / (2 * Math.PI * sigma * sigma));

            kernel = new float[size, size];
            float norm = 0;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    distance = (i * i + j * j) / (sigma * sigma);
                    kernel[i + radius, j + radius] = constant * (float)(Math.Exp(-distance));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radius = size / 2;

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

                    resultR += neighborColor.R * kernel[k + radius, l + radius];
                    resultG += neighborColor.G * kernel[k + radius, l + radius];
                    resultB += neighborColor.B * kernel[k + radius, l + radius];
                }
            }

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }

    }

    class SobelFilter : MatrixFilter
    {
        public float[,] kernelX;
        public float[,] kernelY;

        public SobelFilter()
        {
            size = 3;
            kernelX = new float[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            kernelY = new float[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
        }

        public SobelFilter(float[,] array, int size)
        {
            this.size = size;
            kernelX = new float[size, size];
            kernelY = new float[size, size];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernelX[i, j] = array[i, j];
                    kernelY[i, j] = array[j, i];
                }
            }
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;

            float resultXR = 0, resultXG = 0, resultXB = 0,
                resultYR = 0, resultYG = 0, resultYB = 0;

            for (int i = -radiusX; i <= radiusX; i++)
                for (int j = -radiusY; j <= radiusY; j++)
                {
                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultXR += neighborColor.R * kernelX[i + radiusX, j + radiusY];
                    resultXG += neighborColor.G * kernelX[i + radiusX, j + radiusY];
                    resultXB += neighborColor.B * kernelX[i + radiusX, j + radiusY];

                    resultYR += neighborColor.R * kernelY[i + radiusX, j + radiusY];
                    resultYG += neighborColor.G * kernelY[i + radiusX, j + radiusY];
                    resultYB += neighborColor.B * kernelY[i + radiusX, j + radiusY];
                }

            int R = (int)(Math.Sqrt(resultXR * resultXR + resultYR * resultYR));
            int G = (int)(Math.Sqrt(resultXG * resultXG + resultYG * resultYG));
            int B = (int)(Math.Sqrt(resultXB * resultXB + resultYB * resultYB));

            return Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(B, 0, 255));
        }
    }

    class LinearSmoothing : MatrixFilter
    {
        public LinearSmoothing(int size, bool extendedMask)
        {
            this.size = size;
            if (extendedMask)
            {
                switch (size)
                {
                    case 3:
                        kernel = new float[3, 3] {
                            { 1, 1, 1 },
                            { 1, 1, 1 },
                            { 1, 1, 1 }
                        };
                        break;
                    case 5:
                        kernel = new float[5, 5] {
                            { 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1 },
                            {1, 1, 1, 1, 1 },
                            {1, 1, 1, 1, 1 },
                            {1, 1, 1, 1, 1 }
                        };
                        break;
                    case 7:
                        kernel = new float[7, 7] {
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                            {1, 1, 1, 1, 1, 1, 1},
                        };
                        break;
                }
            }
            else
            {
                switch (size)
                {
                    case 3:
                        kernel = new float[3, 3] {
                            { 1, 2, 1 },
                            { 2, 4, 2 },
                            { 1, 2, 1 }
                        };
                        break;
                    case 5:
                        kernel = new float[5, 5] {
                            { 1, 2, 4, 2, 1 },
                            {2, 4, 8, 4, 2 },
                            {4, 8, 16, 8, 4 },
                            {2, 4, 8, 4, 2 },
                            { 1, 2, 4, 2, 1 }
                        };
                        break;
                    case 7:
                        kernel = new float[7, 7] {
                            {1, 2, 4, 8, 4, 2, 1},
                            {2, 4, 8, 16, 8, 4, 2},
                            {4, 8, 16, 32, 16, 8, 4},
                            {8, 16, 32, 64, 32, 16, 8},
                            {4, 8, 16, 32, 16, 8, 4},
                            {2, 4, 8, 16, 8, 4, 2},
                            {1, 2, 4, 8, 4, 2, 1},
                        };
                        break;
                }
            }

        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            int half = size / 2;
            float length = 0;

            for (int i = -half; i <= half; i++)
                for (int j = -half; j <= half; j++)
                {

                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[i + half, j + half];
                    resultG += neighborColor.G * kernel[i + half, j + half];
                    resultB += neighborColor.B * kernel[i + half, j + half];
                    length += kernel[i + half, j + half];
                }

            resultR /= length;
            resultG /= length;
            resultB /= length;

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }

    }

    class Mediana : MatrixFilter
    {
        public Mediana(int size)
        {
            this.size = size;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;

            int[] R = new int[size * size];
            int[] G = new int[size * size];
            int[] B = new int[size * size];

            int index = 0;

            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = BorderProcessing(x + k, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + l, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);

                    R[index] = neighborColor.R;
                    G[index] = neighborColor.G;
                    B[index] = neighborColor.B;

                    index++;
                }

            }

            Array.Sort(R);
            Array.Sort(G);
            Array.Sort(B);

            int num = (size * size + 1) / 2;

            Color resultColor = Color.FromArgb(R[num], G[num], B[num]);

            return resultColor;
        }
    }

    class Laplass : MatrixFilter
    {

        public Laplass(float[,] array, int size = 3)
        {
            this.size = size;

            kernel = new float[size, size];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernel[i, j] = array[i, j];
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;
            int half = size / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            Color neighborColor;

            for (int i = -radiusX; i <= radiusX; i++)
            {
                for (int j = -radiusY; j <= radiusY; j++)
                {
                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);

                    neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += kernel[i + half, j + half] * neighborColor.R;
                    resultG += kernel[i + half, j + half] * neighborColor.G;
                    resultB += kernel[i + half, j + half] * neighborColor.B;

                }

            }

            // neighborColor = sourceImage.GetPixel(x, y);

            int R = (int)resultR;
            int G = (int)resultG;
            int B = (int)resultB;

            return Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(B, 0, 255));
        }

    }

    class RestoredLaplass : MatrixFilter
    {
        float k;
        public RestoredLaplass(float[,] array, float k = 1)
        {
            this.size = 3;
            this.k = k;
            kernel = new float[size, size];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernel[i, j] = array[i, j];
                }
            }
        }
        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;
            int half = size / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            Color neighborColor;

            for (int i = -radiusX; i <= radiusX; i++)
            {
                for (int j = -radiusY; j <= radiusY; j++)
                {
                    int idX = BorderProcessing(x + i, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + j, 0, sourceImage.Height - 1);

                    neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += kernel[i + half, j + half] * neighborColor.R;
                    resultG += kernel[i + half, j + half] * neighborColor.G;
                    resultB += kernel[i + half, j + half] * neighborColor.B;

                }

            }

            neighborColor = sourceImage.GetPixel(x, y);

            int R = neighborColor.R + (int)(-k * resultR);
            int G = neighborColor.G + (int)(-k * resultG);
            int B = neighborColor.B + (int)(-k * resultB);

            return Color.FromArgb(Clamp(R, 0, 255), Clamp(G, 0, 255), Clamp(B, 0, 255));
        }

    }

    class FrequencyIncrease : MatrixFilter
    {
        private float k;
        private float sigma;

        public FrequencyIncrease(float k, int size, float sigma)
        {
            this.k = k;
            this.size = size;
            this.sigma = sigma;
            CreateGaussianKernel();
        }
        public void CreateGaussianKernel()
        {
            int radius = size / 2;
            float constant = (float)(1 / (2 * Math.PI * sigma * sigma));
            float distance = 0;
            kernel = new float[size, size];
            float norm = 0;

            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    distance = (i * i + j * j) / (sigma * sigma);
                    kernel[i + radius, j + radius] = constant * (float)(Math.Exp(-distance));
                    norm += kernel[i + radius, j + radius];
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            Color neighborColor;
            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);
                    neighborColor = sourceImage.GetPixel(idX, idY);

                    resultR += neighborColor.R * kernel[j + radiusX, i + radiusY];
                    resultG += neighborColor.G * kernel[j + radiusX, i + radiusY];
                    resultB += neighborColor.B * kernel[j + radiusX, i + radiusY];
                }

            }
            neighborColor = sourceImage.GetPixel(x, y);

            resultR = neighborColor.R + (int)(k * (neighborColor.R - resultR));
            resultG = neighborColor.G + (int)(k * (neighborColor.G - resultG));
            resultB = neighborColor.B + (int)(k * (neighborColor.B - resultB));

            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }


    }

    class Dilation : MatrixFilter
    {
        public Dilation(int size)
        {
            this.size = size;
            kernel = new float[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernel[i, j] = 1;
                }
            }
        }

        public Dilation(int size, float[,] kernel)
        {
            this.size = size;
            this.kernel = kernel;
        }

        public Dilation()
        {

        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;
            int max = 0;

            Color resultColor = Color.Black;

            for (int i = -radiusY; i <= radiusY; i++)
            {
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    int Intensity = neighborColor.R;

                    if ((kernel[j + radiusX, i + radiusY] > 0) && (Intensity > max))
                    {
                        max = Intensity;
                        resultColor = neighborColor;
                    }
                }
            }

            return resultColor;
        }
    }

    class Erosion : MatrixFilter
    {

        public Erosion()
        {

        }

        public Erosion(int size)
        {
            this.size = size;
            kernel = new float[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernel[i, j] = 1;
                }
            }
        }

        public Erosion(int size, float[,] kernel)
        {
            this.size = size;
            this.kernel = kernel;
        }

        protected override Color CalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = size / 2;
            int radiusY = size / 2;
            int min = 255;

            Color resultColor = Color.White;


            for (int i = -radiusY; i <= radiusY; i++)
                for (int j = -radiusX; j <= radiusX; j++)
                {
                    int idX = BorderProcessing(x + j, 0, sourceImage.Width - 1);
                    int idY = BorderProcessing(y + i, 0, sourceImage.Height - 1);

                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    int Intensity = neighborColor.R;

                    if ((kernel[j + radiusX, i + radiusY] > 0) && (Intensity < min))
                    {
                        min = Intensity;
                        resultColor = neighborColor;
                    }
                }
            return resultColor;
        }
    }

    class Opening : MatrixFilter
    {

        public Opening(int size)
        {
            this.size = size;
            kernel = new float[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernel[i, j] = 1;
                }
            }
        }

        public Opening()
        {

        }


        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {

            Erosion er = new Erosion(size);
            Dilation di = new Dilation(size);

            return di.processImage(er.processImage(sourceImage, worker), worker);

        }
    }

    class Closing : MatrixFilter
    {

        public Closing()
        {

        }

        public Closing(int size)
        {
            this.size = size;
            kernel = new float[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    kernel[i, j] = 1;
                }
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation di = new Dilation(size);
            Erosion er = new Erosion(size);
            return er.processImage(di.processImage(sourceImage, worker), worker);
        }
    }

    class BlackHoles : MatrixFilter
    {
        private int width;
        private int height;
        private int percent;
        public BlackHoles()
        {

        }

        public BlackHoles(int size, int width, int height, int percent)
        {
            this.size = size;
            this.width = width;
            this.height = height;
            this.percent = percent;

            kernel = new float[size, size];

            switch (size)
            {
                case 3:
                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;
                case 5:
                    kernel = new float[5, 5]
                    {
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0}
                    };
                    break;
                case 7:
                    kernel = new float[7, 7]
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
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap tmp = new Bitmap(width, height);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    tmp.SetPixel(i, j, Color.White);
                }
            }

            Pepper pe = new Pepper(width, height, percent);
            Erosion er = new Erosion(size, kernel);
            tmp = er.processImage(pe.processImage(tmp, worker), worker);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    if (tmp.GetPixel(i, j).R != 0 && tmp.GetPixel(i, j).G != 0 && tmp.GetPixel(i, j).B != 0)
                    {
                        tmp.SetPixel(i, j, sourceImage.GetPixel(i, j));
                    }
                }
            }

            return tmp;
        }
    }

    class WhiteHoles : MatrixFilter
    {
        private int width;
        private int height;
        private int percent;
        public WhiteHoles()
        {

        }

        public WhiteHoles(int size, int width, int height, int percent)
        {
            this.size = size;
            this.width = width;
            this.height = height;
            this.percent = percent;

            kernel = new float[size, size];

            switch (size)
            {
                case 3:
                    kernel = new float[3, 3]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;
                case 5:
                    kernel = new float[5, 5]
                    {
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0}
                    };
                    break;
                case 7:
                    kernel = new float[7, 7]
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
            }
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap tmp = new Bitmap(width, height);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    tmp.SetPixel(i, j, Color.Black);
                }
            }

            Salt sa = new Salt(width, height, percent);
            Dilation di = new Dilation(size, kernel);
            tmp = di.processImage(sa.processImage(tmp, worker), worker);

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    if (tmp.GetPixel(i, j).R != 255 && tmp.GetPixel(i, j).G != 255 && tmp.GetPixel(i, j).B != 255)
                    {
                        tmp.SetPixel(i, j, sourceImage.GetPixel(i, j));
                    }
                }
            }

            return tmp;
        }
    }

}
