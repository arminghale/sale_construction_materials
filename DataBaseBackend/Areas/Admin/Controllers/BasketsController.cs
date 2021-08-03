using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseBackend;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CookiesAdmin")]
    public class BasketsController : Controller
    {
        private readonly Context _context;
        readonly Random random;
        ITakhfif t;

        public BasketsController(Context context)
        {
            _context = context;
            random = new Random();
            t = new TakhfifService(context);
        }

        // GET: Admin/Baskets
        public async Task<IActionResult> Index(string search="",int type=-1)
        {
            var context = _context.Basket.Include(b => b.Address).Include(b => b.User).Where(w=>w.ispay==true);
            if (type==0)
            {
                context = context.Where(w => w.issend == true);
            }
            if (type==1)
            {
                context = context.Where(w => w.isready == true);
            }
            if (type == 2)
            {
                context = context.Where(w => w.iscansel == true);
            }
            if (type == 3)
            {
                context = context.Where(w => w.ispay == true);
            }
            if (!string.IsNullOrEmpty(search))
            {
                context = context.Where(w => w.User.username.Contains(search) || w.User.email.Contains(search));
            }
            ViewBag.ok = type;
            return View(await context.OrderByDescending(w=>w.paydate).ToListAsync());
        }

        // GET: Admin/Baskets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .Include(b => b.Address)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.id == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // GET: Admin/Baskets/Create
        public IActionResult Create()
        {
            ViewData["userid"] = new SelectList(_context.User.Where(w=>w.roleid!=1 && w.roleid != 3), "id", "username");
            return View();
        }

        // POST: Admin/Baskets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Basket basket,int[] ids,int[] counts)
        {
            ViewData["addressid"] = new SelectList(_context.Address.Where(w => w.personid == basket.userid), "id", "ostan" + "-" + "shahr" + "-" + "text", basket.addressid);
            ViewData["userid"] = new SelectList(_context.User.Where(w => w.roleid != 1&& w.roleid != 3), "id", "username", basket.userid);
            List<Product> p = new List<Product>();
            List<int> prices = new List<int>();
            foreach (var item in ids)
            {
                p.Add(_context.Product.Find(item));
            }
            foreach (var item in p)
            {
                prices.Add(t.MablaghBaTakhfif(item));
            }
            ViewBag.prs = p;
            ViewBag.counts = counts;
            ViewBag.prices = prices;
            if (basket.addressid==-1)
            {
                ModelState.AddModelError("addressid", "لطفا آدرس را انتخاب کنید");
                return View(basket);
            }
            if (basket.userid == -1)
            {
                ModelState.AddModelError("userid", "لطفا کاربر را انتخاب کنید");
                return View(basket);
            }
            basket.paymentid = random.Next(1258745, 98526774);
                basket.ispay = true;
                basket.iscansel = false;
                basket.isready = false;
                basket.issend = false;
                basket.paydate = System.DateTime.Now;
                basket.createdate = System.DateTime.Now;
                basket.senddate = System.DateTime.Now;
            Basket b = basket;
                _context.Add(b);
                await _context.SaveChangesAsync();
            for (int i = 0; i < p.Count(); i++)
            {
                p[i].count -= counts[i];
                _context.Update(p[i]);
                _context.BasketItem.Add(new BasketItem { productid = p[i].id, basketid = b.id, count = counts[i], mablagh = (counts[i] * t.MablaghBaTakhfif(p[i])) });
                await _context.SaveChangesAsync();
            }

                return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Baskets/Edit/5
        public async Task<IActionResult> Edit(int? id,bool ready=false,bool send=false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket.FindAsync(id);
            if (ready)
            {
                basket.isready = true;
                _context.Update(basket);
                await _context.SaveChangesAsync();
            }
            if (send)
            {
                basket.issend = true;
                basket.senddate = System.DateTime.Now;
                _context.Update(basket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
            //if (basket == null)
            //{
            //    return NotFound();
            //}
            //ViewData["addressid"] = new SelectList(_context.Address, "id", "codeposti", basket.addressid);
            //ViewData["userid"] = new SelectList(_context.User, "id", "email", basket.userid);
            //return View(basket);
        }

        // POST: Admin/Baskets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,userid,addressid,total,ispay,isready,issend,iscansel,createdate,paydate,senddate,paymentid")] Basket basket)
        {
            if (id != basket.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasketExists(basket.id))
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
            ViewData["addressid"] = new SelectList(_context.Address, "id", "codeposti", basket.addressid);
            ViewData["userid"] = new SelectList(_context.User, "id", "email", basket.userid);
            return View(basket);
        }

        // GET: Admin/Baskets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var basket = await _context.Basket.FindAsync(id);
            _context.Basket.Remove(basket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Addresses(int id)
        {
            var context = await _context.Address.Where(w => w.personid == id).ToListAsync();
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 1;
            var json = JsonConvert.SerializeObject(context, setting);
            return Content(json, "application/json", System.Text.Encoding.UTF8);
        }

        public async Task<IActionResult> Cansel(int? id, bool iscansel)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket.FindAsync(id);
            if (iscansel)
            {
                basket.iscansel = true;
                _context.Update(basket);
                await _context.SaveChangesAsync();
            }else
            {
                basket.iscansel = false;
                _context.Update(basket);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
            //if (basket == null)
            //{
            //    return NotFound();
            //}
            //ViewData["addressid"] = new SelectList(_context.Address, "id", "codeposti", basket.addressid);
            //ViewData["userid"] = new SelectList(_context.User, "id", "email", basket.userid);
            //return View(basket);
        }

        public async Task<IActionResult> ProductSearch(int no = -1, string search = "", int gid = -1)
        {
            var context = await _context.Product.ToListAsync();
            ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title");
            if (gid != -1)
            {
                context = context.Where(w => w.prgroupid == gid).ToList();
                ViewData["prgroupid"] = new SelectList(_context.PrGroup, "id", "title", gid);
            }
            if (no == 0)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    context = context.Where(w => w.title.Contains(search)).ToList();
                }
            }
            if (no == 1)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    context = context.Where(w => w.id.ToString().Contains(search)).ToList();
                }
            }

            return PartialView(context);
        }

        public int ProductBaTakhfif(int id)
        {
            var context = _context.Product.Find(id);

            return t.MablaghBaTakhfif(context);
        }

        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.id == id);
        }
    }
}
