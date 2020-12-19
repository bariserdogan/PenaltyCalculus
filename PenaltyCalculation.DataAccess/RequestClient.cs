using System;
using System.Data;

namespace PenaltyCalculation.DataAccess
{
    /*
        Helper Class for database operations.
        The class gets connectionstring parameter in its constructor method
    */

    public class RequestClient
    {
        public string _connectionString = "";

        public RequestClient(string connectionString) 
        {
            _connectionString = connectionString; 
        }

        public DataSet GetAllCountries()
        {
            DatabaseConnector db = new DatabaseConnector(_connectionString);
            var allCountries = new DataSet();
            try
            {
                db.OpenConnection();
                DataSet ds = db.GetDataSet(@"SELECT COUNTRY_ID, NAME FROM PC_COUNTRIES (NOLOCK)");
                if (ds != null && ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                        allCountries = ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                if (db.IsConnectionOpen)
                    db.CloseConnection();
            }
            return allCountries;
        }

        public DataSet GetCountryById(string countryId)
        {
            DatabaseConnector db = new DatabaseConnector(_connectionString);
            var country = new DataSet();
            try
            {
                db.OpenConnection();
                string query = @"
                                 SELECT 
                                   COUNTRY_ID, NAME, CODE, CURRENCY_CODE, CURRENCY_SYMBOL, PENALTY_AMOUNT
                                 FROM 
                                   PC_COUNTRIES (NOLOCK)
                                 WHERE COUNTRY_ID = @COUNTRY_ID
                                ";
                DataSet ds = db.GetDataSet(query, db.CreateParameter("@COUNTRY_ID", countryId));
                if (ds != null && ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                        country = ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                if (db.IsConnectionOpen)
                    db.CloseConnection();
            }
            return country;
        }

        public DataSet GetPublicHolidaysByCountry(string countryId)
        {
            DatabaseConnector db = new DatabaseConnector(_connectionString);
            var publicHolidays = new DataSet();
            try
            {
                db.OpenConnection();
                DataSet ds = db.GetDataSet(@"SELECT HOLIDAY_ID, COUNTRY_ID, VALUE_DATE FROM PC_PUBLIC_HOLIDAY_CALENDAR (NOLOCK) WHERE COUNTRY_ID = @COUNTRY_ID",
                                            db.CreateParameter("@COUNTRY_ID", countryId));

                if (ds != null && ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                        publicHolidays = ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                if (db.IsConnectionOpen)
                    db.CloseConnection();
            }
            return publicHolidays;
        }

        public DataSet GetWeekendsByCountry(string countryId)
        {
            DatabaseConnector db = new DatabaseConnector(_connectionString);
            var weekends = new DataSet();
            try
            {
                db.OpenConnection();
                DataSet ds = db.GetDataSet(@"SELECT WEEKEND_ID, COUNTRY_ID, NAME, DAY_OF_WEEK FROM PC_WEEKENDS (NOLOCK) WHERE COUNTRY_ID = @COUNTRY_ID",
                                            db.CreateParameter("@COUNTRY_ID", countryId));

                if (ds != null && ds.Tables.Count > 0)
                    if (ds.Tables[0].Rows.Count > 0)
                        weekends = ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                if (db.IsConnectionOpen)
                    db.CloseConnection();
            }
            return weekends;
        }
    }
}
