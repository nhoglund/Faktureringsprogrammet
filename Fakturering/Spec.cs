// Spec.cs created with MonoDevelop
// User: niklas at 08:58 09/10/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;

namespace Fakturering
{
	public class Spec
	{
		const int NumberEntryWidth = 100;
		
		public Entry description;
		public Entry number;
		public Entry price;
		public Entry total;

		public delegate void ChangedType();
		public event ChangedType Changed;

		public static double RealVal(string s)
		{
			try {
                int i = 0;
                while (i < s.Length &&
                       (((s[i] >= '0') && (s[i] <= '9')) ||
                        (s[i] == '.') ||
                        (s[i] == ',') ||
                        (s[i] == ' ')))
                {
                    i++;
                }

                // find out if the current locale uses . or , to separate the fraction part
                char sep = String.Format("{0:0.0}", 0.0)[1];

				return Double.Parse(s.Substring(0, i).Replace(',', sep),
				                    System.Globalization.NumberStyles.Number);
			}
			catch (System.FormatException) {
				return 0.0;
			}
		}

		public static string Currency(double x)
		{
			//return String.Format("{0:0.00}", x);
            return x.ToString("F2", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',');
		}

		private void UpdateTotal(object sender, EventArgs args)
		{
            try
            {
    			double n = RealVal(number.Text);
	    		double p = RealVal(price.Text);
		    	total.Text = Currency(n * p);
                if (Changed != null) Changed();
            }
            catch (System.Exception e)
            {
                MessageDialog("Error in " + e.TargetSite +
                              ": " + e.Message);
            }
        }
		
		private void TotalChanged(object sender, EventArgs args)
		{
            try
            {
                if (Changed != null) Changed();
            }
            catch (System.Exception e)
            {
                MessageDialog("Error in " + e.TargetSite +
                              ": " + e.Message);
            }
		}

        private void MessageDialog(string msg)
        {
            Dialog dialog = new Dialog();
            HButtonBox buttons = new HButtonBox();
            buttons.PackStart(Button.NewWithLabel("Ok"));
            dialog.VBox.PackStart(new Label(msg));
            dialog.ActionArea.PackStart(buttons);
            dialog.ShowAll();
        }

		public Spec(InvoiceSpec ispec, Table table, uint row)
		{
			description = new Entry();
			number = new Entry();
			price = new Entry();
			total = new Entry();
	
			number.Changed += new EventHandler(UpdateTotal);
			price.Changed += new EventHandler(UpdateTotal);
			total.Changed += new EventHandler(TotalChanged);

			number.SetUsize(NumberEntryWidth, -2);
			price.SetUsize(NumberEntryWidth, -2);
			total.SetUsize(NumberEntryWidth, -2);
			description.SetUsize(300, -2);

			table.Attach(description, 0, 1, row, row+1,
			             AttachOptions.Expand | AttachOptions.Fill, 0, 0, 0);
			table.Attach(number, 1, 2, row, row+1, 0, 0, 0, 0);
			table.Attach(price,  2, 3, row, row+1, 0, 0, 0, 0);
			table.Attach(total,  3, 4, row, row+1, 0, 0, 0, 0);

			description.Text =  ispec.beskrivning;
			if (ispec.antal.Length > 0)				  number.Text = ispec.antal;
			price.Text = ispec.styckpris;
			total.Text = ispec.belopp;
		}
		
		public double GetTotal()
		{
			return RealVal(total.Text); 
		}
	}
}
