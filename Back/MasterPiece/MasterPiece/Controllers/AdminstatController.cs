using MasterPiece.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterPiece.Controllers
{
    public class AdminstatController : Controller
    {
        private MasterPieceEntities db = new MasterPieceEntities();

        // GET: Adminstat
        public ActionResult Index()
        {
            ViewBag.totalSales = db.Transactions.Sum(x => x.Amount);
            DateTime today = DateTime.Today;
            DateTime startOfToday = today.Date;
            DateTime endOfToday = startOfToday.AddDays(1).AddTicks(-1);
            double totalSales = Convert.ToDouble(db.Transactions
                .Where(x => x.TransactionDate >= startOfToday && x.TransactionDate <= endOfToday)
                .Sum(x => x.Amount));
            if (totalSales == null)
                ViewBag.todaySales = 0;
            else
                ViewBag.todaySales = totalSales;
            ViewBag.todaysRevenue = ViewBag.todaySales * 10 / 100;

            ViewBag.totalRevenue = ViewBag.totalSales * 10 / 100;


            return View();
        }
    }
}