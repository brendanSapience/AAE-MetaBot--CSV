using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CsvLibrary
{
    public class CellOperations
    {

        
        // Change the value of multiple cells in a column based on Range (Ex: set "USD" in column currency from line 3 to 15)
        public Boolean Save_Cell_Value_On_Range(String InputFile, String ColumnName, int LineNumberStart, int LineNumberEnd, String NewValue)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int ColumnIndex = cu.Get_Column_Index(ColumnName);
            if (ColumnIndex > -1 && LineNumberStart < LineNumberEnd)
            {
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {

                    if (entry.Key >= LineNumberStart && entry.Key <= LineNumberEnd)
                    {
                        entry.Value[ColumnIndex] = NewValue;
                    }
                }
                cu.Save_File_As_CSV(InputFile);
            }
            else { return false; }
            return true;
        }

        // Split Cell Value into other column (ex: on line 3 take the content of Column "Currency" ("Currency (USD)") and copy the regex group 1 to column "Clean_Currency") using Regex expression:" Currency \(([A-Z]{3})\)"
        public String Copy_Cell_Content_To_Other_Column(String InputFile, String OriginColumn, int LineNumber, String RegexPatternGroupToCopy, String TargetColumn)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            String CellContent = cu.Get_Cell_Content(OriginColumn, LineNumber);
            if (CellContent != null)
            {
                var pattern = RegexPatternGroupToCopy;
                var matches = Regex.Matches(CellContent, pattern);
                if (matches.Count > 0 && matches[0].Groups.Count > 1)
                {
                    int idx = cu.Get_Column_Index(TargetColumn);
                    if (idx > -1) // If column exists..
                    {
                        String ValueToCopy = matches[0].Groups[1].Value;
                        // Console.WriteLine("DEBUG: Value Extracted: " + ValueToCopy);
                        cu.Set_Cell_Content(TargetColumn, LineNumber, ValueToCopy);
                        cu.Save_File_As_CSV(InputFile);
                    }
                    else
                    {
                        return "";
                    }

                }
                else
                {
                    return "No Match Found or No Regex Group defined in Regular Expression Pattern";
                }
                return "";
            }
            else
            {
                return "Cell Content is Null";
            }
        }

        // Transforms the content of a specific Cell (by replacing it with a RegEx MATCH from a regular expression)
        public String Transform_Cell_Content(String InputFile, String ColumnName, int LineNumber, String RegExPattern)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            String MyContent = cu.Get_Cell_Content(ColumnName, LineNumber);
            if (MyContent != null)
            {
                String NewValue = MyContent;
                var pattern = @RegExPattern;
                var matches = Regex.Matches(MyContent, pattern);
                if (matches.Count > 0 && matches[0].Groups.Count > 1)
                {
                    NewValue = matches[0].Groups[1].Value;
                }
                else
                {
                    return "No Match Found or No Regex Group defined in Regular Expression Pattern";
                }
                cu.Set_Cell_Content(ColumnName, LineNumber, NewValue);
                cu.Save_File_As_CSV(InputFile);
                return NewValue;
            }
            return null;

        }

        // Change the content of a Cell
        public String Set_Cell_Content(String InputFile, String ColumnName, int LineNumber, String NewValue)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int NbOfLines = cu.Get_Number_Of_Lines();

            if (LineNumber > NbOfLines)
            {
                return "Line Number exceeds the number of total lines in the input file.";
            }

            int ColIdx = cu.Get_Column_Index(ColumnName);
            if (ColIdx < 0)
            {
                return "Column Does Not Exist.";
            }

            cu.Set_Cell_Content(ColumnName, LineNumber, NewValue);
            cu.Save_File_As_CSV(InputFile);
            return "";
        }


        public String Get_Cell_Content(String InputFile, String ColumnName, int LineNumber)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Cell_Content(ColumnName, LineNumber);
        }
        
        // Returns Y if a cell in Column A and line 4 matches a regex, otherwise returns N
        public String Does_Cell_Content_Match_Regex(String InputFile, String ColumnName, int LineNumber, String RegExPattern)
        {
            String IsThereMatch = "N";
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            String MyContent = cu.Get_Cell_Content(ColumnName, LineNumber);
            //Console.Write("Debug:" + MyContent);
            if (MyContent != null)
            {

                var pattern = @RegExPattern;
                var matches = Regex.Matches(MyContent, pattern);
                if (matches.Count > 0)
                {
                    IsThereMatch = "Y";
                }


            }
            return IsThereMatch;
        }
    }
}
