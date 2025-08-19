namespace ProyectoFinal_G3.Models
{
    public class Factura
    {
        public int Id_Factura { get; set; }
        public int? Id_Venta { get; set; }
        public int? Id_Reparacion { get; set; }
        public int Id_TipoFactura { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal Total { get; set; }

      
        public int? Id_Cliente { get; set; } 
        public string ClienteNombre { get; set; } = string.Empty;
        public List<DetalleVenta>? Detalles { get; set; } = new List<DetalleVenta>();
    }
}
