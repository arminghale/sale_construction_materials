using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseBackend;
using Microsoft.AspNetCore.Authorization;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy ="CookiesAdmin")]
    public class PrGroupsController : Controller
    {
        private readonly Context _context;

        public PrGroupsController(Context context)
        {
            _context = context;
        }

        // GET: Admin/PrGroups
        public async Task<IActionResult> Index(string search="")
        {
            var list = await _context.PrGroup.ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(w => w.title.Contains(search)).ToList();
            }
            return View(list);
        }

        // GET: Admin/PrGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prGroup = await _context.PrGroup
                .FirstOrDefaultAsync(m => m.id == id);
            if (prGroup == null)
            {
                return NotFound();
            }

            return View(prGroup);
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
        public async Task<IActionResult> Create([Bind("id,title,vahed")] PrGroup prGroup,string[] field)
        {
            if (ModelState.IsValid)
            {
                PrGroup pr = prGroup;
                _context.Add(pr);
                await _context.SaveChangesAsync();
                foreach (var item in field)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        _context.Field.Add(new Field { prgroupid = pr.id, title = item });
                        await _context.SaveChangesAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.fields = field;
            return View(prGroup);
        }

        // GET: Admin/PrGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prGroup = await _context.PrGroup.FindAsync(id);
            if (prGroup == null)
            {
                return NotFound();
            }
            //List<string> fields = new List<string>();
            //foreach (var item in _context.Field.Where(w=>w.prgroupid==prGroup.id))
            //{
            //    fields.Add(item.title);
            //}
            ViewBag.fields = _context.Field.Where(w => w.prgroupid == prGroup.id);
            return View(prGroup);
        }

        // POST: Admin/PrGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,title,vahed")] PrGroup prGroup, string[] field)
        {
            ViewBag.fields = _context.Field.Where(w => w.prgroupid == prGroup.id);
            if (id != prGroup.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prGroup);
                    await _context.SaveChangesAsync();
                    //foreach (var item in _context.Field.Where(w=>w.prgroupid==id))
                    //{
                    //    _context.Remove(item);
                    //    await _context.SaveChangesAsync();
                    //}
                    int flag;
                    foreach (var item in field)
                    {
                        flag = 0;
                        if (!string.IsNullOrEmpty(item))
                        {
                            foreach (var item2 in _context.Field.Where(w => w.prgroupid == id))
                            {
                                if (item==item2.title)
                                {
                                    flag++;
                                }
                            }
                            if (flag==0)
                            {
                                _context.Field.Add(new Field { prgroupid = id, title = item });
                                await _context.SaveChangesAsync();
                            }
                            else if (flag>1)
                            {
                                ModelState.AddModelError("title", "بعضی از فیلد ها به طور تکراری وارد شده اند.");
                                return View(prGroup);
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrGroupExists(prGroup.id))
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
            
            return View(prGroup);
        }

        // GET: Admin/PrGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var prGroup = await _context.PrGroup.FindAsync(id);
            _context.PrGroup.Remove(prGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //var prGroup = await _context.PrGroup
            //    .FirstOrDefaultAsync(m => m.id == id);
            //if (prGroup == null)
            //{
            //    return NotFound();
            //}

            //return View(prGroup);
        }

        // POST: Admin/PrGroups/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var prGroup = await _context.PrGroup.FindAsync(id);
        //    _context.PrGroup.Remove(prGroup);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpGet]
        public async Task<bool> DeleteField(int id)
        {

            var product = await _context.Field.FindAsync(id);
            _context.Field.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool PrGroupExists(int id)
        {
            return _context.PrGroup.Any(e => e.id == id);
        }
    }
}
