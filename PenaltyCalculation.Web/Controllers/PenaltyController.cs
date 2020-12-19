using PenaltyCalculation.DataAccess;
using PenaltyCalculation.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PenaltyCalculation.Web.Controllers
{
    public class PenaltyController : Controller
    {
        public RequestClient rClient;

        CultureInfo culture = CultureInfo.GetCultureInfo("en-US");

        public PenaltyController()
        {
            string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");

            rClient = new RequestClient(connectionString); // RequestClient object initializer
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Countries = PopulateCountries();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SendValues(FormCollection formCollection)
        {
            DateTime checkoutDate = Convert.ToDateTime(formCollection["CheckOutDate"]);
            DateTime returnedDate = Convert.ToDateTime(formCollection["ReturnedDate"]); 

            TempData["CheckoutDate"] = formCollection["CheckOutDate"];  // keep for after postback
            TempData["ReturnedDate"] = formCollection["ReturnedDate"];  // keep for after postback
            TempData["SelectedCountry"] = formCollection["CountryId"];  // keep for after postback

            if (checkoutDate > returnedDate) // Validation
            {
                TempData["ResultMessage"] = "Tarihleri kontrol edip. Tekrar deneyiniz.";
                return RedirectToAction("Index", "Penalty");
            }

            string countryId = formCollection["CountryId"].ToString();

            TempData["ResultMessage"] = CalculatePenaltyResult(checkoutDate, returnedDate, countryId);

            return RedirectToAction("Index", "Penalty");
        }

        public string CalculatePenaltyResult(DateTime checkoutDate, DateTime returnedDate, string countryId)
        {
            var publicHolidays = GetPublicHolidays(countryId);
            var weekends = GetWeekends(countryId);
            DataRow country = GetCountryById(countryId);

            bool weekend = false;
            int counter = 0;
            string message = string.Empty;
            DateTime tempCheckOutDate = checkoutDate;

            while (tempCheckOutDate <= returnedDate)
            {
                foreach (Weekends w in weekends)
                {
                    if (w.Name.ToLower(culture) == tempCheckOutDate.DayOfWeek.ToString().ToLower(culture))
                    {
                        weekend = true;
                        counter++;
                    }
                }
                foreach (PublicHolidays item in publicHolidays)
                {
                    if (weekend == false)
                    {
                        if (item.ValueDate == tempCheckOutDate.Date)
                            counter++;
                    }
                }
                tempCheckOutDate = tempCheckOutDate.AddDays(1);
            }
            if (((returnedDate - checkoutDate).TotalDays + 1 - counter) > 10)
            {
                message += "Süreyi aştınız\n";
                message += string.Format("Toplam ceza: {0}{1}", country["PENALTY_AMOUNT"], country["CURRENCY_SYMBOL"]);
            }
            else
                message += "Zamanında dönüş yaptınız. Teşekkürler.\n";
            return message;
        }

        // get all countries
        private List<CountryViewObject> PopulateCountries()
        {
            var countryList = new List<CountryViewObject>();
            var countries = rClient.GetAllCountries();

            foreach (DataRow dr in countries.Tables[0].Rows)
            {
                countryList.Add(new CountryViewObject // for dropdown list only two column (COUNTRY_ID, NAME)  
                {
                    CountryId = Convert.ToInt32(dr["COUNTRY_ID"]),
                    CountryName = dr["NAME"].ToString()
                });
            }
            return countryList;
        }

        // get country by Id
        private DataRow GetCountryById(string countryId)
        {
            DataSet country = rClient.GetCountryById(countryId);
            if (country != null)
                return country.Tables[0].Rows[0];
            return null;
        }

        // get public holidays by country
        private List<PublicHolidays> GetPublicHolidays(string countryId)
        {
            var publicHolidayList = new List<PublicHolidays>();

            var countryHolidays = rClient.GetPublicHolidaysByCountry(countryId);

            foreach (DataRow dr in countryHolidays.Tables[0].Rows)
            {
                publicHolidayList.Add(new PublicHolidays
                {
                    HolidayId = Convert.ToInt32(dr["HOLIDAY_ID"]),
                    CountryId = Convert.ToInt32(dr["COUNTRY_ID"]),
                    ValueDate = Convert.ToDateTime(dr["VALUE_DATE"])
                });
            }
            return publicHolidayList;
        }

        // get weekends by country
        private List<Weekends> GetWeekends(string countryId)
        {
            var weekendList = new List<Weekends>();

            var weekends = rClient.GetWeekendsByCountry(countryId);

            foreach (DataRow dr in weekends.Tables[0].Rows)
            {
                weekendList.Add(new Weekends
                {
                    WeekendId = Convert.ToInt32(dr["WEEKEND_ID"]),
                    CountryId = Convert.ToInt32(dr["COUNTRY_ID"]),
                    Name = dr["NAME"].ToString(),
                    DayOfWeek = Convert.ToInt32(dr["DAY_OF_WEEK"])
                });
            }
            return weekendList;
        }


    }
}