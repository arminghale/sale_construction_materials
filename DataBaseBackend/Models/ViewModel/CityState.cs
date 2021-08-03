using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class State
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
    }
    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int province_id { get; set; }
    }
}
