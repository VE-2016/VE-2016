//Coded by Rajneesh Noonia 2007

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.Core;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms.TextDraw;
using AIMS.Libraries.CodeEditor.Core.Globalization;
using System.Globalization;
using System.Collections;
using ICSharpCode.NRefactory.CSharp;

namespace AIMS.Libraries.CodeEditor.WinForms.Painter
{
    /// <summary>
    /// Painter class that uses GDI32 to render the content of a SyntaxBoxControl
    /// </summary>
    public class Painter_GDI : IPainter
    {
        private int _firstSpanRow = 0;
        private int _lastSpanRow = 0;
        private bool _spanFound = false;

        private int _lastRow = 0;
        private int _yOffset = 0;
        private EditViewControl _control;
        private RenderItems _GFX = new RenderItems();
        private int _resizeCount = 0;

        private Word _bracketStart,_bracketEnd;

        /// <summary>
        /// Implementation of the IPainter Resize method
        /// </summary>
        public void Resize()
        {
            _resizeCount++;
            this.InitGraphics();
            //	Console.WriteLine ("painterresize {0} {1}",ResizeCount,Control.Name);

        }

        /// <summary>
        /// Painter_GDI constructor.
        /// </summary>
        /// <param name="control">The control that will use the Painter</param>
        public Painter_GDI(EditViewControl control)
        {
            _control = control;
            InitGraphics();
        }

        /// <summary>
        /// Implementation of the IPainter GetDrawingPos method
        /// </summary>
        /// <param name="tp">Input TextPoint</param>
        /// <returns>Drawing corrdinates in Point</returns>
        /// 
        public Point GetDrawingPos(TextPoint tp)
        {
            return GetTextPointPixelPos(tp);
        }
        /// <summary>
        /// Implementation of the IPainter MeasureString method
        /// </summary>
        /// <param name="s">String to measure</param>
        /// <returns>Size of the string in pixels</returns>
        public Size MeasureString(string s)
        {
            //try
            //{
            _GFX.StringBuffer.Font = _GFX.FontNormal;
            return _GFX.StringBuffer.MeasureTabbedString(s, _control.TabSize);
            //}
            //catch
            //{
            //    return new Size(0, 0);
            //}
        }

        /// <summary>
        /// Implementation of the IPainter InitGraphics method.
        /// Initializes GDI32 backbuffers and brushes.
        /// </summary>
        public void InitGraphics()
        {
            if (_GFX.BackgroundBrush != null)
                _GFX.BackgroundBrush.Dispose();

            if (_GFX.GutterMarginBrush != null)
                _GFX.GutterMarginBrush.Dispose();

            if (_GFX.LineNumberMarginBrush != null)
                _GFX.LineNumberMarginBrush.Dispose();

            if (_GFX.HighLightLineBrush != null)
                _GFX.HighLightLineBrush.Dispose();

            if (_GFX.LineNumberMarginBorderBrush != null)
                _GFX.LineNumberMarginBorderBrush.Dispose();

            if (_GFX.GutterMarginBorderBrush != null)
                _GFX.GutterMarginBorderBrush.Dispose();

            if (_GFX.OutlineBrush != null)
                _GFX.OutlineBrush.Dispose();


            _GFX.BackgroundBrush = new GDIBrush(_control.BackColor);
            _GFX.GutterMarginBrush = new GDIBrush(_control.GutterMarginColor);
            _GFX.LineNumberMarginBrush = new GDIBrush(_control.LineNumberBackColor);
            _GFX.HighLightLineBrush = new GDIBrush(_control.HighLightedLineColor);
            _GFX.LineNumberMarginBorderBrush = new GDIBrush
                (_control.LineNumberBorderColor);
            _GFX.GutterMarginBorderBrush = new GDIBrush
                (_control.GutterMarginBorderColor);
            _GFX.OutlineBrush = new GDIBrush(_control.OutlineColor);


            if (_GFX.FontNormal != null)
                _GFX.FontNormal.Dispose();

            if (_GFX.FontBold != null)
                _GFX.FontBold.Dispose();

            if (_GFX.FontItalic != null)
                _GFX.FontItalic.Dispose();

            if (_GFX.FontBoldItalic != null)
                _GFX.FontBoldItalic.Dispose();

            if (_GFX.FontUnderline != null)
                _GFX.FontUnderline.Dispose();

            if (_GFX.FontBoldUnderline != null)
                _GFX.FontBoldUnderline.Dispose();

            if (_GFX.FontItalicUnderline != null)
                _GFX.FontItalicUnderline.Dispose();

            if (_GFX.FontBoldItalicUnderline != null)
                _GFX.FontBoldItalicUnderline.Dispose();


            //	string font="courier new";
            string font = _control.FontName;
            float fontsize = _control.FontSize;
            _GFX.FontNormal = new GDIFont(font, fontsize, false, false, false, false)
                ;
            _GFX.FontBold = new GDIFont(font, fontsize, true, false, false, false);
            _GFX.FontItalic = new GDIFont(font, fontsize, false, true, false, false);
            _GFX.FontBoldItalic = new GDIFont(font, fontsize, true, true, false,
                                             false);
            _GFX.FontUnderline = new GDIFont(font, fontsize, false, false, true,
                                            false);
            _GFX.FontBoldUnderline = new GDIFont(font, fontsize, true, false, true,
                                                false);
            _GFX.FontItalicUnderline = new GDIFont(font, fontsize, false, true, true,
                                                  false);
            _GFX.FontBoldItalicUnderline = new GDIFont(font, fontsize, true, true,
                                                      true, false);

            this.InitIMEWindow();



            //			try
            //			{

            if (_control != null)
            {
                if (_control.IsHandleCreated)
                {
                    if (_GFX.StringBuffer != null)
                        _GFX.StringBuffer.Dispose();

                    if (_GFX.SelectionBuffer != null)
                        _GFX.SelectionBuffer.Dispose();

                    if (_GFX.BackBuffer != null)
                        _GFX.BackBuffer.Dispose();

                    _GFX.StringBuffer = new GDISurface(1, 1, _control, true);
                    _GFX.StringBuffer.Font = _GFX.FontNormal;
                    int h = _GFX.StringBuffer.MeasureTabbedString("ABC", 0).Height +
                        _control._CodeEditor.RowPadding;
                    _GFX.BackBuffer = new GDISurface(_control.ClientWidth, h, _control, true)
                        ;
                    _GFX.BackBuffer.Font = _GFX.FontNormal;

                    _GFX.SelectionBuffer = new GDISurface(_control.ClientWidth, h, _control,
                                                         true);
                    _GFX.SelectionBuffer.Font = _GFX.FontNormal;

                    _control.View.RowHeight = _GFX.BackBuffer.MeasureTabbedString("ABC", 0)
                        .Height + _control._CodeEditor.RowPadding;
                    _control.View.CharWidth = _GFX.BackBuffer.MeasureTabbedString(" ", 0)
                        .Width;
                }
                else
                {
                    //		System.Windows.Forms.MessageBox.Show ("no handle");

                }
            }
            //			}
            //			catch
            //			{
            //			}
        }

        private void InitIMEWindow()
        {
            if (_control.IMEWindow != null)
                _control.IMEWindow.SetFont(_control.FontName, _control.FontSize);
        }


        private Size MeasureRow(Row xtr, int Count, int OffsetX)
        {
            int width = 0;
            int taborig = -_control.View.FirstVisibleColumn * _control.View.CharWidth
                + _control.View.TextMargin;
            int xpos = _control.View.TextMargin - _control.View.ClientAreaStart;
            if (xtr.InQueue)
            {
                SetStringFont(false, false, false);
                int Padd = Math.Max(Count - xtr.Text.Length, 0);
                string PaddStr = new String(' ', Padd);
                string TotStr = xtr.Text + PaddStr;
                width = _GFX.StringBuffer.MeasureTabbedString(TotStr.Substring(0, Count),
                                                             _control.PixelTabSize).Width;
            }
            else
            {
                int CharNo = 0;
                int TotWidth = 0;
                int CharPos = 0;
                foreach (Word w in xtr.FormattedWords)
                {
                    if (w.Type == WordType.xtWord && w.Style != null)
                        SetStringFont(w.Style.Bold, w.Style.Italic, w.Style.Underline);
                    else
                        SetStringFont(false, false, false);

                    if (w.Text.Length + CharNo >= Count || w ==
                        xtr.FormattedWords[xtr.FormattedWords.Count - 1])
                    {
                        CharPos = Count - CharNo;
                        int MaxChars = Math.Min(CharPos, w.Text.Length);
                        if (MaxChars < 0) break;
                        TotWidth += _GFX.StringBuffer.DrawTabbedString(w.Text.Substring(0,
                                                                                       MaxChars), xpos + TotWidth, 0, taborig, _control.PixelTabSize)
                            .Width;
                        width = TotWidth;
                        break;
                    }
                    else
                    {
                        TotWidth += _GFX.StringBuffer.DrawTabbedString(w.Text, xpos +
                            TotWidth, 0, taborig, _control.PixelTabSize).Width;
                        CharNo += w.Text.Length;
                    }
                }

                SetStringFont(false, false, false);
                int Padd = Math.Max(Count - xtr.Text.Length, 0);
                string PaddStr = new String(' ', Padd);
                width += _GFX.StringBuffer.DrawTabbedString(PaddStr, xpos + TotWidth, 0,
                                                           taborig, _control.PixelTabSize).Width;
            }


            return new Size(width, 0);

            //	return GFX.BackBuffer.MeasureTabbedString (xtr.Text.Substring (0,Count),Control.PixelTabSize);
        }

