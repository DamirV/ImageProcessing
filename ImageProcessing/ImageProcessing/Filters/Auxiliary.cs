using System.Drawing;

namespace ResearchWork
{
    class BlackAndWhite : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
             
            int red = wrapImage[x, y].R;
            int green = wrapImage[x, y].G;
            int blue = wrapImage[x, y].B;

            int result = (red + green + blue) / 3;

            return Color.FromArgb(result, result, result);
        }
    }
}
