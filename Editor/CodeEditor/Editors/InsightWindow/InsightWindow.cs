// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2150 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.CodeEditor.Util;
using AIMS.Libraries.CodeEditor.Syntax;

namespace AIMS.Libraries.CodeEditor.WinForms.InsightWindow
{
    public class InsightWindow : AbstractCompletionWindow
    {
        public InsightWindow(Form parentForm, CodeEditorControl control) : base(parentForm, control)
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public void ShowInsightWindow()
        {
            if (!Visible)
            {
                if (_insightDataProviderStack.Count > 0)
                {
                    ShowCompletionWindow();
                }
            }
            else
            {
                Refresh();
            }
        }

        #region Event handling routines
        public override bool ProcessTextAreaKey(Keys keyData)
        {
            if (!Visible)
            {
                return false;
            }
            switch (keyData)
            {
                case Keys.Down:
                    if (DataProvider != null && DataProvider.InsightDataCount > 0)
                    {
                        CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
                        Refresh();
                    }
                    return true;
                case Keys.Up:
                    if (DataProvider != null && DataProvider.InsightDataCount > 0)
                    {
                        CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
                        Refresh();
                    }
                    return true;
            }
            return base.ProcessTextAreaKey(keyData);
        }

        protected override void CaretOffsetChanged(object sender, EventArgs e)
        {
            // move the window under the caret (don't change the x position)
            TextPoint caretPos = control.ActiveViewControl.Caret.Position;

            Point p = control.ActiveViewControl.PointToScreen(new Point(caretPos.X, caretPos.Y));
            if (p.Y != Location.Y)
            {
                //Location = p;
                base.SetLocation();
            }

            while (DataProvider != null && DataProvider.CaretOffsetChanged())
            {
                CloseCurrentDataProvider();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            control.ActiveViewControl.Focus();
            if (TipPainterTools.DrawingRectangle1.Contains(e.X, e.Y))
            {
                CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
                Refresh();
            }
            if (TipPainterTools.DrawingRectangle2.Contains(e.X, e.Y))
            {
                CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
                Refresh();
            }
        }

        #endregion



        #region Insight Window Drawing routines
        protected override void OnPaint(PaintEventArgs pe)
        {
            string methodCountMessage = null, description;
            if (DataProvider == null || DataProvider.InsightDataCount < 1)
            {
                description = "Unknown Method";
            }
            else
            {
                if (DataProvider.InsightDataCount > 1)
                {
                    methodCountMessage = control.ActiveViewControl.GetRangeDescription(CurrentData + 1, DataProvider.InsightDataCount);
                }
                description = DataProvider.GetInsightData(CurrentData);
            }

            drawingSize = TipPainterTools.GetDrawingSizeHelpTipFromCombinedDescription(this,
                                                                                       pe.Graphics,
                                                                                       Font,
                                                                                       methodCountMessage,
                                                                                       description);
            if (drawingSize != Size)
            {
                SetLocation();
            }
            else
            {
                TipPainterTools.DrawHelpTipFromCombinedDescription(this, pe.Graphics, Font, methodCountMessage, description);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
        }
        #endregion

        #region InsightDataProvider handling
        private Stack<InsightDataProviderStackElement> _insightDataProviderStack = new Stack<InsightDataProviderStackElement>();

        private int CurrentData
        {
            get
            {
                return _insightDataProviderStack.Peek().currentData;
            }
            set
            {
                _insightDataProviderStack.Peek().currentData = value;
            }
        }

        private IInsightDataProvider DataProvider
        {
            get
            {
                if (_insightDataProviderStack.Count == 0)
                {
                    return null;
                }
                return _insightDataProviderStack.Peek().dataProvider;
            }
        }

        public void AddInsightDataProvider(IInsightDataProvider provider, string fileName)
        {
            provider.SetupDataProvider(fileName, control.ActiveViewControl);
            if (provider.InsightDataCount > 0)
            {
                _insightDataProviderStack.Push(new InsightDataProviderStackElement(provider));
            }
        }

        private void CloseCurrentDataProvider()
        {
            _insightDataProviderStack.Pop();
            if (_insightDataProviderStack.Count == 0)
            {
                Close();
            }
            else
            {
                Refresh();
            }
        }

        private class InsightDataProviderStackElement
        {
            public int currentData;
            public IInsightDataProvider dataProvider;

            public InsightDataProviderStackElement(IInsightDataProvider dataProvider)
            {
                this.currentData = Math.Max(dataProvider.DefaultIndex, 0);
                this.dataProvider = dataProvider;
            }
        }
        #endregion
    }
}
