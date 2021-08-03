﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Areas.Anbar.Controllers
{
    [Area("Anbar")]
    [Authorize(Policy = "CookiesAnbar")]
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
        public async Task<IActionResult> ChangePass(string current, string newpass, string renewpass)
        {
            DataBaseBackend.User user = db.User.FirstOrDefault(w => w.username == HttpContext.User.Identity.Name);
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
            if (current != user.password)
            {
                ModelState.AddModelError("name", "کلمه عبور اشتباه است");
                return View();
            }
            if (newpass != renewpass)
            {
                ModelState.AddModelError("email", "کلمه عبور جدید و تکرار آن مطابقت ندارند.");
                return View();
            }
            user.password = newpass;
            db.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ProductIndex(int no = -1, string search = "", int gid = -1)
        {
            var context = await db.Product.ToListAsync();
            ViewData["prgroupid"] = new SelectList(db.PrGroup, "id", "title");
            if (gid != -1)
            {
                context = context.Where(w => w.prgroupid == gid).ToList();
                ViewData["prgroupid"] = new SelectList(db.PrGroup, "id", "title", gid);
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

            return View(context);
        }

        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.Product
                .Include(p => p.PrGroup)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> ProductEdit(int? id,int count)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await db.Product
                .Include(p => p.PrGroup)
                .FirstOrDefaultAsync(m => m.id == id);
            product.count = count;
            db.Update(product);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(ProductIndex));
        }


        [HttpGet]
        public bool IsNewMessage()
        {
            if (db.Message.Any(w => w.userid == db.User.FirstOrDefault(w => w.roleid == 3).id && w.isseen == false))
            {
                return true;
            }
            return false;
        }
    }
}
