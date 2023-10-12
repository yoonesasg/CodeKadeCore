using System.ComponentModel.DataAnnotations;
using codeKade.DataLayer.Entities.Common;

namespace codeKade.DataLayer.Entities.Cooperation
{
    public class Cooperation : BaseEntity
    {
        public long UserId { get; set; }

        [Display(Name = "بیوگرافی")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string Biography { get; set; }

        public Account.User User { get; set; }

        public bool Seen { get; set; }

    }
}
