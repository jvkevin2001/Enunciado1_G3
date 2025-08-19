namespace Proyecto_API.Models
{
    public class Reparacion
    {
        public int Id_Reparacion { get; set; }
        public int Id_Clientes { get; set; }
        public string? Cliente { get; set; }
        public string? EquipoDescripcion { get; set; }
        public string? TipoMaquina { get; set; }
        public DateTime FechaServicio { get; set; }
        public DateTime? FechaSalida { get; set; }
        public decimal CostoServicio { get; set; }
        public int? Id_Inventario { get; set; }
        public string? ProductoNombre { get; set; }
        public string? Estado { get; set; }

    }
}
