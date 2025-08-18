using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public ClientesController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

        [HttpGet]
        [Route("ObtenerClientes")]
        public IActionResult ObtenerClientes()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Query<Cliente>("p_GetClientes");
                if (result.Any())
                    return Ok(_util.RespuestaExitosa(result));
                else
                    return BadRequest(_util.RespuestaFallida("No se encuentrar clientes registrados"));
            }
        }
    }
}
