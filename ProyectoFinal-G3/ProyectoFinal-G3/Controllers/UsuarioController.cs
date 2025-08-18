using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using ProyectoFinal_G3.Services;
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
                    return Ok(new { message = "Usuario registrado correctamente" });
                }
                else
                {
                    var respuesta = resultado.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    return StatusCode((int)resultado.StatusCode, new { message = respuesta?.Mensaje });
                }
            }
        }






    }
}
