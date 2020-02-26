namespace PotigianHH.Controllers.Model
{
    public class Response<T>
    {
        public bool Success { get; set; }

        public T Object { get; set; }

        public ErrorInfo ErrorInfo { get; set; } 
    }

    public class ErrorInfo
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
