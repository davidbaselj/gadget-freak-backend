using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using gadget_freak_backend.Models;
using Microsoft.AspNet.Identity;

namespace gadget_freak_backend.Controllers
{
    public class BlogController : Controller
    {
        private GadgetFreakDatabaseEntities db = new GadgetFreakDatabaseEntities();

        // GET: Blog
        public ActionResult Index(string searchString)
        {
            if (String.IsNullOrEmpty(searchString))
            {
                return View(db.BlogPost.Include(b => b.AspNetUsers).Include(b => b.BlogCategory).OrderByDescending(x => x.CreatedAt).ToList());
            }
            else
            {
                var searchByTitles =
                    db.BlogPost.Where(y => y.Title.Contains(searchString) || y.Content.Contains(searchString))
                        .Include(b => b.AspNetUsers)
                        .Include(b => b.BlogCategory)
                        .OrderByDescending(x => x.CreatedAt)
                        .ToList();
                return View(searchByTitles);
            }
        }

        // GET: Blog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPost.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: Blog/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.CategoryId = new SelectList(db.BlogCategory, "Id", "Name");
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,UserId,CategoryId,Title,Content,UpdatedAt,CreatedAt,CommentsId")] BlogPost blogPost, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                blogPost.CreatedAt = DateTime.Now;

                if (upload != null && upload.ContentLength > 0)
                {
                    byte[] tmpImage;
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        tmpImage = reader.ReadBytes(upload.ContentLength);
                    }

                    blogPost.Image = tmpImage;
                }
                else
                {
                    blogPost.Image = new byte[] { };
                }

                blogPost.UserId = User.Identity.GetUserId();

                BlogLogging bl = new BlogLogging();
                bl.BlogId = blogPost.Id;
                bl.UserId = blogPost.UserId;
                bl.LogDate = DateTime.Now;

                
                db.BlogPost.Add(blogPost);
                db.BlogLogging.Add(bl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", blogPost.UserId);
            ViewBag.CategoryId = new SelectList(db.BlogCategory, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: Blog/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPost.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", blogPost.UserId);
            ViewBag.CategoryId = new SelectList(db.BlogCategory, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,UserId,CategoryId,Title,Content,UpdatedAt,CreatedAt,CommentsId")] BlogPost blogPost, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                blogPost.UpdatedAt = DateTime.Now;

                if (upload != null && upload.ContentLength > 0)
                {
                    byte[] tmpImage;
                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        tmpImage = reader.ReadBytes(upload.ContentLength);
                    }

                    blogPost.Image = tmpImage;
                }
                BlogLogging bl = new BlogLogging();
                bl.BlogId = blogPost.Id;
                bl.UserId = blogPost.UserId;
                bl.LogDate = DateTime.Now;

                blogPost.UserId = User.Identity.GetUserId();
                
                db.Entry(blogPost).State = EntityState.Modified;
                db.BlogLogging.Add(bl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", blogPost.UserId);
            ViewBag.CategoryId = new SelectList(db.BlogCategory, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: Blog/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPost.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPost.Find(id);
            db.BlogPost.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Blog/Logging
        [Authorize(Roles = "Admin")]
        public ActionResult Logging()
        {
            List<BlogLogging> logs = db.BlogLogging.OrderByDescending(x=>x.LogDate).Include(x=>x.BlogPost).Include(y=>y.AspNetUsers).ToList();
            return View(logs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
