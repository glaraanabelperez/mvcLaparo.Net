using Models;

namespace QueryService.Models
{
    public class DiscountDto
    {
        public int Id { get; set; }
        public int Percentage { get; set; }
        public bool State { get; set; }

        public ICollection<ProductDto>? Products { get; set; } = new List<ProductDto>();
    }
}