namespace ProyectoFinal_G3.Models
{
    public class Venta
    {
        public int Id_Venta { get; set; }
        public int Id_Cliente { get; set; }
        public Cliente? Cliente { get; set; } 
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVenta>? Detalles { get; set; }



    }
}
