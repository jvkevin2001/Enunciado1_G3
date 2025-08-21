using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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


            var productosResponse = http.GetFromJsonAsync<List<Inventario>>("api/Inventario/Listar").Result;
            var productosJs = new List<dynamic>();

            if (productosResponse != null)
            {
                productosJs = productosResponse
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

        [HttpGet]
        public IActionResult ExportarFactura(int id)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var response = http.GetAsync($"api/Ventas/ObtenerVentaCompleta/{id}").Result;
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo generar la factura.";
                return RedirectToAction("Ventas");
            }

            var jsonString = response.Content.ReadAsStringAsync().Result;
            using var doc = System.Text.Json.JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            if (!root.TryGetProperty("contenido", out var contenido))
            {
                TempData["Error"] = "No se encontró información de la factura.";
                return RedirectToAction("Ventas");
            }

            var clientesResponse = http.GetFromJsonAsync<RespuestaEstandar<List<Cliente>>>("api/Clientes/ObtenerClientes").Result;
            var clientes = clientesResponse?.Contenido ?? new List<Cliente>();
            var venta = new ProyectoFinal_G3.Models.Venta
            {
                Id_Venta = 0,
                FechaVenta = DateTime.Now,
                Total = 0m,
                Cliente = new ProyectoFinal_G3.Models.Cliente { NombreCliente = "Sin cliente" }
            };

            if (contenido.TryGetProperty("venta", out var ventaJson))
            {
                venta.Id_Venta = ventaJson.TryGetProperty("id_Venta", out var idProp) ? idProp.GetInt32() : 0;
                venta.FechaVenta = ventaJson.TryGetProperty("fechaVenta", out var fechaProp) ? fechaProp.GetDateTime() : DateTime.Now;
                venta.Total = ventaJson.TryGetProperty("total", out var totalVentaProp) ? totalVentaProp.GetDecimal() : 0m;

                var idCliente = ventaJson.TryGetProperty("id_Cliente", out var idClienteProp) ? idClienteProp.GetInt32() : 0;
                venta.Cliente = clientes.FirstOrDefault(c => c.Id_Clientes == idCliente) ?? new Cliente { NombreCliente = "Sin cliente" };
            }

            var detalles = new List<ProyectoFinal_G3.Models.DetalleVenta>();
            if (contenido.TryGetProperty("detalles", out var detallesJson))
            {
                foreach (var d in detallesJson.EnumerateArray())
                {
                    detalles.Add(new ProyectoFinal_G3.Models.DetalleVenta
                    {
                        Cantidad = d.TryGetProperty("cantidad", out var cantidadProp) ? cantidadProp.GetInt32() : 0,
                        PrecioTotal = d.TryGetProperty("precioTotal", out var precioTotalProp) ? precioTotalProp.GetDecimal() : 0m,
                        Producto = new ProyectoFinal_G3.Models.Inventario
                        {
                            ProductoNombre = d.TryGetProperty("productoNombre", out var productoNombreProp) ? productoNombreProp.GetString() ?? "Sin nombre" : "Sin nombre",
                            PrecioUnitario = d.TryGetProperty("precioUnitario", out var precioUnitarioProp) ? precioUnitarioProp.GetDecimal() : 0m
                        }
                    });
                }
            }

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Size(PageSizes.A4);

                    page.Header().Text($"Factura #{venta.Id_Venta}")
                        .FontSize(20).Bold().FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text($"Cliente: {venta.Cliente.NombreCliente}");
                        col.Item().Text($"Fecha: {venta.FechaVenta:yyyy-MM-dd}");
                        col.Item().Text($"Total: ₡{venta.Total:N2}").Bold();

                        col.Item().LineHorizontal(1);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(100);
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Producto").Bold();
                                header.Cell().Text("Cantidad").Bold();
                                header.Cell().Text("Precio Unitario").Bold();
                                header.Cell().Text("Total").Bold();
                            });

                            foreach (var d in detalles)
                            {
                                table.Cell().Text(d.Producto.ProductoNombre);
                                table.Cell().Text(d.Cantidad.ToString());
                                table.Cell().Text($"₡{(d.Cantidad > 0 ? d.PrecioTotal / d.Cantidad : d.PrecioTotal):N2}");
                                table.Cell().Text($"₡{d.PrecioTotal:N2}");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text("Gracias por su compra").Italic();
                });
            });

            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", $"Factura_{venta.Id_Venta}.pdf");
        }



    }
}
