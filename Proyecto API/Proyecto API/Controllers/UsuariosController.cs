using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
namespace Proyecto_API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {

        private readonly IConfiguration _configuration;

        public UsuariosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("consultar_roles")]
        public ActionResult consultar_roles()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultados = connection.Query<Roles>("sp_consultar_roles").ToList();
                if (resultados != null)
                {
                    var roles = new UsuarioViewModel
                    {
                        Usuario = new Usuario(),
                        Roles = resultados
                    };
                    return Ok(roles);
                }
                else {
                    return NotFound("No roles found.");
                }
            }
        }

    }
}
