using CmChoi_BugTracker.Enumerations;
using CmChoi_BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CmChoi_BugTracker.Helpers
{
    public class TicketDecisionHelper : CommonHelper
    {
        public static bool TicketIsDetailIsViewableByUser(int ticketId)
        {
            //How do I use User when I am ouside of the Controller??
            var userId = HttpContext.Current.User.Identity.GetUserId();

            var roleName = roleHelper.ListUserRoles(userId).FirstOrDefault();
            var systemRole = (SystemRole)Enum.Parse(typeof(SystemRole), roleName);

            switch (systemRole)
            {
                case SystemRole.Admin:
                    break;
                case SystemRole.ProjectManager:
                    break;
                case SystemRole.Developer:
                    break;
                case SystemRole.Submitter:
                    break;
            }
            return true;

        }

        public static bool TicketIsEditableByUser(Ticket ticket)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Developer":
                    return ticket.AssignedToUserId == userId;
                case "Submitter":
                    return ticket.OwnerUserId == userId;
                case "ProjectManager":

                    var myProjects = projectHelper.ListUserProjects(userId);
                    foreach (var project in myProjects)
                    {
                        foreach (var projticket in project.Tickets)
                        {
                            if (projticket.Id == ticket.Id)
                                return true;
                        }
                    }
                    return false;
                case "Admin":
                    return true;
                default:
                    return false;
            }

        }

    }

}