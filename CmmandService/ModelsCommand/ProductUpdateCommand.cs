
namespace CmmandService.ModelsCommand
{
    public class ProductUpdateCommand 
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; } 
        public bool? State { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Name { get; set; } 

    }
}