using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;
using Dapper;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialVentasController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public HistorialVentasController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

        [HttpGet("ObtenerHistorialVentas")]
        public IActionResult ObtenerHistorialVentas()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.Query<HistorialVentas>("p_GetHistorialVentas", commandType: CommandType.StoredProcedure);
            return Ok(_util.RespuestaExitosa(resultado));
        }

        [HttpGet("ObtenerHistorialVenta")]
        public IActionResult ObtenerHistorialVenta(int Id_Venta)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.QueryFirstOrDefault<HistorialVentas>(
                "p_GetHistorialVentaById",
                new { Id_Venta },
                commandType: CommandType.StoredProcedure);

            return resultado != null
                ? Ok(_util.RespuestaExitosa(resultado))
                : BadRequest(_util.RespuestaFallida("Historial de venta no encontrado"));
        }

        [HttpPost("Insertar")]
        public IActionResult Insertar(HistorialVentas historial)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute(
                "p_InsertHistorialVenta",
                new
                {
                    historial.Id_Venta,
                    historial.Id_Cliente,
                    historial.Id_Usuario,
                    historial.FechaVenta
                },
                commandType: CommandType.StoredProcedure);

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo insertar el historial de venta"));
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar(HistorialVentas historial)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute(
                "p_UpdateHistorialVenta",
                new
                {
                    historial.Id_Venta,
                    historial.Id_Cliente,
                    historial.Id_Usuario,
                    historial.FechaVenta
                },
                commandType: CommandType.StoredProcedure);

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo actualizar el historial de venta"));
        }

        [HttpDelete("Eliminar")]
        public IActionResult Eliminar(int Id_Venta)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.Execute(
                "p_DeleteHistorialVenta",
                new { Id_Venta },
                commandType: CommandType.StoredProcedure);

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo eliminar el historial de venta"));
        }
    }
}
