
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using mvc.Laparoscopy.Persistence;
using QueryService.Models;
using Utils;

namespace QueryService
{
    public class DiscountServiceQuery : IDiscountServiceQuery
    {
        private readonly ApplicationDbContext _dbContext;
        public IMapper mapper;
        ILogger _logger;

        public DiscountServiceQuery(ApplicationDbContext dbContext, IMapper _mapper, ILogger<ProductServiceQuery> logger)
        {
            _dbContext = dbContext;
            mapper = _mapper;
            _logger = logger;
        }
        Task<DiscountDto> IDiscountServiceQuery.Get(int discountId)
        {
            throw new NotImplementedException();
        }

        async Task<DataCollection<DiscountDto>> IDiscountServiceQuery.GetAll(int From = 1, int Length = 50)
        {
            var query = await _dbContext.Discount_
                 .OrderBy(x => x.Percentage)
                 .GetPagedAsync(From, Length);

            _logger.LogInformation(query.ToString());

            var result = mapper.Map<DataCollection<DiscountDto>>(query);

            return result;
        }

    }
}