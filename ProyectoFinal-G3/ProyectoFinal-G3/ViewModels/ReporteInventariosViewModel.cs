namespace ProyectoFinal_G3.ViewModels
{
    public class ReporteInventarioViewModel
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
