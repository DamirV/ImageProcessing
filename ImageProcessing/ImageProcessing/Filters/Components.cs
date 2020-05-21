using System.Drawing;

namespace ImageProcessing
{
    class ComponentR : Filters
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];

            return Color.FromArgb(color.R, color.R, color.R);
        }
    }

    class ComponentG : Filters
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];

            return Color.FromArgb(color.G, color.G, color.G);
        }
    }

    class ComponentB : Filters
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];

            return Color.FromArgb(color.B, color.B, color.B);
        }
    }
}