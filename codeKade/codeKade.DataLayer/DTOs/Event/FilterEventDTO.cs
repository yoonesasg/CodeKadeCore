using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.DataLayer.DTOs.Paging;

namespace codeKade.DataLayer.DTOs.Event
{
    public class FilterEventDTO : BasePaging
    {
        public string Title { get; set; }

        public List<Entities.Event.Event> Events { get; set; }
    }
}
