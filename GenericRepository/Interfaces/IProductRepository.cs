using Models;

namespace Repositorys.Interfaces
{
    public interface IProductRepository
    {
        public Task<bool> AddRangeAndCleanProduct(List<Product> entity);

    }
}

