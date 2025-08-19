namespace ProyectoFinal_G3.Models
{
    public class ReporteInventarioDTO
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
