using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronPdf;
using Microsoft.AspNetCore.Http;

namespace DataBaseBackend.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Policy ="CookiesAll")]
    public class HomeController : Controller
    {
        readonly Context db;
        readonly Random random;
        IViewRenderService vi;
        private IWebHostEnvironment _enviroment;
        IUser u;
        public HomeController(Context db, IViewRenderService vi, IWebHostEnvironment enviroment)
        {
            this.db = db;
            u = new UserService(db);
            random = new Random();
            this.vi = vi;
            _enviroment = enviroment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("User/ChangePass")]
        public IActionResult ChangePass()
        {
            ViewBag.id = db.User.FirstOrDefault(w => w.username == User.Identity.Name).id;
            return View();
        }
        [HttpPost]
        [Route("User/ChangePass")]
        public async Task<IActionResult> ChangePass( string id,string currentpassword,string password, string repassword)
        {
            if (ModelState.IsValid)
            {
                DataBaseBackend.User user = await db.User.FindAsync(int.Parse(id));
                if (user == null)
                {
                    ModelState.AddModelError("password", "کاربری یافت نشد.");
                    return View();
                }
                if (user.password != currentpassword)
                {
                    ModelState.AddModelError("password", "رمزعبور فعلی اشتباه است.");
                    return View();
                }
                if (password != repassword)
                {
                    ModelState.AddModelError("password", "رمزعبور و تکرار رمز عبور متفاوت اند.");
                    return View();
                }
                user.password = password;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [Route("User/Addresses")]
        public IActionResult Addresses()
        {
            var list = db.User.FirstOrDefault(w => w.username == User.Identity.Name).Person.Addresses;
            ViewBag.personid = db.User.FirstOrDefault(w => w.username == User.Identity.Name).Person.id;
            return View(list);
        }
        public async Task<IActionResult> AddressesDelete(int id)
        {
            var user = await db.Address.FindAsync(id);
            db.Address.Remove(user);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Addresses));
        }
        [Route("User/Addresses")]
        [HttpPost]
        public async Task<IActionResult> AddressesAdd( int personid, int ostan, int shahr,
            int girande, string codeposti, string text, string girandename,
             string girandelastname,  string girandephonenumber)
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
            return RedirectToAction(nameof(Addresses));
        }


        [Route("User/ChangeInfo")]
        public IActionResult ChangeInfo()
        {
            var user = db.User.FirstOrDefault(w => w.username == User.Identity.Name);
            ChangeInfo changeInfo = new ChangeInfo
            {
                name=user.Person.name,
                lastname=user.Person.lastname,
                phonenumber=user.Person.phonenumber,
                email=user.email,
                id=user.id,
                username=user.username
            };
            return View(changeInfo);
        }
        [Route("User/ChangeInfo")]
        [HttpPost]
        public async Task<IActionResult> ChangeInfo(ChangeInfo info)
        {
            if (ModelState.IsValid)
            {
                DataBaseBackend.User user = await db.User.FindAsync(info.id.Value);
                DataBaseBackend.User useremail = db.User.FirstOrDefault(w => w.email == info.email);
                DataBaseBackend.User userusername = db.User.FirstOrDefault(w => w.username == info.username);


                if (useremail == null || user == useremail)
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
                        return RedirectToAction(nameof(ChangeInfo));
                    }
                    else
                    {
                        ModelState.AddModelError("username", "کاربری با این نام کاربری قبلا ثبت نام کرده است.");
                        return View(info);
                    }
                }
                else
                {
                    ModelState.AddModelError("email", "کاربری با این ایمیل قبلا ثبت نام کرده است.");
                    return View(info);
                }
            }
            return View(info);
        }



        [Route("User/Orders")]
        public IActionResult Orders()
        {
            var list = db.User.FirstOrDefault(w => w.username == User.Identity.Name).Baskets.Where(w=>w.ispay);
            return View(list);
        }
        [Route("User/GetFaktor")]
        public ActionResult GetFaktor(int id)
        {
            var list = db.User.FirstOrDefault(w => w.username == User.Identity.Name).Baskets.Find(w=>w.id==id);
            string body = vi.RenderToStringAsync("Faktor", list);
            var Renderer = new IronPdf.HtmlToPdf();
            var PDF = Renderer.RenderHtmlAsPdf(body);
            return new FileStreamResult(PDF.Stream, "application/pdf")
            {
                FileDownloadName = "Faktor.pdf"
            };
            //return View();
        }


        [Route("User/Messages")]
        public IActionResult Messages()
        {
            var list = db.User.FirstOrDefault(w => w.username == User.Identity.Name).Messages2;
            ViewBag.list2= db.User.FirstOrDefault(w => w.username == User.Identity.Name).Messages;
            return View(list);
        }
        [Route("User/Messages")]
        [HttpPost]
        public async Task<IActionResult> Messages(string message)
        {
            var list = db.User.FirstOrDefault(w => w.username == User.Identity.Name);
            Message mess = new Message
            {
                text = message,
                createdate = System.DateTime.Now,
                user2id = list.id
            };
            mess.userid = db.User.FirstOrDefault(w => w.roleid == 1).id;
            await db.Message.AddAsync(mess);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Messages));
        }
    }
}
