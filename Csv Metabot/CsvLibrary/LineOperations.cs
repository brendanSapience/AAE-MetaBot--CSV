using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CsvLibrary
{
    public class LineOperations
    {

        public int Get_Number_Of_Lines(String InputFile)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);

            return cu.Get_Number_Of_Lines();
        }

        public String Get_Line_Content(String InputFile, int LineNumber)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Line_Content(LineNumber);
        }


        // Delete an entire Line if a particular cell of a given column matches a regular expression pattern
        public int Delete_Line_If_Cell_Matches_Pattern(String InputFile, String ColumnName, String RegExPattern)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int CntLinesDeleted = 0;
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = cu.Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                String MyLine = cu.Get_Line_Content(entry.Key);
                String CellValueToCheck = entry.Value[ColumnIndex];

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
            cu.dict = NewDict;
            cu.Save_File_As_CSV(InputFile);
            return CntLinesDeleted;
        }


        // Kepp only Lines that match a regular expression
        public int Keep_Line_If_Cell_Matches_Pattern(String InputFile, String ColumnName, String RegExPattern)
        {
            int CntLinesKept = 0;
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = cu.Get_Column_Index(ColumnName);

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                String MyLine = cu.Get_Line_Content(entry.Key);
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
            cu.dict = NewDict;
            cu.Save_File_As_CSV(InputFile);
            return CntLinesKept;
        }
    }
}
