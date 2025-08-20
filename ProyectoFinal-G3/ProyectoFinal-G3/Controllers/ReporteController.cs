using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;

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

            var resp = await cliente.GetAsync("api/Reportes/Clientes");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "No se pudieron cargar los clientes";
                return View("ReporteClientes", new List<Cliente>());
            }

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Cliente>>>();
            return View("ReporteClientes", datos?.Contenido ?? new List<Cliente>());
        }

        [HttpGet]
        public async Task<IActionResult> ExportarClientesCSV()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Clientes");
            if (!resp.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Cliente>>>();
            var clientes = datos?.Contenido ?? new List<Cliente>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ID,Nombre,Correo,Telefono");

            foreach (var c in clientes)
                sb.AppendLine($"{c.Id_Clientes},{c.NombreCliente},{c.Correo},{c.Telefono}");

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "ReporteClientes.csv");
        }


        [HttpGet]
        public async Task<IActionResult> ExportarClientesPDF()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Clientes");
            if (!resp.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Cliente>>>();
            var clientes = datos?.Contenido ?? new List<Cliente>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<h2>Reporte de Clientes</h2>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<tr><th>ID</th><th>Nombre</th><th>Correo</th><th>Teléfono</th></tr>");

            foreach (var c in clientes)
            {
                sb.AppendLine($"<tr><td>{c.Id_Clientes}</td><td>{c.NombreCliente}</td><td>{c.Correo}</td><td>{c.Telefono}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");

            return Content(sb.ToString(), "text/html");
        }


        // HISTORIAL VENTAS
        [HttpGet]
        public async Task<IActionResult> HistorialVentas(string fechaInicio, string fechaFin)
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/Reportes/HistorialVentas";

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                var inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                url += $"?fechaInicio={inicio:yyyy-MM-dd}&fechaFin={fin:yyyy-MM-dd}";
            }

            var resp = await cliente.GetAsync(url);

            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "No se pudieron cargar las ventas";
                return View("ReporteHistorialVentas", new List<HistorialVentaReporte>());
            }

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<HistorialVentaReporte>>>();

            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View("ReporteHistorialVentas", datos?.Contenido ?? new List<HistorialVentaReporte>());
        }

        [HttpGet]
        public async Task<IActionResult> ExportarHistorialVentasCSV(string fechaInicio, string fechaFin)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/Reportes/HistorialVentas";

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                var inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                url += $"?fechaInicio={inicio:yyyy-MM-dd}&fechaFin={fin:yyyy-MM-dd}";
            }

            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await response.Content.ReadFromJsonAsync<RespuestaEstandar<List<HistorialVentaReporte>>>();
            var ventas = datos?.Contenido ?? new List<HistorialVentaReporte>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ID Venta,Cliente,Usuario,Fecha Venta,Total");

            foreach (var v in ventas)
            {
                sb.AppendLine($"{v.Id_Venta},{v.Cliente},{v.Usuario},{v.FechaVenta:dd/MM/yyyy},{v.TotalVenta}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "HistorialVentas.csv");
        }


        [HttpGet]
        public async Task<IActionResult> ExportarHistorialVentasPDF(string fechaInicio, string fechaFin)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/Reportes/HistorialVentas";

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                var inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                url += $"?fechaInicio={inicio:yyyy-MM-dd}&fechaFin={fin:yyyy-MM-dd}";
            }

            var response = await http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await response.Content.ReadFromJsonAsync<RespuestaEstandar<List<HistorialVentaReporte>>>();
            var ventas = datos?.Contenido ?? new List<HistorialVentaReporte>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<h2>Historial de Ventas</h2>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<tr><th>ID Venta</th><th>Cliente</th><th>Usuario</th><th>Fecha Venta</th><th>Total</th></tr>");

            foreach (var v in ventas)
            {
                sb.AppendLine($"<tr>" +
                              $"<td>{v.Id_Venta}</td>" +
                              $"<td>{v.Cliente}</td>" +
                              $"<td>{v.Usuario}</td>" +
                              $"<td>{v.FechaVenta:dd/MM/yyyy}</td>" +
                              $"<td>{v.TotalVenta:C}</td>" +
                              $"</tr>");
            }

            sb.AppendLine("</table></body></html>");

            return Content(sb.ToString(), "text/html");
        }




        // INVENTARIO
        [HttpGet]
        public async Task<IActionResult> Inventario()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Inventario");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "No se pudieron cargar los registros de inventario";
                return View("ReporteInventario", new List<Inventario>());
            }

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Inventario>>>();
            return View("ReporteInventario", datos?.Contenido ?? new List<Inventario>());
        }


        [HttpGet]
        public async Task<IActionResult> ExportarInventarioCSV()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Inventario");
            if (!resp.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Inventario>>>();
            var inventario = datos?.Contenido ?? new List<Inventario>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Id_Inventario,ProductoNombre,Cantidad,Descripcion,FechaRegistro,Proveedor,PrecioUnitario,Id_Usuario");

            foreach (var i in inventario)
            {
                sb.AppendLine($"{i.Id_Inventario},{i.ProductoNombre},{i.Cantidad},{i.Descripcion},{i.FechaRegistro:dd/MM/yyyy},{i.Proveedor},{i.PrecioUnitario},{i.Id_Usuario}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "ReporteInventario.csv");
        }

        [HttpGet]
        public async Task<IActionResult> ExportarInventarioPDF()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Inventario");
            if (!resp.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Inventario>>>();
            var inventario = datos?.Contenido ?? new List<Inventario>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<h2>Reporte de Inventario</h2>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<tr><th>ID</th><th>Producto</th><th>Cantidad</th><th>Descripción</th><th>Fecha Registro</th><th>Proveedor</th><th>Precio Unitario</th><th>Usuario</th></tr>");

            foreach (var i in inventario)
            {
                sb.AppendLine($"<tr><td>{i.Id_Inventario}</td><td>{i.ProductoNombre}</td><td>{i.Cantidad}</td><td>{i.Descripcion}</td><td>{i.FechaRegistro:dd/MM/yyyy}</td><td>{i.Proveedor}</td><td>{i.PrecioUnitario}</td><td>{i.Id_Usuario}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");

            return Content(sb.ToString(), "text/html");
        }

        // REPARACIONES
        [HttpGet]
        public async Task<IActionResult> Reparaciones(string fechaInicio, string fechaFin)
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/Reportes/Reparaciones";

            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                var inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                url += $"?fechaInicio={inicio:yyyy-MM-dd}&fechaFin={fin:yyyy-MM-dd}";
            }

            var resp = await cliente.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "No se pudieron cargar las reparaciones";
                return View("ReporteReparaciones", new List<Reparacion>());
            }

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Reparacion>>>();
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View("ReporteReparaciones", datos?.Contenido ?? new List<Reparacion>());
        }

        [HttpGet]
        public async Task<IActionResult> ExportarReparacionesCSV(string fechaInicio, string fechaFin)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/Reportes/Reparaciones";
            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                var fi = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var ff = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                url += $"?fechaInicio={fi:yyyy-MM-dd}&fechaFin={ff:yyyy-MM-dd}";
            }

            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await response.Content.ReadFromJsonAsync<RespuestaEstandar<List<Reparacion>>>();
            var reparaciones = datos?.Contenido ?? new List<Reparacion>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("ID,Cliente,Equipo,Tipo,FechaServicio,FechaSalida,Costo,Estado");

            foreach (var r in reparaciones)
            {
                sb.AppendLine($"{r.Id_Reparacion},{r.Cliente},{r.EquipoDescripcion},{r.TipoMaquina}," +
                              $"{r.FechaServicio:dd/MM/yyyy},{(r.FechaSalida.HasValue ? r.FechaSalida.Value.ToString("dd/MM/yyyy") : "")}," +
                              $"{r.CostoServicio},{r.Estado}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "ReporteReparaciones.csv");
        }


        [HttpGet]
        public async Task<IActionResult> ExportarReparacionesPDF(string fechaInicio, string fechaFin)
        {
            using var http = _http.CreateClient();
            http.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var url = "api/Reportes/Reparaciones";
            if (!string.IsNullOrEmpty(fechaInicio) && !string.IsNullOrEmpty(fechaFin))
            {
                var fi = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var ff = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                url += $"?fechaInicio={fi:yyyy-MM-dd}&fechaFin={ff:yyyy-MM-dd}";
            }

            var response = await http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await response.Content.ReadFromJsonAsync<RespuestaEstandar<List<Reparacion>>>();
            var reparaciones = datos?.Contenido ?? new List<Reparacion>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<h2>Reporte de Reparaciones</h2>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<tr><th>ID</th><th>Cliente</th><th>Equipo</th><th>Tipo</th><th>Fecha Servicio</th><th>Fecha Salida</th><th>Costo</th><th>Estado</th></tr>");

            foreach (var r in reparaciones)
            {
                sb.AppendLine($"<tr>" +
                              $"<td>{r.Id_Reparacion}</td>" +
                              $"<td>{r.Cliente}</td>" +
                              $"<td>{r.EquipoDescripcion}</td>" +
                              $"<td>{r.TipoMaquina}</td>" +
                              $"<td>{r.FechaServicio:dd/MM/yyyy}</td>" +
                              $"<td>{(r.FechaSalida.HasValue ? r.FechaSalida.Value.ToString("dd/MM/yyyy") : "")}</td>" +
                              $"<td>{r.CostoServicio:C}</td>" +
                              $"<td>{r.Estado}</td>" +
                              $"</tr>");
            }

            sb.AppendLine("</table></body></html>");

            return Content(sb.ToString(), "text/html");
        }



        // USUARIOS
        [HttpGet]
        public async Task<IActionResult> Usuarios()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Usuarios");
            if (!resp.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "No se pudieron cargar los usuarios";
                return View("ReporteUsuarios", new List<Usuario>()); // 👈 aquí el nombre real de la vista
            }

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Usuario>>>();
            return View("ReporteUsuarios", datos?.Contenido ?? new List<Usuario>()); // 👈 aquí también
        }

        [HttpGet]
        public async Task<IActionResult> ExportarUsuariosCSV()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Usuarios");
            if (!resp.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Usuario>>>();
            var usuarios = datos?.Contenido ?? new List<Usuario>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("IdUsuario,Nombre_Completo,Correo,Id_Rol,Estado");

            foreach (var u in usuarios)
            {
                sb.AppendLine($"{u.IdUsuario},{u.Nombre_Completo},{u.Correo},{u.Id_Rol},{(u.Estado ? "Activo" : "Inactivo")}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "ReporteUsuarios.csv");
        }

        [HttpGet]
        public async Task<IActionResult> ExportarUsuariosPDF()
        {
            using var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            var resp = await cliente.GetAsync("api/Reportes/Usuarios");
            if (!resp.IsSuccessStatusCode)
                return BadRequest("No se pudo obtener la información");

            var datos = await resp.Content.ReadFromJsonAsync<RespuestaEstandar<List<Usuario>>>();
            var usuarios = datos?.Contenido ?? new List<Usuario>();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html><head><meta charset='UTF-8'></head><body>");
            sb.AppendLine("<h2>Reporte de Usuarios</h2>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<tr><th>ID</th><th>Nombre Completo</th><th>Correo</th><th>Rol</th><th>Estado</th></tr>");

            foreach (var u in usuarios)
            {
                sb.AppendLine($"<tr><td>{u.IdUsuario}</td><td>{u.Nombre_Completo}</td><td>{u.Correo}</td><td>{u.Id_Rol}</td><td>{(u.Estado ? "Activo" : "Inactivo")}</td></tr>");
            }

            sb.AppendLine("</table></body></html>");

            return Content(sb.ToString(), "text/html"); // 👈 mismo truco que en clientes
        }


    }


}
