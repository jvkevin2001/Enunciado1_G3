using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public VentasController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

        [HttpPost]
        [Route("CrearVenta")]
        public IActionResult CrearVenta([FromBody] VentaCompleta data)
        {
            var venta = data.Venta;
            var detalles = data.Detalles ?? new List<DetalleVenta>();

            if (venta == null)
                return BadRequest(_util.RespuestaFallida("La venta no puede ser nula"));

            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                context.Open();
                using var transaction = context.BeginTransaction();
                try
                {
                    var parametersVenta = new Dapper.DynamicParameters();
                    parametersVenta.Add("@Id_Cliente", venta.Id_Cliente);
                    parametersVenta.Add("@Total", venta.Total);
                    parametersVenta.Add("@Id_VentaSalida", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    context.Execute("p_InsertVenta", parametersVenta, transaction, commandType: System.Data.CommandType.StoredProcedure);

                    int idVenta = parametersVenta.Get<int>("@Id_VentaSalida");

                    foreach (var d in detalles)
                    {
                        context.Execute("p_InsertDetalleVenta",
                            new
                            {
                                Id_Venta = idVenta,
                                d.Id_Inventario,
                                d.Cantidad,
                                d.PrecioTotal
                            },
                            transaction, commandType: System.Data.CommandType.StoredProcedure);
                    }

                    var parametersFactura = new Dapper.DynamicParameters();
                    parametersFactura.Add("@Id_Venta", idVenta);
                    parametersFactura.Add("@Total", venta.Total);
                    parametersFactura.Add("@Id_FacturaSalida", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                    context.Execute("p_InsertFacturaVenta", parametersFactura, transaction, commandType: System.Data.CommandType.StoredProcedure);

                    int idFactura = parametersFactura.Get<int>("@Id_FacturaSalida");

                    transaction.Commit();

                    return Ok(_util.RespuestaExitosa(new
                    {
                        VentaId = idVenta,
                        FacturaId = idFactura
                    }));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(_util.RespuestaFallida(ex.Message));
                }
            }
        }

        [HttpGet]
        [Route("ObtenerVentas")]
        public IActionResult ObtenerVentas()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Query<Venta>("p_GetVentas", commandType: System.Data.CommandType.StoredProcedure);
                if (result.Any())
                    return Ok(_util.RespuestaExitosa(result));
                else
                    return BadRequest(_util.RespuestaFallida("No se encontraron ventas"));
            }
        }

        [HttpGet]
        [Route("ObtenerVenta")]
        public IActionResult ObtenerVenta(int Id_Venta)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.QueryFirstOrDefault<Venta>("p_GetVenta", new { Id_Venta }, commandType: System.Data.CommandType.StoredProcedure);
                if (result != null)
                    return Ok(_util.RespuestaExitosa(result));
                else
                    return BadRequest(_util.RespuestaFallida($"No se encontró la venta con Id {Id_Venta}"));
            }
        }

        [HttpGet]
        [Route("ObtenerVentaCompleta/{id}")]
        public IActionResult ObtenerVentaCompleta(int id)
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var venta = context.QueryFirstOrDefault<Venta>(
                @"SELECT v.Id_Venta, v.Id_Cliente, v.Total, v.FechaVenta,
                  c.Id_Clientes, c.NombreCliente
                  FROM Ventas v
                  INNER JOIN Clientes c ON v.Id_Cliente = c.Id_Clientes
                  WHERE v.Id_Venta = @Id_Venta",
                new { Id_Venta = id });

            if (venta == null)
                return NotFound(_util.RespuestaFallida("Venta no encontrada"));

            var detalles = context.Query<DetalleVenta>(
                @"SELECT dv.Id_Venta, dv.Id_Inventario, dv.Cantidad, dv.PrecioTotal,
                  i.ProductoNombre, i.PrecioUnitario
                  FROM DetalleVenta dv
                  INNER JOIN Inventario i ON dv.Id_Inventario = i.Id_Inventario
                  WHERE dv.Id_Venta = @Id_Venta",
                new { Id_Venta = id }).ToList();

            foreach (var d in detalles)
            {
                d.Producto = new Inventario
                {
                    Id_Inventario = d.Id_Inventario,
                    ProductoNombre = d.ProductoNombre,
                    PrecioUnitario = d.PrecioUnitario
                };
            }

            var ventaCompleta = new VentaCompleta
            {
                Venta = venta,
                Detalles = detalles
            };

            return Ok(_util.RespuestaExitosa(ventaCompleta));
        }

        [HttpGet]
        [Route("ObtenerFacturas")]
        public IActionResult ObtenerFacturas()
        {
            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var facturas = context.Query<FacturaVenta>(
                "sp_GetFacturas",
                commandType: System.Data.CommandType.StoredProcedure
            ).ToList();

            return Ok(_util.RespuestaExitosa(facturas));
        }
    }
}
