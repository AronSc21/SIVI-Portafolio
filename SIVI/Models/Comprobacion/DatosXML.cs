using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class DatosXML
    {
        public int id {  get; set; }
        public string RFCEmisor { get; set; }
        public string noFactura {  get; set; }
        public DateTime fechaExpedicion { get; set; }
        public string razonSocialEmisor { get; set; }
        public decimal importe { get; set; }
        public decimal gasto { get; set; }
        public int idTipoGasto { get; set; }
        public int? idTipo {  get; set; }
        public int idFactura {  get; set; }
        public int idComision {  get; set; }
        public string clasificadorGasto { get; set; }
        public string rutaXML { get; set; }
        public string rutaPDF { get; set; }
        public string rutaPDFFirmado { get; set; }
        public string rutaComprobacion { get; set; }
        public string metodoPago { get; set; }
    }
}