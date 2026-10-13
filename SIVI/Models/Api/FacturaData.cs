using System;

namespace SIVI.Models.Api
{
    public class FacturaData
    {
        public string uuid { get; set; }
        public string serie { get; set; }
        public string folio { get; set; }

        public string rfcEmisor { get; set; }
        public string razonSocialEmisor { get; set; }

        public string rfcReceptor { get; set; }
        public string razonSocialReceptor { get; set; }

        public decimal subtotal { get; set; }
        public decimal total { get; set; }

        public DateTime fecha { get; set; }
    }
}