        /// <summary>
        /// Implementation of the IPainter MeasureRow method.
        /// </summary>
        /// <param name="xtr">Row to measure</param>
        /// <param name="Count">Last char index</param>
        /// <returns>The size of the row in pixels</returns>
        public Size MeasureRow(Row xtr, int Count)
        {
            return MeasureRow(xtr, Count, 0);
        }

        /// <summary>
        /// Implementation of the IPainter RenderAll method.
        /// </summary>
        public void RenderAll()
        {
            //
            _control.View.RowHeight = _GFX.BackBuffer.MeasureString("ABC").Height;
            _control.View.CharWidth = _GFX.BackBuffer.MeasureString(" ").Width;


            _control.InitVars();

            Graphics g = _control.CreateGraphics();

            

            RenderAll(g);

            g.Dispose();
        }
        /// <summary>
        /// Implementation of the IPainter RenderAll method.
        /// </summary>
        public void RenderRows(int index)
        {
            //
            _control.View.RowHeight = _GFX.BackBuffer.MeasureString("ABC").Height;
            _control.View.CharWidth = _GFX.BackBuffer.MeasureString(" ").Width;


            _control.InitVars();

            Graphics g = _control.CreateGraphics();

            _yOffset = _control.View.YOffset;

            RenderRow(index, 0);

            //RenderAll(g);

            g.Dispose();
        }

        private void SetBrackets()
        {
            Segment CurrentSegment = null;
            _bracketEnd = null;
            _bracketStart = null;

            Word CurrWord = _control.Caret.CurrentWord;
            if (CurrWord != null)
            {
                CurrentSegment = CurrWord.Segment;
                if (CurrentSegment != null)
                {
                    if (CurrWord == CurrentSegment.StartWord || CurrWord ==
                        CurrentSegment.EndWord)
                    {
                        if (CurrentSegment.EndWord != null)
                        {
                            //	if(w!=null)
                            //	{
                            _bracketEnd = CurrentSegment.EndWord;
                            _bracketStart = CurrentSegment.StartWord;
                            //	}
                        }
                    }

                    //ROGER STÖDA HÄR!!!

                    //try
                    //{
                    if (CurrWord == null || CurrWord.Pattern == null)
                        return;

                    if (CurrWord.Pattern.BracketType == BracketType.EndBracket)
                    {
                        Word w = _control.Document.GetStartBracketWord(CurrWord,
                                                                           CurrWord.Pattern.MatchingBracket, CurrWord.Segment);
                        _bracketEnd = CurrWord;
                        _bracketStart = w;
                    }
                    if (CurrWord.Pattern.BracketType == BracketType.StartBracket)
                    {
                        Word w = _control.Document.GetEndBracketWord(CurrWord,
                                                                         CurrWord.Pattern.MatchingBracket, CurrWord.Segment);

                        //	if(w!=null)
                        //	{
                        _bracketEnd = w;
                        _bracketStart = CurrWord;
                        //	}
                    }
                    //}
                    //catch
                    //{
                    //    //	System.Windows.Forms.MessageBox.Show (x.Message + "\n\n\n" + x.StackTrace);
                    //}
                }
            }
        }

        private void SetSpanIndicators()
        {
            _spanFound = false;
            //try
            //{
            Segment s = _control.Caret.CurrentSegment();

            if (s == null || s.StartWord == null || s.StartWord.Row == null ||
                s.EndWord == null || s.EndWord.Row == null)
                return;

            _firstSpanRow = s.StartWord.Row.Index;
            _lastSpanRow = s.EndWord.Row.Index;
            _spanFound = true;
            //}
            //catch
            //{
            //}
        }

        /// <summary>
        /// Implementation of the IPainter RenderCaret method
        /// </summary>
        /// <param name="g"></param>
        private bool _renderCaretRowOnly = false;

        public void RenderCaret(Graphics g)
        {
            _renderCaretRowOnly = true;
            RenderAll(g);
            _renderCaretRowOnly = false;
        }

        /// <summary>
        /// Implementation of the IPainter RenderAll method
        /// </summary>
        /// <param name="g">Target Graphics object</param>
        public void RenderAll(Graphics g)
        {
            word = _control.Selection.GetCaretWord();
            _control.InitVars();
            _control.InitScrollbars();
            SetBrackets();
            SetSpanIndicators();
            int j = _control.View.FirstVisibleRow;

            int diff = j - _lastRow;
            _lastRow = j;
            if (_control.SmoothScroll)
            {
                if (diff == 1)
                {
                    for (int i = _control.View.RowHeight; i > 0; i -=
                        _control.SmoothScrollSpeed)
                    {
                        _yOffset = i + _control.View.YOffset;
                        RenderAll2(g);
                        g.Flush();
                        //BOO Thread.Sleep(0);
                    }
                }
                else if (diff == -1)
                {
                    for (int i = -_control.View.RowHeight; i < 0; i +=
                        _control.SmoothScrollSpeed)
                    {
                        _yOffset = i + _control.View.YOffset;
                        RenderAll2(g);
                        g.Flush();
                        //BOO Thread.Sleep(0);
                    }
                }
            }

            _yOffset = _control.View.YOffset;
            RenderAll2(g);
            //g.Flush ();
            //System.Threading.Thread.Sleep (0);
        }

