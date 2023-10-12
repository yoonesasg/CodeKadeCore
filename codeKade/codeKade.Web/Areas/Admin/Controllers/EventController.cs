using System.Globalization;
using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Event;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Mvc;
using PersianDate.Standard;

namespace codeKade.Web.Areas.Admin.Controllers
{
    public class EventController : AdminBaseController
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [PermissionChecker(2)]
        public async Task<IActionResult> Index(FilterEventDTO filter)
        {
            await _eventService.DisActivePastEvents(DateTime.Now);
            var events = await _eventService.GetAll(filter);
            return View(events);
        }

        [PermissionChecker(11)]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEventDTO add,string stdate)
        {
            if (!ModelState.IsValid)
            {
                TempData[ErrorMessage] = "لطفا فیلد های اجباری را تکمیل نمایید";
                return View(add);
            }
            if (stdate != "")
            {
                string[] std = stdate.Split('/');
                add.StartDate = new DateTime(
                    int.Parse(std[0]),
                    int.Parse(std[1]),
                    int.Parse(std[2]),
                    new PersianCalendar());
            }

            var res = await _eventService.AddEvent(add);
            if (res)
            {
                TempData[SuccessMessage] = "رویداد با موفقیت افزوده شد";
                return RedirectToAction("Index", "Event", new { Area = "Admin" });
            }

            TempData[ErrorMessage] = "مشکلی در انجام فرایند ایجاد شد";
            return View(add);
        }

        [PermissionChecker(10)]
        public async Task<IActionResult> DeletedEvents(FilterEventDTO filter)
        {
            var events = await _eventService.GetDeletedBlogs(filter);
            return View(events);
        }

        [PermissionChecker(13)]
        public async Task<IActionResult> Delete(long id)
        {
            var res = await _eventService.DeleteEvent(id);
            if (res)
            {
                return Json(id);
            }
            else
            {
                return null;
            }
        }

        [PermissionChecker(10)]
        public async Task<IActionResult> Restore(long id)
        {
            var res = await _eventService.RestoreEvent(id);
            if (res)
            {
                return Json(id);
            }
            else
            {
                return null;
            }
        }

        [PermissionChecker(12)]
        public async Task<IActionResult> Edit(long id)
        {
            var Event = await _eventService.GetForEdit(id);
            Event.StartDateShamsi = Event.StartDate.ToFa();
            return View(Event);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditEventDTO Edit,string stdate)
        {
            if (ModelState.IsValid)
            {
                if (stdate != "")
                {
                    string[] std = stdate.Split('/');
                    Edit.StartDate = new DateTime(
                        int.Parse(std[0]),
                        int.Parse(std[1]),
                        int.Parse(std[2]),
                        new PersianCalendar());
                }

                var res = await _eventService.EditEvent(Edit);
                if (res)
                {
                    TempData[SuccessMessage] = "اطلاعات با موفقیت ویرایش شد";
                    return RedirectToAction("Index", "Event", new { Area = "Admin" });
                }
                else
                {
                    TempData[ErrorMessage] = "مشکلی در ثبت اطلاعات پیش آمد";
                    return View(Edit);
                }
            }

            TempData[ErrorMessage] = "لطفا فیلد های اجباری را پر کنید";
            return View(Edit);
        }

        [HttpGet]
        [PermissionChecker(14)]
        public async Task<IActionResult> EventRegisters(long? id,FilterEventBuyDTO filter)
        {
            if (id == null && filter.EventId == null)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index", "Event", new { Area = "Admin" });
            }

            if (id == null)
            {
                id = filter.EventId;
            }

            ViewBag.EventId = id;
            var data = await _eventService.GetEventBuys(id,filter);
            return View(data);
        }

        [PermissionChecker(10008)]
        public async Task<IActionResult> DeleteEventBuy(long id)
        {
            if (id == null)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index", "Event", new { Area = "Admin" });
            }

            var res = await _eventService.DeleteEventBuy(id);
            if (res == 0)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند پیش آمد";
                return RedirectToAction("Index", "Event", new { Area = "Admin" });
            }

            return Json(id);
        }
    }
}
