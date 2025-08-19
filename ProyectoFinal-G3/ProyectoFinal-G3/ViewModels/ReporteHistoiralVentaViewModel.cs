namespace ProyectoFinal_G3.ViewModels
{
    public class ReporteHistorialVentaViewModel
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int Id_Inventario { get; set; }  
        public int Cantidad { get; set; }       
        public decimal PrecioLinea { get; set; } 
        public decimal TotalVentaFactura { get; set; } 
    }
}
