using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class ChangeInfo
    {
        public int? id { get; set; }
        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا نام را وارد کنید")]
        public string name { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام خانوادگی را وارد کنید")]
        public string lastname { get; set; }
        [Display(Name = "شماره تلفن")]
        [Required(ErrorMessage = "لطفا شماره تلفن را وارد کنید")]
        public string phonenumber { get; set; }
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید")]
        public string email { get; set; }
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        public string username { get; set; }
    }
}
