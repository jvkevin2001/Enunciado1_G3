using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoFinal_G3.Models;

namespace ProyectoFinal_G3.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
