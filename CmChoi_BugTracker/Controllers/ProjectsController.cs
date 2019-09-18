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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();

        // GET: Projects
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }


        public ActionResult MyIndex()
        {
            //First get the Id of the logged in User
            var userId = User.Identity.GetUserId();

            //Then get the Role they occupy
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var myProjects = new List<Project>();

            //Then based on the role name we will push different data into the view
            switch (myRole)
            {
                case "Developer":
                    myProjects = db.Users.Find(userId).Projects.ToList();
                    break;
                case "Submitter":
                    myProjects = db.Users.Find(userId).Projects.ToList();
                    break;
                case "ProjectManager":
                    //mytickets are going to be all the Tickets on all the Project I am no.
                    myProjects = db.Users.Find(userId).Projects.ToList();
                    break;
            }
            //MyIndex wants to fill some view with MY Tickets only.
            //Step 1: Ask the question, "What role do I occypy"
            //If I am a Submitter my Tickets are the Tickets where the OwnerUserId equals my Id.
            //If I am the Developer, my Tickets are the Tickets where the AssignedToUserId is my Id
            return View("MyIndex", myProjects);
        }
        
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            //Give my details view a multiSelectList of available PM's
            //Keep an eye out for 'magic strings' throughout your spplication..
            //Magic strings are hard coded strings that should be handled differently
            var allProjectManagers = roleHelper.UsersInRole("ProjectManager");
            var currentProjectManagers = projectHelper.UsersInRoleOnProject(project.Id, "ProjectManager");
            ViewBag.ProjectManagers = new MultiSelectList(allProjectManagers, "Id", "FullNameWithEmail", currentProjectManagers);

            var allSubmitters = roleHelper.UsersInRole("Submitter");
            var currentSubmitters = projectHelper.UsersInRoleOnProject(project.Id, "Submitter");
            ViewBag.Submitters = new MultiSelectList(allSubmitters, "Id", "FullNameWithEmail", currentSubmitters);

            var allDevelopers = roleHelper.UsersInRole("Developer");
            var currentDevelopers = projectHelper.UsersInRoleOnProject(project.Id, "Developer");
            ViewBag.Developers = new MultiSelectList(allDevelopers, "Id", "FullNameWithEmail", currentDevelopers);

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
