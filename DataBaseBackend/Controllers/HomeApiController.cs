using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseBackend.Controllers
{
    [ApiController, DisableRequestSizeLimit]
    [AllowAnonymous]
    public class HomeApiController : ControllerBase
    {
        readonly Context _context;
        readonly Random random;
        IViewRenderService vi;
        private IWebHostEnvironment _enviroment;
        IUser u;
        ITakhfif t;
        public HomeApiController(Context db, IViewRenderService vi, IWebHostEnvironment enviroment)
        {
            _context = db;
            u = new UserService(db);
            t = new TakhfifService(db);
            random = new Random();
            this.vi = vi;
            _enviroment = enviroment;
            //var body = vi.RenderToStringAsync("Faktor",);
            //SendMail.Send(, "کد فعالسازی", body);
        }

        [HttpGet]
        [Route("api/Groups/")]
        public async Task<IActionResult> PrGroup(string search = "")
        {
            var list = await _context.PrGroup.ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(w => w.title.Contains(search)).ToList();
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 1;
            return Content(JsonConvert.SerializeObject(list, setting), "application/json", Encoding.UTF8);
        }

        [HttpGet]
        [Route("api/Products/")]
        public async Task<IActionResult> Product(string search = "", int gid = -1,string sortby="")
        {
            var context = await _context.Product.ToListAsync();
            if (!string.IsNullOrEmpty(sortby))
            {
                if (sortby=="createdate")
                {
                    context = context.OrderByDescending(w => w.createdate).ToList();
                }
                if (sortby == "takhfif")
                {
                    context = context.Where(w => w.price>t.MablaghBaTakhfif(w)).ToList();
                }
                if (sortby == "gran")
                {
                    context = context.OrderByDescending(w=>t.MablaghBaTakhfif(w)).ToList();
                }
                if (sortby == "arzan")
                {
                    context = context.OrderBy(w => t.MablaghBaTakhfif(w)).ToList();
                }
            }
            if (gid != -1)
            {
                context = context.Where(w => w.prgroupid == gid).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                var list = context.Where(w => w.title.Contains(search)||w.text.Contains(search)||w.PrGroup.title.Contains(search)).ToList();
                foreach (var item in context)
                {
                    foreach (var item2 in item.Tags)
                    {
                        if (item2.text.Contains(search))
                        {
                            if (!list.Any(w=>w==item))
                            {
                                list.Add(item);
                            }
                        }
                    }
                    foreach (var item2 in item.FillFields)
                    {
                        if (item2.text.Contains(search))
                        {
                            if (!list.Any(w => w == item))
                            {
                                list.Add(item);
                            }
                        }
                    }
                }

                context = list;
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(context, setting), "application/json", Encoding.UTF8);
        }

        [HttpGet]
        [Route("api/ProductBaTakhfif/{id}")]
        public async Task<IActionResult> ProductBaTakhfif(int id)
        {
            var context = _context.Product.Find(id);

            return Content(JsonConvert.SerializeObject(new { message = t.MablaghBaTakhfif(context) }), "application/json", Encoding.UTF8);

        }

        [HttpGet]
        [Route("api/Products/{id}")]
        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.PrGroup)
                .FirstOrDefaultAsync(m => m.id == id);
            product.seen++;
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
            if (product == null)
            {
                return NotFound();
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(product, setting), "application/json", Encoding.UTF8);

        }


        [HttpGet]
        [Route("api/LastBasket/{id}")]
        [Authorize]
        public async Task<IActionResult> LastBasket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .Include(p => p.BasketItems).OrderByDescending(w=>w.createdate)
                .FirstOrDefaultAsync(m => m.userid == id&&m.ispay==false);
            if (basket == null)
            {
               return BadRequest(new { message = "سبد خرید شما خالی است" });
            }

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

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(basket, setting), "application/json", Encoding.UTF8);

        }

        [HttpGet]
        [Route("api/BasketOK/")]
        [Authorize]
        public async Task<IActionResult> BasketOK(int? id,int address)
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
                return BadRequest(new { message = "سبد خرید شما خالی است" });
            }
            basket.addressid = address;
            basket.ispay = true;
            basket.paydate = System.DateTime.Now;
            _context.Basket.Update(basket);
            await _context.SaveChangesAsync();
            int ready = 0;
            int send = 0;
            foreach (var item in basket.BasketItems)
            {
                if (item.Product.readyday>ready)
                {
                    ready = item.Product.readyday;
                }
                if (item.Product.sendday > send)
                {
                    send = item.Product.sendday;
                }
            }
            return Ok(new { message = " مدت زمان آماده سازی نهایتا"+ready+" روز و مدت زمان رسیدن محصولات به مقصد نهایتا "+send+" روز می باشد. " });

        }

        [HttpPost]
        [Route("api/BasketAdd/")]
        [Authorize]
        public async Task<IActionResult> BasketAdd([FromForm]int userid, [FromForm] int prid, [FromForm] int tedad)
        {
            if (userid == 0)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(prid);
            if ((product.count- tedad) < 0)
            {
                return BadRequest(new { message = "میزان درخواستی بیشتر از موجودی انبار است." });
            }
            var basket = await _context.Basket.OrderByDescending(w => w.createdate)
                .FirstOrDefaultAsync(m => m.userid == userid && m.ispay == false);
            if (basket==null)
            {
                basket=new Basket
                {
                    userid = userid,
                    createdate = System.DateTime.Now,
                    iscansel=false,
                    ispay=false,
                    isready=false,
                    issend=false,
                    paydate= System.DateTime.Now,
                    senddate= System.DateTime.Now,
                    paymentid=0,
                    total= t.MablaghBaTakhfif(product) * tedad,
                    addressid=_context.Address.FirstOrDefault(w=>w.personid==userid).id
                };
                _context.Basket.Add(basket);
                await _context.SaveChangesAsync();

                _context.BasketItem.Add(new BasketItem
                {
                    basketid=basket.id,
                    productid=product.id,
                    count= tedad,
                    mablagh=t.MablaghBaTakhfif(product)*tedad
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                if (basket.BasketItems.Any(w=>w.productid==product.id))
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
                        mablagh = t.MablaghBaTakhfif(product)*tedad
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
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(basket, setting), "application/json", Encoding.UTF8);
        }

        [HttpPost]
        [Route("api/BasketDown/")]
        [Authorize]
        public async Task<IActionResult> BasketDown([FromForm] int? id)
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
            if (item.count-1==0)
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

            var basket= await _context.Basket.FirstOrDefaultAsync(m => m.id == item.basketid);
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

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(basket, setting), "application/json", Encoding.UTF8);

        }

        [HttpPost]
        [Route("api/BasketItemDelete/")]
        [Authorize]
        public async Task<IActionResult> BasketItemDelete([FromForm] int? id)
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

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(basket, setting), "application/json", Encoding.UTF8);

        }
    }
}
