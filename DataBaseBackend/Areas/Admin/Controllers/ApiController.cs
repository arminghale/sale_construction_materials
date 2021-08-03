using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [ApiController, DisableRequestSizeLimit]
    [Area("Admin")]
    [Authorize(Policy = "JWTAdmin")]
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
        [Route("Admin/api/ChangePass")]
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
        [Route("Admin/api/IsNewMessage")]
        public IActionResult IsNewMessage()
        {
            if (_context.Message.Any(w => w.userid == _context.User.FirstOrDefault(w => w.roleid == 1).id && w.isseen == false))
            {
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }




        [HttpGet]
        [Route("Admin/api/PrGroupIndex")]
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
        [Route("Admin/api/PrGroupDetails")]
        public async Task<IActionResult> PrGroupDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prGroup = await _context.PrGroup
                .FirstOrDefaultAsync(m => m.id == id);
            if (prGroup == null)
            {
                return NotFound();
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(prGroup, setting), "application/json", Encoding.UTF8);
        }
        [HttpPost]
        [Route("Admin/api/PrGroupCreate")]
        public async Task<IActionResult> PrGroupCreate([FromForm] PrGroup prGroup, [FromForm] string[] field)
        {
            if (ModelState.IsValid)
            {
                PrGroup pr = prGroup;
                _context.Add(pr);
                await _context.SaveChangesAsync();
                foreach (var item in field)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        _context.Field.Add(new Field { prgroupid = pr.id, title = item });
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpPost]
        [Route("Admin/api/PrGroupEdit")]
        public async Task<IActionResult> PrGroupEdit([FromForm] int id, [FromForm] PrGroup prGroup, [FromForm] string[] field)
        {
            if (id != prGroup.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prGroup);
                    await _context.SaveChangesAsync();
                    int flag;
                    var list =await _context.Field.Where(w => w.prgroupid == id).ToListAsync();
                    foreach (var item in list)
                    {
                        flag = 0;
                        foreach (var item2 in field)
                        {
                            if (item.title==item2)
                            {
                                flag++;
                            }
                        }
                        if (flag==0)
                        {
                            _context.Field.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                    }
                    foreach (var item in field)
                    {
                        flag = 0;
                        if (!string.IsNullOrEmpty(item))
                        {
                            foreach (var item2 in _context.Field.Where(w => w.prgroupid == id))
                            {
                                if (item == item2.title)
                                {
                                    flag++;
                                }
                            }
                            if (flag == 0)
                            {
                                _context.Field.Add(new Field { prgroupid = id, title = item });
                                await _context.SaveChangesAsync();
                            }
                            else if (flag > 1)
                            {
                                return BadRequest("بعضی از فیلد ها به طور تکراری وارد شده اند.");
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrGroupExists(prGroup.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }

            return BadRequest(new { message = "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/PrGroupDelete/{id}")]
        public async Task<IActionResult> PrGroupDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var prGroup = await _context.PrGroup.FindAsync(id);
            _context.PrGroup.Remove(prGroup);
            await _context.SaveChangesAsync();
            return Ok();
        }
        private bool PrGroupExists(int id)
        {
            return _context.PrGroup.Any(e => e.id == id);
        }




        [HttpGet]
        [Route("Admin/api/ProductIndex")]
        public async Task<IActionResult> ProductIndex(int no = -1, string search = "", int gid = -1)
        {
            //PrGroupIndex
            var context = await _context.Product.OrderByDescending(w=>w.createdate).ToListAsync();
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
        [Route("Admin/api/ProductDetails/{id}")]
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
        [Route("Admin/api/ProductCreate")]
        public async Task<IActionResult> ProductCreate([FromBody] CreateProduct pro)
        {
            //PrGroupIndex
            if (ModelState.IsValid)
            {
                Product product = new Product
                {
                    prgroupid=pro.prgroupid,
                    title=pro.title,
                    price=pro.price,
                    sendday=pro.sendday,
                    readyday=pro.readyday,
                    text=pro.text,
                    count=pro.count,
                    seen=0
                };
                product.createdate = System.DateTime.Now;
                if (pro.mainimage != null)
                {
                    string imname = Guid.NewGuid().ToString() + ".jpg";
                    var path = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/Files/",
                                imname);
                    var base64array = Convert.FromBase64String(pro.mainimage);
                    System.IO.File.WriteAllBytes(path, base64array);
                    //using (var stream = new FileStream(path, FileMode.Create))
                    //{
                    //    await mainimage.CopyToAsync(stream).ConfigureAwait(false);
                    //}
                    product.imagename = imname;
                    var thumb = Image.Load(path);
                    thumb.Mutate(x => x.Resize(150, 150));
                    thumb.Save(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Thumb/" + imname));
                }
                else
                {
                    return BadRequest(new { message = "لطفا تصویر اصلی را انتخاب کنید" });
                }
                Product p = product;
                _context.Add(p);
                await _context.SaveChangesAsync();

                foreach (var item in pro.gallery)
                {
                    if (item != null)
                    {
                        string imname = Guid.NewGuid().ToString() + ".jpg";
                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery",
                                    imname);
                        var base64array = Convert.FromBase64String(item);
                        System.IO.File.WriteAllBytes(path, base64array);
                        _context.Add(new Gallery { productid = p.id, imagename = imname });
                        await _context.SaveChangesAsync();
                    }

                }

                string[] tags = pro.tag.Split('-');
                foreach (var item in tags)
                {
                    _context.Add(new Tag { productid = p.id, text = item });
                    await _context.SaveChangesAsync();
                }

                var prf = _context.PrGroup.Find(product.prgroupid).Fields;
                foreach (var item in prf)
                {
                    _context.Add(new FillField { fieldid = item.id, productid = p.id });
                    await _context.SaveChangesAsync();
                }

                return Ok(new {id= p.id});
            }

            return BadRequest(new { message = "خطا!" });
        }
        [HttpPost]
        [Route("Admin/api/ProductEdit")]
        public async Task<IActionResult> ProductEdit([FromBody] CreateProduct pro)
        {

            if (ModelState.IsValid)
            {
                Product product = _context.Product.Find(pro.id);
                try
                {
                    product.title = pro.title;
                    product.prgroupid = pro.prgroupid;
                    product.price = pro.price;
                    product.sendday = pro.sendday;
                    product.readyday = pro.readyday;
                    product.text = pro.text;
                    product.count = pro.count;
                    if (pro.mainimage != null)
                    {
                        if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/", product.imagename)))
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/", product.imagename));
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Thumb", product.imagename));
                        }
                        string imname = Guid.NewGuid().ToString() + ".jpg";
                        var path = Path.Combine(
                                    Directory.GetCurrentDirectory(), "wwwroot/Files/",
                                    imname);
                        var base64array = Convert.FromBase64String(pro.mainimage);
                        System.IO.File.WriteAllBytes(path, base64array);
                        product.imagename = imname;
                        var thumb = Image.Load(path);
                        thumb.Mutate(x => x.Resize(150, 150));
                        thumb.Save(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Thumb/" + imname));
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    foreach (var item in pro.gallery)
                    {
                        if (item != null)
                        {
                            string imname = Guid.NewGuid().ToString() + ".jpg";
                            var path = Path.Combine(
                                        Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery",
                                        imname);
                            var base64array = Convert.FromBase64String(item);
                            System.IO.File.WriteAllBytes(path, base64array);
                            _context.Add(new Gallery { productid = product.id, imagename = imname });
                            await _context.SaveChangesAsync();
                        }

                    }

                    var list =await _context.Tag.Where(w => w.productid == product.id).ToListAsync();
                    foreach (var item in list)
                    {
                        _context.Tag.Remove(item);
                        await _context.SaveChangesAsync();
                    }
                    string[] tags = pro.tag.Split('-');
                    foreach (var item in tags)
                    {
                        _context.Add(new Tag { productid = product.id, text = item });
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
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/ProductDelete/{id}")]
        public async Task<IActionResult> ProductDelete(int? id)
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
            return Ok();
        }
        [HttpPost]
        [Route("Admin/api/ProductField")]
        public async Task<IActionResult> ProductField([FromForm] int id, [FromForm] string[] fields, [FromForm] int[] ids)
        {
            var field = await _context.FillField.Where(w => w.productid == id).ToListAsync();
            for (int i = 0; i < ids.Length; i++)
            {
                if (!string.IsNullOrEmpty(fields[i]))
                {
                    if (fields[i] != field.FirstOrDefault(w=>w.fieldid==ids[i]).text)
                    {
                        field.FirstOrDefault(w => w.fieldid == ids[i]).text = fields[i];
                        _context.Update(field.FirstOrDefault(w => w.fieldid == ids[i]));
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Ok();
        }
        [HttpPost]
        [Route("Admin/api/ProductTakhfif")]
        public async Task<IActionResult> ProductTakhfif([FromForm] int id,[FromForm] string darsad, [FromForm] string start, [FromForm] string end)
        {
            Takhfif pr = new Takhfif { darsad = int.Parse(darsad),productid=id };
            start = start.Replace('-', '/');
            start = changePersianNumbersToEnglish(start);
            pr.starttime = DateTime.Parse(start, new CultureInfo("fa-IR"));
            end = end.Replace('-', '/');
            end = changePersianNumbersToEnglish(end);
            pr.endtime = DateTime.Parse(end, new CultureInfo("fa-IR"));
            if (ModelState.IsValid)
            {
                _context.Add(pr);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest(new { message = "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/ProductTakhfifs/{id}")]
        public async Task<IActionResult> ProductTakhfifs(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(_context.Takhfif.Where(w => w.productid == id).OrderByDescending(w => w.endtime), setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Admin/api/ProductTakhfifDelete/{id}")]
        public async Task<IActionResult> ProductTakhfifDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Takhfif.FindAsync(id);
            _context.Takhfif.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        [Route("Admin/api/ProductGalleryDelete/{id}")]
        public async Task<IActionResult> ProductGalleryDelete(int id)
        {

            var product = await _context.Gallery.FindAsync(id);
            if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery", product.imagename)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Gallery", product.imagename));
            }
            _context.Gallery.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
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





        [HttpGet]
        [Route("Admin/api/RoleIndex")]
        public async Task<IActionResult> RoleIndex()
        {
            //RoleIndex
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 1;
            return Content(JsonConvert.SerializeObject(await _context.Role.ToListAsync(), setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Admin/api/UsersIndex")]
        public async Task<IActionResult> UsersIndex(string search = "", int role = -1)
        {
            //RoleIndex
            var context = await _context.User.Where(w => w.roleid != 1).ToListAsync();
            if (role != -1)
            {
                context = context.Where(w => w.roleid == role).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                context = context.Where(w => w.username.Contains(search) || w.email.Contains(search) || w.id.ToString().Contains(search)).ToList();
                context.Concat(context.Where(w => w.Person!=null&& w.Person.name.Contains(search) ||
                  w.Person.lastname.Contains(search) || w.Person.phonenumber.Contains(search))).ToList();
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(context, setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Admin/api/UsersDetails/{id}")]
        public async Task<IActionResult> UsersDetails(int? id)
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
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(user, setting), "application/json", Encoding.UTF8);

        }
        [HttpPost]
        [Route("Admin/api/UsersCreate")]
        public async Task<IActionResult> UsersCreate([FromBody]DataBaseBackend.User user)
        {
            //RoleIndex
            if (ModelState.IsValid)
            {
                if (_context.User.Any(w => w.email == user.email.ToLower()))
                {
                    return BadRequest(new {message= "کاربری با این ایمیل ثبت شده است" });
                }
                if (_context.User.Any(w => w.username == user.username.ToLower()))
                {
                    return BadRequest(new { message = "کاربری با این نام کاربری ثبت شده است" });
                }
                user.email = user.email.ToLower();
                user.username = user.username.ToLower();
                user.isactive = true;
                user.activecode = random.Next(10000);
                user.lastlogin = System.DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpPost]
        [Route("Admin/api/UsersEdit")]
        public async Task<IActionResult> UsersEdit([FromBody]DataBaseBackend.User user)
        {
            //RoleIndex
            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.User.Any(w => w.email == user.email.ToLower() && w.id != user.id))
                    {
                        return BadRequest(new {message= "کاربری با این ایمیل ثبت شده است" });
                    }
                    if (_context.User.Any(w => w.username == user.username.ToLower() && w.id != user.id))
                    {
                        return BadRequest(new { message = "کاربری با این نام کاربری ثبت شده است" });
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
                return Ok();
            }
            return BadRequest(new { message= "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/UsersDelete/{id}")]
        public async Task<IActionResult> UsersDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost]
        [Route("Admin/api/UsersPerson")]
        public async Task<IActionResult> UsersPerson([FromBody] Person person)
        {
            if (ModelState.IsValid)
            {
                if (_context.Person.Any(w => w == person))
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.Add(person);
                    await _context.SaveChangesAsync();
                }
                return Ok();
            }
            return BadRequest(new {message= "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/UsersAddress/{id}")]
        public IActionResult UsersAddress(int id)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(_context.Address.Where(w => w.personid == id), setting), "application/json", Encoding.UTF8);
        }
        [HttpPost]
        [Route("Admin/api/UsersAddressCreate")]
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
                return BadRequest(new { message= "لطفا کد پستی را وارد کنید" });
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
            Person person = _context.Person.Find(address.personid);
            address.ostan = states.FirstOrDefault(w => w.id == ostan).name;
            address.shahr = cities.FirstOrDefault(w => w.id == shahr).name;
            if (girande == 0)
            {
                address.girandename = person.name;
                address.girandelastname = person.lastname;
                address.girandephonenumber = person.phonenumber;
            }
            _context.Add(address);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        [Route("Admin/api/UsersAddressDelete/{id}")]
        public async Task<IActionResult> UsersAddressDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Address.FindAsync(id);
            var idd = user.personid;
            _context.Address.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        [Route("Admin/api/State")]
        public IActionResult State()
        {
            var json = System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/provinces.json");

            return Content(json, "application/json", System.Text.Encoding.UTF8);

        }
        [HttpGet]
        [Route("Admin/api/City")]
        public IActionResult City()
        {
            var json = System.IO.File.ReadAllText(_enviroment.ContentRootPath + "/wwwroot/lib/cities.json");

            return Content(json, "application/json", System.Text.Encoding.UTF8);

        }
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.id == id);
        }





        [HttpGet]
        [Route("Admin/api/MessageIndex/{userid}")]
        public async Task<IActionResult> MessageIndex(int userid,string search = "", int type = -1)
        {
            var context = await _context.Message.Where(w => w.User.id == userid).OrderByDescending(w => w.createdate).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (type == 0)
                {
                    context = context.Where(w => w.User2.username.Contains(search) || w.User2.email.Contains(search) || w.User2.Person.name.Contains(search) || w.User2.Person.lastname.Contains(search)).ToList();
                }
                else if (type == 1)
                {
                    context = context.Where(w => w.text.Contains(search)).ToList();
                }
                else
                {
                    context = context.Where(w => w.User2.username.Contains(search) || w.text.Contains(search) || w.User2.email.Contains(search) || w.User2.Person.name.Contains(search) || w.User2.Person.lastname.Contains(search)).ToList();
                }
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(context, setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Admin/api/MessageIndexSend/{userid}")]
        public async Task<IActionResult> MessageIndexSend(int userid,string search = "", int type = -1)
        {
            var context = await _context.Message.Where(w => w.User2.id == userid).OrderByDescending(w => w.createdate).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (type == 0)
                {
                    context = context.Where(w => w.User.username.Contains(search) || w.User.email.Contains(search) || w.User.Person.name.Contains(search) || w.User.Person.lastname.Contains(search)).ToList();
                }
                else if (type == 1)
                {
                    context = context.Where(w => w.text.Contains(search)).ToList();
                }
                else
                {
                    context = context.Where(w => w.User.username.Contains(search) || w.text.Contains(search) || w.User.email.Contains(search) || w.User.Person.name.Contains(search) || w.User.Person.lastname.Contains(search)).ToList();
                }
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(context, setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Admin/api/MessageDetails/{id}")]
        public async Task<IActionResult> MessageDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.User)
                .Include(m => m.User2)
                .FirstOrDefaultAsync(m => m.id == id);
            if (message == null)
            {
                return NotFound();
            }
            message.isseen = true;
            _context.Update(message);
            await _context.SaveChangesAsync();

            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(message, setting), "application/json", Encoding.UTF8);

        }
        [HttpPost]
        [Route("Admin/api/MessageCreate")]
        public async Task<IActionResult> MessageCreate([FromForm] int adminid, [FromForm] Message message)
        {
            //UsersIndex
            if (ModelState.IsValid)
            {
                if (message.userid == -1)
                {
                    return BadRequest(new { message = "لطفا کاربر مورد نظر را انتخاب کنید" });
                }
                DataBaseBackend.User user = _context.User.FirstOrDefault(w => w.id == adminid);
                message.user2id = user.id;
                message.isseen = false;
                message.createdate = System.DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpPost]
        [Route("Admin/api/MessageEdit")]
        public async Task<IActionResult> MessageEdit([FromForm] int id, [FromForm] Message message)
        {
            //UsersIndex
            if (id != message.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/MessageDelete/{id}")]
        public async Task<IActionResult> MessageDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        [Route("Admin/api/SeenMessage/{id}")]
        public async Task<IActionResult> OneMessages(int id)
        {
            if (ModelState.IsValid)
            {
                var messages = await _context.Message.FindAsync(id);
                messages.isseen = true;
                _context.Entry(messages).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpPost]
        [Route("Admin/api/MessageSendEmail")]
        public async Task<IActionResult> MessageSendEmail([FromForm] string email, [FromForm] string text)
        {
            SendMail.Send(email, "سامانه فروش مصالح", text);
            return Ok();
        }
        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.id == id);
        }





        [HttpGet]
        [Route("Admin/api/BasketIndex")]
        public async Task<IActionResult> BasketIndex(string search = "", int type = -1)
        {
            var context = _context.Basket.Include(b => b.Address).Include(b => b.User).Where(w => w.ispay == true);
            if (type == 0)
            {
                context = context.Where(w => w.issend == true);
            }
            if (type == 1)
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
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(await context.OrderByDescending(w => w.paydate).ToListAsync(), setting), "application/json", Encoding.UTF8);
        }
        [HttpGet]
        [Route("Admin/api/BasketDetails/{id}")]
        public async Task<IActionResult> BasketDetails(int? id)
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
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 2;
            return Content(JsonConvert.SerializeObject(basket, setting), "application/json", Encoding.UTF8);
        }
        [HttpPost]
        [Route("Admin/api/BasketCreate")]
        public async Task<IActionResult> BasketCreate([FromForm] Basket basket, [FromForm] int[] ids, [FromForm] int[] counts)
        {
            //UsersIndex
            //AddressIndex
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
            if (basket.addressid == -1)
            {
                return BadRequest(new { message = "لطفا آدرس را انتخاب کنید" });
            }
            if (basket.userid == -1)
            {
                return BadRequest(new { message = "لطفا کاربر را انتخاب کنید" });
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

            return Ok();
        }
        [HttpPost]
        [Route("Admin/api/BasketEdit")]
        public async Task<IActionResult> BasketEdit([FromForm] int id, [FromForm] bool ready = false, [FromForm] bool send = false)
        {
            var basket = await _context.Basket.FindAsync(id);
            if (ready)
            {
                basket.isready = true;
                _context.Update(basket);
                await _context.SaveChangesAsync();
                return Ok();
            }
            if (send)
            {
                basket.issend = true;
                basket.senddate = System.DateTime.Now;
                _context.Update(basket);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(new { message = "خطا!" });
        }
        [HttpGet]
        [Route("Admin/api/BasketDelete/{id}")]
        public async Task<IActionResult> BasketDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var basket = await _context.Basket.FindAsync(id);
            _context.Basket.Remove(basket);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        [Route("Admin/api/BasketCansel")]
        public async Task<IActionResult> BasketCansel(int? id, bool iscansel)
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
            }
            else
            {
                basket.iscansel = false;
                _context.Update(basket);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        [HttpGet]
        [Route("Admin/api/ProductBaTakhfif/{id}")]
        public async Task<IActionResult> ProductBaTakhfif(int id)
        {
            var context = _context.Product.Find(id);

            return Content(JsonConvert.SerializeObject(new {message= t.MablaghBaTakhfif(context)}), "application/json", Encoding.UTF8);

        }
        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.id == id);
        }

    }
}
