namespace ProyectoFinal_G3.ViewModels
{
    public class ReporteReparacionViewModel
    {
        public int IdReparacion { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;   
        public string Servicio { get; set; } = string.Empty;  
        public string Estado { get; set; } = string.Empty;    
        public DateTime Fecha { get; set; }              
        public decimal Costo { get; set; }
    }
}
