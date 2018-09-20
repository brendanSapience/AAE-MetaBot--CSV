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
            String FilePath = @"C:\IQBot Input\[7618da06-56f2-45f5-8b89-991f5315c3b8]_0057_486301224 June-2018.pdf.csv";
           
            
            //utils.SetFile();
            //String myVal = utils.Get_Line_Content(2);
            //utils.Add_Column_Between("Invoice_Number", "Invoice_Date", "test Columns!","<No Data>");
           // String CellContent = utils.Get_Cell_Content("Invoice_Date", 3);

            //String NewContent1 = utils.Transform_Cell_Content("Item_Description", 3, @".* size (.*)");
           // utils.Transform_Column_Content("Item_Description", @".* size (\d+).*");
           
            utils.Delete_Line_If_Cell_Matches_Pattern(FilePath,"Amount", @"Page.*|Page .*|.*, .*");
            utils.Append_If_Column_Matches_Pattern(FilePath, "Description", ".*Debit.*", "Amount", "-", false);
            //utils.Keep_Line_If_Cell_Matches_Pattern(FilePath, "Description", @"((?:Transfer (?:Credit|Debit) (?:B\/O|A\/C)){1}[ ]*:.*(?: Hess){1}.*)|^Cash Concentration.*");

            //((?:Book Transfer (?:Credit|Debit) (?:B\/O|A\/C)){1}[ ]*:.*(?: Hess){1}.*)|(Cash Concentration.*)/i

            // utils.SetFile(@"C:\Users\brendan.sapience\Google Drive\AutomationAnywhere\IQ Bot Output\Output\Z_Output_Post_Processing\Layout1_Train(5)_Hess - Full.csv");
            //utils.Save_Cell_Value_On_Range(FilePath,"Table_Type", 1, 109, "Deposits and Additions");
            //Console.WriteLine("Result: " + CellContent + " - ");
            //

            Console.ReadKey();
        }
    }
}
