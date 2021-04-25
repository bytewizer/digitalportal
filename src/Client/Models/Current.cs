namespace Bytewizer.TinyCLR.DigitalPortal.Client.Models
{

#pragma warning disable IDE1006 // Naming Styles
    
    public class Current
    {
        public int dt { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        public double temp { get; set; }
        public double feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        //public Weather[] weather { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles

}
