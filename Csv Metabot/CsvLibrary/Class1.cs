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
        IDictionary<int, List<String>> dict = new Dictionary<int, List<String>>();

        public CsvUtils()
        {
            // This needs to stay empty
        }


        // Set the CSV file up to be processed and available as a C# Dictionary
        private void SetFile(String PathToCsvFile)
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

        // Tested - Delete an entire Line if a particular cell of a given column matches a regular expression pattern
        public int Delete_Column(String InputFile, String ColumnName)
        {
            SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = Get_Column_Index(ColumnName);
            if(ColumnIndex > -1)
            {
                foreach (KeyValuePair<int, List<String>> entry in this.dict)
                {
                    String MyLine = Get_Line_Content(entry.Key);
                    entry.Value.RemoveAt(ColumnIndex);
                }
                Save_File_As_CSV(InputFile);
            }
            return ColumnIndex;

        }

        public int Get_Number_Of_Columns(String InputFile)
        {
            SetFile(InputFile);
            return Get_Number_Of_Columns();
        }

        public int Get_Number_Of_Lines(String InputFile)
        {
            SetFile(InputFile);
            return Get_Number_Of_Lines();
        }

        public String Get_Column_Names(String InputFile)
        {
            SetFile(InputFile);
            return Get_Column_Names();
        }
        
            public int Get_Column_Index(String InputFile, String ColumnName)
        {
            SetFile(InputFile);
            return Get_Column_Index(ColumnName);
        }

        public String Get_Line_Content(String InputFile, int LineNumber)
        {
            SetFile(InputFile);
            return Get_Line_Content(LineNumber);
        }

        public int Add_Column_Before(String InputFile, String ColumnName, String ColumnNameToAdd, String EmptyCellFiller)
        {
            SetFile(InputFile);
            int Index = Get_Column_Index(ColumnName);
            if (Index > -1) // if column found
            {
                foreach (KeyValuePair<int, List<String>> entry in this.dict)
                {
                    // do something with entry.Value or entry.Key
                    String NewValueToInsert = ColumnNameToAdd;
                    if (entry.Key != 0)
                    {
                        NewValueToInsert = EmptyCellFiller;
                    }
                    entry.Value.Insert(Index, NewValueToInsert);

                }
                Save_File_As_CSV(InputFile);
                
            }
            return Index;
        }

        public int Add_Column_After(String InputFile, String ColumnName, String ColumnNameToAdd, String EmptyCellFiller)
        {
            SetFile(InputFile);
            int Index = Get_Column_Index(ColumnName);
            if (Index > -1) // if column found
            {
                foreach (KeyValuePair<int, List<String>> entry in this.dict)
                {
                    // do something with entry.Value or entry.Key
                    String NewValueToInsert = ColumnNameToAdd;
                    if (entry.Key != 0)
                    {
                        NewValueToInsert = EmptyCellFiller;
                    }
                    entry.Value.Insert(Index+1, NewValueToInsert);

                }
                Save_File_As_CSV(InputFile);

            }
            return Index;
        }

        public String Get_Cell_Content(String InputFile, String ColumnName, int LineNumber)
        {
            SetFile(InputFile);
            return Get_Cell_Content(ColumnName,LineNumber);
        }

        // Returns the content of a specific Cell
        private String Get_Cell_Content(String ColumnName, int LineNumber)
        {
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ColumnName);
            List<String> myListOfValues = dict[LineNumber];
            return myListOfValues[ColumnNumber];
        }

        // Transforms the content of a specific Cell (by replacing it with a RegEx MATCH from a regular expression)
        public String Transform_Cell_Content(String InputFile, String ColumnName, int LineNumber, String RegExPattern)
        {
            String MyContent = Get_Cell_Content(ColumnName, LineNumber);
            String NewValue = MyContent;
            var pattern = @RegExPattern;
            var matches = Regex.Matches(MyContent, pattern);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                NewValue = matches[0].Groups[1].Value;
            }
            Save_Cell_Value(InputFile, ColumnName,LineNumber,NewValue);
            return NewValue;
        }

        // Change the content of a Cell
        public void Set_Cell_Content(String InputFile, String ColumnName, int LineNumber, String NewValue)
        {
            Save_Cell_Value(InputFile, ColumnName, LineNumber, NewValue);
        }

        // Transforms the content of an entire column (by replacing it with a RegEx MATCH from a regular expression)
        public void Transform_Column_Content(String InputFile, String ColumnName, String RegExPattern)
        {
            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                if(entry.Key > 0) // dont process the column header line
                {
                    String MyContent = Get_Cell_Content(ColumnName, entry.Key);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    var matches = Regex.Matches(MyContent, pattern);
                    if (matches.Count > 0 && matches[0].Groups.Count > 1)
                    {
                        NewValue = matches[0].Groups[1].Value;
                    }
                    else
                    {
                    }
                    Save_Cell_Value_No_Save(ColumnName, entry.Key, NewValue);
                }
            }
            Save_File_As_CSV(InputFile);
        }

        // Delete an entire Line if a particular cell of a given column matches a regular expression pattern
        public int Delete_Line_If_Cell_Matches_Pattern(String InputFile, String ColumnName, String RegExPattern)
        {
            SetFile(InputFile);
            int CntLinesDeleted = 0;
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                    String MyLine = Get_Line_Content(entry.Key);
                    String CellValueToCheck = entry.Value[ColumnIndex];

                    var pattern = @RegExPattern;
                    var matches = Regex.Matches(CellValueToCheck, pattern);
                    if(entry.Key == 0) // if First Line, then keep it regardless of match or no match
                    {
                        NewDict.Add(entry);
                    }
                    else
                    {
                        if (matches.Count == 0)
                        {
                            NewDict.Add(entry);
                        }
                        else
                        {
                        //Console.WriteLine("Match Found, Removing Line.");
                        CntLinesDeleted++;
                        }
                    //Console.ReadKey();
                }
            }
            this.dict = NewDict;
            Save_File_As_CSV(InputFile);
            return CntLinesDeleted;
        }


        // Kepp only Lines that match a regular expression
        public int Keep_Line_If_Cell_Matches_Pattern(String InputFile, String ColumnName, String RegExPattern)
        {
            int CntLinesKept = 0;
            SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                String MyLine = Get_Line_Content(entry.Key);
                //Console.WriteLine("Debug Line: " + MyLine);
                String CellValueToCheck = entry.Value[ColumnIndex];
                //Console.WriteLine("Checking Value: " + CellValueToCheck + " against Regex: " + @RegExPattern);
                var pattern = @RegExPattern;
                var matches = Regex.Matches(CellValueToCheck, pattern);
                if (entry.Key == 0) // if First Line, then keep it regardless of match or no match
                {
                    NewDict.Add(entry);
                }
                else
                {
                    if (matches.Count == 0)
                    {

                    }
                    else
                    {
                        NewDict.Add(entry);
                        CntLinesKept++;
                        // Console.WriteLine("Match Found, Keeping Line.");
                    }
                    //Console.ReadKey();
                }
            }
            this.dict = NewDict;
            Save_File_As_CSV(InputFile);
            return CntLinesKept;
        }

        // Append a string to the content of a Cell IF a certain corresponding Cell in the same column or a different column matches a Regex (ex: if "Description" contains "Credit" then append a "-" to column "Amount")
        public int Append_If_Column_Matches_Pattern(String InputFile, String ColumnNameToMatch, String RegExPattern, String ColumnNameToModify, String StringToAppend, Boolean AppendAtEnd)
        {
            SetFile(InputFile);
            int CntMods = 0;
            int ColumnIndex = Get_Column_Index(ColumnNameToMatch);
            int ColumnIndexToModify = Get_Column_Index(ColumnNameToModify);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {

                //Console.WriteLine("Debug Column Index | Name: " + ColumnIndex + ":" + ColumnName);
                String MyLine = Get_Line_Content(entry.Key);
                String CellValueToCheck = entry.Value[ColumnIndex];
                var pattern = @RegExPattern;
                var matches = Regex.Matches(CellValueToCheck, pattern);
                if (entry.Key > 0 && matches.Count != 0) // not first line AND match
                {
                    String OriginalValueOfCell = entry.Value[ColumnIndexToModify];
                    String NewValueCell = "";
                    if (OriginalValueOfCell.StartsWith("\""))
                    {
                        NewValueCell = ReplaceFirst(OriginalValueOfCell,"\"", "\""+StringToAppend);
                    }
                    else
                    {
                        NewValueCell = StringToAppend + OriginalValueOfCell;
                    }
                    entry.Value[ColumnIndexToModify] = NewValueCell;
                    CntMods++;
                }
            }
            Save_File_As_CSV(InputFile);
            return CntMods;
        }

        // Split Cell Value into other column
        public void Copy_Cell_Content_To_Other_Column(String InputFile, String OriginColumn, int LineNumber, String RegexPatternGroupToCopy, String TargetColumn)
        {
            String CellContent = Get_Cell_Content(OriginColumn, LineNumber);
            var pattern = RegexPatternGroupToCopy;
            var matches = Regex.Matches(CellContent, pattern);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                String ValueToCopy = matches[0].Groups[1].Value;
               // Console.WriteLine("DEBUG: Value Extracted: " + ValueToCopy);
                Set_Cell_Content(InputFile,TargetColumn, LineNumber, ValueToCopy);
                Save_File_As_CSV(InputFile);
            }
        }

        // Change the value of multiple cells in a column based on Range
        public void Save_Cell_Value_On_Range(String InputFile, String ColumnName, String LineNumberStartS,String LineNumberEndS, String NewValue)
        {
            int LineNumberStart = Int32.Parse(LineNumberStartS);
            int LineNumberEnd = Int32.Parse(LineNumberEndS);
            SetFile(InputFile);
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {

                if (entry.Key >= LineNumberStart && entry.Key <= LineNumberEnd)
                {
                    entry.Value[ColumnIndex] = NewValue;
                }
            }
            Save_File_As_CSV(InputFile);
        }

        // Internal Function
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

        // Internal Function
        public void Save_Cell_Value(String InputFile, String ColumnName, int LineNumber, String NewValue)
        {
            Save_Cell_Value_No_Save(ColumnName, LineNumber, NewValue);
            Save_File_As_CSV(InputFile);
        }

        // Internal Function
        private String ReplaceFirst(String MyString, String PatternToReplace, String StrToReplaceWith)
        {
            var regex = new Regex(Regex.Escape(PatternToReplace));

            var newText = regex.Replace(MyString, StrToReplaceWith, 1);
            return newText;
        }

        // Internal Function
        private int Get_Column_Index(String ColumnName)
        {
            String ProcessedColumnName = ColumnName.Trim('"');
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ProcessedColumnName);
            return ColumnNumber;
        }


        // Returns the total number of columns within the CSV File
        private int Get_Number_Of_Columns()
        {
            return this.dict[0].Count;
        }

        // Returns the total number of lines within the CSV File (including the header line)
        private int Get_Number_Of_Lines()
        {
            return this.dict.Count;
        }

        // Returns all the column names in CSV format
        private String Get_Column_Names()
        {
            var result = String.Join(",", dict[0].ToArray());
            return result;
        }

        // Returns the content of a line in CSV format
        private String Get_Line_Content(int LineNumber)
        {
            var result = String.Join(",", dict[LineNumber].ToArray());
            return result;
        }

        // Internal function - Save dictionary back to CSV File
        private void Save_File_As_CSV(String InputFile)
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
