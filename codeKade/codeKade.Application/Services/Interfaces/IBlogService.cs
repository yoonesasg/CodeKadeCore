using codeKade.DataLayer.DTOs.Blog;
using codeKade.DataLayer.Entities.Blog;

namespace codeKade.Application.Services.Interfaces;

public interface IBlogService : IAsyncDisposable
{
    Task<List<Blog>> GetNewBlogs();

    Task<FilterBlogDTO> GetAll(FilterBlogDTO filter);

    Task<Blog> GetBlogDetail(long id);

    Task<List<BlogCategory>> GetCategories();

    Task<List<Blog>> GetMostSeenBlog();

    Task AddSeenToBlog(long id);

    Task<int> CountOfTodayBlog();

    Task<int> CountOfBlogs();

    Task<FilterBlogDTO> GetDeletedBlogs(FilterBlogDTO filter);

    Task<bool> AddBlog(AddBlogDTO add);

    Task<bool> DeleteBlog(long id);

    Task<bool> RestoreBlog(long id);

    Task<bool> EditBlog(EditBlogDTO edit);

    Task AddCategory(BlogCategory category);

    Task<BlogCategory> GetCategoryById(long id);

    Task EditBlogCategory(BlogCategory blog);

    Task DeleteCategory(long id);
}