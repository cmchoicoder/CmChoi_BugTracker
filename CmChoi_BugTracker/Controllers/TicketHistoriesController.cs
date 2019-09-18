using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CmChoi_BugTracker.Helpers;
using CmChoi_BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace CmChoi_BugTracker.Controllers
{
    public class TicketHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();


        // GET: TicketHistories
        [Authorize]
        public ActionResult Index()
        {
            var ticketHistories = db.TicketHistories.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketHistories.ToList());
        }


        //Get: MyTicketHistories
        //[Authorize(Roles = "Admin,ProjectManager,Developer,submitter")]
        //public ActionResult MyTicketHistories()
        //{
        //    var userId = User.Identity.GetUserId();
        //    var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
        //    var myTicketHistories = new List<TicketHistory>();

        //    switch (myRole)
        //    {
        //        case "Developer":
        //            myTicketHistories = db.TicketHistories.
        //            break;
        //        case "Submitter":
        //            myTicketHistories = db.
        //            break;
        //        case "ProjectManager":
        //            myTicketHistories = db.Users.Find(userId).TicketHistories.ToList();
        //            break;
        //    }

        //    return View("Index", myTicketHistories);
        //}

        // GET: TicketHistories/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistory);
        }

        // GET: TicketHistories/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,UserId,PropertyName,OldValue,NewValue,Updated")] TicketHistory ticketHistory)
        {
            if (ModelState.IsValid)
            {
                db.TicketHistories.Add(ticketHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId", ticketHistory.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketHistory.UserId);
            return View(ticketHistory);
        }

        // GET: TicketHistories/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId", ticketHistory.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketHistory.UserId);
            return View(ticketHistory);
        }

        // POST: TicketHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,PropertyName,OldValue,NewValue,Updated")] TicketHistory ticketHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "OwnerUserId", ticketHistory.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketHistory.UserId);
            return View(ticketHistory);
        }

        // GET: TicketHistories/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            if (ticketHistory == null)
            {
                return HttpNotFound();
            }
            return View(ticketHistory);
        }

        // POST: TicketHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketHistory ticketHistory = db.TicketHistories.Find(id);
            db.TicketHistories.Remove(ticketHistory);
            db.SaveChanges();
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
