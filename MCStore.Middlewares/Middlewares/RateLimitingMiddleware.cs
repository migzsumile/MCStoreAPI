using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MCStore.Middlewares.Middlewares
{
    internal class RateLimitEntry
    {
        public int Count;
        public long WindowStartsAtUnix;
    }

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        // limit: 20 requests per 60 seconds
        private const int LIMIT = 20;
        private static readonly TimeSpan WINDOW = TimeSpan.FromSeconds(60);

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow preflight/OPTIONS to continue (CORS middleware should handle headers)
            if (HttpMethods.IsOptions(context.Request.Method))
            {
                await _next(context);
                return;
            }

            var clientId = GetClientIdentifier(context);

            var now = DateTimeOffset.UtcNow;
            var windowStartsAt = now.ToUnixTimeSeconds();

            var entry = _cache.GetOrCreate(clientId, entryCache =>
            {
                entryCache.AbsoluteExpirationRelativeToNow = WINDOW;
                return new RateLimitEntry { Count = 0, WindowStartsAtUnix = windowStartsAt };
            });

            var currentCount = Interlocked.Increment(ref entry.Count);

            var remaining = LIMIT - currentCount;

            context.Response.Headers["X-RateLimit-Limit"] = LIMIT.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remaining).ToString();

            if (currentCount > LIMIT)
            {
                // Calculate retry-after from the cache entry expiration
                // Estimate remaining seconds by reading the cache entry creation time
                // We stored WindowStartsAtUnix when entry was created
                var elapsed = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - entry.WindowStartsAtUnix;
                var retryAfter = (int)Math.Max(0, WINDOW.TotalSeconds - elapsed);

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = retryAfter.ToString();
                context.Response.ContentType = "application/json";

                var payload = new { Message = "Too many requests. Try again later.", RetryAfterSeconds = retryAfter };
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                return;
            }

            await _next(context);
        }

        private static string GetClientIdentifier(HttpContext context)
        {
            // Prefer X-Forwarded-For when behind proxies
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            {
                var first = forwarded.ToString().Split(',').FirstOrDefault()?.Trim();
                if (!string.IsNullOrEmpty(first)) return $"rl_{first}";
            }

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return $"rl_{ip}";
        }
    }
}
