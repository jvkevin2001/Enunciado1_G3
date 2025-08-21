using CasoEstudio2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;
using System.Text.Json;


namespace CasoEstudio2.Controllers
{
    public class CasasController : Controller
    {



        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _http;

        public CasasController(IConfiguration configuration, IHttpClientFactory http)
        {
            _configuration = configuration;
            _http = http;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Consultar_Casas()
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration["Start:ApiUrl"]);

            var response = http.GetAsync("api/Home/Consultar_Casas").Result;

            if (!response.IsSuccessStatusCode)
            {
                // Guardamos un mensaje de error para la vista
                ViewBag.Error = "Error al obtener la lista de casas desde la API.";
                return View(new List<CasasModel>()); // igual devolvemos lista vacía para no romper la vista
            }

            var json = response.Content.ReadAsStringAsync().Result;

            
            var viewModel = JsonSerializer.Deserialize<List<CasasModel>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CasasModel>();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Alquilar()
        {
            TempData.Remove("Mensaje");
            TempData.Remove("Error");

            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration["Start:ApiUrl"]);

            var response = http.GetAsync("api/Home/CasasDisponibles").Result;
            if (response.IsSuccessStatusCode)
            {
                var casas = response.Content.ReadFromJsonAsync<List<CasasModel>>().Result;
                ViewBag.CasasDisponibles = casas;
            }
            else
            {
                ViewBag.CasasDisponibles = new List<CasasModel>();
            }

            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Alquilar(CasasModel model)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration["Start:ApiUrl"]);

            var response = await http.PostAsJsonAsync("api/Home/Alquilar_Casa", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "Casa alquilada con éxito";
                return RedirectToAction("Consultar_Casas", "Casas");
            }
            else
            {
                TempData["Error"] = "No se pudo alquilar la casa";
                return RedirectToAction("Alquilar", "Casas");
            }
        }

    }
}
