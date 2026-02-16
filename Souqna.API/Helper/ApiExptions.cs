namespace Souqna.API.Helper
{
    public class ApiExptions : ResponseApi
    {
        public ApiExptions(int statusCode, string? message = null, string? details = null) : base(statusCode, message)
        {
            Details = details;
        }

        public string? Details { get; set; }
    }
}
