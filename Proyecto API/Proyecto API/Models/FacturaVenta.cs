namespace Proyecto_API.Models
{
    public class FacturaVenta
    {
        public int Id_Factura { get; set; }
        public int Id_Venta { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Total { get; set; }
    }
}
