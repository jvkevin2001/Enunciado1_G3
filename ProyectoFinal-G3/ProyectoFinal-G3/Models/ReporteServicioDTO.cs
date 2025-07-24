namespace ProyectoFinal_G3.Models
{
    public class ReporteServicioDTO
    {
        public string Fecha { get; set; } = "";
        public string Cliente { get; set; } = "";
        public string Usuario { get; set; } = "";
        public string Servicio { get; set; } = "";
        public decimal Costo { get; set; }
        public string Estado { get; set; } = "";
    }
}