using CmChoi_BugTracker.Models;
using System.Linq;
using System.Web.Mvc;
using CmChoi_BugTracker.Helpers;
using System.Collections.Generic;

namespace CmChoi_BugTracker.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projectHelper = new ProjectsHelper();


        // GET: Admin
        public ActionResult UserIndex()
        {
            var users = db.Users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Email = u.Email
            }).ToList();

            return View(users);
        }
        //GET: UserRole
        public ActionResult ManageUserRole(string userId)
        {
            //How do I load up a DropDownList with Role information??
            //new SelectList("The list of data pushed into the control",
            //               "The column that will be used to commnicate my selcetion(s) to the post"
            //               "The column that we show the user inside the control",
            //               "If they already occupy a role...show this instead of nothing")
            var currentRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            ViewBag.UserId = userId;
            ViewBag.UserRole = new SelectList(db.Roles.ToList(), "Name","Name",currentRole);
            return View();
        }

        //POST: UserRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserRole(string userId, string userRole)
        {
            //This is where I will be using the UseRoleHelper class to make suer my user occupies
            //The first thing I want to do is to make sure I remove the user from ALL ROLES they may occupy
            foreach (var role in roleHelper.ListUserRoles(userId))
            {
                roleHelper.RemoveUserFromRole(userId, role);
            }

            //If the incoming role selection IS NOT NULL I want to assign the user to the selected role
            if (!string.IsNullOrEmpty(userRole))
            {
                roleHelper.AddUserToRole(userId, userRole);
            }

            return RedirectToAction("UserIndex");
        }
        //GET:
        public ActionResult ManageRoles()
        {
            var users = db.Users.Select(u => new UserProfileViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DisplayName = u.DisplayName,
                AvatarUrl = u.AvatarUrl,
                Email = u.Email
            }).ToList();

            ViewBag.Users = new MultiSelectList(db.Users.ToList(), "Id", "Email");
            ViewBag.RoleName = new SelectList(db.Roles.ToList(), "Name", "Name");


            return View(users);
        }
        //Post:
        [HttpPost]
        public ActionResult ManageRoles(List<string> users, string roleName)
        {
            //Let's iterate over the incoming list of Users that were selected from the form
            

            foreach (var userId in users)
            {
                //Get a list of roles for this user
                //and remove each of them from whatever role they occupy only to add them back to the selected rold
                foreach (var role in roleHelper.ListUserRoles(userId))
                {
                    roleHelper.RemoveUserFromRole(userId, role);
                }
                //Only to add the back to the selected role
                if (!string.IsNullOrEmpty(roleName))
                {
                     roleHelper.AddUserToRole(userId, roleName);
                }
            }

            return RedirectToAction("ManageRoles");
        }
        //GET: Manage User Projects
        public ActionResult ManageUserProjects(string userId)
        {
            //I just need a list of project that this is on...
            var myProjects = projectHelper.ListUserProjects(userId).Select(p => p.Id);
            ViewBag.Projects = new MultiSelectList(db.Projects.ToList(), "Id", "Name", myProjects);
            return View();
        }
        //Post: Manage User Projects
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUserProjects(List<int> projects, string userId)
        {
            //First lets remove the user from all projects
            foreach (var project in projectHelper.ListUserProjects(userId).ToList())
            {
                projectHelper.RemoveUserFromProject(userId, project.Id);
            }

            //Then if the incoming list is not null we will reassign them to all that are selected
            if (projects != null)
            {
                foreach (var projectId in projects)
                {
                    projectHelper.AddUserToProject(userId, projectId);
                }
            }

            return RedirectToAction("UserIndex");
        }

        //POST: Details/Manage Project Users
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectUsers(int projectId, List<string> ProjectManagers, List<string> Developers, List<string> Submitters)
        {
            //Step1: Remove all users from the project
            foreach (var user in projectHelper.UsersOnProject(projectId).ToList())
            {
                projectHelper.RemoveUserFromProject(user.Id, projectId);
            }
            //Step2: Add back all the selected PM's
            if (ProjectManagers != null)
            {
                foreach (var projectManagerId in ProjectManagers)
                {
                    projectHelper.AddUserToProject(projectManagerId, projectId);
                }
            }
            
            //Step3: Add back all the selected Developers
            if (Developers != null)
            {
                foreach (var developerId in Developers)
                {
                    projectHelper.AddUserToProject(developerId, projectId);
                }
            }
            //Step4: Add back all the selected Submitters
            if (Submitters != null)
            {
                foreach (var submitterId in Submitters)
                {
                    projectHelper.AddUserToProject(submitterId, projectId);
                }
            }
            //Step5: Redirect the user somewhere
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }


        public ActionResult ManageProjects()
        {

            return View();
        }

        public ActionResult ManageUsers()
        {

            return View();
        }

        public ActionResult TicketError()
        {
            return View();
        }
    }
}