using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Comment;
using codeKade.DataLayer.DTOs.Event;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Controllers
{
    public class EventController : SiteBaseController
    {
        #region Constructor

        private readonly IEventService _eventService;
        private readonly ICommentService _commentService;

        public EventController(IEventService eventService, ICommentService commentService)
        {
            _eventService = eventService;
            _commentService = commentService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(FilterEventDTO filter)
        {
            var events = await _eventService.GetAll(filter);
            ViewBag.bigEvent = await _eventService.GetBigEvent();
            return View(events);
        }
        #endregion

        #region Details

        public async Task<IActionResult> Details(long id)
        {
            if (id == 0)
            {
                TempData[ErrorMessage] = "رویداد مورد نظر یافت نشد";
                return Redirect("/event");
            }
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.UserBuy = await _eventService.IsUserHaveEvent(User.GetUserID(), id);
            }
            var single_event = await _eventService.GetSingleEvent(id);
            ViewBag.Comments = await _commentService.GetEventComments(id);
            await _eventService.AddSeenToEvent(id);
            return View(single_event);
        }

        #endregion

        #region AddComment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(AddcommentDTO comments)
        {
            await _commentService.AddComment(comments.Text, User.GetUserID(), comments.EventId, comments.ParentId);
            TempData[SuccessMessage] = "نظر شما با موفقیت ثبت شد";
            return Redirect("/event/Details/" + comments.EventId);
        }
        #endregion

        #region BuyEvent

        [Authorize]
        public async Task<IActionResult> BuyEvent(long EventId)
        {
            if (EventId == 0)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند ایجاد شد";
                return Redirect("/event");
            }
            var Event = await _eventService.GetSingleEvent(EventId);
            if (Event == null)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند ایجاد شد";
                return Redirect("/event/Details/" + EventId);
            }

            if (Event.Price == 0)
            {
                var res = await _eventService.BuyEvent(EventId, User.GetUserID());
                if (res)
                {
                    TempData[SuccessMessage] = "ثبت نام شما با موفقیت انجام شد";
                    return Redirect("/event/Details/" + EventId);
                }
                else
                {
                    TempData[ErrorMessage] = "مشکلی در انجام فرایند ایجاد شد";
                    return Redirect("/event/Details/" + EventId);
                }
            }
            else
            {
                #region Online Payment
                var payment = new ZarinpalSandbox.Payment(Event.Price);
                var res = payment.PaymentRequest("شارژ کیف پول", BaseURL + "event/CallBack/" + EventId);
                if (res.Result.Status == 100)
                {
                    return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
                }
                #endregion
            }
            TempData[ErrorMessage] = "مشکلی در انجام فرایند ایجاد شد";
            return Redirect("/event/Details/" + EventId);
        }

        [Authorize]
        public async Task<IActionResult> CallBack(long id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];
                var Event = await _eventService.GetSingleEvent(id);
                var payment = new ZarinpalSandbox.Payment(Event.Price);
                var res = payment.Verification(authority).Result;
                if (res.Status == 100)
                {
                    ViewBag.code = res.RefId;
                    ViewBag.IsSuccess = true;
                    await _eventService.BuyEvent(id, User.GetUserID());
                }

            }
            TempData[SuccessMessage] = "ثبت نام شما با موفقیت انجام شد";
            return Redirect("/event/Details/" + id);
        }
        #endregion
    }


}
