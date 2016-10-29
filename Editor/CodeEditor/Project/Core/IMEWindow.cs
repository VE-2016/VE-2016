using System;
using System.Drawing; 
using AIMS.Libraries.CodeEditor.Win32;

namespace AIMS.Libraries.CodeEditor.Core.Globalization
{
    public class IMEWindow
    {
        private const int IMC_SETCOMPOSITIONWINDOW = 0x000c;
        private const int CFS_POINT = 0x0002;
        private const int IMC_SETCOMPOSITIONFONT = 0x000a;
        private const byte FF_MODERN = 48;
        private const byte FIXED_PITCH = 1;
        private IntPtr hIMEWnd;

        #region ctor

        public IMEWindow(IntPtr hWnd, string fontname, float fontsize)
        {
            hIMEWnd = NativeImm32Api.ImmGetDefaultIMEWnd(hWnd);
            SetFont(fontname, fontsize);
        }

        #endregion

        #region PUBLIC PROPERTY FONT

        private Font _Font = null;

        public Font Font
        {
            get { return _Font; }
            set
            {
                if (_Font.Equals(value) == false)
                {
                    SetFont(value);
                    _Font = value;
                }
            }
        }

        public void SetFont(Font font)
        {
            LogFont lf = new LogFont();
            font.ToLogFont(lf);
            lf.lfPitchAndFamily = FIXED_PITCH | FF_MODERN;

            NativeUser32Api.SendMessage(
                hIMEWnd, (int)WindowMessage.WM_IME_CONTROL,
                IMC_SETCOMPOSITIONFONT,
                lf
                );
        }

        public void SetFont(string fontname, float fontsize)
        {
            LogFont tFont = new LogFont();
            tFont.lfItalic = (byte)0;
            tFont.lfStrikeOut = (byte)0;
            tFont.lfUnderline = (byte)0;
            tFont.lfWeight = 400;
            tFont.lfWidth = 0;
            tFont.lfHeight = (int)(-fontsize * 1.3333333333333);
            tFont.lfCharSet = 1;
            tFont.lfPitchAndFamily = FIXED_PITCH | FF_MODERN;
            tFont.lfFaceName = fontname;

            LogFont lf = tFont;

            NativeUser32Api.SendMessage(
                hIMEWnd, (int)WindowMessage.WM_IME_CONTROL,
                IMC_SETCOMPOSITIONFONT,
                lf
                );
        }

        #endregion

        #region PUBLIC PROPERTY LOATION

        private Point _Loation;

        public Point Loation
        {
            get { return _Loation; }
            set
            {
                _Loation = value;

                POINTAPI p = new POINTAPI();
                p.X = value.X;
                p.Y = value.Y;

                COMPOSITIONFORM lParam = new COMPOSITIONFORM();
                lParam.dwStyle = CFS_POINT;
                lParam.ptCurrentPos = p;
                lParam.rcArea = new RECTAPI();

                NativeUser32Api.SendMessage(
                    hIMEWnd,
                    (int)WindowMessage.WM_IME_CONTROL,
                    IMC_SETCOMPOSITIONWINDOW,
                    lParam
                    );
            }
        }

        #endregion
    }
}