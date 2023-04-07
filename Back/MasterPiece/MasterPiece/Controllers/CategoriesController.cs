using MasterPiece.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MasterPiece.Controllers
{
    public class CategoriesController : Controller
    {
        private MasterPieceEntities db = new MasterPieceEntities();

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            return View(await db.Categories.Where(x=>x.isDeleted !=true).ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Category_id,Category_Name,Category_Image")] Category category, HttpPostedFileBase Category_Image)
        {
            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();
                string path = guid + Category_Image.FileName;
                Category_Image.SaveAs(Server.MapPath("../AdminContent/CategoryImages/" + path));
                category.Category_Image = path;
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, [Bind(Include = "Category_id,Category_Name,Category_Image")] Category category, HttpPostedFileBase Category_Image)
        {
            if (ModelState.IsValid)
            {

                if (Category_Image != null)
                {
                    Guid guid = Guid.NewGuid();
                    string path = guid + Category_Image.FileName;
                    Category_Image.SaveAs(Server.MapPath("../../AdminContent/CategoryImages/" + path));
                    category.Category_Image = path;
                }
                else
                {
                    var existingModel = db.Categories.AsNoTracking().FirstOrDefault(x => x.Category_id == id);

                    category.Category_Image = existingModel.Category_Image;
                }

                db.Entry(category).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = await db.Categories.FindAsync(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Category category = await db.Categories.FindAsync(id);
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
