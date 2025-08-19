namespace ProyectoFinal_G3.Models
{
    public class VentaDirecta
    {
        public int Id_Venta { get; set; }
        public int Id_Cliente { get; set; }
        public decimal Total { get; set; }

        public List<DetalleVenta> Detalles { get; set; }

        public VentaDirecta()
        {
            Detalles = new List<DetalleVenta>();
        }
    }
}
