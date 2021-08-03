using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Components
{
    public class HeaderViewComponent : ViewComponent
    {
        readonly Context _context;
        public HeaderViewComponent(Context db)
        {
            _context = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_context.PrGroup);
        }
    }
}
