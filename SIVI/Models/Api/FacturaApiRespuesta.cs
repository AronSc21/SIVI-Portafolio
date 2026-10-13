namespace SIVI.Models.Api
{
    public class FacturaApiRespuesta
    {
        public bool success { get; set; }
        public string message { get; set; }
        public FacturaData data { get; set; }
    }
}
