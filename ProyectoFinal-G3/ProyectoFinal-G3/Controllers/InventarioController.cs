
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models; // Asegúrate de que este 'using' apunte a tu modelo
using System.Text;
using System.Text.Json;

namespace ProyectoFinal_G3.Controllers
{
    public class InventarioController : Controller
    {
        private readonly HttpClient _httpClient;

        // Inyectamos IHttpClientFactory para crear un cliente HTTP y
        // IConfiguration para leer nuestro archivo appsettings.json
        public InventarioController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

            // Leemos la URL base de nuestra API desde la configuración
            // y se la asignamos al cliente para no tener que repetirla en cada llamada.
            string apiBaseUrl = configuration.GetValue<string>("Start:ApiUrl");
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
        }

        // --- ACCIÓN PARA MOSTRAR LA LISTA (READ) ---
        // Se activa con una petición GET del navegador a /Inventario
        public async Task<IActionResult> Index()
        {
            // Llama a la API usando el verbo GET para obtener la lista
            var response = await _httpClient.GetAsync("api/Inventario/Listar");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var productos = JsonSerializer.Deserialize<List<Inventario>>(content, options);
                return View(productos); // Envía la lista a la vista Index.cshtml
            }

            return View(new List<Inventario>()); // Devuelve una lista vacía si falla
        }

        // --- ACCIONES PARA CREAR UN PRODUCTO (CREATE) ---
        // 1. Muestra el formulario vacío
        // Se activa con una petición GET del navegador a /Inventario/Create
        public IActionResult Create()
        {
            return View(); // Muestra la vista Create.cshtml
        }

        // 2. Recibe los datos del formulario y los envía a la API
        // Se activa con una petición POST del formulario de creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventario inventario)
        {
            // Primero, revisa si los datos del formulario son válidos
            if (ModelState.IsValid)
            {
                // ===== INICIO DE CAMBIOS =====

                // 1. Obtenemos el ID del usuario desde la sesión del frontend.
                //    Asegúrate de que la clave "IdUsuario" sea la correcta.
                var idUsuarioString = HttpContext.Session.GetString("IdUsuario");

                if (string.IsNullOrEmpty(idUsuarioString))
                {
                    // Si no hay usuario en la sesión, lo mejor es enviarlo al Login.
                    return RedirectToAction("Login", "Usuario");
                }

                // 2. Asignamos los datos que faltan al objeto 'inventario'
                inventario.Id_Usuario = int.Parse(idUsuarioString);
                inventario.FechaRegistro = DateTime.Now;

                // ===== FIN DE CAMBIOS =====

                // 3. Convertimos el objeto 'inventario' (ya completo) a JSON
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(inventario),
                    Encoding.UTF8,
                    "application/json"
                );

                // 4. Enviamos el objeto completo a la API
                var response = await _httpClient.PostAsync("api/Inventario/Crear", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Si la API aun así da un error, lo mostramos
                    ModelState.AddModelError(string.Empty, "Error desde la API al crear el producto.");
                }
            }

            // Si el modelo no es válido, volvemos a mostrar el formulario
            return View(inventario);
        }

        // --- ACCIONES PARA EDITAR UN PRODUCTO (UPDATE) ---
        // 1. Obtiene el producto y muestra el formulario de edición
        // Se activa con una petición GET del navegador a /Inventario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Llama a la API usando GET para obtener el producto específico
            var response = await _httpClient.GetAsync($"api/Inventario/Obtener/{id}");
            if (response.IsSuccessStatusCode)
            {
                var producto = await response.Content.ReadFromJsonAsync<Inventario>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(producto); // Envía el producto a la vista Edit.cshtml
            }
            return NotFound();
        }

        // 2. Recibe los datos modificados y los envía a la API
        // Se activa con una petición POST del formulario de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventario inventario)
        {
            if (id != inventario.Id_Inventario) return BadRequest();

            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(inventario), Encoding.UTF8, "application/json");

                // Llama a la API usando el verbo PUT para actualizar el recurso
                var response = await _httpClient.PutAsync($"api/Inventario/Actualizar/{id}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index)); // Vuelve a la lista
                }
                ModelState.AddModelError(string.Empty, "Error desde la API al actualizar el producto.");
            }
            return View(inventario); // Si hay error, muestra el formulario de nuevo
        }

        // --- ACCIONES PARA ELIMINAR UN PRODUCTO (DELETE) ---
        // 1. Obtiene el producto y muestra una página de confirmación
        // Se activa con una petición GET del navegador a /Inventario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Llama a la API con GET para saber qué vamos a borrar
            var response = await _httpClient.GetAsync($"api/Inventario/Obtener/{id}");
            if (response.IsSuccessStatusCode)
            {
                var producto = await response.Content.ReadFromJsonAsync<Inventario>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(producto); // Muestra la vista de confirmación Delete.cshtml
            }
            return NotFound();
        }

        // 2. Ejecuta la eliminación
        // Se activa con una petición POST desde el formulario de confirmación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Llama a la API usando el verbo DELETE para eliminar el recurso
            var response = await _httpClient.DeleteAsync($"api/Inventario/Eliminar/{id}");

            if (!response.IsSuccessStatusCode)
            {
                // Si hay un error, podemos guardar un mensaje para mostrarlo en la página principal
                TempData["Error"] = "Error al eliminar el producto.";
            }
            return RedirectToAction(nameof(Index)); // Siempre vuelve a la lista
        }
    }
}