namespace Proyecto_API.Models
{
    public class DetalleVenta
    {
        public int Id_DetalleVenta { get; set; }
        public int Id_Venta { get; set; }
        public int Id_Inventario { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
