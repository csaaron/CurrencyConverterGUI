using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrencyConverterGUI
{
    public partial class Form1 : Form
    {

        private List<string> currencyList;

        public Form1()
        {
            InitializeComponent();
            // load list boxes
            currencyList = GetCurrencies();
            ToComboBox.Items.AddRange(currencyList.ToArray());
            ToComboBox.SelectedIndex = 0;
            FromComboBox.Items.AddRange(currencyList.ToArray());
            FromComboBox.SelectedIndex = 0;

            // place text in amount box
            AmountTextBox.Text = "0.00";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Attempts to convert currency in "From" box to the currency in "To" box. 
        /// 
        /// If unable to convert due unparsable text in AmountTextBox, will display a message box.
        /// </summary>
        private void ConvertButton_Click(object sender, EventArgs e)
        {
            string fromCurrency = FromComboBox.SelectedItem.ToString();
            string ToCurrency = ToComboBox.SelectedItem.ToString();
            decimal amount = 0.00m;
            if (!decimal.TryParse(AmountTextBox.Text, out amount))
            {
                string message = "Please enter a numeric amount into the Amount box";
                MessageBox.Show(message);
                AmountTextBox.Text = "0.00";
                return;
            }
            ResultTextBox.Text = ConvertCurrency(fromCurrency, ToCurrency, amount).ToString();
        }

        /// <summary>
        /// Attempts to update the database with that of https://www.exchangerate-api.com
        /// Blocks controls until completed
        /// 
        /// If unable to update database, displays message
        /// 
        /// </summary>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            Task<bool> result = UpdateDatabaseFromAPI();
            DisableAndEnableFormElements();

            string message = "Please wait while exchange rates \n" +
                "are updated from https://www.exchangerate-api.com";
            MessageBox.Show(message);

            //block and wait for result
            result.Wait();
            if (result.GetAwaiter().GetResult())
            {
                
                message = "rates updated successfully";
            }
            else
            {
                
                message = "update failed";
            }
            DisableAndEnableFormElements();
            MessageBox.Show(message);

        }

        /// <summary>
        /// If form elements are enabled, disable them, else enable them.
        /// </summary>
        private void DisableAndEnableFormElements()
        {
            UpdateButton.Enabled = !UpdateButton.Enabled;
            ConvertButton.Enabled = !ConvertButton.Enabled;
            FromComboBox.Enabled = !FromComboBox.Enabled;
            ToComboBox.Enabled = !ToComboBox.Enabled;
        }

        /* -------------------------- Program Logic -----------------------------------------*/
        
        /// <summary>
        /// Converts given amount from one currency to another. 
        /// </summary>
        /// <param name="currencyFrom">3 Character Currency Code from which to convert currency</param>
        /// <param name="currencyTo">3 Character Currency Code to convert currency into</param>
        /// <param name="amount">Amount of currency to be converted</param>
        /// <returns></returns>
        public decimal ConvertCurrency(string currencyFrom, string currencyTo, decimal amount)
        {
            return (decimal)LookupRelativeRateByCurrency(currencyFrom, currencyTo) * amount;
        }

        /// <summary>
        /// Looks up the exchange rate per 1 USD for the given currency. 
        /// </summary>
        /// <param name="currency">A three letter currency code</param>
        /// <returns>The exchange rate per $1 USD of currency</returns>
        private double LookupRateByCurrency(string currency)
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

        private List<String> GetCurrencies()
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
        private double LookupRelativeRateByCurrency(string fromCurrency, string toCurrency)
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

        /// <summary>
        /// Updates the CurrencyCode in the database with the given new rate.
        /// If the CurrencyCode is not in the database, will add it to the table. 
        /// </summary>
        /// <param name="newCurrencyCode">3 digit currency code to be updated</param>
        /// <param name="newRate">The updated exchange rate</param>
        /// <returns></returns>
        private bool UpdateDatabase(string newCurrencyCode, double newRate)
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

        /// <summary>
        /// Attempts to update the database from the api provided by www.exchangerate-api.com
        /// </summary>
        /// <returns>a non blocking task that contains the value true if the database was update, else false </returns>
        public async Task<bool> UpdateDatabaseFromAPI()
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

        /// <summary>
        /// Retrieves and deserializes the json object provided by the www.exchangerate-api.com api
        /// </summary>
        /// <returns></returns>
        private async Task<CurrencyAPIResult> GetAPIRates()
        {
            // make HTTP GET request
            string apiAddress = "https://v6.exchangerate-api.com/v6/e05a5085046ee147e911003a/latest/USD";
            HttpClient client = new HttpClient();
            Task<string> apiStringTask = client.GetStringAsync(apiAddress);

            CurrencyAPIResult result = JsonConvert.DeserializeObject<CurrencyAPIResult>(await apiStringTask);

            return result;

        }

        /// <summary>
        /// Object for Deserializing the JSON object returned by the www.exchangerate-api.com api
        /// </summary>
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
