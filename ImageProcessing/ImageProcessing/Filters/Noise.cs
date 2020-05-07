using System;
using System.ComponentModel;
using System.Drawing;

namespace ImageProcessing
{
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
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            if (_matrixOfRandomNumbers[x, y] < _saltPercent)
            {
                return Color.FromArgb(255, 255, 255);
            }
            if (_matrixOfRandomNumbers[x, y] >= 100 - _pepperPercent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return wrapImage[x, y];
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
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            if (_matrixOfRandomNumbers[x, y] < _percent)
            {
                return Color.FromArgb(255, 255, 255);
            }

            return wrapImage[x, y];
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
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {

            if (_matrixOfRandomNumbers[x, y] < _percent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return wrapImage[x, y];
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
            Bitmap resultImage = new Bitmap(sourceImage);
            Bitmap tempBitmap = new Bitmap(_width, _height);

            Width = resultImage.Width;
            Height = resultImage.Height;
            int checkProgress = -1;

            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                foreach (var point in wrapTempImage)
                {
                    wrapTempImage[point] = Color.White;
                }
            }

            Pepper pe = new Pepper(_width, _height, _percent);
            Erosion er = new Erosion(Diameter, Kernel);
            tempBitmap = er.ProcessImage(pe.ProcessImage(tempBitmap, worker), worker);


            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                using (ImageWrapper wrapImage = new ImageWrapper(resultImage,true))
                {
                    for (int i = 0; i < Height; ++i)
                    {
                        if (i > checkProgress)
                        {
                            worker.ReportProgress((int) ((float) i / resultImage.Height * 100));
                            if (worker.CancellationPending)
                            {
                                return null;
                            }

                            checkProgress += 100;
                        }

                        for (int j = 0; j < Width; ++j)
                        {
                            Color neighborColor = wrapTempImage[j, i];
                            if (neighborColor.R == 0 && neighborColor.G == 0 && neighborColor.B == 0)
                            {
                                wrapImage[j, i] = wrapTempImage[j, i];
                            }
                            
                        }
                    }
                }
            }

            return resultImage;
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
            Bitmap resultImage = new Bitmap(sourceImage);
            Bitmap tempBitmap = new Bitmap(_width, _height);

            Width = resultImage.Width;
            Height = resultImage.Height;
            int checkProgress = -1;

            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                foreach (var point in wrapTempImage)
                {
                    wrapTempImage[point] = Color.Black;
                }
            }

            Salt sa = new Salt(_width, _height, _percent);
            Dilation di = new Dilation(Diameter, Kernel);
            tempBitmap = di.ProcessImage(sa.ProcessImage(tempBitmap, worker), worker);

            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                using (ImageWrapper wrapImage = new ImageWrapper(resultImage, true))
                {
                    for (int i = 0; i < Height; ++i)
                    {
                        if (i > checkProgress)
                        {
                            worker.ReportProgress((int)((float)i / resultImage.Height * 100));
                            if (worker.CancellationPending)
                            {
                                return null;
                            }

                            checkProgress += 100;
                        }

                        for (int j = 0; j < Width; ++j)
                        {
                            Color neighborColor = wrapTempImage[j, i];
                            if (neighborColor.R == 255 && neighborColor.G == 255 && neighborColor.B == 255)
                            {
                                wrapImage[j, i] = wrapTempImage[j, i];
                            }
                        }
                    }
                }
            }

            return resultImage;
        }
    }
}
