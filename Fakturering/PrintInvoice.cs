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
		Pen thinPen;
		float currentX, currentY;

		public PrintInvoice(Invoice inv)
		{
			invoice = inv;
		}

        protected override void OnPrintPage(PrintPageEventArgs e) 
        {
            base.OnPrintPage(e);

			thinPen = new Pen(Color.Black, 1);

			Graphics g = e.Graphics;
            ExtendedGraphics eg = new ExtendedGraphics(g);
    
			g.TranslateTransform(e.MarginBounds.Left, e.MarginBounds.Top);
			float width = e.MarginBounds.Right - e.MarginBounds.Left;
			float height = e.MarginBounds.Bottom - e.MarginBounds.Top;
			
//			float Sx = width / 84.0f;
//			float Sy = height / 84.0f;
			float S = width / 84.0f;

//  HFONT font  = CreateFont(int(2.5*S),0     ,0,0, FW_NORMAL,FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
//  HFONT specf = CreateFont(int(2.2*S),0     ,0,0, FW_NORMAL,FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
//  HFONT bfont = CreateFont(int(2.5*S),0     ,0,0, FW_BOLD,  FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
//  HFONT lfont = CreateFont(int(2*S)  ,0     ,0,0, FW_NORMAL,FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
//  HFONT hfont = CreateFont(int(4*S)  ,0     ,0,0, FW_BOLD,  FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_SWISS,"");
			Font font  = new Font(FontFamily.GenericSerif,    2.5f * 2.5f * S, FontStyle.Regular, GraphicsUnit.Document);
			Font specF = new Font(FontFamily.GenericSerif,    2.5f * 2.2f * S, FontStyle.Regular, GraphicsUnit.Document);
			Font bfont = new Font(FontFamily.GenericSerif,    2.5f * 2.5f * S, FontStyle.Bold,    GraphicsUnit.Document);
			Font lfont = new Font(FontFamily.GenericSerif,    2.5f * 2f   * S, FontStyle.Regular, GraphicsUnit.Document);
			Font hFont = new Font(FontFamily.GenericSansSerif,2.5f * 4f   * S, FontStyle.Bold,    GraphicsUnit.Document);

			TextLeft(g,hFont, 1*S,1*S,"Båt & Inredningssnickerier");

			float doubleliney = 18*S;
			eg.DrawDoubleLine(80*S, 83*S, doubleliney, S, thinPen);
			
			bool isCopy = false;
			string whatText = isCopy ? " FAKTURAKOPIA " : " FAKTURA ";
			SizeF whatSize = g.MeasureString(whatText, hFont);
			g.DrawString(whatText, hFont, Brushes.Black, new PointF(80*S-whatSize.Width, doubleliney+0.5f*S-0.5f*whatSize.Height));
			eg.DrawDoubleLine(S, 80*S-whatSize.Width, doubleliney, S, thinPen);
			
			g.DrawString("Svante Höglund", font, Brushes.Black, new PointF(1*S,5*S));
			g.DrawString("Kalkstadsgatan 6", font, Brushes.Black, new PointF(1*S,8*S));
			g.DrawString("386 93 Färjestaden", font, Brushes.Black, new PointF(1*S,11*S));
			g.DrawString("070-544 10 98", font, Brushes.Black, new PointF(1*S,14*S));
		
			int recty=(int)(21*S);
			RectangleF rr1 = new RectangleF(1*S,  recty, 46*S, 9*S);
			RectangleF rr2 = new RectangleF(48*S, recty, 35*S, 9*S);
			eg.DrawRoundRectangle(thinPen, rr1, 1*S);
			eg.DrawRoundRectangle(thinPen, rr2, 1*S);

			float l1=recty+1*S, l2=recty+3.5f*S, l3=recty+6*S;
			TextLeft(g, lfont, 2*S,  l1, "Namn:");
			TextLeft(g, lfont, 2*S,  l2, "Adress:");
			TextLeft(g, lfont, 2*S,  l3, "Postadress:");
			TextLeft(g, lfont, 49*S, l1, "Datum:");
			TextLeft(g, lfont, 49*S, l2, "Fakturanr:");
			TextLeft(g, lfont, 49*S, l3, "Er ref:");
			TextUL(g, font, 11*S, l1, 35*S, 2.5f*S, invoice.namn);
			TextUL(g, font, 11*S, l2, 35*S, 2.5f*S, invoice.address);
			TextUL(g, font, 11*S, l3, 35*S, 2.5f*S, invoice.postnr + " " + invoice.postort);
			TextUL(g, font, 57*S, l1, 25*S, 2.5f*S, invoice.datum);
			TextUL(g, font, 57*S, l2, 25*S, 2.5f*S, invoice.fakturanr);
			TextUL(g, font, 57*S, l3, 25*S, 2.5f*S, invoice.referens);
			

			float specy = 34*S;
			float speclinespacing = 3.3f*S;
			float summay = 88*S;
			float summayend = summay+12*S;


			MoveTo(    1*S, specy-0.5f*S);
			LineTo(g, 83*S, specy-0.5f*S);
			LineTo(g, 83*S, summayend);
			LineTo(g,  1*S, summayend);
			LineTo(g,  1*S, specy-0.5f*S);
			MoveTo(    1*S, specy+speclinespacing-2);
			LineTo(g, 83*S, specy+speclinespacing-2);
			MoveTo(   70*S, specy-0.5f*S);
			LineTo(g, 70*S, summayend);
			MoveTo(   57*S, specy-0.5f*S);
			LineTo(g, 57*S, summayend);
			MoveTo(   44*S, specy-0.5f*S);
			LineTo(g, 44*S, summayend);
			
			
			TextLeft(g, bfont,  2*S,  specy, "Specifikation");
			TextRight(g, bfont, 56*S, specy, "Antal");
			TextRight(g, bfont, 69*S, specy, "À pris");
			TextRight(g, bfont, 82*S, specy, "Belopp");
			for(int i=0; i<15; i++) {
				float y=specy+speclinespacing*(i+1);
				InvoiceSpec spec = invoice.specs[i];
				TextLeft (g, specF,  2*S, y, spec.beskrivning);
				TextRight(g, specF, 56*S, y, spec.antal);
				TextRight(g, specF, 69*S, y, spec.styckpris);
				TextRight(g, specF, 82*S, y, spec.belopp);
			}
			
			
			TextLeft(g,  bfont,  2*S,  summay+0*S, "Avrundning");
			TextRight(g, bfont, 82*S, summay+0*S, Spec.Currency(invoice.Avrundning()));
			TextLeft(g,  bfont,  2*S,  summay+3*S, "Summa");
			TextRight(g, bfont, 82*S, summay+3*S, Spec.Currency(invoice.Summa()));
			TextLeft(g,  bfont,  2*S,  summay+6*S, "Moms");
			TextRight(g, bfont, 82*S, summay+6*S, Spec.Currency(invoice.Moms()));
			TextLeft(g,  bfont,  2*S,  summay+9*S, "Att betala");
			TextRight(g, bfont, 82*S, summay+9*S, Spec.Currency(invoice.AttBetala()));
			
			
			float legaly = 105*S;
			string legal1 = "F-skattesedel finns.  Momsreg.nr. 490531-0134";
			string legal2 = "Betalningsvillkor: " + invoice.antaldgr + " dagar netto, dröjsmålsränta 2%/mån.";
			string pg = "Bankgiro 5718-2305";
			
			TextLeft (g, lfont,  1*S, legaly, legal1);
			TextRight(g, lfont, 83*S, legaly, legal2);
			eg.DrawDoubleLine(S, 83*S, legaly+3*S, S, thinPen);
			TextLeft(g, font,  1*S, legaly+5*S, pg);
			
			e.HasMorePages = false;
        }
		
		private void TextLeft(Graphics g, Font font, float x, float y, string text) 
		{
			g.DrawString(text, font, Brushes.Black, new PointF(x, y));
		}
		
		private void TextUL(Graphics g, Font font, float x, float y, float l, float fh, string text)
		{
			TextLeft(g, font, x, y, text);
			int ypos = (int)(y+fh) - 1;
			g.DrawLine(thinPen, new PointF(x, ypos), new PointF(x+l, ypos));
		}
		
		void TextRight(Graphics g, Font font, float x, float y, string text)
		{
			float wid =  g.MeasureString(text, font).Width;
			g.DrawString(text, font, Brushes.Black, new PointF(x - wid, y));
		}
				
		private void MoveTo(float x, float y)
		{
			currentX = x;
			currentY = y;
		}
		
		private void LineTo(Graphics g, float x, float y)
		{
			g.DrawLine(thinPen, currentX, currentY, x, y);
			currentX = x;
			currentY = y;
		}
    }
}
