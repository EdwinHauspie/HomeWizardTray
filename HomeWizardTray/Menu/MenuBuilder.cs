using System.Drawing;
using System.Windows.Forms;

namespace HomeWizardTray.Menu
{
    internal static class MenuBuilder
    {
        public static ContextMenuStrip Build(MenuItem[] items)
        {
            var output = new ContextMenuStrip
            {
                //ShowImageMargin = false,
                Font = new Font("Segoe ui", 10f),
                //Renderer = new CustomToolStripRenderer(Color.White, Color.FromArgb(26, 28, 35), Color.FromArgb(26, 28, 35), Color.FromArgb(39, 41, 48), Color.FromArgb(39, 41, 48))
            };

            Fill(output.Items, items);

            return output;
        }

        private static void Fill(ToolStripItemCollection toolStripItems, MenuItem[] items)
        {
            foreach (var item in items)
            {
                var toolStripItem = new ToolStripMenuItem(item.Caption, null, item.OnClick);
                toolStripItems.Add(toolStripItem);
                Fill(toolStripItem.DropDownItems, item.SubMenu);
            }
        }
    }
}
