using System;
using Gtk;

namespace Fakturering
{
	public class EditWindow : Gtk.Window
	{
		private Table table;
		private Table tablesum;

		Entry namn;
		Entry address;
		Entry postnr;
		Entry postort;
		Entry referens;
		Entry datum;
		Entry fakturanr;
		Entry antaldgr;
		
		Spec[] specs;
		
		Label labround;
		Label labsum;
		Label labmoms;
		Label labtot;
		
		// if this is null, then we don't have a filename
		// that happens if we're creating a new invoice
		string loadedFile;
		
		InvoiceDirectory idir;

		public delegate void InvoicesChanged();
        InvoicesChanged callback;		

        public EditWindow(Invoice invoice,
		                  string file,
		                  InvoiceDirectory idir_,
                          InvoicesChanged cb)
            : base (WindowType.Toplevel)
		{
			loadedFile = file;
            idir = idir_;
            callback = cb;

			VBox box = new VBox(false, 0);
			table = new Table(7, 3, false);
			Table tablespec = new Table(16, 4, false);
			tablesum = new Table(4, 2, false);

			HButtonBox buttons = new HButtonBox();
			Button save = Button.NewWithLabel("Spara");
			Button abort = Button.NewWithLabel("Avbryt");

			namn = new Entry();
			address = new Entry();
			postnr = new Entry();
			postort = new Entry();
			referens = new Entry();
			datum = new Entry();
			fakturanr = new Entry();
			antaldgr = new Entry();

			ScrolledWindow scrolled = new ScrolledWindow();
			Button GetAddress = Button.NewWithLabel("Slå upp address");

			Add(box);
			scrolled.AddWithViewport(tablespec);
			scrolled.SetPolicy(PolicyType.Never, PolicyType.Automatic);
			box.PackStart(table, false, false, 0);
			box.PackStart(scrolled, true, true, 0);
			scrolled.SetUsize(~1, 250);

			box.PackStart(tablesum, false, false, 0);
			box.PackStart(buttons,  false, false, 0);

			buttons.PackStart(save);
			buttons.PackStart(abort);

			specs = new Spec[15];
			for (uint i=0; i<15; i++) {
                specs[i] = new Spec(invoice.specs[i], tablespec, i + 1);
                specs[i].Changed += RecalcSum;
			}

			tablespec.Attach(new Label("Beskrivning"), 0, 1, 0, 1, 0, 0, 0, 0);
			tablespec.Attach(new Label("Antal"),       1, 2, 0, 1, 0, 0, 0, 0);
			tablespec.Attach(new Label("À pris"),      2, 3, 0, 1, 0, 0, 0, 0);
			tablespec.Attach(new Label("Belopp"),      3, 4, 0, 1, 0, 0, 0, 0);

			SetPosition(WindowPosition.Center);
			
			namn.Changed += UpdateTitle;
			fakturanr.Changed += UpdateTitle;

			namn.Text = invoice.namn;
			address.Text = invoice.address;
			postnr.Text = invoice.postnr;
			postort.Text = invoice.postort;
			referens.Text = invoice.referens;
			datum.Text = invoice.datum;
			fakturanr.Text = invoice.fakturanr;
			antaldgr.Text = invoice.antaldgr;

			labround = new Label("0");
			labsum   = new Label("0");
			labmoms  = new Label("0");
			labtot   = new Label("0");

            RecalcSum();

			Attach2(0, "Namn", namn);
			table.Attach(GetAddress, 2, 3, 0, 1, 0, 0, 0, 0);
			Attach2(1, "Address", address);
			Attach2(2, "Postnummer", postnr);
			Attach2(3, "Postort", postort);
			Attach2(4, "Er referens", referens);
			Attach2(5, "Datum", datum);
			Attach2(6, "Fakturanummer", fakturanr);
			Attach2(7, "Antal dagar",   antaldgr);

			Attach3(0, "Avrundning", labround);
			Attach3(1, "Summa", labsum);
			Attach3(2, "Moms", labmoms);
			Attach3(3, "Att betala", labtot);

			abort.Clicked += Abort;
			save.Clicked += Save;
			GetAddress.Clicked += FindAddress;

			box.ShowAll();
		}

		void Attach2(uint row, string name, Widget widget)
		{
			Label lab = new Label(name);
			lab.SetAlignment(1.0f, 0.5f);
			table.Attach(lab, 0, 1, row, row+1, AttachOptions.Fill, 0, 0, 0);
			table.Attach(widget, 1, 2, row, row+1, AttachOptions.Expand | AttachOptions.Fill, 0, 0, 0);
		}

		void Attach3(uint row, string name, Label widget)
		{
			Label lab = new Label(name);
			lab.SetAlignment(0.0f, 0.5f);
			widget.SetAlignment(1.0f, 0.5f);
			tablesum.Attach(lab, 0, 1, row, row+1, AttachOptions.Fill, 0, 0, 0);
			tablesum.Attach(widget, 1, 2, row, row+1, AttachOptions.Expand | AttachOptions.Fill, 0, 0, 0);
		}

		void RecalcSum()
		{
			double sum = 0.0;
			for (uint i=0; i<specs.Length; i++)
				sum += specs[i].GetTotal();
			
			double round = Math.Floor(sum/4.0) * 4.0 - sum;
			double rounded = sum + round;
			double vat = 0.25 * rounded;
			double tot = rounded + vat;
			
			labround.Text = Spec.Currency(round);
			labsum.Text   = Spec.Currency(rounded);
			labmoms.Text  = Spec.Currency(vat);
			labtot.Text   = Spec.Currency(tot);
		}

		void UpdateTitle(object sender, EventArgs args)
		{
			Title = fakturanr.Text + " " + namn.Text;
		}
		
		void Abort(object sender, EventArgs args)
		{
			Destroy();
		}

		void Save(object sender, EventArgs args)
		{
			Invoice i = new Invoice(fakturanr.Text);
			i.namn     = namn.Text;
			i.address  = address.Text;
			i.postnr   = postnr.Text;
			i.postort  = postort.Text;
			i.referens = referens.Text;
			i.datum    = datum.Text;
			i.antaldgr = antaldgr.Text;
			
			for (int k=0; k<specs.Length; k++) {
				i.specs[k].beskrivning = specs[k].description.Text;
				i.specs[k].antal       = specs[k].number.Text;
				i.specs[k].styckpris   = specs[k].price.Text;
			    i.specs[k].belopp      = specs[k].total.Text;
		    }
			
        	if (loadedFile != null)
			    InvoiceDirectory.remove(loadedFile);
			i.save(idir.PathName(i.fakturanr + " " + i.namn));
            callback();
            Destroy();
		}
		
		void FindAddress(object sender, EventArgs args)
		{
			idir.Invoices().ForEach(delegate (string name) {
				Invoice invoice = new Invoice();
				invoice.load(idir.PathName(name));
				if (String.Compare(invoice.namn, namn.Text, true) == 0) {
					address.Text  = invoice.address;
					postnr.Text   = invoice.postnr;
					postort.Text  = invoice.postort;
					referens.Text = invoice.referens;
				}
			});
		}
	}
}
