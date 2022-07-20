using System;
using System.IO;
using System.Net;
using System.Threading;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.DigitalPortal.Client;
using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

using GHIElectronics.TinyCLR.Data.Json;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class WeatherService
    {
        private readonly ILogger _logger;
        private readonly SettingsService _settings;
        private readonly ClockService _clock;

        public WeatherService(ILoggerFactory loggerFactory, SettingsService settings, ClockService clock)
        {
            _logger = loggerFactory.CreateLogger(nameof(WeatherWorker));
            _settings = settings;
            _clock = clock;
        }

        public void ConnectAsync()
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(delegate (object state)
                {
                    Connect();
                }));
        }

        public void Connect()
        {
            var settings = SettingsService.Flash;

            var response = Connect(settings.Lat, settings.Lon, settings.Units, settings.OwmAppId);
            if (response != null)
            {
                _settings.SetWeather(response);
                _clock.SetTime(response.DateTime, settings.TimeOffset);
            }
        }

        private WeatherResponse Connect(string lat, string lon, string units, string appId)
        {
            var url = $"http://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,alerts&units={units}&appid={appId}";
            
            _logger.LogTrace("Request Url: {0}", url);

            try
            {
                using (var request = WebRequest.Create(url) as HttpWebRequest)
                {
                    request.KeepAlive = false;
                    request.ReadWriteTimeout = 3000;

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    do
                                    {
                                        var jsonResponse = reader.ReadToEnd();
                                        _logger.LogTrace("Response Json: {0}", jsonResponse);

                                        return (WeatherResponse)JsonConverter.DeserializeObject(jsonResponse, typeof(WeatherResponse), CreateInstance);

                                    } while (!reader.EndOfStream);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request Url: {0}", url);
            }

            return null;
        }

        private WeatherObject Connect(string lat, string lon, Units units, string appId)
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

            var response = Connect(lat, lon, tempUnits, appId);
            try 
            {
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

                    var weatherObject = new WeatherObject();

                    weatherObject.DateTime = ConvertToDate(response.current.dt);
                    weatherObject.TimeOffset = response.timezone_offset;
                    weatherObject.Temp = response.current.temp.ToString("N0");
                    weatherObject.TempUnit = tempIcon;
                    weatherObject.Icon = SettingsService.Weather.GetIconUnicode(id, icon);
                    weatherObject.High = response.daily[0].temp.max.ToString("N0");
                    weatherObject.Low = response.daily[0].temp.min.ToString("N0");
                    weatherObject.Main = response.current.weather[0].main.ToString();
                    weatherObject.Description = response.current.weather[0].description.ToString();
                    weatherObject.Wind = string.Format($"wind: {response.current.wind_speed.ToString("N0")} {windSpeed} {windDirection}");
                    weatherObject.Humidity = string.Format($"humidity: {response.current.humidity.ToString("N0")}%");

                    for (var i = 0; i < 5; i++)
                    {
                        id = response.daily[i + 1].weather[0].id.ToString();
                        icon = response.daily[i + 1].weather[0].icon.ToString();

                        weatherObject.Forcast[i] = new Forcast
                        {
                            Date = DateTime.Now.AddDays(i + 1).ToString("ddd dd"),
                            High = response.daily[i + 1].temp.max.ToString("N0"),
                            Low = response.daily[i + 1].temp.min.ToString("N0"),
                            Icon = SettingsService.Weather.GetIconUnicode(id, icon)
                        };
                    }

                    return weatherObject;
                }
            }
            catch
            {
               return null;
            }

            return null;

        }

        private DateTime ConvertToDate(int timestamp)
        {
            var getDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return getDate.AddSeconds(timestamp).ToLocalTime();
        }

        private string GetWindDirection(double deg)
        {
            string[] direction = new string[] 
                { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                  "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
            
            double degrees = deg % 360;
            degrees = (degrees / 22.5) + 1;

            return direction[(int)degrees];
        }

        private object CreateInstance(string path, JToken root, Type baseType, string name, int length)
        {
            if (path == "/current" & name == null)
                return new Current();

            if (path == "/" & name == "daily")
                return new Daily[length];

            if (path == "//daily" & name == null)
                return new Daily();

            if (path == "//daily" & name == "weather")
                return new Weather[length];

            if (path == "//daily/weather" & name == null)
                return new Weather();

            if (path == "/current" & name == "weather")
                return new Weather[length];

            if (path == "/current/weather" & name == null)
                return new Weather();

            return null;
        }
    }
}