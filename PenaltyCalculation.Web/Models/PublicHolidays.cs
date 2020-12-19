using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PenaltyCalculation.Web.Models
{
    public class PublicHolidays
    {
        public int HolidayId { get; set; }
        public int CountryId { get; set; }
        public DateTime ValueDate { get; set; }
    }
}