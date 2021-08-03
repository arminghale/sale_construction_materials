using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseBackend;
using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CookiesAdmin")]
    public class ProductsController : Controller
    {
        private readonly Context _context;

        public ProductsController(Context context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(int no=-1,string search="",int gid=-1)
        {
            var context = await _context.Product.OrderByDescending(w => w.createdate).ToListAsync();
            ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title");
            if (gid != -1)
            {
                context = context.Where(w => w.prgroupid == gid).ToList();
                ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title", gid);
            }
            if (no==0)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    context = context.Where(w => w.title.Contains(search)||w.text.Contains(search)).ToList();
                }
            }
            if (no == 1)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    context = context.Where(w => w.id.ToString().Contains(search)).ToList();
                }
            }
            
            return View(context);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.PrGroup)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("prgroupid,title,price,count,readyday,sendday,text")] Product product,IFormFile mainimage, string tag,IFormFile[] gallery)
        {
            ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title", product.prgroupid);
            ViewBag.tag = tag;
            if (ModelState.IsValid)
            {
                product.createdate = System.DateTime.Now;
                if (mainimage!=null)
                {
                    string imname = Guid.NewGuid().ToString() + Path.GetExtension(mainimage.FileName.ToLowerInvariant());
                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/Files/",
                                imname);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await mainimage.CopyToAsync(stream).ConfigureAwait(false);
                    }
                    product.imagename = imname;
                    var thumb = Image.Load(mainimage.OpenReadStream());
                    thumb.Mutate(x => x.Resize(150, 150));
                    thumb.Save(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Thumb/" + imname));
                }
                else
                {
                    ModelState.AddModelError("createdate", "لطفا تصویر اصلی را انتخاب کنید");
                    return View(product);
                }
                product.seen = 0;
                Product p = product;
                _context.Add(p);
                await _context.SaveChangesAsync();

                foreach (var item in gallery)
                {
                    if (item != null)
                    {
                        string imname = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName.ToLowerInvariant());
                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery",
                                    imname);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await item.CopyToAsync(stream).ConfigureAwait(false);
                        }
                        _context.Add(new Gallery { productid = p.id, imagename = imname });
                        await _context.SaveChangesAsync();
                    }
                    
                }

                string[] tags = tag.Split('-');
                foreach (var item in tags)
                {
                    _context.Add(new Tag {productid=p.id,text=item });
                    await _context.SaveChangesAsync();
                }

                var prf = _context.PrGroup.Find(product.prgroupid).Fields;
                foreach (var item in prf)
                {
                    _context.Add(new FillField { fieldid=item.id,productid=product.id });
                    await _context.SaveChangesAsync();
                }

                return Redirect("/Admin/Products");
            }
            
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            string tag = "";
            foreach (var item in _context.Tag.Where(w => w.productid ==product.id))
            {
                tag += item.text + "-";
            }
            ViewBag.tag = tag.Remove(tag.Length-1);
            ViewBag.gallery = _context.Gallery.Where(w => w.productid == product.id);
            ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title", product.prgroupid);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,prgroupid,title,price,imagename,count,readyday,sendday,text,createdate")] Product product, IFormFile mainimage, string tag, IFormFile[] gallery)
        {
            if (id != product.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (mainimage != null)
                    {
                        if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/", product.imagename)))
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/", product.imagename));
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Thumb", product.imagename));
                        }
                        string imname = Guid.NewGuid().ToString() + Path.GetExtension(mainimage.FileName.ToLowerInvariant());
                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot/Files/",
                                    imname);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await mainimage.CopyToAsync(stream).ConfigureAwait(false);
                        }
                        product.imagename = imname;
                        var thumb = Image.Load(mainimage.OpenReadStream());
                        thumb.Mutate(x => x.Resize(150, 150));
                        thumb.Save(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Thumb/" + imname));
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    foreach (var item in gallery)
                    {
                        if (item != null)
                        {
                            string imname = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName.ToLowerInvariant());
                            var path = Path.Combine(
                                        Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery",
                                        imname);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await item.CopyToAsync(stream).ConfigureAwait(false);
                            }
                            _context.Add(new Gallery { productid = id, imagename = imname });
                            await _context.SaveChangesAsync();
                        }

                    }
                    var list =await _context.Tag.Where(w => w.productid == id).ToListAsync();
                    foreach (var item in list)
                    {
                        _context.Tag.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                    string[] tags = tag.Split('-');
                    foreach (var item in tags)
                    {
                        _context.Add(new Tag { productid = id, text = item });
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Admin/Products");
            }

            ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title", product.prgroupid);
            ViewBag.tag = tag;
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(id);
            var list = await _context.FillField.Where(w => w.productid == product.id).ToListAsync();
            foreach (var item in list)
            {
                _context.FillField.Remove(item);
                await _context.SaveChangesAsync();
            }
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //var product = await _context.Product
            //    .Include(p => p.PrGroup)
            //    .FirstOrDefaultAsync(m => m.id == id);
            //if (product == null)
            //{
            //    return NotFound();
            //}

            //return View(product);
        }

        // POST: Admin/Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _context.Product.FindAsync(id);
        //    _context.Product.Remove(product);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        public async Task<IActionResult> Field(int id)
        {
            ViewBag.id = id;
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.prfields = product.FillFields;
            foreach (var item in product.PrGroup.Fields)
            {
                if (_context.FillField.FirstOrDefault(w=>w.productid==product.id&&w.fieldid==item.id)==null)
                {
                    _context.Add(new FillField { productid = product.id, fieldid = item.id });
                    await _context.SaveChangesAsync();
                }
            }
            return View(_context.FillField.Where(w=>w.productid==id));
        }

        [HttpPost]
        public async Task<IActionResult> Field(int id,string[] fields,int[] ids)
        {
            var field = await _context.FillField.Where(w => w.productid == id).ToListAsync();
            for (int i = 0; i < ids.Length; i++)
            {
                if (!string.IsNullOrEmpty(fields[i]))
                {
                    if (fields[i] != field.FirstOrDefault(w => w.id == ids[i]).text)
                    {
                        field.FirstOrDefault(w => w.id == ids[i]).text = fields[i];
                        _context.Update(field.FirstOrDefault(w => w.fieldid == ids[i]));
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Redirect("/Admin/Products");
        }


        public async Task<IActionResult> Takhfif(int productid)
        {
            ViewBag.productid = productid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Takhfif(Takhfif takhfif,string start,string end)
        {
            ViewBag.productid = takhfif.productid;
            start=start.Replace('-', '/');
            start = changePersianNumbersToEnglish(start);
            takhfif.starttime= DateTime.Parse(start, new CultureInfo("fa-IR"));
            end = end.Replace('-', '/');
            end = changePersianNumbersToEnglish(end);
            takhfif.endtime = DateTime.Parse(end, new CultureInfo("fa-IR"));
            if (ModelState.IsValid)
            {
                Takhfif pr = takhfif;
                _context.Add(pr);
                await _context.SaveChangesAsync();
                return Redirect("/Admin/Products");
            }
            
            return View(takhfif);
        }
        

        public async Task<IActionResult> TakhfifList(int id)
        {
            ViewBag.id = id;
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(_context.Takhfif.Where(w => w.productid == id).OrderByDescending(w=>w.endtime));
        }

        public async Task<IActionResult> TakhfifDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Takhfif.FindAsync(id);
            _context.Takhfif.Remove(product);
            await _context.SaveChangesAsync();
            return Redirect("/Admin/Products");
        }


        [HttpGet]
        public async Task<bool> DeleteGallery(int id)
        {

            var product = await _context.Gallery.FindAsync(id);
            if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery", product.imagename)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery", product.imagename));
            }
            _context.Gallery.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.id == id);
        }

        private string changePersianNumbersToEnglish(string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int j = 0; j < persian.Length; j++)
                input = input.Replace(persian[j], j.ToString());

            return input;
        }
    }
}
