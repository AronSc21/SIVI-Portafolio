using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class VehiculosVM
    {
        public int id { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string tipo { get; set; }
        public string placa { get; set; }
        public string color { get; set; }
    }
}