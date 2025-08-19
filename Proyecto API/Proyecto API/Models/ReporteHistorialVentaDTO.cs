namespace Proyecto_API.Models
{
    public class ReporteHistorialVentaDTO
    {
        public int Id_Venta { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaEmision { get; set; }
        public int? Id_Cliente { get; set; }
        public string? NombreCliente { get; set; }
        public int? Id_Usuario { get; set; }
        public string? NombreUsuario { get; set; }
        public int Id_DetalleVenta { get; set; }
        public int? Id_Inventario { get; set; }
        public int? Cantidad { get; set; }
        public decimal? PrecioUnitario { get; set; }
        public decimal PrecioLinea { get; set; }
        public decimal TotalVentaFactura { get; set; }
    }
}
