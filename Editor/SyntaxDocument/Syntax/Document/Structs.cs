using System;

namespace AIMS.Libraries.CodeEditor.Syntax
{
    /// <summary>
    /// Class representing a point in a text.
    /// where x is the column and y is the row.
    /// </summary>
    public class TextPoint
    {
        private int _x = 0;
        private int _y = 0;

        /// <summary>
        /// Event fired when the X or Y property has changed.
        /// </summary>
        public event EventHandler Change = null;

        private void OnChange()
        {
            if (Change != null)
                Change(this, new EventArgs());
        }

        /// <summary>
        /// 
        /// </summary>
        public TextPoint()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public TextPoint(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// 
        /// </summary>
        public int X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnChange();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Y
        {
            get { return _y; }
            set
            {
                _y = value;
                OnChange();
            }
        }
    }
}