using MasterPiece.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterPiece.Controllers
{
    public class HomeController : Controller
    {
        private MasterPieceEntities db = new MasterPieceEntities();

        public ActionResult Index()
        {
            var products = db.Products.ToList();

            return View(products);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Products()
        {
            var products = db.Products.ToList();

            return View(products);
        }
        public ActionResult Categories()
        {
            var categories = db.Categories.ToList();    
            return View(categories);
        }
        public ActionResult SingleCategory(int? id)
        {
            var Products = db.Products.Where(x=>x.Category_id==id).ToList();
            return View(Products);
        }
        public ActionResult Cart() 
        {
            HttpCookie cart = Request.Cookies["cart"];
            List<Product> allProducts = new List<Product>();
            if (cart != null)
            {
                // read the cart items from the cookie
                List<string> items = new List<string>(cart.Value.Split('|'));
                dynamic UserProducts=new Product();

                foreach (var product in items) {
                    int id = Convert.ToInt32(product);
                    List<Product> itemProducts = db.Products.Where(x => x.Product_id == id).ToList();
                    allProducts.AddRange(itemProducts);
                }
                UserProducts = allProducts;
                // render the shopping cart view with the cart items
                return View(UserProducts);
            }
            else
            {
                IEnumerable<Product> model = new List<Product>();
                // render the empty cart view
                return View(model);
            }

        }
        public ActionResult AddItem(int id, string returnUrl)
        {
            // add the selected item to the shopping cart
            // ...

            // create a new cart cookie
            HttpCookie cart = new HttpCookie("cart");


            // read the existing cart items from the cookie
            List<string> items = new List<string>();
            if (Request.Cookies["cart"] != null)
            {
                items = new List<string>(Request.Cookies["cart"].Value.Split('|'));
            }

            // add the new item to the cart
            items.Add(id.ToString());

            // update the cart cookie with the new items
            cart.Value = string.Join("|", items);
            cart.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Add(cart);

            // redirect back to the shopping cart view
            return Redirect(returnUrl);
        }
        public ActionResult RemoveItem(int id)
        {
            // remove the selected item from the shopping cart
            // ...

            // create a new cart cookie
            HttpCookie cart = new HttpCookie("cart");

            // read the existing cart items from the cookie
            List<string> items = new List<string>();
            if (Request.Cookies["cart"] != null)
            {
                items = new List<string>(Request.Cookies["cart"].Value.Split('|'));
            }

            // remove the item from the cart
            items.Remove(id.ToString());

            // update the cart cookie with the new items or delete it if no items left
            if (items.Count > 0)
            {
                cart.Value = string.Join("|", items);
                cart.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cart);
            }
            else
            {
                cart.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cart);
            }


            // redirect back to the shopping cart view
            return RedirectToAction("Cart");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}