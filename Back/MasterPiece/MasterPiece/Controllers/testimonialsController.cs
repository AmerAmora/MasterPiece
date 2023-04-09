using MasterPiece.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterPiece.Controllers
{
    public class testimonialsController : Controller
    {
        private MasterPieceEntities db = new MasterPieceEntities();

        // GET: testimonials
        public ActionResult Index()
        {
            return View(db.testimonials.ToList());
        }

        // GET: testimonials/Details/5
        public ActionResult Details(int id)
        {
            var testimonial = db.testimonials.Find(id);

            return View(testimonial);
        }

        public ActionResult Accept(int id)
        {
            var testimonial = db.testimonials.Find(id);
            testimonial.isAccepted = true;
            db.SaveChanges();

            return View("Index", db.testimonials.ToList());
        }
        public ActionResult Reject(int id)
        {
            var testimonial = db.testimonials.Find(id);
            testimonial.isAccepted = false;
            db.SaveChanges();

            return View("Index", db.testimonials.ToList());
        }
    }
}
