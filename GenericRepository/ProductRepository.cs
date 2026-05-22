using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Models;
using mvc.Laparoscopy.Persistence;
using Repositorys.Interfaces;

namespace Repositorys
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext dbContext, ILogger<ProductRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> AddRangeAndCleanProduct(List<Product> entity)
        {
            using (IDbContextTransaction transac = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _dbContext.Database.ExecuteSqlRawAsync(
                        "DELETE FROM Product"
                    );
                    var dbSet = _dbContext.Set<Product>();
                    dbSet.AddRange(entity);
                    var res = await _dbContext.SaveChangesAsync();

                    await transac.CommitAsync();
                    _logger.LogInformation("Transaction Sql OK");
                    return res > 0 ? true : false;
                }
                catch (System.Exception ex)
                {
                    await transac.RollbackAsync();
                    string value = ((ex.InnerException != null) ? ex.InnerException!.Message : ex.Message);
                    throw;
                }

            }
        }


    }
}

