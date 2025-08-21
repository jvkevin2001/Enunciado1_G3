using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CasoEstudio2_API.Models
{
    public class CasasModel
    {
        public long IdCasa { get; set; }
        public string DescripcionCasa { get; set; } = string.Empty;
        public decimal PrecioCasa { get; set; }
        public string? UsuarioAlquiler { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string FechaAlquiler { get; set; } = string.Empty;
    }
}
