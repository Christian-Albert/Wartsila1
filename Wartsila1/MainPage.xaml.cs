using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Wartsila1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text = String.Empty;
            ResultTextBox.Text += "Length of listOfGases: " + listOfGases.Count().ToString() + "\n\n";
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

        // Using FileOpenPicker example from Microsoft docs for selecting file
        private async void OpenEnthalpyFileBtn_Click(Object sender, RoutedEventArgs e)
        {
            EnthalpyTextBox.Text = String.Empty;
            OpenFileClass instance = new OpenFileClass();
            IStorageFile file = await instance.OpenFileAsync();
            EnthalpyTextBox.Text += "File: " + file.Name + "\n\n";
            string text = await FileIO.ReadTextAsync(file);

            // Splitting the text at every new line - this will go into separate utility class
            var records = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var record in records)
            {
                var gasKeyValuePair = record.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                bool gasAlreadyExistsInList = listOfGases.Any(item => String.Equals(item.Name, gasKeyValuePair[0]));
                if (gasKeyValuePair[0] == "Component")
                {
                    continue;
                } else
                {
                    // Find correct enthalpy value for gas
                    if (!gasAlreadyExistsInList)
                    {
                        listOfGases.Add(new Gas() { Name=gasKeyValuePair[0], Enthalpy=float.Parse(gasKeyValuePair[1]) });
                    } else
                    {
                        var gas = listOfGases.Find(item => item.Name == gasKeyValuePair[0]);
                        gas.Enthalpy = float.Parse(gasKeyValuePair[1]);
                    }
                }
            }
            foreach (var gas in listOfGases)
            {
                EnthalpyTextBox.Text += gas.Name + "," + gas.Enthalpy.ToString() + "," + gas.Fraction.ToString() + "\n";
            }

            validEnthalpy = true;
            if (validAnalysis)
            {
                CalculateBtn.IsEnabled = true;
            }
        }

        private async void OpenAnalysisFileBtn_Click(object sender, RoutedEventArgs e)
        {
            AnalysisTextBox.Text = String.Empty;
            OpenFileClass instance = new OpenFileClass();
            IStorageFile file = await instance.OpenFileAsync();
            AnalysisTextBox.Text += "File: " + file.Name + "\n\n";
            string text = await FileIO.ReadTextAsync(file);

            // Splitting the text at every new line - this will go into separate utility class
            var records = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var record in records)
            {
                var analysisArray = record.Split(new string[] { "\t" }, StringSplitOptions.None);
                if (analysisArray[0] == "Inject Date")
                {
                    AnalysisTextBox.Text += "Injection date: " + analysisArray[1] + "\n\n";
                    injectionDate = analysisArray[1];
                } else if (analysisArray[0] == "--")
                {
                    gasData = true;
                    continue;
                } else if (analysisArray[0] == "==")
                {
                    gasData = false;
                    break;
                }
                if (gasData)
                {
                    if (analysisArray[1] != String.Empty)
                    {
                        bool gasAlreadyExistsInList = listOfGases.Any(item => String.Equals(item.Name, analysisArray[1]));
                        if (!gasAlreadyExistsInList)
                        {
                            listOfGases.Add(new Gas() { Name=analysisArray[1], Fraction=float.Parse(analysisArray[4]) });
                        } else
                        {
                            var gas = listOfGases.Find(item => String.Equals(item.Name, analysisArray[1]));
                            gas.Fraction = float.Parse(analysisArray[4]);
                        }
                    }
                }
            }
            foreach (var gas in listOfGases)
            {
                AnalysisTextBox.Text += gas.Name + "," + gas.Enthalpy.ToString() + "," + gas.Fraction.ToString() + "\n";
            }
            validAnalysis = true;
            if (validEnthalpy)
            {
                CalculateBtn.IsEnabled = true;
            }
        }

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
