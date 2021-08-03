using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Product
    {
        public Product()
        {

        }
        [Key]
        public int id { get; set; }
        [Display(Name ="گروه")]
        public int prgroupid { get; set; }
        [Display(Name ="عنوان")]
        [Required(ErrorMessage = "لطفا عنوان را وارد کنید")]
        public string title { get; set; }
        [Display(Name ="قیمت به تومان")]
        [Required(ErrorMessage = "لطفا قیمت را وارد کنید")]
        public int price { get; set; }
        [Display(Name ="تصویر")]
        public string imagename { get; set; }
        [Display(Name ="تعداد")]
        [Required(ErrorMessage = "لطفا تعداد را وارد کنید")]
        public int count { get; set; }
        [Display(Name = "مدت آماده سازی")]
        [Required(ErrorMessage = "لطفا مدت آماده سازی را وارد کنید")]
        public int readyday { get; set; }
        [Display(Name = "مدت ارسال")]
        [Required(ErrorMessage = "لطفا مدت ارسال را وارد کنید")]
        public int sendday { get; set; }
        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "لطفا توضیحات را وارد کنید")]
        public string text { get; set; }
        [Display(Name = "تاریخ ایجاد")]
        public System.DateTime createdate { get; set; }
        [Display(Name = "بازدید")]
        public int seen { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual PrGroup PrGroup { get; set; }
        public virtual List<FillField> FillFields { get; set; }
        public virtual List<Gallery> Galleries { get; set; }
        public virtual List<Takhfif> Takhfifs { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Comment> Comments { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<BasketItem> BasketItems { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
}
