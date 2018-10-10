using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvLibrary
{
    class KVP
    {
    }

    public class KeyValuePair
    {
        public String KeyName;
        public String Value;
        public KeyValuePair(String KeyName, String Value)
        {
            this.KeyName = KeyName;
            this.Value = Value;
        }

    }

    public class KeyValuePairArray
    {
        public List<KeyValuePair> listOfItems = new List<KeyValuePair>();
        public KeyValuePairArray()
        {

        }
        public void AddElement(KeyValuePair kvp)
        {
            this.listOfItems.Add(kvp);
        }
    }
}
