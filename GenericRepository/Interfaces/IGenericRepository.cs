namespace Repositorys.Interfaces
{
    public interface IGenericRepository
    {
        public Task<T> Add<T>(T entity) where T : class;
        public Task<bool> AddRange<T>(List<T> entity) where T : class;
        public Task<T> Update<T>(T entity) where T : class;
        public void Delete<T>(int id) where T : class ;

    }
}

