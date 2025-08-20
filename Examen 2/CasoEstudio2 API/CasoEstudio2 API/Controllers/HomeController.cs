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

    }
}
