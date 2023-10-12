using codeKade.DataLayer.Entities.Account;
using codeKade.DataLayer.Entities.Blog;
using codeKade.DataLayer.Entities.Comment;
using codeKade.DataLayer.Entities.Cooperation;
using codeKade.DataLayer.Entities.Event;
using codeKade.DataLayer.Entities.Permissions;
using codeKade.DataLayer.Entities.School;
using codeKade.DataLayer.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace codeKade.DataLayer.Context
{
    public class ApplicationDbContext : DbContext
    {
        #region Constructor


        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        #endregion

        #region User

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        #endregion

        #region Permission

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        #endregion

        #region Cooperation

        public DbSet<Cooperation> Cooperations { get; set; }

#endregion

        #region Event

        public DbSet<Event> Events { get; set; }

        public DbSet<EventBuy> EventBuys { get; set; }

        #endregion

        #region Comment

        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }

        #endregion

        #region Blog

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }

        #endregion

        #region School

        public DbSet<School> Schools { get; set; }

        #endregion

        #region Query Filter

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Blog>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Event>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<BlogComment>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Comment>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<EventBuy>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<School>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<BlogCategory>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Role>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<UserRole>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Permission>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<RolePermission>().HasQueryFilter(u => !u.IsDelete);
            modelBuilder.Entity<Cooperation>().HasQueryFilter(u => !u.IsDelete);
        }
        #endregion
    }


}
