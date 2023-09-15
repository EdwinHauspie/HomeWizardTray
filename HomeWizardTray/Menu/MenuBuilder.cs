using System.Windows.Forms;

namespace HomeWizardTray.Menu
{
    internal static class MenuBuilder
    {
        public static ContextMenuStrip Build(Menu[] items)
        {
            var output = new ContextMenuStrip { };
            Fill(output.Items, items);
            return output;
        }

        private static void Fill(ToolStripItemCollection toolStripItems, Menu[] items)
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
