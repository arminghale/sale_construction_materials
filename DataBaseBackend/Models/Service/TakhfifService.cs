using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend
{
    public class TakhfifService : ITakhfif
    {
        readonly Context db;
        public TakhfifService(Context db)
        {
            this.db = db;
        }
        public int MablaghBaTakhfif(Product p)
        {
            Takhfif last = p.Takhfifs.OrderByDescending(w => w.endtime).FirstOrDefault();
            if (last!=null)
            {
                if (System.DateTime.Now < last.endtime && System.DateTime.Now > last.starttime)
                {
                    double first = ((100.0 - last.darsad) / 100.0);
                    double mablagh = first * (p.price);
                    return int.Parse(mablagh.ToString());
                }
            }
            return p.price;
        }
    }
}
