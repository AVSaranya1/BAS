using DataAccessLayer.Model;

namespace WebApi.Middleware;

public class OrgDbContextMiddleware
{
    private readonly RequestDelegate _next;

    public OrgDbContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var orgId = context.User.FindFirst(JwtClaimType.orgGuid)?.Value;
            if (!string.IsNullOrEmpty(orgId))
            {
                context.Items["OrgId"] = orgId;
            }
        }
        await _next(context);
    }
}
