using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using QueryService;

namespace mvc.Laparoscopy.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServiceQuery queryServiceProduct;
        public IMapper mapper;
        public ILogger<ProductController> _logger;

        public ProductController(IProductServiceQuery queryServiceProduct_, 
            IMapper mapper, ILogger<ProductController> logger)
        {
            queryServiceProduct = queryServiceProduct_;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Index(
            string categoryId,
            bool? favorite,
            bool? discount,
            bool? state,
            string? search,
            int page = 1)
        {
            int length = 50;

            var response = await queryServiceProduct.GetAll(
                categoryId,
                favorite,
                discount,
                state,
                search,
                page,
                length
            );

            try
            {
                var result = mapper.Map<PagedResponse<ProductViewModel>>(response);

                if (result == null || !result.HasItems)
                    throw new Exception("Error en los datos. Estos son null");

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
