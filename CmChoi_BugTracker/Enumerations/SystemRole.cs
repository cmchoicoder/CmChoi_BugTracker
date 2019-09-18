using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmChoi_BugTracker.Models;

namespace CmChoi_BugTracker.Enumerations
{
    public enum SystemRole
    {
        Admin,
        ProjectManager,
        Submitter,
        Developer
    }
}