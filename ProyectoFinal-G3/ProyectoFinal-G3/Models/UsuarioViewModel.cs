using System.ComponentModel.DataAnnotations;

namespace ProyectoFinal_G3.Models
{
    public class UsuarioViewModel
    {
        public Usuario Usuario { get; set; }
        public List<Roles> Roles { get; set; }

    }

    public class CambiarContrasennaViewModel
    {
        public string ContrasennaActual { get; set; } = string.Empty;
        public string ContrasennaNueva { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }
}
