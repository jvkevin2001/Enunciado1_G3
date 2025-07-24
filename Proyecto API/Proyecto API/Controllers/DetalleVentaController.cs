using System.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleVentaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public DetalleVentaController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

        [HttpGet("ObtenerDetalleVenta")]
        public IActionResult ObtenerDetalleVenta(int Id_DetalleVenta)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.QueryFirstOrDefault<DetalleVenta>("p_GetDetalleVentaById", new { Id_DetalleVenta });

            if (resultado != null)
                return Ok(_util.RespuestaExitosa(resultado));
            else
                return BadRequest(_util.RespuestaFallida("Detalle de venta no encontrado"));
        }

        [HttpGet("ObtenerTotalDetalleVenta")]
        public IActionResult ObtenerTotalDetalleVenta()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var total = connection.QueryFirstOrDefault<decimal>(
                "[dbo].[p_GetTotalDetalleVenta]",
                commandType: CommandType.StoredProcedure);

            return Ok(_util.RespuestaExitosa(total));
        }

        [HttpPost("Insertar")]
        public IActionResult Insertar(DetalleVenta detalle)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute("p_InsertDetalleVenta", new
            {
                detalle.Id_Venta,
                detalle.Id_Inventario,
                detalle.Cantidad,
                detalle.PrecioTotal
            });

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo insertar el detalle de venta"));
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar(DetalleVenta detalle)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute("p_UpdateDetalleVenta", new
            {
                detalle.Id_DetalleVenta,
                detalle.Id_Venta,
                detalle.Id_Inventario,
                detalle.Cantidad,
                detalle.PrecioTotal
            });

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo actualizar el detalle de venta"));
        }

        [HttpDelete("Eliminar")]
        public IActionResult Eliminar(int Id_DetalleVenta)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute("p_DeleteDetalleVenta", new { Id_DetalleVenta });

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo eliminar el detalle de venta"));
        }

    }
}
