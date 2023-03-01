using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Flight.WEBAPP.Models
{
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
        public string VilleDepart { get; set; }
        public string VilleArrive { get; set; }
        public string AeroportDepart { get; set; }
        public string AeroportArrive { get; set; }
        public string NumeroVol { get; set; }
    }
}
