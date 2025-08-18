using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;
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

        [HttpGet]
        public IActionResult Analisis()
        {
            var modelo = new ReporteAnalisisViewModel
            {
                FechaInicio = DateTime.Today.AddDays(-7),
                FechaFin = DateTime.Today
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Analisis(ReporteAnalisisViewModel filtros)
        {
            if (!ModelState.IsValid)
            {
                return View(filtros);
            }

            var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            string fechaInicio = filtros.FechaInicio.ToString("yyyy-MM-dd");
            string fechaFin = filtros.FechaFin.ToString("yyyy-MM-dd");

            var responseVentas = await cliente.GetAsync($"api/reportes/ventas?fechaInicio={fechaInicio}&fechaFin={fechaFin}");
            var responseServicios = await cliente.GetAsync($"api/reportes/servicios?fechaInicio={fechaInicio}&fechaFin={fechaFin}");

            if (responseVentas.IsSuccessStatusCode)
            {
                var jsonVentas = await responseVentas.Content.ReadAsStringAsync();
                var resultadoVentas = JsonSerializer.Deserialize<RespuestaEstandar<List<ReporteVentaDTO>>>(jsonVentas, _jsonOptions);
                filtros.Ventas = resultadoVentas?.Contenido ?? [];
            }

            if (responseServicios.IsSuccessStatusCode)
            {
                var jsonServicios = await responseServicios.Content.ReadAsStringAsync();
                var resultadoServicios = JsonSerializer.Deserialize<RespuestaEstandar<List<ReporteServicioDTO>>>(jsonServicios, _jsonOptions);
                filtros.Servicios = resultadoServicios?.Contenido ?? [];
            }

            if (filtros.Ventas.Count == 0 && filtros.Servicios.Count == 0)
            {
                ViewBag.Mensaje = "No se encontraron resultados para el rango de fechas seleccionado.";
            }

            return View(filtros);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarAnalisis(ReporteAnalisisViewModel filtros)
        {
            if (filtros.Formato != "PDF" && filtros.Formato != "Excel")
            {
                ViewBag.Error = "Formato inválido.";
                return RedirectToAction("Analisis", filtros);
            }

            var cliente = _http.CreateClient();
            cliente.BaseAddress = new Uri(_configuration.GetSection("Start:ApiUrl").Value!);

            string endpoint = $"api/reportes/analisis/exportar?fechaInicio={filtros.FechaInicio:yyyy-MM-dd}&fechaFin={filtros.FechaFin:yyyy-MM-dd}&formato={filtros.Formato}";

            var response = await cliente.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var contentType = filtros.Formato == "PDF"
                    ? "application/pdf"
                    : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                var fileName = $"Reporte_Analisis_{DateTime.Now:yyyyMMdd_HHmmss}.{(filtros.Formato == "PDF" ? "pdf" : "xlsx")}";

                return File(content, contentType, fileName);
            }

            ViewBag.Error = "No se pudo exportar el reporte.";
            return RedirectToAction("Analisis");
        }
    }
}
