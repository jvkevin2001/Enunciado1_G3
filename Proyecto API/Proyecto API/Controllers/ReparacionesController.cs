using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                if (result > 0)
                    return Ok(_util.RespuestaExitosa(null));
                else
                    return BadRequest(_util.RespuestaFallida("No se pudo agregar la reparacion correctamente"));
            }
        }

        [HttpPut]
        [Route("FinalizarReparacion")]
        public IActionResult FinalizarReparacion(Reparacion data)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                data.Estado = "Finalizada";
                data.FechaSalida = DateTime.Now;
                var result = context.Execute("p_UpdateReparacion",
                    new
                    {
                        data.Id_Reparacion,
                        data.Estado,
                        data.EquipoDescripcion,
                        data.TipoMaquina,
                        data.CostoServicio,
                        data.Id_Inventario,
                        data.FechaSalida
                    });

                if (result > 0)
                    return Ok(_util.RespuestaExitosa(null));
                else
                    return BadRequest(_util.RespuestaFallida("Hubo un problema al finalizar la reparacion"));
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

        [HttpGet]
        [Route("ObtenerReparacionesActivas")]
        public IActionResult ObtenerReparacionesActivas()
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.Query<Reparacion>("p_GetReparacionesActivas");
                if (result.Any())
                    return Ok(_util.RespuestaExitosa(result));
                else
                    return BadRequest(_util.RespuestaFallida("No se encuentra ninguna reparacion"));
            }
        }

        [HttpGet]
        [Route("ObtenerReparacion")]
        public IActionResult ObtenerReparacion(int Id_Reparacion)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var result = context.QueryFirstOrDefault<Reparacion>("GetReparacion", new
                {
                    Id_Reparacion
                });

                if (result != null)
                {
                    return Ok(_util.RespuestaExitosa(result));
                }

                else
                    return BadRequest(_util.RespuestaFallida("No se encuentra ninguna con el id " + Id_Reparacion));
            }
        }

        [HttpPut]
        [Route("ActualizarReparacion")]
        public IActionResult ActualizarReparacion(Reparacion reparacion)
        {
            using (var context = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                reparacion.FechaSalida = null;
                var result = context.Execute("p_UpdateReparacion", new
                {
                    reparacion.Id_Reparacion,
                    reparacion.Estado,
                    reparacion.EquipoDescripcion,
                    reparacion.TipoMaquina,
                    reparacion.CostoServicio,
                    reparacion.Id_Inventario,
                    reparacion.FechaSalida
                });

                if (result > 0)
                {
                    return Ok(_util.RespuestaExitosa(result));
                }

                else
                    return BadRequest(_util.RespuestaFallida("No se pudo actualizar la reparacion numero " + reparacion.Id_Reparacion));
            }
        }

    }
}
