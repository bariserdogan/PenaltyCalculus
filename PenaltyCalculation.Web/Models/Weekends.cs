using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PenaltyCalculation.Web.Models
{
    public class Weekends
    {
        public int WeekendId { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public int DayOfWeek { get; set; }
    }
}