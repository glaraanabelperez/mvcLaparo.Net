using AutoMapper;
using CmmandService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using QueryService;
using Repositorys;
using System.Net.Http;
using System.Net.Http.Json;

namespace mvc.Laparoscopy.Controllers
{
    public class ProductUploadController : Controller
    {
        private readonly IProductCommandService commandService;
        private ILogger<ProductUploadController> _logger;

        public ProductUploadController(IProductCommandService commandService_, ILogger<ProductUploadController> logger)
        {
            commandService = commandService_;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult UploadForm()
        {
            return View();
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Error en el archivo que quiere subir";
                return RedirectToAction(nameof(UploadForm));
            }

            try
            {
                var res = await commandService.ChargeData(file);
                if(res.Succeeded == false)
                {
                    TempData["Error"] = res.message ?? "Error al cargar el archivo";
                    return RedirectToAction(nameof(UploadForm));
                }
                TempData["Success"] = "Archivo cargado correctamente ✔";
                _logger.LogInformation("Charge OK");

                return RedirectToAction(nameof(UploadForm));
            }
            catch (Exception ex)
            {
                var message = ((ex.InnerException != null) ? ex.InnerException!.Message : ex.Message);
                TempData["Error"] = "Hubo un problema en el sistema, comuniquelo al administrador.";
                _logger.LogError(message);

                return RedirectToAction(nameof(UploadForm));
            }
        }
    }
}
