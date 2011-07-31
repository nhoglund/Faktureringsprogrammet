using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fakturering
{
	public class ShowWindow : Form
	{
		Invoice invoice;

		public ShowWindow(Invoice inv)
		{
			Text = "Förhandsgranskning - Faktura";
			Width = 561;
			Height = 770;
			invoice = inv;

			Paint += DrawInvoice;
		}

		private void DrawInvoice(object sender, PaintEventArgs args)
		{
			Graphics g = args.Graphics;
			//g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.Clear(Color.White);
			invoice.Draw(g, ClientSize.Width, ClientSize.Height, false);
		}
	}
}
