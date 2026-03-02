using Azure;
using Microsoft.AspNetCore.Mvc;
using mvc.Laparoscopy.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace mvc.Laparoscopy.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ProductsApi");
        }

        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            int length = 50;

            var response =

                await _httpClient.GetFromJsonAsync<PagedResponse<ProductViewModel>>(
                    $"list?Search={search}&From={page}&Length={length}"
                );

            if (response == null || !response.HasItems)
                return View(new PagedResponse<ProductViewModel>());

            return View(response);
        }

    }
}
