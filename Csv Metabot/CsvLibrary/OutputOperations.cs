using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CsvLibrary
{
    public class OutputOperations
    {

        // the following method is fairly complex and undocumented.. TBD
        public string Get_Output_As_Json(String InputFile, String TitleForSingleValues, String ListOfColumsOfStandardFields, String TitleForItemValues, String ListOfColumnsOfItemizedFields)
        {
            CsvUtils cu = new CsvUtils();
            cu.SetFile(InputFile);

            // Get the list of columns that contain standard fields
            String[] ColumsOfStandardFields = ListOfColumsOfStandardFields.Split(',');

            // Get the list of columns that contain itemized fields
            String[] ColumnsOfItemizedFields = ListOfColumnsOfItemizedFields.Split(',');

            // This List will contain all "Key|Value" pairs retrieved from columns with standard fields: ex:[ {invoice_Date:1/2/2018} , {invoice_number:1234} ]
            List<KeyValuePair> AllKeyValuePairsStdFields = new List<KeyValuePair>();

            // this list will contain all lists of "Key|Value" pairs retrieved form columns with itemized fields[ [{Item Number:1},{Item Total:122}],[{Item Number:2},{Item Total:144}] ]
            List<KeyValuePairArray> AllKeyValuePairsItemFields = new List<KeyValuePairArray>();

            foreach (String ColName in ColumsOfStandardFields)
            {
                int idx = cu.Get_Column_Index(ColName);
                if(idx < 0) { return GetErrorJson("Standard Column Not Found: "+ColName); }

                List<String> AllVals = cu.GetAllValuesFromColumn(ColName);
                List<String> distinct = AllVals.Distinct().ToList();
                KeyValuePair kvp = new KeyValuePair(ColName, distinct[1]);
                AllKeyValuePairsStdFields.Add(kvp);
                //Console.Write("Value Std:" + distinct[1]);
            }

            List<int> ColumnsToExtract = new List<int>();
            foreach (String ColName in ColumnsOfItemizedFields)
            {
                int idx = cu.Get_Column_Index(ColName);
                if (idx < 0) { return GetErrorJson("Itemized Column Not Found: " + ColName); }
                int cIdx = cu.Get_Column_Index(ColName);
                ColumnsToExtract.Add(cIdx);
            }

            foreach (KeyValuePair<int, List<String>> entry in cu.dict)
            {

                // List<String> OneSetofValues = new List<String>();
                if (entry.Key > 0)
                {
                    KeyValuePairArray kvpa = new KeyValuePairArray();
                    int idx = 0;
                    foreach (String s in entry.Value)
                    {

                        //var result0 = String.Join(",", ColumnsToExtract.ToArray());
                        //Console.Write("Debug:" + entry.Value.IndexOf(s) + "|"+ result0);
                        String ColumnHeader = cu.Get_Column_name(idx);
                        
                        bool isInList = ColumnsToExtract.IndexOf(idx) != -1;
                        if (isInList)
                        {

                            KeyValuePair kvp = new KeyValuePair(ColumnHeader, s);

                            kvpa.AddElement(kvp);
                            // OneSetofValues.Add(s);
                        }
                        idx++;
                    }
                    AllKeyValuePairsItemFields.Add(kvpa);
                }

            }

            String dataString = "{" + "\"" + TitleForSingleValues + "\":{";
            foreach (KeyValuePair kvp in AllKeyValuePairsStdFields)
            {
                dataString = dataString + "\"" + kvp.KeyName + "\":\"" + kvp.Value + "\",";
            }
            dataString = dataString.TrimEnd(',');
            dataString = dataString + "}";

            dataString = dataString + "," + "\"" + TitleForItemValues + "\":[";
            foreach (KeyValuePairArray kvpa in AllKeyValuePairsItemFields)
            {
                dataString = dataString + "{";
                foreach (KeyValuePair kvp in kvpa.listOfItems)
                {
                    dataString = dataString + "\"" + kvp.KeyName + "\":" + "\"" + kvp.Value + "\",";
                }
                dataString = dataString.TrimEnd(',');
                dataString = dataString + "},";
            }
            dataString = dataString.TrimEnd(',');
            dataString = dataString + "]";
            // Final Brace
            dataString = dataString + "}";

            return dataString;
        }

        private String GetErrorJson(String msg)
        {
            return "{\"error\":\""+msg+"\"" +"}";
        }

    }
}
