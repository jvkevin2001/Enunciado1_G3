using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using ProyectoFinal_G3.Services;
using System;
using System.Reflection;
using System.Text.Json;

namespace ProyectoFinal_G3.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IEncriptacionService _encriptacionService;
        private readonly IHttpClientFactory _http;
        public UsuarioController(IConfiguration configuration, IEncriptacionService encriptacionService, IHttpClientFactory http)
        {
            _configuration = configuration;
            _encriptacionService = encriptacionService;
            _http = http;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.GetAsync("api/Usuarios/consultar_roles").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;

                    var viewModel = JsonSerializer.Deserialize<UsuarioViewModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return View(viewModel ?? new UsuarioViewModel
                    {
                        Usuario = new Usuario(),
                        Roles = new List<Roles>()
                    });
                }

                ViewBag.Error = "Error al obtener los roles.";
                return View(new UsuarioViewModel()); // Modelo vacío si falla
            }
        }



        [HttpPost]
        public IActionResult Registrarse([FromBody] Usuario usuario)
        {
            usuario.Contrasenna = _encriptacionService.Encrypt(usuario.Contrasenna!);

            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var resultado = http.PostAsJsonAsync("api/Usuarios/crear_usuario", usuario).Result;

                if (resultado.IsSuccessStatusCode)
                {
                    return RedirectToAction("Usuarios", "Reportes");
                }
                else
                {
                    var respuesta = resultado.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    return StatusCode((int)resultado.StatusCode, new { message = respuesta?.Mensaje });
                }
            }
        }

        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {

            var contra = usuario.Contrasenna;

            usuario.Contrasenna = _encriptacionService.Encrypt(usuario.Contrasenna!);

            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var resultado = http.PostAsJsonAsync("api/Usuarios/login", usuario).Result;

                if (resultado.IsSuccessStatusCode)
                {
                    var datos = resultado.Content.ReadFromJsonAsync<RespuestaEstandar<Usuario>>().Result;

                    HttpContext.Session.SetString("IdUsuario", datos?.Contenido?.IdUsuario.ToString()!);
                    HttpContext.Session.SetString("Nombre", datos?.Contenido?.Nombre_Completo!);
                    HttpContext.Session.SetString("IdRol", datos?.Contenido?.Id_Rol.ToString()!);
                    HttpContext.Session.SetString("Correo", datos?.Contenido?.Correo!);
                    HttpContext.Session.SetString("JWT", datos?.Contenido?.Token ?? "");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var respuesta = resultado.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    ViewBag.Mensaje = respuesta?.Mensaje;
                    return View();
                }
            }
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Usuario");
        }

        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarAcceso(Usuario Usuario)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            // Envías solo el correo
            var resultado = await http.PostAsJsonAsync("api/Usuarios/RecuperarAcceso", new { Usuario.Correo });

            if (resultado.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "Se ha enviado un correo de recuperación.";
                return RedirectToAction("Login", "Usuario");
            }
            else
            {
                // Evitamos leer JSON si no hay
                ViewBag.Mensaje = "Ocurrió un error al enviar el correo.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult CambiarContrasenna()
        {
            return View();
        }

        [HttpPost]
        [Route("Usuarios/CambiarContrasennaAjax")]
        public async Task<IActionResult> CambiarContrasennaAjax([FromBody] CambiarContrasennaViewModel model)
        {
            try
            {
                using var http = _http.CreateClient();
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

                var resultado = await http.PostAsJsonAsync("api/Usuarios/CambiarContrasenna", new
                {
                    Correo = HttpContext.Session.GetString("Correo"),
                    ContrasennaActual = model.ContrasennaActual,
                    ContrasennaNueva = model.ContrasennaNueva
                });

                var respuesta = await resultado.Content.ReadFromJsonAsync<JsonElement>();
                string mensaje = respuesta.TryGetProperty("Mensaje", out var prop) ? prop.GetString()! : "Operación completada";

                if (resultado.IsSuccessStatusCode)
                {
                    return Json(new { success = true, mensaje });
                }
                else
                {
                    return Json(new { success = false, mensaje });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error en el servidor: " + ex.Message });
            }
        }




    }
}
