using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Proyecto_API.Models;

namespace Proyecto_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReporteController : ControllerBase
{
    private readonly string _cs;

    public ReporteController(IConfiguration configuration)
    {
        _cs = configuration.GetConnectionString("Connection")
              ?? throw new InvalidOperationException("Cadena de conexión no encontrada.");
    }

    [HttpGet("ventas")]
    public async Task<IActionResult> Ventas(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] int? idCliente,
        [FromQuery] int? idUsuario)
    {
        var fi = (fechaInicio ?? DateTime.UtcNow.AddDays(-30)).Date;
        var ff = (fechaFin ?? DateTime.UtcNow).Date;

        using var cn = new SqlConnection(_cs);
        var data = await cn.QueryAsync<ReporteHistorialVentaDTO>(
            "dbo.p_GetVentasPorRango",
            new { FechaInicio = fi, FechaFin = ff, IdCliente = idCliente, IdUsuario = idUsuario },
            commandType: CommandType.StoredProcedure);

        return Ok(data);
    }
}
