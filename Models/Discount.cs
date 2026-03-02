namespace Models
{
    public class Discount
    {
        public int Id { get; set; }
        public int Percentage { get; set; }
        public bool State { get; set; }

        public ICollection<Product>? Products { get; set; } = new List<Product>();

    }
}