using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class Role
    {
        public Role()
        {

        }
        [Key]
        public int id { get; set; }
        public string title { get; set; }


        [Newtonsoft.Json.JsonIgnore]
        public virtual List<User> Users { get; set; }
    }
}
