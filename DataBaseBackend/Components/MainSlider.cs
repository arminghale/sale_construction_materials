using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Components
{
    public class MainSliderViewComponent : ViewComponent
    {
        readonly Context _context;
        public MainSliderViewComponent(Context db)
        {
            _context = db;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_context.MainSlider);
        }
    }
}
