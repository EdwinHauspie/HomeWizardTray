using System;
using Serilog;
using System.Windows.Forms;
using HomeWizardTray.Assets;
using HomeWizardTray.DataProviders.HomeWizard;
using HomeWizardTray.DataProviders.Sma;
using System.Threading.Tasks;
using HomeWizardTray.DataProviders.Daikin;
using System.Diagnostics;
using Timer = System.Timers.Timer;
using HomeWizardTray.DataProviders.Daikin.Constants;
using System.Linq;

namespace HomeWizardTray
{
    internal sealed class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly P1MeterDataProvider _homeWizardDataProvider;
        private readonly SunnyBoyDataProvider _sunnyBoyDataProvider;
        private readonly Ftxm25DataProvider _ftxm25DataProvider;

        private NotifyIcon TrayIcon { get; set; }
        private Timer UpdateTimer { get; set; }

        public App(AppSettings appSettings, P1MeterDataProvider homeWizardDataProvider, SunnyBoyDataProvider sunnyBoyDataProvider, Ftxm25DataProvider ftxm25DataProvider)
        {
            _appSettings = appSettings;
            _homeWizardDataProvider = homeWizardDataProvider;
            _sunnyBoyDataProvider = sunnyBoyDataProvider;
            _ftxm25DataProvider = ftxm25DataProvider;

            var menu = MenuBuilder.Build(new[]
            {
                new Menu("Airco", new []
                {
                    new Menu("Presets", new []
                    {
                        new Menu("Max", async (s, e) => await _ftxm25DataProvider.SetMax()),
                        new Menu("Normal", async (s, e) => await _ftxm25DataProvider.SetLevel2()),
                        new Menu("Eco", async (s, e) => await _ftxm25DataProvider.SetEco()),
                        new Menu("Dehumidify", async (s, e) => await _ftxm25DataProvider.SetDehumidify()),
                    }),
                    new Menu("Power", new []
                    {
                        new Menu("On", async (s, e) => await _ftxm25DataProvider.SetLevel2()),
                        new Menu("Off", async (s, e) => await _ftxm25DataProvider.SetOff())
                    }),
                    new Menu("Status", async (s, e) => MessageBox.Show(await _ftxm25DataProvider.GetStatus(), "SunnyTray"))
                }),
                new Menu("Open Log", (s, e) => Process.Start("notepad.exe", "./log.txt")),
                new Menu("Quit",  async (s, e) => await Exit()),
            });

            TrayIcon = new NotifyIcon { Text = "SunnyTray", Icon = Resources.SunIcon, Visible = true, ContextMenuStrip = menu };
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

                TrayIcon.Text = $"""
                {FormatPower(sunnyPower)} Opbrengst
                {FormatPower(homeWizardPower)} {(homeWizardPower > 0 ? "Afname" : "Injectie")}
                {FormatPower(sunnyPower + homeWizardPower)} Verbruik
                """;

                UpdateTimer.Interval = TimeSpan.FromSeconds(_appSettings.UpdateIntervalSeconds).TotalMilliseconds;
                UpdateTimer.Start();
            }
            catch (Exception ex)
            {
                TrayIcon.Text = "An error has occured. Please see the log file.";
                Log.Error(ex, ex.Message);
            }
        }

        private static string FormatPower(int power)
        {
            // Try to fill out with "thin space" character U+2009 " "
            var width = 0;
            power.ToString().ToArray().ToList().ForEach(x => { if (x == '1') width += 2; else width += 3; });
            var fillout = 20 - width;
            return $"{power}W{new string(' ', fillout)}".Replace("-", "–");
        }

        public async Task Exit(int exitCode = 0)
        {
            UpdateTimer.Stop();
            await _sunnyBoyDataProvider.Logout(); // TODO make disposable instead?
            if (TrayIcon != null) TrayIcon.Visible = false;
            Environment.Exit(exitCode);
        }
    }
}