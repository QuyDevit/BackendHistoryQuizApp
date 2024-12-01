using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace HistoryQuizApp.Security
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public CustomAuthorizeAttribute(string role)
        {
            _role = role;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var allowAnonymous = filterContext.ActionDescriptor.EndpointMetadata
                       .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            // Bỏ qua kiểm tra nếu action có thuộc tính AllowAnonymous
            if (allowAnonymous)
            {
                return;
            }

            var cookieAuth = filterContext.HttpContext.RequestServices.GetRequiredService<CookieAuth>();

            var token = filterContext.HttpContext.Request.Cookies["accesstoken"];
            if (string.IsNullOrEmpty(token) || !cookieAuth.ValidateToken(token, out string userRole))
            {
                // Nếu token không hợp lệ hoặc không tồn tại, trả về 403 Forbidden
                filterContext.Result = new ForbidResult();
                return;
            }
            if (userRole == "Admin")
            {
                return;
            }

            if (_role != userRole)
            {
                filterContext.Result = new ForbidResult();
            }
        }
    }
}
