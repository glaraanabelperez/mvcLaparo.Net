namespace mvc.Laparoscopy.Models
{
    public class ProductViewModel
    {


        public int Id { get; set; }
        public string? CategoryNameId { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? Fauvorite { get; set; }
        public bool State { get; set; }
        public DateTime DateInit { get; set; }
        public string Image { get; set; } = string.Empty;

    }
}
