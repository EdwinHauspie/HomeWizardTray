namespace HomeWizardTray.DataProviders.SunnyBoy
{
    internal sealed partial class SunnyBoyDataProvider
    {
        private class LoginResponse
        {
            public LoginResult Result { get; set; }
        }

        public class LoginResult
        {
            public string Sid { get; set; }
        }
    }
}
