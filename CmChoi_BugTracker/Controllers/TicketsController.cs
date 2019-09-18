using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CmChoi_BugTracker.Helpers;
using CmChoi_BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace CmChoi_BugTracker.Controllers
{ 
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();


        [Authorize(Roles ="Admin")]
        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.ToList();
            return View(tickets);
        }



        [Authorize(Roles = "Admin,ProjectManager,Developer,Submitter")]
        public ActionResult MyIndex(string req)
        {
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
                    myTickets = db.Tickets.ToList();
                    break;
            }

            switch (req)
            {
                case "Immediate":
                    myTickets = myTickets.Where(t => t.TicketPriority.Name.Equals("Immediate")).ToList();
                    break;
                case "Recent":
                    var yesterday = DateTime.Now.AddHours(-24);
                    myTickets = myTickets.Where(t => t.Created >= yesterday).ToList();
                    break;
            }

            //MyIndex wants to fill some view with MY Tickets only.
            //Step 1: Ask the question, "What role do I occypy"
            //If I am a Submitter my Tickets are the Tickets where the OwnerUserId equals my Id.
            //If I am the Developer, my Tickets are the Tickets where the AssignedToUserId is my Id
            return View("Index", myTickets);
        }



        // GET: Tickets/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var myProjects = projectHelper.ListUserProjects(User.Identity.GetUserId());
            ProjectsHelper projectsHelper = new ProjectsHelper();
            List<ApplicationUser> projectusers = new List<ApplicationUser>();
            List<string> projectDevIds = new List<string>();
            foreach (Project project in myProjects)
            {
                projectDevIds = projectsHelper.UsersInRoleOnProject(project.Id, "Developer");
            }
            foreach (string devId in projectDevIds)
            {
                ApplicationUser devUser = db.Users.Find(devId);
                projectusers.Add(devUser);
            }
            ViewBag.AssignedToUserId = new SelectList(projectusers, "Id", "DisplayName", ticket.AssignedToUserId);



            return View(ticket);
        }


        //POST: Tickets/Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string AssignedToUserId, int ticketid)
        {
            Ticket ticket = db.Tickets.Find(ticketid);
            ticket.AssignedToUserId = AssignedToUserId;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketid });
        }





        
        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            
            var userId = User.Identity.GetUserId();
            var myProject = projectHelper.ListUserProjects(userId);

            ViewBag.ProjectId = new SelectList(myProject, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,TicketPriorityId,TicketTypeId,Title,Description")] Ticket ticket)
        {
            //Since the user is not able to set the initial Ticket Status we have to do it behind the scene
            //like we set the Created date on a new BlogPost...
            //How is this done?? 'Seed Data Method'

            if (ModelState.IsValid)
            {
                ticket.Created = DateTime.Now;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(t => t.Name == "New/Unassigned").Id;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("MyIndex","Tickets");
            }

          
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }




        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Edit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Ticket ticket = db.Tickets.Find(id);

            //Kick this Developer out if they are not the Developer on this Ticket
            //UserRolesHelper helper = new UserRolesHelper();
            //string userId = User.Identity.GetUserId();
            //bool IsAdmin = helper.IsUserInRole(userId, "Admin");

            //Based on my role should i be able to edit this Ticket
            //if (ticket.AssignedToUserId != userId && !IsAdmin)
            //{
            //    return RedirectToAction("TicketError", "Admin");
            //}

            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (TicketDecisionHelper.TicketIsEditableByUser(ticket))
            {
                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FullName", ticket.AssignedToUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FullName", ticket.OwnerUserId);
                return View(ticket);
            }
            else
            {
                return RedirectToAction("AccessViolation", "Admin");
            }
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Admin, Developer, ProjectManager, Submitter")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketStatusId,TicketPriorityId,TicketTypeId,OwnerUserId,AssignedToUserId,Title,Description,Created,Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //Go out to the DB and get a copy of the Ticket before it is changed
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                ticket.Updated = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                //Now call the NotificationHelper to determine if a Notification needs to be created
                var notificationHelper = new NotificationHelper();
                notificationHelper.ManageNotifications(oldTicket, ticket);

                //Now I need a History helper as well....               
                var historyHelper = new HistoryHelper();
                historyHelper.RecordHistory(oldTicket, ticket);

                return RedirectToAction("MyIndex");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FullName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin, Developer, ProjectManager, Submitter")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("MyIndex");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //GET
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult AssignTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            UserRolesHelper helper = new UserRolesHelper();
            var users = helper.UsersInRole("Developer").ToList();
            if(users == null)
            {
                users = new List<ApplicationUser>();
            }
            ViewBag.AssignedToUserId = new SelectList(users, "Id", "FullName", ticket.AssignedToUserId);
            return View(ticket);
        }

        //POST
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignTicket(Ticket model)
        {
            //Go out to the DB and get a copy of the Ticket before it is changed
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(m => m.Id == model.Id);

            var ticket = db.Tickets.Find(model.Id);
            ticket.AssignedToUserId = model.AssignedToUserId;
            ticket.Updated = DateTime.Now;
            db.SaveChanges();

            //Now call the NotificationHelper to determine if a Notification needs to be created
            var notificationHelper = new NotificationHelper();
            notificationHelper.ManageNotifications(oldTicket, ticket);

            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);

            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(model.AssignedToUserId);
                msg.Destination = user.Email;
                msg.Body = "You have been assigned a new Ticket." + Environment.NewLine + "Please click the following link to view the details " + "<a href=\"" + callbackUrl + "\"> NEW TICKET</a>";
                msg.Subject = "New Ticket Assignment";

                await ems.SendMailAsync(msg);
            }
            catch (Exception)
            {
                await Task.FromResult(0);
            }
            return RedirectToAction("MyIndex");
        }
    }
}
