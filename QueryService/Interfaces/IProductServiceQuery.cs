
using QueryService.Models;
using Utils;

namespace QueryService
{
    public interface IProductServiceQuery
    {
        Task<DataCollection<ProductDto>> GetAll(bool? Fauvorite, bool? Discount, bool? State,
            string? Search, int From, int Length);

    }
}