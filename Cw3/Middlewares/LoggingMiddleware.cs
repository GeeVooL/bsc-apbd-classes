using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly string LogPath = "requestsLogs.txt";

        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            using (var reader = new StreamReader(httpContext.Request.Body))
            {
                var logBuilder = new StringBuilder();
                logBuilder.Append('[');
                logBuilder.Append(DateTime.Now);
                logBuilder.Append("] ");
                logBuilder.Append(httpContext.Request.Method);
                logBuilder.Append(' ');
                logBuilder.Append(httpContext.Request.Path);
                logBuilder.Append(httpContext.Request.QueryString.Value);
                logBuilder.Append('\n');
                logBuilder.Append(await reader.ReadToEndAsync());

                await File.AppendAllTextAsync(LogPath, logBuilder.ToString());
            }

            await _next(httpContext);
        }
    }
}
