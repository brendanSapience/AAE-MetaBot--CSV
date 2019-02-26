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
            ColumnOperations co = new ColumnOperations();
            OutputOperations oo = new OutputOperations();
            LineOperations lo = new LineOperations();
            CellOperations cop = new CellOperations();
            AddressOperations aop = new AddressOperations();



            String Res2 = "CHIPPEWA CAPITAL, LLC PO Box 671206 Detroit, Ml48267-1206 USA";
            String Res3 = "ENDEKA Ceramics Inc. P.O. BOX934597 ATLANTA GA 31193-4597";
            String Res4 = "CDW Direct P.O. Box 75723 __ Chicago, IL 60675-5723";
            String Res5 = "Robert Half Technology 12400 COLLECTIONS CENTER DRIVE CHICAGO IL 60693";
            String Res6 = "333 W. 7th Street Suite 333 Royal Oak, MI 48067";
            String Res7 = "GlobalMaterial Technologies 8468 SolutionCenter Chicago, IL 60677-8004";
            String Res8 = "8443 Solution Center Chicago, IL 60677-8004";
            String Res9 = "8443 Solution Center Chicago, IL 60677 - 8004";

           // String res =  aop.Process_Address_US("ASURION Attention: RITA SWEENEY PO Box 209348 Austin, TX 78720-9348");
            //String res0 = aop.Process_Address_US("818 lexington ave, apt 6A, Brooklyn, NY 11221");
            //String res1 = aop.Process_Address_US("119 Mott St, Apt 4, NY, 10013");
            
            Console.Write(aop.Process_Address_US(Res2) + "\n");
            Console.Write(aop.Process_Address_US(Res3) + "\n");
            Console.Write(aop.Process_Address_US(Res4) + "\n");
            Console.Write(aop.Process_Address_US(Res5) + "\n");
            Console.Write(aop.Process_Address_US(Res6) + "\n");
            Console.Write(aop.Process_Address_US(Res7) + "\n");
            Console.Write(aop.Process_Address_US(Res8) + "\n");
            Console.Write(aop.Process_Address_US(Res9) + "\n");
            Console.ReadKey();
            
            string fileName = "simple_test2.csv";
           // string sourcePath = @"C:\Users\brendan.sapience\Documents\git\AAE-MetaBot--CSV\Documentation\Tests";
           // string targetPath = @"C:\IQBot Input";

            string sourcePath = @"C:\Dev2";
            string targetPath = @"C:\Dev2\Output";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            System.IO.File.Copy(sourceFile, destFile, true);

            String FilePath = targetPath+ @"\"+fileName;

            String OutputStr = co.Split_Column_Content_based_on_matches(FilePath, "Total", @"\d*,*\d+.\d+", "Total","Subtotal_");

            Console.Write(OutputStr);
            Console.ReadKey();

            String Out = cop.Does_Cell_Content_Match_Regex(FilePath, "Billing_Address", 1, @"\b\d{5}\b(?:[-\s]\d{4})?");
            Console.Write(Out+"\n");
            Out = cop.Does_Cell_Content_Match_Regex(FilePath, "Billing_Address", 2, @"\b\d{5}\b(?:[-\s]\d{4})?");
            Console.Write(Out + "\n");
             Out = cop.Does_Cell_Content_Match_Regex(FilePath, "Billing_Address", 3, @"\b\d{5}\b(?:[-\s]\d{4})?");
            Console.Write(Out + "\n");
             Out = cop.Does_Cell_Content_Match_Regex(FilePath, "Billing_Address", 4, @"\b\d{5}\b(?:[-\s]\d{4})?");
            Console.Write(Out + "\n");
             Out = cop.Does_Cell_Content_Match_Regex(FilePath, "Billing_Address", 5, @"\b\d{5}\b(?:[-\s]\d{4})?");
            Console.Write(Out + "\n");
             Out = cop.Does_Cell_Content_Match_Regex(FilePath, "Billing_Address", 6, @"\b\d{5}\b(?:[-\s]\d{4})?");
            Console.Write(Out + "\n");
            Console.ReadKey();

            //String JsonStr = oo.Get_Output_As_Json(FilePath,"SingleValues", "Before,one,test,two,four,New", "Items", "");
            //Console.Write(JsonStr+"\n");
            //Console.ReadKey();

            String Output = "";

            // Test: Change the content of a cell to the "Match" of a regular expression applied to it: .+ (.*) Trn.*
            //Output = cop.Transform_Cell_Content(FilePath, "Before", 2, "(test).+");
            //Console.WriteLine("Transforming Cell Content: " + Output);

            // Test: Change the content of a column to the "Match" of a regular expression applied to it
            //Output = co.Transform_Column_Content(FilePath, "Before", "(test).+");
            //Console.WriteLine("Transforming Column Content: "+ Output);
            //Console.ReadKey();

            // Test: Change the content of a cell
           // String Msg2 = cop.Set_Cell_Content(FilePath, "one", 2, "\"Test, hello?!\"");
           // Console.Write("Debug:"+Msg2 + "\n");
           // Console.ReadKey();

            // Test: copy the "Match" from a regular expression to another column (for 1 cell)
            //String Msg = cop.Copy_Cell_Content_To_Other_Column(FilePath, "Before", 2, ".+", "New");
            //co.Copy_Column_Content_To_Other_Column(FilePath, "Description", ".+ (Credit|Debit)", "Desc_Type");
            //Console.WriteLine("Copying content from columns / cells to other columns: "+ Msg);
            //Console.ReadKey();

            String Msg1 = co.Copy_Column_Content_To_Other_Column(FilePath, "Before", "^(test).+$", "New");
            Console.WriteLine("Copying content from columns / cells to other columns: " + Msg1);
            Console.ReadKey();


            co.Replace_String_In_Column_Content(FilePath, "Invoice_Date", "/", "");
            Console.ReadKey();

            co.Remove_String_In_Column_Content(FilePath, "Invoice_Date", "20");
            Console.ReadKey();

            co.Rename_Column(FilePath, "Description", "Description2");
            Console.ReadKey();

            co.Rename_Column(FilePath, "Description2", "Description");
            Console.ReadKey();

            // test: get Columns and Lines number
            int NbColumns = co.Get_Number_Of_Columns(FilePath);
            int NbLines = lo.Get_Number_Of_Lines(FilePath);
            Console.WriteLine("Number of [Columns|Lines]: [" + NbColumns + "|" + NbLines+"]");

            // Test: get column names
            String ColNames = co.Get_Column_Names(FilePath);
            Console.WriteLine("Column Names: [" + ColNames + "]");

            // Test: get the content of a specific line
            String LineContent = lo.Get_Line_Content(FilePath,2);
            Console.WriteLine("Line Content: [" + LineContent + "]");

            // Test: delete lines where Amount column contains "Page or 2018"
            int NbLinesDeleted = lo.Delete_Line_If_Cell_Matches_Pattern(FilePath,"Description", @"Page.*|2018.*");
            Console.WriteLine("Lines Deleted: " + NbLinesDeleted);

            // Test: add a "-" in front of values in column Amount if column Description contains "Debit"
            int NbCellsMods = co.Append_If_Column_Matches_Pattern(FilePath, "Description", ".*Debit.*", "Item_Amount", "-", false);
            Console.WriteLine("Cells modified: " + NbCellsMods);

            // Test: delete column named "Not_Real" (no such column)
            int idx = co.Delete_Column(FilePath, "Not_Real");
            Console.WriteLine("Column Index for Column Not_Real: " + idx);

            // Test: delete column named "Vendor_Name"
            idx = co.Delete_Column(FilePath, "Vendor_Name");
            Console.WriteLine("Column Index for Column Vendor_Name: " + idx);

            // Test: get the index of a particular column
            idx = co.Get_Column_Index(FilePath, "Invoice_Total");
            Console.WriteLine("Index for Column Invoice_Total: " + idx);

            // Test: insert a new column Before an existing one
            int IdxColumnBefore = co.Add_Column_Before(FilePath, "Invoice_Number", "Invoice_Type","Standard");
            Console.WriteLine("Inserting Before column with index: " + IdxColumnBefore);

            // Test: insert a new column After an existing one
            int IdxColumnAfter = co.Add_Column_After(FilePath, "Item_Amount", "Desc_Type", "<Data>");
            Console.WriteLine("Inserting After column with index: " + IdxColumnAfter);

            int IdxColumnAfter1 = co.Add_Column_After(FilePath, "Desc_Type", "New_Col_1", "<Data>");
            Console.WriteLine("Inserting After column with index: " + IdxColumnAfter1);

            // Test: get the content of a cell
            String CellContent = cop.Get_Cell_Content(FilePath, "Description",2);
            Console.WriteLine("Cell Content: " + CellContent);

            // Test: Change the content of a cell
            cop.Set_Cell_Content(FilePath, "New_Col_1", 2, "\"Test, hello?!\"");

            // Test: Change the content of a cell to the "Match" of a regular expression applied to it: .+ (.*) Trn.*
            String NewValue = cop.Transform_Cell_Content(FilePath, "New_Col_1", 2, "Test, (.*)");
            Console.WriteLine("New Cell Content: " + NewValue);

            // Test: Change the content of a column to the "Match" of a regular expression applied to it
            co.Transform_Column_Content(FilePath, "Description", ".+ (Pro.* (?:Credit|Debit))");
            Console.WriteLine("Transforming Column Content.");

            // Test: keep lines where Amount column contains "Page "
            int NbLinesKept = lo.Keep_Line_If_Cell_Matches_Pattern(FilePath, "Description", @"Product.*");
            Console.WriteLine("Lines Kept: " + NbLinesKept);

            // Test: copy the "Match" from a regular expression to another column (for 1 cell)
            cop.Copy_Cell_Content_To_Other_Column(FilePath, "Description", 1, ".+ (Credit|Debit)", "New_Col_1");
            co.Copy_Column_Content_To_Other_Column(FilePath, "Description", ".+ (Credit|Debit)", "Desc_Type");
            Console.WriteLine("Copying content from columns / cells to other columns.");

            // Test: Change the value of a cell within a range for a given column
            cop.Save_Cell_Value_On_Range(FilePath, "New_Col_1", 3, 6, "\"New Ranged Value, here\"");
            Console.WriteLine("Change Cell Values based on Range");

            // Test: Swap 2 columns
            co.Switch_Columns(FilePath, "New_Col_1", "Invoice_Number");
            Console.WriteLine("Swapping Columns.");

            // Test: Force the order of columns and rearrange if necessary
            co.Enforce_Column_Order(FilePath, "Invoice_Type,New_Col_1,Invoice_Date,Invoice_Total,Invoice_Number,Item_Number,Item_Amount,Desc_Type,Description");
            Console.WriteLine("Enforcing Column Order.");

            Console.ReadKey();
        }
    }
}
