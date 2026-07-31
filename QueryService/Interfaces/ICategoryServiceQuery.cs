using QueryService.Models;

namespace QueryService.Interfaces
{
    public interface ICategoryServiceQuery
    {
        Task<List<CategoryDto>> GetAll();
    }
}