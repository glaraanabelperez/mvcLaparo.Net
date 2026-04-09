using AutoMapper;
using CmmandService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using QueryService;
using System.Net.Http;
using System.Net.Http.Json;

namespace mvc.Laparoscopy.Controllers
{
    public class ProductUploadController : Controller
    {
        private readonly IProductCommandService commandService;

        public ProductUploadController(IProductCommandService commandService_)
        {
            commandService = commandService_;
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
                await commandService.ChargeData(file);
                TempData["Success"] = "Archivo cargado correctamente ✔";
                return RedirectToAction(nameof(UploadForm));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(UploadForm));
            }
        }
    }
}
