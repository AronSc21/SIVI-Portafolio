using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class rptSolicitud
    {
        public int idComision { get; set; }
        public string noEmpleado { get; set; }
        public string noOficio { get; set; }
        public string objetivoComision { get; set; }
        public string lugarComision { get; set; }
        public Nullable<System.DateTime> fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFinal { get; set; }
        public string principalesAcciones { get; set; }
        public Nullable<bool> avion { get; set; }
        public Nullable<bool> autobus { get; set; }
        public Nullable<bool> automovil { get; set; }
        public string noPlacas { get; set; }
        public Nullable<System.DateTime> horaSalida { get; set; }
        public Nullable<int> diasComision { get; set; }
        public Nullable<decimal> hospedaje { get; set; }
        public Nullable<decimal> alimentacion { get; set; }
        public Nullable<decimal> terrestre { get; set; }
        public Nullable<decimal> combustible { get; set; }
        public Nullable<decimal> peaje { get; set; }
        public Nullable<decimal> traslados { get; set; }
        public Nullable<decimal> otrosGastos { get; set; }
    }
}