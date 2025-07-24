namespace Proyecto_API.Models
{
    public class HistorialVentas
    {
        public int Id_Venta { get; set; }
        public int Id_Cliente { get; set; }
        public int Id_Usuario { get; set; }
        public DateTime FechaVenta { get; set; }

    }
}
