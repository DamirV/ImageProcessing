using System;
using System.ComponentModel;
using System.Drawing;

namespace ImageProcessing
{
    class SaltPepper : Filter
    {
        private int saltPercent;
        private int pepperPercent;
        private Random randomPercent;
        public SaltPepper(int saltPercent, int pepperPercent)
        {
            this.saltPercent = saltPercent;
            this.pepperPercent = pepperPercent;
            this.randomPercent = new Random();
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int currentPercent = (int)(randomPercent.NextDouble() * 100);

            if (0 <= currentPercent && currentPercent < saltPercent)
            {
                return Color.FromArgb(255, 255, 255);
            }
            if (saltPercent <= currentPercent && currentPercent < saltPercent + pepperPercent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return wrapImage[x, y];
        }
    }

    class Salt : Filter
    {
        private int saltPercent;
        private Random randomValue;
        public Salt(int saltPercent)
        {
            this.saltPercent = saltPercent;
            this.randomValue = new Random();
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int currentPercent = (int)(randomValue.NextDouble() * 100);
            if ( 0 <= currentPercent && currentPercent < saltPercent)
            {
                return Color.FromArgb(255, 255, 255);
            }

            return wrapImage[x, y];
        }
    }

    class Pepper : Filter
    {
        private int pepperPercent;
        private Random randomValue;
        public Pepper(int pepperPercent)
        {
            this.pepperPercent = pepperPercent;
            this.randomValue = new Random();
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int currentPercent = (int)(randomValue.NextDouble() * 100);
            if (0 <= currentPercent && currentPercent < pepperPercent)
            {
                return Color.FromArgb(0, 0, 0);
            }

            return wrapImage[x, y];
        }
    }

    class BlackHoles : Filter
    {
        private int percent;

        public BlackHoles(int diameter, int percent)
        {
            this.diameter = diameter;
            this.radius = diameter / 2;
            this.percent = percent;

            switch (diameter)
            {
                case 3:
                    this.kernel = new double[,]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case 5:
                    this.kernel = new double[,]
                    {
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0}
                    };
                    break;

                case 7:
                    this.kernel = new double[,]
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

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            this.width = sourceImage.Width;
            this.height = sourceImage.Height;

            Bitmap resultImage = new Bitmap(sourceImage);
            Bitmap tempBitmap = new Bitmap(width, height);

            int checkProgress = -1;

            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                foreach (var point in wrapTempImage)
                {
                    wrapTempImage[point] = Color.White;
                }
            }

            Pepper pepper = new Pepper(percent);
            Erosion erosion = new Erosion(diameter, kernel);
            tempBitmap = erosion.ProcessImage(pepper.ProcessImage(tempBitmap, worker), worker);


            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                using (ImageWrapper wrapImage = new ImageWrapper(resultImage,true))
                {
                    for (int i = 0; i < height; ++i)
                    {
                        if (i > checkProgress)
                        {
                            worker.ReportProgress((int) ((double) i / resultImage.Height * 100));
                            if (worker.CancellationPending)
                            {
                                return null;
                            }

                            checkProgress += 100;
                        }

                        for (int j = 0; j < width; ++j)
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
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            return wrapImage[x, y];
        }
    }

    class WhiteHoles : Filter
    {
        private int percent;
       

        public WhiteHoles(int diameter, int percent)
        {
            this.diameter = diameter;
            this.percent = percent;

            switch (diameter)
            {
                case 3:
                    this.kernel = new double[,]
                    {
                        {0, 1, 0},
                        {1, 1, 1},
                        {0, 1, 0}
                    };
                    break;

                case 5:
                    this.kernel = new double[,]
                    {
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0}
                    };
                    break;

                case 7:
                    this.kernel = new double[,]
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

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            width = sourceImage.Width;
            height = sourceImage.Height;

            Bitmap resultImage = new Bitmap(sourceImage);
            Bitmap tempBitmap = new Bitmap(width, height);

            int checkProgress = -1;

            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                foreach (var point in wrapTempImage)
                {
                    wrapTempImage[point] = Color.Black;
                }
            }

            Salt salt = new Salt(percent);
            Dilation dilation = new Dilation(diameter, kernel);
            tempBitmap = dilation.ProcessImage(salt.ProcessImage(tempBitmap, worker), worker);

            using (ImageWrapper wrapTempImage = new ImageWrapper(tempBitmap))
            {
                using (ImageWrapper wrapImage = new ImageWrapper(resultImage, true))
                {
                    for (int i = 0; i < height; ++i)
                    {
                        if (i > checkProgress)
                        {
                            worker.ReportProgress((int)((double)i / resultImage.Height * 100));
                            if (worker.CancellationPending)
                            {
                                return null;
                            }

                            checkProgress += 100;
                        }

                        for (int j = 0; j < width; ++j)
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
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            return wrapImage[x, y];
        }
    }

    class GaussianNoise : Filter
    {
        private int middle;
        private double sigma;
        private Random randomValue;
        public GaussianNoise(double sigma, int middle)
        {
            this.randomValue = new Random();
            this.middle = middle;
            this.sigma = sigma;
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            double u1 = 1.0 - randomValue.NextDouble();
            double u2 = 1.0 - randomValue.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = middle + sigma * randStdNormal;

            int red = Clamp((int)(wrapImage[x, y].R + randNormal), 0, 255);
            int green = Clamp((int)(wrapImage[x, y].G + randNormal), 0, 255);
            int blue = Clamp((int)(wrapImage[x, y].B + randNormal), 0, 255);

            return Color.FromArgb(red, green, blue);
        }
    }

    class UniformNoise : Filter
    {
        private  Random randomValue;
        private int leftValue;
        private  int rightValue;
        private  double constant;
        public UniformNoise (int leftValue, int rightValue)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
            randomValue = new Random();
            constant = (rightValue-leftValue);
        }
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int randNumber = (int)(leftValue + randomValue.NextDouble() * constant);

            int red = Clamp(wrapImage[x, y].R + randNumber, 0, 255);
            int green = Clamp(wrapImage[x, y].G + randNumber, 0, 255);
            int blue = Clamp(wrapImage[x, y].B + randNumber, 0, 255);

            return Color.FromArgb(red, green, blue);
        }
    }

}
