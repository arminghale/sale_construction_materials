using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Field
    {
        public Field()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="گروه")]
        public int prgroupid { get; set; }
        [Display(Name ="نام فیلد")]
        [Required(ErrorMessage = "لطفا نام فیلد را وارد کنید")]
        public string title { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual PrGroup PrGroup { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<FillField> FillFields { get; set; }
    }
}
