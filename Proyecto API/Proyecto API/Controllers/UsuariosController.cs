using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;
using System.Data;
namespace Proyecto_API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IUtilitarios _utilitarios;

        public UsuariosController(IConfiguration configuration, IUtilitarios utilitarios)
        {
            _configuration = configuration;
            _utilitarios = utilitarios;
        }

        [HttpGet]
        [Route("consultar_roles")]
        public ActionResult consultar_roles()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                var resultados = connection.Query<Roles>("sp_consultar_roles").ToList();
                if (resultados != null)
                {
                    var roles = new UsuarioViewModel
                    {
                        Usuario = new Usuario(),
                        Roles = resultados
                    };
                    return Ok(roles);
                }
                else {
                    return NotFound("No roles found.");
                }
            }
        }

        [HttpPost]
        [Route("crear_usuario")]
        public ActionResult CrearUsuario([FromBody] Usuario usuario)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("Connection")))
            {
                // Verificar si ya existe un usuario con ese correo
                var existe = connection.ExecuteScalar<int>(
                    "SELECT COUNT(1) FROM Usuarios WHERE Correo = @Correo",
                    new { usuario.Correo }
                );

                if (existe > 0)
                {
                    return Conflict(new { message = "Ya existe un usuario con este correo." });
                    // 409 Conflict es el status más correcto
                }

                // Si no existe, procedemos a crearlo
                var parametros = new
                {
                    usuario.Nombre_Completo,
                    usuario.Correo,
                    usuario.Contrasenna,
                    usuario.Estado,
                    usuario.Token,
                    usuario.Id_Rol
                };

                var idUsuario = connection.ExecuteScalar<int>(
                    "sp_crear_usuario",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                if (idUsuario > 0)
                {
                    return Ok(new { message = "Usuario creado correctamente" });
                }
                else
                {
                    return NotFound("No se pudo crear el usuario.");
                }
            }
        }



        [HttpPost]
        [Route("login")]
        public IActionResult login(Usuario Usuario)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Usuario>("sp_login",
                    new
                    {
                        Usuario.Correo,
                        Usuario.Contrasenna
                    });

                if (resultado != null)
                {
                    resultado.Token = _utilitarios.GenerarToken(resultado.IdUsuario);
                    return Ok(_utilitarios.RespuestaCorrecta(resultado));
                }
                else
                    return BadRequest(_utilitarios.RespuestaIncorrecta("Su información no fue validada"));
            }
        }









    }
}
