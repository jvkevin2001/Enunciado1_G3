namespace ProyectoFinal_G3.Models
{
    public class ReporteAnalisisViewModel
    {
        public DateTime FechaInicio { get; set; } = DateTime.Today.AddDays(-7);
        public DateTime FechaFin { get; set; } = DateTime.Today;
        public string Formato { get; set; } = "PDF";

        public List<ReporteVentaDTO> Ventas { get; set; } = [];
        public List<ReporteServicioDTO> Servicios { get; set; } = [];
    }
}
