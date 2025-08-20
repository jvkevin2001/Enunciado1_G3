using System.Data;
using System.Globalization;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public ReportesController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

        [HttpGet]
        [Route("Clientes")]
        public IActionResult ReporteClientes()
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var result = context.Query<Cliente>(
                "sp_ReportesClientes",
                commandType: CommandType.StoredProcedure
            );

            if (result.Any())
                return Ok(_util.RespuestaExitosa(result));
            else
                return BadRequest(_util.RespuestaFallida("No se encontraron clientes."));
        }

        [HttpGet]
        [Route("Usuarios")]
        public IActionResult ReporteUsuarios()
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var result = context.Query<Usuario>(
                "sp_ReportesUsuarios",
                commandType: CommandType.StoredProcedure
            );

            if (result.Any())
                return Ok(_util.RespuestaExitosa(result));
            else
                return BadRequest(_util.RespuestaFallida("No se encontraron usuarios."));
        }

        [HttpGet]
        [Route("Inventario")]
        public IActionResult ReporteInventario()
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));
            var result = context.Query<Inventario>("sp_ReportesInventario");

            if (result.Any())
                return Ok(_util.RespuestaExitosa(result));
            else
                return BadRequest(_util.RespuestaFallida("No se encontraron registros de inventario."));
        }

        [HttpGet("Reparaciones")]
        public IActionResult ReporteReparaciones(string? fechaInicio, string? fechaFin)
        {
            DateTime? inicio = null;
            DateTime? fin = null;

            var formatos = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };

            if (!string.IsNullOrEmpty(fechaInicio))
                inicio = DateTime.ParseExact(fechaInicio, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None);

            if (!string.IsNullOrEmpty(fechaFin))
                fin = DateTime.ParseExact(fechaFin, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None);

            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var result = context.Query<Reparacion>(
                "sp_ReportesReparaciones",
                new { FechaInicio = inicio, FechaFin = fin },
                commandType: CommandType.StoredProcedure
            );

            if (result.Any())
                return Ok(_util.RespuestaExitosa(result));
            else
                return BadRequest(_util.RespuestaFallida("No se encontraron reparaciones."));
        }


        [HttpGet("HistorialVentas")]
        public IActionResult ReporteHistorialVentas(string? fechaInicio, string? fechaFin)
        {
            DateTime? inicio = null;
            DateTime? fin = null;

            // Soportar ambos formatos: dd/MM/yyyy y yyyy-MM-dd
            var formatos = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };

            if (!string.IsNullOrEmpty(fechaInicio))
                inicio = DateTime.ParseExact(fechaInicio, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None);

            if (!string.IsNullOrEmpty(fechaFin))
                fin = DateTime.ParseExact(fechaFin, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None);

            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var result = context.Query<HistorialVentaReporte>(
                "sp_ReportesHistorialVentas",
                new { FechaInicio = inicio, FechaFin = fin },
                commandType: CommandType.StoredProcedure
            );

            if (result.Any())
                return Ok(_util.RespuestaExitosa(result));
            else
                return BadRequest(_util.RespuestaFallida("No se encontraron ventas en el rango de fechas."));
        }

    }
}
