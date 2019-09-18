using CmChoi_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmChoi_BugTracker.Helpers
{

   
    public class NavigationHelpers : CommonHelper
    {
       
        public static List<Project> ListUserProjects(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Project>();


            //User Projects are whichever Projects the User is assigned to ...this is not based on Role
            return db.Users.Find(userId).Projects.ToList();
        }
        
       
        public static List<Ticket> ListUserTickets(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Ticket>();

            //Ticket Lists are based on Role...
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch (myRole)
            {
                case "Admin":
                    return db.Tickets.ToList();
                 
                case "ProjectManager":
                    return db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).ToList();
                   
                case "Developer":
                    return db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
                 
                case "Submitter":
                    return db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
                  
                default:
                    //Basically return an empty list just to satisfy the return type
                    return new List<Ticket>();

            }

        }
    }
}