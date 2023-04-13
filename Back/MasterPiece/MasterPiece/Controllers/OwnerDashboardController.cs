using MasterPiece.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MasterPiece.Controllers
{
    public class OwnerDashboardController : Controller
    {
        // GET: OwnerDashboard
        private MasterPieceEntities db = new MasterPieceEntities();

        public ActionResult Index()
        {
            int storeid =Convert.ToInt32(Session["LoggedStoreId"]);
            var products = db.Products.Where(x=>x.Store_id== storeid && x.isDeleted!=true   ).ToList();

            return View(products);
        }
        public ActionResult CreateProduct()
        {
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name");
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProduct([Bind(Include = "Product_id,Product_Name,Product_Image,Product_Price,Quantity,Product_Description,Category_id")] Product product, HttpPostedFileBase Product_Image) 
        
        {
            if (ModelState.IsValid)
            {
                int storeid = Convert.ToInt32(Session["LoggedStoreId"]);
                Guid guid = Guid.NewGuid();
                string path = guid + Product_Image.FileName;
                Product_Image.SaveAs(Server.MapPath("../ProductImage/" + path));
                product.Product_Image = path;
                product.Store_id = storeid;
                db.Products.Add(product);

                db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Category_id);
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name", product.Store_id);
            return View(product);


        }
        public async Task<ActionResult> Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int storeCheck=Convert.ToInt32(db.Products.Where(x=>x.Product_id==id).Select(x=>x.Store_id).FirstOrDefault());
            int loggedStoreId = Convert.ToInt32(Session["LoggedStoreId"]);
            if (storeCheck != loggedStoreId) { return View("NoAccess"); }
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
        public async Task<ActionResult> Edit(int? id,[Bind(Include = "Product_id,Product_Name,Product_Price,Quantity,Product_Description,Store_id,Category_id")] Product product,HttpPostedFileBase Product_Image)
        {
            if (ModelState.IsValid)
            {
                if (Product_Image != null)
                {
                    Guid guid = Guid.NewGuid();
                    string path = guid + Product_Image.FileName;
                    Product_Image.SaveAs(Server.MapPath("../../ProductImage/" + path));
                    product.Product_Image = path;
                }
                else
                {
                    var existingModel = db.Products.AsNoTracking().FirstOrDefault(x => x.Product_id ==id);
                    product.Product_Image = existingModel.Product_Image;
                }
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Category_id = new SelectList(db.Categories, "Category_id", "Category_Name", product.Category_id);
            ViewBag.Store_id = new SelectList(db.Stores, "Store_id", "Store_Name", product.Store_id);
            return View(product);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int storeCheck = Convert.ToInt32(db.Products.Where(x => x.Product_id == id).Select(x => x.Store_id).FirstOrDefault());
            int loggedStoreId = Convert.ToInt32(Session["LoggedStoreId"]);
            if (storeCheck != loggedStoreId) { return View("NoAccess"); }
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
        public ActionResult StoreSales()
        {
            int loggedStore =Convert.ToInt32(Session["LoggedStoreId"]);
            ViewBag.totalSales = db.Transactions.Where(x=>x.Order.Store_Id==loggedStore).Sum(x => x.Amount);
            DateTime today = DateTime.Today;
            DateTime startOfToday = today.Date;
            DateTime endOfToday = startOfToday.AddDays(1).AddTicks(-1);
            double totalSales = Convert.ToDouble(db.Transactions
                .Where(x => x.TransactionDate >= startOfToday && x.TransactionDate <= endOfToday&& x.Order.Store_Id==loggedStore)
                .Sum(x => x.Amount));
            if (totalSales == null)
                ViewBag.todaySales = 0;
            else
                ViewBag.todaySales = totalSales;
            ViewBag.todaysRevenue = ViewBag.todaySales * 90 / 100;

            ViewBag.totalRevenue = ViewBag.totalSales * 90 / 100;


            return View();
        }

        public async Task<ActionResult> Profile()
        {

           
            int loggedStoreId = Convert.ToInt32(Session["LoggedStoreId"]);
            Store store = await db.Stores.FindAsync(loggedStoreId);
            if (store == null)
            {
                return HttpNotFound();
            }
           
            return View(store);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Profile( HttpPostedFileBase Store_Image)
        {
            int loggedStoreId = Convert.ToInt32(Session["LoggedStoreId"]);
            Store store = await db.Stores.FindAsync(loggedStoreId);
            if (ModelState.IsValid)
            {
                if (Store_Image != null)
                {
                    Guid guid = Guid.NewGuid();
                    string path = guid + Store_Image.FileName;
                    Store_Image.SaveAs(Server.MapPath("../Images/" + path));
                    store.Store_Image = path;
                }
                else
                {
                    var existingModel = db.Stores.AsNoTracking().FirstOrDefault(x => x.Store_id == loggedStoreId);
                    store.Store_Image = existingModel.Store_Image;
                }
                store.Store_Name = Request["Store_Name"];
                store.Owner_Name = Request["Owner_Name"];
                await db.SaveChangesAsync();
                return RedirectToAction("Profile");
            }
           
            return View(store);
        }


    }
}