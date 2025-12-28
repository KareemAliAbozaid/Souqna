namespace Souqna.API.Helper
{
    public class ResponseApi
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ResponseApi(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }

    public class ResponseApiResponse<T> : ResponseApi
    {
        public T Data { get; set; }
        public ResponseApiResponse(int statusCode, T data, string? message = null)
            : base(statusCode, message)
        {
            Data = data;
        }
    }
}