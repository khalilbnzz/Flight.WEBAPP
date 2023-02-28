using Flight.WEBAPP.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Flight.WEBAPP.Models
{
    public class FlightModel
    {
        public List<Datuma> data { get; set; }
    }
    public class Datuma
    {
        [Display(Name = "Date du vol")]
        public string flight_date { get; set; }
        [Display(Name = "Statut du vol")]
        public string flight_status { get; set; }
        public Flight.WEBAPP.Common.Models.Departure departure { get; set; }
        public Flight.WEBAPP.Common.Models.Arrival arrival { get; set; }
        public Flight.WEBAPP.Common.Models.Airline airline { get; set; }
        public Flight.WEBAPP.Common.Models.Flight flight { get; set; }
        public object aircraft { get; set; }
        public object live { get; set; }
    }
}
