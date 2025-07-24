using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal_G3.Controllers
{
    public class FacturaciónController : Controller
    {
        public IActionResult FacturacionVentas()
        {
            return View();
        }

        public IActionResult FacturacionReparaciones()
        {
            return View();
        }

        public IActionResult FacturacionAdmin()
        {
            return View();
        }

    }
}
