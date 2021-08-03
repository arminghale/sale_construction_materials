using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Person
    {
        public Person()
        {

        }
        [ForeignKey("User"),Key]
        public int id { get; set; }
        [Display(Name ="نام")]
        [Required(ErrorMessage ="لطفا نام را وارد کنید")]
        public string name { get; set; }
        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا نام خانوادگی را وارد کنید")]
        public string lastname { get; set; }
        [Display(Name = "شماره تلفن")]
        [Required(ErrorMessage = "لطفا شماره تلفن را وارد کنید")]
        public string phonenumber { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Address> Addresses { get; set; }
    }
}
