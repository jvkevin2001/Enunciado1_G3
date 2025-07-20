using System.Security.Claims;

namespace ProyectoFinal_G3.Services
{
    public interface IEncriptacionService
    {
        string Encrypt(string texto);

        long ObtenerIdUsuario(IEnumerable<Claim> token);
    }
}
