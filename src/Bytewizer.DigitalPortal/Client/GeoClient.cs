using System.IO;
using System.Diagnostics;
using System.Net;

using GHIElectronics.TinyCLR.Data.Json;

namespace Bytewizer.TinyCLR.DigitalPortal.Client
{
    class GeoClient
    {
        public static GeoResponse Connect()
        {
            var url = $"http://ip-api.com/json/?fields=status,message,region,city,lat,lon,offset,query";

            try
            {
                using (var request = WebRequest.Create(url) as HttpWebRequest)
                {
                    request.KeepAlive = false;
                    request.ReadWriteTimeout = 2000;

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                //return  (WeatherResponse)JsonConverter.DeserializeObject(stream, typeof(WeatherResponse), CreateInstance);

                                var reader = new StreamReader(stream);
                                while (reader.Peek() != -1)
                                {
                                    var jsonResponse = reader.ReadToEnd();
                                    Debug.WriteLine(jsonResponse);
                                    return (GeoResponse)JsonConverter.DeserializeObject(jsonResponse, typeof(GeoResponse));
                                }
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
    }
}