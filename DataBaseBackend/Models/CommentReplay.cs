using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class CommentReplay
    {
        public CommentReplay()
        {

        }
        [Key]
        public int id { get; set; }
        public int commentid { get; set; }
        [Display(Name ="متن")]
        [Required(ErrorMessage = "لطفا متن را وارد کنید")]
        public string text { get; set; }

        public System.DateTime createdate { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual Comment Comment { get; set; }
    }
}
