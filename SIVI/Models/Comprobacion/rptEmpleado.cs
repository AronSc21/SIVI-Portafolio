using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class rptEmpleado
    {
        public string NoEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string ClavePuesto { get; set; }
        public Nullable<byte> AreaId { get; set; }
        public string Tipo_Contrato { get; set; }
        public string Area { get; set; }
    }
}