using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using mvc.Laparoscopy.Persistence;
using Repositorys.Interfaces;

namespace Repositorys
{
    public class GenericRepository : IGenericRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> Add<T>(T entity)  where T : class 
        {
            var sql  = await _dbContext.Database.GetDbConnection().GetSchemaAsync("Tables");
            try
                {
                    var dbSet = _dbContext.Set<T>();
                    var res_ = dbSet.Add(entity).Entity;
                    await _dbContext.SaveChangesAsync();
                    return res_;
                   
                }
                catch (System.Exception ex)
                {
                    string value = ((ex.InnerException != null) ? ex.InnerException!.Message : ex.Message);
                    throw;
                }
            
        }

        public async Task<bool> AddRange<T>(List<T> entity) where T : class
        {
            using (IDbContextTransaction transac = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var dbSet = _dbContext.Set<T>();
                    dbSet.AddRange(entity);
                    var res = await _dbContext.SaveChangesAsync();

                    await transac.CommitAsync();
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

        public void Delete<T>(int id) where T : class
        {
            throw new NotImplementedException();
        }

      

        public async Task<T1> Update<T1>(T1 entity) where T1 : class
        {
            using (IDbContextTransaction transac = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var dbSet = _dbContext.Set<T1>();
                    var entyResult = dbSet.Attach(entity).Entity;
                    _dbContext.Entry(entity).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    await transac.CommitAsync();
                    return entyResult;
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

