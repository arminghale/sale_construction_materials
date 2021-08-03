using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataBaseBackend;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CookiesAdmin")]
    public class UsersController : Controller
    {
        private readonly Context _context;
        readonly Random random;
        private IWebHostEnvironment _enviroment;

        public UsersController(Context context, IWebHostEnvironment enviroment)
        {
            _context = context;
            random = new Random();
            _enviroment = enviroment;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index(string search="",int role=-1)
        {
            ViewData["roleid"] = new SelectList(_context.Role.Where(w=>w.id!=1), "id", "title");
            var context =await _context.User.Where(w=>w.roleid!=1).ToListAsync();
            if (role!=-1)
            {
                context = context.Where(w => w.roleid == role).ToList();
                ViewData["roleid"] = new SelectList(_context.Role.Where(w => w.id != 1), "id", "title",role);
            }
            if (!string.IsNullOrEmpty(search))
            {
                context = context.Where(w => w.username.Contains(search) || w.email.Contains(search) || w.id.ToString().Contains(search)).ToList();
                context.Concat(context.Where(w => w.Person != null && w.Person.name.Contains(search) ||
                  w.Person.lastname.Contains(search) || w.Person.phonenumber.Contains(search))).ToList();
            }
            return View(context);
        }

        // GET: Admin/Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Admin/Users/Create
        public IActionResult Create()
        {
            ViewData["roleid"] = new SelectList(_context.Role.Where(w=>w.id!=1), "id", "title");
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,roleid,email,username,password,lastlogin,activecode,isactive")] DataBaseBackend.User user)
        {
            ViewData["roleid"] = new SelectList(_context.Role.Where(w => w.id != 1), "id", "title", user.roleid);
            if (ModelState.IsValid)
            {
                if (_context.User.Any(w=>w.email==user.email.ToLower()))
                {
                    ModelState.AddModelError("email", "کاربری با این ایمیل ثبت شده است");
                    return View(user);
                }
                if (_context.User.Any(w => w.username == user.username.ToLower()))
                {
                    ModelState.AddModelError("username", "کاربری با این نام کاربری ثبت شده است");
                    return View(user);
                }
                user.email = user.email.ToLower();
                user.username = user.username.ToLower();
                user.isactive = true;
                user.activecode = random.Next(10000);
                user.lastlogin = System.DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["roleid"] = new SelectList(_context.Role.Where(w => w.id != 1), "id", "title",user.roleid);
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,roleid,email,username,password,lastlogin,activecode,isactive")] DataBaseBackend.User user)
        {
            ViewData["roleid"] = new SelectList(_context.Role.Where(w => w.id != 1), "id", "title", user.roleid);
            if (id != user.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.User.Any(w => w.email == user.email.ToLower() && w.id!=user.id))
                    {
                        ModelState.AddModelError("email", "کاربری با این ایمیل ثبت شده است");
                        return View(user);
                    }
                    if (_context.User.Any(w => w.username == user.username.ToLower() && w.id != user.id))
                    {
                        ModelState.AddModelError("username", "کاربری با این نام کاربری ثبت شده است");
                        return View(user);
                    }
                    user.email = user.email.ToLower();
                    user.username = user.username.ToLower();
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.id))
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
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //var user = await _context.User
            //    .Include(u => u.Role)
            //    .FirstOrDefaultAsync(m => m.id == id);
            //if (user == null)
            //{
            //    return NotFound();
            //}

            //return View(user);
        }

        public IActionResult Person(int id)
        {
            ViewBag.id = id;
            if (_context.Person.Any(w=>w.id==id))
            {
                return View(_context.Person.Find(id));
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Person(Person person)
        {   if (ModelState.IsValid)
            {
                if (_context.Person.Any(w=>w==person))
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(person);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }


        public IActionResult AddressList(int id)
        {
            ViewBag.id = id;
            return View(_context.Address.Where(w=>w.personid==id));
        }
        public IActionResult AddressCreate(int personid)
        {
            ViewBag.personid = personid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddressCreate(Address address,int ostan,int shahr,int girande)
        {
            ViewBag.personid = address.personid;
            List<State> states = JsonConvert.DeserializeObject<List<State>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json"));
            List<City> cities = JsonConvert.DeserializeObject<List<City>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json"));
            if (string.IsNullOrEmpty(address.codeposti))
            {
                ModelState.AddModelError("codeposti", "لطفا کد پستی را وارد کنید");
                return View(address);
            }
            if (string.IsNullOrEmpty(address.text))
            {
                ModelState.AddModelError("text", "لطفا آدرس را وارد کنید");
                return View(address);
            }
            if (ostan==-1)
                {
                    ModelState.AddModelError("ostan","لطفا استان را انتخاب کنید");
                    return View(address);
                }
                if (shahr == -1)
                {
                    ModelState.AddModelError("ostan", "لطفا شهر را انتخاب کنید");
                    return View(address);
                }
                if (girande==1)
                {
                    if (string.IsNullOrEmpty(address.girandename))
                    {
                        ModelState.AddModelError("girandename", "لطفا نام گیرنده را وارد کنید");
                        return View(address);
                    }
                    if (string.IsNullOrEmpty(address.girandelastname))
                    {
                        ModelState.AddModelError("girandelastname", "لطفا نام خانوادگی گیرنده را وارد کنید");
                        return View(address);
                    }
                    if (string.IsNullOrEmpty(address.girandephonenumber))
                    {
                        ModelState.AddModelError("girandephonenumber", "لطفا شماره تلفن گیرنده را وارد کنید");
                        return View(address);
                    }
                    
                }
                Person person = _context.Person.Find(address.personid);
                address.ostan = states.FirstOrDefault(w=>w.id==ostan).name;
                address.shahr = cities.FirstOrDefault(w=>w.id== shahr).name;
                if (girande==0)
                {
                    address.girandename = person.name;
                    address.girandelastname = person.lastname;
                    address.girandephonenumber = person.phonenumber;
                }
                _context.Add(address);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddressList),new {id=person.id });
            return View(address);
        }

        public async Task<IActionResult> AddressDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Address.FindAsync(id);
            var idd = user.personid;
            _context.Address.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddressList),new {id=idd });
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.id == id);
        }

        public IActionResult State()
        {
            var json = System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json");

            return Content(json,"application/json",System.Text.Encoding.UTF8);

        }
        public IActionResult City()
        {
            var json = System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json");

            return Content(json, "application/json", System.Text.Encoding.UTF8);

        }
    }
}
