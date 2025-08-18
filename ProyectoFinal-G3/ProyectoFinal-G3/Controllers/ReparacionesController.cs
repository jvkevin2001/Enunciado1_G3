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
                data.CostoServicio = 5000;
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.PostAsJsonAsync("api/Reparaciones/AgregarReparacion", data).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("CrearReparacion");
                }
                else
                {
                    var respuesta = response.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    TempData["Error"] = respuesta?.Mensaje;
                    return CrearReparacion();
                }
            }
        }

        [HttpGet]
        public IActionResult Reparaciones()
        {
            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.GetAsync("api/Reparaciones/ObtenerReparacionesActivas").Result;

                if (response.IsSuccessStatusCode)
                {
                    var datos = response.Content.ReadFromJsonAsync<RespuestaEstandar<List<Reparacion>>>().Result;
                    return View(datos?.Contenido);
                }
                else
                {
                    var respuesta = response.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    TempData["Error"] = respuesta.Mensaje;
                    return View();
                }
                    
            }
            
        }

        [HttpGet]
        public IActionResult EditarReparacion(int id)
        {
            using (var http = _http.CreateClient())
            {
                http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);
                var response = http.GetAsync("api/Reparaciones/ObtenerReparacion?Id_Reparacion=" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    var solicitud = http.GetAsync("api/Inventario/p_GetInventario").Result;
                    if(solicitud.IsSuccessStatusCode)
                    {
                        var result = solicitud.Content.ReadFromJsonAsync<RespuestaEstandar<List<Inventario>>>().Result;
                        ViewBag.Inventario = result?.Contenido;
                    }
                    else
                    {
                        ViewBag.Inventario = new List<Inventario>();
                    }
                    var datos = response.Content.ReadFromJsonAsync<RespuestaEstandar<Reparacion>>().Result;
                    return View(datos?.Contenido);
                }
                else
                {
                    var respuesta = response.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                    TempData["Error"] = respuesta?.Mensaje;
                    return View();
                }
            }
        }
    }
}
