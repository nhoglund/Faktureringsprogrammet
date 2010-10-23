using System;
using System.IO;
using System.Collections.Generic;

namespace Fakturering
{
	public class InvoiceDirectory
	{
		// The folder where invoices are stored
		string dir;
		
		// All invoice filenames end with this suffix
		const string suffix = ".faktura";


		public InvoiceDirectory()
		{
			dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
			                   "Fakturor");
			// create the directory, ignoring the error if it exists
			try {
				Directory.CreateDirectory(dir);
			} catch(Exception) {}
		}


		// Generate the full pathname to the invoice
		public string PathName(string invoicename)
		{
			return dir + "/" + invoicename + suffix;
		}


		// Try to delete a file, silently failing on all errors
		public static void remove(string path)
		{
			try {
				System.IO.File.Delete(path);
			}
			catch (Exception) {}
		}


		static bool IsInvoice(string filename)
		{
			return filename.EndsWith(suffix);
		}
		

		static string AddPrefix(string s)
		{
			if (s[0] == '8' || s[0] == '9')
				return "19" + s;
			else
				return "20" + s;
		}


		// A comparison function for invoice names. Does a normal string compare
		// but adds 19 before all names beginning with 8 or 9, or 20 otherwise.
		// This makes invoices like 98-01 xxx sort before 00-01 (year order).
		static int InvoiceNameCompare(string a, string b)
		{
			return String.Compare(AddPrefix(a), AddPrefix(b));
		}


		public List<string> Invoices()
		{
			List<string> files = new List<string>(System.IO.Directory.GetFiles(dir));
			List<string> invoices = files.FindAll(IsInvoice);
			List<string> names = invoices.ConvertAll<string>(System.IO.Path.GetFileNameWithoutExtension);
			names.Sort(InvoiceNameCompare);
			return names;
		}


		public string NextFreeInvoiceNr()
		{
			string year = String.Format("{0:00}", System.DateTime.Now.Year % 100);
			int nextFree = 1;

			foreach (string invoice in Invoices()) {
				if (invoice.StartsWith(year)) {
					int used;
					if (Int32.TryParse(invoice.Substring(3, 2), out used) &&
					    (used + 1 > nextFree))
					{
						nextFree = used + 1;
					}
				}
			}
			
			return year + "-" + String.Format("{0:00}", nextFree);
		}
	}
}
