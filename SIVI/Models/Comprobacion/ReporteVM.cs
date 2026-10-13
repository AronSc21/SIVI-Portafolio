using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class ReporteVM
    {
        public int idComision { get; set; }
        public string noEmpleado { get; set; }
        public string noOficio { get; set; }
        public string objetivoComision { get; set; }
        public string lugarComision { get; set; }
        public Nullable<System.DateTime> fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFinal { get; set; }
        public string principalesAcciones { get; set; }
        public string actividadesrealizadas { get; set; }
        public string resultadosObtenidos { get; set; }
        public string contribuciones { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public byte idArea { get; set; }
        public string Area { get; set; }
        public string noPoliza { get; set; }
        public string rutaOficio { get; set; }
        public Nullable<decimal> montoOtorgado { get; set; }
        public Nullable<decimal> montoComprobado { get; set; }
        public Nullable<System.DateTime> fechacomprobacion { get; set; }
        public Nullable<decimal> diferencia { get; set; }
        public Nullable<bool> permitirCorrecion { get; set; }
    }
}