using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;

namespace OutsideServer.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizationFilter : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        string? token = context.HttpContext.Request.Headers["Authorization"];
        if (token is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string parsedToken = token.Replace("Bearer ", string.Empty);
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(parsedToken);

        if (Guid.TryParse(jwtToken.Subject, out Guid userId)) context.RouteData.Values.Add("userId", userId);
        else context.Result = new UnauthorizedResult();
    }
}
