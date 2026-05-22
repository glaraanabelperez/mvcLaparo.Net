using CmmandService.ModelsCommand;
using Microsoft.AspNetCore.Http;
using Models;
using Utils;

namespace CmmandService.Interfaces
{
    public interface IProductCommandService
    {
        public Task<ResultApp<Product?>> ChargeData(IFormFile filePath);
        public Task<ResultApp<Product?>> Add(ProductCreateCommand command);

    }
}