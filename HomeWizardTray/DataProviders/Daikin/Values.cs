namespace HomeWizardTray.DataProviders.Daikin
{
    internal static class Power
    {
        public static string Off = "0";
        public static string On = "1";

        public static string GetName(string value)
        {
            switch (value)
            {
                case "1": return nameof(On);
                default: return nameof(Off);
            }
        }
    }

    internal static class Mode
    {
        public static string Auto = "0";
        public static string Dehumidify = "2";
        public static string Cooling = "3";
        public static string Heating = "4";
        public static string FanOnly = "6";

        public static string GetName(string value)
        {
            switch (value)
            {
                case "2": return nameof(Dehumidify);
                case "3": return nameof(Cooling);
                case "4": return nameof(Heating);
                case "6": return nameof(FanOnly);
                default: return nameof(Auto);
            }
        }
    }

    internal static class FanSpeed
    {
        public static string Auto = "A";
        public static string Silent = "B";
        public static string Level1 = "3";
        public static string Level2 = "4";
        public static string Level3 = "5";
        public static string Level4 = "6";
        public static string Level5 = "7";
        
        public static string GetName(string value)
        {
            switch (value)
            {
                case "B": return nameof(Silent);
                case "3": return nameof(Level1);
                case "4": return nameof(Level2);
                case "5": return nameof(Level3);
                case "6": return nameof(Level4);
                case "7": return nameof(Level5);
                default: return nameof(Auto);
            }
        }
    }

    internal static class FanMotion
    {
        public static string None = "0";
        public static string Vertical = "1";
        public static string Horizontal = "2";
        public static string Full = "3";

        public static string GetName(string value)
        {
            switch (value)
            {
                case "1": return nameof(Vertical);
                case "2": return nameof(Horizontal);
                case "3": return nameof(Full);
                default: return nameof(None);
            }
        }
    }

    internal static class SpecialMode
    {
        public static string Streamer = "0";
        public static string Powerful = "1";
        public static string Econo = "2";
    }

    internal static class SpecialModeState
    {
        public static string Off = "0";
        public static string On = "1";
    }
}
