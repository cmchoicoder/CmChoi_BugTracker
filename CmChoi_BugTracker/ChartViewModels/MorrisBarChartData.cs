using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmChoi_BugTracker.ChartViewModels
{
    public class MorrisBarChartData
    {
        public string label { get; set; }
        public int value { get; set; }
    }

    public class FusionBarChartData
    {
        public string label { get; set; }
        public string value { get; set; }
    }
}