using System;

namespace HomeWizardTray.Menu
{
    internal sealed class Menu
    {
        public string Caption { get; set; }
        public Menu[] SubMenu { get; set; } = Array.Empty<Menu>();
        public EventHandler OnClick { get; set; }

        public Menu(string caption, EventHandler onClick)
        {
            Caption = caption;
            OnClick = onClick;
        }

        public Menu(string caption, Menu[] subMenu)
        {
            Caption = caption;
            SubMenu = subMenu;
        }
    }
}
