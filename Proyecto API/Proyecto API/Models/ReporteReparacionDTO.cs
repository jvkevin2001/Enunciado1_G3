namespace ProyectoFinal_G3.Models
{
    public class ReporteReparacionDTO
    {
        public int IdReparacion { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public decimal Costo { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
