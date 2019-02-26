using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CsvLibrary
{
    public class AddressOperations
    {
        String REGEX_US_ZIPCODE = @".*(\d{5}-\d{4}|\d{5}).*";
        String REGEX_US_STATES = @"(?:(A[KLRZ]|C[AOT]|D[CE]|FL|GA|HI|I[ADLN]|K[SY]|LA|M[ADEINOST]|N[CDEHJMVY]|O[HKR]|P[AR]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY]))";
        String REGEX_US_POBOX = @"[ ]+([Pp][Oo][ ]+[Bb][Oo][Xx][ ]*\d+[ ]*\d*)[ |$]+";
        //https://pe.usps.com/text/pub28/28apc_002.htm
        String REGEX_US_STREET = @"(?: |^)(\d+)[ ]+([A-Z]+[ ]+(?:parkway|field|fld|dr|drive|cir|circle|crcl|blvd|boul|boulevard|ave|av|alley|ally|avenue|av|street|st|ln|lane)).*";
        // 818 Lexington Ave, apt 6A, brooklyn, NY 11221
        // ATTN: Accounts Payable PO Box 1 10656 Nashville, TN 37222
        // ASURION Attention: RITA SWEENEY PO Box 209348 Austin, TX 78720-9348
        // Asurion 350 Parkhurst Square Brampton, ON L6T 5W1
        // Attn: Accounts Payable New Customer Service Co. PO #IP16320 22660 Executive Dr.,

        public String Process_Address(String CountryCode, String RawAddress)
        {
            if (CountryCode == "US" || CountryCode == "USA")
            {
                return Process_Address_US(RawAddress);
            }

            return "";

        }
        public String Process_Address_US(String RawAddress)
        {
            String ZIPCODE = "";
            String STATE = "";
            String POBOX = "";
            String NUMBER = "";
            String STREET = "";

            String RawAddressP1 = RawAddress;
            var matches = Regex.Matches(RawAddress, REGEX_US_ZIPCODE);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
                {
                    ZIPCODE = matches[0].Groups[1].Value;
                    RawAddressP1 = RawAddress.Replace(ZIPCODE, "");
            }

            matches = Regex.Matches(RawAddressP1, REGEX_US_STATES);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                STATE = matches[0].Groups[1].Value;
                RawAddressP1 = RawAddressP1.Replace(STATE, "");
            }

            //RawAddressP1 = RawAddress.Replace(ZIPCODE, "").Replace(STATE, "");
            String RawAddressP2 = RawAddressP1;

            matches = Regex.Matches(RawAddressP1, REGEX_US_POBOX);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                POBOX = matches[0].Groups[1].Value;
                RawAddressP2 = RawAddressP1.Replace(POBOX, "");
            }

            String RawAddressP3 = RawAddressP2;
            //Console.WriteLine("LEFT: "+RawAddressP2);
           // Regex r = new Regex(REGEX_US_STREETRegexOptions.IgnoreCase);
            matches = Regex.Matches(RawAddressP2, REGEX_US_STREET,RegexOptions.IgnoreCase);
            if (matches.Count > 0 && matches[0].Groups.Count > 2)
            {
               // Console.WriteLine("Debug:" + matches[0].Groups[1].Value);
                NUMBER = matches[0].Groups[1].Value;
                STREET = matches[0].Groups[2].Value;
                RawAddressP3 = RawAddressP2.Replace(STREET, "").Replace(NUMBER,"");
            }

            //Console.WriteLine(RawAddressP3);

            return ZIPCODE+"|"+STATE+"|"+ NUMBER + "|" + STREET + "|"+ POBOX;
        }
    }
        
}
