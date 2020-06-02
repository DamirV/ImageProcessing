using System.Drawing;

namespace ResearchWork
{
    class RedComponent : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int red = wrapImage[x, y].R;

            return Color.FromArgb(red, red, red);
        }
    }

    class GreenComponent : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int green = wrapImage[x, y].G;

            return Color.FromArgb(green, green, green);
        }
    }

    class BlueComponent : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            int blue = wrapImage[x, y].B;

            return Color.FromArgb(blue, blue, blue);
        }
    }
}