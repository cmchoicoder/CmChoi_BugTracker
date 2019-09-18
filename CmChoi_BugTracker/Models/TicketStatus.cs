﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmChoi_BugTracker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //nav
        public virtual ICollection<Ticket> Tickets { get; set; }
        public TicketStatus()
        {
            Tickets = new HashSet<Ticket>();
        }

    }
}