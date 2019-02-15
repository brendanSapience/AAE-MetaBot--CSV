
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CsvLibrary
{

    
    public class CsvUtils
    {
        // the Dictonary Object serves as a container for the CSV content
        private String FilePath = "";
        private StreamReader reader ;
        public IDictionary<int, List<String>> dict = new Dictionary<int, List<String>>();

        public CsvUtils(){} // Should remain empty
        
        // Set the CSV file up to be processed and available as a C# Dictionary
        public void SetFile(String PathToCsvFile)
        {
            this.dict = new Dictionary<int, List<String>>();
            this.FilePath = PathToCsvFile;
            reader = new StreamReader(this.FilePath);
            int index = 0;
            while (!reader.EndOfStream)
            {
                // Turn each line of the file into an Array of Strings

                var line = reader.ReadLine();
                List<string> ValuesArray = new List<string>();

                // Some elements of CSVs are complex and have commas within them. Consider the following: 
                //      123,,"my test, here",345,hello  
                // This CSV line has 5 elements including an empty one and one that has a comma in its value
                // To accomodate for complex use cases, we use regular expressions:
                // (?:(?<=^)|(?<=,))(?:(?!$)\s)*"?((?<=").*?(?=")|((?!,).)*)"?(?:(?!$)\s)*(?=$|,)

                String RegExPattern = "(?:(?<=^)|(?<=,))(?:(?!$)\\s)*\"?((?<=\").*?(?=\")|((?!,).)*)\"?(?:(?!$)\\s)*(?=$|,)";
                Regex csvSplit = new Regex(RegExPattern, RegexOptions.Compiled);
                string curr = null;
                // each match is exactly one element of a CSV line
                foreach (Match match in csvSplit.Matches(line))
                {
                    curr = match.Value;

                    if (0 == curr.Length) // if the element is empty, we add an empty string to the array.
                    {
                        ValuesArray.Add(" ");
                    }
                    else
                    {
                        String currFiltered = curr.TrimEnd(',').Trim(' ');
                        ValuesArray.Add(currFiltered);
                    }
                }
                // Finally we add the Array into the dictionary modelling the csv file. the Key to each entry is the line number
                dict.Add(index, ValuesArray);
                index++;
            }
            reader.Close();
        }


        //
        // General Internal Functions only used by other classes. they should NOT take the filename as a parameter
        // 

        public List<String> GetAllValuesFromColumn(String ColumnName)
        {
            int ColIdx = Get_Column_Index(ColumnName);
            if (ColIdx > -1)
            {
                List<String> AllColumnValue = new List<string>();
                foreach (KeyValuePair<int, List<String>> entry in this.dict)
                {
                    AllColumnValue.Add(entry.Value[ColIdx]);
                }
                return AllColumnValue;
            }
            return null;

        }

        public int GetNumberOfDistinctValuesInColumn(String ColumnName)
        {
            List<String> AllValues = GetAllValuesFromColumn(ColumnName);
            List<String> distinct = AllValues.Distinct().ToList();
            int DisctinctCount = distinct.Count();
            return DisctinctCount;
        }

        public String Get_Column_name(int idx)
        {
            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                if (entry.Key == 0)
                {
                    return entry.Value[idx];
                }

            }
            return "";
        }

        // Returns the content of a specific Cell
        public String Get_Cell_Content(String ColumnName, int LineNumber)
        {
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ColumnName);
            if(ColumnNumber > -1)
            {
                List<String> myListOfValues = dict[LineNumber];
                return myListOfValues[ColumnNumber];
            }
            return null;
        }

        // Internal Function
        public void Save_Cell_Value_No_Save(String ColumnName, int LineNumber, String NewValue)
        {
            int ColumnIndex = Get_Column_Index(ColumnName);
            if(ColumnIndex > -1)
            {
                foreach (KeyValuePair<int, List<String>> entry in this.dict)
                {

                    if (entry.Key == LineNumber)
                    {
                        entry.Value[ColumnIndex] = NewValue;
                    }
                }
            }


        }

        public void Save_Cell_Value_No_Save(int ColumnIndex, int LineNumber, String NewValue)
        {
            
            if (ColumnIndex > -1)
            {
                foreach (KeyValuePair<int, List<String>> entry in this.dict)
                {

                    if (entry.Key == LineNumber)
                    {
                        entry.Value[ColumnIndex] = NewValue;
                    }
                }
            }


        }

        public void Swap(List<String> list, int indexA, int indexB)
        {
            String tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public void Set_Cell_Content(String ColumnName, int LineNumber, String NewValue)
        {
            Save_Cell_Value_No_Save(ColumnName, LineNumber, NewValue);
        }


        // Internal Function
        //public void Save_Cell_Value(String InputFile, String ColumnName, int LineNumber, String NewValue)
        //{
        //   Save_Cell_Value_No_Save(ColumnName, LineNumber, NewValue);
        //   Save_File_As_CSV(InputFile);
        //}

        // Internal Function
        public String ReplaceFirst(String MyString, String PatternToReplace, String StrToReplaceWith)
        {
            var regex = new Regex(Regex.Escape(PatternToReplace));

            var newText = regex.Replace(MyString, StrToReplaceWith, 1);
            return newText;
        }


        public int Get_Column_Index(String ColumnName)
        {
            String ProcessedColumnName = ColumnName.Trim('"');
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ProcessedColumnName);
            return ColumnNumber;
        }


        // Returns the total number of columns within the CSV File
        public int Get_Number_Of_Columns()
        {
            return this.dict[0].Count;
        }

        // Returns the total number of lines within the CSV File (including the header line)
        public int Get_Number_Of_Lines()
        {
            return this.dict.Count;
        }

        // Returns all the column names in CSV format
        public String Get_Column_Names()
        {
            var result = String.Join(",", dict[0].ToArray());
            return result;
        }

        // Returns the content of a line in CSV format
        public String Get_Line_Content(int LineNumber)
        {
            var result = String.Join(",", dict[LineNumber].ToArray());
            return result;
        }

        // Internal function - Save dictionary back to CSV File
        public void Save_File_As_CSV(String InputFile)
        {
            //before your loop
            var csv = new StringBuilder();

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                var newLine = String.Join(",", entry.Value.ToArray());
                csv.AppendLine(newLine);
            }

            // Dont forget to change the following value to false before compiling!!!
            Boolean Debug = false;

            if (Debug)
            {
                String DebugNewFilePath = @"C:\IQBot Input\2_[7618da06-56f2-45f5-8b89-991f5315c3b8]_0057_486301224 June-2018.pdf.csv";
                File.WriteAllText(DebugNewFilePath, csv.ToString());
            }
            else
            {
                File.WriteAllText(InputFile, csv.ToString());
            }
        }
    }
}
