﻿namespace HomeWizardTray.DataProviders.SunnyBoy
{
    internal sealed class LoginResponse
    {
        public LoginResult Result { get; set; }
    }

    internal sealed class LoginResult
    {
        public string Sid { get; set; }
    }
}
