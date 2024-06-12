using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderManagmentSytemBAL.Services.Auth;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;

namespace OrderManagmentSystem.Controllers
{
   


    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {

        public CustomAuthorization()
        {
            
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJWTService>();
            if (jwtService == null)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }

            string token=context.HttpContext.Request.Headers["jwtToken"].FirstOrDefault();
            if (token == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!jwtService.Validate(token, out JwtSecurityToken jwtSecurityToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var claimsIdentity = new ClaimsIdentity(jwtSecurityToken.Claims);
            context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);
        }
    }
}
