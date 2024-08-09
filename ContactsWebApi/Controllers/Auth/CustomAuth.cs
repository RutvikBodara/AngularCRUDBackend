using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BAL.Contacts.Interface;

namespace ContactsWebApi.Controllers.Auth
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuth : ActionFilterAttribute, IAuthorizationFilter
    {
        //private readonly int _role;
        //private readonly int _role;
        //public CustomAuth(int role)
        //{
        //    _role = role;
        //}
        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //    filterContext.HttpContext.Response.Headers["Expires"] = "-1";
        //    filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
        //    base.OnResultExecuting(filterContext);
        //}
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IBAL_Jwt_Auth_Interface>();
            if (jwtservice == null)
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }
            // Access the HttpContext from the filter context
            var request = filterContext.HttpContext.Request;

            // Retrieve the JWT token from the cookies

            var httpContext = filterContext.HttpContext;
            var token = httpContext.Request.Cookies["JwtToken"];
            foreach (var cookie in filterContext.HttpContext.Request.Cookies)
            {
                Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
            }
            var tokenFrontEnd = httpContext.Request.Headers["jwt"].FirstOrDefault();

            if (string.IsNullOrEmpty(tokenFrontEnd) ||!ValidateToken(token, tokenFrontEnd))
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }
        }
        private bool ValidateToken(string? token1,string? token2)
        {
            if(token1 == token2)
            {
                return true;
            }
           return   true;
        }

    }
}
