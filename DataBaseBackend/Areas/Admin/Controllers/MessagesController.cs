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
    public class MessagesController : Controller
    {
        private readonly Context _context;

        public MessagesController(Context context)
        {
            _context = context;
        }

        // GET: Admin/Messages
        public async Task<IActionResult> Index(string search="",int type=-1)
        {
            var context =await _context.Message.Where(w => w.User.username == HttpContext.User.Identity.Name).OrderByDescending(w=>w.createdate).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                if (type==0)
                {
                    context = context.Where(w => w.User2.username.Contains(search) || w.User2.email.Contains(search) || w.User2.Person.name.Contains(search) || w.User2.Person.lastname.Contains(search)).ToList();
                }
                else if(type==1)
                {
                    context = context.Where(w => w.text.Contains(search)).ToList();
                }
                else
                {
                    context = context.Where(w => w.User2.username.Contains(search)|| w.text.Contains(search) || w.User2.email.Contains(search) || w.User2.Person.name.Contains(search) || w.User2.Person.lastname.Contains(search)).ToList();
                }
            }
            return View(context);
        }
        public async Task<IActionResult> IndexSend(string search = "", int type = -1)
        {
            var context = await _context.Message.Where(w => w.User2.username == HttpContext.User.Identity.Name).OrderByDescending(w => w.createdate).ToListAsync();
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
            return View(context);
        }

        // GET: Admin/Messages/Details/5
        public async Task<IActionResult> Details(int? id,int which=0)
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
            if (which==0)
            {
                ViewBag.res = true;
                message.isseen = true;
                _context.Update(message);
               await _context.SaveChangesAsync();
            }
            
            if (which==1)
            {
                ViewBag.res = false;
            }

            return View(message);
        }

        // GET: Admin/Messages/Create
        public IActionResult Create(int userid=-1)
        {
            ViewData["userid"] = new SelectList(_context.User.Where(w => w.roleid != 1), "id", "username");
            ViewBag.res = false;
            if (userid!=-1)
            {
                ViewData["userid"] = new SelectList(_context.User.Where(w => w.roleid != 1), "id", "username",userid);
                ViewBag.res = true;
            }
            
            return View();
        }

        // POST: Admin/Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,userid,user2id,text,isseen,createdate,sendername,senderlastname,senderemail")] Message message)
        {
            ViewData["userid"] = new SelectList(_context.User.Where(w => w.roleid != 1), "id", "username", message.userid);
            if (ModelState.IsValid)
            {
                if (message.userid==-1)
                {
                    ModelState.AddModelError("userid", "لطفا کاربر مورد نظر را انتخاب کنید");
                    return View(message);
                }
                DataBaseBackend.User user = _context.User.FirstOrDefault(w => w.username == HttpContext.User.Identity.Name);
                message.user2id = user.id;
                message.isseen = false;
                message.createdate = System.DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexSend));
            }
            return View(message);
        }

        // GET: Admin/Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["userid"] = new SelectList(_context.User, "id", "email", message.userid);
            ViewData["user2id"] = new SelectList(_context.User, "id", "email", message.user2id);
            return View(message);
        }

        // POST: Admin/Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,userid,user2id,text,isseen,createdate,sendername,senderlastname,senderemail")] Message message)
        {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["userid"] = new SelectList(_context.User, "id", "email", message.userid);
            ViewData["user2id"] = new SelectList(_context.User, "id", "email", message.user2id);
            return View(message);
        }

        // GET: Admin/Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Users(string search="")
        {
            var context =await _context.User.Where(w=>w.roleid!=1).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                context = context.Where(w => w.username.Contains(search) || w.email.Contains(search) || w.Person.name.Contains(search) || w.Person.lastname.Contains(search)).ToList();
            }
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.MaxDepth = 1;
            var json = JsonConvert.SerializeObject(context,setting);
            return Content(json,"application/json",System.Text.Encoding.UTF8);
        }

        public IActionResult SendEmail(string email)
        {
            ViewBag.email = email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(string email,string text)
        {
            SendMail.Send(email, "سامانه فروش مصالح", text);
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.id == id);
        }
    }
}
