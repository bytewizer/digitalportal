using System;
using System.Diagnostics;

using GHIElectronics.TinyCLR.Data.Json;

using Bytewizer.TinyCLR.DigitalPortal.Client;
using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

namespace Bytewizer.TinyCLR.JsonTest
{
    class Program
    {
        static void Main()
        {
            string validJson = "{\"lat\":33.8578,\"lon\":-117.7869,\"timezone\":\"America/Los_Angeles\",\"timezone_offset\":-25200,\"current\":{\"dt\":1619231063,\"sunrise\":1619183415,\"sunset\":1619231290,\"temp\":60.08,\"feels_like\":58.95,\"pressure\":1016,\"humidity\":67,\"dew_point\":49.1,\"uvi\":0.11,\"clouds\":1,\"visibility\":10000,\"wind_speed\":9.22,\"wind_deg\":280,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"clear sky\",\"icon\":\"01d\"}]},\"daily\":[{\"dt\":1619204400,\"sunrise\":1619183415,\"sunset\":1619231290,\"moonrise\":1619218140,\"moonset\":1619176800,\"moon_phase\":0.37,\"temp\":{\"day\":66.33,\"min\":57.27,\"max\":67.86,\"night\":57.97,\"eve\":60.67,\"morn\":57.51},\"feels_like\":{\"day\":65.01,\"night\":56.25,\"eve\":59.5,\"morn\":56.25},\"pressure\":1016,\"humidity\":50,\"dew_point\":46.56,\"wind_speed\":11.16,\"wind_deg\":221,\"wind_gust\":5.59,\"weather\":[{\"id\":802,\"main\":\"Clouds\",\"description\":\"scattered clouds\",\"icon\":\"03d\"}],\"clouds\":40,\"pop\":0,\"uvi\":8.64},{\"dt\":1619290800,\"sunrise\":1619269748,\"sunset\":1619317737,\"moonrise\":1619308620,\"moonset\":1619265180,\"moon_phase\":0.41,\"temp\":{\"day\":67.59,\"min\":55.18,\"max\":69.93,\"night\":58.35,\"eve\":66.2,\"morn\":55.18},\"feels_like\":{\"day\":66.2,\"night\":53.89,\"eve\":64.72,\"morn\":53.89},\"pressure\":1016,\"humidity\":46,\"dew_point\":45.45,\"wind_speed\":11.3,\"wind_deg\":231,\"wind_gust\":4.74,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"clear sky\",\"icon\":\"01d\"}],\"clouds\":4,\"pop\":0,\"uvi\":8.44},{\"dt\":1619377200,\"sunrise\":1619356081,\"sunset\":1619404184,\"moonrise\":1619399280,\"moonset\":1619353560,\"moon_phase\":0.45,\"temp\":{\"day\":65.07,\"min\":54.43,\"max\":65.07,\"night\":57.25,\"eve\":62.26,\"morn\":54.43},\"feels_like\":{\"day\":63.43,\"night\":53.1,\"eve\":60.26,\"morn\":53.1},\"pressure\":1014,\"humidity\":46,\"dew_point\":43.2,\"wind_speed\":13.47,\"wind_deg\":231,\"wind_gust\":13.51,\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"overcast clouds\",\"icon\":\"04d\"}],\"clouds\":100,\"pop\":0.02,\"uvi\":6.37},{\"dt\":1619463600,\"sunrise\":1619442416,\"sunset\":1619490631,\"moonrise\":1619490000,\"moonset\":1619442000,\"moon_phase\":0.5,\"temp\":{\"day\":61.56,\"min\":55.62,\"max\":64.81,\"night\":55.67,\"eve\":61.97,\"morn\":55.65},\"feels_like\":{\"day\":59.49,\"night\":54.25,\"eve\":59.79,\"morn\":54.25},\"pressure\":1012,\"humidity\":44,\"dew_point\":38.79,\"wind_speed\":14.03,\"wind_deg\":223,\"wind_gust\":11.25,\"weather\":[{\"id\":804,\"main\":\"Clouds\",\"description\":\"overcast clouds\",\"icon\":\"04d\"}],\"clouds\":100,\"pop\":0.5,\"uvi\":6.82},{\"dt\":1619550000,\"sunrise\":1619528752,\"sunset\":1619577078,\"moonrise\":1619580840,\"moonset\":1619530620,\"moon_phase\":0.53,\"temp\":{\"day\":64.04,\"min\":52.99,\"max\":69.46,\"night\":59.88,\"eve\":68.9,\"morn\":52.99},\"feels_like\":{\"day\":62.11,\"night\":51.19,\"eve\":67.32,\"morn\":51.19},\"pressure\":1017,\"humidity\":42,\"dew_point\":39.67,\"wind_speed\":10.58,\"wind_deg\":235,\"wind_gust\":6.26,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"clear sky\",\"icon\":\"01d\"}],\"clouds\":2,\"pop\":0,\"uvi\":8.78},{\"dt\":1619636400,\"sunrise\":1619615089,\"sunset\":1619663525,\"moonrise\":1619671740,\"moonset\":1619619540,\"moon_phase\":0.56,\"temp\":{\"day\":77.18,\"min\":57.4,\"max\":82.58,\"night\":68.23,\"eve\":79.39,\"morn\":57.4},\"feels_like\":{\"day\":75.45,\"night\":55.65,\"eve\":79.39,\"morn\":55.65},\"pressure\":1021,\"humidity\":18,\"dew_point\":30.72,\"wind_speed\":11.12,\"wind_deg\":255,\"wind_gust\":11.81,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"clear sky\",\"icon\":\"01d\"}],\"clouds\":0,\"pop\":0,\"uvi\":9},{\"dt\":1619722800,\"sunrise\":1619701427,\"sunset\":1619749972,\"moonrise\":1619762520,\"moonset\":1619708760,\"moon_phase\":0.6,\"temp\":{\"day\":84.09,\"min\":64.99,\"max\":89.37,\"night\":72.1,\"eve\":87.06,\"morn\":64.99},\"feels_like\":{\"day\":81.16,\"night\":62.56,\"eve\":83.48,\"morn\":62.56},\"pressure\":1019,\"humidity\":12,\"dew_point\":26.28,\"wind_speed\":9.73,\"wind_deg\":251,\"wind_gust\":3,\"weather\":[{\"id\":801,\"main\":\"Clouds\",\"description\":\"few clouds\",\"icon\":\"02d\"}],\"clouds\":14,\"pop\":0,\"uvi\":9},{\"dt\":1619809200,\"sunrise\":1619787765,\"sunset\":1619836420,\"moonrise\":0,\"moonset\":1619798460,\"moon_phase\":0.64,\"temp\":{\"day\":83.97,\"min\":67.17,\"max\":88.29,\"night\":69.51,\"eve\":84.58,\"morn\":67.17},\"feels_like\":{\"day\":81.18,\"night\":65.34,\"eve\":81.72,\"morn\":65.34},\"pressure\":1016,\"humidity\":18,\"dew_point\":33.76,\"wind_speed\":9.64,\"wind_deg\":236,\"wind_gust\":2.75,\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"clear sky\",\"icon\":\"01d\"}],\"clouds\":9,\"pop\":0,\"uvi\":9}]}";
            Debug.WriteLine(validJson);  // This is valid JSON.  Checked on a couple differnt websites.

            var jsonResponse = (WeatherResponse)JsonConverter.DeserializeObject(validJson, typeof(WeatherResponse), CreateInstance);
            Debug.WriteLine($"Temp: {jsonResponse.current.temp}");
        }

        private static object CreateInstance(string path, string name, int length)
        {
            if (path == "/" & name == "daily")
                return new Daily[length];

            else if (path == "//daily" & name == null)
                return new Daily();

            else if (path == "//daily" & name == "weather")
                return new Weather[length];

            else if (path == "//daily/weather" & name == null)
                return new Weather();

            else if (path == "/current" & name == "weather")
                return new Weather[length];

            else if (path == "/current/weather" & name == null)
                return new Weather();
            
            else
                return null;
        }
    }
}
