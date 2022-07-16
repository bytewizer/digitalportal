using System;

using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    [Serializable]
    public class FlashObject
    {
        public FlashObject()
        {
            Ssid = "ssid";
            Password = "password";
            NetworkEnabled = false;
            DefaultPage = 0;
            ThemeColor = new byte[] { 255, 28, 172, 120 };
            Lat = "39.03";
            Lon = "-77.5";
            Query = null;
            Units = Units.Imperial;
            TimeServer = "pool.ntp.org";
            TimeOffset = -5;
            Location = "Ashburn, VA";
            FlipOrientation = true;

            // Sign up for your own unique free App Id: http://openweathermap.org/
            OwmAppId = "52a6cba2df23488f29f127047aaf4d81";  
            
            ShowDow = true;
            ShowDate = true;
            ShowWeather = true;
        }

        public string Ssid { get; set; }
        public string Password { get; set; }
        public bool NetworkEnabled { get; set; }
        public int DefaultPage { get; set; }
        public byte[] ThemeColor { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
        public string Query { get; set; }
        public Units Units { get; set; }
        public string TimeServer { get; set; }
        public int TimeOffset { get; set; }
        public string Location { get; set; }
        public bool FlipOrientation { get; set; }
        public string OwmAppId { get; set; }
        public bool ShowDow { get; set; }
        public bool ShowDate { get; set; }
        public bool ShowWeather { get; set; }
    }
}