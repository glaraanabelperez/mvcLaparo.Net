
namespace CmmandService.ModelsCommand
{
    public class ProductCreateCommand
    {
        public string? Category { get; set; }
        public string? Description { get; set; } 
        public bool State { get; set; }
        public decimal? Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Name { get; set; }
        public string? Codigo { get; set; }
        public int? DiscountId { get; set; }
        public bool? Fauvorite { get; set; }
        public DateTime? DateInit { get; set; }
        public string? image { get; set; }

    }
}