using System.Drawing;

namespace HomeWizardTray.Extensions
{
    internal static class GraphicsExtensions
    {
        public static void FillAndOutlineRectangle(this Graphics g, Rectangle rectangle, Brush fillBrush, Color outlineColor)
        {
            rectangle.Inflate(1, 1);

            using (var outlineBrush = new SolidBrush(outlineColor))
            {
                g.FillRectangle(outlineBrush, rectangle);
                rectangle.Inflate(-1, -1);
                g.FillRectangle(fillBrush, rectangle);
            }
        }
    }
}
