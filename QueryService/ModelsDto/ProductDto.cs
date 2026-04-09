using Models;

namespace QueryService.Models
{
    public class ProductDto
    {
        public int? Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Codigo { get; set; }

        public int? DiscountId { get; set; }
        public string Description { get; set; }
        public bool? Fauvorite { get; set; }
        public bool State { get; set; }
        public DateTime? DateInit { get; set; }
        public string? image { get; set; }
        //public decimal? Price { get; set; }
        //public decimal? TotalPrice { get; set; }
        public string Name { get; set; }


    }
}