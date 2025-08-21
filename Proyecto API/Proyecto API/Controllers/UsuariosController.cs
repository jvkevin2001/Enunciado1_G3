using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Proyecto_API.Models;
using Proyecto_API.Services;
using System.Data;
using System.Text;
namespace Proyecto_API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        private readonly IUtilitarios _utilitarios;

        public UsuariosController(IConfiguration configuration, IHostEnvironment environment, IUtilitarios utilitarios)
        {
            _configuration = configuration;
            _environment = environment;
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

        [HttpPost]
        [Route("RecuperarAcceso")]
        public IActionResult RecuperarAcceso(Usuario Usuario)
        {
            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                var resultado = context.QueryFirstOrDefault<Usuario>("ValidarCorreo",
                    new { Usuario.Correo });

                if (resultado != null)
                {
                    var ContrasennaNotificar = _utilitarios.GenerarContrasenna(10);
                    var Contrasenna = _utilitarios.Encrypt(ContrasennaNotificar);

                    var resultadoActualizacion = context.Execute("ActualizarContrasenna",
                        new
                        {
                            Id_Usuario = resultado.IdUsuario,  
                            Contrasena = Contrasenna           
                        });


                    if (resultadoActualizacion > 0)
                    {
                        var ruta = Path.Combine(_environment.ContentRootPath, "Correos.html");
                        var html = System.IO.File.ReadAllText(ruta, UTF8Encoding.UTF8);

                        html = html.Replace("@@Usuario", resultado.Nombre_Completo);
                        html = html.Replace("@@Contrasenna", ContrasennaNotificar);

                        _utilitarios.EnviarCorreo(resultado.Correo!, "Recuperación de Acceso", html);
                        return Ok(_utilitarios.RespuestaCorrecta(null));
                    }
                }

                return BadRequest(_utilitarios.RespuestaIncorrecta("Su información no fue validada"));
            }
        }

        [HttpPost]
        [Route("CambiarContrasenna")]
        public IActionResult CambiarContrasennaAjax([FromBody] CambiarContrasennaViewModel model)
        {


            var actualEncriptada = _utilitarios.Encrypt(model.ContrasennaActual);
            var nuevaEncriptada = _utilitarios.Encrypt(model.ContrasennaNueva);

            using var context = new SqlConnection(_configuration.GetConnectionString("Connection"));

            var resultado = context.QueryFirstOrDefault<int>(
                "CambiarContrasenna",
                new
                {
                    Correo = model.Correo,
                    ContrasennaActual = actualEncriptada,
                    ContrasennaNueva = nuevaEncriptada
                },
                commandType: CommandType.StoredProcedure
            );

            if (resultado == 1)
                return Ok(new { mensaje = "Contraseña actualizada correctamente" });
            else
                return BadRequest(new { mensaje = "Contraseña actual incorrecta" });
        }







    }
}
