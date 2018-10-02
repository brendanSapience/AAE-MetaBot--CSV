using CsvLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvTests
{
    class Program
    {
        static void Main(string[] args)
        {
            CsvUtils utils = new CsvUtils();
            String FilePath = @"C:\IQBot Input\[ee257c39-1d40-412f-8c96-e8c03df06e42]_0057_486301224 June-2018.pdf.csv";
            /*
            // test: get Columns and Lines number
            int NbColumns = utils.Get_Number_Of_Columns(FilePath);
            int NbLines = utils.Get_Number_Of_Lines(FilePath);
            Console.WriteLine("Number of [Columns|Lines]: [" + NbColumns + "|" + NbLines+"]");

            // Test: get column names
            String ColNames = utils.Get_Column_Names(FilePath);
            Console.WriteLine("Column Names: [" + ColNames + "]");

            // Test: get the content of a specific line
            String LineContent = utils.Get_Line_Content(FilePath,2);
            Console.WriteLine("Line Content: [" + LineContent + "]");

            // Test: delete lines where Amount column contains "Page "
            int NbLinesDeleted = utils.Delete_Line_If_Cell_Matches_Pattern(FilePath,"Amount", @"Page.*|Page .*|.*, .*");
            Console.WriteLine("Lines Deleted: " + NbLinesDeleted);

            // Test: add a "-" in front of values in column Amount if column Description contains "Debit"
            int NbCellsMods = utils.Append_If_Column_Matches_Pattern(FilePath, "Description", ".*Debit.*", "Amount", "-", false);
            Console.WriteLine("Cells modified: " + NbCellsMods);

            // Test: delete column named "Not_Real" (no such column)
            int idx = utils.Delete_Column(FilePath, "Not_Real");
            Console.WriteLine("Column Index for Column Not_Real: " + idx);

            // Test: delete column named "Instances"
            idx = utils.Delete_Column(FilePath, "Instances");
            Console.WriteLine("Column Index for Column Instances: " + idx);

            // Test: get the index of a particular column
            idx = utils.Get_Column_Index(FilePath, "Statement_Date");
            Console.WriteLine("Index for Column Statement_Date: " + idx);

            // Test: insert a new column Before an existing one
            int IdxColumnBefore = utils.Add_Column_Before(FilePath, "Legal_Entity", "New_Col_1","<Data>");
            Console.WriteLine("Inserting Before column with index: " + IdxColumnBefore);

            // Test: insert a new column After an existing one
            int IdxColumnAfter = utils.Add_Column_After(FilePath, "File Path", "New_Col_2", "<Data>");
            Console.WriteLine("Inserting After column with index: " + IdxColumnAfter);

            // Test: get the content of a cell
            String CellContent = utils.Get_Cell_Content(FilePath, "Description",1);
            Console.WriteLine("Cell Content: " + CellContent);

            // Test: Change the content of a cell to the "Match" of a regular expression applied to it: .+ (.*) Trn.*
            String NewValue = utils.Transform_Cell_Content(FilePath, "Description", 1, ".+ (.*) Trn.*");
            Console.WriteLine("New Cell Content: " + NewValue);

            // Test: Change the content of a cell
            utils.Set_Cell_Content(FilePath, "Description", 2, "\"Test, hello?!\"");

            // Test: Change the content of a column to the "Match" of a regular expression applied to it
            utils.Transform_Column_Content(FilePath, "Legal_Entity", ".*(HESS .*),.*");

            // Test: keep lines where Amount column contains "Page "
            //int NbLinesKept = utils.Keep_Line_If_Cell_Matches_Pattern(FilePath, "Description", @".*Cash Concentration.*");
            //Console.WriteLine("Lines Kept: " + NbLinesKept);

            // Test: copy the "Match" from a regular expression to another column (for 1 cell)
            utils.Copy_Cell_Content_To_Other_Column(FilePath, "Legal_Entity", 2, "HESS (.*) INV.*", "New_col_2");

            // Test: Change the value of a cell within a range for a given column
            utils.Save_Cell_Value_On_Range(FilePath, "New_col_2", 3, 45, "\"New Value, here\"");

            utils.Switch_Columns(FilePath, "New_Col_1", "New_Col_2");
            */

            // Force the order of columns and rearrange if necessary
            utils.Enforce_Column_Order(FilePath, "Description, New_Col_2, New_Col_1, Bank_Name, Statement_Date, Legal_Entity, Account_Number, Beginning_Balance, Deposits_and_Additions, Electronic_Withdrawals, Other_Withdrawals_Fees_and_Charges, Ending_Balance, Checks_Paid, Date, Section_Type, Amount, Result, File Path");

            Console.ReadKey();
        }
    }
}
