using MasterPiece.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var products = db.Products.Where(x=>x.Store_id== storeid).ToList();

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
    }
}