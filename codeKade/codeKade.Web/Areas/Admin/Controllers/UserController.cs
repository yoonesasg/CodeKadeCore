using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Account;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Areas.Admin.Controllers
{
    public class UserController : AdminBaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [PermissionChecker(36)]
        public async Task<IActionResult> Index(FilterUserDTO filter)
        {
            return View(await _userService.GetUsersList(filter));
        }

        [PermissionChecker(36)]
        public async Task<IActionResult> GetUserDetail(long id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return PartialView("_UserDetail", await _userService.GetById(id));
        }

        [PermissionChecker(37)]
        public async Task<IActionResult> AddUser()
        {
            return PartialView("_AddUser");
        }

        [PermissionChecker(38)]
        public async Task<IActionResult> EditUser(long id)
        {
            var user = await _userService.GetById(id);
            var editDetail = new EditProfileDTO()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Mobile = user.Mobile,
                Id = user.ID
            };
            return PartialView("_EditUser", editDetail);
        }

        [PermissionChecker(39)]
        public async Task<IActionResult> UserRoles(long id)
        {
            ViewBag.UserId = id;

            var roles = await _roleService.GetRoles();

            ViewBag.selectedroles = await _roleService.GetUserRoles(id);


            return PartialView("_UserRoles",roles);
        }

        [HttpPost]
        public async Task<IActionResult> UserRoles(long ID,List<long> SelectedRole)
        {
            if (ID == 0 || SelectedRole == null)
            {
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            var res = await _roleService.AddRolesToUser(ID, SelectedRole);

            if (res)
            {
                TempData[SuccessMessage] = "نقش ها با موفقیت ویرایش شد";
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditProfileDTO edit)
        {
            var res = await _userService.EditUser(edit);
            if (res)
            {
                TempData[SuccessMessage] = "اطلاعات با موفقیت ویرایش شد";
            }

            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterUserDTO register)
        {
            var res = await _userService.AdminRegisterUser(register);
            if (res == RegisterUserResult.EmailConflict)
            {
                TempData[ErrorMessage] = "ایمیل وارد شده تکراری میباشد";
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            TempData[SuccessMessage] = "کاربر جدید با موفقیت افزوده شد";
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [PermissionChecker(40)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            var res = await _userService.DeleteUser(id);
            if (res == 0)
            {
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            else
            {
                TempData[SuccessMessage] = "کاربر با موفقیت حذف شد";
                return Json(id);
            }
        }


        [PermissionChecker(36)]
        public async Task<IActionResult> TodayUsers(FilterUserDTO filter)
        {
            var users = await _userService.GetTodayUsers(filter);
            return View(users);
        }

        [PermissionChecker(41)]
        public async Task<IActionResult> DeletedUsers(FilterUserDTO filter)
        {
            var users = await _userService.GetDeletedUsers(filter);

            return View(users);
        }

        [PermissionChecker(42)]
        public async Task<IActionResult> ReturnDeletedUser(long id)
        {
            var res = await _userService.ReturnDeletedUser(id);
            if (res)
            {
                TempData[SuccessMessage] = "کاربر با موفقیت بازگشت داده شد";
            }
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }
    }
}
