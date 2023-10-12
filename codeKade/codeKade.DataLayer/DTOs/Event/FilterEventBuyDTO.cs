using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.DataLayer.DTOs.Paging;
using codeKade.DataLayer.Entities.Event;

namespace codeKade.DataLayer.DTOs.Event
{
    public class FilterEventBuyDTO : BasePaging
    {
        public long EventId { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public Entities.Event.Event Event { get; set; }

        public List<EventBuy> EventBuys { get; set; }
    }
}
