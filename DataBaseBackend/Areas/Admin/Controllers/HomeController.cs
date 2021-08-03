using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy ="CookiesAdmin")]
    public class HomeController : Controller
    {
        readonly Context db;
        readonly Random random;
        IViewRenderService vi;
        IUser u;
        public HomeController(Context db, IViewRenderService vi)
        {
            this.db = db;
            u = new UserService(db);
            random = new Random();
            this.vi = vi;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChangePass()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePass(string current,string newpass,string renewpass)
        {
            DataBaseBackend.User user = db.User.FirstOrDefault(w=>w.username==HttpContext.User.Identity.Name);
            if (string.IsNullOrEmpty(current))
            {
                ModelState.AddModelError("name", "لطفا کلمه عبور فعلی را وارد کنید");
                return View();
            }
            if (string.IsNullOrEmpty(newpass))
            {
                ModelState.AddModelError("phonenumber", "لطفا کلمه عبور جدید را وارد کنید");
                return View();
            }
            if (string.IsNullOrEmpty(renewpass))
            {
                ModelState.AddModelError("email", "لطفا تکرار کلمه عبور جدید را وارد کنید");
                return View();
            }
            if (current!=user.password)
            {
                ModelState.AddModelError("name","کلمه عبور اشتباه است");
                return View();
            }
            if (newpass!= renewpass)
            {
                ModelState.AddModelError("email", "کلمه عبور جدید و تکرار آن مطابقت ندارند.");
                return View();
            }
            user.password = newpass;
            db.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public bool IsNewMessage()
        {
            if (db.Message.Any(w=>w.userid==db.User.FirstOrDefault(w=>w.roleid==1).id&&w.isseen==false))
            {
                return true;
            }
            return false;
        }
    }
}
