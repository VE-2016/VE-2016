//Coded by Rajneesh Noonia 2007

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using AIMS.Libraries.CodeEditor.Win32;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Summary description for AutoListForm.
    /// </summary>
    [ToolboxItem(false)]
    public class AutoListForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int message, int _data, int _id);

        private TabListBox _LB;
        private ArrayList _items = new ArrayList();
        //private ToolTip tooltip;
        private IContainer _components;
        private WeakReference _Control = null;
        private DeclarationViewWindow _declaration = null;
        private Control ParentControl
        {
            get
            {
                if (_Control != null)
                    return (Control)_Control.Target;
                else
                    return null;
            }
            set { _Control = new WeakReference(value); }
        }


        /// <summary>
        /// The imagelist that should be used by the AutoListForm
        /// </summary>
        public ImageList Images = null;

        /// <summary>
        /// Default AltoListControl constructor.
        /// </summary>
        public AutoListForm(Control Owner)
        {
            this.ParentControl = Owner;
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            //SetStyle(ControlStyles.ContainerControl  ,false);
            SetStyle(ControlStyles.Selectable, true);
            _declaration = new DeclarationViewWindow(this);
            _declaration.HideOnClick = true;
            //TooltipMaxWidth(300);

            // TODO: Add any initialization after the InitForm call

        }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public void SendKey(int KeyCode)
        {
            SendMessage(_LB.Handle, (int)WindowMessage.WM_KEYDOWN, KeyCode, 0);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();
            _LB = new AIMS.Libraries.CodeEditor.WinForms.TabListBox();
            //this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // LB
            // 
            _LB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            _LB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            _LB.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _LB.IntegralHeight = false;
            _LB.ItemHeight = 16;
            _LB.Location = new System.Drawing.Point(4, 4);
            _LB.Name = "LB";
            _LB.Size = new System.Drawing.Size(168, 184);
            _LB.Sorted = true;
            _LB.TabIndex = 0;
            _LB.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LB_DrawItem);
            _LB.DoubleClick += new System.EventHandler(this.LB_DoubleClick);
            _LB.SelectedIndexChanged += new System.EventHandler(this.LB_SelectedIndexChanged);
            _LB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LB_MouseMove);
            _LB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LB_MouseDown);
            _LB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LB_KeyPress);
            _LB.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LB_KeyUp);
            _LB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LB_KeyDown);
            // 
            // tooltip
            // 
            //this.tooltip.AutoPopDelay = 5000;
            //this.tooltip.InitialDelay = 100;
            //this.tooltip.ReshowDelay = 100;
            // 
            // AutoListForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(174, 191);
            this.Controls.Add(_LB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AutoListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Resize += new System.EventHandler(this.AutoListForm_Resize);
            this.VisibleChanged += new EventHandler(AutoListForm_VisibleChanged);
            this.ResumeLayout(false);
        }

        //void TooltipMaxWidth(int Width)
        //{
        //    object o = typeof(ToolTip).InvokeMember("Handle", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty, null, this.tooltip, null);
        //    IntPtr hwnd = (IntPtr)o;
        //    SendMessage(hwnd, 0x0418, 0,Width); 
        //}

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            SetLocation();
        }
        private void AutoListForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Owner != null)
                if (this.Visible)
                {
                    this.Refresh();
                    LB_SelectedIndexChanged(sender, e);
                }
                else
                    _declaration.Hide();
            //tooltip.Hide(this.Owner);
        }

        #endregion

        /// <summary>		
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(SystemColors.Control);
            ControlPaint.DrawBorder3D(e.Graphics, 0, 0, this.Width, this.Height, Border3DStyle.Raised);
        }
        public void SelectItemByIndex(int Index)
        {
            if (_LB.Items.Count > Index)
            {
                _LB.SelectedIndex = Index;
            }
        }

        public void SelectItem(string startText)
        {
            if (startText == null || startText.Length == 0) return;
            string originalStartText = startText;
            startText = startText.ToLower();
            int bestIndex = -1;
            int bestQuality = -1;
            // Qualities: 0 = match start
            //            1 = match start case sensitive
            //            2 = full match
            //            3 = full match case sensitive
            double bestPriority = 0;
            int selectedItem = _LB.SelectedIndex;
            for (int i = 0; i < _LB.Items.Count; i++)
            {
                ListItem li = (ListItem)_LB.Items[i];

                string itemText = li.Text;
                string lowerText = itemText.ToLower();
                if (lowerText.StartsWith(startText))
                {
                    double priority = 0.0d;
                    if (li.CodeCompletionData == null)
                        priority = 0.0d;
                    else
                        priority = li.CodeCompletionData.Priority;
                    int quality;
                    if (lowerText == startText)
                    {
                        if (itemText == originalStartText)
                            quality = 3;
                        else
                            quality = 2;
                    }
                    else if (itemText.StartsWith(originalStartText))
                    {
                        quality = 1;
                    }
                    else
                    {
                        quality = 0;
                    }
                    bool useThisItem;
                    if (bestQuality < quality)
                    {
                        useThisItem = true;
                    }
                    else
                    {
                        if (bestIndex == selectedItem)
                        {
                            useThisItem = false;
                        }
                        else if (i == selectedItem)
                        {
                            useThisItem = bestQuality == quality;
                        }
                        else
                        {
                            useThisItem = bestQuality == quality && bestPriority < priority;
                        }
                    }
                    if (useThisItem)
                    {
                        bestIndex = i;
                        bestPriority = priority;
                        bestQuality = quality;
                    }
                }
            } //For Loop Finished


            _LB.SelectedIndex = bestIndex;
        }

        private void LB_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
            //	e.Handled =true;
        }

        private void LB_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
            //	e.Handled =true;
        }

        private void LB_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
            //	e.Handled =true;
        }

        /// <summary>
        /// For public use only.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        /// <summary>
        /// For public use only.
        /// </summary>
        /// <param name="charCode"></param>
        /// <returns></returns>
        protected override bool IsInputChar(char charCode)
        {
            return true;
        }

        /// <summary>
        /// Adds a new ListItem to the AutoListForm.
        /// </summary>
        /// <param name="Text">Text of the new ListItem</param>
        /// <param name="ImageIndex">Image index that should be assigned to the new ListItem</param>
        /// <returns></returns>
        public ListItem Add(string Text, AutoListIcons ImageIndex)
        {
            return this.Add(Text, Text, ImageIndex);
        }


        /// <summary>
        /// Adds a new ListItem to the AutoListForm.
        /// </summary>
        /// <param name="Text">Text of the new ListItem</param>
        /// <param name="InsertText">Text to insert when this item is selected</param>
        /// <param name="ImageIndex">Image index that should be assigned to the new ListItem</param>
        /// <returns></returns>
        public ListItem Add(string Text, string InsertText, AutoListIcons ImageIndex)
        {
            ListItem li = new ListItem(Text, (int)ImageIndex, "", InsertText);
            _LB.Items.Add(li);


            //this.LB.Sorted =true;
            return li;
        }

        public ListItem Add(ICompletionData CompletionData, AutoListIcons ImageIndex)
        {
            ListItem li = new ListItem(CompletionData, (int)ImageIndex);
            _LB.Items.Add(li);

            //this.LB.Sorted =true;
            return li;
        }

        public ListItem Add(string Text, string InsertText, string ToolTip, string ExtendedData, AutoListIcons ImageIndex)
        {
            ListItem li = new ListItem(Text, (int)ImageIndex, "", InsertText, ExtendedData);
            _LB.Items.Add(li);
            li.ToolTip = ToolTip;

            //this.LB.Sorted =true;
            return li;
        }

        public ListItem Add(string Text, string InsertText, string ToolTip, AutoListIcons ImageIndex)
        {
            ListItem li = new ListItem(Text, (int)ImageIndex, "", InsertText);
            _LB.Items.Add(li);
            li.ToolTip = ToolTip;

            //this.LB.Sorted =true;
            return li;
        }

        /// <summary>
        /// Clears the content of the AutoList.
        /// </summary>
        public void Clear()
        {
            _LB.Items.Clear();
        }

        private void LB_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            if (e.Index == -1)
                return;

            int Offset = 24;

            ListItem li = (ListItem)_LB.Items[e.Index];
            string text = li.Text;
            Brush bg, fg;

            if (selected)
            {
                bg = SystemBrushes.Highlight;
                fg = SystemBrushes.HighlightText;
                //fg=Brushes.Black;
            }
            else
            {
                bg = SystemBrushes.Window;
                fg = SystemBrushes.WindowText;
                //bg=Brushes.White;
                //fg=Brushes.Black;
            }

            if (!selected)
            {
                e.Graphics.FillRectangle(bg, 0, e.Bounds.Top, e.Bounds.Width, _LB.ItemHeight);
                //e.Graphics.FillRectangle (SystemBrushes.Highlight,0,e.Bounds.Top,27 ,LB.ItemHeight); 
            }
            else
            {
                e.Graphics.FillRectangle(SystemBrushes.Window, Offset, e.Bounds.Top, e.Bounds.Width - Offset, _LB.ItemHeight);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, new Rectangle(Offset + 1, e.Bounds.Top + 1, e.Bounds.Width - Offset - 2, _LB.ItemHeight - 2));


                //e.Graphics.FillRectangle (SystemBrushes.Highlight,27,e.Bounds.Top,e.Bounds.Width-27 ,LB.ItemHeight); 
                //e.Graphics.FillRectangle (new SolidBrush(Color.FromArgb (182,189,210)),new Rectangle (1+27,e.Bounds.Top+1,e.Bounds.Width-2- ,LB.ItemHeight-2));


                ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(Offset, e.Bounds.Top, e.Bounds.Width - Offset, _LB.ItemHeight));
            }


            e.Graphics.DrawString(text, e.Font, fg, Offset + 2, e.Bounds.Top + 1);


            if (Images != null)
                e.Graphics.DrawImage(Images.Images[li.Type], 6, e.Bounds.Top + 0);
        }

        /// <summary>
        /// Gets the "insert text" from the selected item.
        /// </summary>
        public string SelectedText
        {
            get
            {
                if (_LB.SelectedItem == null)
                    return "";

                ListItem li = (ListItem)_LB.SelectedItem;
                return li.InsertText;
            }
        }

        public ListItem SelectedListItem
        {
            get
            {
                if (_LB.SelectedItem == null)
                    return null;

                ListItem li = (ListItem)_LB.SelectedItem;
                return li;
            }
        }

        private void LB_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        public void BeginLoad()
        {
            _LB.Sorted = false;
            _LB.DrawMode = DrawMode.Normal;
            _LB.SuspendLayout();
        }

        public void EndLoad()
        {
            _LB.ResumeLayout();
            _LB.Sorted = true;
            _LB.DrawMode = DrawMode.OwnerDrawFixed;

            //set height
            this.Height = 0;
            if (_LB.Items.Count > 10)
            {
                this.Height = _LB.ItemHeight * 11 + 12;
            }
            else
            {
                this.Height = _LB.ItemHeight * (_LB.Items.Count) + 12;
            }
            int max = 0;
            Graphics g = _LB.CreateGraphics();
            foreach (ListItem li in _LB.Items)
            {
                int w = (int)g.MeasureString(li.Text, _LB.Font).Width + 45;
                if (w > max)
                    max = w;
            }
            this.Width = max + SystemInformation.VerticalScrollBarWidth;
            this.Refresh();
            g.Dispose();
        }


        private void AutoListForm_Resize(object sender, EventArgs e)
        {
            _LB.Size = new Size(this.Width - 8, this.Height - 8);
        }

        private string GetToolTipText(ListItem li)
        {
            if (li != null)
            {
                StringBuilder ex = new StringBuilder(li.ToolTip);
                if (li.ExtendedData != null)
                {
                    if (li.ExtendedData.Length > 0)
                        ex.Append(Environment.NewLine + GetDocumentation(li.ExtendedData));
                }
                return ex.ToString();
            }
            return "";
        }

        private void SetLocation()
        {
            int x = (this.Width + 5);
            int index = _LB.SelectedIndex - _LB.TopIndex;
            int y = ((index) * _LB.ItemHeight + 5);
            Point p = new Point(x, y);
            p = this.PointToScreen(p);
            _declaration.Location = p;
            _declaration.Refresh();
        }
        private void LB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem li = (ListItem)_LB.SelectedItem;
            if (li != null)
            {
                string text = "";
                if (li.CodeCompletionData == null)
                    text = li.Text;
                else
                    text = li.CodeCompletionData.Description;
                if (text.Length > 0)
                {
                    SetLocation();
                    _declaration.Description = text;
                    if (this.Visible)
                        _declaration.ShowDeclarationViewWindow();
                    else
                        _declaration.Hide();
                }
            }
        }

        private void LB_MouseDown(object sender, MouseEventArgs e)
        {
            SelectItem(e.X, e.Y);
        }

        private void SelectItem(int x, int y)
        {
            Point p = new Point(x, y);
            int r = (p.Y / _LB.ItemHeight) + _LB.TopIndex;
            if (r != _LB.SelectedIndex)
            {
                if (r < _LB.Items.Count && r >= 0)
                {
                    _LB.SelectedIndex = r;
                }
            }
        }

        private void LB_MouseMove(object sender, MouseEventArgs e)
        {
            //	if (e.Button != 0)
            //	{
            //SelectItem(e.X, e.Y);
            //	}
        }

        internal static Regex whitespace = new Regex(@"\s+");
        /// <summary>
        /// Converts the xml documentation string into a plain text string.
        /// </summary>
        private static string GetDocumentation(string ExtendedData)
        {
            System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + ExtendedData + "</docroot>");
            XmlTextReader xml = new XmlTextReader(reader);
            StringBuilder ret = new StringBuilder();
            ////Regex whitespace    = new Regex(@"\s+");
            bool blnExceptionStarted = false;

            try
            {
                xml.Read();
                do
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        string elname = xml.Name.ToLowerInvariant();
                        switch (elname)
                        {
                            case "filterpriority":
                                xml.Skip();
                                break;
                            case "remarks":
                                ret.Append(Environment.NewLine);
                                ret.Append("Remarks:");
                                ret.Append(Environment.NewLine);
                                break;
                            case "example":
                                ret.Append(Environment.NewLine);
                                ret.Append("Example:");
                                ret.Append(Environment.NewLine);
                                break;
                            case "exception":
                                ret.Append(Environment.NewLine);
                                if (blnExceptionStarted == false)
                                {
                                    ret.Append("Exception:");
                                    ret.Append(Environment.NewLine);
                                    blnExceptionStarted = true;
                                }
                                ret.Append(GetCref(xml["cref"]));
                                ret.Append(": ");
                                xml.Skip();
                                break;
                            case "returns":
                                xml.Skip();
                                //ret.Append(Environment.NewLine);
                                //ret.Append("Returns: ");
                                break;
                            case "see":
                                ret.Append(GetCref(xml["cref"]));
                                ret.Append(xml["langword"]);
                                break;
                            case "seealso":
                                ret.Append(Environment.NewLine);
                                ret.Append("See also: ");
                                ret.Append(GetCref(xml["cref"]));
                                break;
                            case "paramref":
                                ret.Append(xml["name"]);
                                break;
                            case "param":
                                ret.Append(Environment.NewLine);
                                ret.Append(whitespace.Replace(xml["name"].Trim(), " "));
                                ret.Append(": ");
                                break;
                            case "value":
                                ret.Append(Environment.NewLine);
                                ret.Append("Value: ");
                                ret.Append(Environment.NewLine);
                                break;
                            case "br":
                            case "para":
                                ret.Append(Environment.NewLine);
                                break;
                        }
                    }
                    else if (xml.NodeType == XmlNodeType.Text)
                    {
                        ret.Append(whitespace.Replace(xml.Value, " "));
                    }
                } while (xml.Read());
            }
            catch (Exception ex)
            {
                return ExtendedData;
            }
            return ret.ToString();
        }

        private static string GetCref(string cref)
        {
            if (cref == null || cref.Trim().Length == 0)
            {
                return "";
            }
            if (cref.Length < 2)
            {
                return cref;
            }
            if (cref.Substring(1, 1) == ":")
            {
                return cref.Substring(2, cref.Length - 2);
            }
            return cref;
        }
    }
}