﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CmChoi_BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Display(Name="Project Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

        //virtual nav
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();


        }

    }
}