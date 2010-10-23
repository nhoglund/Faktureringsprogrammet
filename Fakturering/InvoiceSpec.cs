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
	}
}
