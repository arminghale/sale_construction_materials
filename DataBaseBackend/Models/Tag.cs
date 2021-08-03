using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Tag
    {
        public Tag()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="کالا")]
        public int productid { get; set; }
        public string text { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
