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
        private String FilePath = "";
        private StreamReader reader ;
        private List<string> ColumnHeaders = new List<string>();

        IDictionary<int, List<String>> dict = new Dictionary<int, List<String>>();

        public CsvUtils(String PathToCsvFile)
        {
            this.FilePath = PathToCsvFile;
            reader = new StreamReader(this.FilePath);
            int index = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                List<string> ValuesArray = new List<string>();
                ValuesArray.AddRange(values);

                dict.Add(index, ValuesArray);
                index++;
            }
            reader.Close();
            // Check if file is available
            // Check if file is a valid CSV
            // Open File Handle
        }

        public int Get_Number_Of_Columns()
        {
            return this.dict[0].Count;
        }

        public int Get_Number_Of_Lines()
        {
            return this.dict.Count;
        }

        public String Get_Column_Names()
        {

            var result = String.Join(", ", dict[0].ToArray());
            return result;
        }

        public String Get_Line_Content(int LineNumber)
        {

            var result = String.Join(", ", dict[LineNumber-1].ToArray());
            return result;
        }

        // Add Columns
        public void Add_Column_Between(String ColumnName1, String ColumnName2, String ColumnNameToAdd,String EmptyDataFiller)
        {
            List<String> AllColumns = this.dict[0];
            int IndexBefore = AllColumns.IndexOf(ColumnName1);
            int IndexAfter = AllColumns.IndexOf(ColumnName2);

            Add_Column_Between(IndexBefore, IndexAfter, ColumnNameToAdd, EmptyDataFiller);



        }

        public void Add_Column_Between(int ColumnNumber1, int ColumnNumber2, String ColumnNameToAdd, String EmptyDataFiller)
        {
            if (ColumnNumber2 - ColumnNumber1 != 1)
            {
                // ERROR!
            }


            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                // do something with entry.Value or entry.Key
                String NewValueToInsert = ColumnNameToAdd;
                if (entry.Key != 0)
                {
                    NewValueToInsert = EmptyDataFiller;
                }
                entry.Value.Insert(ColumnNumber1 + 1, NewValueToInsert);

            }
            Save_File_As_CSV();
        }

        public String Get_Cell_Content(String ColumnName, int LineNumber)
        {
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ColumnName);
            List<String> myListOfValues = dict[LineNumber];
            return myListOfValues[ColumnNumber];

        }

        public String Transform_Cell_Content(String ColumnName, int LineNumber, String RegExPattern)
        {
            String MyContent = Get_Cell_Content(ColumnName, LineNumber);
            String NewValue = MyContent;
            var pattern = @RegExPattern;
            var matches = Regex.Matches(MyContent, pattern);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                NewValue = matches[0].Groups[1].Value;

            }
            Save_Cell_Value(ColumnName,LineNumber,NewValue);
            return NewValue;

        }

        public void Set_Cell_Content(String ColumnName, int LineNumber, String NewValue)
        {
            
            Save_Cell_Value(ColumnName, LineNumber, NewValue);
     

        }

        public void Transform_Column_Content(String ColumnName, String RegExPattern)
        {
            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                if(entry.Key > 0) // dont process the column header line
                {
                    String MyContent = Get_Cell_Content(ColumnName, entry.Key);
                    Console.WriteLine("DEBUG: " + MyContent);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    var matches = Regex.Matches(MyContent, pattern);
                    if (matches.Count > 0 && matches[0].Groups.Count > 1)
                    {
                        NewValue = matches[0].Groups[1].Value;
                        Console.WriteLine("DEBUG: New Value: "+ NewValue);

                    }
                    else
                    {
                        Console.WriteLine("DEBUG: No Change!");
                    }
                    Save_Cell_Value_No_Save(ColumnName, entry.Key, NewValue);
                }
                

            }
            Save_File_As_CSV();

        }

        public void Delete_Line_If_Cell_Matches_Pattern(String ColumnName, String RegExPattern)
        {
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                String CellValueToCheck = entry.Value[ColumnIndex];
                Console.WriteLine("Checking Value: " + CellValueToCheck +" against Regex: "+@RegExPattern);
                var pattern = @RegExPattern;
                var matches = Regex.Matches(CellValueToCheck, pattern);
                if (matches.Count == 0)
                {
                    NewDict.Add(entry);
                    Console.WriteLine("No Match Found, Keeping Line.");
                }
                else
                {
                    Console.WriteLine("Match Found, Removing Line.");
                }

            }
            this.dict = NewDict;
            Save_File_As_CSV();
        }

        // Split Cell Value into other column
        public void Copy_Cell_Content_To_Other_Column(String OriginColumn, int LineNumber, String RegexPatternGroupToCopy, String TargetColumn)
        {
            String CellContent = Get_Cell_Content(OriginColumn, LineNumber);
            var pattern = RegexPatternGroupToCopy;
            var matches = Regex.Matches(CellContent, pattern);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                String ValueToCopy = matches[0].Groups[1].Value;
                Console.WriteLine("DEBUG: Value Extracted: " + ValueToCopy);
                Set_Cell_Content(TargetColumn, LineNumber, ValueToCopy);
            }

        }

        private void Save_Cell_Value_No_Save(String ColumnName, int LineNumber, String NewValue)
        {
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {

                if (entry.Key == LineNumber)
                {
                    entry.Value[ColumnIndex] = NewValue;
                }

            }
            
        }

        public void Save_Cell_Value(String ColumnName, int LineNumber, String NewValue)
        {
            Save_Cell_Value_No_Save(ColumnName, LineNumber, NewValue);
            Save_File_As_CSV();
        }

        private int Get_Column_Index(String ColumnName)
        {
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ColumnName);
            return ColumnNumber;
        }
        // Save structure to CSV File
        private void Save_File_As_CSV()
        {
            //before your loop
            var csv = new StringBuilder();

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                // do something with entry.Value or entry.Key
                
                var newLine = String.Join(", ", entry.Value.ToArray());
                csv.AppendLine(newLine);
            }

            String NewFilePath = @"C:\Users\brendan.sapience\Google Drive\AutomationAnywhere\IQ Bot Output\Output\Z_Output_Post_Processing\2_[8903ca8a-2995-4334-85bd-6d0c7ca1390b]_Medical Innovations 87167.pdf.csv";
            
            // Uncomment the following line before build!!
            //NewFilePath = this.FilePath;
            
            File.WriteAllText(NewFilePath, csv.ToString());
        }
    }
}
