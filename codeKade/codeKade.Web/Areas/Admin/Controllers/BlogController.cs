using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Blog;
using codeKade.DataLayer.Entities.Blog;
using codeKade.Web.HttpContext;
using Microsoft.AspNetCore.Mvc;

namespace codeKade.Web.Areas.Admin.Controllers
{
    public class BlogController : AdminBaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [PermissionChecker(21)]
        public async Task<IActionResult> Index(FilterBlogDTO filter)
        {
            var blogs = await _blogService.GetAll(filter);
            return View(blogs);
        }

        [PermissionChecker(24)]
        public async Task<IActionResult> DeletedBlogs(FilterBlogDTO filter)
        {
            return View(await _blogService.GetDeletedBlogs(filter));
        }

        [PermissionChecker(22)]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = await _blogService.GetCategories();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogDTO add)
        {
            add.UserId = User.GetUserID();
            if (!ModelState.IsValid)
            {
                TempData[ErrorMessage] = "لطفا اطلاعات را به درستی وارد کنید";
                ViewBag.Categories = await _blogService.GetCategories();
                return View();
            }

            if (await _blogService.AddBlog(add))
            {

                TempData[SuccessMessage] = "مقاله با موفقیت افزوده شد";
                return RedirectToAction("Index","Blog" ,new {Area="Admin"});
            }

            return View();
        }

        public async Task<IActionResult> Edit(long id)
        {
            ViewBag.Categories = await _blogService.GetCategories();
            ViewBag.Id = id;
            var blog = await _blogService.GetBlogDetail(id);

            var model = new EditBlogDTO()
            {
                UserId = blog.UserId,
                BlogCategoryId = blog.BlogCategoryId,
                Body = blog.Body,
                Title = blog.Title,
                ImageName = blog.ImageName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogDTO edit)
        {
            if (edit == null)
            {
                TempData[ErrorMessage] = "مشکلی در انجام فرایند ایجاد شد";
                return RedirectToAction("Index", "Blog", new { Area = "Admin" });
            }
            var res = await _blogService.EditBlog(edit);
            if (res == true)
            {
                TempData[SuccessMessage] = "مقاله با موفقیت ویرایش شد";
                return RedirectToAction("Index", "Blog", new { Area = "Admin" });
            }
            ViewBag.Categories = await _blogService.GetCategories();
            ViewBag.Id = edit.ID;
            return View(edit);
        }

        [PermissionChecker(25)]
        public async Task<IActionResult> Delete(long id)
        {
            var res = await _blogService.DeleteBlog(id);
            if (res)
            {
                return Json(id);
            }
            else
            {
                return null;
            }
        }

        [PermissionChecker(24)]
        public async Task<IActionResult> Restore(long id)
        {
            var res = await _blogService.RestoreBlog(id);
            if (res)
            {
                TempData[SuccessMessage] = "مقاله با موفقیت بازگشت داده شد";
                return RedirectToAction("DeletedBlogs", "Blog", new { Area = "Admin" });
            }

            TempData[ErrorMessage] = "مشکلی در فرایند ایجاد شد";
            return RedirectToAction("DeletedBlogs", "Blog", new { Area = "Admin" });
        }

        #region Blog Controller

        public async Task<IActionResult> Categories()
        {
            var model = await _blogService.GetCategories();
            return View(model);
        }

        public IActionResult AddCategory()
        {
            return PartialView("BlogCategory/_AddBlogCategory");
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(BlogCategory category)
        {
            await _blogService.AddCategory(category);
            TempData[SuccessMessage] = "دسته بندی با موفقیت افزوده شد";
            return RedirectToAction("Categories", "Blog", new { Area = "Admin" });
        }

        public async Task<IActionResult> EditCategory(long id)
        {
            if (id == 0)
            {
                TempData[ErrorMessage] = "مشکلی در فرایند ایجاد شد";
                return RedirectToAction("Categories", "Blog", new { Area = "Admin" });
            }
            var model = await _blogService.GetCategoryById(id);
            if (model == null)
            {
                TempData[ErrorMessage] = "مشکلی در فرایند ایجاد شد";
                return RedirectToAction("Categories", "Blog", new { Area = "Admin" });
            }

            return PartialView("BlogCategory/_EditBlogCategory",model);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(BlogCategory blog)
        {
            await _blogService.EditBlogCategory(blog);
            TempData[SuccessMessage] = "دسته بندی با موفقیت ویرایش شد";
            return RedirectToAction("Categories", "Blog", new { Area = "Admin" });
        }

        public async Task<IActionResult> DeleteBlogCategory(long id)
        {
            var model = await _blogService.GetCategoryById(id);
            return PartialView("BlogCategory/_DeleteBlogCategory", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlogCategory(BlogCategory blog)
        {
            if (blog.ID == 0)
            {
                TempData[ErrorMessage] = "مشکلی در فرایند ایجاد شد";
                return RedirectToAction("Categories", "Blog", new { Area = "Admin" });
            }

            await _blogService.DeleteCategory(blog.ID);
            TempData[SuccessMessage] = "دسته بندی با موفقیت حذف شد";
            return RedirectToAction("Categories", "Blog", new { Area = "Admin" });
        }
        #endregion
    }
}
