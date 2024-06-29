using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.School;
using codeKade.DataLayer.Entities.School;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Areas.Admin.Controllers
{
    public class SchoolController : AdminBaseController
    {
        private readonly ISchoolService _schoolService;
        private readonly IUserService _userService;

        public SchoolController(ISchoolService schoolService, IUserService userService)
        {
            _schoolService = schoolService;
            _userService = userService;
        }

        [PermissionChecker(31)]
        public async Task<IActionResult> Index(FilterSchoolDTO filter)
        {
            var data = await _schoolService.GetAllFilter(filter);
            return View(data);
        }

        [PermissionChecker(32)]
        public IActionResult AddSchool()
        {
            return PartialView("_AddSchool");
        }

        [PermissionChecker(19)]
        public async Task<IActionResult> DeleteSchool(long id)
        {
            var res = await _schoolService.DeleteSchool(id);
            if (res == false)
            {
                return RedirectToAction("Index", "School", new { Area = "Admin" });
            }
            return Json(id);
        }

        [PermissionChecker(34)]
        public async Task<IActionResult> EditSchool(long id)
        {
            var school = await _schoolService.GetSchoolById(id);
            return PartialView("_EditSchool",school);
        }

        [HttpPost]
        public async Task<IActionResult> EditSchool(School school)
        {
            await _schoolService.EditSchool(school);
            TempData[SuccessMessage] = "مکان آموزشی با موفقیت ویرایش شد";
            return RedirectToAction("Index", "School", new { Area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> AddSchool(School school)
        {
            var res = await _schoolService.AddSchool(school);
            

            TempData[SuccessMessage] = "مکان آموزشی با موفقیت افزوده شد";
            return RedirectToAction("Index", "School", new { Area = "Admin" });
        }

        [PermissionChecker(35)]
        public async Task<IActionResult> Students(long id)
        {
            if (id == 0)
            {
                TempData[SuccessMessage] = "انجام فرایند با مشکل مواجه شد";
                return RedirectToAction("Index", "School", new { Area = "Admin" });
            }
            var model = await _userService.GetSchoolStudents(id);
            if (model == null)
            {
                TempData[SuccessMessage] = "انجام فرایند با مشکل مواجه شد";
                return RedirectToAction("Index", "School", new { Area = "Admin" });
            }
            return PartialView("_SchoolStudents", model);
        }
    }
}
