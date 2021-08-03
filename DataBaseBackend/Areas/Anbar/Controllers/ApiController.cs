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

namespace DataBaseBackend.Areas.Anbar.Controllers
{
    [ApiController]
    [Area("Anbar")]
    [Authorize(Policy = "JWTAnbar")]
    public class ApiController : ControllerBase
    {
        private readonly Context _context;
        readonly Random random;
        private IWebHostEnvironment _enviroment;
        ITakhfif t;

        public ApiController(Context context, IWebHostEnvironment enviroment)
        {
            _context = context;
            random = new Random();
            _enviroment = enviroment;
            t = new TakhfifService(context);
        }

        [HttpPost]
        [Route("Anbar/api/ChangePass")]
        public async Task<IActionResult> ChangePass([FromForm] string id, [FromForm] string currentpassword, [FromForm] string password, [FromForm] string repassword)
        {
            if (ModelState.IsValid)
            {
                DataBaseBackend.User user = await _context.User.FindAsync(int.Parse(id));
                if (user == null)
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                if (user.password != currentpassword)
                {
                    return BadRequest(new { message = "رمزعبور فعلی اشتباه است." });
                }
                if (password != repassword)
                {
                    return BadRequest(new { message = "رمزعبور و تکرار رمز عبور متفاوت اند." });
                }
                user.password = password;
                _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpGet]
        [Route("Anbar/api/PrGroupIndex")]
        public async Task<IActionResult> PrGroupIndex(string search = "")
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
        [Route("Anbar/api/ProductIndex")]
        public async Task<IActionResult> ProductIndex(int no = -1, string search = "", int gid = -1)
        {
            //PrGroupIndex
            var context = await _context.Product.OrderByDescending(w => w.createdate).ToListAsync();
            if (gid != -1)
            {
                context = context.Where(w => w.prgroupid == gid).ToList();
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
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(context, setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Anbar/api/ProductDetails/{id}")]
        public async Task<IActionResult> ProductDetails(int? id)
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
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(product, setting), "application/json", Encoding.UTF8);

        }


        [HttpPost]
        [Route("Anbar/api/ProductUpdate/")]
        public async Task<IActionResult> ProductEdit([FromForm] int? id, [FromForm] int count)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.PrGroup)
                .FirstOrDefaultAsync(m => m.id == id);
            product.count = count;
            _context.Update(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
