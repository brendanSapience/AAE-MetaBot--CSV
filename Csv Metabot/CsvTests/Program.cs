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
            String FilePath = @"C:\IQBot Input\sample_csv_output.csv";
            
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

            // Test: delete lines where Amount column contains "Page or 2018"
            int NbLinesDeleted = utils.Delete_Line_If_Cell_Matches_Pattern(FilePath,"Description", @"Page.*|2018.*");
            Console.WriteLine("Lines Deleted: " + NbLinesDeleted);

            // Test: add a "-" in front of values in column Amount if column Description contains "Debit"
            int NbCellsMods = utils.Append_If_Column_Matches_Pattern(FilePath, "Description", ".*Debit.*", "Item_Amount", "-", false);
            Console.WriteLine("Cells modified: " + NbCellsMods);

            // Test: delete column named "Not_Real" (no such column)
            int idx = utils.Delete_Column(FilePath, "Not_Real");
            Console.WriteLine("Column Index for Column Not_Real: " + idx);

            // Test: delete column named "Vendor_Name"
            idx = utils.Delete_Column(FilePath, "Vendor_Name");
            Console.WriteLine("Column Index for Column Vendor_Name: " + idx);

            // Test: get the index of a particular column
            idx = utils.Get_Column_Index(FilePath, "Invoice_Total");
            Console.WriteLine("Index for Column Invoice_Total: " + idx);

            // Test: insert a new column Before an existing one
            int IdxColumnBefore = utils.Add_Column_Before(FilePath, "Invoice_Number", "Invoice_Type","Standard");
            Console.WriteLine("Inserting Before column with index: " + IdxColumnBefore);

            // Test: insert a new column After an existing one
            int IdxColumnAfter = utils.Add_Column_After(FilePath, "Item_Amount", "Desc_Type", "<Data>");
            Console.WriteLine("Inserting After column with index: " + IdxColumnAfter);

            int IdxColumnAfter1 = utils.Add_Column_After(FilePath, "Desc_Type", "New_Col_1", "<Data>");
            Console.WriteLine("Inserting After column with index: " + IdxColumnAfter1);

            // Test: get the content of a cell
            String CellContent = utils.Get_Cell_Content(FilePath, "Description",2);
            Console.WriteLine("Cell Content: " + CellContent);

            // Test: Change the content of a cell
            utils.Set_Cell_Content(FilePath, "New_Col_1", 2, "\"Test, hello?!\"");

            // Test: Change the content of a cell to the "Match" of a regular expression applied to it: .+ (.*) Trn.*
            String NewValue = utils.Transform_Cell_Content(FilePath, "New_Col_1", 2, "Test, (.*)");
            Console.WriteLine("New Cell Content: " + NewValue);

            // Test: Change the content of a column to the "Match" of a regular expression applied to it
            utils.Transform_Column_Content(FilePath, "Description", ".+ (Pro.* (?:Credit|Debit))");
            Console.WriteLine("Transforming Column Content.");

            // Test: keep lines where Amount column contains "Page "
            int NbLinesKept = utils.Keep_Line_If_Cell_Matches_Pattern(FilePath, "Description", @"Product.*");
            Console.WriteLine("Lines Kept: " + NbLinesKept);

            // Test: copy the "Match" from a regular expression to another column (for 1 cell)
            utils.Copy_Cell_Content_To_Other_Column(FilePath, "Description", 1, ".+ (Credit|Debit)", "New_Col_1");
            utils.Copy_Column_Content_To_Other_Column(FilePath, "Description", ".+ (Credit|Debit)", "Desc_Type");
            Console.WriteLine("Copying content from columns / cells to other columns.");

            // Test: Change the value of a cell within a range for a given column
            utils.Save_Cell_Value_On_Range(FilePath, "New_Col_1", 3, 6, "\"New Ranged Value, here\"");
            Console.WriteLine("Change Cell Values based on Range");

            // Test: Swap 2 columns
            utils.Switch_Columns(FilePath, "New_Col_1", "Invoice_Number");
            Console.WriteLine("Swapping Columns.");

            // Test: Force the order of columns and rearrange if necessary
            utils.Enforce_Column_Order(FilePath, "Invoice_Type,New_Col_1,Invoice_Date,Invoice_Total,Invoice_Number,Item_Number,Item_Amount,Desc_Type,Description");
            Console.WriteLine("Enforcing Column Order.");

            Console.ReadKey();
        }
    }
}
