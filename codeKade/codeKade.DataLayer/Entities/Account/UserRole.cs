using codeKade.DataLayer.Entities.Common;


namespace codeKade.DataLayer.Entities.User
{
    public class UserRole : BaseEntity
    {
        #region Constructor
        public UserRole()
        {

        }
        #endregion

        public long UserId { get; set; }

        public long RoleId { get; set; }

        #region Relations
        public virtual codeKade.DataLayer.Entities.Account.User User { get; set; }

        public virtual Role Role { get; set; }
        #endregion
    }
}
