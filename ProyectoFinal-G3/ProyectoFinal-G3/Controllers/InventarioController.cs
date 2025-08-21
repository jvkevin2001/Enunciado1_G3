using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using System.Text;
using System.Text.Json;

namespace ProyectoFinal_G3.Controllers
{
    public class InventarioController : Controller
    {
        private readonly HttpClient _httpClient;

        public InventarioController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            string apiBaseUrl = configuration.GetValue<string>("Start:ApiUrl");
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Inventario/Listar");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var respuesta = JsonSerializer.Deserialize<RespuestaEstandar<List<Inventario>>>(content, options);
                var productos = respuesta?.Contenido ?? new List<Inventario>();
                return View(productos);
            }
            return View(new List<Inventario>());
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventario inventario)
        {
            if (!ModelState.IsValid) return View(inventario);

            var idUsuarioString = HttpContext.Session.GetString("IdUsuario");
            if (string.IsNullOrEmpty(idUsuarioString)) return RedirectToAction("Login", "Usuario");

            inventario.Id_Usuario = int.Parse(idUsuarioString);
            inventario.FechaRegistro = DateTime.Now;

            var jsonContent = new StringContent(JsonSerializer.Serialize(inventario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Inventario/Crear", jsonContent);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Error desde la API al crear el producto.");
            return View(inventario);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Inventario/Obtener/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var producto = await response.Content.ReadFromJsonAsync<Inventario>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventario inventario)
        {
            if (id != inventario.Id_Inventario) return BadRequest();
            if (!ModelState.IsValid) return View(inventario);

            var jsonContent = new StringContent(JsonSerializer.Serialize(inventario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/Inventario/Actualizar/{id}", jsonContent);

            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "Error desde la API al actualizar el producto.");
            return View(inventario);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"api/Inventario/Obtener/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var producto = await response.Content.ReadFromJsonAsync<Inventario>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Inventario/Eliminar/{id}");
            if (!response.IsSuccessStatusCode) TempData["Error"] = "Error al eliminar el producto.";
            return RedirectToAction(nameof(Index));
        }
    }
}
