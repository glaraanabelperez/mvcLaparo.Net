namespace Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? Description { get; set; } 
        public string? image { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime? DateInit { get; set; }
        public bool State { get; set; }
        public bool? Fauvorite { get; set; }
        public int? DiscountId { get; set; }
        public string? Codigo { get; set; }
        public string? CategoryNameId { get; set; }
        public Discount? Discount_ { get; set; }
        public Category? Category_ { get; set; }


    }
}