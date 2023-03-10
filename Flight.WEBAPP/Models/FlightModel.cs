using Flight.WEBAPP.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Flight.WEBAPP.Models
{
    public class FlightModel
    {
        public List<Datuma> data { get; set; }
        public List<VolModel> dataVol { get; set; }
        public FlightModel()
        {
            data = new List<Datuma>();
            dataVol = new List<VolModel>();
        }
    }
    
    public class Datuma
    {
        [Display(Name = "Date du vol")]
        public string flight_date { get; set; }
        [Display(Name = "Statut du vol")]
        public string flight_status { get; set; }
        [Display(Name = "Duree du vol")]
        public string flight_time { get; set; }
        [Display(Name = "Distance entre aeroport")]
        public string distance { get; set; }
        public Flight.WEBAPP.Common.Models.Departure departure { get; set; }
        public Flight.WEBAPP.Common.Models.Arrival arrival { get; set; }
        public Flight.WEBAPP.Common.Models.Airline airline { get; set; }
        public Flight.WEBAPP.Common.Models.Flight flight { get; set; }
        public string lag { get; set; }
        public string lon { get; set; }
        public object aircraft { get; set; }
        public object live { get; set; }
        public int Identifier { get; set; }
        public string NamePlane { get; set; }
        public string KeroseneQuantity { get; set; }
    }

    public class GeoCordinations
    {
        public string Longiture { get; set; }
        public string Latitude { get; set; }
    }

    public class VolModel
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Depart")]
        public DateTime HeureDepart { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Arrivee")]
        public DateTime HeureArrive { get; set; }
        [Display(Name = "Avion")]
        public string ModelAvion { get; set; }
        [Display(Name = "Ville Depart")]
        public string VilleDepart { get; set; }
        [Display(Name = "Ville Arrivee")]
        public string VilleArrive { get; set; }
        [Display(Name = "Aeroport Depart")]
        public string AeroportDepart { get; set; }
        [Display(Name = "Aeroport Arrive")]
        public string AeroportArrive { get; set; }
        [Display(Name = "Numero du vol")]
        public string NumeroVol { get; set; }
        
        private double _vitesse = 459.0;
        [Display(Name = "Vitesse Moyenne")]
        public double Vitesse
        {
            get { return _vitesse; }
        }
        [Display(Name = "Consommation Moyenne")]
        public double Consommation
        {
            get { return _vitesse * 29; }
        }
    }
}
