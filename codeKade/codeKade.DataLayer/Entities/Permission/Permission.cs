
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using codeKade.DataLayer.Entities.Common;


namespace codeKade.DataLayer.Entities.Permissions
{
    public class Permission : BaseEntity
    {
        [Display(Name ="عنوان دسترسی")]
        [Required(ErrorMessage ="لطفا {0} را وارد کنید")]
        [MaxLength(200,ErrorMessage ="{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string PermissionTitle { get; set; }

        public long? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public List<Permission> Permissions { get; set; }

        public List<RolePermission> RolePermissions { get; set; }
    }
}
