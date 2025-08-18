using Proyecto_API.Models;
using System.Security.Claims;

namespace Proyecto_API.Services
{
    public interface IUtil
    {
        Respuesta RespuestaExitosa(object? contenido);
        Respuesta RespuestaFallida(string mensaje);
        string GenerarContrasena(int longitud);
        string Encriptar(string texto);
        string GenerarToken(int IdUsuario);

        string Desencriptar(string texto);
    }
}
