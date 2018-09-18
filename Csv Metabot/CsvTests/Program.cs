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
            CsvUtils utils = new CsvUtils(@"C:\Users\brendan.sapience\Google Drive\AutomationAnywhere\IQ Bot Output\Output\Z_Output_Post_Processing\[8903ca8a-2995-4334-85bd-6d0c7ca1390b]_Medical Innovations 87167.pdf.csv");
            //String myVal = utils.Get_Line_Content(2);
            //utils.Add_Column_Between("Invoice_Number", "Invoice_Date", "test Columns!","<No Data>");
            String CellContent = utils.Get_Cell_Content("Invoice_Date", 3);

            //String NewContent1 = utils.Transform_Cell_Content("Item_Description", 3, @".* size (.*)");
           // utils.Transform_Column_Content("Item_Description", @".* size (\d+).*");
           

            utils.Delete_Line_If_Cell_Matches_Pattern("Item_Description", @"^.*Shiley distal.*$");

            Console.WriteLine("Result: " + CellContent + " - ");

            //

            Console.ReadKey();
        }
    }
}
