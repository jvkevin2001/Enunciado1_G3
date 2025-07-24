namespace Proyecto_API.Models
{
    public class Factura
    {
        public int Id_Factura { get; set; }
        public int Id_TipoFactura { get; set; }
        public int? Id_Venta { get; set; }
        public int? Id_Reparacion { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Total { get; set; }
    }
}
