using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class MainSlider
    {
        public MainSlider()
        {

        }

        public int id { get; set; }
        [Display(Name ="تصویر")]
        public string imagename { get; set; }
        [Required(ErrorMessage ="لطفا تیتر اسلاید را وارد کنید.")]
        [Display(Name = "تیتر")]
        public string title { get; set; }
        [Display(Name = "لینک")]
        public string link { get; set; }

    }
}
