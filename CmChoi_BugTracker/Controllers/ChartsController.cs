using CmChoi_BugTracker.ChartViewModels;
using CmChoi_BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CmChoi_BugTracker.Controllers
{
    public class ChartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Charts
        public JsonResult GetHardCodedMorrisBarData()
        {
            var dataSet = new List<MorrisBarChartData>();
            dataSet.Add(new MorrisBarChartData { label = "New/UnAssigned", value = 10 });
            dataSet.Add(new MorrisBarChartData { label = "UnAssigned", value = 3});
            dataSet.Add(new MorrisBarChartData { label = "New/Assigned", value = 30 });
            dataSet.Add(new MorrisBarChartData { label = "Assigned", value = 60 });
            dataSet.Add(new MorrisBarChartData { label = "InProgress", value = 80 });
            dataSet.Add(new MorrisBarChartData { label = "OnHold", value = 2 });
            dataSet.Add(new MorrisBarChartData { label = "Complete", value = 300 });
            dataSet.Add(new MorrisBarChartData { label = "Archived", value = 450 });

            return Json(dataSet);
        }

        public JsonResult GetRealMorrisData()
        {
            var dataSet = new List<MorrisBarChartData>();

            foreach (var ticketStatus in db.TicketStatuses.ToList())
            {
                var value = db.TicketStatuses.Find(ticketStatus.Id).Tickets.Count();
                dataSet.Add(new MorrisBarChartData { label = ticketStatus.Name, value = value });
            }
            return Json(dataSet);
        }

        public JsonResult GetRealFusionData()
        {
            var dataSet = new List<FusionBarChartData>();
            foreach (var ticketStatus in db.TicketStatuses.ToList())
            {
                var value = db.TicketStatuses.Find(ticketStatus.Id).Tickets.Count();
                dataSet.Add(new FusionBarChartData { label = ticketStatus.Name, value = value.ToString() });
            }

            return Json(dataSet);
        }

        public JsonResult GetRealFusionDataByType()
        {
            var dataSet = new List<FusionBarChartData>();
            foreach (var ticketTypes in db.TicketTypes.ToList())
            {
                var value = db.TicketTypes.Find(ticketTypes.Id).Tickets.Count();
                dataSet.Add(new FusionBarChartData { label = ticketTypes.Name, value = value.ToString() });
            }

            return Json(dataSet);
        }

        public JsonResult GetRealFusionDataByPriority()
        {
            var dataSet = new List<FusionBarChartData>();
            foreach (var ticketPriorities in db.TicketPriorities.ToList())
            {
                var value = db.TicketPriorities.Find(ticketPriorities.Id).Tickets.Count();
                dataSet.Add(new FusionBarChartData { label = ticketPriorities.Name, value = value.ToString() });
            }

            return Json(dataSet);
        }
    }
}