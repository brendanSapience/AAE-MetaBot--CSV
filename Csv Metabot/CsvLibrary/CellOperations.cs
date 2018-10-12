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
        // Change the value of multiple cells in a column based on Range
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

        // Split Cell Value into other column
        public Boolean Copy_Cell_Content_To_Other_Column(String InputFile, String OriginColumn, int LineNumber, String RegexPatternGroupToCopy, String TargetColumn)
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
                        return false;
                    }

                }
                return true;
            }
            return false;

            
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
                cu.Set_Cell_Content(ColumnName, LineNumber, NewValue);
                cu.Save_File_As_CSV(InputFile);
                return NewValue;
            }
            return null;

        }

        // Change the content of a Cell
        public void Set_Cell_Content(String InputFile, String ColumnName, int LineNumber, String NewValue)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            cu.Set_Cell_Content(ColumnName, LineNumber, NewValue);
            cu.Save_File_As_CSV(InputFile);
        }


        public String Get_Cell_Content(String InputFile, String ColumnName, int LineNumber)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Cell_Content(ColumnName, LineNumber);
        }

    }
}
