using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeKade.DataLayer.DTOs.Paging;
using codeKade.DataLayer.Entities.Account;
using codeKade.DataLayer.Entities.Common;

namespace codeKade.DataLayer.Entities.Event
{
    public class EventBuy : BaseEntity
    {
        public long UserId { get; set; }

        public long EventId { get; set; }

        public Event Event { get; set; }

        public Account.User User { get; set; }

    }
}
