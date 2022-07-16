using System.Drawing;

using Bytewizer.TinyCLR.DigitialPortal.Assets.Properties;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class ResourcesProvider
    {
        public static Font SmallDigitalFont = Resources.GetFont(Resources.FontResources.SmallDigitalFont); 
        public static Font MediumDigitalFont = Resources.GetFont(Resources.FontResources.MediumDigitalFont);
        public static Font LargeDigitalFont = Resources.GetFont(Resources.FontResources.LargeDigitalFont);
        public static Font SmallRobotoFont = Resources.GetFont(Resources.FontResources.SmallRobotoFont);
        public static Font SmallWeatherIcons = Resources.GetFont(Resources.FontResources.SmallWeatherIcons);
        public static Font MediumWeatherIcons = Resources.GetFont(Resources.FontResources.MediumWeatherIcons);
        public static Font SmallUxIcons = Resources.GetFont(Resources.FontResources.SmallUxIcons);

        public const string UxNotification = "\ue7f5"; //59381
        public const string UxNotificationOff = "\ue7f6"; //59382
        public const string UxWiFi = "\ue63e"; //58942
        public const string UxWiFiOff = "\ue648"; // 58952
        public const string UxAccessTime = "\ue192"; // 57746
        public const string UxCloud = "\ue42d"; //58413
        public const string UxSettings = "\ue8b8"; //59576
        public const string UxNavigateBefore = "\ue5e0"; //58848
        public const string UxNavigateNext = "\ue5e1"; //58849

        public const string UxFahrenheit = "\uf045"; //61509
        public const string UxCelsius = "\uf03c"; //61500

        public const string UxNA = "\uf07b"; //61563

        public const string UxSave = "\ue171"; //57713

        public const string UxCached = "\ue028"; //57384
    }
}