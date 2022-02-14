using System;
using System.Collections.Generic;
using System.Linq;

namespace Wartsila1
{
    public class Utilities
    {
        /***
         * Method for processing Enthalpy files. Returns a list of gases.
         ***/
        public static List<Gas> ProcessEnthalpyFile(string fileContent, List<Gas> listOfGases)
        {
            var records = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            // TODO: Do some checks on format and content
            foreach (var record in records)
            {
                var gasKeyValuePair = record.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                bool gasAlreadyExistsInList = listOfGases.Any(item => String.Equals(item.Name, gasKeyValuePair[0]));
                // Remove the first row containing the column headers
                if (gasKeyValuePair[0] == "Component")
                {
                    continue;
                }
                // Extracting the data
                else
                {
                    // Find correct enthalpy value for gas
                    if (!gasAlreadyExistsInList)
                    {
                        listOfGases.Add(new Gas() { Name=gasKeyValuePair[0], Enthalpy=float.Parse(gasKeyValuePair[1]) });
                    }
                    else
                    {
                        var gas = listOfGases.Find(item => item.Name == gasKeyValuePair[0]);
                        gas.Enthalpy = float.Parse(gasKeyValuePair[1]);
                    }
                }
            }

            return listOfGases;
        }

        /***
         * Method for processing the chromatograph files. It returns a Tuple
         * containing a list of gases and the injection date as a string.
         ***/
        public static (List<Gas>, string) ProcessAnalysisFile(string fileContent, List<Gas> listOfGases)
        {
            // Before starting processing the file, we will set the Fraction
            // value of all gases to zero. Otherwise in case the chromatograph
            // file only contains a subset of a previous file, we might have
            // some stray values left.
            foreach (var gas in listOfGases)
            {
                gas.Fraction = 0;
            }
            var records = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            // TODO: Do some checks on format and content
            var injectionDate = "";
            bool gasData = false;
            foreach (var record in records)
            {
                var analysisArray = record.Split(new string[] { "\t" }, StringSplitOptions.None);
                if (analysisArray[0] == "Inject Date")
                {
                    injectionDate = analysisArray[1];
                    continue;
                }
                // End of file header
                else if (analysisArray[0] == "--")
                {
                    gasData = true;
                    continue;
                }
                // End of data block
                else if (analysisArray[0] == "==")
                {
                    gasData = false;
                    break;
                }
                // Analysis data processing
                if (gasData)
                {
                    if (analysisArray[1] != String.Empty)
                    {
                        // Here we check if a gas already exists in the list.
                        // If it does not exist we add it as a new gas, but if
                        // it already exist we just update its Fraction value.
                        bool gasAlreadyExistsInList = listOfGases.Any(item => String.Equals(item.Name, analysisArray[1]));
                        if (!gasAlreadyExistsInList)
                        {
                            listOfGases.Add(new Gas() { Name=analysisArray[1], Fraction=float.Parse(analysisArray[4]) });
                        }
                        else
                        {
                            var gas = listOfGases.Find(item => String.Equals(item.Name, analysisArray[1]));
                            gas.Fraction = float.Parse(analysisArray[4]);
                        }
                    }
                }
            }

            return (listOfGases, injectionDate);
        }
    }
}
