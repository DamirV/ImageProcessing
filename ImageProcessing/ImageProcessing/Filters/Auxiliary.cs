using System.Drawing;

namespace ImageProcessing
{
    class BlackAndWhite : Filters
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];
            int r = color.R;
            int g = color.G;
            int b = color.B;

            int result = (r + g + b) / 3;

            return Color.FromArgb(result, result, result);
        }
    }
}
