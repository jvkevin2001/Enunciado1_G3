namespace ProyectoFinal_G3.Models

{
    public class Reporte
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string UsuarioGenerador { get; set; } = "";
        public string Formato { get; set; } = "";
        public string? RutaArchivo { get; set; }
    }

}