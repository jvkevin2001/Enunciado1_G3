namespace ProyectoFinal_G3.Models
{
    public class ReporteVentaDTO
    {
        public string Fecha { get; set; } = "";
        public string Cliente { get; set; } = "";
        public string Usuario { get; set; } = "";
        public string Producto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Total { get; set; }
    }
}
