using System.Drawing;
using System;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    public class FormatLabelWord : IDisposable
    {
        private Image _Image = null;
        private string _Text = "";
        private int _Width = 0;
        private int _Height = 0;
        private FormatLabelElement _Element = null;
        private Rectangle _ScreenArea = new Rectangle(0, 0, 0, 0);
        //	public bool Link=false;

        public Rectangle ScreenArea
        {
            get { return _ScreenArea; }
            set { _ScreenArea = value; }
        }

        public FormatLabelElement Element
        {
            get { return _Element; }
            set { _Element = value; }
        }

        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        public Image Image
        {
            get { return _Image; }
            set { _Image = value; }
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        public FormatLabelWord()
        {
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (Image != null)
                Image.Dispose();

            if (Element != null)
                Element.Dispose();
        }

        #endregion
    }
}