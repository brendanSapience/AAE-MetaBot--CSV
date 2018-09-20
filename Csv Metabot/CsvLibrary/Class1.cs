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

        public CsvUtils()
        {
         
        }

        private void SetFile(String PathToCsvFile)
        {
            this.dict = new Dictionary<int, List<String>>();
            this.FilePath = PathToCsvFile;
            reader = new StreamReader(this.FilePath);
            int index = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                List<string> ValuesArray = new List<string>();

                // Original:            (?:(?<=^)|(?<=,))(?:(?!$)\s)*"?((?<=").*?(?=")|((?!,).)*)"?(?:(?!$)\s)*(?=$|,)
                String RegExPattern = "(?:(?<=^)|(?<=,))(?:(?!$)\\s)*\"?((?<=\").*?(?=\")|((?!,).)*)\"?(?:(?!$)\\s)*(?=$|,)";
               // Regex csvSplit = new Regex("(?:\\s*(?:\"([^\"]*)\"|([^,]+)|([,,]))\\s*,?)+?", RegexOptions.Compiled);
                Regex csvSplit = new Regex(RegExPattern, RegexOptions.Compiled);
                string curr = null;
                foreach (Match match in csvSplit.Matches(line))
                {
                    curr = match.Value;

                    
                    

                    if (0 == curr.Length)
                    {
                        ValuesArray.Add(" ");
                        //Console.WriteLine("DEBUG: Value Detected: EMPTY");
                    }
                    else
                    {
                        
                        String currFiltered = curr.TrimEnd(',').Trim(' ');
                        ValuesArray.Add(currFiltered);
                        //Console.WriteLine("DEBUG: Value Detected:"+ currFiltered+":");
                    }
                    //Console.ReadKey();

                }

                dict.Add(index, ValuesArray);
                index++;
            }
            reader.Close();
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

            var result = String.Join(", ", dict[LineNumber].ToArray());
            return result;
        }

        // Add Columns
        public void Add_Column_Between(String InputFile, String ColumnName1, String ColumnName2, String ColumnNameToAdd,String EmptyDataFiller)
        {
            List<String> AllColumns = this.dict[0];
            int IndexBefore = AllColumns.IndexOf(ColumnName1);
            int IndexAfter = AllColumns.IndexOf(ColumnName2);

            Add_Column_Between(InputFile, IndexBefore, IndexAfter, ColumnNameToAdd, EmptyDataFiller);



        }

        public void Add_Column_Between(String InputFile, int ColumnNumber1, int ColumnNumber2, String ColumnNameToAdd, String EmptyDataFiller)
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
            Save_File_As_CSV(InputFile);
        }

        public String Get_Cell_Content(String ColumnName, int LineNumber)
        {
            List<String> myListOfColumns = dict[0];
            int ColumnNumber = myListOfColumns.IndexOf(ColumnName);
            List<String> myListOfValues = dict[LineNumber];
            return myListOfValues[ColumnNumber];

        }

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

        public void Set_Cell_Content(String InputFile, String ColumnName, int LineNumber, String NewValue)
        {
            
            Save_Cell_Value(InputFile, ColumnName, LineNumber, NewValue);
     

        }

        public void Transform_Column_Content(String InputFile, String ColumnName, String RegExPattern)
        {
            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                if(entry.Key > 0) // dont process the column header line
                {
                    String MyContent = Get_Cell_Content(ColumnName, entry.Key);
                    //Console.WriteLine("DEBUG: " + MyContent);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    var matches = Regex.Matches(MyContent, pattern);
                    if (matches.Count > 0 && matches[0].Groups.Count > 1)
                    {
                        NewValue = matches[0].Groups[1].Value;
                        //Console.WriteLine("DEBUG: New Value: "+ NewValue);

                    }
                    else
                    {
                        //Console.WriteLine("DEBUG: No Change!");
                    }
                    Save_Cell_Value_No_Save(ColumnName, entry.Key, NewValue);
                }
                

            }
            Save_File_As_CSV(InputFile);

        }

        public void Delete_Line_If_Cell_Matches_Pattern(String InputFile, String ColumnName, String RegExPattern)
        {
            SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {

                    //Console.WriteLine("Debug Column Index | Name: " + ColumnIndex + ":" + ColumnName);
                    String MyLine = Get_Line_Content(entry.Key);
                    //Console.WriteLine("Debug Line [" +entry.Key+"]:"+ MyLine);
                    String CellValueToCheck = entry.Value[ColumnIndex];
                    //Console.WriteLine("Checking Value: " + CellValueToCheck + " against Regex: " + @RegExPattern);
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
                            //Console.WriteLine("No Match Found, Keeping Line.");
                        }
                        else
                        {
                            //Console.WriteLine("Match Found, Removing Line.");
                        }
                        //Console.ReadKey();
                    }


            }
            this.dict = NewDict;
            Save_File_As_CSV(InputFile);
        }

        public void Keep_Line_If_Cell_Matches_Pattern(String InputFile, String ColumnName, String RegExPattern)
        {
            SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {

                //Console.WriteLine("Debug Column Index | Name: " + ColumnIndex + ":" + ColumnName);
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
                        
                        //Console.WriteLine("No Match Found, Removing Line.");
                    }
                    else
                    {
                        NewDict.Add(entry);
                       // Console.WriteLine("Match Found, Keeping Line.");
                    }
                    //Console.ReadKey();
                }


            }
            this.dict = NewDict;
            Save_File_As_CSV(InputFile);
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

        public void Save_Cell_Value(String InputFile, String ColumnName, int LineNumber, String NewValue)
        {
            Save_Cell_Value_No_Save(ColumnName, LineNumber, NewValue);
            Save_File_As_CSV(InputFile);
        }

        public void Save_Cell_Value_On_Range(String InputFile, String ColumnName, int LineNumberStart,int LineNumberEnd, String NewValue)
        {
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

        private int Get_Column_Index(String ColumnName)
        {
            String ProcessedColumnName = ColumnName.Trim('"');
            List<String> myListOfColumns = dict[0];
            foreach (String el in myListOfColumns)
            {
                //Console.WriteLine("DEBUG:" + el+ ":");

                
            }
            //Console.ReadKey();
            int ColumnNumber = myListOfColumns.IndexOf(ProcessedColumnName);
            return ColumnNumber;
        }
        // Save structure to CSV File
        private void Save_File_As_CSV(String InputFile)
        {
            //before your loop
            var csv = new StringBuilder();

            foreach (KeyValuePair<int, List<String>> entry in this.dict)
            {
                // do something with entry.Value or entry.Key
                
                var newLine = String.Join(",", entry.Value.ToArray());
                //Console.WriteLine("DEBUG:" + newLine);
                //Console.ReadKey();
                csv.AppendLine(newLine);
            }

            String DebugNewFilePath = @"C:\IQBot Input\2_[7618da06-56f2-45f5-8b89-991f5315c3b8]_0057_486301224 June-2018.pdf.csv";
            //File.WriteAllText(DebugNewFilePath, csv.ToString());

            // Uncomment the following line before compiling!
            File.WriteAllText(InputFile, csv.ToString());
            
        }
    }
}