        private void RenderAll2(Graphics g)
        {



            int j = _control.View.FirstVisibleRow;

            Row r = null;


            if (_control.AutoListStartPos != null)
            {
                if (_control.AutoListVisible)
                {
                    Point alP = GetTextPointPixelPos(_control.AutoListStartPos);
                    if (alP == new Point(-1, -1))
                    {
                        _control.AutoList.Visible = false;
                    }
                    else
                    {
                        alP.Y += _control.View.RowHeight + 2;
                        alP.X += -20;
                        alP = _control.PointToScreen(alP);

                        Screen screen =
                            Screen.FromPoint(new Point
                                (_control.Right, alP.Y));

                        if (alP.Y + _control.AutoList.Height > screen.WorkingArea.Height)
                        {
                            alP.Y -= _control.View.RowHeight + 2 + _control.AutoList.Height;
                        }

                        if (alP.X + _control.AutoList.Width > screen.WorkingArea.Width)
                        {
                            alP.X -= alP.X + _control.AutoList.Width -
                                screen.WorkingArea.Width;
                        }


                        _control.AutoList.Location = alP;
                        _control.AutoList.Visible = true;
                        //Control.Controls[0].Focus();
                        _control.Focus();
                    }
                }
                else
                {
                }
            }

            //if (Control.InfoTipStartPos != null)
            //{

            //        if (Control.InfoTipVisible)
            //        {
            //            Point itP = GetTextPointPixelPos(Control.InfoTipStartPos);
            //            if (itP == new Point(-1, -1))
            //            {
            //                Control.InfoTip.Visible = false;
            //            }
            //            else
            //            {
            //                itP.Y += Control.View.RowHeight + 2;
            //                itP.X += -20;

            //                itP = Control.PointToScreen(itP);

            //                Screen screen =
            //                    Screen.FromPoint(new Point
            //                        (this.Control.Right, itP.Y));

            //                if (itP.Y + Control.InfoTip.Height > screen.WorkingArea.Height)
            //                {
            //                    itP.Y -= Control.View.RowHeight + 2 + Control.InfoTip.Height;
            //                }

            //                if (itP.X + Control.InfoTip.Width > screen.WorkingArea.Width)
            //                {
            //                    itP.X -= itP.X + Control.InfoTip.Width -
            //                        screen.WorkingArea.Width;
            //                }


            //                Control.InfoTip.Location = itP;
            //                Control.InfoTip.Visible = true;
            //               // System.Diagnostics.Debug.WriteLine("Infotip Made Visible");
            //            }
            //        }
            //        else
            //        {
            //            Control.InfoTip.Visible = false;
            //            //System.Diagnostics.Debug.WriteLine("Infotip Made Invisible");
            //        }
            //}


            VSParsers.CSParsers csd = _control.CodeEditor.csd;


            if (_control.NeededTypeRedaw == true)
            if (csd != null)
            {
                Color c = Color.FromArgb(0, 155, 155); // Color.FromArgb(222, 225, 231);
                SyntaxDocument d = _control.Document;
                ArrayList E = csd.ResolveAtTypes(_control.Document.Text, "filename");
                foreach(AstNode node in E)
                {

                    if (node.Role.ToString() == "Import")
                        continue;
                    if (node.Role.ToString() == "Target")
                        continue;
                    if (node.GetType() == typeof(ICSharpCode.NRefactory.CSharp.PrimitiveType))
                        continue;

                    TextPoint p = null;
                    if (node.Role.ToString() == "BaseType")
                    {
                        continue;
                    }

                   else if (node.GetType() == typeof(ICSharpCode.NRefactory.CSharp.TypeDeclaration))
                    {
                        TypeDeclaration dd = node as TypeDeclaration;
                        p = new TextPoint(dd.NameToken.StartLocation.Column - 1, dd.NameToken.StartLocation.Line - 1);
                    }
                    else if (node.GetType() == typeof(ICSharpCode.NRefactory.CSharp.ConstructorDeclaration))
                    {
                        ConstructorDeclaration dd = node as ConstructorDeclaration;
                        p = new TextPoint(dd.NameToken.StartLocation.Column - 1, dd.NameToken.StartLocation.Line - 1);
                    }
                    else if (node.GetType() == typeof(ICSharpCode.NRefactory.CSharp.MemberType))
                    {
                        MemberType dd = node as MemberType;
                        p = new TextPoint(dd.Target.StartLocation.Column - 1, dd.Target.StartLocation.Line - 1);
                        Word ww = d.GetFormatWordFromPos(p);
                        ww.Style = new TextStyle();
                        ww.Style.ForeColor = Color.DarkGreen;

                        p = new TextPoint(dd.MemberNameToken.StartLocation.Column - 1, dd.MemberNameToken.StartLocation.Line - 1);
                    }
                    else
                    
                        p = new TextPoint(node.StartLocation.Column - 1, node.StartLocation.Line -1);

                    Word w = d.GetFormatWordFromPos(p);

                    if (w.Style.Name != "Keywords")
                    {

                        w.Style = new TextStyle();

                        w.Style.ForeColor = c;

                    }

                }
            }

            for (int i = 0; i < _control.View.VisibleRowCount; i++)
            {
                if (j >= 0 && j < _control.Document.VisibleRows.Count)
                {
                    r = _control.Document.VisibleRows[j];
                    if (_renderCaretRowOnly)
                    {
                        if (r == _control.Caret.CurrentRow)
                        {
                            // if(Control.hovered == false)
                            //     RenderRow(g, Control.Document.IndexOf(r), i);
                        }
                        //Control.Caret.CurrentRow.Expansion_EndSegment.StartRow.Index
                        if (_control.Caret.CurrentRow.Expansion_EndSegment != null &&
                            _control.Caret.CurrentRow.Expansion_EndSegment.StartRow !=
                                null &&
                            _control.Caret.CurrentRow.Expansion_EndSegment.StartRow == r)
                        {
                            RenderRow(g, _control.Document.IndexOf(r), i);
                        }
                    }
                    else
                    {
                        RenderRow(g, _control.Document.IndexOf(r), i);
                    }
                }
                else
                {
                    if (_renderCaretRowOnly)
                    {
                    }
                    else
                    {
                        RenderRow(g, _control.Document.Count, i);
                    }
                }
                j++;
            }
            _control.NeededTypeRedaw = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RowIndex"></param>
        public void RenderRow(int RowIndex)
        {
            RenderRow(RowIndex, 10);
        }

        private void RenderRow(int RowIndex, int RowPos)
        {
            using (Graphics g = _control.CreateGraphics())
            {
                RenderRow(g, RowIndex, RowPos);
            }
        }

        public static Color hc = Color.FromArgb(255, 242, 242, 242);

        private void RenderRow(Graphics g, int RowIndex, int RowPos)
        {
            //		if (RowIndex ==-1)
            //			System.Diagnostics.Debugger.Break ();

            if (RowIndex >= 0 && RowIndex < _control.Document.Count)
            {
                //do keyword parse before we render the line...
                if (_control.Document[RowIndex].RowState == RowState.SegmentParsed)
                {
                    _control.Document.Parser.ParseLine(RowIndex, true);
                    _control.Document[RowIndex].RowState = RowState.AllParsed;
                }
            }


            GDISurface bbuff = _GFX.BackBuffer;
            bool found = false;


            GDIBrush bg = _GFX.BackgroundBrush;


            if (RowIndex < _control.Document.Count && RowIndex >= 0)
            {
                Row r = _control.Document[RowIndex];
                if (_spanFound && RowIndex >= _firstSpanRow && RowIndex <=
                    _lastSpanRow && _control._CodeEditor.ScopeBackColor !=
                        Color.Transparent)
                {
                    bg = new GDIBrush(_control._CodeEditor.ScopeBackColor);
                    found = true;
                }
                else if (r.BackColor != Color.Transparent)
                {
                    bg = new GDIBrush(r.BackColor);
                    found = true;
                }
                else
                {
                    if (r.EndSegment != null)
                    {
                        Segment tmp = null;
                        tmp = r.Expansion_EndSegment;
                        while (tmp != null)
                        {
                            if (tmp.BlockType.Transparent == false)
                            {
                                bg = new GDIBrush(tmp.BlockType.BackColor);
                                found = true;
                                break;
                            }
                            tmp = tmp.Parent;
                        }


                        if (!found)
                        {
                            tmp = r.EndSegment;
                            while (tmp != null)
                            {
                                if (tmp.BlockType.Transparent == false)
                                {
                                    bg = new GDIBrush(tmp.BlockType.BackColor);
                                    found = true;
                                    break;
                                }
                                tmp = tmp.Parent;
                            }
                        }
                        if (!found)
                        {
                            tmp = r.Expansion_EndSegment;
                            while (tmp != null)
                            {
                                if (tmp.BlockType.Transparent == false)
                                {
                                    bg = new GDIBrush(tmp.BlockType.BackColor);
                                    found = true;
                                    break;
                                }
                                tmp = tmp.Parent;
                            }
                        }
                    }
                }
            }




            if (RowIndex == _control.Caret.Position.Y && _control.HighLightActiveLine)
            {
                if (_control.hovered == false)
                {
                    bbuff.Clear(_GFX.HighLightLineBrush);
                    _GFX.BackBuffer.DrawRect(hc, 2, 20 + _control.View.GutterMarginWidth, 1, _control.View.ClientAreaWidth - 4, _control.View.RowHeight - 2);
                }
            }
            else if (RowIndex >= 0 && RowIndex < _control.Document.Count)
            {
                if (_control.Document[RowIndex].IsCollapsed)
                {
                    if (_control.Document[RowIndex].Expansion_EndRow.Index ==
                        _control.Caret.Position.Y && _control.HighLightActiveLine)
                        bbuff.Clear(_GFX.HighLightLineBrush);
                    else
                        bbuff.Clear(bg);
                }
                else
                    bbuff.Clear(bg);
            }
            else
                bbuff.Clear(bg);


            //only render normal text if any part of the row is visible
            if (RowIndex <= _control.Selection.LogicalBounds.FirstRow || RowIndex >=
                _control.Selection.LogicalBounds.LastRow)
            {
                RenderText(RowIndex);
            }

            //only render selection text if the line is selected
            if (_control.Selection.IsValid)
            {
                if (RowIndex >= _control.Selection.LogicalBounds.FirstRow && RowIndex
                    <= _control.Selection.LogicalBounds.LastRow)
                {
                    if (_control.hovered == false)
                    {
                        if (_control.ContainsFocus)
                            _GFX.SelectionBuffer.Clear(_control.SelectionBackColor);
                        else
                            _GFX.SelectionBuffer.Clear(_control.InactiveSelectionBackColor);
                    }

                    RenderSelectedText(RowIndex);
                }
            }


            if (_control.ContainsFocus || _control.View.Action ==
                XTextAction.xtDragText)
            {
                RenderCaret(RowIndex, RowPos * _control.View.RowHeight + _yOffset);
            }

            RenderSelection(RowIndex, true);
            RenderMargin(RowIndex);
            if (_control.Document.Folding)
                RenderExpansion(RowIndex);

            RowPaintEventArgs e = new RowPaintEventArgs();

            Rectangle rec = new Rectangle(0, 0, _control.Width,
                                          _control.View.RowHeight);
            e.Graphics = Graphics.FromHdc(bbuff.hDC);
            e.Bounds = rec;
            e.Row = null;
            if (RowIndex >= 0 && RowIndex < _control.Document.Count)
                e.Row = _control.Document[RowIndex];

            _control._CodeEditor.OnRenderRow(e);
            e.Graphics.Dispose();

            bbuff.Flush();
            bbuff.RenderToControl(0, RowPos * _control.View.RowHeight + _yOffset);

            //GFX.SelectionBuffer.RenderToControl (0,RowPos*Control.View.RowHeight+this.yOffset);


            if (found)
                bg.Dispose();
        }

        private void SetFont(bool Bold, bool Italic, bool Underline, GDISurface
            bbuff)
        {
            if (Bold)
                if (Italic)
                    if (Underline)
                        bbuff.Font = _GFX.FontBoldItalicUnderline;
                    else
                        bbuff.Font = _GFX.FontBoldItalic;
                else if (Underline)
                    bbuff.Font = _GFX.FontBoldUnderline;
                else
                    bbuff.Font = _GFX.FontBold;
            else if (Italic)
                if (Underline)
                    bbuff.Font = _GFX.FontItalicUnderline;
                else
                    bbuff.Font = _GFX.FontItalic;
            else if (Underline)
                bbuff.Font = _GFX.FontUnderline;
            else
                bbuff.Font = _GFX.FontNormal;
        }

        private void SetStringFont(bool Bold, bool Italic, bool Underline)
        {
            GDISurface bbuff = _GFX.StringBuffer;
            if (Bold)
                if (Italic)
                    if (Underline)
                        bbuff.Font = _GFX.FontBoldItalicUnderline;
                    else
                        bbuff.Font = _GFX.FontBoldItalic;
                else if (Underline)
                    bbuff.Font = _GFX.FontBoldUnderline;
                else
                    bbuff.Font = _GFX.FontBold;
            else if (Italic)
                if (Underline)
                    bbuff.Font = _GFX.FontItalicUnderline;
                else
                    bbuff.Font = _GFX.FontItalic;
            else if (Underline)
                bbuff.Font = _GFX.FontUnderline;
            else
                bbuff.Font = _GFX.FontNormal;
        }

        private void RenderCollapsedSelectedText(int RowIndex, int xPos)
        {
            GDISurface bbuff = _GFX.SelectionBuffer;
            bbuff.Font = _GFX.FontBold;
            bbuff.FontTransparent = true;

            if (_control.ContainsFocus)
                bbuff.TextForeColor = _control.SelectionForeColor;
            else
                bbuff.TextForeColor = _control.InactiveSelectionForeColor;

            //bbuff.TextForeColor =Color.DarkBlue;
            Row r = _control.Document[RowIndex];
            string str = "";
            str = r.CollapsedText;


            xPos++;
            int taborig = -_control.View.FirstVisibleColumn * _control.View.CharWidth
                + _control.View.TextMargin;
            _GFX.StringBuffer.Font = _GFX.FontBold;
            int wdh = _GFX.StringBuffer.DrawTabbedString(str, xPos + 1, 0, taborig,
                                                        _control.PixelTabSize).Width;

            if (_control.ContainsFocus)
            {
                bbuff.FillRect(_control.SelectionForeColor, xPos + 0, 0, wdh + 2,
                               _control.View.RowHeight);
                bbuff.FillRect(_control.SelectionBackColor, xPos + 1, 1, wdh,
                               _control.View.RowHeight - 2);
            }
            else
            {
                bbuff.FillRect(_control.InactiveSelectionForeColor, xPos + 0, 0, wdh + 2,
                               _control.View.RowHeight);
                bbuff.FillRect(_control.InactiveSelectionBackColor, xPos + 1, 1, wdh,
                               _control.View.RowHeight - 2);
            }


            wdh = bbuff.DrawTabbedString(str, xPos + 1, 0, taborig,
                                         _control.PixelTabSize).Width;


            //this can crash if document not fully parsed , on error resume next

            if (r.Expansion_StartSegment.EndRow != null)
            {
                if (r.Expansion_StartSegment.EndRow.RowState ==
                    RowState.SegmentParsed)
                    _control.Document.Parser.ParseLine
                        (r.Expansion_StartSegment.EndRow.Index, true);

                Word last = r.Expansion_StartSegment.EndWord;
                xPos += _control.View.FirstVisibleColumn * _control.View.CharWidth;
                r.Expansion_StartSegment.EndRow.Expansion_PixelStart = xPos + wdh -
                    _control.View.TextMargin + 2;
                r.Expansion_PixelEnd = xPos - 1;
                RenderSelectedText(_control.Document.IndexOf
                    (r.Expansion_StartSegment.EndRow),
                                   r.Expansion_StartSegment.EndRow.Expansion_PixelStart, last);
            }
        }

        private void RenderCollapsedText(int RowIndex, int xPos)
        {
            GDISurface bbuff = _GFX.BackBuffer;
            bbuff.Font = _GFX.FontBold;
            bbuff.FontTransparent = true;

            bbuff.TextForeColor = _control.OutlineColor;
            //bbuff.TextForeColor =Color.DarkBlue;
            Row r = _control.Document[RowIndex];
            string str = "";
            str = r.CollapsedText;


            xPos++;
            int taborig = -_control.View.FirstVisibleColumn * _control.View.CharWidth
                + _control.View.TextMargin;
            _GFX.StringBuffer.Font = _GFX.FontBold;
            int wdh = _GFX.StringBuffer.DrawTabbedString(str, xPos + 1, 0, taborig,
                                                        _control.PixelTabSize).Width;
            bbuff.FillRect(_GFX.OutlineBrush, xPos + 0, 0, wdh + 2,
                           _control.View.RowHeight);
            bbuff.FillRect(_GFX.BackgroundBrush, xPos + 1, 1, wdh,
                           _control.View.RowHeight - 2);
            wdh = bbuff.DrawTabbedString(str, xPos + 1, 0, taborig,
                                         _control.PixelTabSize).Width;


            //this can crash if document not fully parsed , on error resume next

            if (r.Expansion_StartSegment.EndRow != null)
            {
                if (r.Expansion_StartSegment.EndRow.RowState ==
                    RowState.SegmentParsed)
                    _control.Document.Parser.ParseLine
                        (r.Expansion_StartSegment.EndRow.Index, true);

                Word last = r.Expansion_StartSegment.EndWord;
                xPos += _control.View.FirstVisibleColumn * _control.View.CharWidth;
                r.Expansion_StartSegment.EndRow.Expansion_PixelStart = xPos + wdh -
                    _control.View.TextMargin + 2;
                r.Expansion_PixelEnd = xPos - 1;
                RenderText(_control.Document.IndexOf(r.Expansion_StartSegment.EndRow),
                           r.Expansion_StartSegment.EndRow.Expansion_PixelStart, last)
                    ;
            }
        }

        private void RenderText(int RowIndex)
        {
            RenderText(RowIndex, 0, null);
        }

        public string word { get; set; }

        private void RenderText(int RowIndex, int XOffset, Word StartWord)
        {
            GDISurface bbuff = _GFX.BackBuffer;
            bbuff.Font = _GFX.FontNormal;
            bbuff.FontTransparent = true;
            bool DrawBreakpoint = false;
            if (RowIndex <= _control.Document.Count - 1)
            {
                bbuff.TextForeColor = Color.Black;
                Row xtr = _control.Document[RowIndex];

                //if (xtr.StartSegment != null)
                //	bbuff.DrawTabbedString (xtr.StartSegment.GetHashCode ().ToString (System.Globalization.CultureInfo.InvariantCulture),100,0,0,0);

                //bbuff.TextForeColor = Color.Black;
                //bbuff.DrawTabbedString (xtr.Text,(int)(Control.View.TextMargin -Control.View.ClientAreaStart),1,-Control.View.FirstVisibleColumn*Control.View.CharWidth+Control.View.TextMargin,Control.PixelTabSize);					

                int xpos = _control.View.TextMargin - _control.View.ClientAreaStart +
                    XOffset;
                int wdh = 0;
                int taborig = -_control.View.FirstVisibleColumn *
                    _control.View.CharWidth + _control.View.TextMargin;


                bool ws = _control.ShowWhitespace;
                bool StartDraw = false;
                if (StartWord == null)
                    StartDraw = true;
                xtr.Expansion_StartChar = 0;
                xtr.Expansion_EndChar = 0;
                bool HasExpansion = false;

                WordCollection wordCollection = xtr.FormattedWords;

                Word w = null;

                for (int i = 0; i < wordCollection.Count; i++)
                {
                    w = wordCollection[i];

                    if (StartDraw)
                    {
                        if (w.Segment == xtr.Expansion_StartSegment &&
                            xtr.Expansion_StartSegment != null)
                            if (xtr.Expansion_StartSegment.Expanded == false)
                            {
                                RenderCollapsedText(RowIndex, xpos);
                                HasExpansion = true;
                                break;
                            }

                        if ((w.Type == WordType.xtSpace || w.Type == WordType.xtTab) &&
                            !DrawBreakpoint && _control.ShowTabGuides)
                        {
                            int xtab = xpos - (_control.View.TextMargin -
                                _control.View.ClientAreaStart + XOffset);
                            if (((double)xtab / (double)_control.PixelTabSize) == (xtab /
                                _control.PixelTabSize))
                                bbuff.FillRect(_control.TabGuideColor, xpos, 0, 1,
                                               _control.View.RowHeight);
                        }

                        if (w.Type == WordType.xtWord || ws == false)
                        {
                            if (w.Style != null)
                            {
                                SetFont(w.Style.Bold, w.Style.Italic, w.Style.Underline, bbuff);
                                //bbuff.TextBackColor = w.Style.BackColor;
                                bbuff.TextForeColor = w.Style.ForeColor;
                                bbuff.FontTransparent = w.Style.Transparent;
                            }
                            else
                            {
                                bbuff.Font = _GFX.FontNormal;
                                bbuff.TextForeColor = Color.Black;
                                bbuff.FontTransparent = true;
                            }



                            if (w.Type == WordType.xtWord)
                                DrawBreakpoint = true;

                            if (xtr.Breakpoint && DrawBreakpoint)
                            {
                                bbuff.TextForeColor = _control.BreakPointForeColor;
                                bbuff.TextBackColor = _control.BreakPointBackColor;
                                bbuff.FontTransparent = false;
                            }


                            if (_control.BracketMatching && (w == _bracketEnd || w ==
                                _bracketStart))
                            {
                                bbuff.TextForeColor = _control.BracketForeColor;

                                if (_control.BracketBackColor != Color.Transparent)
                                {
                                    bbuff.TextBackColor = _control.BracketBackColor;
                                    bbuff.FontTransparent = false;
                                }

                                wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                             _control.PixelTabSize).Width;
                                if (_control.BracketBorderColor != Color.Transparent)
                                {
                                    bbuff.DrawRect(_control.BracketBorderColor, xpos, 0, wdh,
                                                   _control.View.RowHeight - 1);
                                }
                            }
                            else
                            {
                                wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                             _control.PixelTabSize).Width;
                            }


                            //render errors
                            if (w.HasError || w.HasWarning)
                            {
                                //bbuff.FillRect (Color.Red,xpos,Control.View.RowHeight-2,wdh,2);
                                int ey = _control.View.RowHeight - 1;

                                Color c;
                                if (w.HasError)
                                    c = w.ErrorColor;
                                else
                                    c = w.WarningColor;

                                for (int x = 0; x < wdh + 3; x += 4)
                                {
                                    bbuff.DrawLine(c, new Point(xpos + x, ey), new Point(xpos + x
                                        + 2, ey - 2));
                                    bbuff.DrawLine(c, new Point(xpos + x + 2, ey - 2), new Point
                                        (xpos + x + 4, ey));
                                }
                            }
                        }
                        else if (w.Type == WordType.xtSpace && ws)
                        {
                            bbuff.Font = _GFX.FontNormal;
                            bbuff.TextForeColor = _control.WhitespaceColor;
                            bbuff.FontTransparent = true;

                            if (xtr.Breakpoint && DrawBreakpoint)
                            {
                                bbuff.TextForeColor = _control.BreakPointForeColor;
                                bbuff.TextBackColor = _control.BreakPointBackColor;
                                bbuff.FontTransparent = false;
                            }

                            bbuff.DrawTabbedString("\u00B7", xpos, 0, taborig,
                                                   _control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                         _control.PixelTabSize).Width;
                        }
                        else if (w.Type == WordType.xtTab && ws)
                        {
                            bbuff.Font = _GFX.FontNormal;
                            bbuff.TextForeColor = _control.WhitespaceColor;
                            bbuff.FontTransparent = true;

                            if (xtr.Breakpoint && DrawBreakpoint)
                            {
                                bbuff.TextForeColor = _control.BreakPointForeColor;
                                bbuff.TextBackColor = _control.BreakPointBackColor;
                                bbuff.FontTransparent = false;
                            }

                            bbuff.DrawTabbedString("\u00BB", xpos, 0, taborig,
                                                   _control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                         _control.PixelTabSize).Width;
                        }
                        if (w.Pattern != null)
                            if (w.Pattern.IsSeparator)
                            {
                                bbuff.FillRect(_control.SeparatorColor,
                                               _control.View.TextMargin - 4,
                                               _control.View.RowHeight - 1,
                                               _control.View.ClientAreaWidth, 1);
                            }

                        xpos += wdh;
                    }


                    if (!StartDraw)
                        xtr.Expansion_StartChar += w.Text.Length;

                    if (w == StartWord)
                        StartDraw = true;

                    xtr.Expansion_EndChar += w.Text.Length;
                }

                if (_control._CodeEditor.ShowEOLMarker && !HasExpansion)
                {
                    bbuff.Font = _GFX.FontNormal;
                    bbuff.TextForeColor = _control._CodeEditor.EOLMarkerColor;
                    bbuff.FontTransparent = true;
                    bbuff.DrawTabbedString("\u00B6", xpos, 0, taborig, _control.PixelTabSize);
                }
            }
            else
            {
                //bbuff.TextForeColor =Color.Red;
                //bbuff.DrawTabbedString ("",Control.View.TextMargin ,1,0,Control.PixelTabSize);
            }
        }


