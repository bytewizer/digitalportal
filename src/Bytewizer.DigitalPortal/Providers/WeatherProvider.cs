using System;

using Bytewizer.TinyCLR.DigitalPortal.Client;
using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WeatherData
    {
        public WeatherData()
        {
            Forcast = new Forcast[5];
        }

        public DateTime DateTime { get; set; }
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

    public static class WeatherProvider
    {
        private static readonly object _lock = new object();

        private static bool _initialized;

        public static WeatherData Weather { get; private set; }

        private static WeatherIconProvider iconProvider;

        public static void Initialize()
        {
            if (_initialized)
                return;

            iconProvider = new WeatherIconProvider();

            Weather = new WeatherData()
            {
                Location = "city, st",
                Icon = ResourcesProvider.UxNA,
                Temp = "100",
                High = "100",
                Low = "100",
                Wind = "10",
                Humidity = "10",
                Description = "description",
                Main = "main",
            };

            for (var i = 0; i < 5; i++)
            {
                Weather.Forcast[i] = new Forcast
                {
                    Date = "mmm dd",
                    Icon = ResourcesProvider.UxNA,
                    High = "100",
                    Low = "100"
                };
            }

            _initialized = true;
        }

        public static WeatherData Connect()
        {
            if (NetworkProvider.IsConnected == false)
            {
                return null;
            }

            var settings = SettingsProvider.Flash;
            
            return Connect(settings.Lat, settings.Lon, settings.Units, settings.OwmAppId);
        }

        public static WeatherData Connect(string lat, string lon, Units units, string appId)
        {
            lock (_lock)
            {
                string tempUnits;
                string tempIcon;
                string windSpeed;
                switch (units) 
                {
                    case Units.Imperial:
                        tempUnits = "imperial";
                        tempIcon = ResourcesProvider.UxFahrenheit;
                        windSpeed = "mph";
                        break;

                    case Units.Metric:
                        tempUnits = "metric";
                        tempIcon = ResourcesProvider.UxCelsius;
                        windSpeed = "mps";
                        break;
                    default:
                        tempUnits = "standard";
                        tempIcon = ResourcesProvider.UxCelsius;
                        windSpeed = "mps";
                        break;
                }
             
                var response = WeatherClient.Connect(lat, lon, tempUnits, appId);
                if (response != null)
                {
                    var id = response.current.weather[0].id.ToString();
                    var icon = response.current.weather[0].icon.ToString();

                    string windDirection;
                    if (response.current.wind_speed > 0)
                    {
                        windDirection = GetWindDirection(response.current.wind_deg);
                    }
                    else
                    {
                        windDirection = string.Empty;
                    }



                    Weather = new WeatherData
                    {
                        DateTime = ConvertToDate(response.current.dt),
                        Temp = response.current.temp.ToString("N0"),
                        TempUnit = tempIcon,
                        Icon = iconProvider.GetIconUnicode(id, icon),
                        High = response.daily[0].temp.max.ToString("N0"),
                        Low = response.daily[0].temp.min.ToString("N0"),
                        Main = response.current.weather[0].main.ToString(),
                        Description = response.current.weather[0].description.ToString(),
                        Wind = string.Format($"wind: { response.current.wind_speed.ToString("N0") } { windSpeed } {windDirection}"),
                        Humidity = string.Format($"humidity: { response.current.humidity.ToString("N0") }%")
                    };

                    for (var i = 0; i < 5; i++)
                    {
                        id = response.daily[i + 1].weather[0].id.ToString();
                        icon = response.daily[i + 1].weather[0].icon.ToString();

                        Weather.Forcast[i] = new Forcast
                        {
                            Date = DateTime.Now.AddDays(i + 1).ToString("ddd dd"),
                            High = response.daily[i + 1].temp.max.ToString("N0"),
                            Low = response.daily[i + 1].temp.min.ToString("N0"),
                            Icon = iconProvider.GetIconUnicode(id, icon)
                        };
                    }

                    var value = response.current.temp.ToString(); // This works
                    //var value = response.current.temp; // This will not
                    var results = double.Parse(value).ToString();
                   //Debug.WriteLine(results);

                    return Weather;
                }

                return null;
            }
        }

        private static DateTime ConvertToDate(int timestamp)
        {
            var getDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return getDate.AddSeconds(timestamp).ToLocalTime();
        }

        public static string GetWindDirection(double deg)
        {
            string[] direction = new string[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
            double degrees = deg % 360;
            degrees = (degrees / 22.5) + 1;
            
            return direction[(int)degrees];
        }
    }
}