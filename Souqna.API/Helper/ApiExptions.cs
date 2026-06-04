using System.Text.Json.Serialization;

namespace Souqna.API.Helper
{
    public class ApiExptions : ResponseApi
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Details { get; set; }

        public ApiExptions(int statusCode, string? message = null, string? details = null) : base(statusCode, message)
        {
            Details = details;
        }
    }
}
