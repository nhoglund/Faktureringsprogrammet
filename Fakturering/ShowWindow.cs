using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fakturering
{
	public class ShowWindow : Form
	{
		public ShowWindow()
		{
			Text = "Förhandsgranskning - Faktura";
			Width = 561;
			Height = 770;

			Paint += DrawInvoice;
		}

		private void DrawInvoice(object sender, PaintEventArgs args)
		{
			Graphics g = args.Graphics;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.Clear(Color.White);
			g.DrawEllipse(new Pen(Color.Black), 100, 100, 100, 100);
		}
	}
}
