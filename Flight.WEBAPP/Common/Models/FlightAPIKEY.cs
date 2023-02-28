namespace Flight.WEBAPP.Common.Models
{
    public class FlightAPIKEY
    {
        public string link { get; set; }
        public string name { get; set; } = "access_key";
        public string value { get; set; }
    }

    public class AirportAppSetting
    {
        public string linkLatLong { get; set; }
        public string linkCode { get; set; }
    }
}
