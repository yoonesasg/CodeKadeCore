using codeKade.DataLayer.Entities.Common;
using codeKade.DataLayer.Entities.Permissions;
using codeKade.DataLayer.Entities.User;

namespace codeKade.DataLayer.Entities.Permissions
{
    public class RolePermission : BaseEntity
    {
        public long RoleId { get; set; }

        public long PermissionId { get; set; }

        public Role Role { get; set; }

        public Permission Permission { get; set; }
    }
}
