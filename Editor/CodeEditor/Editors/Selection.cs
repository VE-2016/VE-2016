//Coded by Rajneesh Noonia 2007

using System;
using System.Drawing;
using AIMS.Libraries.CodeEditor.Syntax;
using System.ComponentModel;

using System.Windows.Forms;
using System.Collections.Generic;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Selection class used by the SyntaxBoxControl
    /// </summary>

    public class Selection
    {
        /// <summary>
        /// Event fired when the selection has changed.
        /// </summary>
        public event EventHandler Change = null;

        #region Instance constructors

        /// <summary>
        /// Selection Constructor.
        /// </summary>
        /// <param name="control">Control that will use this selection</param>
        public Selection(EditViewControl control)
        {
            _control = control;
            this.Bounds = new TextRange();
        }

        #endregion Instance constructors

        #region Public instance properties

        /// <summary>
        /// Gets the text of the active selection
        /// </summary>
        public String Text
        {
            get
            {
                if (!this.IsValid)
                {
                    return "";
                }
                else
                {
                    return _control.Document.GetRange(this.LogicalBounds);
                }
            }
            set
            {
                if (this.Text == value) return;

                //selection text bug fix 
                //
                //selection gets too short if \n is used instead of newline
                string tmp = value.Replace(Environment.NewLine, "\n");
                tmp = tmp.Replace("\n", Environment.NewLine);
                value = tmp;
                //---


                TextPoint oCaretPos = _control.Caret.Position;
                int nCaretX = oCaretPos.X;
                int nCaretY = oCaretPos.Y;
                _control.Document.StartUndoCapture();
                this.DeleteSelection();
                _control.Document.InsertText(value, oCaretPos.X, oCaretPos.Y);
                this.SelLength = value.Length;
                if (nCaretX != oCaretPos.X || nCaretY != oCaretPos.Y)

                {
                    _control.Caret.Position = new TextPoint(this.Bounds.LastColumn, this.Bounds.LastRow);
                }

                _control.Document.EndUndoCapture();
                _control.Document.InvokeChange();
            }
        }

        /// <summary>
        /// Returns the normalized positions of the selection.
        /// Swapping start and end values if the selection is reversed.
        /// </summary>
        public TextRange LogicalBounds
        {
            get
            {
                TextRange r = new TextRange();
                if (this.Bounds.FirstRow < this.Bounds.LastRow)
                {
                    return this.Bounds;
                }
                else if (this.Bounds.FirstRow == this.Bounds.LastRow && this.Bounds.FirstColumn < this.Bounds.LastColumn)
                {
                    return this.Bounds;
                }
                else
                {
                    r.FirstColumn = this.Bounds.LastColumn;
                    r.FirstRow = this.Bounds.LastRow;
                    r.LastColumn = this.Bounds.FirstColumn;
                    r.LastRow = this.Bounds.FirstRow;
                    return r;
                }
            }
        }

        /// <summary>
        /// Returns true if the selection contains One or more chars
        /// </summary>
        public bool IsValid
        {
            get
            {
                return (this.LogicalBounds.FirstColumn != this.LogicalBounds.LastColumn ||
                    this.LogicalBounds.FirstRow != this.LogicalBounds.LastRow);
            }
        }

        /// <summary>
        /// gets or sets the length of the selection in chars
        /// </summary>
        public int SelLength
        {
            get
            {
                TextPoint p1 = new TextPoint(this.Bounds.FirstColumn, this.Bounds.FirstRow);
                TextPoint p2 = new TextPoint(this.Bounds.LastColumn, this.Bounds.LastRow);
                int i1 = _control.Document.PointToIntPos(p1);
                int i2 = _control.Document.PointToIntPos(p2);
                return i2 - i1;
            }
            set { this.SelEnd = this.SelStart + value; }
        }

        /// <summary>
        /// Gets or Sets the Selection end as an index in the document text.
        /// </summary>
        public int SelEnd
        {
            get
            {
                TextPoint p = new TextPoint(this.Bounds.LastColumn, this.Bounds.LastRow);
                return _control.Document.PointToIntPos(p);
            }
            set
            {
                TextPoint p = _control.Document.IntPosToPoint(value);
                this.Bounds.LastColumn = p.X;
                this.Bounds.LastRow = p.Y;
            }
        }


        /// <summary>
        /// Gets or Sets the Selection start as an index in the document text.
        /// </summary>
        public int SelStart
        {
            get
            {
                TextPoint p = new TextPoint(this.Bounds.FirstColumn, this.Bounds.FirstRow);
                return _control.Document.PointToIntPos(p);
            }
            set
            {
                TextPoint p = _control.Document.IntPosToPoint(value);
                this.Bounds.FirstColumn = p.X;
                this.Bounds.FirstRow = p.Y;
            }
        }

        /// <summary>
        /// Gets or Sets the logical Selection start as an index in the document text.
        /// </summary>
        public int LogicalSelStart
        {
            get
            {
                TextPoint p = new TextPoint(this.LogicalBounds.FirstColumn, this.LogicalBounds.FirstRow);
                return _control.Document.PointToIntPos(p);
            }
            set
            {
                TextPoint p = _control.Document.IntPosToPoint(value);
                this.Bounds.FirstColumn = p.X;
                this.Bounds.FirstRow = p.Y;
            }
        }

        #endregion Public instance properties

        #region Public instance methods

        /// <summary>
        /// Indent the active selection one step.
        /// </summary>
        public void Indent()
        {
            if (!this.IsValid)
                return;

            Row xtr = null;
            UndoBlockCollection ActionGroup = new UndoBlockCollection();
            for (int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++)
            {
                xtr = _control.Document[i];
                xtr.Text = "\t" + xtr.Text;
                UndoBlock b = new UndoBlock();
                b.Action = UndoAction.InsertRange;
                b.Text = "\t";
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
            }
            if (ActionGroup.Count > 0)
                _control.Document.AddToUndoList(ActionGroup);
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            _control.Caret.Position.X = this.LogicalBounds.LastColumn;
            _control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }

        public string[] Outdents()
        {
            int X = _control.Caret.Position.X;
            int Y = _control.Caret.Position.Y;

            string[] s = GetWord(X, Y);//"(" + Control.Caret.Position.X + "," + Control.Caret.Position.Y + ")";

            return s;
        }


        public int GetCursorLine()
        {
            return _control.Caret.Position.Y;
        }

        public Point GetCursor()
        {
            return new Point(_control.Caret.Position.X, _control.Caret.Position.Y);
        }


        public Word getCaretWord()
        {
            Row row = _control.Document[_control.Caret.Position.Y];



            Word w = row.GetCaretWord(_control.Caret.Position.X);

            return w;
        }

        

        public string GetCaretWord()
        {
            Row row = _control.Document[_control.Caret.Position.Y];



            Word w = row.GetCaretWord(_control.Caret.Position.X);

            if (w != null)
                return w.Text;
            else return "";
        }
        public string[] GetPrevCaretWord()
        {
            Row row = _control.Document[_control.Caret.Position.Y];



            Word w = row.GetCaretWord(_control.Caret.Position.X);


            if (w == null)
                return null;

            string[] s = row.Words(w.Text);


            return s;
        }
        public List<string> GetCaretWords()
        {
            Row row = _control.Document[_control.Caret.Position.Y];

            Word w = row.GetCaretWord(_control.Caret.Position.X);

            return row.GetWords(w);

        }
        public string GetCaretString()
        {
            Row row = _control.Document[_control.Caret.Position.Y];

            return row.Text;

        }
        public void AppendToCaretString(string text)
        {
            Row row = _control.Document[_control.Caret.Position.Y];

            row.Text += text;

        }
        public Word CaretWord()
        {
            Row row = _control.Document[_control.Caret.Position.Y];

            Word w = row.GetCaretWord(_control.Caret.Position.X);
            
            return w;
        }

        public bool IsCaretInWord()
        {
            int X = _control.Caret.Position.X;

            Word w = CaretWord();

            int s = w.Text.IndexOf("(");
            int e = w.Text.IndexOf(")");

            if (w.Column + s <= X)
                if (w.Column + e >= X)
                    return true;

            return false;
        }
        public string[] GetWord(int X, int Y)
        {
            Row row = _control.Document[Y];



            Word w = row.GetCaretWord(X);


            string[] words = row.Words(w.Text);

            return words;

            //if (w != null)
            //    return w.Text;
            //else return "";
        }


        /// <summary>
        /// Outdent the active selection one step
        /// </summary>
        public void Outdent()
        {
            if (!this.IsValid)
                return;

            Row xtr = null;
            UndoBlockCollection ActionGroup = new UndoBlockCollection();
            for (int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++)
            {
                xtr = _control.Document[i];
                UndoBlock b = new UndoBlock();
                b.Action = UndoAction.DeleteRange;
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
                string s = xtr.Text;
                if (s.StartsWith("\t"))
                {
                    b.Text = s.Substring(0, 1);
                    s = s.Substring(1);
                }
                if (s.StartsWith("    "))
                {
                    b.Text = s.Substring(0, 4);
                    s = s.Substring(4);
                }
                xtr.Text = s;
            }
            if (ActionGroup.Count > 0)
                _control.Document.AddToUndoList(ActionGroup);
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            _control.Caret.Position.X = this.LogicalBounds.LastColumn;
            _control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }


        public void Indent(string Pattern)
        {
            if (!this.IsValid)
                return;

            Row xtr = null;
            UndoBlockCollection ActionGroup = new UndoBlockCollection();
            for (int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++)
            {
                xtr = _control.Document[i];
                xtr.Text = Pattern + xtr.Text;
                UndoBlock b = new UndoBlock();
                b.Action = UndoAction.InsertRange;
                b.Text = Pattern;
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
            }
            if (ActionGroup.Count > 0)
                _control.Document.AddToUndoList(ActionGroup);
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            _control.Caret.Position.X = this.LogicalBounds.LastColumn;
            _control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }

        /// <summary>
        /// Outdent the active selection one step
        /// </summary>
        public void Outdent(string Pattern)
        {
            if (!this.IsValid)
                return;

            Row xtr = null;
            UndoBlockCollection ActionGroup = new UndoBlockCollection();
            for (int i = this.LogicalBounds.FirstRow; i <= this.LogicalBounds.LastRow; i++)
            {
                xtr = _control.Document[i];
                UndoBlock b = new UndoBlock();
                b.Action = UndoAction.DeleteRange;
                b.Position.X = 0;
                b.Position.Y = i;
                ActionGroup.Add(b);
                string s = xtr.Text;
                if (s.StartsWith(Pattern))
                {
                    b.Text = s.Substring(0, Pattern.Length);
                    s = s.Substring(Pattern.Length);
                }
                xtr.Text = s;
            }
            if (ActionGroup.Count > 0)
                _control.Document.AddToUndoList(ActionGroup);
            this.Bounds = this.LogicalBounds;
            this.Bounds.FirstColumn = 0;
            this.Bounds.LastColumn = xtr.Text.Length;
            _control.Caret.Position.X = this.LogicalBounds.LastColumn;
            _control.Caret.Position.Y = this.LogicalBounds.LastRow;
        }

        /// <summary>
        /// Delete the active selection.
        /// <seealso cref="ClearSelection"/>
        /// </summary>
        public void DeleteSelection()
        {
            TextRange r = this.LogicalBounds;

            int x = r.FirstColumn;
            int y = r.FirstRow;
            _control.Document.DeleteRange(r);
            _control.Caret.Position.X = x;
            _control.Caret.Position.Y = y;
            ClearSelection();
            _control.ScrollIntoView();
        }

        /// <summary>
        /// Clear the active selection
        /// <seealso cref="DeleteSelection"/>
        /// </summary>
        public void ClearSelection()
        {
            //    if(!(Bounds.FirstColumn == Bounds.LastColumn && Bounds.FirstRow == Bounds.LastRow))
            //  {
            //   Bounds = new TextRange(Control.Caret.Position.X, Control.Caret.Position.Y, Control.Caret.Position.X, Control.Caret.Position.Y);
            Bounds.FirstColumn = _control.Caret.Position.X;
            Bounds.FirstRow = _control.Caret.Position.Y;
            Bounds.LastColumn = _control.Caret.Position.X;
            Bounds.LastRow = _control.Caret.Position.Y;
            //}
        }

        /// <summary>
        /// Make a selection from the current selection start to the position of the caret
        /// </summary>
        public void MakeSelection()
        {
            Bounds.LastColumn = _control.Caret.Position.X;
            Bounds.LastRow = _control.Caret.Position.Y;
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        public void SelectAll()
        {
            Bounds.FirstColumn = 0;
            Bounds.FirstRow = 0;
            Bounds.LastColumn = _control.Document[_control.Document.Count - 1].Text.Length;
            Bounds.LastRow = _control.Document.Count - 1;
            _control.Caret.Position.X = Bounds.LastColumn;
            _control.Caret.Position.Y = Bounds.LastRow;
            _control.ScrollIntoView();
        }

        #endregion Public instance methods

        #region Public instance fields

        /// <summary>
        /// The bounds of the selection
        /// </summary>
        /// 
        private TextRange _Bounds;

        public TextRange Bounds
        {
            get { return _Bounds; }
            set
            {
                if (_Bounds != null)
                {
                    _Bounds.Change -= new EventHandler(this.Bounds_Change);
                }

                _Bounds = value;
                _Bounds.Change += new EventHandler(this.Bounds_Change);
                OnChange();
            }
        }

        private void Bounds_Change(object s, EventArgs e)
        {
            OnChange();
        }

        #endregion Public instance fields

        #region Protected instance fields

        private EditViewControl _control;

        #endregion Protected instance fields

        private void PositionChange(object s, EventArgs e)
        {
            OnChange();
        }

        private void OnChange()
        {
            if (Change != null)
                Change(this, null);
        }
    }
}