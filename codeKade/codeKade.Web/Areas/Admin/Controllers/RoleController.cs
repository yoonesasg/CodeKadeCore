using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.Entities.User;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Areas.Admin.Controllers
{
    public class RoleController : AdminBaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [PermissionChecker(10003)]
        public async Task<IActionResult> Index()
        {
            var model = await _roleService.GetRoles();
            return View(model);
        }

        [PermissionChecker(10004)]
        public IActionResult AddRole()
        {
            return PartialView("_AddRole");
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(Role role)
        {
            var res = await _roleService.AddRole(role);

            if (res == 0)
            {
                TempData[ErrorMessage] = "انجام فرایند با مشکل مواجه شد";
                return RedirectToAction("Index", "Role", new { Area = "Admin" });
            }

            TempData[SuccessMessage] = "نقش با موفقیت افزوده شد";
            return RedirectToAction("Index", "Role", new { Area = "Admin" });
        }

        [PermissionChecker(10006)]
        public async Task<IActionResult> EditRole(long id)
        {
            ViewBag.RoleId = id;
            var role = await _roleService.GetRoleByID(id);
            return PartialView("_EditRole",role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(Role role)
        {
            if (role == null || role.ID == 0)
            {
                TempData[ErrorMessage] = "انجام فرایند با مشکل مواجه شد";
                return RedirectToAction("Index", "Role", new { Area = "Admin" });
            }
            await _roleService.EditRole(role);

            TempData[SuccessMessage] = "نقش با موفقیت ویرایش شد";
            return RedirectToAction("Index", "Role", new { Area = "Admin" });
        }

        [PermissionChecker(10007)]
        public async Task<IActionResult> DeleteRole(long id)
        {
            if (id != 0)
            {
                await _roleService.DeleteRole(id);
                return Json(id);
            }
            else
            {
                TempData[ErrorMessage] = "انجام فرایند با مشکل مواجه شد";
                return RedirectToAction("Index", "Role", new { Area = "Admin" });
            }
        }

        [PermissionChecker(10005)]
        public async Task<IActionResult> Permissions(long id)
        {
            ViewBag.RoleId = id;
            ViewBag.RolePermissions = await _roleService.PermissionsRole(id);
            var model = await _roleService.GetAllPermissions();
            return PartialView("_PermissionsList",model);
        }

        [HttpPost]
        [PermissionChecker(10005)]
        public async Task<IActionResult> Permissions(long ID, List<long> SelectedPermission)
        {
            await _roleService.AddPermissionsToRole(ID, SelectedPermission);
            TempData[SuccessMessage] = "دسترسی با موفقیت ویرایش شد";
            return RedirectToAction("Index", "Role", new { Area = "Admin" });
        }
    }
}
