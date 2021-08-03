using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Components
{
    public class SliderLastTViewComponent : ViewComponent
    {
        private readonly Context _context;
        private ITakhfif t;
        public SliderLastTViewComponent(Context db)
        {
            _context = db;
            t =new TakhfifService(db);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Product> products = new List<Product>();
            List<int> takhfif = new List<int>();
            var list = _context.Product.ToList();
            foreach (var item in list)
            {
                if (item.price!=t.MablaghBaTakhfif(item))
                {
                    takhfif.Add(t.MablaghBaTakhfif(item));
                    products.Add(item);
                }
            }
            ViewBag.t = takhfif;
            return View(products.OrderByDescending(w => w.createdate));
        }
    }
}
