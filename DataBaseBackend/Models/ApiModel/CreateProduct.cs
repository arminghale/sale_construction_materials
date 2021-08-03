using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class CreateProduct
    {
        public int? id { get; set; }
        public int prgroupid { get; set; }
        public string title { get; set; }
        public int price { get; set; }
        public int count { get; set; }
        public int readyday { get; set; }
        public int sendday { get; set; }
        public string text { get; set; }
        public string mainimage { get; set; }
        public string tag { get; set; }
        public string[] gallery { get; set; }
    }
}
