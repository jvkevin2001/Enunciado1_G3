using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using ProyectoFinal_G3.Services;
using System.Net.Http.Json;

namespace ProyectoFinal_G3.Controllers
{
    public class FacturacionController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _configuration;

        public FacturacionController(IConfiguration configuration, IHttpClientFactory http)
        {
            _configuration = configuration;
            _http = http;
        }

        [HttpGet]
        public IActionResult CrearVenta()
        {
            using var http = _http.CreateClient();

            var apiUrl = _configuration["Start:ApiUrl"];
            if (string.IsNullOrEmpty(apiUrl))
                throw new Exception("Falta ApiUrl en configuración");

            http.BaseAddress = new Uri(apiUrl);

            var clientesResponse = http.GetAsync("api/Clientes/ObtenerClientes").Result;
            ViewBag.Clientes = clientesResponse.Content
                                .ReadFromJsonAsync<RespuestaEstandar<List<Cliente>>>()
                                .Result?.Contenido ?? new List<Cliente>();

            var productosResponse = http.GetAsync("api/Inventario/Listar").Result;
            ViewBag.Productos = productosResponse.Content
                                .ReadFromJsonAsync<RespuestaEstandar<List<Inventario>>>()
                                .Result?.Contenido ?? new List<Inventario>();

            return View();
        }

        [HttpPost]
        public IActionResult CrearVenta(VentaDirecta venta)
        {
            using var http = _http.CreateClient();
            var apiUrl = _configuration["Start:ApiUrl"];
            http.BaseAddress = new Uri(apiUrl);

            venta.Detalles = venta.Detalles ?? new List<DetalleVenta>();

            foreach (var detalle in venta.Detalles)
            {
                detalle.Id_Venta = venta.Id_Venta; 
                var responseDetalle = http.PostAsJsonAsync("api/DetalleVenta/Insertar", detalle).Result;
                if (!responseDetalle.IsSuccessStatusCode)
                {
                    TempData["Error"] = "No se pudo insertar un detalle de venta";
                    return CrearVenta();
                }
            }

            var factura = new Factura
            {
                Id_Venta = venta.Id_Venta,
                FechaEmision = DateTime.Now,
                Total = venta.Total
            };
            var facturaResponse = http.PostAsJsonAsync("api/Factura/Insertar", factura).Result;

            if (facturaResponse.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Venta y factura generadas correctamente";
                return RedirectToAction("HistorialFacturas");
            }

            TempData["Error"] = "No se pudo generar la factura";
            return CrearVenta();
        }

        [HttpGet]
        public IActionResult HistorialFacturas(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? clienteId = null)
        {
            using var http = _http.CreateClient();
            var apiUrl = _configuration["Start:ApiUrl"];
            http.BaseAddress = new Uri(apiUrl);

            var response = http.GetAsync("api/Factura/ObtenerFacturas").Result;
            var facturas = response.Content
                                .ReadFromJsonAsync<RespuestaEstandar<List<Factura>>>()
                                .Result?.Contenido ?? new List<Factura>();

            if (fechaInicio.HasValue) facturas = facturas.Where(f => f.FechaEmision >= fechaInicio.Value).ToList();
            if (fechaFin.HasValue) facturas = facturas.Where(f => f.FechaEmision <= fechaFin.Value).ToList();
            if (clienteId.HasValue) facturas = facturas.Where(f => f.Id_Cliente == clienteId.Value).ToList();

            return View(facturas);
        }


        [HttpGet]
        public IActionResult VerFactura(int idFactura)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration["Start:ApiUrl"]);

            var response = http.GetAsync($"api/Factura/ObtenerFactura?Id_Factura={idFactura}").Result;
            var factura = response.Content.ReadFromJsonAsync<RespuestaEstandar<Factura>>().Result?.Contenido;

            if (factura == null)
            {
                TempData["Error"] = "Factura no encontrada";
                return RedirectToAction("HistorialFacturas");
            }

            var detallesResponse = http.GetAsync($"api/DetalleVenta/ObtenerDetalleVenta?Id_Venta={factura.Id_Venta}").Result;
            var detalles = detallesResponse.Content.ReadFromJsonAsync<RespuestaEstandar<List<DetalleVenta>>>().Result?.Contenido;
            factura.Detalles = detalles ?? new List<DetalleVenta>();

            return View(factura);
        }

    }
}
