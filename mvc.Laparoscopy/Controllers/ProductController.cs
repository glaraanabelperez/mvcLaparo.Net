using AutoMapper;
using CmmandService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using QueryService;
using QueryService.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace mvc.Laparoscopy.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServiceQuery queryService;
        public IMapper mapper;
        public ILogger<ProductController> _logger;

        public ProductController(IProductServiceQuery queryService_, IMapper mapper, ILogger<ProductController> logger)
        {
            queryService = queryService_;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            int? categoryId,
            bool? favorite,
            bool? discount,
            bool? state,
            string? search,
            int page = 1)
        {
            int length = 50;

            var response = await queryService.GetAll(
                favorite,
                discount,
                state,
                search,
                page,
                length
            );

            //var result = new PagedResponse<ProductViewModel>
            //{
            //    Items = mapper.Map<List<ProductViewModel>>(response.Items),
            //    Total = response.Total,
            //    Page = response.Page,
            //    Pages = response.Pages,
            //    HasItems = response.HasItems
            //};

            try
            {
                var result = mapper.Map<PagedResponse<ProductViewModel>>(response);

                if (result == null || !result.HasItems)
                    return View(new PagedResponse<ProductViewModel>());

                return View(result);
            }
            catch(Exception ex)
            {
                var message = ((ex.InnerException != null) ? ex.InnerException!.Message : ex.Message);
                TempData["Error"] = "Hubo un problema en el sistema, comuniquelo al administrador.";
                _logger.LogError(message);
                return View(new PagedResponse<ProductViewModel>());

            }

        }

       
    }
}
