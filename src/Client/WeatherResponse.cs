using Bytewizer.TinyCLR.DigitalPortal.Client.Models;

namespace Bytewizer.TinyCLR.DigitalPortal.Client
{

#pragma warning disable IDE1006 // Naming Styles

    public class WeatherResponse
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
        public int timezone_offset { get; set; }
        public Current current { get; set; }
        public Daily[] daily { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles
}
