using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class PrGroup
    {
        public PrGroup()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="نام گروه")]
        [Required(ErrorMessage = "لطفا نام گروه را وارد کنید")]
        public string title { get; set; }
        [Display(Name = "واحد")]
        [Required(ErrorMessage = "لطفا واحد اندازه گیری را وارد کنید")]
        public string vahed { get; set; }

        
        public virtual List<Field> Fields { get; set; }
        public virtual List<Product> Products { get; set; }
    }
}
