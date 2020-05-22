using System.Drawing;

namespace ImageProcessing
{
    class ComponentR : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];

            return Color.FromArgb(color.R, color.R, color.R);
        }
    }

    class ComponentG : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];

            return Color.FromArgb(color.G, color.G, color.G);
        }
    }

    class ComponentB : Filter
    {
        protected override Color CalculateNewPixelColor(ImageWrapper wrapImage, int x, int y)
        {
            Color color = wrapImage[x, y];

            return Color.FromArgb(color.B, color.B, color.B);
        }
    }
}