using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Proyecto_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportesController(IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;

    [HttpGet("ventas")]
    public IActionResult ObtenerVentas([FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        var resultados = new List<object>();
        string? connectionStringRaw = _configuration.GetConnectionString("Connection");
        string connectionString = connectionStringRaw ?? throw new InvalidOperationException("Cadena de conexión no encontrada.");

        using SqlConnection connection = new(connectionString);
        string query = @"
            SELECT 
                FORMAT(F.FechaEmision, 'yyyy-MM-dd') AS Fecha,
                SUM(DV.PrecioTotal) AS TotalVentas
            FROM Factura F
            JOIN DetalleVenta DV ON F.Id_Venta = DV.Id_Venta
            WHERE F.FechaEmision BETWEEN @FechaInicio AND @FechaFin
            GROUP BY FORMAT(F.FechaEmision, 'yyyy-MM-dd')
            ORDER BY Fecha
        ";

        using SqlCommand command = new(query, connection);
        command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
        command.Parameters.AddWithValue("@FechaFin", fechaFin);

        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            resultados.Add(new
            {
                Fecha = reader["Fecha"]?.ToString(),
                TotalVentas = reader["TotalVentas"] != DBNull.Value ? Convert.ToDecimal(reader["TotalVentas"]) : 0
            });
        }

        return Ok(resultados);
    }
}
