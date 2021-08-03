using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Message
    {
        public Message()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name = "به کاربر")]
        public int userid { get; set; }
        [Display(Name = "از کاربر")]
        public int? user2id { get; set; }
        [Display(Name ="متن پیام")]
        [Required(ErrorMessage ="لطفا متن پیام را وارد کنید")]
        public string text { get; set; }
        public bool isseen { get; set; }
        public System.DateTime createdate { get; set; }
        public string sendername { get; set; }
        public string senderlastname { get; set; }
        public string senderemail { get; set; }
        public virtual User User { get; set; }

        public virtual User User2 { get; set; }
    }
}
