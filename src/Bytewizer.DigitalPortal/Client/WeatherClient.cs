using System;
using System.IO;
using System.Net;
using System.Diagnostics;

using GHIElectronics.TinyCLR.Data.Json;

using Bytewizer.TinyCLR.DigitalPortal.Client.Models;


namespace Bytewizer.TinyCLR.DigitalPortal.Client
{
    public class WeatherClient
    {
        public static WeatherResponse Connect(string lat, string lon, string units, string appId)
        {
            var url = $"http://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude=minutely,hourly,alerts&units={units}&appid={appId}";

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
                                return  (WeatherResponse)JsonConverter.DeserializeObject(stream, typeof(WeatherResponse), CreateInstance);

                                //var reader = new StreamReader(stream);
                                //while (reader.Peek() != -1)
                                //{
                                //    var jsonResponse = reader.ReadToEnd();
                                //    Debug.WriteLine(jsonResponse);
                                //    return (WeatherResponse)JsonConverter.DeserializeObject(jsonResponse, typeof(WeatherResponse), CreateInstance);
                                //}

                                //using (var reader = new StreamReader(stream))
                                //{
                                //    do
                                //    {
                                //        var jsonResponse = reader.ReadToEnd();
                                //        Debug.WriteLine(jsonResponse);
                                //        return (WeatherResponse)JsonConverter.DeserializeObject(jsonResponse, typeof(WeatherResponse), CreateInstance);

                                //    } while (!reader.EndOfStream);
                                //}
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return null;
        }

        private static object CreateInstance(string path, JToken root, Type baseType, string name, int length)
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


//if (path == "/" || name == "daily")
//    return new Daily[length];

//if (path == "//daily" || name == null)
//    return new Daily();

//if (path == "//daily" || name == "weather")
//    return new Weather[length];

//if (path == "//daily/weather" || name == null)
//    return new Weather();

//if (path == "/current" || name == "weather")
//    return new Weather[length];

//if (path == "/current/weather" || name == null)
//    return new Weather();