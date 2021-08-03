using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseBackend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy ="CookiesAdmin")]
    public class MainSliderController : Controller
    {
        private readonly Context _context;

        public MainSliderController(Context context)
        {
            _context = context;
        }

        // GET: Admin/PrGroups
        public async Task<IActionResult> Index()
        {
            var list = await _context.MainSlider.ToListAsync();
            return View(list);
        }

        // GET: Admin/PrGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PrGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( MainSlider mainslider,IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string imname = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName.ToLowerInvariant());
                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/Files/Slider",
                                imname);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream).ConfigureAwait(false);
                    }
                    mainslider.imagename = imname;
                }
                else
                {
                    ModelState.AddModelError("imagename", "لطفا تصویر را انتخاب کنید.");
                    return View(mainslider);
                }
                _context.Add(mainslider);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mainslider);
        }

        // GET: Admin/PrGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mainslider = await _context.MainSlider.FindAsync(id);
            if (mainslider == null)
            {
                return NotFound();
            }
            return View(mainslider);
        }

        // POST: Admin/PrGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MainSlider mainslider,IFormFile image)
        {
            if (id != mainslider.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    MainSlider m =await _context.MainSlider.FindAsync(id);
                    if (image != null)
                    {
                        if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Slider", m.imagename)))
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Slider", m.imagename));
                        }
                        string imname = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName.ToLowerInvariant());
                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot/Files/Slider",
                                    imname);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await image.CopyToAsync(stream).ConfigureAwait(false);
                        }
                        m.imagename = imname;
                        m.title = mainslider.title;
                        m.link = mainslider.link;
                    }
                    _context.Update(m);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainSliderExists(mainslider.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            return View(mainslider);
        }

        // GET: Admin/PrGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var prGroup = await _context.MainSlider.FindAsync(id);
            _context.MainSlider.Remove(prGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MainSliderExists(int id)
        {
            return _context.MainSlider.Any(e => e.id == id);
        }
    }
}
