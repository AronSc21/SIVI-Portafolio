using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIVI.Models.Comprobacion
{
    public class AreasVM
    {
        public byte Id { get; set; }
        public string Descripcion { get; set; }
        public Nullable<byte> PadreID { get; set; }
        public bool Vigente { get; set; }
    }
}