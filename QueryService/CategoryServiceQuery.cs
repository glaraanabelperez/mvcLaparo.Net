using Microsoft.EntityFrameworkCore;
using mvc.Laparoscopy.Persistence;
using QueryService.Interfaces;
using QueryService.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryService
{
    public class CategoryServiceQuery : ICategoryServiceQuery
    {
        private readonly ApplicationDbContext _db;

        public CategoryServiceQuery(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            return await _db.Category
                .Select(c => new CategoryDto { Name = c.Name})
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}