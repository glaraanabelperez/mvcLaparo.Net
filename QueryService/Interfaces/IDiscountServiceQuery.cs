
using QueryService.Models;
using Utils;

namespace QueryService
{
    public interface IDiscountServiceQuery
    {
        Task<DataCollection<DiscountDto>> GetAll(int from, int length);
        Task<DiscountDto> Get(int discountId);

    }
}