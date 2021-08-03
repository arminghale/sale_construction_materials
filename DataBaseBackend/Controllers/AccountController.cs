using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataBaseBackend.Controllers
{
    [DisableRequestSizeLimit]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        readonly Context db;
        readonly Random random;
        IViewRenderService vi;
        private IWebHostEnvironment _enviroment;
        IUser u;
        public AccountController(Context db, IViewRenderService vi, IWebHostEnvironment enviroment)
        {
            this.db = db;
            u = new UserService(db);
            random = new Random();
            this.vi = vi;
            _enviroment = enviroment;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login(string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                ViewBag.url = ReturnUrl;
            }
            return View();
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Login login, System.Uri ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var check = u.check(login.username.ToLower(), login.password);
                if (check != null)
                {
                    if (check == "ok")
                    {
                        var user = u.whichuser(login.username.ToLower(), login.password);
                        if (!user.isactive)
                        {
                            user.activecode = random.Next(10000);
                            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            await db.SaveChangesAsync();
                            SendMail.Send(user.email, "کد فعالسازی", user.activecode.ToString());
                            if (user.roleid == 2)
                            {
                                if (user.Person == null)
                                {
                                    return Redirect("/ActiveAccount/" + user.id);
                                }
                                else
                                {
                                    return Redirect("/ActiveAccount/?id=" + user.id + "&logback=true");
                                }
                            }
                            else
                            {
                                return Redirect("/ActiveAccount/?id=" + user.id + "&logback=true");
                            }


                        }
                        else if (user.isactive && user.Person == null && user.roleid == 2)
                        {
                            return Redirect("/Register2/" + user.id);
                        }
                        else
                        {
                            user.lastlogin = System.DateTime.Now;
                            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            await db.SaveChangesAsync();
                            const string Issuer = "https://localhost:44390";
                            var claims = new List<Claim>();
                            claims.Add(new Claim(ClaimTypes.Name, login.username.ToLower(), ClaimValueTypes.String, Issuer));
                            if (user.Person != null)
                            {
                                claims.Add(new Claim("name", user.Person.name + " " + user.Person.lastname, ClaimValueTypes.String, Issuer));
                                claims.Add(new Claim("userid", user.id.ToString(), ClaimValueTypes.String, Issuer));
                            }
                            claims.Add(new Claim(ClaimTypes.Role, user.Role.title, ClaimValueTypes.String, Issuer));

                            var userIdentity = new ClaimsIdentity(user.Role.title);
                            userIdentity.AddClaims(claims);
                            var userPrincipal = new ClaimsPrincipal(userIdentity);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                            if (ReturnUrl != null)
                            {
                                return Redirect(ReturnUrl.ToString());
                            }
                            return Redirect("/");
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("password", check);
                        return View(login);
                    }
                }
                ModelState.AddModelError("password", "خطا");
                return View(login);
            }
            return View(login);
        }


        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }



        [HttpGet]
        [Route("Register")]
        public IActionResult Register1()
        {
            return View();
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register1(Register1 register)
        {
            if (ModelState.IsValid)
            {
                if (register.password != register.repassword)
                {
                    ModelState.AddModelError("repassword", "رمز عبور و تکرار رمز عبور متفاوت هستند.");
                    return View(register);
                }
                if (!u.emailexist(register.email.ToLower()))
                {
                    if (!u.usernameexist(register.username.ToLower()))
                    {
                        User user = new User
                        {
                            email = register.email.ToLower(),
                            username = register.username.ToLower(),
                            password = register.password,
                            activecode = random.Next(10000),
                            isactive = false,
                            lastlogin = System.DateTime.Now,
                            roleid = 2
                        };
                        await db.User.AddAsync(user);
                        await db.SaveChangesAsync();
                        SendMail.Send(register.email.ToLower(), "کد فعالسازی", user.activecode.ToString());
                        return Redirect("/ActiveAccount/" + user.id);
                    }
                    else
                    {
                        ModelState.AddModelError("username", "کاربری با این نام کاربری قبلا ثبت نام کرده است.");
                        return View(register);
                    }
                }
                else
                {
                    ModelState.AddModelError("email", "کاربری با این ایمیل قبلا ثبت نام کرده است.");
                    return View(register);
                }
            }
            return View(register);
        }



        [HttpGet]
        [Route("ActiveAccount/{id}")]
        public IActionResult Active(int id, bool logback = false)
        {
            ViewBag.id = id;
            ViewBag.logback = logback;
            return View();
        }
        [HttpPost]
        [Route("ActiveAccount/{id}")]
        public async Task<IActionResult> Active(int activecode, int id, bool logback = false)
        {
            ViewBag.code = activecode;
            ViewBag.id = id;
            ViewBag.logback = logback;
            User user = await db.User.FindAsync(id);
            if (user == null)
            {
                ModelState.AddModelError("password", "کاربری یافت نشد.");
                return View();
            }
            if (user.activecode != activecode)
            {
                ModelState.AddModelError("password", "کد وارد شده اشتباه است");
                return View();
            }
            user.isactive = true;
            db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await db.SaveChangesAsync();
            if (logback)
            {
                user.lastlogin = System.DateTime.Now;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                const string Issuer = "https://localhost:44390";
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.username, ClaimValueTypes.String, Issuer));
                claims.Add(new Claim(ClaimTypes.Role, user.Role.title, ClaimValueTypes.String, Issuer));

                var userIdentity = new ClaimsIdentity(user.Role.title);
                userIdentity.AddClaims(claims);
                var userPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return Redirect("/");
            }
            return Redirect("/Register2/" + id);
            //return View();
        }



        [HttpGet]
        [Route("Register2/{id}")]
        public IActionResult Register2(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [Route("Register2/{id}")]
        public async Task<IActionResult> Register2(Register2 register, int id)
        {
            List<State> states = JsonConvert.DeserializeObject<List<State>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json"));
            List<City> cities = JsonConvert.DeserializeObject<List<City>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json"));
            ViewBag.id = id;
            if (ModelState.IsValid)
            {
                if (register.ostan == -1)
                {
                    ModelState.AddModelError("ostan", "لطفا استان را انتخاب کنید.");
                    return View(register);
                }
                if (register.shahr == -1)
                {
                    ModelState.AddModelError("ostan", "لطفا شهر را انتخاب کنید.");
                    return View(register);
                }
                User user = await db.User.FindAsync(id);
                if (user == null)
                {
                    ModelState.AddModelError("name", "کاربری یافت نشد.");
                    return View(register);
                }
                Person person = new Person
                {
                    name = register.name,
                    lastname = register.lastname,
                    phonenumber = register.phonenumber,
                    id = id
                };
                await db.Person.AddAsync(person);
                await db.SaveChangesAsync();
                Address address = new Address
                {
                    codeposti = register.codeposti,
                    text = register.text,
                    girandename = register.name,
                    girandelastname = register.lastname,
                    girandephonenumber = register.phonenumber,
                    personid = id

                };
                address.ostan = states.FirstOrDefault(w => w.id == register.ostan).name;
                address.shahr = cities.FirstOrDefault(w => w.id == register.shahr).name;
                await db.Address.AddAsync(address);
                await db.SaveChangesAsync();

                user.lastlogin = System.DateTime.Now;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                const string Issuer = "https://localhost:44390";
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.username, ClaimValueTypes.String, Issuer));
                claims.Add(new Claim(ClaimTypes.Role, user.Role.title, ClaimValueTypes.String, Issuer));

                var userIdentity = new ClaimsIdentity(user.Role.title);
                userIdentity.AddClaims(claims);
                var userPrincipal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

                return Redirect("/");
            }
            return View(register);
        }



        [HttpGet]
        [Route("ForgetPass")]
        public IActionResult ForgetPass()
        {
            return View();
        }
        [HttpPost]
        [Route("ForgetPass")]
        public async Task<IActionResult> ForgetPass(string email)
        {
            ViewBag.email = email;
            if (ModelState.IsValid)
            {
                if (!u.emailexist(email.ToLower()))
                {
                    ModelState.AddModelError("password", "کاربری یافت نشد.");
                    return View();
                }
                User user = db.User.FirstOrDefault(w => w.email == email.ToLower());
                user.activecode = random.Next(10000);
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                SendMail.Send(email.ToLower(), "کد فعالسازی", user.activecode.ToString());
                return Redirect("/ForgetPass2/?email=" + email.ToLower());
            }
            return View();
        }

        [HttpGet]
        [Route("ForgetPass2")]
        public IActionResult ForgetPass2(string email)
        {
            ViewBag.email = email;
            return View();
        }
        [HttpPost]
        [Route("ForgetPass2")]
        public async Task<IActionResult> ForgetPass2(string email, int activecode)
        {
            ViewBag.email = email;
            if (ModelState.IsValid)
            {
                if (!u.emailexist(email))
                {
                    ModelState.AddModelError("password", "کاربری یافت نشد.");
                    return View();
                }
                User user = db.User.FirstOrDefault(w => w.email == email);
                if (user.activecode != activecode)
                {
                    ModelState.AddModelError("password", "کد وارد شده اشتباه است");
                    return View();
                }
                return Redirect("/ForgetPass3/?email=" + email + "&activecode=" + activecode);
            }
            return View();
        }

        [HttpGet]
        [Route("ForgetPass3")]
        public IActionResult ForgetPass3(string email, int activecode)
        {
            ViewBag.email = email;
            ViewBag.code = activecode;
            return View();
        }
        [HttpPost]
        [Route("ForgetPass3")]
        public async Task<IActionResult> ForgetPass3(string email, int activecode, string password, string repassword)
        {
            ViewBag.email = email;
            if (ModelState.IsValid)
            {
                if (!u.emailexist(email))
                {
                    ModelState.AddModelError("password", "کاربری یافت نشد.");
                    return View();
                }
                User user = db.User.FirstOrDefault(w => w.email == email);
                if (user.activecode != activecode)
                {
                    ModelState.AddModelError("password", "کد وارد شده اشتباه است");
                    return View();
                }
                if (password != repassword)
                {
                    ModelState.AddModelError("repassword", "رمزعبور و تکرار رمز عبور متفاوت اند.");
                    return View();
                }
                user.password = password;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Redirect("/Login");
            }
            return View();
        }

    }
}
