using codeKade.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.HttpContext
{
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private IRoleService _roleService;
        private long _permissionId;
        public PermissionCheckerAttribute(long permissionId)
        {
            _permissionId = permissionId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _roleService = (IRoleService)context.HttpContext.RequestServices.GetService(typeof(IRoleService));

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = Convert.ToInt64(context.HttpContext.User.GetUserClaimByType(HttpContextExtentions.CustomClaimType.NameIdentifier));
                var IsAdmin = _roleService.CheckPermission(_permissionId,userId).Result;
                if (!IsAdmin)
                {
                    context.Result = new RedirectResult("/");
                }

            }
            else
            {
                context.Result = new RedirectResult("/");
            }
        }
    }
}
