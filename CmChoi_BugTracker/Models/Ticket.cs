﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CmChoi_BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Display(Name = "Status")]
        public int TicketStatusId { get; set; }

        [Display(Name = "Priority")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Type")]
        public int TicketTypeId { get; set; }

        [Display(Name = "Submitter")]
        public string OwnerUserId { get; set; }

        [Display(Name = "Developer")]
        public string AssignedToUserId { get; set; }

        //[StringLength(50, ErrorMessage ="The Title must  be between {5} and {50} characters long.", MinimumLength = 5)]
        public string Title { get; set; }
        //[StringLength(200, ErrorMessage = "The Title must  be between {5} and {200} characters long.", MinimumLength = 5)]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }


        //nav properties
        public virtual Project Project { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }

        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

        public Ticket()
        {
            TicketComments = new HashSet<TicketComment>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketHistories = new HashSet<TicketHistory>();
            TicketNotifications = new HashSet<TicketNotification>();
        }

    }
}