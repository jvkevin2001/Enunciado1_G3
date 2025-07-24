using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;
using Dapper;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public FacturaController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

         [HttpGet("ObtenerFacturas")]
        public IActionResult ObtenerFacturas()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.Query<Factura>("p_GetFacturas", commandType: System.Data.CommandType.StoredProcedure);
            return Ok(_util.RespuestaExitosa(resultado));
        }


        [HttpGet("ObtenerFactura")]
        public IActionResult ObtenerFactura(int Id_Factura)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.QueryFirstOrDefault<Factura>("p_GetFacturaById", new { Id_Factura });

            return resultado != null
                ? Ok(_util.RespuestaExitosa(resultado))
                : BadRequest(_util.RespuestaFallida("Factura no encontrada"));
        }

        [HttpPost("Insertar")]
        public IActionResult Insertar(Factura factura)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute("p_InsertFactura", new
            {
                factura.Id_TipoFactura,
                factura.Id_Venta,
                factura.Id_Reparacion,
                factura.FechaEmision,
                factura.Total
            });

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo insertar la factura"));
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar(Factura factura)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = connection.Execute("p_UpdateFactura", new
            {
                factura.Id_Factura,
                factura.Id_TipoFactura,
                factura.Id_Venta,
                factura.Id_Reparacion,
                factura.FechaEmision,
                factura.Total
            });

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo actualizar la factura"));
        }


        [HttpDelete("Eliminar")]
        public IActionResult Eliminar(int Id_Factura)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var resultado = connection.Execute("p_DeleteFactura", new { Id_Factura }, commandType: System.Data.CommandType.StoredProcedure);

            return resultado > 0
                ? Ok(_util.RespuestaExitosa(null))
                : BadRequest(_util.RespuestaFallida("No se pudo eliminar la factura"));
        }
    }
}
