using MasterPiece.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MasterPiece.Controllers
{
    public class ProductsController : Controller
    {
        private MasterPieceEntities db = new MasterPieceEntities();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            var products = db.Products.Where(x=>x.isDeleted !=true).Include(p => p.Category).Include(p => p.Store);
            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name");
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Product_id,Product_Name,Product_Image,Product_Price,Quantity,Product_Description,Store_id,Category_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Category_id);
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name", product.Store_id);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Category_id);
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name", product.Store_id);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Product_id,Product_Name,Product_Image,Product_Price,Quantity,Product_Description,Store_id,Category_id")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Category_id);
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name", product.Store_id);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            product.isDeleted = true;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult Feature(int id)
        {
            int featuredcount = db.Products.Count(x => x.isFeatured==true);
            if (featuredcount >= 3) 
            {
                TempData["swal_message"] = $"You have reached the max amount of featured products";
                ViewBag.title = "Error";
                ViewBag.icon = "error";
                return Redirect(Request.UrlReferrer.ToString());

            }
            var Product = db.Products.Find(id);
            Product.isFeatured = true;
            db.SaveChanges();

            return View("Index", db.Products.ToList());
        }
        public ActionResult UnFeature(int id)
        {
            var Product = db.Products.Find(id);
            Product.isFeatured = false;
            db.SaveChanges();

            return View("Index", db.Products.ToList());
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
