using CmChoi_BugTracker.Helpers;
using CmChoi_BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmChoi_BugTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        public ActionResult Dashboard()
        {
            //int recentTicketCounter = 0;
            //int immediateTicketCounter = 0;

            //foreach(Ticket ticket in db.Tickets.ToList())
            //{
            //    if(ticket.Created.AddHours(24) <= DateTime.Now)
            //    {
            //        recentTicketCounter++;
            //    }
            //    if(ticket.TicketPriority.Name == "Immediate")
            //    {
            //        immediateTicketCounter++;
            //    }
            //}
            
            //First get the Id of the logged in User
            var userId = User.Identity.GetUserId();

            //Then get the Role they occupy
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            var myTickets = new List<Ticket>();

            //Then based on the role name we will push different data into the view
            switch (myRole)
            {
                case "Developer":
                    myTickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                    break;
                case "Submitter":
                    myTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                    break;
                case "ProjectManager":
                    //mytickets are going to be all the Tickets on all the Project I am no.
                    myTickets = db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).ToList();
                    break;
                case "Admin":
                    //mytickets are going to be all the Tickets on all the Project I am no.
                    myTickets = db.Tickets.ToList();
                    break;
            }

            ViewBag.TotalTicketNumber = myTickets.Count();

            var yesterday = DateTime.Now.AddHours(-24);
            ViewBag.RecentTicketNumber = myTickets.Where(t => t.Created >= yesterday).Count();
            ViewBag.ImmediateTicketNumber = myTickets.Where(t => t.TicketPriority.Name == "Immediate").Count();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}