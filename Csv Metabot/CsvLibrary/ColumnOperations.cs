using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvLibrary;
using System.Text.RegularExpressions;

namespace CsvLibrary
{
    public class ColumnOperations
    {

        public String Get_Column_Names(String InputFile)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Column_Names();
        }

        // Tested - Delete an entire Line if a particular cell of a given column matches a regular expression pattern
        public int Delete_Column(String InputFile, String ColumnName)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex = cu.Get_Column_Index(ColumnName);
            if (ColumnIndex > -1)
            {
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {
                    String MyLine = cu.Get_Line_Content(entry.Key);
                    entry.Value.RemoveAt(ColumnIndex);
                }
                cu.Save_File_As_CSV(InputFile);
            }
            return ColumnIndex;

        }

        public Boolean Rename_Column(String InputFile, String CurrentColumnName, String NewColumnName)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int idx = cu.Get_Column_Index(CurrentColumnName);
            if (idx > -1)
            {
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {
                    if (entry.Key == 0)
                    {
                        entry.Value[idx] = NewColumnName;
                    }
                    cu.Save_File_As_CSV(InputFile);
                    return true;
                }

            }
            return false;
        }

        // Append a string to the content of a Cell IF a certain corresponding Cell in the same column or a different column matches a Regex (ex: if "Description" contains "Credit" then append a "-" to column "Amount")
        public int Append_If_Column_Matches_Pattern(String InputFile, String ColumnNameToMatch, String RegExPattern, String ColumnNameToModify, String StringToAppend, Boolean AppendAtEnd)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int CntMods = 0;
            int ColumnIndex = cu.Get_Column_Index(ColumnNameToMatch);
            int ColumnIndexToModify = cu.Get_Column_Index(ColumnNameToModify);

            if(ColumnIndex < 0 || ColumnIndexToModify < 0) { return -1; }

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {

                //Console.WriteLine("Debug Column Index | Name: " + ColumnIndex + ":" + ColumnName);
                String MyLine = cu.Get_Line_Content(entry.Key);
                String CellValueToCheck = entry.Value[ColumnIndex];
                var pattern = @RegExPattern;
                var matches = Regex.Matches(CellValueToCheck, pattern);
                if (entry.Key > 0 && matches.Count != 0) // not first line AND match
                {
                    String OriginalValueOfCell = entry.Value[ColumnIndexToModify];
                    String NewValueCell = "";
                    if (OriginalValueOfCell.StartsWith("\""))
                    {
                        NewValueCell = cu.ReplaceFirst(OriginalValueOfCell, "\"", "\"" + StringToAppend);
                    }
                    else
                    {
                        NewValueCell = StringToAppend + OriginalValueOfCell;
                    }
                    entry.Value[ColumnIndexToModify] = NewValueCell;
                    CntMods++;
                }
            }
            cu.Save_File_As_CSV(InputFile);
            return CntMods;
        }

        public int Get_Column_Index(String InputFile, String ColumnName)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Column_Index(ColumnName);
        }

        public int Get_Number_Of_Columns(String InputFile)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Number_Of_Columns();
        }

        public Boolean Enforce_Column_Order(String InputFile, String ColumnOrderTemplate)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            String[] ColNames = ColumnOrderTemplate.Split(',');
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            List<string> AllColumnNameFromTemplate = new List<string>();
            List<int> Order = new List<int>();
            foreach (String s in ColNames)
            {
                AllColumnNameFromTemplate.Add(s.Trim());
                // Console.WriteLine("Debug, adding:" + s.Trim());
            }
            int NumberOfColumnsInParameters = AllColumnNameFromTemplate.Count();
            int NumberOfColumnsInCsv = cu.Get_Number_Of_Columns();

            if (NumberOfColumnsInParameters != NumberOfColumnsInCsv)
            {
                // Number of Columns in parameter passed is different from number of columns in CSV
                return false;
            }

            //Console.WriteLine("sdf:" + NumberOfColumnsInParameters + ":" + NumberOfColumnsInCsv);
            //Console.ReadKey();

            foreach (String Col in AllColumnNameFromTemplate)
            {
                if (cu.Get_Column_Index(Col) < 0) { return false; }
                Order.Add(cu.Get_Column_Index(Col));
            }

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {

                List<String> result1 = Order.Select(i => entry.Value[i]).ToList();
                NewDict.Add(entry.Key, result1);

            }
            cu.dict = NewDict;
            cu.Save_File_As_CSV(InputFile);
            return true;
        }

        // Tested - Switch Columns
        public Boolean Switch_Columns(String InputFile, String ColumnName1, String ColumnName2)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            IDictionary<int, List<String>> NewDict = new Dictionary<int, List<String>>();
            int ColumnIndex1 = cu.Get_Column_Index(ColumnName1);
            int ColumnIndex2 = cu.Get_Column_Index(ColumnName2);

            if (ColumnIndex1 > -1 && ColumnIndex2 > -1)
            {
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {
                    String MyLine = cu.Get_Line_Content(entry.Key);

                    cu.Swap(entry.Value, ColumnIndex1, ColumnIndex2);

                    // entry.Value.RemoveAt(ColumnIndex);
                }
                cu.Save_File_As_CSV(InputFile);
            }
            else
            {
                return false;
            }
            return true;

        }

        // Split Cell Value into other column
        public String Copy_Column_Content_To_Other_Column(String InputFile, String OriginColumn, String RegexPatternGroupToCopy, String TargetColumn)
        {

            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int idxTar = cu.Get_Column_Index(TargetColumn);
            int idxOri = cu.Get_Column_Index(OriginColumn);
            int NumberOfMatches = 0;
            if (idxTar > -1 && idxOri > -1) // If column exists..
            {
                
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {
                    if (entry.Key > 0) // preserving column headers
                    {
                        String CellContent = cu.Get_Cell_Content(OriginColumn, entry.Key);
                        if(CellContent != null)
                        {
                            var pattern = RegexPatternGroupToCopy;
                            var matches = Regex.Matches(CellContent, pattern);
                            if (matches.Count > 0 && matches[0].Groups.Count > 1)
                            {
                                String ValueToCopy = matches[0].Groups[1].Value;
                                cu.Set_Cell_Content(TargetColumn, entry.Key, ValueToCopy);
                                NumberOfMatches++;
                            }

                        }
                        else
                        {
                            return "Cell Content is Null";
                        }

                    }
                }
            }
            else
            {
                return "At Least 1 Column in parameter does not exist.";
            }
            if(NumberOfMatches == 0)
            {
                return "No Match Found or No Regex Group defined in Regular Expression Pattern";
            }
            else
            {
                cu.Save_File_As_CSV(InputFile);
                return "Number of Matching Cells Found and moved: "+ NumberOfMatches;
            }
        }


        // Transforms the content of an entire column (by replacing it with a RegEx MATCH from a regular expression)
        public String Transform_Column_Content(String InputFile, String ColumnName, String RegExPattern)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int colIdx = cu.Get_Column_Index(ColumnName);
            if(colIdx < 0) { return "Column Does Not Exist."; }
            int NbOfMatches = 0;

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnName, entry.Key);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    var matches = Regex.Matches(MyContent, pattern);
                    if (matches.Count > 0 && matches[0].Groups.Count > 1)
                    {
                        NewValue = matches[0].Groups[1].Value;
                        NbOfMatches++;
                    }
                    else
                    {
                    }
                    cu.Save_Cell_Value_No_Save(ColumnName, entry.Key, NewValue);
                }
            }
            cu.Save_File_As_CSV(InputFile);
            if (NbOfMatches == 0)
            {
                return "No Match Found or No Regex Group defined in Regular Expression Pattern";
            }
            else
            {
                return "Number of Matching Cells Found and transformed: " + NbOfMatches;
            }
        }

        // Replace String in Column Content
        public Boolean Replace_String_In_Column_Content(String InputFile, String ColumnName, String StringToReplace, String Replacement)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int idx = cu.Get_Column_Index(ColumnName);
            if(idx < 0) { return false; }
            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {

                    if (idx > -1)
                    {
                        String MyContent = cu.Get_Cell_Content(ColumnName, entry.Key);
                        String NewValue = MyContent.Replace(StringToReplace, Replacement);
                        cu.Save_Cell_Value_No_Save(ColumnName, entry.Key, NewValue);
                    }

                }
            }
            cu.Save_File_As_CSV(InputFile);
            return true;
        }

        // Remove String in Column Content
        public Boolean Remove_String_In_Column_Content(String InputFile, String ColumnName, String StringToRemove)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int idx = cu.Get_Column_Index(ColumnName);
            if (idx < 0) { return false; }
            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {

                    if (idx > -1)
                    {
                        String MyContent = cu.Get_Cell_Content(ColumnName, entry.Key);
                        String NewValue = MyContent.Replace(StringToRemove, "");
                        cu.Save_Cell_Value_No_Save(ColumnName, entry.Key, NewValue);
                    }

                }
            }
            cu.Save_File_As_CSV(InputFile);
            return true;
        }

        public int Add_Column_After(String InputFile, String ColumnName, String ColumnNameToAdd, String EmptyCellFiller)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int Index = cu.Get_Column_Index(ColumnName);
            if (Index > -1) // if column found
            {
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {
                    // do something with entry.Value or entry.Key
                    String NewValueToInsert = ColumnNameToAdd;
                    if (entry.Key != 0)
                    {
                        NewValueToInsert = EmptyCellFiller;
                    }
                    entry.Value.Insert(Index + 1, NewValueToInsert);

                }
                cu.Save_File_As_CSV(InputFile);

            }
            return Index;
        }

        public int Add_Column_Before(String InputFile, String ColumnName, String ColumnNameToAdd, String EmptyCellFiller)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int Index = cu.Get_Column_Index(ColumnName);
            if (Index > -1) // if column found
            {
                foreach (KeyValuePair<int, List<String>> entry in cu.dict)
                {
                    // do something with entry.Value or entry.Key
                    String NewValueToInsert = ColumnNameToAdd;
                    if (entry.Key != 0)
                    {
                        NewValueToInsert = EmptyCellFiller;
                    }
                    entry.Value.Insert(Index, NewValueToInsert);

                }
                cu.Save_File_As_CSV(InputFile);

            }
            return Index;
        }
    }
}
