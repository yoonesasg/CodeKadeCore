using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.DataLayer.DTOs.Paging;

namespace codeKade.DataLayer.DTOs.School
{
    public class FilterSchoolDTO : BasePaging
    {
        public string Name { get; set; }

        public List<Entities.School.School> Schools { get; set; }
    }
}
