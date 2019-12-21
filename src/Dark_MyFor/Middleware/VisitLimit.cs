using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dark_MyFor.Middleware
{
    /// <summary>
    /// 限制访问时间为 0 点到 6 点
    /// </summary>
    public class VisitLimit
    {
        private readonly RequestDelegate _next;

        public VisitLimit(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            int hour = DateTimeOffset.Now.Hour;
            if (hour >= 0 && hour < 6)
                await _next(context);
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("no this time, baby\r\nwe open at wee hours");
            }
        }
    }
}
