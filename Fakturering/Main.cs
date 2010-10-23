using System;
using Gtk;

namespace Fakturering
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            try
            {
                InvoiceDirectory idir = new InvoiceDirectory();

                Application.Init();
                MainWindow win = new MainWindow(idir);
                win.Show();
                Application.Run();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error in " + e.TargetSite + ": " + e.Message);
            }
		}
	}
}