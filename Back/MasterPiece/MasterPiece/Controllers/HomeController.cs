using MasterPiece.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

                foreach (var item in items)
                {
                    int id = Convert.ToInt32(item.Split('_')[0]);
                    int quantity = Convert.ToInt32(item.Split('_')[1]);
                    Product product = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
                    if (quantity > 0)
                    {
                        totalprice = Convert.ToDouble(totalprice + (product.Product_Price * quantity));
                    }
                    else { return RedirectToAction("Error"); }
                   

                }
                if (totalprice < 100) totalprice = totalprice + 15;
                ViewBag.TotalPrice = totalprice;

            }
            return View();

        }
        public ActionResult CheckOutItems() 
        {
            Guid guid = Guid.NewGuid();
            HttpCookie cart = Request.Cookies["cart"];
            List<string> items = new List<string>(cart.Value.Split('|'));
            double totalprice = 0;
            int productid = Convert.ToInt32(Convert.ToInt32(items[0].Split('_')[0]));
            int storeid =Convert.ToInt32( db.Products.Where(x => x.Product_id == productid).FirstOrDefault().Store_id);
            Order neworder = new Order();
            neworder.Order_id = guid.ToString();
            neworder.userId = User.Identity.GetUserId();
            neworder.Order_Date = DateTime.Now;
            neworder.Store_Id = storeid;
            db.Orders.Add(neworder);

            SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587);

            // Enable SSL encryption
            client.EnableSsl = true;

            // Set your credentials (username and password)
            client.Credentials = new System.Net.NetworkCredential("NoCatsMasterPiece@outlook.com", "***Zz123");

            // Create a new email message object
            MailMessage message = new MailMessage();

            // Set the sender and recipient addresses
            message.From = new MailAddress("NoCatsMasterPiece@outlook.com");
            message.To.Add(new MailAddress(User.Identity.GetUserName()));

            // Set the subject and body of the email
            message.Subject = "Welcome To NoCats";
            message.IsBodyHtml = true;
            string EmailBody =
   "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n\r\n  <meta charset=\"utf-8\">\r\n  <meta http-equiv=\"x-ua-compatible\" content=\"ie=edge\">\r\n  <title>Email Receipt</title>\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n  <style type=\"text/css\">\r\n  /**\r\n   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.\r\n   */\r\n  @media screen {\r\n    @font-face {\r\n      font-family: 'Source Sans Pro';\r\n      font-style: normal;\r\n      font-weight: 400;\r\n      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');\r\n    }\r\n\r\n    @font-face {\r\n      font-family: 'Source Sans Pro';\r\n      font-style: normal;\r\n      font-weight: 700;\r\n      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');\r\n    }\r\n  }\r\n\r\n  /**\r\n   * Avoid browser level font resizing.\r\n   * 1. Windows Mobile\r\n   * 2. iOS / OSX\r\n   */\r\n  body,\r\n  table,\r\n  td,\r\n  a {\r\n    -ms-text-size-adjust: 100%; /* 1 */\r\n    -webkit-text-size-adjust: 100%; /* 2 */\r\n  }\r\n\r\n  /**\r\n   * Remove extra space added to tables and cells in Outlook.\r\n   */\r\n  table,\r\n  td {\r\n    mso-table-rspace: 0pt;\r\n    mso-table-lspace: 0pt;\r\n  }\r\n\r\n  /**\r\n   * Better fluid images in Internet Explorer.\r\n   */\r\n  img {\r\n    -ms-interpolation-mode: bicubic;\r\n  }\r\n\r\n  /**\r\n   * Remove blue links for iOS devices.\r\n   */\r\n  a[x-apple-data-detectors] {\r\n    font-family: inherit !important;\r\n    font-size: inherit !important;\r\n    font-weight: inherit !important;\r\n    line-height: inherit !important;\r\n    color: inherit !important;\r\n    text-decoration: none !important;\r\n  }\r\n\r\n  /**\r\n   * Fix centering issues in Android 4.4.\r\n   */\r\n  div[style*=\"margin: 16px 0;\"] {\r\n    margin: 0 !important;\r\n  }\r\n\r\n  body {\r\n    width: 100% !important;\r\n    height: 100% !important;\r\n    padding: 0 !important;\r\n    margin: 0 !important;\r\n  }\r\n\r\n  /**\r\n   * Collapse table borders to avoid space between cells.\r\n   */\r\n  table {\r\n    border-collapse: collapse !important;\r\n  }\r\n\r\n  a {\r\n    color: #1a82e2;\r\n  }\r\n\r\n  img {\r\n    height: auto;\r\n    line-height: 100%;\r\n    text-decoration: none;\r\n    border: 0;\r\n    outline: none;\r\n  }\r\n  </style>\r\n\r\n</head>\r\n<body style=\"background-color: #EDEDED;\">\r\n\r\n  <!-- start preheader -->\r\n  <div class=\"preheader\" style=\"display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;\">\r\n    A preheader is the short summary text that follows the subject line when an email is viewed in the inbox.\r\n  </div>\r\n  <!-- end preheader -->\r\n\r\n  <!-- start body -->\r\n  <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n\r\n    <!-- start logo -->\r\n    <tr>\r\n      <td align=\"center\" bgcolor=\"#EDEDED\">\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">\r\n        <tr>\r\n        <td align=\"center\" valign=\"top\" width=\"600\">\r\n        <![endif]-->\r\n        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n          <tr>\r\n            <td align=\"center\" valign=\"top\" style=\"padding: 36px 24px;\">\r\n              <a href=\"https://sendgrid.com\" target=\"_blank\" style=\"display: inline-block;\">\r\n                <img src=\"../../AdminContent/img/noCatsLogo.png\" alt =\"Logo\" border=\"0\" width=\"150\" style=\"display: block; width: 150px; max-width: 150px; min-width: 150px;\">\r\n              </a>\r\n            </td>\r\n          </tr>\r\n        </table>\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        </td>\r\n        </tr>\r\n        </table>\r\n        <![endif]-->\r\n      </td>\r\n    </tr>\r\n    <!-- end logo -->\r\n\r\n    <!-- start hero -->\r\n    <tr>\r\n      <td align=\"center\" bgcolor=\"#EDEDED\">\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">\r\n        <tr>\r\n        <td align=\"center\" valign=\"top\" width=\"600\">\r\n        <![endif]-->\r\n        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n          <tr>\r\n            <td align=\"left\" bgcolor=\"#ffffff\" style=\"padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;\">\r\n              <h1 style=\"margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;\">Thank you for your order!</h1>\r\n            </td>\r\n          </tr>\r\n        </table>\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        </td>\r\n        </tr>\r\n        </table>\r\n        <![endif]-->\r\n      </td>\r\n    </tr>\r\n    <!-- end hero -->\r\n\r\n    <!-- start copy block -->\r\n    <tr>\r\n      <td align=\"center\" bgcolor=\"#EDEDED\">\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">\r\n        <tr>\r\n        <td align=\"center\" valign=\"top\" width=\"600\">\r\n        <![endif]-->\r\n        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n\r\n          <!-- start copy -->\r\n          <tr>\r\n            <td align=\"left\" bgcolor=\"#ffffff\" style=\"padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">\r\n              <p style=\"margin: 0;\">Here is a summary of your recent order. If you have any questions or concerns about your order, please <a href=\"https://sendgrid.com\">contact us</a>.</p>\r\n            </td>\r\n          </tr>"
   + $"<tr>\r\n            <td align=\"left\" bgcolor=\"#ffffff\" style=\"padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">\r\n              <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                <tr>\r\n                  <td align=\"left\" bgcolor=\"#EDEDED\" width=\"75%\" style=\"padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\"><strong>Order #</strong></td>\r\n                  <td align=\"left\" bgcolor=\"#EDEDED\" width=\"25%\" style=\"padding: 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\"><strong>{guid.ToString()}</strong></td>\r\n                </tr>";
            string rows = "";
            foreach (var item in items)
            {
                int id = Convert.ToInt32(item.Split('_')[0]);
                int quantity = Convert.ToInt32(item.Split('_')[1]);
                Product product = db.Products.Where(x => x.Product_id == id).FirstOrDefault();
                if (quantity > 0)
                {
                    totalprice = Convert.ToDouble(totalprice + (product.Product_Price * quantity));
                }
                else { return RedirectToAction("Error"); }
                Order_Details newOrderDetail = new Order_Details();
                newOrderDetail.Product_id = id;
                newOrderDetail.Order_id = guid.ToString();
                newOrderDetail.Quantity = quantity;
                db.Order_Details.Add(newOrderDetail);
                db.SaveChanges();
                rows = rows +
                    $"<tr>\r\n                  <td align=\"left\" width=\"50%\" style=\"padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">{product.Product_Name}</td>\r\n                  <td align=\"left\" width=\"25%\" style=\"padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">{quantity}</td>\r\n                  <td align=\"left\" width=\"25%\" style=\"padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">{product.Product_Price*quantity} JOD</td>\r\n                </tr>";
            }

            EmailBody = EmailBody + rows + "</table>\r\n            </td>\r\n          </tr>\r\n          <!-- end reeipt table -->\r\n\r\n        </table>\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        </td>\r\n        </tr>\r\n        </table>\r\n        <![endif]-->\r\n      </td>\r\n    </tr>\r\n    <!-- end copy block -->\r\n\r\n    <!-- start receipt address block -->\r\n    <tr>\r\n      <td align=\"center\" bgcolor=\"#EDEDED\" valign=\"top\" width=\"100%\">\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">\r\n        <tr>\r\n        <td align=\"center\" valign=\"top\" width=\"600\">\r\n        <![endif]-->\r\n        <table align=\"center\" bgcolor=\"#ffffff\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n          <tr>\r\n            <td align=\"center\" valign=\"top\" style=\"font-size: 0; border-bottom: 3px solid #d4dadf\">\r\n              <!--[if (gte mso 9)|(IE)]>\r\n              <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">\r\n              <tr>\r\n              <td align=\"left\" valign=\"top\" width=\"300\">\r\n              <![endif]-->\r\n              <div style=\"display: inline-block; width: 100%; max-width: 50%; min-width: 240px; vertical-align: top;\">\r\n                <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 300px;\">\r\n                  <tr>\r\n                    <td align=\"left\" valign=\"top\" style=\"padding-bottom: 36px; padding-left: 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">\r\n                      <p><strong>Delivery Address</strong></p>\r\n                      <p>1234 S. Broadway Ave<br>Unit 2<br>Denver, CO 80211</p>\r\n                    </td>\r\n                  </tr>\r\n                </table>\r\n              </div>\r\n              <!--[if (gte mso 9)|(IE)]>\r\n              </td>\r\n              <td align=\"left\" valign=\"top\" width=\"300\">\r\n              <![endif]-->\r\n              <div style=\"display: inline-block; width: 100%; max-width: 50%; min-width: 240px; vertical-align: top;\">\r\n                <table align=\"left\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 300px;\">\r\n                  <tr>\r\n                    <td align=\"left\" valign=\"top\" style=\"padding-bottom: 36px; padding-left: 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;\">\r\n                      <p><strong>Billing Address</strong></p>\r\n                      <p>1234 S. Broadway Ave<br>Unit 2<br>Denver, CO 80211</p>\r\n                    </td>\r\n                  </tr>\r\n                </table>\r\n              </div>\r\n              <!--[if (gte mso 9)|(IE)]>\r\n              </td>\r\n              </tr>\r\n              </table>\r\n              <![endif]-->\r\n            </td>\r\n          </tr>\r\n        </table>\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        </td>\r\n        </tr>\r\n        </table>\r\n        <![endif]-->\r\n      </td>\r\n    </tr>\r\n    <!-- end receipt address block -->\r\n\r\n    <!-- start footer -->\r\n    <tr>\r\n      <td align=\"center\" bgcolor=\"#EDEDED\" style=\"padding: 24px;\">\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"600\">\r\n        <tr>\r\n        <td align=\"center\" valign=\"top\" width=\"600\">\r\n        <![endif]-->\r\n        <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\r\n\r\n          <!-- start permission -->\r\n          <tr>\r\n            <td align=\"center\" bgcolor=\"#EDEDED\" style=\"padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;\">\r\n              <p style=\"margin: 0;\">You received this email because we received a request for [type_of_action] for your account. If you didn't request [type_of_action] you can safely delete this email.</p>\r\n            </td>\r\n          </tr>\r\n          <!-- end permission -->\r\n\r\n          <!-- start unsubscribe -->\r\n          <tr>\r\n            <td align=\"center\" bgcolor=\"#EDEDED\" style=\"padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;\">\r\n              <p style=\"margin: 0;\">To stop receiving these emails, you can <a href=\"https://sendgrid.com\" target=\"_blank\">unsubscribe</a> at any time.</p>\r\n              <p style=\"margin: 0;\">Paste 1234 S. Broadway St. City, State 12345</p>\r\n            </td>\r\n          </tr>\r\n          <!-- end unsubscribe -->\r\n\r\n        </table>\r\n        <!--[if (gte mso 9)|(IE)]>\r\n        </td>\r\n        </tr>\r\n        </table>\r\n        <![endif]-->\r\n      </td>\r\n    </tr>\r\n    <!-- end footer -->\r\n\r\n  </table>\r\n  <!-- end body -->\r\n\r\n</body>\r\n</html>";
            message.Body = EmailBody;
            client.Send(message);

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
        [HttpPost]
        public ActionResult UpdateCartQuantity(int productId, int cartQuantity)
        {
            HttpCookie cart = Request.Cookies["cart"];

            if (cart != null)
            {
                // read the cart items from the cookie
                List<string> items = new List<string>(cart.Value.Split('|'));
                var newItems = new List<string>();

                // update the quantity for the selected product
                foreach (var item in items)
                {
                    int id = Convert.ToInt32(item.Split('_')[0]);
                    int quantity = Convert.ToInt32(item.Split('_')[1]);

                    if (id == productId)
                    {
                        quantity = cartQuantity;
                    }

                    newItems.Add(id + "_" + quantity);
                }

                // set the updated cookie
                cart.Value = string.Join("|", newItems);
                Response.SetCookie(cart);
            }

            // return the updated cart quantity in JSON format
            return RedirectToAction("Cart");
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