using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Comment
    {
        public Comment()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="کالا")]
        public int productid { get; set; }
        [Display(Name ="کاربر")]
        public int userid { get; set; }
        [Display(Name ="متن")]
        [Required(ErrorMessage ="لطفا متن را وارد کنید")]
        public string text { get; set; }

        public System.DateTime createdate { get; set; }

        public virtual Product Product { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }
        public virtual List<CommentReplay> CommentReplays { get; set; }
    }
}
