namespace HomeWizardTray.DataProviders.Daikin
{
    internal static class Power
    {
        public static string Off = "0";
        public static string On = "1";
    }

    internal static class Mode
    {
        public static string Auto = "0";
        public static string Dehumid = "2";
        public static string Cool = "3";
        public static string Heat = "4";
        public static string FanOnly = "6";
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
    }

    internal static class FanMotion
    {
        public static string None = "0";
        public static string Vertical = "1";
        public static string Horizontal = "2";
        public static string Full = "3";
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
