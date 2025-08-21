using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using CasoEstudio2_API.Models;
using System.Data;

namespace CasoEstudio2_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        [HttpGet]
        [Route("Consultar_Casas")]
        public IActionResult get_compras()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var compras = connection.Query<CasasModel>("sp_ConsultarCasas").ToList();

                return Ok(compras);
            }
        }
        [HttpPost]
        [Route("Alquilar_Casa")]
        public IActionResult AlquilarCasa([FromBody] CasasModel request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var parametros = new
                {
                    IdCasa = request.IdCasa,
                    UsuarioAlquiler = request.UsuarioAlquiler,
                    FechaAlquiler = DateTime.Now
                };

                var filas = connection.Execute("sp_AlquilarCasa", parametros, commandType: CommandType.StoredProcedure);

                if (filas > 0)
                    return Ok(new { mensaje = "Casa alquilada con éxito" });
                else
                    return BadRequest(new { mensaje = "No se pudo alquilar la casa" });
            }
        }

        [HttpGet]
        [Route("CasasDisponibles")]
        public IActionResult CasasDisponibles()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var casas = connection.Query<CasasModel>("sp_CasasDisponibles", commandType: CommandType.StoredProcedure).ToList();
            return Ok(casas);
        }

    }
}
