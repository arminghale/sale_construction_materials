using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Takhfif
    {
        public Takhfif()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="کالا")]
        public int productid { get; set; }
        [Display(Name ="زمان شروع")]
        [Required(ErrorMessage = "لطفا زمان شروع را وارد کنید")]
        public System.DateTime starttime { get; set; }
        [Display(Name ="زمان پایان")]
        [Required(ErrorMessage = "لطفا زمان پایان را وارد کنید")]
        public System.DateTime endtime { get; set; }
        [Display(Name ="درصد")]
        [Required(ErrorMessage = "لطفا درصد را وارد کنید")]
        public int darsad { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
