using System.ComponentModel.DataAnnotations;
using codeKade.DataLayer.Entities.Common;
using codeKade.DataLayer.Entities.Permissions;


namespace codeKade.DataLayer.Entities.User
{
    public class Role : BaseEntity
    {
        public Role()
        {

        }

        [Display(Name ="عنوان")]
        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        [MaxLength(200,ErrorMessage ="{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string RoleTitle { get; set; }

        #region Relations
        public virtual List<UserRole> UserRoles { get; set; }

        public List<RolePermission> RolePermissions { get; set; }
        #endregion
    }
}
