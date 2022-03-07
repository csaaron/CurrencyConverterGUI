using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrencyConverterGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Application.Run(form);

            
        }

        public static decimal ConvertCurrency(string currencyFrom, string currencyTo, decimal amount)
        {
            // currencyFromRate/currencyToRate * amount = converted amount
            return (decimal)LookupRelativeRateByCurrency(currencyFrom, currencyTo) * amount;
        }

        /// <summary>
        /// Looks up the exchange rate per 1 USD for the given currency. 
        /// </summary>
        /// <param name="currency">A three letter currency code</param>
        /// <returns>The exchange rate per $1 USD of currency</returns>
        private static double LookupRateByCurrency(string currency)
        {
            double rate = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["ExchangeRatesDB"].ToString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // create transaction
                string queryString = "SELECT ExchangeRate FROM ExchangeRates WHERE CurrencyCode = @currency;";
                SqlCommand command = connection.CreateCommand();
                command.CommandText = queryString;
                command.Parameters.AddWithValue("@currency", currency);

                // open connection
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Double.TryParse(reader["ExchangeRate"].ToString(), out rate);
                    }
                }
            }


            return rate;
        }

        public static List<String> GetCurrencies()
        {
            List<string> currencyList = new List<string>();

            string connectionString = ConfigurationManager.ConnectionStrings["ExchangeRatesDB"].ToString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // create transaction
                string queryString = "SELECT CurrencyCode FROM ExchangeRates;";
                SqlCommand command = connection.CreateCommand();
                command.CommandText = queryString;
               
                // open connection
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        currencyList.Add(reader["CurrencyCode"].ToString());
                    }
                }
            }

            return currencyList;

        }

        /// <summary>
        /// Looks up the relative exchange rate per 1 unit of toCurrency for the given currency. 
        /// </summary>
        /// <param name="currency">A three letter currency code</param>
        /// <returns>The exchange rate per 1 unit of toCurrency</returns>
        private static double LookupRelativeRateByCurrency(string fromCurrency, string toCurrency)
        {
            double rate = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["ExchangeRatesDB"].ToString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // create transaction

                SqlCommand command = connection.CreateCommand();

                // Pull both currency queries in as one and let server do the calculation
                string queryString = "SELECT ToRate.ExchangeRate / FromRate.ExchangeRate AS RelativeRate FROM ExchangeRates FromRate, ExchangeRates ToRate WHERE FromRate.CurrencyCode = @fromCurrency AND ToRate.CurrencyCode = @toCurrency;";
                command.CommandText = queryString;

                // add with value should sanatize parameters 
                command.Parameters.AddWithValue("@fromCurrency", fromCurrency);
                command.Parameters.AddWithValue("@toCurrency", toCurrency);

                // open connection
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Double.TryParse(reader["RelativeRate"].ToString(), out rate);
                    }
                }
            }


            return rate;
        }

        private static bool UpdateDatabase(string newCurrencyCode, double newRate)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["ExchangeRatesDB"].ToString();
            //string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\clamr\source\repos\CurrencyConverter\CurrencyConverter\CurrencyDB.mdf; Integrated Security = True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction();
                command.Connection = connection;
                command.Transaction = transaction;
                
                
                    

                // Pull both currency queries in as one and let server do the calculation
                string updateString = "IF EXISTS (SELECT 1 FROM ExchangeRates where CurrencyCode = @newCurrencyCode) \n" +
                                        "BEGIN \n" +
                                        "  UPDATE ExchangeRates \n" +
                                        "  SET ExchangeRate = @newRate \n" +
                                        "  WHERE CurrencyCode = @newCurrencyCode; \n" +
                                        "END\n" +
                                        "ELSE\n" +
                                        "BEGIN\n" +
                                        "  INSERT INTO ExchangeRates(CurrencyCode, ExchangeRate) \n" +
                                        "  VALUES(@newCurrencyCode, @newRate); \n" +
                                        "END";

                try
                {

                    command.CommandText = updateString;

                    // add with value should sanatize parameters 
                    command.Parameters.AddWithValue("@newCurrencyCode", newCurrencyCode);
                    command.Parameters.AddWithValue("@newRate", newRate);

                    
                    int result = 0;

                    result = command.ExecuteNonQuery();
                    command.Transaction.Commit();

                    return result > 0;

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
                
            }
        }

        public static async Task<bool> UpdateDatabaseFromAPI()
        {
            // get rates from API
            CurrencyAPIResult updatedData = await GetAPIRates();

            // update each currency in database
            foreach (string currencyCode in updatedData.conversion_rates.Keys)
            {
                if (!UpdateDatabase(currencyCode, updatedData.conversion_rates[currencyCode]))
                    return false;
            }

            return true;
        }

        private static async Task<CurrencyAPIResult> GetAPIRates()
        {
            // make HTTP GET request
            string apiAddress = "https://v6.exchangerate-api.com/v6/e05a5085046ee147e911003a/latest/USD";
            HttpClient client = new HttpClient();
            Task<string> apiStringTask = client.GetStringAsync(apiAddress);

            CurrencyAPIResult result = JsonConvert.DeserializeObject<CurrencyAPIResult>(await apiStringTask);

            return result;

        }

        [DataContract]
        private class CurrencyAPIResult
        {
            [DataMember]
            public string result { get; private set; }
            [DataMember]
            public string documentation { get; private set; }
            [DataMember]
            public string terms_of_use { get; private set; }
            [DataMember]
            public string time_last_update_unix { get; private set; }
            [DataMember]
            public string time_last_update_utc { get; private set; }
            [DataMember]
            public string time_next_update_unix { get; private set; }
            [DataMember]
            public string time_next_update_utc { get; private set; }
            [DataMember]
            public string base_code { get; private set; }
            [DataMember]
            public Dictionary<string, double> conversion_rates { get; private set; }
        }



    }

}

