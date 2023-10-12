using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Cooperarion;
using codeKade.DataLayer.Entities.Cooperation;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Areas.Admin.Controllers
{
    public class CooperationController : AdminBaseController
    {
        private readonly ICooperationService _cooperationService;

        public CooperationController(ICooperationService cooperationService)
        {
            _cooperationService = cooperationService;
        }

        public async Task<IActionResult> Index(FilterCooperationDTO filter)
        {
            var model = await _cooperationService.GetAll(filter);
            return View(model);
        }

        public async Task<IActionResult> Show(long id)
        {
            if (id == 0)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index");
            }
            var model = await _cooperationService.GetCooperation(id);
            if (model == null)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index");
            }
            return PartialView("Cooperation/_ShowCooperation", model);
        }

        public async Task<IActionResult> Delete(long id)
        {
            if (id == 0)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index");
            }
            var model = await _cooperationService.GetCooperation(id);
            return PartialView("Cooperation/_DeleteCooperation" , model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Cooperation delete)
        {
            if (delete.ID == 0)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index");
            }

            var cooperation = await _cooperationService.GetCooperation(delete.ID);
            if (cooperation == null)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index");
            }
            await _cooperationService.DeleteCooperation(delete.ID);
            TempData[SuccessMessage] = "درخواست با موفقیت حذف شد";
            return RedirectToAction("Index");
        }
    }
}
