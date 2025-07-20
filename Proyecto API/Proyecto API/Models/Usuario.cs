namespace Proyecto_API.Models
{
    public class Usuario
    {
        public long IdUsuario { get; set; }
        public string? Nombre_Completo { get; set; }
        public string? Correo { get; set; }
        public string? Contrasenna { get; set; }
        public bool Estado { get; set; } = true;
        public string? Token { get; set; }
        public int Id_Rol { get; set; }
    }
}
