using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using System.Net.Http.Json;

namespace ProyectoFinal_G3.Controllers
{
    public class VentasController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _configuration;

        public VentasController(IConfiguration configuration, IHttpClientFactory http)
        {
            _configuration = configuration;
            _http = http;
        }

        [HttpGet]
        public IActionResult CrearVenta()
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var clientesResponse = http.GetFromJsonAsync<RespuestaEstandar<List<Cliente>>>("api/Clientes/ObtenerClientes").Result;
            ViewBag.Clientes = clientesResponse?.Contenido ?? new List<Cliente>();

            var productosResponse = http.GetFromJsonAsync<RespuestaEstandar<List<Inventario>>>("api/Inventario/Listar").Result;
            var productosJs = new List<dynamic>();

            if (productosResponse?.Contenido != null)
            {
                productosJs = productosResponse.Contenido
                    .Select(p => new
                    {
                        Id_Inventario = p.Id_Inventario,
                        Nombre = p.ProductoNombre ?? "",
                        Precio = p.PrecioUnitario
                    })
                    .Cast<dynamic>()
                    .ToList();
            }

            ViewBag.Productos = productosJs;

            return View();
        }

        [HttpPost]
        public IActionResult CrearVenta(Venta venta, string detallesJson)
        {
            var detalles = System.Text.Json.JsonSerializer.Deserialize<List<DetalleVenta>>(detallesJson);

            if (detalles == null || detalles.Count == 0)
            {
                TempData["Error"] = "Debe agregar al menos un producto";
                return CrearVenta();
            }

            var payload = new
            {
                Venta = new
                {
                    Id_Cliente = venta.Id_Cliente,
                    NombreCliente = venta.Cliente?.NombreCliente ?? "",
                    Total = venta.Total,
                    FechaVenta = DateTime.Now
                },
                Detalles = detalles.Select(d => new
                {
                    Id_Inventario = d.Id_Inventario,
                    Cantidad = d.Cantidad,
                    PrecioTotal = d.PrecioTotal
                }).ToList()
            };

            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = http.PostAsJsonAsync("api/Ventas/CrearVenta", payload).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Ventas");
            }
            else
            {
                var respuesta = response.Content.ReadFromJsonAsync<RespuestaEstandar>().Result;
                TempData["Error"] = respuesta?.Mensaje;
                return CrearVenta();
            }
        }

        [HttpGet]
        public IActionResult Ventas()
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = http.GetAsync("api/Ventas/ObtenerVentas").Result;
            var ventas = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<RespuestaEstandar<List<Venta>>>().Result?.Contenido
                : new List<Venta>();

            var clientesResponse = http.GetFromJsonAsync<RespuestaEstandar<List<Cliente>>>("api/Clientes/ObtenerClientes").Result;
            var clientes = clientesResponse?.Contenido ?? new List<Cliente>();

            foreach (var v in ventas)
            {
                v.Cliente = clientes.FirstOrDefault(c => c.Id_Clientes == v.Id_Cliente);
            }

            return View(ventas);
        }

        [HttpGet]
        public IActionResult HistorialFacturas()
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = http.GetAsync("api/ventas/ObtenerFacturas").Result;

            var facturas = response.IsSuccessStatusCode
                ? response.Content.ReadFromJsonAsync<RespuestaEstandar<List<FacturaVenta>>>().Result?.Contenido
                : new List<FacturaVenta>();

            return View(facturas);
        }


        [HttpGet]
        public IActionResult DetalleVenta(int id)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = http.GetAsync($"api/Ventas/ObtenerVentaCompleta/{id}").Result;

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo cargar la venta.";
                return RedirectToAction("Ventas");
            }

            var jsonString = response.Content.ReadAsStringAsync().Result;
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            var root = doc.RootElement;
            var contenido = root.GetProperty("contenido");
            var ventaJson = contenido.GetProperty("venta");

            var clientesResponse = http.GetFromJsonAsync<RespuestaEstandar<List<Cliente>>>("api/Clientes/ObtenerClientes").Result;
            var clientes = clientesResponse?.Contenido ?? new List<Cliente>();

            var idCliente = ventaJson.GetProperty("id_Cliente").GetInt32();
            var venta = new ProyectoFinal_G3.Models.Venta
            {
                Id_Venta = ventaJson.GetProperty("id_Venta").GetInt32(),
                Id_Cliente = idCliente,
                FechaVenta = ventaJson.GetProperty("fechaVenta").GetDateTime(),
                Total = ventaJson.GetProperty("total").GetDecimal(),
                Cliente = clientes.FirstOrDefault(c => c.Id_Clientes == idCliente) ?? new ProyectoFinal_G3.Models.Cliente
                {
                    Id_Clientes = idCliente,
                    NombreCliente = "Sin cliente"
                }
            };

            var detalles = new List<ProyectoFinal_G3.Models.DetalleVenta>();
            foreach (var d in contenido.GetProperty("detalles").EnumerateArray())
            {
                detalles.Add(new ProyectoFinal_G3.Models.DetalleVenta
                {
                    Id_Venta = d.GetProperty("id_Venta").GetInt32(),
                    Id_Inventario = d.GetProperty("id_Inventario").GetInt32(),
                    Cantidad = d.GetProperty("cantidad").GetInt32(),
                    PrecioTotal = d.GetProperty("precioTotal").GetDecimal(),
                    Producto = new ProyectoFinal_G3.Models.Inventario
                    {
                        Id_Inventario = d.GetProperty("id_Inventario").GetInt32(),
                        ProductoNombre = d.GetProperty("productoNombre").GetString(),
                        PrecioUnitario = d.GetProperty("precioUnitario").GetDecimal()
                    }
                });
            }

            ViewBag.Venta = venta;
            ViewBag.Detalles = detalles;

            return View();
        }


    }
}