        private void RenderSelectedText(int RowIndex)
        {
            RenderSelectedText(RowIndex, 0, null);
        }

        private void RenderSelectedText(int RowIndex, int XOffset, Word StartWord)
        {
            GDISurface bbuff = _GFX.SelectionBuffer;
            bbuff.Font = _GFX.FontNormal;
            bbuff.FontTransparent = true;
            if (RowIndex <= _control.Document.Count - 1)
            {
                if (_control.ContainsFocus)
                    bbuff.TextForeColor = _control.SelectionForeColor;
                else
                    bbuff.TextForeColor = _control.InactiveSelectionForeColor;

                Row xtr = _control.Document[RowIndex];

                //if (xtr.StartSegment != null)
                //	bbuff.DrawTabbedString (xtr.StartSegment.GetHashCode ().ToString (System.Globalization.CultureInfo.InvariantCulture),100,0,0,0);

                //bbuff.TextForeColor = Color.Black;
                //bbuff.DrawTabbedString (xtr.Text,(int)(Control.View.TextMargin -Control.View.ClientAreaStart),1,-Control.View.FirstVisibleColumn*Control.View.CharWidth+Control.View.TextMargin,Control.PixelTabSize);					

                int xpos = _control.View.TextMargin - _control.View.ClientAreaStart +
                    XOffset;
                int wdh = 0;
                int taborig = -_control.View.FirstVisibleColumn *
                    _control.View.CharWidth + _control.View.TextMargin;


                bool ws = _control.ShowWhitespace;
                bool StartDraw = false;
                if (StartWord == null)
                    StartDraw = true;
                xtr.Expansion_StartChar = 0;
                xtr.Expansion_EndChar = 0;
                bool HasExpansion = false;


                WordCollection wordCollection = xtr.FormattedWords;

                Word w = null;

                for (int i = 0; i < wordCollection.Count; i++)
                {
                    w = wordCollection[i];

                    if (StartDraw)
                    {
                        if (w.Segment == xtr.Expansion_StartSegment &&
                            xtr.Expansion_StartSegment != null)
                            if (xtr.Expansion_StartSegment.Expanded == false)
                            {
                                RenderCollapsedSelectedText(RowIndex, xpos);
                                HasExpansion = true;
                                break;
                            }


                        if (w.Type == WordType.xtWord || ws == false)
                        {
                            if (w.Style != null)
                            {
                                SetFont(w.Style.Bold, w.Style.Italic, w.Style.Underline, bbuff);
                            }
                            else
                            {
                                bbuff.Font = _GFX.FontNormal;
                            }

                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                         _control.PixelTabSize).Width;

                            //render errors
                            if (w.HasError || w.HasWarning)
                            {
                                //bbuff.FillRect (Color.Red,xpos,Control.View.RowHeight-2,wdh,2);
                                int ey = _control.View.RowHeight - 1;
                                Color c;
                                if (w.HasError)
                                    c = w.ErrorColor;
                                else
                                    c = w.WarningColor;

                                for (int x = 0; x < wdh + 3; x += 4)
                                {
                                    bbuff.DrawLine(c, new Point(xpos + x, ey), new Point(xpos + x
                                        + 2, ey - 2));
                                    bbuff.DrawLine(c, new Point(xpos + x + 2, ey - 2), new Point
                                        (xpos + x + 4, ey));
                                }
                            }
                        }
                        else if (w.Type == WordType.xtSpace && ws)
                        {
                            bbuff.Font = _GFX.FontNormal;
                            bbuff.DrawTabbedString("\u00B7", xpos, 0, taborig,
                                                   _control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                         _control.PixelTabSize).Width;
                        }
                        else if (w.Type == WordType.xtTab && ws)
                        {
                            bbuff.Font = _GFX.FontNormal;
                            bbuff.DrawTabbedString("\u00BB", xpos, 0, taborig,
                                                   _control.PixelTabSize);
                            wdh = bbuff.DrawTabbedString(w.Text, xpos, 0, taborig,
                                                         _control.PixelTabSize).Width;
                        }
                        if (w.Pattern != null)
                            if (w.Pattern.IsSeparator)
                            {
                                bbuff.FillRect(_control.SeparatorColor,
                                               _control.View.TextMargin - 4,
                                               _control.View.RowHeight - 1,
                                               _control.View.ClientAreaWidth, 1);
                            }

                        xpos += wdh;
                    }


                    if (!StartDraw)
                        xtr.Expansion_StartChar += w.Text.Length;

                    if (w == StartWord)
                        StartDraw = true;

                    xtr.Expansion_EndChar += w.Text.Length;
                }
                if (_control._CodeEditor.ShowEOLMarker && !HasExpansion)
                {
                    bbuff.Font = _GFX.FontNormal;
                    bbuff.TextForeColor = _control.SelectionForeColor;
                    bbuff.FontTransparent = true;
                    bbuff.DrawTabbedString("\u00B6", xpos, 0, taborig, _control.PixelTabSize);
                }
            }
            else
            {
                //bbuff.TextForeColor =Color.Red;
                //bbuff.DrawTabbedString ("",Control.View.TextMargin ,1,0,Control.PixelTabSize);
            }
        }

        private void RenderCaret(int RowIndex, int ypos)
        {
            int StartRow = -1;
            int cr = _control.Caret.Position.Y;
            bool Collapsed = false;

            if (cr >= 0 && cr <= _control.Document.Count - 1)
            {
                Row r = _control.Document[cr];
                if (r.Expansion_EndSegment != null)
                {
                    if (r.Expansion_EndSegment.Expanded == false)
                    {
                        r = r.Expansion_EndSegment.StartRow;
                        StartRow = r.Index;
                    }
                }
            }

            Collapsed = (RowIndex == StartRow);


            if (RowIndex != cr && RowIndex != StartRow)
                return;

            if (_control.View.Action == XTextAction.xtDragText)
            {
                //drop Control.Caret
                Row xtr = _control.Document[cr];

                int pos = MeasureRow(xtr, _control.Caret.Position.X).Width + 1;

                if (Collapsed)
                {
                    pos += xtr.Expansion_PixelStart;
                    pos -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                      xtr.Expansion_PixelStart).Width;
                }
                if (_control.hovered == false)
                {
                    _GFX.BackBuffer.InvertRect(pos + _control.View.TextMargin -
                    _control.View.ClientAreaStart - 1, 0, 3,
                                          _control.View.RowHeight);
                    _GFX.BackBuffer.InvertRect(pos + _control.View.TextMargin -
                        _control.View.ClientAreaStart, 1, 1,
                                              _control.View.RowHeight - 2);
                }
            }
            else
            {
                //normal Control.Caret

                Row xtr = _control.Document[cr];
                if (!_control.OverWrite)
                {
                    int pos = _control.View.TextMargin - _control.View.ClientAreaStart;
                    pos += MeasureRow(xtr, _control.Caret.Position.X,
                                      xtr.Expansion_PixelStart).Width + 1;
                    if (Collapsed)
                    {
                        pos += xtr.Expansion_PixelStart;
                        pos -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                          xtr.Expansion_PixelStart).Width;
                    }

                    int wdh = _control.View.CharWidth / 12 + 1;
                    if (wdh < 2)
                        wdh = 2;

                    if (_control.Caret.Blink)
                    {
                        if (_control.hovered == false)
                            _GFX.BackBuffer.InvertRect(pos, 0, wdh, _control.View.RowHeight);
                    }

                    //GFX.BackBuffer.DrawRect(Color.Green, pos, 0, wdh, Control.View.RowHeight);

                    if (_control.IMEWindow == null)
                    {
                        _control.IMEWindow = new IMEWindow
                            (_control.Handle, _control.FontName,
                             _control.FontSize);
                        this.InitIMEWindow();
                    }
                    _control.IMEWindow.Loation = new Point(pos, ypos);
                }
                else
                {
                    int pos1 = MeasureRow(xtr, _control.Caret.Position.X).Width;
                    int pos2 = MeasureRow(xtr, _control.Caret.Position.X + 1).Width;
                    int wdh = pos2 - pos1;
                    if (Collapsed)
                    {
                        pos1 += xtr.Expansion_PixelStart;
                        pos1 -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                           xtr.Expansion_PixelStart).Width;
                    }

                    int pos = pos1 + _control.View.TextMargin -
                        _control.View.ClientAreaStart;
                    if (_control.Caret.Blink)
                    {
                        if (_control.hovered == false)
                            _GFX.BackBuffer.InvertRect(pos, 0, wdh, _control.View.RowHeight);
                    }
                    _control.IMEWindow.Loation = new Point(pos, ypos);
                }
            }
        }

        private void RenderMargin(int RowIndex)
        {
            GDISurface bbuff = _GFX.BackBuffer;

            if (_control.ShowGutterMargin)
            {
                bbuff.FillRect(_GFX.GutterMarginBrush, 0, 0,
                               _control.View.GutterMarginWidth, _control.View.RowHeight);
                bbuff.FillRect(_GFX.GutterMarginBorderBrush,
                               _control.View.GutterMarginWidth - 1, 0, 1,
                               _control.View.RowHeight);
                if (RowIndex <= _control.Document.Count - 1)
                {
                    Row r = _control.Document[RowIndex];

                    //if (_control.View.RowHeight >=
                    //    _control._CodeEditor.GutterIcons.ImageSize.Height)
                    //{
                    //    if (r.Bookmarked)
                    //        _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                    //                                            0, 0, 1);
                    //    if (r.Breakpoint)
                    //        _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                    //                                            0, 0, 0);
                    //}
                    //else
                    {
                        int w = _control.View.RowHeight;
                        if (r.Bookmarked)
                            _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                                                                0, 0, w, w, 1);
                        if (r.Breakpoint)
                        {
                            if(r.BreakpointEnabled == true)
                            _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                                                                0, 0, w, w, 0);
                            else _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                                                                0, 0, w, w, 2);

                        }
                    }

                    if (r.Images != null)
                    {
                        foreach (int i in r.Images)
                        {
                            if (_control.View.RowHeight >=
                                _control._CodeEditor.GutterIcons.ImageSize.Height)
                            {
                                _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                                                                    0, 0, i);
                            }
                            else
                            {
                                int w = _control.View.RowHeight;
                                _control._CodeEditor.GutterIcons.Draw(Graphics.FromHdc(bbuff.hDC),
                                                                    0, 0, w, w, i);
                            }
                        }
                    }
                }
            }


            //if (Control.ShowLineNumbers)
            //{
            //    bbuff.FillRect(GFX.LineNumberMarginBrush,
            //                   Control.View.GutterMarginWidth, 0,
            //                   Control.View.LineNumberMarginWidth + 1,
            //                   Control.View.RowHeight);

            //    //bbuff.FillRect (GFX.LineNumberMarginBrush  ,Control.View.GutterMarginWidth+Control.View.LineNumberMarginWidth,0,1,Control.View.RowHeight);

            //    for (int j = 0; j < this.Control.View.RowHeight; j += 2)
            //    {
            //        bbuff.FillRect(GFX.LineNumberMarginBorderBrush,
            //                       Control.View.GutterMarginWidth +
            //                        Control.View.LineNumberMarginWidth, j, 1, 1);
            //    }
            //}

            if (_control.ShowLineNumbers)
            {
                //bbuff.FillRect(GFX.LineNumberMarginBrush,
                //               Control.View.GutterMarginWidth, 0,
                //               Control.View.LineNumberMarginWidth + 1,
                //               Control.View.RowHeight);

                using (Graphics gfx = Graphics.FromHdc(bbuff.hDC))
                {
                    _control.LineMarginRender.Bounds = new Rectangle(_control.View.GutterMarginWidth, 0,
                               _control.View.LineNumberMarginWidth + 1,
                               _control.View.RowHeight);

                    _control.LineMarginRender.Render(gfx);
                }
                //bbuff.FillRect (GFX.LineNumberMarginBrush  ,Control.View.GutterMarginWidth+Control.View.LineNumberMarginWidth,0,1,Control.View.RowHeight);

                for (int j = 0; j < _control.View.RowHeight; j += 2)
                {
                    bbuff.FillRect(_GFX.LineNumberMarginBorderBrush,
                                   _control.View.GutterMarginWidth +
                                    _control.View.LineNumberMarginWidth, j, 1, 1);
                }
            }
            //if (!Control.ShowLineNumbers || !Control.ShowGutterMargin)
            //{
            //    bbuff.FillRect(GFX.BackgroundBrush, Control.View.TotalMarginWidth, 0,
            //                   Control.View.TextMargin - Control.View.TotalMarginWidth
            //                    - 3, Control.View.RowHeight);
            //}
            //else
            //{
            //    bbuff.FillRect(GFX.BackgroundBrush, Control.View.TotalMarginWidth + 1,
            //                   0, Control.View.TextMargin -
            //                    Control.View.TotalMarginWidth - 4,
            //                   Control.View.RowHeight);
            //}

            if (!_control.ShowLineNumbers || !_control.ShowGutterMargin)
            {
                //using (Graphics gfx = Graphics.FromHdc(bbuff.hDC))
                //{
                //    Control.LineMarginRender.Bounds = new Rectangle(
                //        Control.View.TotalMarginWidth,0,
                //        Control.View.TextMargin - Control.View.TotalMarginWidth - 3,
                //        Control.View.RowHeight);

                //    Control.LineMarginRender.Render(gfx);
                //}

                bbuff.FillRect(_GFX.BackgroundBrush, _control.View.TotalMarginWidth, 0,
                               _control.View.TextMargin - _control.View.TotalMarginWidth
                                - 3, _control.View.RowHeight);
            }
            else
            {
                bbuff.FillRect(_GFX.BackgroundBrush, _control.View.TotalMarginWidth + 1,
                               0, _control.View.TextMargin -
                                _control.View.TotalMarginWidth - 4,
                               _control.View.RowHeight);
                //using (Graphics gfx = Graphics.FromHdc(bbuff.hDC))
                //{

                //    Control.LineMarginRender.Bounds = new Rectangle(
                //        Control.View.TotalMarginWidth + 1, 0,
                //         Control.View.TextMargin - Control.View.TotalMarginWidth - 4,
                //         Control.View.RowHeight);

                //    Control.LineMarginRender.Render(gfx);
                //}
            }

            if (_control.ShowLineNumbers)
            {
                bbuff.Font = _GFX.FontNormal;
                bbuff.FontTransparent = true;

                bbuff.TextForeColor = _control.LineNumberForeColor;
                if (RowIndex <= _control.Document.Count - 1)
                {
                    int nw = this.MeasureString((RowIndex + 1).ToString
                        (CultureInfo.InvariantCulture)).Width;

                    bbuff.DrawTabbedString((RowIndex + 1).ToString
                        (CultureInfo.InvariantCulture), _control.View.GutterMarginWidth + _control.View.LineNumberMarginWidth - nw - 1, 1, 0, _control.PixelTabSize);
                }
            }
        }

        private void RenderExpansion(int RowIndex)
        {
            if (RowIndex <= _control.Document.Count - 1)
            {
                int yo = 4;
                Row xtr = _control.Document[RowIndex];
                GDISurface bbuff = _GFX.BackBuffer;
                if (xtr.EndSegment != null)
                {
                    if (xtr.Expansion_StartSegment != null && xtr.StartSegment.Parent ==
                        null)
                    {
                        if (!xtr.IsCollapsed)
                        {
                            bbuff.FillRect(_GFX.OutlineBrush, _control.View.TotalMarginWidth +
                                6, yo, 1, _control.View.RowHeight - yo);
                            //	bbuff.DrawTabbedString ("AAAA",0,0,0,0);
                        }
                    }
                    else if ((xtr.EndSegment.Parent != null || xtr.Expansion_EndSegment
                        != null))
                    {
                        bbuff.FillRect(_GFX.OutlineBrush, _control.View.TotalMarginWidth + 6,
                                       0, 1, _control.View.RowHeight);
                        //	bbuff.DrawTabbedString ("BBBB",0,0,0,0);

                    }

                    if (xtr.Expansion_StartSegment != null)
                    {
                        //bbuff.FillRect  (GFX.BackgroundBrush,Control.View.TotalMarginWidth+2,3,9,9); 


                        bbuff.FillRect(_GFX.OutlineBrush, _control.View.TotalMarginWidth + 2,
                                       yo, 9, 9);
                        bbuff.FillRect(_GFX.BackgroundBrush, _control.View.TotalMarginWidth +
                            3, yo + 1, 7, 7);
                        //render plus / minus
                        bbuff.FillRect(_GFX.OutlineBrush, _control.View.TotalMarginWidth + 4,
                                       yo + 4, 5, 1);
                        if (!xtr.Expansion_StartSegment.Expanded)
                            bbuff.FillRect(_GFX.OutlineBrush, _control.View.TotalMarginWidth +
                                6, yo + 2, 1, 5);
                    }
                    if (xtr.Expansion_EndSegment != null)
                    {
                        bbuff.FillRect(_GFX.OutlineBrush, _control.View.TotalMarginWidth + 7,
                                       _control.View.RowHeight - 1, 5, 1);
                    }
                }

                //				//RENDER SPAN LINES
                //				if (SpanFound)
                //				{
                //					if (RowIndex==FirstSpanRow)
                //						bbuff.FillRect (GFX.OutlineBrush,this.Control.View.TotalMarginWidth +14,0,Control.ClientWidth ,1);
                //
                //					if (RowIndex==LastSpanRow)
                //						bbuff.FillRect (GFX.OutlineBrush,this.Control.View.TotalMarginWidth +14,Control.View.RowHeight-1,Control.ClientWidth,1);				
                //				}

                //RENDER SPAN MARGIN
                if (_spanFound && _control._CodeEditor.ScopeIndicatorColor !=
                    Color.Transparent && _control.CodeEditor.ShowScopeIndicator)
                {
                    if (RowIndex >= _firstSpanRow && RowIndex <= _lastSpanRow)
                        bbuff.FillRect(_control._CodeEditor.ScopeIndicatorColor,
                                       _control.View.TotalMarginWidth + 14, 0, 2,
                                       _control.View.RowHeight);

                    if (RowIndex == _firstSpanRow)
                        bbuff.FillRect(_control._CodeEditor.ScopeIndicatorColor,
                                       _control.View.TotalMarginWidth + 14, 0, 4, 2);

                    if (RowIndex == _lastSpanRow)
                        bbuff.FillRect(_control._CodeEditor.ScopeIndicatorColor,
                                       _control.View.TotalMarginWidth + 14,
                                       _control.View.RowHeight - 2, 4, 2);
                }
            }
        }


        //draws aControl.Selection.LogicalBounds row in the backbuffer
        private void RenderSelection(int RowIndex, bool Invert)
        {
            if (RowIndex <= _control.Document.Count - 1 && _control.Selection.IsValid)
            {
                Row xtr = _control.Document[RowIndex];
                if (!xtr.IsCollapsed)
                {
                    if ((RowIndex > _control.Selection.LogicalBounds.FirstRow) &&
                        (RowIndex < _control.Selection.LogicalBounds.LastRow))
                    {
                        int width = MeasureRow(xtr, xtr.Text.Length).Width +
                            this.MeasureString("\u00B6").Width + 3;
                        RenderBox(_control.View.TextMargin, 0, Math.Max(width -
                            _control.View.ClientAreaStart, 0), _control.View.RowHeight,
                                  Invert);
                    }
                    else if ((RowIndex == _control.Selection.LogicalBounds.FirstRow) &&
                        (RowIndex == _control.Selection.LogicalBounds.LastRow))
                    {
                        int start = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                             _control.Selection.LogicalBounds.FirstColumn))
                            .Width;
                        int width = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                             _control.Selection.LogicalBounds.LastColumn))
                            .Width - start;
                        RenderBox(_control.View.TextMargin + start -
                            _control.View.ClientAreaStart, 0, width,
                                  _control.View.RowHeight, Invert);
                    }
                    else if (RowIndex == _control.Selection.LogicalBounds.LastRow)
                    {
                        int width = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                             _control.Selection.LogicalBounds.LastColumn))
                            .Width;
                        RenderBox(_control.View.TextMargin, 0, Math.Max(width -
                            _control.View.ClientAreaStart, 0), _control.View.RowHeight,
                                  Invert);
                    }
                    else if (RowIndex == _control.Selection.LogicalBounds.FirstRow)
                    {
                        int start = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                             _control.Selection.LogicalBounds.FirstColumn))
                            .Width;
                        int width = MeasureRow(xtr, xtr.Text.Length).Width +
                            this.MeasureString("\u00B6").Width + 3 - start;
                        RenderBox(_control.View.TextMargin + start -
                            _control.View.ClientAreaStart, 0, width,
                                  _control.View.RowHeight, Invert);
                    }
                }
                else
                {
                    RenderCollapsedSelection(RowIndex, Invert);
                }
            }
        }

        private void RenderCollapsedSelection(int RowIndex, bool Invert)
        {
            Row xtr = _control.Document[RowIndex];
            if ((RowIndex > _control.Selection.LogicalBounds.FirstRow) && (RowIndex <
                _control.Selection.LogicalBounds.LastRow))
            {
                int width = MeasureRow(xtr, xtr.Expansion_EndChar).Width;
                RenderBox(_control.View.TextMargin, 0, Math.Max(width -
                    _control.View.ClientAreaStart, 0), _control.View.RowHeight,
                          Invert);
            }
            else if ((RowIndex == _control.Selection.LogicalBounds.FirstRow) &&
                (RowIndex == _control.Selection.LogicalBounds.LastRow))
            {
                int start = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                     _control.Selection.LogicalBounds.FirstColumn))
                    .Width;
                int min = Math.Min(xtr.Text.Length,
                                   _control.Selection.LogicalBounds.LastColumn);
                min = Math.Min(min, xtr.Expansion_EndChar);
                int width = MeasureRow(xtr, min).Width - start;
                RenderBox(_control.View.TextMargin + start -
                    _control.View.ClientAreaStart, 0, width,
                          _control.View.RowHeight, Invert);
            }
            else if (RowIndex == _control.Selection.LogicalBounds.LastRow)
            {
                int width = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                     _control.Selection.LogicalBounds.LastColumn))
                    .Width;
                RenderBox(_control.View.TextMargin, 0, Math.Max(width -
                    _control.View.ClientAreaStart, 0), _control.View.RowHeight,
                          Invert);
            }
            else if (RowIndex == _control.Selection.LogicalBounds.FirstRow)
            {
                int start = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                     _control.Selection.LogicalBounds.FirstColumn))
                    .Width;
                int width = MeasureRow(xtr, Math.Min(xtr.Text.Length,
                                                     xtr.Expansion_EndChar)).Width - start;
                RenderBox(_control.View.TextMargin + start -
                    _control.View.ClientAreaStart, 0, width,
                          _control.View.RowHeight, Invert);
            }

            if (_control.Selection.LogicalBounds.LastRow > RowIndex &&
                _control.Selection.LogicalBounds.FirstRow <= RowIndex)
            {
                int start = xtr.Expansion_PixelEnd;
                int end = xtr.Expansion_EndRow.Expansion_PixelStart - start +
                    _control.View.TextMargin;
                //start+=100;
                //end=200;
                RenderBox(start - _control.View.ClientAreaStart, 0, end,
                          _control.View.RowHeight, Invert);
            }

            RowIndex = xtr.Expansion_EndRow.Index;
            xtr = xtr.Expansion_EndRow;

            if (_control.Selection.LogicalBounds.FirstRow <= RowIndex &&
                _control.Selection.LogicalBounds.LastRow >= RowIndex)
            {
                int endchar = 0;

                if (_control.Selection.LogicalBounds.LastRow == RowIndex)
                    endchar = Math.Min(_control.Selection.LogicalBounds.LastColumn,
                                       xtr.Text.Length);
                else
                    endchar = xtr.Text.Length;


                int end = MeasureRow(xtr, endchar, xtr.Expansion_PixelStart).Width;
                end += xtr.Expansion_PixelStart;
                end -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                  xtr.Expansion_PixelStart).Width;

                int start = 0;

                if (_control.Selection.LogicalBounds.FirstRow == RowIndex)
                {
                    int startchar = Math.Max(_control.Selection.LogicalBounds.FirstColumn,
                                             xtr.Expansion_StartChar);
                    start = MeasureRow(xtr, startchar, xtr.Expansion_PixelStart).Width;
                    start += xtr.Expansion_PixelStart;
                    start -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                        xtr.Expansion_PixelStart).Width;
                }
                else
                {
                    start = MeasureRow(xtr, xtr.Expansion_StartChar,
                                       xtr.Expansion_PixelStart).Width;
                    start += xtr.Expansion_PixelStart;
                    start -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                        xtr.Expansion_PixelStart).Width;
                }

                end -= start;

                if (_control.Selection.LogicalBounds.LastRow != RowIndex)
                    end += 6;


                RenderBox(_control.View.TextMargin + start -
                    _control.View.ClientAreaStart, 0, end, _control.View.RowHeight,
                          Invert);
            }
        }

        private void RenderBox(int x, int y, int width, int height, bool Invert)
        {
            //if (Invert)
            //GFX.BackBuffer.InvertRect  (x,y,width,height);
            _GFX.SelectionBuffer.RenderTo(_GFX.BackBuffer, x, y, width, height, x, y);

            //else
            //{
            //	Color c1=Control.SelectionBackColor;
            //	Color c2=Color.FromArgb (255-c1.R,255-c1.G,255-c1.B);
            //	GFX.BackBuffer.FillRect    (c2,x,y,width,height);
            //}
        }

        /// <summary>
        /// Implementation of the iPainter CharFromPixel method
        /// </summary>
        /// <param name="X">Screen x position in pixels</param>
        /// <param name="Y">Screen y position in pixels</param>
        /// <returns>a Point where x is the column and y is the rowindex</returns>
        public TextPoint CharFromPixel(int X, int Y)
        {
            //try
            //{
            int RowIndex = Y / _control.View.RowHeight +
                _control.View.FirstVisibleRow;
            RowIndex = Math.Min(RowIndex, _control.Document.VisibleRows.Count);
            if (RowIndex == _control.Document.VisibleRows.Count)
            {
                RowIndex--;
                Row r = _control.Document.VisibleRows[RowIndex];
                if (r.IsCollapsed)
                    r = r.Expansion_EndRow;

                return new TextPoint(r.Text.Length, r.Index);
            }

            RowIndex = Math.Max(RowIndex, 0);
            Row xtr = null;
            if (_control.Document.VisibleRows.Count != 0)
            {
                xtr = _control.Document.VisibleRows[RowIndex];
                RowIndex = _control.Document.IndexOf(xtr);
            }
            else
            {
                return new TextPoint(0, 0);
            }
            if (RowIndex == -1)
                return new TextPoint(-1, -1);


            //normal line
            if (!xtr.IsCollapsed)
                return ColumnFromPixel(RowIndex, X);

            //this.RenderRow (xtr.Index,-200);

            if (X < xtr.Expansion_PixelEnd - _control.View.FirstVisibleColumn *
                _control.View.CharWidth)
            {
                //start of collapsed line
                return ColumnFromPixel(RowIndex, X);
            }
            else if (X >= xtr.Expansion_EndRow.Expansion_PixelStart -
                _control.View.FirstVisibleColumn * _control.View.CharWidth +
                _control.View.TextMargin)
            {
                //end of collapsed line
                return ColumnFromPixel(xtr.Expansion_EndRow.Index, X -
                    xtr.Expansion_EndRow.Expansion_PixelStart +
                    MeasureRow(xtr.Expansion_EndRow,
                               xtr.Expansion_EndRow.Expansion_StartChar,
                               xtr.Expansion_EndRow.Expansion_PixelStart)
                        .Width);
            }
            else
            {
                //the collapsed text
                return new TextPoint(xtr.Expansion_EndChar, xtr.Index);
            }
            //}
            //catch
            //{
            //    this.Control._CodeEditor.FontName = "Courier New";
            //    this.Control._CodeEditor.FontSize = 10;
            //    return new TextPoint(0, 0);
            //}
        }

        private TextPoint ColumnFromPixel(int RowIndex, int X)
        {
            Row xtr = _control.Document[RowIndex];
            int CharIndex = xtr.Text.Length;
            X -= _control.View.TextMargin - 2 - _control.View.FirstVisibleColumn *
                _control.View.CharWidth;

            if (xtr.Count == 0)
            {
                if (_control.VirtualWhitespace && _control.View.CharWidth > 0)
                {
                    return new TextPoint(X / _control.View.CharWidth, RowIndex);
                }
                else
                {
                    return new TextPoint(0, RowIndex);
                }
            }


            int taborig = -_control.View.FirstVisibleColumn * _control.View.CharWidth
                + _control.View.TextMargin;
            int xpos = _control.View.TextMargin - _control.View.ClientAreaStart;
            int Count = xtr.Text.Length;
            int CharNo = 0;
            int TotWidth = 0;
            Word Word = null;
            int WordStart = 0;

            WordCollection wordCollection = xtr.FormattedWords;

            Word w = null;

            for (int i = 0; i < wordCollection.Count; i++)
            {
                w = wordCollection[i];

                Word = w;
                WordStart = TotWidth;

                if (w.Type == WordType.xtWord && w.Style != null)
                    SetStringFont(w.Style.Bold, w.Style.Italic, w.Style.Underline);
                else
                    SetStringFont(false, false, false);

                int tmpWidth = _GFX.StringBuffer.DrawTabbedString(w.Text, xpos +
                    TotWidth, 0, taborig, _control.PixelTabSize).Width;

                if (TotWidth + tmpWidth >= X)
                {
                    break;
                }

                //dont do this for the last word
                if (w != xtr.FormattedWords[xtr.FormattedWords.Count - 1])
                {
                    TotWidth += tmpWidth;
                    CharNo += w.Text.Length;
                }
            }


            //CharNo is the index in the text where 'word' starts
            //'Word' is the word object that contains th 'X'
            //'WordStart' contains the pixel start position for 'Word'

            if (Word.Type == WordType.xtWord && Word.Style != null)
                SetStringFont(Word.Style.Bold, Word.Style.Italic, Word.Style.Underline);
            else
                SetStringFont(false, false, false);

            //now , lets measure each char and get a correct pos

            bool found = false;
            foreach (char c in Word.Text)
            {
                int tmpWidth = _GFX.StringBuffer.DrawTabbedString(c + "", xpos +
                    WordStart, 0, taborig, _control.PixelTabSize).Width;
                if (WordStart + tmpWidth >= X)
                {
                    found = true;
                    break;
                }
                CharNo++;
                WordStart += tmpWidth;
            }

            if (!found && _control.View.CharWidth > 0 &&
                _control.VirtualWhitespace)
            {
                int xx = X - WordStart;
                int cn = xx / _control.View.CharWidth;
                CharNo += cn;
            }

            if (CharNo < 0)
                CharNo = 0;

            return new TextPoint(CharNo, RowIndex);
        }

        public bool IsMouseOverWord(int RowIndex, int X)
        {
            Row xtr = _control.Document[RowIndex];
            int CharIndex = xtr.Text.Length;
            X -= _control.View.TextMargin - 2 - _control.View.FirstVisibleColumn *
                _control.View.CharWidth;

            if (xtr.Count == 0)
            {
                if (_control.VirtualWhitespace && _control.View.CharWidth > 0)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }


            int taborig = -_control.View.FirstVisibleColumn * _control.View.CharWidth
                + _control.View.TextMargin;
            int xpos = _control.View.TextMargin - _control.View.ClientAreaStart;
            int Count = xtr.Text.Length;
            int CharNo = 0;
            int TotWidth = 0;
            Word Word = null;
            int WordStart = 0;

            WordCollection wordCollection = xtr.FormattedWords;

            Word w = null;

            for (int i = 0; i < wordCollection.Count; i++)
            {
                w = wordCollection[i];

                Word = w;
                WordStart = TotWidth;

                if (w.Type == WordType.xtWord && w.Style != null)
                    SetStringFont(w.Style.Bold, w.Style.Italic, w.Style.Underline);
                else
                    SetStringFont(false, false, false);

                int tmpWidth = _GFX.StringBuffer.DrawTabbedString(w.Text, xpos +
                    TotWidth, 0, taborig, _control.PixelTabSize).Width;

                if (TotWidth + tmpWidth >= X)
                {
                    break;
                }

                //dont do this for the last word
                if (w != xtr.FormattedWords[xtr.FormattedWords.Count - 1])
                {
                    TotWidth += tmpWidth;
                    CharNo += w.Text.Length;
                }
            }

            if (TotWidth < X)
            {
                return false;
            }
            else return true;
        }


        private Point GetCaretPixelPos()
        {
            return GetTextPointPixelPos(_control.Caret.Position);
        }

        public Point GetTextPointPixelPos(TextPoint tp)
        {
            Row xtr = _control.Document[tp.Y];
            if (xtr == null) return new Point();
            if (xtr.RowState == RowState.SegmentParsed)
                _control.Document.Parser.ParseLine(xtr.Index, true);

            Row r = null;

            if (xtr.IsCollapsedEndPart)
                r = xtr.Expansion_StartRow;
            else
                r = xtr;

            int index = r.VisibleIndex;
            int yPos = (index - _control.View.FirstVisibleRow);
            if (yPos < 0 || yPos > _control.View.VisibleRowCount)
                return new Point(-1, -1);

            yPos *= _control.View.RowHeight;

            bool Collapsed = false;
            Collapsed = (xtr.IsCollapsedEndPart);
            int pos = MeasureRow(xtr, tp.X, xtr.Expansion_PixelStart).Width + 1;


            if (Collapsed)
            {
                pos += xtr.Expansion_PixelStart;
                pos -= MeasureRow(xtr, xtr.Expansion_StartChar,
                                  xtr.Expansion_PixelStart).Width;
            }

            int xPos = pos + _control.View.TextMargin - _control.View.ClientAreaStart;

            if (xPos < _control.View.TextMargin || xPos > _control.View.ClientAreaWidth
                + _control.View.TextMargin)
                return new Point(-1, -1);


            return new Point(xPos, yPos);
        }

        public int GetMaxCharWidth()
        {
            string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int max = 0;

            foreach (char c in s)
            {
                int tmp = this.MeasureString(c + "").Width;
                if (tmp > max)
                    max = tmp;
            }
            return max;
        }

        public void Dispose()
        {
            _GFX.Dispose();
        }
    }
}