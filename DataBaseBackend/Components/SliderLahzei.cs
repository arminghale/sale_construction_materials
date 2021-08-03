using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Components
{
    public class SliderLahzeiViewComponent : ViewComponent
    {
        private readonly Context _context;
        private ITakhfif t;
        public SliderLahzeiViewComponent(Context db)
        {
            _context = db;
            t = new TakhfifService(db);
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Product> products = new List<Product>();
            List<int> takhfif = new List<int>();
            var list = _context.Product.Take(15).ToList();
            foreach (var item in list)
            {
                takhfif.Add(t.MablaghBaTakhfif(item));
                products.Add(item);
            }
            ViewBag.t = takhfif;
            return View(products);
        }
    }
}
