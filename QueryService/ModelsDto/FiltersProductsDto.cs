namespace QueryService.Models
{
    public class FiltersProductsDto
    {

        public int? Codigo {get; set; }
        public bool? Fauvorite { get; set; }
        public bool? Discount {get; set; }
        public bool? State { get; set; }
        public string? Search {get; set; }
        public int From { get; set; } = 1;
        public int Length { get; set; } = 50;
        public string? OrderAsce { get; set; }

    }
}
