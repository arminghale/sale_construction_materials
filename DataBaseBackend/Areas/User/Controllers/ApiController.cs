using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace DataBaseBackend.Areas.User.Controllers
{
    [ApiController, DisableRequestSizeLimit]
    [Authorize(Policy = "JWTAll")]
    [Area("User")]
    public class ApiController : ControllerBase
    {
        readonly Context db;
        readonly Random random;
        IViewRenderService vi;
        private IWebHostEnvironment _enviroment;
        IUser u;
        public ApiController(Context db, IViewRenderService vi, IWebHostEnvironment enviroment)
        {
            this.db = db;
            u = new UserService(db);
            random = new Random();
            this.vi = vi;
            _enviroment = enviroment;
        }


        [HttpPost]
        [Route("User/api/ChangePass")]
        public async Task<IActionResult> ChangePass([FromForm] string id, [FromForm] string currentpassword, [FromForm] string password, [FromForm] string repassword)
        {
            if (ModelState.IsValid)
            {
                DataBaseBackend.User user = await db.User.FindAsync(int.Parse(id));
                if (user == null)
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                if (user.password!=currentpassword)
                {
                    return BadRequest(new { message = "رمزعبور فعلی اشتباه است." });
                }
                if (password != repassword)
                {
                    return BadRequest(new { message = "رمزعبور و تکرار رمز عبور متفاوت اند." });
                }
                user.password = password;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpPost]
        [Route("User/api/ChangeInfo")]
        public async Task<IActionResult> ChangeInfo([FromBody] ChangeInfo info)
        {
            if (ModelState.IsValid)
            {
                DataBaseBackend.User user =await db.User.FindAsync(info.id.Value);
                DataBaseBackend.User useremail = db.User.FirstOrDefault(w=>w.email==info.email);
                DataBaseBackend.User userusername = db.User.FirstOrDefault(w => w.username == info.username);


                if (useremail==null||user==useremail)
                {
                    if (userusername == null || user == userusername)
                    {
                        user.email = info.email;
                        user.username = info.username;
                        user.Person.name = info.name;
                        user.Person.lastname = info.lastname;
                        user.Person.phonenumber = info.phonenumber;
                        db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        await db.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new { message = "کاربری با این نام کاربری قبلا ثبت نام کرده است." });
                    }
                }
                else
                {
                    return BadRequest(new { message = "کاربری با این ایمیل قبلا ثبت نام کرده است." });
                }
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpPost]
        [Route("User/api/ChangeAddress")]
        public async Task<IActionResult> ChangeAddress([FromForm] int id,[FromForm] int personid, [FromForm] int ostan, [FromForm] int shahr,
            [FromForm] int girande, [FromForm] string codeposti, [FromForm] string text, [FromForm] string girandename,
            [FromForm] string girandelastname, [FromForm] string girandephonenumber)
        {
            List<State> states = JsonConvert.DeserializeObject<List<State>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json"));
            List<City> cities = JsonConvert.DeserializeObject<List<City>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json"));

            if (ModelState.IsValid)
            {
                Address address = await db.Address.FindAsync(id);
                if (address!=null)
                {
                    if (string.IsNullOrEmpty(address.codeposti))
                    {
                        return BadRequest(new { message = "لطفا کد پستی را وارد کنید" });
                    }
                    if (string.IsNullOrEmpty(address.text))
                    {
                        return BadRequest(new { message = "لطفا آدرس را وارد کنید" });
                    }
                    if (ostan == -1)
                    {
                        return BadRequest(new { message = "لطفا استان را انتخاب کنید" });
                    }
                    if (shahr == -1)
                    {
                        return BadRequest(new { message = "لطفا شهر را انتخاب کنید" });
                    }
                    if (girande == 1)
                    {
                        if (string.IsNullOrEmpty(address.girandename))
                        {
                            return BadRequest(new { message = "لطفا نام گیرنده را وارد کنید" });
                        }
                        if (string.IsNullOrEmpty(address.girandelastname))
                        {
                            return BadRequest(new { message = "لطفا نام خانوادگی گیرنده را وارد کنید" });
                        }
                        if (string.IsNullOrEmpty(address.girandephonenumber))
                        {
                            return BadRequest(new { message = "لطفا شماره تلفن گیرنده را وارد کنید" });
                        }

                    }
                    address.ostan = states.FirstOrDefault(w => w.id == ostan).name;
                    address.shahr = cities.FirstOrDefault(w => w.id == shahr).name;
                    address.codeposti = codeposti;
                    Person person = db.Person.Find(address.personid);
                    if (girande == 0)
                    {
                        address.girandename = person.name;
                        address.girandelastname = person.lastname;
                        address.girandephonenumber = person.phonenumber;
                    }
                    else
                    {
                        address.girandelastname = girandelastname;
                        address.girandename = girandename;
                        address.girandephonenumber = girandephonenumber;
                    }
                    db.Entry(address).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest(new { message = "آدرسی یافت نشد." });
                }
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpPost]
        [Route("User/api/AddAddress")]
        public async Task<IActionResult> UsersAddressCreate([FromForm] int personid, [FromForm] int ostan, [FromForm] int shahr,
            [FromForm] int girande, [FromForm] string codeposti, [FromForm] string text, [FromForm] string girandename,
            [FromForm] string girandelastname, [FromForm] string girandephonenumber)
        {
            Address address = new Address
            {
                personid = personid,
                text = text,
                codeposti = codeposti,
                girandelastname = girandelastname,
                girandename = girandename,
                girandephonenumber = girandephonenumber
            };
            List<State> states = JsonConvert.DeserializeObject<List<State>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json"));
            List<City> cities = JsonConvert.DeserializeObject<List<City>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json"));
            if (string.IsNullOrEmpty(address.codeposti))
            {
                return BadRequest(new { message = "لطفا کد پستی را وارد کنید" });
            }
            if (string.IsNullOrEmpty(address.text))
            {
                return BadRequest(new { message = "لطفا آدرس را وارد کنید" });
            }
            if (ostan == -1)
            {
                return BadRequest(new { message = "لطفا استان را انتخاب کنید" });
            }
            if (shahr == -1)
            {
                return BadRequest(new { message = "لطفا شهر را انتخاب کنید" });
            }
            if (girande == 1)
            {
                if (string.IsNullOrEmpty(address.girandename))
                {
                    return BadRequest(new { message = "لطفا نام گیرنده را وارد کنید" });
                }
                if (string.IsNullOrEmpty(address.girandelastname))
                {
                    return BadRequest(new { message = "لطفا نام خانوادگی گیرنده را وارد کنید" });
                }
                if (string.IsNullOrEmpty(address.girandephonenumber))
                {
                    return BadRequest(new { message = "لطفا شماره تلفن گیرنده را وارد کنید" });
                }

            }
            Person person = db.Person.Find(address.personid);
            address.ostan = states.FirstOrDefault(w => w.id == ostan).name;
            address.shahr = cities.FirstOrDefault(w => w.id == shahr).name;
            if (girande == 0)
            {
                address.girandename = person.name;
                address.girandelastname = person.lastname;
                address.girandephonenumber = person.phonenumber;
            }
            db.Add(address);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("User/api/AllAddress/{id}")]
        public async Task<IActionResult> AllAddress(int id)
        {
            if (ModelState.IsValid)
            {
                var orders = db.Address.Where(w => w.personid == id).ToList();
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.MaxDepth = 1;
                return Content(JsonConvert.SerializeObject(orders,setting), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpGet]
        [Route("User/api/Orders/{id}")]
        public async Task<IActionResult> Orders(int id)
        {
            if (ModelState.IsValid)
            {
                var orders = db.Basket.Where(w => w.userid == id && w.ispay == true).OrderByDescending(w=>w.paydate).ToList();
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.MaxDepth = 2;
                return Content(JsonConvert.SerializeObject(orders,setting), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpGet]
        [Route("User/api/OneOrders/{id}")]
        public async Task<IActionResult> OneOrders(int id)
        {
            if (ModelState.IsValid)
            {
                var orders = db.Basket.Find(id);
                return Content(JsonConvert.SerializeObject(orders), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }


        [HttpGet]
        [Route("User/api/AllRecMessages/{id}")]
        public async Task<IActionResult> AllRecMessages(int id)
        {
            if (ModelState.IsValid)
            {
                var messages = db.Message.Where(w => w.userid == id).OrderByDescending(w => w.createdate).ToList();
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.MaxDepth = 2;
                return Content(JsonConvert.SerializeObject(messages,setting), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpGet]
        [Route("User/api/AllSendMessages/{id}")]
        public async Task<IActionResult> AllSendMessages(int id)
        {
            if (ModelState.IsValid)
            {
                var messages = db.Message.Where(w => w.user2id == id).OrderByDescending(w => w.createdate).ToList();
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.MaxDepth = 2;
                return Content(JsonConvert.SerializeObject(messages, setting), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpGet]
        [Route("User/api/OneMessages/{id}")]
        public async Task<IActionResult> OneMessages(int id)
        {
            if (ModelState.IsValid)
            {
                var messages =await db.Message.FindAsync(id);
                messages.isseen = true;
                db.Entry(messages).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
    }
}
