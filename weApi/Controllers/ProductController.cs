using CmmandService.Interfaces;
using CmmandService.ModelsCommand;
using Microsoft.AspNetCore.Mvc;
using QueryService;

namespace webApi.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
      
        private readonly ILogger<ProductController> _logger;
        private readonly IProductServiceQuery queryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IProductCommandService commandService;
        private readonly IWebHostEnvironment _env;

        public ProductController(ILogger<ProductController> logger
            ,IProductServiceQuery _queryService, IHttpContextAccessor httpContextAccessor
            ,IProductCommandService _commandService)
        {
            _logger = logger;
            queryService = _queryService;
            commandService= _commandService;
            _httpContextAccessor= httpContextAccessor;
        }
 
        [HttpGet("/list")]
        public async Task<IActionResult> GetByFilters(
             [FromQuery] int? CategoryId,
             [FromQuery] bool? Fauvorite,
             [FromQuery] bool? Discount,
             [FromQuery] bool? State,
             [FromQuery] string? Search,
             [FromQuery] int From = 1,
             [FromQuery] int Length = 50)
        {
            var result = await queryService.GetAll( Fauvorite, Discount, State, Search, From, Length);
            return Ok(result);
        }


        [HttpPost("UploadFiles")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var request = _httpContextAccessor.HttpContext != null ? _httpContextAccessor.HttpContext.Request : null;
            if (request == null || file.Length <= 0)
            {
                return BadRequest();
            }
            try
            {
                await this.commandService.ChargeData(file);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);//poner bien el manejo del 400 y del 500
            }

            


            
        

        }

        [HttpPost("/add")]
        public async Task<IActionResult> PostProductAdd([FromForm] ProductCreateCommand prod)
        {

            var result = await commandService.Add(prod);
            if (result.Succeeded == true) return Ok(result);
            return BadRequest(result);

        }
    }
}