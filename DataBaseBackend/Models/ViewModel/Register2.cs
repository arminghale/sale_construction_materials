using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Register2
    {
        public int? id { get; set; }
        [Display(Name ="نام")]
        [Required(ErrorMessage = "لطفا نام را وارد کنید")]
        public string name { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام خانوادگی را وارد کنید")]
        public string lastname { get; set; }
        [Display(Name = "شماره تلفن")]
        [Required(ErrorMessage = "لطفا شماره تلفن را وارد کنید")]
        public string phonenumber { get; set; }
        [Display(Name = "استان")]
        [Required(ErrorMessage = "لطفا استان را وارد کنید")]
        public int ostan { get; set; }
        [Display(Name = "شهر")]
        [Required(ErrorMessage = "لطفا شهر را وارد کنید")]
        public int shahr { get; set; }
        [Display(Name = "کدپستی")]
        [Required(ErrorMessage = "لطفا کدپستی را وارد کنید")]
        public string codeposti { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا آدرس را وارد کنید")]
        public string text { get; set; }

    }
}
