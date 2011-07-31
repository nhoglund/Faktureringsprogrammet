using System;
using Gtk;
using System.Drawing.Printing;

namespace Fakturering
{
	public partial class MainWindow : Gtk.Window
	{
		InvoiceDirectory idir;

		HBox maingroup;
		VButtonBox buttons;
		ScrolledWindow scrolledhd;
		TreeView listview;
		ListStore liststore;
		Button create;
		Button edit;
		Button delete;
		Button showbut;
		Button printbut;

		private void CreateListView()
		{
			listview = new TreeView();
			liststore = new ListStore(typeof(string));
			listview.HeadersVisible = false;
			listview.Model = liststore;

			TreeViewColumn invoiceName = new TreeViewColumn();
			CellRendererText invoiceNameRenderer = new CellRendererText();
			invoiceName.Title = "Fakturor";
			invoiceName.PackStart(invoiceNameRenderer, true);
			listview.AppendColumn(invoiceName);
			invoiceName.AddAttribute(invoiceNameRenderer, "text", 0);	
		}

		public MainWindow(InvoiceDirectory idir_) : base (WindowType.Toplevel)
		{
			idir = idir_;

			SetSizeRequest(500, 500);
			Title = "Fakturering 2.6";

			maingroup = new HBox(false, 0);
			buttons = new VButtonBox();
			scrolledhd = new ScrolledWindow();
			create   = Button.NewWithLabel("Skapa ny faktura");
			edit     = Button.NewWithLabel("Redigera faktura");
			delete   = Button.NewWithLabel("Radera faktura");
			showbut  = Button.NewWithLabel("Visa faktura");
			printbut = Button.NewWithLabel("Skriv ut faktura");
			
			CreateListView();

			Add(maingroup);
			maingroup.PackStart(scrolledhd, true, true, 0);
            maingroup.PackStart(buttons, false, false, 0);
			buttons.Layout = ButtonBoxStyle.Start;
			buttons.PackStart(create,   false, false, 0);
			buttons.PackStart(edit,     false, false, 0);
			buttons.PackStart(showbut,  false, false, 0);
			buttons.PackStart(printbut, false, false, 0);
			buttons.PackStart(delete,   false, false, 0);
			scrolledhd.AddWithViewport(listview);
			scrolledhd.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

			UpdateHDList();
			
			create.Clicked   += new EventHandler(Create);
			edit.Clicked     += new EventHandler(Edit);
			delete.Clicked   += new EventHandler(DeleteInvoice);
			showbut.Clicked  += new EventHandler(ShowInvoice);
			printbut.Clicked += new EventHandler(PrintInvoice);

			maingroup.ShowAll();

			//Den här måste vara här då storleken inte är känd innan
			ScrollToEnd(scrolledhd);

			DeleteEvent += new Gtk.DeleteEventHandler(DeleteWindow);
		}
		
		private void UpdateHDList()
		{
			liststore.Clear();
			idir.Invoices().ForEach(delegate (string s) {
				liststore.AppendValues(s);
			});			
		}

		private void Create(object sender, EventArgs args)
		{
			Window editWindow = new EditWindow(new Invoice(idir.NextFreeInvoiceNr()), null, idir, UpdateHDList);
			editWindow.Show();
		}

		private bool SelectedInvoice(out string name)
		{
			TreeIter iter;
			if (listview.Selection.GetSelected(out iter)) {
			    name = (string)liststore.GetValue(iter, 0);
				return true;
			}
			
			name = null;
			return false;
		}

		private void Edit(object sender, EventArgs args)
		{
            try
            {
                string name;
                if (SelectedInvoice(out name))
                {
                    Invoice invoice = new Invoice();
                    string file = idir.PathName(name);
                    invoice.load(file);
                    Window editWindow = new EditWindow(invoice, file, idir, UpdateHDList);
                    editWindow.Show();
                }
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
			Button ok = Button.NewWithLabel("Ok");
            buttons.PackStart(ok);
			
			ok.Clicked += delegate (object o, EventArgs dummy) {
				dialog.Destroy();
			};

            dialog.VBox.PackStart(new Label(msg));
            dialog.ActionArea.PackStart(buttons);
            dialog.ShowAll();
        }

		private void DeleteInvoice(object sender, EventArgs args)
		{
			string inv;
			if (SelectedInvoice(out inv)) {
				Dialog dialog = new Dialog();
				Label msg = new Label("Vill du radera fakturan " + inv + "?");
				HButtonBox buttons = new HButtonBox();
				Button yes = Button.NewWithLabel("Ja");
				Button no = Button.NewWithLabel("Nej");
				dialog.VBox.PackStart(msg);
				dialog.ActionArea.PackStart(buttons);
				buttons.PackStart(yes);
				buttons.PackStart(no);

				no.Clicked += delegate (object o, EventArgs dummy) {
					dialog.Destroy();
				};

				yes.Clicked += delegate (object o, EventArgs dummy) {
					InvoiceDirectory.remove(idir.PathName(inv));
					UpdateHDList();
					dialog.Destroy();
				};
			
				dialog.ShowAll();
			}
		}

		private void ShowInvoice(object sender, EventArgs args)
		{
            try
            {
                string name;
                if (SelectedInvoice(out name))
                {
                    Invoice invoice = new Invoice();
                    string file = idir.PathName(name);
                    invoice.load(file);
					ShowWindow sw = new ShowWindow(invoice);
					sw.Show();

//					PrintInvoice printDoc = new PrintInvoice(invoice);
//					System.Windows.Forms.PrintPreviewDialog dlgPrintPreview = new System.Windows.Forms.PrintPreviewDialog();
//					dlgPrintPreview.Document = printDoc;
//					dlgPrintPreview.WindowState = System.Windows.Forms.FormWindowState.Maximized;
//					dlgPrintPreview.ShowDialog();
                }
            }
            catch (System.Exception e)
            {
                MessageDialog("Error in " + e.TargetSite +
                              ": " + e.Message);
			}
		}

		private void PrintInvoice(object sender, EventArgs args)
		{
            try
            {
                string name;
                if (SelectedInvoice(out name))
                {
                    Invoice invoice = new Invoice();
                    string file = idir.PathName(name);
                    invoice.load(file);

					PrintInvoice printDoc = new PrintInvoice(invoice);
					System.Windows.Forms.PrintDialog dlg = new System.Windows.Forms.PrintDialog();
					dlg.AllowSomePages = true;
					dlg.ShowHelp = true;
					dlg.Document = printDoc;
					dlg.PrinterSettings.MinimumPage = 1;
					dlg.PrinterSettings.MaximumPage = 2;
					dlg.PrinterSettings.FromPage = 1;
					dlg.PrinterSettings.ToPage = 2;
					if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
						printDoc.Print();
					}
                }
            }
            catch (System.Exception e)
            {
                MessageDialog("Error in " + e.TargetSite +
                              ": " + e.Message);
			}
		}

		// Den här måste anropas efter att fönstret visats
		static private void ScrollToEnd(ScrolledWindow window)
		{
			Adjustment adj = window.Vadjustment;
			adj.Value = adj.Upper - adj.PageSize;
			window.Vadjustment = adj;
		}

		protected void DeleteWindow(object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}
	}
}
