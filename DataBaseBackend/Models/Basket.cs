using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Basket
    {
        public Basket()
        {

        }
        [Key]
        public int id { get; set; }
        public int userid { get; set; }
        public int addressid { get; set; }
        [Display(Name ="مبلغ کل")]
        public int total { get; set; }
        [Display(Name ="پرداخت/عدم پرداخت")]
        public bool ispay { get; set; }
        [Display(Name ="آماده شده/در حال بررسی")]
        public bool isready { get; set; }
        [Display(Name ="ارسال شده/ارسال نشده")]
        public bool issend { get; set; }
        [Display(Name ="کنسل شده")]
        public bool iscansel { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public System.DateTime createdate { get; set; }
        [Display(Name = "تاریخ پرداخت")]
        public System.DateTime paydate { get; set; }
        [Display(Name = "تاریخ ارسال")]
        public System.DateTime senddate { get; set; }
        [Display(Name = "شماره پیگیری تراکنش")]
        public int paymentid { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual User User { get; set; }
        public virtual Address Address { get; set; }
        public virtual List<BasketItem> BasketItems { get; set; }
    }
}
