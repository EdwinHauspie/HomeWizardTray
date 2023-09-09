using System.Windows.Forms;
using System.Drawing;

namespace HomeWizardTray
{
    public class CustomToolStripRenderer : ToolStripProfessionalRenderer
    {
        public Color Text { get; set; }
        public Color Border { get; set; }
        public Color Background { get; set; }
        public Color HoverBackground { get; set; }
        public Color MarginBackground { get; set; }
        public int VerticalPadding { get; set; }

        public CustomToolStripRenderer(Color text, Color border, Color background, Color hoverBackground, Color marginBackground, int verticalPadding)
        {
            Text = text;
            Border = border;
            Background = background;
            HoverBackground = hoverBackground;
            MarginBackground = marginBackground;
            VerticalPadding = verticalPadding;
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Text;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            var size = new Size(e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            var rc = new Rectangle(Point.Empty, size);
            e.Graphics.DrawRectangle(new Pen(Border), rc);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected)
            {
                base.OnRenderMenuItemBackground(e);
            }
            else
            {
                var rc = new Rectangle(new Point(2, 0), new Size(e.Item.Size.Width - 3, e.Item.Size.Height));
                e.Graphics.FillRectangle(new SolidBrush(HoverBackground), rc);
            }
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = Text;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(HoverBackground), new Point(0, e.Item.Height / 2), new Point(e.Item.Width, e.Item.Height / 2));
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Background), e.ToolStrip.ClientRectangle);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(MarginBackground), e.AffectedBounds);
        }
    }
}