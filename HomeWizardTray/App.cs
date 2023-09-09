using System;
using Serilog;
using System.Windows.Forms;
using HomeWizardTray.Assets;
using HomeWizardTray.DataProviders.HomeWizard;
using HomeWizardTray.DataProviders.Sma;
using Timer = System.Timers.Timer;
using System.Drawing;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HomeWizardTray.DataProviders.Daikin;

namespace HomeWizardTray
{
    internal sealed class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly P1MeterDataProvider _homeWizardDataProvider;
        private readonly SunnyBoyDataProvider _sunnyBoyDataProvider;
        private readonly Ftxm25DataProvider _ftxm25DataProvider;

        private ContextMenuStrip Menu { get; set; }
        private NotifyIcon TrayIcon { get; set; }
        private Timer UpdateTimer { get; set; }

        public App(AppSettings appSettings, P1MeterDataProvider homeWizardDataProvider, SunnyBoyDataProvider sunnyBoyDataProvider, Ftxm25DataProvider ftxm25DataProvider)
        {
            _appSettings = appSettings;
            _homeWizardDataProvider = homeWizardDataProvider;
            _sunnyBoyDataProvider = sunnyBoyDataProvider;
            _ftxm25DataProvider = ftxm25DataProvider;

            Menu = new ContextMenuStrip
            {
                ShowImageMargin = false,
                Font = new Font("Segoe ui", 11f),
                // TODO Opkuis van class CustomToolStripRenderer
                Renderer = new CustomToolStripRenderer(Color.White, Color.FromArgb(26, 28, 35), Color.FromArgb(26, 28, 35), Color.FromArgb(39, 41, 48), Color.BlueViolet, 4)
            };

            Menu.Items.Add(new ToolStripMenuItem("Airco"));
            ((Menu.Items[0] as ToolStripMenuItem).DropDown as ToolStripDropDownMenu).ShowImageMargin = false;
            (Menu.Items[0] as ToolStripMenuItem).DropDownItems.Add(new ToolStripMenuItem("Status", null, async (s, e) => { MessageBox.Show(JsonConvert.SerializeObject(await _ftxm25DataProvider.GetInfo(), Formatting.Indented), "SunnyTray", MessageBoxButtons.OK, MessageBoxIcon.Information); }));
            (Menu.Items[0] as ToolStripMenuItem).DropDownItems.Add(new ToolStripMenuItem("Max", null, async (s, e) => { await _ftxm25DataProvider.SetMax(); }));
            (Menu.Items[0] as ToolStripMenuItem).DropDownItems.Add(new ToolStripMenuItem("Normal", null, async (s, e) => { await _ftxm25DataProvider.SetLevel2(); }));
            (Menu.Items[0] as ToolStripMenuItem).DropDownItems.Add(new ToolStripMenuItem("Eco", null, async (s, e) => { await _ftxm25DataProvider.SetEco(); }));

            Menu.Items.Add(new ToolStripMenuItem("Quit", null, async (s, e) => await Exit()));

            TrayIcon = new NotifyIcon { Text = "SunnyTray", Icon = Resources.SunIcon, Visible = true, ContextMenuStrip = Menu };
            TrayIcon.Click += async (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Middle) await Exit(); };

            UpdateTimer = new Timer();
            UpdateTimer.Elapsed += (sender, args) => UpdateIconText();
            UpdateTimer.Start();
        }

        private async void UpdateIconText()
        {
            try
            {
                UpdateTimer.Stop();

                var sunnyPower = await _sunnyBoyDataProvider.GetActivePower();
                var homeWizardPower = await _homeWizardDataProvider.GetActivePower();
                TrayIcon.Text = $"🔆   {sunnyPower} W\r\n💡 {(homeWizardPower < 0 ? "" : "  ")}{homeWizardPower.ToString().Replace("-", "–")} W";

                UpdateTimer.Interval = TimeSpan.FromSeconds(_appSettings.UpdateIntervalSeconds).TotalMilliseconds;
                UpdateTimer.Start();
            }
            catch (Exception ex)
            {
                TrayIcon.Text = "An error has occured. Please see the log file.";
                Log.Error(ex, ex.Message);
            }
        }

        public async Task Exit(int exitCode = 0)
        {
            UpdateTimer.Stop();
            await _sunnyBoyDataProvider.Logout(); //TODO make disposable instead?
            if (TrayIcon != null) TrayIcon.Visible = false;
            Environment.Exit(exitCode);
        }
    }
}