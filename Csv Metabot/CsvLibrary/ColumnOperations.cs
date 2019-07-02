﻿using System;
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

        // Get all column names (as CSV)
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

        // Get the Column Number based on the Column Name
        public int Get_Column_Index(String InputFile, String ColumnName)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Column_Index(ColumnName);
        }

        // get the total number of columns
        public int Get_Number_Of_Columns(String InputFile)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            return cu.Get_Number_Of_Columns();
        }

        // Rearranges columns to enforce a particular order
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

        // Transforms the content of an entire column (by replacing it with a RegEx MATCH from a regular expression)
        
        public String Split_Column_Content_based_on_groups(String InputFile, String ColumnNameToRead, String RegExPattern, String InsertAfterColumnName, String ColumnNameStub)
        {

            //Check the number of matches for each Row and retrieve the Max number(N)
            //Create N columns after “Insert After Column” named “Col_1”, “Col_2”, etc.
            //For each row, split it into the proper number of elements


            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int colIdx = cu.Get_Column_Index(ColumnNameToRead);
            if (colIdx < 0) { return "Column Does Not Exist."; }
            int colIdxInsert = cu.Get_Column_Index(InsertAfterColumnName);
            if (colIdxInsert < 0) { return "Column Does Not Exist:" + InsertAfterColumnName; }
            int MaxNumberOfGroups = 0;


            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnNameToRead, entry.Key);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    //Console.WriteLine("Debug: Content:" + MyContent);
                    var matches = Regex.Matches(MyContent, pattern, RegexOptions.Multiline);
                    int NumOfGroups = matches[0].Groups.Count;

                    if (NumOfGroups > MaxNumberOfGroups) { MaxNumberOfGroups = NumOfGroups; }
                }
            }
            //Console.WriteLine("Debug: Max Number of Matches" + MaxNumberOfGroups);

            for (int i = 0; i < MaxNumberOfGroups; i++)
            {
                int tempIdx = MaxNumberOfGroups - i;
                //icolIdx = MaxNumberOfMatches - i;
                Add_Column_After(cu, InsertAfterColumnName, ColumnNameStub + tempIdx, "");
            }

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnNameToRead, entry.Key);

                    var matches = Regex.Matches(MyContent, RegExPattern);
                    int NumOfGroups = matches[0].Groups.Count;
                    int idxGrp = 0;
                    foreach (Group group in matches[0].Groups)
                    {
                        idxGrp++;
                        if (idxGrp > 1)
                        {
                            //Console.WriteLine("Group Number:" + idxGrp + ":" + group.Value);
                            String CurrentCellValue = group.Value;
                            //Console.WriteLine("Debug: " + CurrentCellValue);
                            if (CurrentCellValue.Contains(','))
                            {
                               // Console.WriteLine("Quote detected");
                                entry.Value[colIdxInsert + idxGrp] = "\"" + CurrentCellValue + "\"";
                            }
                            else
                            {
                                entry.Value[colIdxInsert + idxGrp] = group.Value;
                            }
                        }

                    }
                }
            }


            cu.Save_File_As_CSV(InputFile);
            return "";

        }
        

        // Transforms the content of an entire column (by replacing it with a RegEx MATCH from a regular expression)
        public String Split_Column_Content_based_on_matches(String InputFile, String ColumnNameToRead, String RegExPattern, String InsertAfterColumnName, String ColumnNameStub)
        {

            //Check the number of matches for each Row and retrieve the Max number(N)
            //Create N columns after “Insert After Column” named “Col_1”, “Col_2”, etc.
            //For each row, split it into the proper number of elements

            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int colIdx = cu.Get_Column_Index(ColumnNameToRead);
            if (colIdx < 0) { return "Column Does Not Exist: "+ ColumnNameToRead; }
            int colIdxInsert = cu.Get_Column_Index(InsertAfterColumnName);
            if (colIdxInsert < 0) { return "Column Does Not Exist:"+ InsertAfterColumnName; }
            int MaxNumberOfMatches = 0;


            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnNameToRead, entry.Key);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    //Console.WriteLine("Debug: Content:" + MyContent);
                    var matches = Regex.Matches(MyContent, pattern);
                    int NumOfMatches = matches.Count;

                    if (NumOfMatches > MaxNumberOfMatches) { MaxNumberOfMatches = NumOfMatches; }
                }
            }
            //Console.WriteLine("Debug: Max Number of Matches: " + MaxNumberOfMatches);

            for (int i = 0; i < MaxNumberOfMatches; i++)
            {
                int tempIdx = MaxNumberOfMatches - i;
                //icolIdx = MaxNumberOfMatches - i;
                Add_Column_After(cu, InsertAfterColumnName, ColumnNameStub + tempIdx, "");
            }

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnNameToRead, entry.Key);

                    var matches = Regex.Matches(MyContent, RegExPattern);
                    //int NumOfMatches = matches.Count;
                    int idxGrp = 0;
                    foreach (Match match in matches)
                    {
                        idxGrp++;
                        if (idxGrp > 0)
                        {
                            //Console.WriteLine("Group Number:" + idxGrp + ":" + group.Value);
                            String CurrentCellValue = match.Groups[1].Value;
                            //Console.WriteLine("Debug: " + CurrentCellValue);
                            if (CurrentCellValue.Contains(','))
                            {
                                // Console.WriteLine("Quote detected");
                                entry.Value[colIdxInsert + idxGrp] = "\"" + CurrentCellValue + "\"";
                            }
                            else
                            {
                                entry.Value[colIdxInsert + idxGrp] = match.Groups[1].Value;
                            }
                        }

                    }
                }
            }


            cu.Save_File_As_CSV(InputFile);
            return "";

        }

        /*
        // Transforms the content of an entire column (by replacing it with a RegEx MATCH from a regular expression)
        public String Split_Column_Content_based_on_groups(String InputFile, String ColumnNameToRead, String RegExPattern, String InsertAfterColumnName, String ColumnNameStub)
        {

            //Check the number of matches for each Row and retrieve the Max number(N)
            //Create N columns after “Insert After Column” named “Col_1”, “Col_2”, etc.
            //For each row, split it into the proper number of elements

            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            int colIdx = cu.Get_Column_Index(ColumnNameToRead);
            if (colIdx < 0) { return "Column Does Not Exist."; }
            int colIdxInsert = cu.Get_Column_Index(InsertAfterColumnName);
            if (colIdxInsert < 0) { return "Column Does Not Exist:" + InsertAfterColumnName; }
            int MaxNumberOfMatches = 0;

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnNameToRead, entry.Key);
                    String NewValue = MyContent;
                    var pattern = @RegExPattern;
                    var matches = Regex.Matches(MyContent, pattern);
                    int NumOfMatches = matches.Count;
                    if (NumOfMatches > MaxNumberOfMatches) { MaxNumberOfMatches = NumOfMatches; }
                }
            }
            //Console.WriteLine("Debug: Max Number of Matches" + MaxNumberOfMatches);
            
            for(int i = 0;i< MaxNumberOfMatches; i++)
            {
                int tempIdx = MaxNumberOfMatches - i;
                //icolIdx = MaxNumberOfMatches - i;
                Add_Column_After(cu, InsertAfterColumnName, ColumnNameStub + tempIdx, "");
            }

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {
                if (entry.Key > 0) // dont process the column header line
                {
                    String MyContent = cu.Get_Cell_Content(ColumnNameToRead, entry.Key);
                    
                    var matches = Regex.Matches(MyContent, RegExPattern);
                    int NumOfMatches = matches.Count;
                    int idxMatch = 0;
                    foreach (Match match in matches)
                    {
                        idxMatch++;
                        // Console.WriteLine("Match Number:" + idxMatch + ":" + match.Value);
                        String CurrentCellValue = match.Value;
                       // Console.WriteLine("Debug: " + CurrentCellValue);
                       if (CurrentCellValue.Contains(','))
                        {
                            //Console.WriteLine("Quote detected");
                            entry.Value[colIdxInsert + idxMatch] = "\"" + CurrentCellValue + "\"";
                        }
                        else
                        {
                            entry.Value[colIdxInsert + idxMatch] = match.Value;
                        }
                        
                    }
                }
            }


            cu.Save_File_As_CSV(InputFile);
            return "";

        }
        */

        // Takes a row and duplicates it while extracting part of a given column (Regex Group Matches)
        public String Split_Column_Content_into_rows_based_on_matches(String InputFile, String ColumnNameToRead, String RegExPattern, int RowNumber)
        {

            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);
            // index below is line number (without counting header)

            int colIdx = cu.Get_Column_Index(ColumnNameToRead);
            if (colIdx < 0) { return "Column Does Not Exist."; }
            //int MaxNumberOfMatches = 0;

            List<String> MyRowContent = cu.dict[RowNumber];
            String MyContent = MyRowContent[colIdx];

            var pattern = @RegExPattern;
            var matches = Regex.Matches(MyContent, pattern);
            int NumOfMatches = matches.Count;

            List<List<String>> AllNewRows = new List<List<String>>();

            foreach (Match match in matches)
            {
                List<String> RowCopy = new List<String>(MyRowContent);
                String MatchValue = match.Value;
                RowCopy[colIdx] = MatchValue;
                AllNewRows.Add(RowCopy);
            }

            Add_Rows_After2(cu, RowNumber, AllNewRows);
            cu.Save_File_As_CSV(InputFile);
            return "";
        }

        // Add a list of Rows after a given row
        private void Add_Rows_After2(CsvUtils cu, int RowNumber, List<List<String>> rowsToAdd)
        {
            // We first convert the Dictionary to a List (easier to insert into)
            List<List<String>> myList = DictionaryToList(cu.dict);
            myList.InsertRange(RowNumber+1, rowsToAdd);
            // We convert the List back to a Dictionary
            cu.dict = ListToDictionary(myList);
        }
        

        // Delete a single row
        public void Delete_Row(String InputFile, int RowNumber)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);

            List<List<String>> myList = DictionaryToList(cu.dict);
            
            myList.RemoveAt(RowNumber);

            cu.dict = ListToDictionary(myList);

            cu.Save_File_As_CSV(InputFile);
           
        }

        // Add a new Column after an existing one
        private void Add_Column_After(CsvUtils cu, String ColumnName, String ColumnNameToAdd, String EmptyCellFiller)
        {
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
            }
        }

        // Add a new Column after an existing one
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

        // Add a new Column before an existing one
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

        private List<List<String>> DictionaryToList(IDictionary<int,List<String>> myDict)
        {
            List<List<String>> TheList = new List<List<String>>();

            foreach(KeyValuePair<int, List<String>> entry in myDict){
                TheList.Add(entry.Value);
            }

            return TheList;
        }

        private IDictionary<int, List<String>>  ListToDictionary(List<List<String>> myList)
        {
           
            IDictionary<int, List<String>> TheDict = new Dictionary<int, List<String>>();

            int idx = 0;
            foreach ( List<String> row in myList)
            {
                TheDict[idx]=row;
                idx++;
            }

            return TheDict;
        }

    }


}
