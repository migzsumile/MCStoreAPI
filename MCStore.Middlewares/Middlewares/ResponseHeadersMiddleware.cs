using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MCStore.Middlewares.Middlewares
{
    public class ResponseHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000;includeSubdomains;preload");
                //context.Response.Headers.Remove("Server"); not working
                //context.Response.Headers.Remove("X-Powered-By"); not working

                return Task.CompletedTask;
            });

            await _next(context);
        }

    }
}
