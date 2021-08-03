
using DataBaseBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Controllers
{
    [DisableRequestSizeLimit]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Context _context;
        private readonly Random random = new Random();
        ITakhfif t;

        public HomeController(Context context,ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
            t = new TakhfifService(context);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Route("/ContactUS")]
        public IActionResult ContactUS()
        {
            return View();
        }
        [Route("/ContactUS")]
        [HttpPost]
        public async Task<IActionResult> ContactUS(Message message)
        {
            if (ModelState.IsValid)
            {
                message.createdate = System.DateTime.Now;
                message.userid = _context.User.FirstOrDefault(w => w.roleid == 1).id;
                await _context.Message.AddAsync(message);
                await _context.SaveChangesAsync();
                return Redirect("/");
            }
            return View(message);
        }



        [Route("/Basket/{id}")]
        [Authorize]
        public async Task<IActionResult> Basket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .Include(p => p.BasketItems).OrderByDescending(w => w.createdate)
                .FirstOrDefaultAsync(m => m.userid == id && m.ispay == false);

            var total = 0;
            foreach (var item in basket.BasketItems)
            {
                item.mablagh = item.count * t.MablaghBaTakhfif(_context.Product.Find(item.productid));
                total += item.mablagh;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            basket.total = total;
            _context.Update(basket);
            await _context.SaveChangesAsync();
            return View(basket);
        }

        [Route("/BasketOK")]
        [Authorize]
        public async Task<IActionResult> BasketOK(int? id, int address)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .Include(p => p.BasketItems).OrderByDescending(w => w.createdate)
                .FirstOrDefaultAsync(m => m.userid == id && m.ispay == false);
            if (basket == null)
            {
                return NotFound();
            }
            basket.addressid = address;
            basket.ispay = true;
            basket.paydate = System.DateTime.Now;
            basket.paymentid = random.Next(1987400);
            _context.Basket.Update(basket);
            await _context.SaveChangesAsync();
            int ready = 0;
            int send = 0;
            foreach (var item in basket.BasketItems)
            {
                if (item.Product.readyday > ready)
                {
                    ready = item.Product.readyday;
                }
                if (item.Product.sendday > send)
                {
                    send = item.Product.sendday;
                }
            }
            ViewBag.send = send;
            ViewBag.ready = ready;
            return View(basket);
        }

        [HttpPost]
        [Route("/BasketAdd")]
        [Authorize]
        public async Task<IActionResult> BasketAdd(int userid,int prid,int tedad)
        {
            if (userid == 0)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(prid);
            if ((product.count - tedad) < 0)
            {
                return BadRequest(new { message = "میزان درخواستی بیشتر از موجودی انبار است." });
            }
            var basket = await _context.Basket.OrderByDescending(w => w.createdate)
                .FirstOrDefaultAsync(m => m.userid == userid && m.ispay == false);
            if (basket == null)
            {
                basket = new Basket
                {
                    userid = userid,
                    createdate = System.DateTime.Now,
                    iscansel = false,
                    ispay = false,
                    isready = false,
                    issend = false,
                    paydate = System.DateTime.Now,
                    senddate = System.DateTime.Now,
                    paymentid = 0,
                    total = t.MablaghBaTakhfif(product) * tedad,
                    addressid = _context.Address.FirstOrDefault(w => w.personid == userid).id
                };
                _context.Basket.Add(basket);
                await _context.SaveChangesAsync();

                _context.BasketItem.Add(new BasketItem
                {
                    basketid = basket.id,
                    productid = product.id,
                    count = tedad,
                    mablagh = t.MablaghBaTakhfif(product) * tedad
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                if (basket.BasketItems.Any(w => w.productid == product.id))
                {
                    var item = basket.BasketItems.FirstOrDefault(w => w.productid == product.id);
                    var count = item.count + tedad;
                    item.mablagh = t.MablaghBaTakhfif(product) * count;
                    item.count = count;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    var total = 0;
                    foreach (var item2 in basket.BasketItems)
                    {
                        total += item2.mablagh;
                    }
                    basket.total = total;
                    _context.Update(basket);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.BasketItem.Add(new BasketItem
                    {
                        basketid = basket.id,
                        productid = product.id,
                        count = tedad,
                        mablagh = t.MablaghBaTakhfif(product) * tedad
                    });
                    await _context.SaveChangesAsync();

                    var total = 0;
                    foreach (var item2 in basket.BasketItems)
                    {
                        total += item2.mablagh;
                    }
                    basket.total = total;
                    _context.Update(basket);
                    await _context.SaveChangesAsync();
                }
            }
            return Redirect("/Basket/"+userid);
        }

        [HttpPost]
        [Route("/BasketDown")]
        [Authorize]
        public async Task<IActionResult> BasketDown(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.BasketItem
                .FirstOrDefaultAsync(m => m.id == id);
            if (item == null)
            {
                return NotFound();
            }
            if (item.count - 1 == 0)
            {
                _context.BasketItem.Remove(item);
            }
            else
            {
                item.mablagh = (item.count - 1) * t.MablaghBaTakhfif(_context.Product.Find(item.productid));
                item.count--;
                _context.Update(item);
            }
            await _context.SaveChangesAsync();

            var basket = await _context.Basket.FirstOrDefaultAsync(m => m.id == item.basketid);
            var total = 0;
            foreach (var item2 in basket.BasketItems)
            {
                item2.mablagh = item2.count * t.MablaghBaTakhfif(_context.Product.Find(item2.productid));
                total += item2.mablagh;
                _context.Update(item2);
                await _context.SaveChangesAsync();
            }
            basket.total = total;
            _context.Update(basket);
            await _context.SaveChangesAsync();

            return Ok();
            //return Redirect("Basket/?id=" + basket.userid);

        }

        [Route("/BasketItemDelete/{id}")]
        [Authorize]
        public async Task<IActionResult> BasketItemDelete( int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.BasketItem
                .FirstOrDefaultAsync(m => m.id == id);
            if (item == null)
            {
                return NotFound();
            }
            _context.BasketItem.Remove(item);
            await _context.SaveChangesAsync();

            var basket = await _context.Basket.FirstOrDefaultAsync(m => m.id == item.basketid);
            var total = 0;
            foreach (var item2 in basket.BasketItems)
            {
                item2.mablagh = item2.count * t.MablaghBaTakhfif(_context.Product.Find(item2.productid));
                total += item2.mablagh;
                _context.Update(item2);
                await _context.SaveChangesAsync();
            }
            basket.total = total;
            _context.Update(basket);
            await _context.SaveChangesAsync();

            return Redirect("Basket/?id=" + basket.userid);

        }

    }
}
