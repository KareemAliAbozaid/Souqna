using Microsoft.Extensions.Caching.Memory;
using Souqna.API.Helper;
using System.Net;
using System.Text.Json;

namespace Souqna.API.Middleware
{
    public class ExptionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _ratelimitWindow = TimeSpan.FromMinutes(1);
        public ExptionsMiddleware(RequestDelegate next, IHostEnvironment env, IMemoryCache memoryCache)
        {
            _next = next;
            _env = env;
            _memoryCache = memoryCache;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                    if (!IsRequstAllowed(context))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                        context.Response.ContentType = "application/json";
    
                        var errorResponse = new ApiExptions((int)HttpStatusCode.TooManyRequests, "Too many requests. Please try again later.");
    
                        var json = JsonSerializer.Serialize(errorResponse);
                        await context.Response.WriteAsJsonAsync(json);
                        return;
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = _env.IsDevelopment() ? new ApiExptions((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace) :
                    new ApiExptions((int)HttpStatusCode.InternalServerError, ex.Message);

                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsJsonAsync(json);
            }
        }
        private bool IsRequstAllowed(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"RateLimit_{ipAddress}";
            var dateNow = DateTime.Now;

            var (timestamp, requestCount) = _memoryCache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _ratelimitWindow;
                return (timestamp: dateNow, requestCount: 0);
            });

            if (dateNow - timestamp < _ratelimitWindow)
            {
                if (requestCount >= 8)
                {
                    return false;
                }
                _memoryCache.Set(cacheKey, (timestamp, requestCount + 1), _ratelimitWindow);
            }
            else
            {
                _memoryCache.Set(cacheKey, (timestamp, requestCount), _ratelimitWindow);
            }

            return true;
        }
    }
}
