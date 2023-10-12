using codeKade.DataLayer.Entities.Permissions;
using codeKade.DataLayer.Entities.User;

namespace codeKade.Application.Services.Interfaces
{
    public interface IRoleService : IAsyncDisposable
    {
        #region Role Services
        Task<List<Role>> GetRoles();

        Task AddUserRoles(List<long> Roles, long userId);

        Task EditUserRoles(List<long> Roles, long id);

        Task<List<long>> GetUserRoles(long id); 

        Task<long> AddRole(Role role);

        Task<Role> GetRoleByID(long RoleId);

        Task EditRole(Role role);

        Task DeleteRole(long id);
        #endregion
        #region Permission Services
        Task<List<Permission>> GetAllPermissions();

        Task AddPermissionsToRole(long roleId, List<long> permissions);

        Task<List<long>> PermissionsRole(long roleId);

        Task EditRolePermissions(long roleId, List<long> newPermissions);

        Task<bool> CheckPermission(long permissionId, long uerName);

        Task<bool> AddRolesToUser(long userId,List<long> roles);
        #endregion
    }
}
