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
        private readonly int _role;
        public CustomAuth(int role)
        {
            _role = role;
        }
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
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }

            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["Jwt"];

            if (token == null || !jwtservice.ValidateToken(token, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("~/Login/");
                return;
            }
            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }
            if (_role != 0 && int.Parse(roles.Value) != _role)
            {
                filterContext.Result = new RedirectResult("~/Home/AccessDenied");
            }
        }
    }
}
