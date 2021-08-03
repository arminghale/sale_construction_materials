using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Address
    {
        public Address()
        {

        }
        [Key]
        public int id { get; set; }
        public int personid { get; set; }
        [Display(Name ="استان")]
        public string ostan { get; set; }
        [Display(Name = "شهر")]
        public string shahr { get; set; }
        [Display(Name = "کدپستی")]
        [Required(ErrorMessage = "لطفا کدپستی را وارد کنید")]
        public string codeposti { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا آدرس را وارد کنید")]
        public string text { get; set; }
        [Display(Name = "نام گیرنده")]
        public string girandename { get; set; }
        [Display(Name = "نام خانوادگی گیرنده")]
        public string girandelastname { get; set; }
        [Display(Name ="شماره تلفن گیرنده")]
        public string girandephonenumber { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual Person Person { get; set; }
    }
}
