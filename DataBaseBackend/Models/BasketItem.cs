using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class BasketItem
    {
        public BasketItem()
        {

        }
        [Key]
        public int id { get; set; }
        public int basketid { get; set; }
        public int productid { get; set; }
        [Display(Name ="تعداد")]
        public int count { get; set; }
        [Display(Name = "مبلغ")]
        public int mablagh { get; set; }

        public virtual Product Product { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual Basket Basket { get; set; }
    }
}
