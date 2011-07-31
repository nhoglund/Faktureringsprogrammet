using System;
using System.IO;

namespace Fakturering
{
	public class Invoice
	{
		public class FileFormatException : Exception {}

		public string namn;
		public string address;
		public string postnr;
		public string postort;
		public string referens;
		public string fakturanr;
		public string antaldgr;
		public string datum;

		public InvoiceSpec[] specs;

		public Invoice(string nr)
		{
			namn = "";
			address = "";
			postnr = "";
			postort = "";
			referens = "";
			fakturanr = nr;
			antaldgr = "30";
			DateTime now = DateTime.Now;
			datum = String.Format("{0:0000}-{1:00}-{2:00}", now.Year, now.Month, now.Day);

			specs = new InvoiceSpec[15];
			for (int i=0; i<15; i++)
				specs[i] = new InvoiceSpec();
		}
		
		public Invoice() : this("") {}


		static string ReadLine(Stream stream)
		{
			string line = "";
			int x = stream.ReadByte();
			while (x != '\n' && x != -1) {
				line = line + Char.ConvertFromUtf32(x);
				x = stream.ReadByte();
			}
				
			return line;
		}
		
		static void WriteLine(Stream stream, string line)
		{
			foreach (char c in line) {
				stream.WriteByte((byte)c);
			}
			stream.WriteByte((byte)'\n');
		}

		static string Mangle(string s)
		{
			return s.Replace("\t", "\\t").Replace("\n", "\\n");
		}
		
		static string MangleOrEmpty(double x)
		{
			if (Math.Abs(x) < 0.00001)
				return "";
			else
				return Mangle(Spec.Currency(x));
		}
			                 
		static string Unmangle(string s)
		{
			return s.Replace("\\t", "\t").Replace("\\n", "\n");
		}

		
		public void load(string path)
		{
			FileStream stream = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read);

			namn = Unmangle(ReadLine(stream));
			bool version2 = ((namn == "||VERSION2||") || (namn == "||VERSION3||"));
			if (version2)
				namn = Unmangle(ReadLine(stream));

			address   = Unmangle(ReadLine(stream));
			postnr    = Unmangle(ReadLine(stream));
			postort   = Unmangle(ReadLine(stream));
			referens  = Unmangle(ReadLine(stream));
			datum     = Unmangle(ReadLine(stream));
			fakturanr = Unmangle(ReadLine(stream));
			if (version2)
				antaldgr = Unmangle(ReadLine(stream));

			specs = new InvoiceSpec[15];
			for (int i=0; i<15; i++) {
				string line = ReadLine(stream);
				string[] elems = line.Split('\t');
				if (elems.Length != 4)
					throw new FileFormatException();
				specs[i] = new InvoiceSpec(Unmangle(elems[0]),
				                           Unmangle(elems[1]),
				                           Unmangle(elems[2]),
				                           Unmangle(elems[3]));
			}

			stream.Close();
		}
		
		
		public static void WriteMangle(Stream stream, string line)
		{
			WriteLine(stream, Mangle(line));
		}

		public void save(string path)
		{
			FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);

			WriteMangle(stream, "||VERSION3||");
			WriteMangle(stream, namn);
			WriteMangle(stream, address);
			WriteMangle(stream, postnr);
			WriteMangle(stream, postort);
			WriteMangle(stream, referens);
			WriteMangle(stream, datum);
			WriteMangle(stream, fakturanr);
			WriteMangle(stream, antaldgr);

			foreach (InvoiceSpec spec in specs) {
				WriteLine(stream, Mangle(spec.beskrivning) + "\t" +
				                  Mangle(spec.antal)       + "\t" +
				                  spec.styckpris           + "\t" +
				                  spec.belopp);
			}
			stream.Close();
		}
		public double OavrundadSumma()
		{
			double sum = 0.0;
			for (uint i=0; i<specs.Length; i++)
				sum += specs[i].GetAmount();
			return sum;
		}
		
		public double Avrundning()
		{
			double sum = OavrundadSumma();
			return Math.Floor(sum/4.0) * 4.0 - sum;
		}
		
		public double Summa()
		{
			return OavrundadSumma() + Avrundning();
		}
		
		public double Moms()
		{
			return 0.25 * Summa();
		}
		
		public double AttBetala()
		{
			return Summa() + Moms();
		}
	}
}
