using System;

namespace Fakturering
{
	public class InvoiceSpec
	{
		public string beskrivning;
		public string antal;
		public string styckpris;
		public string belopp;

		public InvoiceSpec()
		{
			beskrivning = "";
			antal = "";
			styckpris = "";
			belopp = "";
		}
		
		public InvoiceSpec(string b, string a, string s, string be)
		{
			beskrivning = b;
			antal = a;
			styckpris = s;
			belopp = be;
		}

		public double GetAmount()
		{
			if (belopp.Trim() == "") return 0.0;

            int i = 0;
            while (i < belopp.Length &&
                   (((belopp[i] >= '0') && (belopp[i] <= '9')) ||
                    (belopp[i] == '.') ||
                    (belopp[i] == ',') ||
                    (belopp[i] == ' ')))
            {
                i++;
            }

            // find out if the current locale uses . or , to separate the fraction part
            char sep = String.Format("{0:0.0}", 0.0)[1];

			return Double.Parse(belopp.Substring(0, i).Replace(',', sep),
			                    System.Globalization.NumberStyles.Number);
		}
	}
}
