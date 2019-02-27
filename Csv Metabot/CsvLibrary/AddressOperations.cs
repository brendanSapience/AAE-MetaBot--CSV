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
        String REGEX_US_STATES = @"[,| ]+(?:(A[KLRZ]|C[AOT]|D[CE]|FL|GA|HI|I[ADLN]|K[SY]|LA|M[ADEINOST]|N[CDEHJMVY]|O[HKR]|P[AR]|RI|S[CD]|T[NX]|UT|V[AIT]|W[AIVY]))[ |,]+";
        String REGEX_US_POBOX = @"[ ]+([Pp][.]*[Oo][.]*[ ]+[Bb][Oo][Xx][ ]*(\d+[ ]*\d)*)[ |$]+";
        //https://pe.usps.com/text/pub28/28apc_002.htm
        String REGEX_US_STREET = @"(?: |^)(\d+)[ ]+([A-Z]+[ ]+(?:parkway|field|fld|dr|drive|cir|circle|crcl|blvd|boul|boulevard|ave|av|alley|ally|avenue|av|street|st|ln|lane)).*";
        String REGEX_US_STREET2 = @"(?:^| )+((?:\d+.*)(?:parkway|field|fld|dr|drive|cir|circle|crcl|blvd|boul|boulevard|ave|av|alley|ally|avenue|av|street|st|ln|lane))";
        String REGEX_TOP_USCITIES = @".*(Royal[ ]*Oak|New[ ]*York|Los[ ]*Angeles|Chicago|Brooklyn|Queens|Houston|Manhattan|Philadelphia|Phoenix|San[ ]*Antonio|Bronx|San[ ]*Diego|Dallas|San[ ]*Jose|East[ ]*San[ ]*Gabriel[ ]*Valley|Austin|Jacksonville|San[ ]*Francisco|Indianapolis|Columbus|Fort[ ]*Worth|Charlotte|Detroit|El[ ]*Paso|Seattle|Denver|Washington|Memphis|Boston|Nashville|Baltimore|Oklahoma[ ]*City|Portland|Las[ ]*Vegas|Milwaukee|Albuquerque|Tucson|Fresno|East[ ]*Seattle|Central[ ]*Contra[ ]*Costa|Sacramento|Staten[ ]*Island|Long Beach|Northeast[ ]*Tarrant|Kansas[ ]*City|Mesa|Northwest[ ]*Harris|Atlanta|Jefferson|Virginia[ ]*Beach|Omaha|Colorado[ ]*Springs|Raleigh|Miami|Oakland|Minneapolis|Tulsa|Cleveland|Wichita|New[ ]*Orleans|Arlington|North[ ]*Coast|Honolulu|Bakersfield|Tampa|Aurora|Honolulu|Anaheim|Santa[ ]*Ana|South[ ]*Aurora|San[ ]*Gabriel|Corpus[ ]*Christi|Riverside|St[. ]*Louis|Lexington-Fayette|West[ ]*Adams|Pittsburgh|Stockton|Anchorage|South Coast|Cincinnati|St[. ]*Paul|Montgomery|Fayette|Greensboro|Toledo|Newark|Ewa|Plano|Henderson|Lincoln|Louisville|Orlando|Jersey[ ]*City|Chula[ ]*Vista|Buffalo|Fort[ ]*Wayne).*";
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
            String ZIPCODE = " ";
            String STATE = " ";
            String POBOX = " ";
            String NUMBER = " ";
            String STREET = " ";
            String CITY = " ";

            RawAddress = RawAddress.Replace("_","").Replace(" - ","-");
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
                POBOX = matches[0].Groups[2].Value;
                String POBOX_R = matches[0].Groups[1].Value;

                RawAddressP2 = RawAddressP1.Replace(POBOX_R, "");
            }

            String RawAddressP3 = RawAddressP2;


            String RawAddressP4 = RawAddressP3;
            matches = Regex.Matches(RawAddressP3, REGEX_TOP_USCITIES, RegexOptions.IgnoreCase);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                // Console.WriteLine("Debug:" + matches[0].Groups[1].Value);
                CITY = matches[0].Groups[1].Value;
                
                RawAddressP4 = RawAddressP3.Replace(CITY, "");
            }

            String RawAddressP5 = RawAddressP4;
            //Console.WriteLine("LEFT: "+ RawAddressP4);
            // Regex r = new Regex(REGEX_US_STREETRegexOptions.IgnoreCase);
            matches = Regex.Matches(RawAddressP4, REGEX_US_STREET2, RegexOptions.IgnoreCase);
            if (matches.Count > 0 && matches[0].Groups.Count > 1)
            {
                // Console.WriteLine("Debug:" + matches[0].Groups[1].Value);
               // NUMBER = matches[0].Groups[1].Value;
                STREET = matches[0].Groups[1].Value;
                RawAddressP5 = RawAddressP4.Replace(STREET, "");
            }
            RawAddressP5 = RawAddressP5.Replace(",", "");

            //Console.WriteLine(RawAddressP3);

            return ZIPCODE +"|"+STATE + "|" + STREET + "|"+ POBOX+"|"+CITY+"|"+ RawAddressP5;
        }
    }
        
}
