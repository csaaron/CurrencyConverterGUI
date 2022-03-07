using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            currencyList = Program.GetCurrencies();
            ToComboBox.Items.AddRange(currencyList.ToArray());
            ToComboBox.SelectedIndex = 0;
            FromComboBox.Items.AddRange(currencyList.ToArray());
            FromComboBox.SelectedIndex = 0;

            AmountTextBox.Text = "0.00";
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

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
            ResultTextBox.Text = Program.ConvertCurrency(fromCurrency, ToCurrency, amount).ToString();
        }

        
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            Task<bool> result = Program.UpdateDatabaseFromAPI();
            DisableAndEnableFormElements();

            string message = "Please wait while exchange rates \n" +
                "are updated from https://www.exchangerate-api.com";
            MessageBox.Show(message);

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
    }
}
