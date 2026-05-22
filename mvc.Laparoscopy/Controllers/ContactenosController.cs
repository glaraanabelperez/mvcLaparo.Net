using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using System.Diagnostics;

namespace mvc.Laparoscopy.Controllers
{
    [Route("")]
    public class ContactenosController : Controller
    {
        private readonly ILogger<ContactenosController> _logger;

        public ContactenosController(ILogger<ContactenosController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Contactenos(ContactoViewModel model)
        {
            if (ModelState.IsValid)
            {
                // - Enviar email
                TempData["MensajeEnviado"] = "OK";
            }

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}