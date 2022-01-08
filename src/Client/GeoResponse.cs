namespace Bytewizer.TinyCLR.DigitalPortal.Client
{

#pragma warning disable IDE1006 // Naming Styles

    public class GeoResponse
    {
        public string status { get; set; }
        
        public string message { get; set; }

        public string region { get; set; }

        public string city { get; set; }

        public float lat { get; set; }

        public float lon { get; set; }

        public int offset { get; set; }

        public string query { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles
}