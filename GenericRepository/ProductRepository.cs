//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Models;
//using mvc.Laparoscopy.Persistence;
//using Repositorys.Interfaces;

//namespace Repositorys
//{
//    public class ProductRepository : IProductRepository
//    {
//        private readonly ILogger<ProductRepository> _logger;
//        private readonly ApplicationDbContext _dbContext;

//        public ProductRepository(ApplicationDbContext dbContext, ILogger<ProductRepository> logger, IMapper mapper)
//        {
//            _dbContext = dbContext;
//            _logger = logger;
//        }

//        public async Task DelletAll()
//        {
//            try
//            {
//                await _dbContext.Database.ExecuteSqlRawAsync(
//                "DELETE FROM Product");
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
            
//        }
//    }
//}

