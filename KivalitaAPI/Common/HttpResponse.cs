using System.Net;

namespace KivalitaAPI.Common
{
    public class HttpResponse<T>
    {
        public HttpStatusCode statusCode { get; set; }
        public bool IsStatusCodeSuccess { get; set; }
        public T data { get; set; }
    }
}
