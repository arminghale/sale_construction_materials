using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class FillField
    {
        public FillField()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="فیلد")]
        public int fieldid { get; set; }
        [Display(Name ="کالا")]
        public int productid { get; set; }
        [Display(Name ="مقدار فیلد")]
        public string text { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual Field Field { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
