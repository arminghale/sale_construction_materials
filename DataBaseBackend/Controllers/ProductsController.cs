using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBaseBackend.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Context _context;
        ITakhfif t;
        public ProductsController(Context context)
        {
            _context = context;
            t = new TakhfifService(context);
        }

        [Route("Products")]
        public async Task<IActionResult> Index(string search = "", int gid = -1, string sortby = "")
        {
            var context = await _context.Product.ToListAsync();
            if (!string.IsNullOrEmpty(sortby))
            {
                if (sortby == "createdate")
                {
                    context = context.OrderByDescending(w => w.createdate).ToList();
                }
                if (sortby == "takhfif")
                {
                    context = context.Where(w => w.price > t.MablaghBaTakhfif(w)).ToList();
                }
                if (sortby == "gran")
                {
                    context = context.OrderByDescending(w => t.MablaghBaTakhfif(w)).ToList();
                }
                if (sortby == "arzan")
                {
                    context = context.OrderBy(w => t.MablaghBaTakhfif(w)).ToList();
                }
                if (sortby == "seen")
                {
                    context = context.OrderByDescending(w => w.seen).ToList();
                }
            }
            if (gid != -1)
            {
                context = context.Where(w => w.prgroupid == gid).ToList();
            }
            if (!string.IsNullOrEmpty(search))
            {
                var list = context.Where(w => w.title.Contains(search) || w.text.Contains(search) || w.PrGroup.title.Contains(search)).ToList();
                foreach (var item in context)
                {
                    foreach (var item2 in item.Tags)
                    {
                        if (item2.text.Contains(search))
                        {
                            if (!list.Any(w => w == item))
                            {
                                list.Add(item);
                            }
                        }
                    }
                    foreach (var item2 in item.FillFields)
                    {
                        if (item2.text.Contains(search))
                        {
                            if (!list.Any(w => w == item))
                            {
                                list.Add(item);
                            }
                        }
                    }
                }

                context = list;
            }
            return View(context);
        }


        [Route("Products/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.PrGroup).Include(p=>p.FillFields).Include(p=>p.Comments).Include(p=>p.Galleries)
                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            product.seen++;
            _context.Product.Update(product);
            await _context.SaveChangesAsync();

            int mablagh = t.MablaghBaTakhfif(product);
            if (product.price!=mablagh)
            {
                ViewBag.takhfif = mablagh;
            }
            ViewBag.userid = _context.User.FirstOrDefault(w => w.username == User.Identity.Name).id;
            return View(product);
        }



        [HttpPost]
        public async Task<IActionResult> AddComment(int prid,string text)
        {

            var user = _context.User.FirstOrDefault(w => w.username == HttpContext.User.Identity.Name);
            Comment comment = new Comment
            {
                productid=prid,
                text=text,
                userid=user.id,
                createdate=System.DateTime.Now,
            };
            await _context.Comment.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Redirect("/Products/"+prid);
        }

        [Route("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = _context.Comment.Find(id);
            int prid = comment.productid;
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return Redirect("/Products/" + prid);
        }

        [HttpPost]
        public async Task<IActionResult> AddCommentReply(int comid, string text)
        {
            var com = _context.Comment.Find(comid);
            CommentReplay comment = new CommentReplay
            {
                commentid = comid,
                text = text,
                createdate = System.DateTime.Now,
            };
            await _context.CommentReplay.AddAsync(comment);
            await _context.SaveChangesAsync();

            return Redirect("/Products/" + com.productid);
        }

        [Route("DeleteCommentReplay/{id}")]
        public async Task<IActionResult> DeleteCommentReplay(int id)
        {
            var comment = _context.CommentReplay.Find(id);
            int prid = comment.Comment.productid;
            _context.CommentReplay.Remove(comment);
            await _context.SaveChangesAsync();

            return Redirect("/Products/" + prid);
        }
    }
}
