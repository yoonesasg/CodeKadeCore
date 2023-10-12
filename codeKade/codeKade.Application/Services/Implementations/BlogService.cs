using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.Application.PathExtentions;
using codeKade.Application.Security;
using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.DTOs.Blog;
using codeKade.DataLayer.Entities.Blog;
using codeKade.DataLayer.Repository.Interfaces;
using LightElectric.Application.Extensions;
using Microsoft.EntityFrameworkCore;

namespace codeKade.Application.Services.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly IGenericRepository<Blog> _blogRepository;
        private readonly IGenericRepository<BlogCategory> _blogCategoryRepository;

        public BlogService(IGenericRepository<Blog> blogRepository, IGenericRepository<BlogCategory> blogCategoryRepository)
        {
            _blogRepository = blogRepository;
            _blogCategoryRepository = blogCategoryRepository;
        }

        public async ValueTask DisposeAsync()
        {
            if (_blogRepository != null)
            {
                await _blogRepository.DisposeAsync();
            }
        }

        public async Task<List<Blog>> GetNewBlogs()
        {
            return await _blogRepository.GetEntityQuery().OrderByDescending(b => b.ID).Take(3).Include(b => b.BlogCategory).ToListAsync();
        }

        public async Task<FilterBlogDTO> GetAll(FilterBlogDTO filter)
        {
            var query = _blogRepository.GetEntityQuery().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => EF.Functions.Like(s.Title, $"%{filter.Title}%"));
            }


            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.Include(b => b.BlogCategory).Include(b => b.User).OrderBy(s => s.ID).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.Blogs = products;
            return filter;
        }

        public async Task<Blog> GetBlogDetail(long id)
        {
            return await _blogRepository.GetEntityQuery().Include(b => b.BlogCategory).Include(b => b.User).SingleOrDefaultAsync(b => b.ID == id);
        }

        public async Task<List<BlogCategory>> GetCategories()
        {
            return await _blogCategoryRepository.GetEntityQuery().ToListAsync();
        }

        public async Task<List<Blog>> GetMostSeenBlog()
        {
            return await _blogRepository.GetEntityQuery().OrderByDescending(b => b.Seen).Take(5).ToListAsync();
        }

        public async Task AddSeenToBlog(long id)
        {
            var blog = await _blogRepository.GetByID(id);
            blog.Seen += 1;
            _blogRepository.EditEntity(blog);
            await _blogRepository.SaveChanges();
        }

        public async Task<int> CountOfTodayBlog()
        {
            return await _blogRepository.GetEntityQuery().CountAsync(b => b.CreateDate.Date == DateTime.Now.Date);
        }

        public async Task<int> CountOfBlogs()
        {
            return await _blogRepository.GetEntityQuery().CountAsync();
        }

        public async Task<FilterBlogDTO> GetDeletedBlogs(FilterBlogDTO filter)
        {
            var query = _blogRepository.GetEntityQuery().AsQueryable().Include(b=>b.User).IgnoreQueryFilters().Where(b => b.IsDelete);

            if (!string.IsNullOrEmpty(filter.Title))
            {
                query = query.Where(s => EF.Functions.Like(s.Title, $"%{filter.Title}%"));
            }


            filter.SkipEntity = (filter.PageID - 1) * filter.TakeEntity;
            var entitiesCount = await query.CountAsync();
            filter.PageCount = (int)Math.Ceiling(entitiesCount / (double)filter.TakeEntity);

            var products = await query.Include(b => b.BlogCategory).OrderBy(s => s.ID).Skip(filter.SkipEntity).Take(filter.TakeEntity)
                .ToListAsync();
            filter.StartPage = filter.PageID - 3 > 0 ? filter.PageID - 3 : 1;
            filter.EndPage = filter.PageID + 3 <= filter.PageCount ? filter.PageID + 3 : filter.PageCount;
            filter.Blogs = products;
            return filter;
        }

        public async Task<bool> AddBlog(AddBlogDTO add)
        {
            if (add.Image != null && add.Image.IsImage())
            {
                var ImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(add.Image.FileName);
                add.Image.AddImageToServer(ImageName, PathTools.BlogImageUpload, 150, 100, PathTools.BlogThumbImageUpload);
                var blog = new Blog()
                {
                    BlogCategoryId = add.BlogCategoryId,
                    Body = add.Body,
                    IsDelete = false,
                    Title = add.Title,
                    UserId = add.UserId,
                    ImageName = ImageName
                };
                await _blogRepository.AddEntity(blog);
                await _blogRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> DeleteBlog(long id)
        {
            if (id != 0)
            {
                var blog = await _blogRepository.GetByID(id);
                if (blog == null)
                {
                    return false;
                }

                blog.IsDelete = true;
                _blogRepository.EditEntity(blog);
                await _blogRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RestoreBlog(long id)
        {
            if (id != 0)
            {
                var blog = await _blogRepository.GetEntityQuery().IgnoreQueryFilters().SingleOrDefaultAsync(b=>b.ID == id);
                if (blog == null)
                {
                    return false;
                }

                blog.IsDelete = false;
                _blogRepository.EditEntity(blog);
                await _blogRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> EditBlog(EditBlogDTO edit)
        {
            var blog = await _blogRepository.GetByID(edit.ID);

            if (edit.Image == null)
            {
                blog.Title = edit.Title;
                blog.BlogCategoryId = edit.BlogCategoryId;
                blog.Body = edit.Body;
            }
            else
            {
                var ImageName = Guid.NewGuid().ToString("N") + Path.GetExtension(edit.Image.FileName);
                edit.Image.AddImageToServer(ImageName, PathTools.BlogImageUpload, 150, 100, PathTools.BlogThumbImageUpload,blog.ImageName);
                blog.ImageName = ImageName;
                blog.Title = edit.Title;
                blog.BlogCategoryId = edit.BlogCategoryId;
                blog.Body = edit.Body;
            }
            _blogRepository.EditEntity(blog);
            await _blogRepository.SaveChanges();
            return true;
        }

        public async Task AddCategory(BlogCategory category)
        {
            await _blogCategoryRepository.AddEntity(category);
            await _blogCategoryRepository.SaveChanges();
        }

        public async Task<BlogCategory> GetCategoryById(long id)
        {
            return await _blogCategoryRepository.GetByID(id);
        }

        public async Task EditBlogCategory(BlogCategory blog)
        {
            _blogCategoryRepository.EditEntity(blog);
            await _blogCategoryRepository.SaveChanges();
        }

        public async Task DeleteCategory(long id)
        {
            await _blogCategoryRepository.DeleteEntity(id);
            await _blogCategoryRepository.SaveChanges();
        }
    }
}
