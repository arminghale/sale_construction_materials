using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Register1
    {
        [Display(Name ="ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل را وارد کنید")]
        public string email { get; set; }
        [Display(Name ="نام کاربری")]
        [Required(ErrorMessage = "لطفا نام کاربری را وارد کنید")]
        public string username { get; set; }
        [Display(Name ="رمز عبور")]
        [Required(ErrorMessage = "لطفا رمز عبور را وارد کنید")]
        [StringLength(100, ErrorMessage = "رمز عبور باید بیشتر از 8 کاراکتر باشد", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Display(Name ="تکرار رمز عبور")]
        [Required(ErrorMessage = "لطفا تکرار رمز عبور را وارد کنید")]
        [Compare("password", ErrorMessage ="رمز عبور و تکرار رمز عبور متفاوت هستند")]
        [DataType(DataType.Password)]
        public string repassword { get; set; }
    }
}
