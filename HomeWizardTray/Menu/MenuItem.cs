using System;

namespace HomeWizardTray.Menu
{
    internal sealed class MenuItem
    {
        public string Caption { get; set; }
        public MenuItem[] SubMenu { get; set; } = Array.Empty<MenuItem>();
        public EventHandler OnClick { get; set; }

        public MenuItem(string caption, EventHandler onClick)
        {
            Caption = caption;
            OnClick = onClick;
        }

        public MenuItem(string caption, MenuItem[] subMenu)
        {
            Caption = caption;
            SubMenu = subMenu;
        }
    }
}
