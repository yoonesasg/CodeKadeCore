using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace codeKade.DataLayer.DTOs.Event
{
    public class EditEventDTO
    {
        public long Id { get; set; }

        [Display(Name = "تاریخ شمسی")]
        public string? StartDateShamsi { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "روز برگزاری")]
        public DateTime StartDate { get; set; }

        [Display(Name = "قیمت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int Price { get; set; }

        [Display(Name = "فعال / غیرفعال")]
        public bool IsActive { get; set; }

        [Display(Name = "محل برگذاری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string LocationName { get; set; }

        [Display(Name = "آدرس محل برگذاری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(70, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string Address { get; set; }
        
        public IFormFile? Image { get; set; }

        [Display(Name = "نام عکس")]
        public string? ImageName { get; set; }
    }
}
