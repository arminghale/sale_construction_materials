using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class User
    {
        public User()
        {

        }
        [Key]
        public int id { get; set; }
        public int roleid { get; set; }
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا ایمیل وارد کنید")]
        public string email { get; set; }
        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا نام کاربری وارد کنید")]
        public string username { get; set; }
        [Display(Name = "رمزعبور")]
        [Required(ErrorMessage = "لطفا رمزعبور وارد کنید")]
        public string password { get; set; }
        [Display(Name = "آخرین ورود")]
        public System.DateTime lastlogin { get; set; }
        [Display(Name = "کد فعالسازی")]
        public int activecode { get; set; }
        [Display(Name = "فعال/غیر فعال")]
        public bool isactive { get; set; }


        public virtual Role Role { get; set; }
        public virtual Person Person { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Message> Messages { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Message> Messages2 { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Basket> Baskets { get; set; }

    }
}
