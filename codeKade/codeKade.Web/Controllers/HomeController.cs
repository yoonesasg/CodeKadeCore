using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Comment;
using codeKade.DataLayer.Entities.Cooperation;
using codeKade.DataLayer.Migrations;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Controllers
{
    public class HomeController : SiteBaseController
    {
        #region Constructor

        private readonly IEventService _eventService;
        private readonly ICommentService _commentService;
        private readonly IBlogService _blogService;
        private readonly ICooperationService _cooperationService;

        public HomeController(IEventService eventService, ICommentService commentService, IBlogService blogService, ICooperationService cooperationService)
        {
            _eventService = eventService;
            _commentService = commentService;
            _blogService = blogService;
            _cooperationService = cooperationService;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index()
        {
            ViewBag.IndexComment = await _commentService.GetIndexComments();
            ViewBag.ActiveEvents = await _eventService.GetNewEvents();
            ViewBag.NewBlogs = await _blogService.GetNewBlogs();
            return View();
        }

        #endregion

        #region Cooperation

        [Authorize]
        public async Task<IActionResult> Cooperation()
        {
            if (await _cooperationService.CheckCooperation(User.GetUserID()))
            {
                ViewBag.IsSent = true;
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cooperation(Cooperation add)
        {
            if (add != null)
            {
                add.UserId = User.GetUserID();
                await _cooperationService.AddCooperation(add);
                TempData[SuccessMessage] = "درخواست شما با موفقیت ثبت شد";
                return Redirect("/");
            }
            return View(add);
        }

        #endregion
    }
}