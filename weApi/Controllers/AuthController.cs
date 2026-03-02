using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QueryService;
using QueryService.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

namespace webApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;
        //private readonly IProductServiceQuery queryService;
        public AuthController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            if (login.UserName == "lara" && login.Password == "123")  // Ejemplo simple de validación
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return Ok();
            }

            return Unauthorized();
        }

        [HttpGet]
        [Route("login/accessdenied")]
        public async Task<IActionResult> GetAccesError()
        {
            return BadRequest();
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}