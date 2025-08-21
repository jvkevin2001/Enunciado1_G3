namespace Proyecto_API.Models
{
    public class VentaCompleta
    {
        public Venta Venta { get; set; } = new Venta();
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
