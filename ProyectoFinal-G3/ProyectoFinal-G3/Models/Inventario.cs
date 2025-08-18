namespace ProyectoFinal_G3.Models
{
    public class Inventario
    {
        public int Id_Inventario { get; set; }
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Proveedor { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Id_Usuario { get; set; }

    }
}
