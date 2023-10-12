using codeKade.DataLayer.DTOs.Paging;
using codeKade.DataLayer.Entities.Account;

namespace codeKade.DataLayer.DTOs.School
{
    public class FilterStudentsDTO : BasePaging
    {
        public List<User> Students { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }
    }
}
