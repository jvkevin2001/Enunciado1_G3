using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;

namespace Proyecto_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReparacionesController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IUtil _util;

        public ReparacionesController(IConfiguration configuration, IUtil util)
        {
            _configuration = configuration;
            _util = util;
        }

        [HttpPost]
        [Route("AgregarReparacion")]
        public IActionResult AgregarReparacion(Reparacion data)
        {
            data.FechaServicio = DateTime.Now;
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Execute("RegistrarReparacion",
                    new
                    {
                        data.Id_Clientes,
                        data.EquipoDescripcion,
                        data.TipoMaquina,
                        data.FechaServicio,
                        data.CostoServicio
                    });

                if(result > 0)
                    return Ok(_util.RespuestaExitosa(null));
                else
                    return BadRequest(_util.RespuestaFallida("No se pudo agregar la reparacion correctamente"));
            }
        }

        [HttpPut]
        [Route("AgregarProducto")]
        public IActionResult AgregarProducto(Reparacion data)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Execute("p_AddProductoReparacion",
                    new
                    {
                        data.Id_Reparacion,
                        data.Id_Inventario
                    });

                if (result > 0)
                    return Ok(_util.RespuestaExitosa(null));
                else
                    return BadRequest(_util.RespuestaFallida("Hubo un problema al actualizar la reparacion"));
            }
        }

        [HttpPut]
        [Route("ActualizarEstado")]
        public IActionResult ActualizarEstado(Reparacion data)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Execute("p_UpdateEstadoReparacion",
                    new
                    {
                        data.Id_Reparacion,
                        data.Estado
                    });

                if (result > 0)
                    return Ok(_util.RespuestaExitosa(null));
                else
                    return BadRequest(_util.RespuestaFallida("Hubo un problema al actualizar el estado de la reparacion"));
            }
        }

        [HttpPut]
        [Route("AgregarCostos")]
        public IActionResult AgregarCostos(Reparacion data)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Execute("p_UpdateCostosReparacion",
                    new
                    {
                        data.Id_Reparacion,
                        data.CostoServicio
                    });

                if (result > 0)
                    return Ok(_util.RespuestaExitosa(null));
                else
                    return BadRequest(_util.RespuestaFallida("Hubo un problema al actualizar los costos de la reparacion"));
            }
        }

        [HttpGet]
        [Route("ObtenerReparaciones")]
        public IActionResult ObtenerReparaciones()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Query<Reparacion>("p_GetReparaciones");
                if (result.Any())
                    return Ok(_util.RespuestaExitosa(result));
                else
                    return BadRequest(_util.RespuestaFallida("No se encuentra ninguna reparacion"));
            }
        }

    }
}
