namespace AIMS.Libraries.Scripting.ScriptControl
{
    partial class Document
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            AIMS.Libraries.CodeEditor.WinForms.LineMarginRender lineMarginRender1 = new AIMS.Libraries.CodeEditor.WinForms.LineMarginRender();
            this.CodeEditorCtrl = new AIMS.Libraries.CodeEditor.CodeEditorControl();
            this.syntaxDocument1 = new AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument(this.components);
            this.SuspendLayout();
            // 
            // CodeEditorCtrl
            // 
            this.CodeEditorCtrl.ActiveView = AIMS.Libraries.CodeEditor.WinForms.ActiveView.BottomRight;
            this.CodeEditorCtrl.AutoListPosition = null;
            //this.CodeEditorCtrl.AutoListSelectedText = "";
            //this.CodeEditorCtrl.AutoListVisible = false;
            this.CodeEditorCtrl.BorderColor = System.Drawing.Color.White;
            this.CodeEditorCtrl.BorderStyle = AIMS.Libraries.CodeEditor.WinForms.ControlBorderStyle.FixedSingle;
            this.CodeEditorCtrl.ChildBorderColor = System.Drawing.SystemColors.Window;
            this.CodeEditorCtrl.ChildBorderStyle = AIMS.Libraries.CodeEditor.WinForms.ControlBorderStyle.None;
            this.CodeEditorCtrl.CopyAsRTF = false;
            this.CodeEditorCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CodeEditorCtrl.FileName = null;
            this.CodeEditorCtrl.GutterMarginWidth = 20;
            this.CodeEditorCtrl.HighLightActiveLine = true;
            this.CodeEditorCtrl.HighLightedLineColor = System.Drawing.Color.White;
            //this.CodeEditorCtrl.InfoTipCount = 1;
            this.CodeEditorCtrl.InfoTipPosition = null;
            //this.CodeEditorCtrl.InfoTipSelectedIndex = 1;
            //this.CodeEditorCtrl.InfoTipVisible = false;
            lineMarginRender1.Bounds = new System.Drawing.Rectangle(19, 0, 19, 16);
            this.CodeEditorCtrl.LineMarginRender = lineMarginRender1;
            this.CodeEditorCtrl.Location = new System.Drawing.Point(0, 0);
            this.CodeEditorCtrl.LockCursorUpdate = false;
            this.CodeEditorCtrl.Name = "CodeEditorCtrl";
            this.CodeEditorCtrl.Saved = false;
            this.CodeEditorCtrl.ShowGutterMargin = true;// false;
            this.CodeEditorCtrl.ShowLineNumbers = false;
            this.CodeEditorCtrl.ShowScopeIndicator = false;
            this.CodeEditorCtrl.Size = new System.Drawing.Size(535, 368);
            this.CodeEditorCtrl.SmoothScroll = false;
            this.CodeEditorCtrl.SplitView = false;
            this.CodeEditorCtrl.SplitviewH = -4;
            this.CodeEditorCtrl.SplitviewV = -4;
            this.CodeEditorCtrl.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(219)))), ((int)(((byte)(214)))));
            this.CodeEditorCtrl.TabIndex = 0;
            this.CodeEditorCtrl.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            // 
            // syntaxDocument1
            // 
            this.syntaxDocument1.Lines = new string[] {
        ""};
            this.syntaxDocument1.MaxUndoBufferSize = 1000;
            this.syntaxDocument1.Modified = false;
            this.syntaxDocument1.UndoStep = 0;
            // 
            // Document
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(535, 368);
            this.Controls.Add(this.CodeEditorCtrl);
            this.DoubleBuffered = true;
            this.Name = "Document";
            this.ResumeLayout(false);

        }

        #endregion

        private AIMS.Libraries.CodeEditor.CodeEditorControl CodeEditorCtrl;
        public AIMS.Libraries.CodeEditor.Syntax.SyntaxDocument syntaxDocument1;
    }
}