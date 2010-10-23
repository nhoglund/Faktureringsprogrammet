#include <windows.h>
#include <memory.h>
#include <math.h>
#include <stdlib.h>
#include <stdio.h>
#include <sys/stat.h>

char *namn, *address, *postnr, *postort, *referens, *datum, *fakturanr, *antaldagar;
char summa[100], rounding[100], moms[100], total[100];

struct specifikation {
  char *desc, *antal, *pris, *belopp;
};
struct specifikation specar[15];

int mystrlen(const char *s)
{
  if(!s) return 0; else return strlen(s);
}
char *advance(char *s, char *delim)
{
  char *n = strstr(s, delim);
  if(!n) return s+strlen(s);
  n[0]=0;
  return n+1;
}
void LaddaFaktura(char *fakturanamn)
{
  struct stat sb;
  int bufsize;
  int version2;
  
  stat(fakturanamn, &sb);
  bufsize = sb.st_size+1000;
  FILE *f = fopen(fakturanamn, "r");
  static char *buf = (char *)malloc(bufsize);
  memset(buf, 0, bufsize);
  fread(buf, sb.st_size, 1, f);
  fclose(f);

  if(memcmp(buf, "||VERSION3||", 12) == 0) {
	  version2 = 1;  // may need to introduce a version3 variable in the future
  } else if(memcmp(buf, "||VERSION2||", 12) == 0) {
	  version2 = 1;
  } else {
	  version2 = 0;
  }
  
  if(version2) {
	  namn = buf + 13;
  } else {
  	  namn = buf;
  }
  address   = strstr(namn,     "\n"); address[0]   = 0; address++;
  postnr    = strstr(address,  "\n"); postnr[0]    = 0; postnr++;
  postort   = strstr(postnr,   "\n"); postort[0]   = 0; postort++;
  referens  = strstr(postort,  "\n"); referens[0]  = 0; referens++;
  datum     = strstr(referens, "\n"); datum[0]     = 0; datum++;
  fakturanr = strstr(datum,    "\n"); fakturanr[0] = 0; fakturanr++;
  
  if(version2) {
  	  antaldagar = strstr(fakturanr, "\n"); antaldagar[0] = 0; antaldagar++;
  } else {
	  antaldagar = "30";
  }
  
  char *sbuf;
  if(version2) {
	  sbuf = strstr(antaldagar, "\n");
  } else {
	  sbuf = strstr(fakturanr, "\n");
  }
  sbuf[0] = 0; sbuf++;
  
  memset(specar, 0, 15*sizeof(specifikation));

  for(int i=0; i<15; i++) {
    specar[i].desc = sbuf;
    sbuf = advance(sbuf, "\t");
    specar[i].antal = sbuf;
    sbuf = advance(sbuf, "\t");
    specar[i].pris = sbuf;
    sbuf = advance(sbuf, "\t");
    specar[i].belopp = sbuf;
    sbuf = advance(sbuf, "\n");
  }



  double sum = 0, r;
  for(int i=0; i<15; i++) if(specar[i].belopp) sum += atof(specar[i].belopp);
  r = floor(sum/4.0)*4 - sum;
  sum += r;
  sprintf(summa, "%.2f", sum);
  sprintf(rounding, "%.2f", r);
  sprintf(moms, "%.2f", 0.25*sum);
  sprintf(total, "%.2f", 1.25*sum);
}

