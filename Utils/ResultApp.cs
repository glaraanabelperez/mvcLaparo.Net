

namespace Utils
{
    public class ResultApp<T> where T : class?
    {
        public bool? Succeeded { get; set; }
        public ErrorResult? errors { get; set; } 
        public string? message { get; set; }
        public T? objectResult { get; set; } = null;


    }
}
