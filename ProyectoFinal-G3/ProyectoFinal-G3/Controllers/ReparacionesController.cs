using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using ProyectoFinal_G3.Services;
using System.Configuration;
using static System.Net.WebRequestMethods;

namespace ProyectoFinal_G3.Controllers
{
    public class ReparacionesController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _configuration;

        public ReparacionesController(IConfiguration configuration, IHttpClientFactory http)
        {
            _configuration = configuration;
            _http = http;
        }

        [HttpGet]
        public IActionResult CrearReparacion()
        {
            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.GetAsync("api/Clientes/ObtenerClientes").Result;
                var result = response.Content.ReadFromJsonAsync<RespuestaEstandar<List<Cliente>>>().Result;
                ViewBag.Clientes = result?.Contenido;
            }
                return View();
        }

        [HttpPost]
        public IActionResult CrearReparacion(Reparacion data)
        {
            using (var http = _http.CreateClient())
            {
                data.FechaServicio = DateTime.Now;
                data.CostoServicio = 5000;

                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.PostAsJsonAsync("api/Reparaciones/AgregarReparacion", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    return CrearReparacion();
                }
                else
                {
                    var respuesta = response.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    ViewBag.Mensaje = respuesta?.Mensaje;
                    return CrearReparacion();
                }
            }
        }
    }
}
