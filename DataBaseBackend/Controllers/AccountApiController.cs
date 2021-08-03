using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseBackend.Controllers
{
    [ApiController, DisableRequestSizeLimit]
    //[Produces("application/json")]

    public class AccountApiController : ControllerBase
    {
        readonly Context db;
        readonly Random random;
        IViewRenderService vi;
        private IWebHostEnvironment _enviroment;
        IUser u;
        public AccountApiController(Context db, IViewRenderService vi, IWebHostEnvironment enviroment)
        {
            this.db = db;
            u = new UserService(db);
            random = new Random();
            this.vi = vi;
            _enviroment = enviroment;
        }


        [HttpPost]
        [Route("api/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]Login login)
        {
            var check = u.check(login.username.ToLower(), login.password);
            if (check != null)
            {
                if (check == "ok")
                {
                    var user = u.whichuser(login.username.ToLower(), login.password);
                    user.lastlogin = System.DateTime.Now;
                    db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await db.SaveChangesAsync();
                    const string Issuer = "https://localhost:44390";
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("azdatabasemazidi"));

                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var tokeOptions = new JwtSecurityToken(
                          expires: DateTime.Now.AddYears(5),
                          issuer: Issuer,
                          audience: Issuer,
                          claims: new List<Claim>
                          {
                            new Claim(ClaimTypes.Role, user.Role.title)
                          },
                          signingCredentials: signinCredentials
                      );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    //var claims = new List<Claim>();
                    //claims.Add(new Claim(ClaimTypes.Name, login.username, ClaimValueTypes.String, Issuer));
                    //claims.Add(new Claim(ClaimTypes.Role, user.Role.title, ClaimValueTypes.String, Issuer));

                    //var userIdentity = new ClaimsIdentity(user.Role.title);
                    //userIdentity.AddClaims(claims);
                    //var userPrincipal = new ClaimsPrincipal(userIdentity);
                    //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
                    //string value = null;
                    return Ok(new { cookie=tokenString,id=user.id});
                }
                else
                {
                    return BadRequest(new { message = check });
                }
            }
            return BadRequest(new { message = "خطا!" });
        }


        [Authorize]
        [HttpPost]
        [Route("api/Profile")]
        public async Task<IActionResult> Profile([FromForm] int id)
        {
            if (db.User.Any(w=>w.id==id))
            {
                User user = await db.User.FindAsync(id);
                return Content(JsonConvert.SerializeObject(user), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "کاربری یافت نشد." });
        }



        [HttpPost]
        [Route("api/Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register1([FromBody] Register1 register)
        {
            if (ModelState.IsValid)
            {
                if (register.password != register.repassword)
                {
                    return BadRequest(new { message = "رمز عبور و تکرار رمز عبور متفاوت هستند." });
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
                        return Content(JsonConvert.SerializeObject(new { id=user.id}), "application/json", Encoding.UTF8);
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
        [Route("api/SendActiveCode/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Active([FromForm] int id)
        {
            if (ModelState.IsValid)
            {
                User user = await db.User.FindAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                user.activecode = random.Next(10000);
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }


        [HttpPost]
        [Route("api/ActiveAccount/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Active([FromForm] int activecode, int id)
        {
            if (ModelState.IsValid)
            {
                User user = await db.User.FindAsync(id);
                if (user == null)
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                if (user.activecode != activecode)
                {
                    return BadRequest(new { message = "کد وارد شده اشتباه است" });
                }
                user.isactive = true;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpPost]
        [Route("api/Register2/")]
        [AllowAnonymous]
        public async Task<IActionResult> Register2([FromBody] Register2 register)
        {
            List<State> states = JsonConvert.DeserializeObject<List<State>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json"));
            List<City> cities = JsonConvert.DeserializeObject<List<City>>(System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json"));
            if (ModelState.IsValid)
            {
                if (register.ostan == -1)
                {
                    return BadRequest(new { message = "لطفا استان را انتخاب کنید" });
                }
                if (register.shahr == -1)
                {
                    return BadRequest(new { message = "لطفا شهر را انتخاب کنید" });
                }
                User user = await db.User.FindAsync(register.id);
                if (user == null)
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                Person person = new Person
                {
                    name = register.name,
                    lastname = register.lastname,
                    phonenumber = register.phonenumber,
                    id = register.id.Value
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
                    personid = register.id.Value

                };
                address.ostan = states.FirstOrDefault(w => w.id == register.ostan).name;
                address.shahr = cities.FirstOrDefault(w => w.id == register.shahr).name;
                await db.Address.AddAsync(address);
                await db.SaveChangesAsync();
                return Content(JsonConvert.SerializeObject(user), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }


        [HttpPost]
        [Route("api/ForgetPass")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPass([FromForm] string email)
        {
            if (ModelState.IsValid)
            {
                if (!u.emailexist(email.ToLower()))
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                User user = db.User.FirstOrDefault(w => w.email == email.ToLower());
                user.activecode = random.Next(10000);
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                SendMail.Send(email.ToLower(), "کد فعالسازی", user.activecode.ToString());
                return Content(JsonConvert.SerializeObject(new { id = user.id }), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpPost]
        [Route("api/ForgetPass2")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPass2([FromForm] string id, [FromForm] string activecode, [FromForm] string password, [FromForm] string repassword)
        {
            if (ModelState.IsValid)
            {
                User user =await db.User.FindAsync(int.Parse(id));
                if (user==null)
                {
                    return BadRequest(new { message = "کاربری یافت نشد." });
                }
                if (user.activecode != int.Parse(activecode))
                {
                    return BadRequest(new { message = "کد وارد شده اشتباه است." });
                }
                if (password != repassword)
                {
                    return BadRequest(new { message = "رمزعبور و تکرار رمز عبور متفاوت اند." });
                }
                user.password = password;
                db.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await db.SaveChangesAsync();
                return Content(JsonConvert.SerializeObject(user), "application/json", Encoding.UTF8);
            }
            return BadRequest(new { message = "خطا!" });
        }

        [HttpPost]
        [Route("api/MessageToAdmin")]
        [AllowAnonymous]
        public async Task<IActionResult> MessageToAdmin([FromBody] Message message)
        {
            if (ModelState.IsValid)
            {
                message.userid = db.User.FirstOrDefault(w=>w.roleid==1).id;
                message.senderemail = message.senderemail.ToLower();
                await db.Message.AddAsync(message);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }



        [HttpGet]
        [Route("api/State")]
        [AllowAnonymous]
        public IActionResult State()
        {
            var json = System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json");

            return Content(json, "application/json", System.Text.Encoding.UTF8);

        }
        [HttpGet]
        [Route("api/City")]
        [AllowAnonymous]
        public IActionResult City()
        {
            var json = System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json");

            return Content(json, "application/json", System.Text.Encoding.UTF8);

        }
    }
}
