using System;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WeatherObject
    {
        private readonly WeatherIcons _weatherIcons;

        public WeatherObject()
        {
            _weatherIcons = new WeatherIcons();

            Forcast = new Forcast[5];
            Location = "city, st";
            Icon = ResourcesProvider.UxNA;
            Temp = "100";
            TempUnit = ResourcesProvider.UxFahrenheit;
            High = "100";
            Low = "100";
            Wind = "10";
            Humidity = "10";
            Description = "description";
            Main = "main";

            for (var i = 0; i < 5; i++)
            {
                Forcast[i] = new Forcast
                {
                    Date = "mmm dd",
                    Icon = ResourcesProvider.UxNA,
                    High = "100",
                    Low = "100"
                };
            }
        }

        public string GetIconUnicode(string id, string icon)
        {
            return _weatherIcons.GetIconUnicode(id, icon);
        }

        public DateTime DateTime { get; set; }
        public int TimeOffset { get; set; }
        public string Location { get; set; }
        public string Temp { get; set; }
        public string TempUnit { get; set; }
        public string Icon { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Wind { get; set; }
        public string Humidity { get; set; }
        public Forcast[] Forcast { get; set; }
    }

    public class Forcast
    {
        public string Date { get; set; }
        public string Icon { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
    }
}