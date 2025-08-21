using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;

namespace ProyectoFinal_G3.Controllers
{
    public class ClientesController : Controller
    {

        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _configuration;

        public ClientesController(IHttpClientFactory http, IConfiguration configuration)
        {
            _http = http;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Principal()
        {
            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.GetAsync("api/Clientes/ObtenerClientes").Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<RespuestaEstandar<List<Cliente>>>().Result;
                    return View(result?.Contenido);
                }
                else
                {
                    TempData["Error"] = "No se pudieron obtener los clientes.";
                    return View(new List<Cliente>());
                }
            }     
        }

        [HttpGet]
        public IActionResult CrearCliente()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearCliente(Cliente data)
        {
            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.PostAsJsonAsync("api/Clientes/RegistrarClientes",data).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Principal","Clientes");
                }
                else
                {
                    TempData["Error"] = "No se pudo registrar el cliente.";
                    return View(new List<Cliente>());
                }
            }
        }
    }
}

