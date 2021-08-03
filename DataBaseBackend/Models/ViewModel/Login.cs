using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Login
    {
        [Display(Name ="نام کاربری/ایمیل")]
        [Required(ErrorMessage = "لطفا نام کاربری/ایمیل را وارد کنید")]
        public string username { get; set; }
        [Display(Name ="رمز عبور")]
        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}
