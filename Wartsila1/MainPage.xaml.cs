using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Wartsila1
{
    public sealed partial class MainPage : Page
    {
        List<Gas> listOfGases = new List<Gas>();
        public string injectionDate = "";
        // Adding some flags
        public bool validEnthalpy = false;
        public bool validAnalysis = false;
        public bool gasData = false;

        public MainPage()
        {
            this.InitializeComponent();
            EnthalpyTextBox.Text = String.Empty;
            AnalysisTextBox.Text = String.Empty;
            ResultTextBox.Text = String.Empty;
            listOfGases.Clear();
        }

        /***
         * Method for calculating the HHV value. CalculateBtn only activated
         * if both an Enthalpy and an Analysis file have been loaded.
         * Result is written to a TextBox.
         ***/
        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = String.Empty;
            ResultTextBox.Text += "Number of gases in list: " + listOfGases.Count().ToString() + "\n\n";
            var todaysDate = DateTime.Now.ToString("d.M.yyyy hh:mm:ss");
            float totalHHV = 0;
            foreach (var gas in listOfGases)
            { 
                    totalHHV += (gas.Enthalpy*gas.Fraction)/100;
            }
            ResultTextBox.Text += "Report date: " + todaysDate.ToString() + "\n";
            ResultTextBox.Text += "Analysis date: " + injectionDate.ToString() + "\n\n";
            ResultTextBox.Text += "Calculated HHV: " + totalHHV.ToString() + " kJ/mol\n";
        }

        /***
         * Method for opening an Enthalpy file, extracting the data, adding it
         * to the list of gases and displaying some data in a TextBox.
         ***/
        private async void OpenEnthalpyFileBtn_Click(Object sender, RoutedEventArgs e)
        {
            EnthalpyTextBox.Text = String.Empty;
            OpenFileClass instance = new OpenFileClass();
            IStorageFile file = await instance.OpenFileAsync();
            if (file != null) 
            {
                EnthalpyTextBox.Text += "File: " + file.Name + "\n\n";
                string fileContent = await FileIO.ReadTextAsync(file);
                validEnthalpy = true;

                listOfGases = Utilities.ProcessEnthalpyFile(fileContent, listOfGases);

                // Outputting current state of listOfGases to EnthalpyTextBox        
                foreach (var gas in listOfGases)
                {
                    EnthalpyTextBox.Text += gas.Name + "," + gas.Enthalpy.ToString() + "," + gas.Fraction.ToString() + "\n";
                }

                if (validAnalysis)
                {
                    CalculateBtn.IsEnabled = true;
                }
            }
            else
            {
                EnthalpyTextBox.Text = "Operation cancelled";
                validEnthalpy = false;
                CalculateBtn.IsEnabled = false;
            }
            
        }

        /***
         * Method for opening a chromatograph file, extracting the data, adding it
         * to the list of gases and displaying some data in a TextBox.
         ***/
        private async void OpenAnalysisFileBtn_Click(object sender, RoutedEventArgs e)
        {
            AnalysisTextBox.Text = String.Empty;
            OpenFileClass instance = new OpenFileClass();
            IStorageFile file = await instance.OpenFileAsync();
            if (file != null)
            {
                AnalysisTextBox.Text += "File: " + file.Name + "\n\n";
                string fileContent = await FileIO.ReadTextAsync(file);
                validAnalysis = true;

                (listOfGases, injectionDate) = Utilities.ProcessAnalysisFile(fileContent, listOfGases);

                AnalysisTextBox.Text += "Injection date: " + injectionDate + "\n\n";

                // Outputting current state of listOfGases to AnalysisTextBox
                foreach (var gas in listOfGases)
                {
                    AnalysisTextBox.Text += gas.Name + "," + gas.Enthalpy.ToString() + "," + gas.Fraction.ToString() + "\n";
                }

                if (validEnthalpy)
                {
                    CalculateBtn.IsEnabled = true;
                }
            } 
            else
            {
                AnalysisTextBox.Text = "Operation cancelled";
                validAnalysis = false;
                CalculateBtn.IsEnabled = false;
            }
            
        }

        /***
         * Method for populating the Gas selector. Chosing a certain gas will
         * display its enthalpy and fraction values in a TextBlock above the
         * selector. 
         ***/
        private void GasesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string gasName = e.AddedItems[0].ToString();
            var gas = listOfGases.Find(item => String.Equals(item.Name, gasName));
            float hhvValue = (gas.Enthalpy*gas.Fraction)/100;
            GasTextBlock.Text = "Selected gas: " + gas.Name + "\nEnthalpy value: " + gas.Enthalpy.ToString() 
                + "\nFraction: " + gas.Fraction + "%\n\nHHV portion from this gas: " + hhvValue.ToString() + " kJ/mol\n";
        }
    }
}
