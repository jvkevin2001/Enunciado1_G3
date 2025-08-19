using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.ViewModels;
using System.Text.Json;

namespace ProyectoFinal_G3.Controllers
{
    public class ReportesController(IConfiguration configuration, IHttpClientFactory http) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpClientFactory _http = http;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // CLIENTES
        [HttpGet]
        public async Task<IActionResult> Clientes()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/reportes/clientes");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Error = $"API respondió {(int)resp.StatusCode}";
                return View("ReporteClientes", new List<ReporteClienteViewModel>());
            }

            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<ReporteClienteViewModel>>(json, _jsonOptions);
            return View("ReporteClientes", data ?? new List<ReporteClienteViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Clientes(DateTime? fechaInicio, DateTime? fechaFin)
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            string url = "api/reportes/clientes";
            if (fechaInicio.HasValue && fechaFin.HasValue)
                url += $"?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}";

            try
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<List<ReporteClienteViewModel>>(json, _jsonOptions);
                    return View("ReporteClientes", data ?? new List<ReporteClienteViewModel>());
                }

                ViewBag.Error = "No se pudieron obtener los clientes.";
                return View("ReporteClientes", new List<ReporteClienteViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al conectar con la API: {ex.Message}";
                return View("ReporteClientes", new List<ReporteClienteViewModel>());
            }
        }

        // HISTORIAL VENTAS
        [HttpGet]
        public IActionResult HistorialVentas()
        {
            return View("ReporteHistorialVentas", new List<ReporteHistorialVentaViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> HistorialVentas(DateTime? fechaInicio, DateTime? fechaFin, int? idCliente, int? idUsuario)
        {
            var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/reportes/ventas";
            var qs = new List<string>();
            if (fechaInicio.HasValue) qs.Add($"fechaInicio={fechaInicio:yyyy-MM-dd}");
            if (fechaFin.HasValue) qs.Add($"fechaFin={fechaFin:yyyy-MM-dd}");
            if (idCliente.HasValue) qs.Add($"idCliente={idCliente}");
            if (idUsuario.HasValue) qs.Add($"idUsuario={idUsuario}");
            if (qs.Count > 0) url += "?" + string.Join("&", qs);

            var response = await cliente.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ReporteHistorialVentaViewModel>>(json, _jsonOptions);
                return View("ReporteHistorialVentas", data ?? new List<ReporteHistorialVentaViewModel>());
            }

            ViewBag.Error = "No se encontraron ventas en el rango seleccionado.";
            return View("ReporteHistorialVentas", new List<ReporteHistorialVentaViewModel>());
        }

        // INVENTARIO
        [HttpGet]
        public IActionResult Inventario()
        {
            return View("ReporteInventario", new List<ReporteInventarioViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Inventario(DateTime? fecha)
        {
            var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = await cliente.GetAsync("api/reportes/inventario");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ReporteInventarioViewModel>>(json, _jsonOptions);
                return View("ReporteInventario", data ?? new List<ReporteInventarioViewModel>());
            }

            ViewBag.Error = "No se pudo obtener el inventario.";
            return View("ReporteInventario", new List<ReporteInventarioViewModel>());
        }

        // REPARACIONES
        [HttpGet]
        public IActionResult Reparaciones()
        {
            return View("ReporteReparaciones", new List<ReporteReparacionViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Reparaciones(DateTime fechaInicio, DateTime fechaFin)
        {
            var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = await cliente.GetAsync(
                $"api/reportes/reparaciones?fechaInicio={fechaInicio:yyyy-MM-dd}&fechaFin={fechaFin:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ReporteReparacionViewModel>>(json, _jsonOptions);
                return View("ReporteReparaciones", data ?? new List<ReporteReparacionViewModel>());
            }

            ViewBag.Error = "No se encontraron reparaciones en el rango de fechas.";
            return View("ReporteReparaciones", new List<ReporteReparacionViewModel>());
        }

        // USUARIOS
        [HttpGet]
        public IActionResult Usuarios()
        {
            return View("ReporteUsuarios", new List<ReporteUsuarioViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Usuarios(string rol = "")
        {
            var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/reportes/usuarios";
            if (!string.IsNullOrEmpty(rol))
                url += $"?rol={rol}";

            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ReporteUsuarioViewModel>>(json, _jsonOptions);
                return View("ReporteUsuarios", data ?? new List<ReporteUsuarioViewModel>());
            }

            ViewBag.Error = "No se pudieron obtener los usuarios.";
            return View("ReporteUsuarios", new List<ReporteUsuarioViewModel>());
        }
    }
}
