using System.Drawing;

namespace ImageProcessing
{
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
}
