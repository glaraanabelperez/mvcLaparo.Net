using Microsoft.AspNetCore.Mvc;
using QueryService;
using QueryService.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace webApi.Controllers
{
    [ApiController]
    [Route("discount")]
    public class DiscountController : ControllerBase
    {

        private readonly ILogger<DiscountController> _logger;
        private readonly IDiscountServiceQuery queryService;
        public DiscountController(ILogger<DiscountController> logger, IDiscountServiceQuery _queryService)
        {
            _logger = logger;
            queryService = _queryService;
        }
 
        //[HttpGet("/discount/list")]
        //public async Task<IActionResult> GetAll([FromQuery] int From = 1,[FromQuery] int Length = 50)
        //{
        //    var result = await queryService.GetAll(From, Length);
        //    return Ok(result);
        //}

        //[HttpGet]
        //[Route("/discount")]
        //public async Task<IActionResult> Get([FromQuery] int discountId )
        //{
        //    var results = await queryService.Get(discountId);
        //    return Ok(results);
        //}

    }
}