using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using System.Diagnostics;

namespace mvc.Laparoscopy.Controllers
{
    [Route("import-surgery/Home")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contactenos()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contactenos(ContactoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Acá podrías:
                // - Enviar email
                // - Guardar en base de datos
                // - Enviar a CRM
                TempData["MensajeEnviado"] = "OK";

                return RedirectToAction("Contactenos");
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}