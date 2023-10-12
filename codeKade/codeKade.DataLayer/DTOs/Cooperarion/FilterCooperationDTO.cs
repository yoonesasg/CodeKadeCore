using codeKade.DataLayer.DTOs.Paging;
using codeKade.DataLayer.Entities.Cooperation;

namespace codeKade.DataLayer.DTOs.Cooperarion
{
    public class FilterCooperationDTO : BasePaging
    {
        public string Name { get; set; }

        public List<Cooperation> Cooperations { get; set; }
    }
}
