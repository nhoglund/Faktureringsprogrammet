using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Fakturering
{
	public class PrintInvoice : PrintDocument
	{
		Invoice invoice;
		int pageno;

		public PrintInvoice(Invoice inv)
		{
			invoice = inv;
		}

		protected override void OnBeginPrint(PrintEventArgs e) 
		{
			base.OnBeginPrint(e);
			pageno = 1;
		}
		
		protected override void OnEndPrint(PrintEventArgs e)
		{
			base.OnEndPrint(e);
		}

        protected override void OnPrintPage(PrintPageEventArgs e) 
        {
            base.OnPrintPage(e);
			
			e.Graphics.TranslateTransform(e.MarginBounds.Left, e.MarginBounds.Top);
			float width = e.MarginBounds.Right - e.MarginBounds.Left;
			float height = e.MarginBounds.Bottom - e.MarginBounds.Top;
			
			invoice.Draw(e.Graphics, width, height, pageno == 2);
			pageno++;
			e.HasMorePages = (pageno <= 2);
        }
    }
}
