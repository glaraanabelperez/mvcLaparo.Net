namespace mvc.Laparoscopy.Models
{
    public class PagedResponse<T>
    {
        public bool HasItems { get; set; }
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
    }
}
