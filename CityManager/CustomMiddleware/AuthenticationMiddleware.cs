using System.Security.Claims;

namespace CityManager.CustomMiddleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            var token = context.Request.Cookies["Authorization"];

            if (!string.IsNullOrEmpty(authHeader))
            {
                if (!string.IsNullOrEmpty(token) && authHeader == token)
                {
                    context.Response.StatusCode = 200;
                    var claims = new[]
                    {
                        new Claim("name", authHeader.Substring(0, 4)),
                        new Claim(ClaimTypes.Role, "User")
                    };
                    var identity = new ClaimsIdentity(claims, "IdentityBasic");
                    context.User = new ClaimsPrincipal(identity);
                }
                else
                    context.Response.StatusCode = 401;
            }
            else
            {
                authHeader = context.Request.Query["Authorization"];
                if (!string.IsNullOrEmpty(authHeader))
                {
                    if (!string.IsNullOrEmpty(token) && authHeader == token)
                    {
                        context.Response.StatusCode = 200;
                        var claims = new[]
                        {
                            new Claim("name", authHeader.Substring(0, 4)),
                            new Claim(ClaimTypes.Role, "User")
                        };
                        var identity = new ClaimsIdentity(claims, "IdentityBasic");
                        context.User = new ClaimsPrincipal(identity);
                    }
                    else
                        context.Response.StatusCode = 401;
                }
                else
                    context.Response.StatusCode = 401;
            }

            await _next(context);
        }
    }
}