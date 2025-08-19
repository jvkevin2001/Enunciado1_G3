namespace ProyectoFinal_G3.Models
{
    public class DetalleVenta
    {
        public int Id_DetalleVenta { get; set; }
        public int Id_Venta { get; set; }
        public int Id_Inventario { get; set; }
        public string ProductoNombre { get; set; } = string.Empty; 
        public int Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
