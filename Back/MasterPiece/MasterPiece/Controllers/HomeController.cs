using MasterPiece.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace MasterPiece.Controllers
{
    public class HomeController : Controller
    {
        private MasterPieceEntities db = new MasterPieceEntities();

        public ActionResult Index()
        {
            var products = db.Products.OrderByDescending(p => p.Product_id).
                Where(x=>x.Store.isAccepted==true&&x.isDeleted!=true&&x.Store.isBlocked!=true).ToList();
            var testimonia = db.testimonials.Where(x=>x.isAccepted==true).ToList();
            var data = Tuple.Create(products, testimonia);


            return View(data);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Products()
        {
            var products = db.Products.Where(x=>x.Store.isBlocked!=true&& x.isDeleted !=true).ToList();

            return View(products);
        }
        [Authorize(Roles = "User")]

        public ActionResult CheckOut()
        {
            string userid = User.Identity.GetUserId();
            HttpCookie cart = Request.Cookies["cart"];
            List<Product> allProducts = new List<Product>();
            if (cart != null)
            {
                // read the cart items from the cookie
                List<string> items = new List<string>(cart.Value.Split('|'));
                double totalprice = 0;
                foreach (var product in items)
                {
                    int id = Convert.ToInt32(product);
                    Product itemProducts = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
                    totalprice = totalprice + Convert.ToDouble(itemProducts.Product_Price);
                    
                }
                if (totalprice < 100) totalprice = totalprice + 15;
                ViewBag.TotalPrice = totalprice;    
           
                // render the shopping cart view with the cart items
            }
         return View();

        }
        public ActionResult CheckOutItems() 
        {
            Guid guid = Guid.NewGuid();
            HttpCookie cart = Request.Cookies["cart"];

            List<string> items = new List<string>(cart.Value.Split('|'));
            int productid = Convert.ToInt32(items[0]);
            int storeid =Convert.ToInt32( db.Products.Where(x => x.Product_id == productid).FirstOrDefault().Store_id);
            Order neworder = new Order();
            neworder.Order_id = guid.ToString();
            neworder.userId = User.Identity.GetUserId();
            neworder.Order_Date = DateTime.Now;
            neworder.Store_Id = storeid;
            db.Orders.Add(neworder);
            double totalprice = 0;
            foreach (var product in items)
            {
                int id = Convert.ToInt32(product);
                Product itemProducts = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
                totalprice = totalprice + Convert.ToDouble(itemProducts.Product_Price);
                Order_Details newOrderDetail = new Order_Details();
                newOrderDetail.Product_id = id;
                newOrderDetail.Order_id = guid.ToString();
                db.Order_Details.Add(newOrderDetail);
                db.SaveChanges();
            }
            neworder.Amount = totalprice;
            Transaction newTransaction = new Transaction();
            newTransaction.Amount = totalprice;
            newTransaction.Order_id = guid.ToString();
            newTransaction.userId = User.Identity.GetUserId();
            newTransaction.TransactionDate = DateTime.Now;
            cart.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(cart);
            
            db.Transactions.Add(newTransaction);
            db.SaveChanges();


            return RedirectToAction("Cart","Home","");
        }

        public ActionResult Categories()
        {
            var categories = db.Categories.ToList();    
            return View(categories);
        }
        public ActionResult SingleCategory(int? id)
        {
            var Products = db.Products.Where(x=>x.Category_id==id&&x.isDeleted!=true).ToList();
            return View(Products);
        }
        public ActionResult SingleProduct(int? id)
        {
            var Product = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
            var comments = db.Comments.Where(x => x.Product_id == id).ToList();
            var data = Tuple.Create(comments, Product);
            return View(data);
        }
        public ActionResult Cart()
        {
            HttpCookie cart = Request.Cookies["cart"];
            List<Product> allProducts = new List<Product>();
            dynamic UserProducts = new Product();

            if (cart != null)
            {
                // read the cart items from the cookie
                List<string> items = new List<string>(cart.Value.Split('|'));

                foreach (var item in items)
                {
                    int id = Convert.ToInt32(item.Split('_')[0]);
                    int quantity = Convert.ToInt32(item.Split('_')[1]);
                    List<Product> itemProducts = db.Products.Where(x => x.Product_id == id).ToList();

                    // set the quantity for each product
                    foreach (var product in itemProducts)
                    {
                        product.QuantityInCart = quantity;
                    }

                    allProducts.AddRange(itemProducts);
                }

                UserProducts = allProducts;
            }
            else
            {
                IEnumerable<Product> model = new List<Product>();
                // render the empty cart view
                return View(model);
            }

            return View(UserProducts);
        }
        public ActionResult AddItem(int id, string returnUrl)
        {
            // create a new cart cookie
            HttpCookie cart = new HttpCookie("cart");

            // read the existing cart items from the cookie
            List<string> items = new List<string>();
            if (Request.Cookies["cart"] != null)
            {
                items = new List<string>(Request.Cookies["cart"].Value.Split('|'));
            }

            // check if the item is already in the cart
            bool itemFound = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].StartsWith(id.ToString() + "_"))
                {
                    // update the existing item's quantity
                    int quantity = Convert.ToInt32(items[i].Split('_')[1]) + 1;
                    items[i] = id.ToString() + "_" + quantity.ToString();
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                // add the new item to the cart with a quantity of 1
                items.Add(id.ToString() + "_1");
            }

            // update the cart cookie with the new items
            cart.Value = string.Join("|", items);
            cart.Expires = DateTime.Now.AddDays(7);
            Response.Cookies.Add(cart);

            // redirect back to the shopping cart view
            return Redirect(returnUrl);
        }
        public ActionResult RemoveItem(int id)
        {
            HttpCookie cart = new HttpCookie("cart");

            // read the existing cart items from the cookie
            List<string> items = new List<string>();
            if (Request.Cookies["cart"] != null)
            {
                items = new List<string>(Request.Cookies["cart"].Value.Split('|'));
            }

            // remove the item from the cart
            items.RemoveAll(x => x.Split('_')[0] == id.ToString());

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
        public ActionResult AddComment(int Product_id, string Comment_text, string returnUrl) 
        {
            Comment comment = new Comment()
            {
                userId = User.Identity.GetUserId(),
                Product_id = Product_id,
                commentDate = DateTime.Now.Date,
                Comment_text = Comment_text
            };
            db.Comments.Add(comment);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult Contact()
        {

            return View();
        }
        public ActionResult NoAccess()
        {

            return View();
        }


        public ActionResult Stores() 
        {
            var stores = db.Stores.Where(x=>x.isBlocked!=true&& x.isAccepted==true).ToList();
            return View(stores);
        
        }
        public ActionResult SingleStore(int? id)
        {
            var Products = db.Products.Where(x => x.Store_id == id &&x.isDeleted!=true ).ToList();
            return View(Products);
        }
        public ActionResult Testimonial() 
        {
            return View();
        }
        [HttpPost]
        public ActionResult Testimonial(string userMessage,string UserName,HttpPostedFileBase userImage)
        {
            testimonial testimonial= new testimonial();
            testimonial.UserName = UserName;
            testimonial.userMessage=userMessage;
            Guid guid = Guid.NewGuid();
            string path = guid + userImage.FileName;
            userImage.SaveAs(Server.MapPath("../assets/img/" + path));
            testimonial.userImage = path;
            db.testimonials.Add(testimonial);
            db.SaveChanges();
            return View();
        }



    }
}