using codeKade.Application.Services.Interfaces;
using codeKade.DataLayer.Entities.Permissions;
using codeKade.DataLayer.Entities.User;
using codeKade.DataLayer.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TopLearnCore.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        #region Constructor
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<Permission> _permissionRepository;
        private readonly IGenericRepository<RolePermission> _rolePermissionRepository;
        public RoleService(IGenericRepository<UserRole> userRoleRepository, IGenericRepository<Role> RoleRepository , IGenericRepository<Permission> permissionRepository,IGenericRepository<RolePermission> rolePermissionRepository)
        {
            _userRoleRepository = userRoleRepository;
            _roleRepository = RoleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }
        #endregion


        #region Add
        public async Task AddPermissionsToRole(long roleId, List<long> permissions)
        {
            var rolePermissions = await _rolePermissionRepository.GetEntityQuery().Where(ur => ur.RoleId == roleId).ToListAsync();

            foreach (var userRole in rolePermissions)
            {
                await _rolePermissionRepository.DeleteEntity(userRole.ID);
            }

            foreach (var item in permissions)
            {
                await _rolePermissionRepository.AddEntity(new RolePermission
                {
                    PermissionId = item,
                    RoleId = roleId
                });
                

            }
            await _userRoleRepository.SaveChanges();
        }

        public async Task<List<long>> GetUserRoles(long id)
        {
            var userRoles = await _userRoleRepository.GetEntityQuery().Where(ur => ur.UserId == id).ToListAsync();

            var selected = new List<long>();

            foreach (var role in userRoles)
            {
                
                selected.Add(role.RoleId);
            }

            return selected;
        }

        public async Task<long> AddRole(Role role)
        {
            await _roleRepository.AddEntity(role);
            await _roleRepository.SaveChanges();
            return role.ID;
        }

        public async Task AddUserRoles(List<long> Roles, long userId)
        {
            var userRoles = await _userRoleRepository.GetEntityQuery().Where(ur => ur.UserId == userId).ToListAsync();

            foreach (var userRole in userRoles)
            {
                await _userRoleRepository.DeleteEntity(userRole.ID);
            }

            foreach(var item in Roles)
            {
                await _userRoleRepository.AddEntity(new UserRole
                {
                    RoleId = item,
                    UserId = userId
                });
                
            }
            await _userRoleRepository.SaveChanges();
        }
        #endregion


        #region ChechPermission
        public async Task<bool> CheckPermission(long permissionId, long userName)
        {
            var permission = await _userRoleRepository.GetEntityQuery().Where(s => s.UserId == userName).Select(r=>r.RoleId).ToListAsync();
            if (!permission.Any())
            {
                return false;
            }
            var RolePermissions = await _rolePermissionRepository.GetEntityQuery().Where(r => r.PermissionId == permissionId).Select(r => r.RoleId).ToListAsync();

            return RolePermissions.Where(p => permission.Contains(p)).Any();
        }

        public async Task<bool> AddRolesToUser(long userId, List<long> roles)
        {
            var userRoles = await _userRoleRepository.GetEntityQuery().Where(ur => ur.UserId == userId).ToListAsync();

            foreach (var userRole in userRoles)
            {
                await _userRoleRepository.DeleteEntity(userRole.ID);
            }

            foreach (var role in roles)
            {
                await _userRoleRepository.AddEntity(new UserRole()
                {
                    UserId = userId,
                    RoleId = role
                });
            }

            await _userRoleRepository.SaveChanges();
            return true;
        }

        #endregion


        #region Delete
        public async Task DeleteRole(long id)
        {
            await _roleRepository.DeleteEntity(id);
            await _roleRepository.SaveChanges();
        }

        #endregion


        #region Dispose
        public async ValueTask DisposeAsync()
        {
            if (_roleRepository != null)
            {
                await _roleRepository.DisposeAsync();
            }
            if (_rolePermissionRepository != null)
            {
                await _rolePermissionRepository.DisposeAsync();
            }
            if (_permissionRepository != null)
            {
                await _permissionRepository.DisposeAsync();
            }
            if (_userRoleRepository != null)
            {
                await _userRoleRepository.DisposeAsync();
            }
        }
        #endregion


        #region Edit
        public async Task EditRole(Role role)
        {
            var getRole = await _roleRepository.GetByID(role.ID);
            if (role != null)
            {
                getRole.RoleTitle = role.RoleTitle;
                _roleRepository.EditEntity(getRole);
                await _roleRepository.SaveChanges();
            }
        }

        public async Task EditRolePermissions(long roleId, List<long> newPermissions)
        {
            var permissions = await _rolePermissionRepository.GetEntityQuery().Where(r => r.RoleId == roleId).ToListAsync();
            if (permissions != null)
            {
                foreach(var item in permissions)
                {
                    await _rolePermissionRepository.DeletePermanentEntity(item.ID);
                }
                foreach(var item in newPermissions)
                {
                    await _rolePermissionRepository.AddEntity(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId  = item
                    });
                }
            }
            else
            {
                foreach (var item in newPermissions)
                {
                    await _rolePermissionRepository.AddEntity(new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = item
                    });
                }
            }
        }

        public async Task EditUserRoles(List<long> Roles, long id)
        {
            var UserRoles =  _userRoleRepository.GetEntityQuery().Where(r => r.UserId == id).ToList();
            foreach(var item in UserRoles)
            {
                await _userRoleRepository.DeletePermanentEntity(item.ID);
            }

            if (Roles != null)
            {
                foreach (var item in Roles)
                {
                    await _userRoleRepository.AddEntity(new UserRole
                    {
                        RoleId = item,
                        UserId = id
                    });
                }
            }
            await _userRoleRepository.SaveChanges();
            
            
        }
#endregion


        #region Get
        public async Task<List<Permission>> GetAllPermissions()
        {
            return await _permissionRepository.GetEntityQuery().ToListAsync();
        }

        public async Task<Role> GetRoleByID(long RoleId)
        {
            return await _roleRepository.GetByID(RoleId);
        }

        public async Task<List<Role>> GetRoles()
        {
            return await _roleRepository.GetEntityQuery().ToListAsync();
        }

        public async Task<List<long>> PermissionsRole(long roleId)
        {
            return await _rolePermissionRepository.GetEntityQuery().Where(r => r.RoleId == roleId).Select(s=>s.PermissionId).ToListAsync();
        }
        #endregion
    }
}
