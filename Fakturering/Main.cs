using Gtk;
using System;

namespace Fakturering
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            try
            {
                InvoiceDirectory idir = new InvoiceDirectory();

                Gtk.Application.Init();
                MainWindow win = new MainWindow(idir);
                win.Show();
                Gtk.Application.Run();
            }
            catch (System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Error in " + e.TargetSite + ": " + e.Message);
            }
		}
	}
}