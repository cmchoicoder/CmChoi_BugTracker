﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmChoi_BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string AuthorId { get; set; }

        public string CommentBody { get; set; }
        public DateTime Created { get; set; }

        //nav
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Author { get; set; }

    }
}