int StrWid(HDC dc, const char *s)
{
  SIZE size;
  GetTextExtentPoint32(dc,s,mystrlen(s),&size);
  return size.cx;
}
void TextRight(HDC dc, double x, double y, const char *s)
{
  int wid = StrWid(dc,s);
  TextOut(dc,int(x)-wid,int(y),s,mystrlen(s));
}
void TextLeft(HDC dc, double x, double y, char *s)
{
  TextOut(dc,int(x),int(y),s,mystrlen(s));
}
void TextUL(HDC dc, double x, double y, double l, double fh, char *s)
{
  TextLeft(dc, x, y, s);
  MoveToEx(dc, int(x), int(y+fh)-1, 0);
  LineTo(dc, int(x+l), int(y+fh)-1);
}
void DrawDoubleLine(HDC dc, double x1, double x2, double y, double height)
{
  int thinheight = int(height / 6);
  int skipheight = thinheight + int(height/3);
  int thickheight = int(height - skipheight);

  for(int i=int(y); i<y+height; i++) {
    if(i<y+thinheight || i >= y+skipheight) {
      MoveToEx(dc, int(x1), int(i), NULL);
      LineTo(dc,   int(x2), int(i));
    }
  }
}
void DrawFakt(HDC dc, double S, bool isCopy)
{
  SetViewportExtEx(dc, 84, 84, NULL);

  HFONT font  = CreateFont(int(2.5*S),0     ,0,0, FW_NORMAL,FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
  HFONT specf = CreateFont(int(2.2*S),0     ,0,0, FW_NORMAL,FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
  HFONT bfont = CreateFont(int(2.5*S),0     ,0,0, FW_BOLD,  FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
  HFONT lfont = CreateFont(int(2*S)  ,0     ,0,0, FW_NORMAL,FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_ROMAN,"");
  HFONT hfont = CreateFont(int(4*S)  ,0     ,0,0, FW_BOLD,  FALSE,FALSE,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,CLIP_DEFAULT_PRECIS,PROOF_QUALITY,DEFAULT_PITCH | FF_SWISS,"");
  
  double doubleliney = 18*S;
  HANDLE oldfont = SelectObject(dc, hfont);
  TextLeft(dc,1*S,1*S,"Båt & Inredningssnickerier");
  char *whatText = (char *)(isCopy ? " FAKTURAKOPIA " : " FAKTURA ");
  DrawDoubleLine(dc, 80*S, 83*S, doubleliney, S);
  double whatWid = StrWid(dc, whatText);
  TextLeft(dc, 80*S-whatWid, doubleliney-1.5*S, whatText);
  DrawDoubleLine(dc, S, 80*S-whatWid, doubleliney, S);

  SelectObject(dc, font);
  TextLeft(dc,1*S,5*S,"Svante Höglund");
  TextLeft(dc,1*S,8*S,"Kalkstadsgatan 6");
  TextLeft(dc,1*S,11*S,"386 93 Färjestaden");
  TextLeft(dc,1*S,14*S,"070-544 10 98");

  
  int recty=int(21*S);
  RoundRect(dc, int( 1*S), recty, int(47*S), int(recty+9*S), int(2*S), int(2*S));
  RoundRect(dc, int(48*S), recty, int(83*S), int(recty+9*S), int(2*S), int(2*S));
  SelectObject(dc, lfont);
  int l1=int(recty+1*S), l2=int(recty+3.5*S), l3=int(recty+6*S);
  TextLeft(dc, 2*S,  l1, "Namn:");
  TextLeft(dc, 2*S,  l2, "Adress:");
  TextLeft(dc, 2*S,  l3, "Postadress:");
  TextLeft(dc, 49*S, l1, "Datum:");
  TextLeft(dc, 49*S, l2, "Fakturanr:");
  TextLeft(dc, 49*S, l3, "Er ref:");
  SelectObject(dc, font);
  TextUL(dc, 11*S, l1, 35*S, 2.5*S, namn);
  TextUL(dc, 11*S, l2, 35*S, 2.5*S, address);
  int pnrlen  = mystrlen(postnr);
  int portlen = mystrlen(postort);
  char buf[pnrlen+portlen+2];
  strcpy(buf, postnr);
  strcat(buf, " ");
  strcat(buf, postort);
  TextUL(dc, 11*S, l3, 35*S, 2.5*S, buf);
  TextUL(dc, 57*S, l1, 25*S, 2.5*S, datum);
  TextUL(dc, 57*S, l2, 25*S, 2.5*S, fakturanr);
  TextUL(dc, 57*S, l3, 25*S, 2.5*S, referens);




  double specy = 34*S;
  double speclinespacing = 3.3*S;
  double summay = 88*S;
  double summayend = summay+12*S;
  
  MoveToEx(dc, int( 1*S), int(specy-0.5*S), 0);
  LineTo(dc,   int(83*S), int(specy-0.5*S));
  LineTo(dc,   int(83*S), int(summayend));
  LineTo(dc,   int( 1*S), int(summayend));
  LineTo(dc,   int( 1*S), int(specy-0.5*S));
  MoveToEx(dc, int( 1*S), int(specy+speclinespacing-2), 0);
  LineTo(dc,   int(83*S), int(specy+speclinespacing-2));
  MoveToEx(dc, int(70*S), int(specy-0.5*S), 0);
  LineTo(dc,   int(70*S), int(summayend));
  MoveToEx(dc, int(57*S), int(specy-0.5*S), 0);
  LineTo(dc,   int(57*S), int(summayend));
  MoveToEx(dc, int(44*S), int(specy-0.5*S), 0);
  LineTo(dc,   int(44*S), int(summayend));


  SelectObject(dc, bfont);
  TextLeft(dc,   2*S,  specy, "Specifikation");
  TextRight(dc, 56*S, specy, "Antal");
  TextRight(dc, 69*S, specy, "À pris");
  TextRight(dc, 82*S, specy, "Belopp");
  SelectObject(dc, specf);
  for(int i=0; i<15; i++) {
    double y=specy+speclinespacing*(i+1);
    TextLeft (dc,  2*S, y, specar[i].desc);
    TextRight(dc, 56*S, y, specar[i].antal);
    TextRight(dc, 69*S, y, specar[i].pris);
    TextRight(dc, 82*S, y, specar[i].belopp);
  }


  SelectObject(dc, bfont);
  TextLeft(dc,   2*S,  summay+0*S, "Avrundning");
  TextRight(dc, 82*S, summay+0*S, rounding);
  TextLeft(dc,   2*S,  summay+3*S, "Summa");
  TextRight(dc, 82*S, summay+3*S, summa);
  TextLeft(dc,   2*S,  summay+6*S, "Moms");
  TextRight(dc, 82*S, summay+6*S, moms);
  TextLeft(dc,   2*S,  summay+9*S, "Att betala");
  TextRight(dc, 82*S, summay+9*S, total);


  double legaly = 105*S;
  char *legal1 = "F-skattesedel finns.  Momsreg.nr. 490531-0134";
  char *legal2temp = "Betalningsvillkor: %s dagar netto, dröjsmålsränta 2%%/mån.";
  char legal2[1024];
  char *pg = "Bankgiro 5718-2305";

  sprintf(legal2, legal2temp, antaldagar);
  
  SelectObject(dc, lfont);
  TextLeft (dc,  1*S, legaly, legal1);
  TextRight(dc, 83*S, legaly, legal2);
  DrawDoubleLine(dc, S, 83*S, legaly+3*S, S);
  SelectObject(dc, font);
  TextLeft (dc,  1*S, legaly+5*S, pg);


  SelectObject(dc, oldfont);
  DeleteObject(font);
  DeleteObject(bfont);
  DeleteObject(lfont);
  DeleteObject(hfont);
  DeleteObject(specf);
}
LRESULT CALLBACK StatusProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
  static unsigned int charheight; static int statuslines=5;
  HDC dc;

  switch(uMsg) {
  case WM_CREATE:
    {
      TEXTMETRIC tm;
      dc = GetDC(hwnd);
      GetTextMetrics(dc, &tm);
      charheight=tm.tmHeight+tm.tmExternalLeading;
      ReleaseDC(hwnd, dc);
    }
    break;
  case WM_PAINT:
    {
      PAINTSTRUCT paintStruct;
      RECT clientRect;
      GetClientRect(hwnd, &clientRect);

      dc = BeginPaint(hwnd, &paintStruct);
      if(dc) {
	Rectangle(dc, clientRect.left,clientRect.top,clientRect.right,clientRect.bottom);
	double ws = (clientRect.right-clientRect.left)/84.0;
	double hs = (clientRect.bottom-clientRect.top)/113.0;
	DrawFakt(dc, ws < hs ? ws : hs, false);

	EndPaint(hwnd, &paintStruct);
      }
    }
    break;
  case WM_DESTROY:
    PostQuitMessage(0);
    break;
  default:
    return DefWindowProc(hwnd, uMsg, wParam, lParam);
  }
  return 0;
}
int main(int argc, char **argv)
{
  MSG msg;
  WNDCLASS wndClass;
  HWND mainwindow;

  if(argc!=3) return EXIT_FAILURE;
  LaddaFaktura(argv[2]);

  if(strcmp(argv[1], "PRINT")) {
    if(!0) {
      memset(&wndClass, 0, sizeof(wndClass));
      wndClass.style = CS_HREDRAW | CS_VREDRAW;
      wndClass.lpfnWndProc = StatusProc;
      wndClass.hInstance = 0;
      wndClass.hCursor = LoadCursor(NULL, IDC_ARROW);
      wndClass.hbrBackground = CreateSolidBrush(0x000000);
      wndClass.lpszClassName = "SCSTATUS";
      if(!RegisterClass(&wndClass)) return FALSE;    
    }
    
    mainwindow = CreateWindow("SCSTATUS", "Förhandsgransning - Faktura",
			      WS_OVERLAPPEDWINDOW|WS_CLIPCHILDREN,
			      CW_USEDEFAULT, 0,
			      561, 770,
			      NULL, NULL, 0, NULL);
    
    ShowWindow(mainwindow, SW_SHOW);
    //  UpdateWindow(mainwindow);
    
    while (GetMessage(&msg, NULL, 0, 0)) {
      if(!IsDialogMessage(mainwindow, &msg)) {
	TranslateMessage(&msg);
	DispatchMessage(&msg);
      }
    }
  } else {
    PRINTDLG dlg;
    memset(&dlg, 0, sizeof(PRINTDLG));
    dlg.lStructSize = sizeof(PRINTDLG);
    dlg.Flags = PD_RETURNDC | PD_NOSELECTION;
    dlg.nCopies = 1;
    dlg.nFromPage = 1;
    dlg.nToPage = 2;
    dlg.nMinPage = 1;
    dlg.nMaxPage = 2;
    if(PrintDlg(&dlg) && dlg.hDC) {
      DOCINFO di;
      memset(&di, 0, sizeof(di));
      di.cbSize = sizeof(DOCINFO);
      di.lpszDocName = "Faktura";
      StartDoc(dlg.hDC, &di);
      for(int copy=0; copy<dlg.nCopies; copy++) {
	for(int page=dlg.nFromPage; page<=dlg.nToPage; page++) {
	  SetViewportOrgEx(dlg.hDC, 75*3, 100*3, NULL);
	  StartPage(dlg.hDC);
	  DrawFakt(dlg.hDC, 30, page==2);
	  EndPage(dlg.hDC);
	}
      }
      EndDoc(dlg.hDC);
    }
  }
  return 0;
}
