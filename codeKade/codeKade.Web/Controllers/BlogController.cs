﻿using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Blog;
using codeKade.DataLayer.DTOs.Comment;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Controllers
{
    public class BlogController : SiteBaseController
    {
        #region Constructor

        private readonly IBlogService _blogService;
        private readonly IBlogCommentService _blogCommentService;

        public BlogController(IBlogService blogService, IBlogCommentService blogCommentService)
        {
            _blogService = blogService;
            _blogCommentService = blogCommentService;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(FilterBlogDTO filter)
        {
            var data = await _blogService.GetAll(filter);
            return View(data);
        }

        #endregion

        #region Blog Details

        public async Task<IActionResult> Details(long id)
        {
            if (id == 0)
            {
                TempData[ErrorMessage] = "مقاله مورد نظر یافت نشد";
                return Redirect("/blog");
            }
            
            var data = await _blogService.GetBlogDetail(id);
            if (data == null)
            {
                TempData[ErrorMessage] = "مقاله مورد نظر یافت نشد";
                return Redirect("/");
            }
            await _blogService.AddSeenToBlog(id);
            ViewBag.Comments = await _blogCommentService.GetBlogComments(id);
            ViewBag.MostSeenBlog = await _blogService.GetMostSeenBlog();
            ViewBag.Categories = await _blogService.GetCategories();
            return View(data);
        }

        #endregion

        #region AddComment

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(AddBlogCommentDTO comments)
        {
            if (comments == null)
            {
                return NotFound();
            }
            comments.UserId = User.GetUserID();
            await _blogCommentService.AddComment(comments);
            TempData[SuccessMessage] = "نظر شما با موفقیت ثبت شد";
            return Redirect("/Blog/Details/"+ comments.BLogId);
        }

        #endregion
    }
}
