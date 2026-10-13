using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class Pers_permVM
    {
        public string NoEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string ClavePuesto { get; set; }
        public Nullable<int> AreaId { get; set; }
        public string Tipo_Contrato { get; set; }
        public Nullable<System.DateTime> Fecha_Ingreso { get; set; }
        public string Telefono { get; set; }
        public string Extension { get; set; }
        public Nullable<bool> Vigente { get; set; }
        public string Area { get; set; }
        public Nullable<decimal> hospedaje { get; set; }
        public Nullable<decimal> alimentacion { get; set; }        
    }
}