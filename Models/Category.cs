namespace Models
{
    public class Category
    {
        public string Name { get; set; }
        public ICollection<Product>? Products { get; set; } = new List<Product>();


    }
}