using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class ComisionGeneralVM
    {
        public int id { get; set; }
        public string noEmpleado { get; set; }
        public string noOficio { get; set; }
        public Nullable<int> tipoGasto { get; set; }
        public string objetivoComision { get; set; }
        public string lugarComision { get; set; }
        public Nullable<bool> avion { get; set; }
        public Nullable<bool> autobus { get; set; }
        public Nullable<bool> automovil { get; set; }
        public string noPlacas { get; set; }
        public Nullable<System.DateTime> fechaInicio { get; set; }
        public Nullable<System.DateTime> fechaFinal { get; set; }
        public Nullable<System.DateTime> horaSalida { get; set; }
        public Nullable<int> diasComision { get; set; }
        public string ctaBancaria { get; set; }
        public Nullable<System.DateTime> fechaPago { get; set; }
        public string noOperacion { get; set; }
        public string principalesAcciones { get; set; }
        public int idUsuario { get; set; }
        public System.DateTime fechaAlta { get; set; }
        public string actividadesrealizadas { get; set; }
        public string resultadosObtenidos { get; set; }
        public string contribuciones { get; set; }
        public string noPoliza { get; set; }
        public string rutaOficio { get; set; }
        public Nullable<System.DateTime> fechaOficio { get; set; }
        public Nullable<System.DateTime> fechaComprobacion { get; set; }
        public Nullable<decimal> reintegro { get; set; }
        public Nullable<bool> reintegrado { get; set; }
        public string jefeComision { get; set; }
        public string observaciones { get; set; }
        public Nullable<bool> validadoRF { get; set; }
        public Nullable<bool> reposicion { get; set; }
        public Nullable<bool> permitirCorrecion { get; set; }

    }
}