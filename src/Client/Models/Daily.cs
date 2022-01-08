namespace Bytewizer.TinyCLR.DigitalPortal.Client.Models
{

#pragma warning disable IDE1006 // Naming Styles

    public class Daily
    {
        public int dt { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public Temp temp { get; set; }
        public FeelsLike feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        public Weather[] weather { get; set; }
        public int clouds { get; set; }
        public double rain { get; set; }
        public double uvi { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles

}
