using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class MontoMaximoVM
    {
        public int id { get; set; }
        public int idComision { get; set; }
        public Nullable<decimal> hospedaje { get; set; }
        public Nullable<decimal> alimentacion { get; set; }
        public Nullable<decimal> terrestre { get; set; }
        public Nullable<decimal> combustible { get; set; }
        public Nullable<decimal> peaje { get; set; }
        public Nullable<decimal> traslados { get; set; }
        public Nullable<decimal> otrosGastos { get; set; }
        public int idUsuarioAlta { get; set; }
        public System.DateTime fechaAlta { get; set; }

        public static decimal? MontoOtorgado(int idComision)
        {
            decimal? res = null;
            using (contabilidadEntities db = new contabilidadEntities())
            {
                MontoMaximo montoMaximo = (from m in db.MontoMaximo
                                           where m.idComision == idComision
                                           select m).FirstOrDefault();
                if (montoMaximo != null)
                {
                    res = montoMaximo?.hospedaje + montoMaximo?.alimentacion + montoMaximo?.terrestre + montoMaximo?.combustible + montoMaximo?.peaje + montoMaximo?.traslados + montoMaximo?.otrosGastos;
                }
            }
            return res;
        }
        public static decimal? Montocomprobado(int idComision)
        {
            decimal? res = null;
            using (contabilidadEntities db = new contabilidadEntities())
            {
                List<GastosComprobar> gastos = (from m in db.GastosComprobar
                                                where m.idComision == idComision
                                                select m).ToList();
                if(gastos.Count > 0)
                {
                    res = gastos.Sum(p => p.importe);
                }
                
            }
            return res;
        }
    }    
}