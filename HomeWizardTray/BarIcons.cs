using System.Drawing;
using System.Collections.Generic;
using HomeWizardTray.Extensions;

namespace HomeWizardTray
{
    internal class BarIcons
    {
        private readonly AppSettings _appSettings;

        public BarIcons(AppSettings appSettings)
        {
            _appSettings = appSettings;
            Icons = CreateIcons();
        }

        public readonly List<Icon> Icons;

        public Icon NoBars { get { return Icons[0]; } }
        public Icon OneBar { get { return Icons[1]; } }
        public Icon TwoBars { get { return Icons[2]; } }
        public Icon ThreeBars { get { return Icons[3]; } }
        public Icon FourBars { get { return Icons[4]; } }
        public Icon FiveBars { get { return Icons[5]; } }

        private List<Icon> CreateIcons()
        {
            var output = new List<Icon>(6);
            var activeColor = ColorTranslator.FromHtml(_appSettings.Color);

            using (var bitmap = new Bitmap(16, 16))
            using (var activeFillBrush = new SolidBrush(activeColor))
            using (var inactiveFillBrush = new SolidBrush(Color.FromArgb(255, 80, 80, 80)))
            {
                //Define rectangles
                var bars = new List<Rectangle>
                {
                    new Rectangle(1, 11, 2, 2),
                    new Rectangle(4, 9, 2, 4),
                    new Rectangle(7, 7, 2, 6),
                    new Rectangle(10, 5, 2, 8),
                    new Rectangle(13, 3, 2, 10)
                };

                //Draw rectangles
                foreach (var bar in bars)
                {
                    Graphics.FromImage(bitmap).FillAndOutlineRectangle(bar, inactiveFillBrush, Color.FromArgb(130, 0, 0, 0));
                }

                //Fill active rectangles
                output.Add(Icon.FromHandle(bitmap.GetHicon()));
                foreach (var bar in bars)
                {
                    Graphics.FromImage(bitmap).FillRectangle(activeFillBrush, bar);
                    output.Add(Icon.FromHandle(bitmap.GetHicon()));
                }
            }

            return output;
        }
    }
}
