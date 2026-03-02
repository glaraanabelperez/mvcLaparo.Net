using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using mvc.Laparoscopy.Persistence;
using QueryService.Models;
using System.Linq;
using Utils;

namespace QueryService
{
    public class ProductServiceQuery : IProductServiceQuery
    {
        private readonly ApplicationDbContext _dbContext;
        public IMapper mapper;
        ILogger _logger;

        public ProductServiceQuery(ApplicationDbContext dbContext, IMapper _mapper, ILogger<ProductServiceQuery> logger) { 
            _dbContext = dbContext;
            mapper = _mapper;
            _logger = logger;
        }

        public async Task<DataCollection<ProductDto>> GetAll( bool? Fauvorite, bool? Discount, bool? State,
            string? Search, int From =1, int Length =50)
        {
            var query = await _dbContext.Products
                //.Include(x => x.Category)
                //.Include(x => x.Discount_)
                .Where(x => (Search == null || string.IsNullOrEmpty(Search) || x.Name.Contains(Search))
                            && (Discount == null || Discount == false || (Discount == true && x.DiscountId != null))
                            && (Fauvorite == null || Fauvorite == false || (Fauvorite == true && x.Fauvorite == true))
                            && (State == null || State == false || (State == true && x.State == true))
                        )
                 .OrderBy(x => x.Name)
                 .GetPagedAsync(From, Length);

            _logger.LogInformation(query.ToString());

            var result = mapper.Map<DataCollection<ProductDto>>(query);

            return result;
        }
    
    }
}