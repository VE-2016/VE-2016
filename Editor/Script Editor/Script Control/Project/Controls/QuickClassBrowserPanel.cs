using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using VSParsers;
using static VSParsers.CSParsers;



namespace AIMS.Libraries.Scripting.ScriptControl
{
    public class QuickClassBrowserPanel : System.Windows.Forms.UserControl
    {
        public System.Windows.Forms.ComboBox namespaceComboBox;
        private System.Windows.Forms.ComboBox _classComboBox;
        private System.Windows.Forms.ComboBox _membersComboBox;

        //private ICompilationUnit currentCompilationUnit;
        public CodeEditorControl textAreaControl;
        private ComboBox _comboBox1;
        private bool _autoselect = true;
        public Document doc { get; set; }

        private class ComboBoxItem : System.IComparable
        {
            private object _item;
            private string _text;
            private int _iconIndex;
            private bool _isInCurrentPart;
            public string imagekey;

            public int IconIndex
            {
                get
                {
                    return _iconIndex;
                }
            }

            public object Item
            {
                get
                {
                    return _item;
                }
            }

            public bool IsInCurrentPart
            {
                get
                {
                    return _isInCurrentPart;
                }
            }

            public DomRegion ItemRegion
            {
                get
                {
                    IClass classItem = _item as IClass;
                    if (_item is IClass)
                        return ((IClass)_item).Region;
                    else if (_item is IMember)
                        return ((IMember)_item).Region;
                    else
                        return DomRegion.Empty;
                }
            }

            public int Line
            {
                get
                {
                    DomRegion r = this.ItemRegion;
                    if (r.IsEmpty)
                        return 0;
                    else
                        return r.BeginLine - 1;
                }
            }

            public int Column
            {
                get
                {
                    DomRegion r = this.ItemRegion;
                    if (r.IsEmpty)
                        return 0;
                    else
                        return r.BeginColumn - 1;
                }
            }

            public int EndLine
            {
                get
                {
                    DomRegion r = this.ItemRegion;
                    if (r.IsEmpty)
                        return 0;
                    else
                        return r.EndLine - 1;
                }
            }

            public ComboBoxItem(object item, string text, string imagekey, bool isInCurrentPart)
            {
                _item = item;
                _text = text;
                this.imagekey = imagekey;
                _isInCurrentPart = isInCurrentPart;
            }

            public ComboBoxItem(object item, string text, int iconIndex, bool isInCurrentPart, string itemkey)
            {
                _item = item;
                _text = text;
                //this.iconIndex = iconIndex;
                this.imagekey = itemkey;
                _isInCurrentPart = isInCurrentPart;
            }

            public bool IsInside(int lineNumber)
            {
                //if (!isInCurrentPart)
                //    return false;
                EntityDeclaration classItem = _item as EntityDeclaration;
                if (classItem != null)
                {
                    if (classItem.Region.IsEmpty)
                        return false;
                    return classItem.Region.BeginLine - 1 <= lineNumber &&
                        classItem.Region.EndLine - 1 >= lineNumber;
                }

                EntityDeclaration member = _item as EntityDeclaration;
                if (member == null || member.Region.IsEmpty)
                {
                    return false;
                }
                bool isInside = member.Region.BeginLine - 1 <= lineNumber;

                if (member is TypeDeclaration)
                {
                    if (((TypeDeclaration)member).Region.EndLine >= 0)
                    {
                        isInside &= lineNumber <= ((TypeDeclaration)member).Region.EndLine - 1;
                    }
                    else
                    {
                        return member.Region.BeginLine - 1 == lineNumber;
                    }
                }
                else
                {
                    isInside &= lineNumber <= member.Region.EndLine - 1;
                }
                return isInside;
            }

            public int CompareItemTo(object obj)
            {
                ComboBoxItem boxItem = (ComboBoxItem)obj;

                if (boxItem.Item is IComparable)
                {
                    return ((IComparable)boxItem.Item).CompareTo(_item);
                }
                if (boxItem._text != _text || boxItem.Line != Line || boxItem.EndLine != EndLine || boxItem._iconIndex != _iconIndex)
                {
                    return 1;
                }
                return 0;
            }

            private string _cachedString;

            public override string ToString()
            {
                // ambience lookups can be expensive when the return type is
                // resolved on the fly.
                // Therefore, we need to cache the generated string because it is used
                // very often for the sorting.
                if (_cachedString == null)
                    _cachedString = ToStringInternal();
                return _cachedString;
            }

            private string ToStringInternal()
            {
                IAmbience ambience = Parser.ProjectParser.CurrentAmbience;
                ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
                if (_item is IMethod)
                {
                    return ambience.Convert((IMethod)_item);
                }
                if (_item is IProperty)
                {
                    return ambience.Convert((IProperty)_item);
                }
                if (_item is IField)
                {
                    return ambience.Convert((IField)_item);
                }
                if (_item is IProperty)
                {
                    return ambience.Convert((IProperty)_item);
                }
                if (_item is IEvent)
                {
                    return ambience.Convert((IEvent)_item);
                }
                return _text;
            }

            #region System.IComparable interface implementation

            public int CompareTo(object obj)
            {
                return ToString().CompareTo(obj.ToString());
            }

            #endregion System.IComparable interface implementation
        }

        public QuickClassBrowserPanel() : this(null)
        {
            namespaceComboBox = _comboBox1;
        }

        public QuickClassBrowserPanel(CodeEditorControl textAreaControl)
        {
            InitializeComponent();
            _membersComboBox.MaxDropDownItems = 20;
            
            base.Dock = DockStyle.Top;
            if (textAreaControl != null)
                this.Editor = textAreaControl;
        }

        public CodeEditorControl Editor
        {
            get { return this.textAreaControl; }
            set
            {
                this.textAreaControl = value;
                this.textAreaControl.CaretChange += new EventHandler(CaretPositionChanged);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.textAreaControl != null) this.textAreaControl.CaretChange -= new EventHandler(CaretPositionChanged);
                _membersComboBox.Dispose();
                _classComboBox.Dispose();
            }
            base.Dispose(disposing);
        }

        public async void PopulateCombo()
        {
            // ignore simple movements





            try
            {
                running = true;

                await Task.Delay(2000);

                mappers maps = VSParsers.CSParsers.AnalyzeCSharpFile(textAreaControl.FileName, textAreaControl.Document.Text);

                //ParseInformation parseInfo = Parser.ProjectParser.GetParseInformation(textAreaControl.FileName);
                //if (parseInfo != null)
                //{
                //    if (currentCompilationUnit != (ICompilationUnit)parseInfo.MostRecentCompilationUnit)
                {
                    //currentCompilationUnit = (ICompilationUnit)parseInfo.MostRecentCompilationUnit;
                    //if (currentCompilationUnit != null)
                    {
                        FillClassComboBox(true, maps);
                        FillMembersComboBox();
                    }
                }
                // else
                {
                    // UpdateClassComboBox();
                    UpdateMembersComboBox();
                }
                //}

                running = false;
            }
            catch (Exception ex)
            {
                running = false;

                
            }
        }

        private System.Threading.Timer updateTimer { get; set; }

        //System.Timers.Timer _timer = new System.Timers.Timer();

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            // milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }
         

            UpdateComboBox(null);
        }

        public bool running = false;

        public void UpdateComboBox(object state)
        {
            if (running == false)
                this.BeginInvoke(new Action(() =>
                {
                    UpdateClassComboBox();
                    UpdateMembersComboBox();
                }));
        }

        private bool _membersComboBoxSelectedMember = false;

        private void UpdateMembersComboBox()
        {
            _autoselect = false;
            try
            {
                //if (currentCompilationUnit != null)
                {
                    for (int i = 0; i < _membersComboBox.Items.Count; ++i)
                    {
                        if (((ComboBoxItem)_membersComboBox.Items[i]).IsInside(textAreaControl.ActiveViewControl.Caret.Position.Y))
                        {
                            if (_membersComboBox.SelectedIndex != i)
                            {
                                _membersComboBox.SelectedIndex = i;
                            }
                            if (!_membersComboBoxSelectedMember)
                            {
                                _membersComboBox.Refresh();
                            }
                            _membersComboBoxSelectedMember = true;
                            return;
                        }
                    }
                }
                _membersComboBox.SelectedIndex = -1;
                if (_membersComboBoxSelectedMember)
                {
                    _membersComboBox.Refresh();
                    _membersComboBoxSelectedMember = false;
                }
            }
            finally
            {
                _autoselect = true;
            }
        }

        private bool _classComboBoxSelectedMember = false;

        public int lines = 0;

        private void UpdateClassComboBox()
        {
            if (lines != textAreaControl.Document.Count)
            {
                PopulateCombo();

                lines = textAreaControl.Document.Count;
            }


            // Still needed ?
            //if (currentCompilationUnit == null)
            {
                //currentCompilationUnit = (ICompilationUnit)Parser.ProjectParser.GetParseInformation(Path.GetFullPath(textAreaControl.FileName)).MostRecentCompilationUnit;
            }

            _autoselect = false;
            try
            {
                //if (currentCompilationUnit != null)
                {
                    //// Alex: when changing between files in different compilation units whole process must be restarted
                    //// happens usually when files are opened from different project(s)
                    for (int i = 0; i < _classComboBox.Items.Count; ++i)
                    {
                        if (((ComboBoxItem)_classComboBox.Items[i]).IsInside(textAreaControl.ActiveViewControl.Caret.Position.Y))
                        {
                            bool innerClassContainsCaret = false;
                            for (int j = i + 1; j < _classComboBox.Items.Count; ++j)
                            {
                                if (((ComboBoxItem)_classComboBox.Items[j]).IsInside(textAreaControl.ActiveViewControl.Caret.Position.Y))
                                {
                                    innerClassContainsCaret = true;
                                    break;
                                }
                            }
                            if (!innerClassContainsCaret)
                            {
                                if (_classComboBox.SelectedIndex != i)
                                {
                                    _classComboBox.SelectedIndex = i;
                                    FillMembersComboBox();
                                }
                                if (!_classComboBoxSelectedMember)
                                {
                                    _classComboBox.Refresh();
                                }
                                _classComboBoxSelectedMember = true;
                                return;
                            }
                        }
                    }
                }
                if (_classComboBoxSelectedMember)
                {
                    _classComboBox.Refresh();
                    _classComboBoxSelectedMember = false;
                }
            }
            finally
            {
                _autoselect = true;
            }
            //				classComboBox.SelectedIndex = -1;
        }

        private bool NeedtoUpdate(ArrayList items, ComboBox comboBox)
        {
            if (items.Count != comboBox.Items.Count)
            {
                return true;
            }
            for (int i = 0; i < items.Count; ++i)
            {
                ComboBoxItem oldItem = (ComboBoxItem)comboBox.Items[i];
                ComboBoxItem newItem = (ComboBoxItem)items[i];
                if (oldItem.GetType() != newItem.GetType())
                {
                    return true;
                }
                if (newItem.CompareItemTo(oldItem) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private EntityDeclaration _lastClassInMembersComboBox;

        private void FillMembersComboBox()
        {
            EntityDeclaration c = GetCurrentSelectedClass();
            if (c != null && _lastClassInMembersComboBox != c)
            {
                _lastClassInMembersComboBox = c;
                ArrayList items = new ArrayList();
                bool partialMode = false;
                EntityDeclaration currentPart = c;
                //if (c.iIsPartial)
                //{
                //    CompoundClass cc = c.GetCompoundClass() as CompoundClass;
                //    if (cc != null)
                //    {
                //        partialMode = true;
                //        c = cc;
                //    }
                //}
                // TODO lock has been removed...
                //lock (c)
                {
                    if (c.GetType() == typeof(DelegateDeclaration))
                    {
                        DelegateDeclaration de = (DelegateDeclaration)c;

                        if (de != null)
                        {
                            items.Add(new ComboBoxItem(de, de.Name, (int)0, true, "delegate"));
                        }
                    }

                    if (c.GetType() == typeof(TypeDeclaration))
                    {
                        TypeDeclaration dd = (TypeDeclaration)c;

                        int lastIndex = 0;
                        IComparer comparer = new Comparer(System.Globalization.CultureInfo.InvariantCulture);

                        foreach (EntityDeclaration m in dd.Members)
                        {
                            Type T = m.GetType();

                            if (m.GetType() == typeof(MethodDeclaration))
                            {
                                items.Add(new ComboBoxItem(m, m.Name, (int)0, true, "method"));
                            }
                            //items.Sort(lastIndex, c.Methods.Count, comparer);
                            lastIndex = items.Count;

                            if (m.GetType() == typeof(ConstructorDeclaration))
                            {
                                items.Add(new ComboBoxItem(m, m.Name, (int)0, true, "Class"));
                            }
                            //items.Sort(lastIndex, c.Methods.Count, comparer);
                            lastIndex = items.Count;

                            if (m.GetType() == typeof(PropertyDeclaration))
                            {
                                items.Add(new ComboBoxItem(m, m.Name, (int)0, true, "property"));
                            }
                            //items.Sort(lastIndex, c.Properties.Count, comparer);
                            lastIndex = items.Count;

                            if (m.GetType() == typeof(FieldDeclaration))
                            {
                                string s = m.Name;

                                FieldDeclaration g = (FieldDeclaration)m;

                                if (g.Variables != null)
                                    if (g.Variables.Count > 0)
                                    {
                                        AstNodeCollection<VariableInitializer> v = g.Variables;
                                        foreach (VariableInitializer vv in v)
                                            s = vv.Name;
                                    }

                                items.Add(new ComboBoxItem(m, s, (int)0, true, "Class"));
                            }
                            //items.Sort(lastIndex, c.Fields.Count, comparer);
                            lastIndex = items.Count;

                            if (m.GetType() == typeof(EventDeclaration))
                            {
                                items.Add(new ComboBoxItem(m, m.Name, (int)0, true, "Class"));
                            }
                        }

                        //items.Sort(lastIndex, c.Events.Count, comparer);
                        lastIndex = items.Count;
                    }
                }

                _membersComboBox.BeginUpdate();
                _membersComboBox.Items.Clear();
                _membersComboBox.Items.AddRange(items.ToArray());
                _membersComboBox.EndUpdate();
                UpdateMembersComboBox();
            }
        }

        private void AddClasses(ArrayList items, mappers map)
        {
            //foreach (IClass c in classes)
            {
                //   items.Add(new ComboBoxItem(c, c.FullyQualifiedName, (int)ScriptControl.GetIcon(c), true, "Class"));
                //   AddClasses(items, c.InnerClasses, map);
            }
            foreach (TypeDeclaration d in map.Classes)
            {
                items.Add(new ComboBoxItem(d, d.Name, (int)0, true, "Class"));
                //AddClasses(items, c.InnerClasses, map);
            }
            foreach (DelegateDeclaration d in map.Delegates)
            {
                items.Add(new ComboBoxItem(d, d.Name, (int)0, true, "delegate"));
                //AddClasses(items, c.InnerClasses, map);
            }
        }

        private void FillNamespace(ArrayList items)
        {
            //foreach (IClass c in classes)
            {
                if (doc != null)
                {
                    string project = Path.GetExtension(doc.vp.FileName);

                    //CodeEditorControl.AutoListImages = doc.bmp;
                    //CodeEditorControl.AutoListImages.TransparentColor = Color.Fuchsia;

                    string name = "";

                    if (doc.vp != null)
                        name = doc.vp.Name;

                    if (VSProject.dc.ContainsKey(project) == true)
                        items.Add(new ComboBoxItem(null, name, VSProject.dc[project], true));
                    //else
                    //    items.Add(new ComboBoxItem(c, name, VSProject.dc[project], true));

                    return;
                }

                //items.Add(new ComboBoxItem(c, c.Namespace, (int)ScriptControl.GetIcon(c), true));
                return;
            }
        }

        private void FillClassComboBox(bool isUpdateRequired, mappers map)
        {
            ArrayList items = new ArrayList();

            ArrayList ns = new ArrayList();

            FillNamespace(ns);

            AddClasses(items, map);
            if (isUpdateRequired)
            {
                _classComboBox.BeginUpdate();
                namespaceComboBox.BeginUpdate();
            }
            namespaceComboBox.Items.Clear();
            _classComboBox.Items.Clear();
            _membersComboBox.Items.Clear();
            _classComboBox.Items.AddRange(items.ToArray());
            namespaceComboBox.Items.AddRange(ns.ToArray());
            if (items.Count == 1)
            {
                try
                {
                    _autoselect = false;
                    _classComboBox.SelectedIndex = 0;
                    FillMembersComboBox();
                }
                finally
                {
                    _autoselect = true;
                }
            }
            if (isUpdateRequired)
            {
                _classComboBox.EndUpdate();
                namespaceComboBox.EndUpdate();
            }
            // UpdateClassComboBox();

            namespaceComboBox.SelectedIndex = 0;
        }

        // THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
        // DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
        private void InitializeComponent()
        {
            this._membersComboBox = new System.Windows.Forms.ComboBox();
            this._classComboBox = new System.Windows.Forms.ComboBox();
            this._comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _membersComboBox
            // 
            this._membersComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._membersComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this._membersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._membersComboBox.Location = new System.Drawing.Point(419, 2);
            this._membersComboBox.Name = "_membersComboBox";
            this._membersComboBox.Size = new System.Drawing.Size(237, 21);
            this._membersComboBox.TabIndex = 2;
            this._membersComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
            this._membersComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
            this._membersComboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectedIndexChanged);
            // 
            // _classComboBox
            // 
            this._classComboBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this._classComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._classComboBox.Location = new System.Drawing.Point(208, 2);
            this._classComboBox.Name = "_classComboBox";
            this._classComboBox.Size = new System.Drawing.Size(205, 21);
            this._classComboBox.TabIndex = 1;
            this._classComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItem);
            this._classComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
            this._classComboBox.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSelectedIndexChanged);
            // 
            // _comboBox1
            // 
            this._comboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this._comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._comboBox1.Location = new System.Drawing.Point(0, 2);
            this._comboBox1.Name = "_comboBox1";
            this._comboBox1.Size = new System.Drawing.Size(202, 21);
            this._comboBox1.TabIndex = 0;
            this._comboBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ComboBoxDrawItems);
            this._comboBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.MeasureComboBoxItem);
            // 
            // QuickClassBrowserPanel
            // 
            this.Controls.Add(this._comboBox1);
            this.Controls.Add(this._membersComboBox);
            this.Controls.Add(this._classComboBox);
            this.Name = "QuickClassBrowserPanel";
            this.Size = new System.Drawing.Size(659, 25);
            this.Load += new System.EventHandler(this.QuickClassBrowserPanel_Load);
            this.Resize += new System.EventHandler(this.QuickClassBrowserPanelResize);
            this.ResumeLayout(false);

        }

        public EntityDeclaration GetCurrentSelectedClass()
        {
            if (_classComboBox.SelectedIndex >= 0)
            {
                return (EntityDeclaration)((ComboBoxItem)_classComboBox.Items[_classComboBox.SelectedIndex]).Item;
            }
            return null;
        }

        public IMember GetCurrentMember()
        {
            if (_membersComboBox.SelectedIndex < 0)
                return null;

            ComboBoxItem item = (ComboBoxItem)_membersComboBox.Items[_membersComboBox.SelectedIndex];

            IMember m = item.Item as IMember;

            if (m == null)
                return null;

            return m;
        }

        private void ComboBoxSelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                if (textAreaControl == null) return;
                if (_autoselect)
                {
                    ComboBoxItem item = (ComboBoxItem)comboBox.Items[comboBox.SelectedIndex];
                    //if (item.IsInCurrentPart)
                    //{
                    //    textAreaControl.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
                    //    textAreaControl.ActiveViewControl.ScrollIntoView();
                    //    textAreaControl.ActiveViewControl.Focus();
                    //}
                    //else
                    {
                        if (comboBox == _classComboBox)
                        {
                            FillMembersComboBox();
                            // UpdateMembersComboBox();
                        }

                        EntityDeclaration m = item.Item as EntityDeclaration;
                        if (m != null)
                        {
                            //string fileName = m.DeclaringType.CompilationUnit.FileName;
                            //if (fileName == this.textAreaControl.FileName)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    textAreaControl.ActiveViewControl.Focus();
                                    this.textAreaControl.ActiveViewControl.Caret.Position = new TextPoint(m.Region.BeginColumn, m.Region.BeginLine);
                                    textAreaControl.ActiveViewControl.GotoLine(m.Region.BeginLine);
                                    textAreaControl.ActiveViewControl.ScrollIntoView();
                                    textAreaControl.ActiveViewControl.Focus();
                                }));
                            }
                            //else
                            //{
                            //    Document doc = (Document)this.textAreaControl.Parent;
                            //    ScriptControl sc = (ScriptControl)doc.ParentScriptControl;
                            //    Document docnew = sc.ShowFile(fileName);
                            //    if (docnew != null)
                            //    {
                            //        docnew.ParseContentsNow();
                            //        docnew.Editor.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
                            //        docnew.Editor.ActiveViewControl.ScrollIntoView();
                            //        docnew.Editor.ActiveViewControl.Focus();
                            //    }
                            //    else
                            //    {
                            //        docnew = ((Document)this.textAreaControl.Parent);
                            //        docnew.ParseContentsNow();
                            //        docnew.Editor.ActiveViewControl.Caret.Position = new TextPoint(item.Column, item.Line);
                            //        docnew.Editor.ActiveViewControl.ScrollIntoView();
                            //        docnew.Editor.ActiveViewControl.Focus();
                            //    }
                            //}
                        }
                    }
                    if (comboBox == _classComboBox)
                    {
                        FillMembersComboBox();
                        // UpdateMembersComboBox();
                    }
                }
            }
            catch (Exception ee) { }
        }

        // font - has to be static - don't create on each draw
        private static Font s_font = s_font = new Font("Arial", 8.25f);

        private static StringFormat s_drawStringFormat = new StringFormat(StringFormatFlags.NoWrap);

        private void ComboBoxDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                e.DrawBackground();
                if (this.textAreaControl == null) return;
                if (e.Index >= 0)
                {
                    if (comboBox.Items.Count <= e.Index)
                        return;

                    ComboBoxItem item = (ComboBoxItem)comboBox.Items[e.Index];

                    if ((this.textAreaControl.AutoListIcons.Images.Count <= item.IconIndex))
                        return;

                    e.Graphics.DrawImageUnscaled(this.textAreaControl.AutoListIcons.Images[item.imagekey/*item.IconIndex*/],
                                                 new Point(e.Bounds.X, e.Bounds.Y + (e.Bounds.Height - this.textAreaControl.AutoListIcons.ImageSize.Height) / 2));
                    Rectangle drawingRect = new Rectangle(e.Bounds.X + this.textAreaControl.AutoListIcons.ImageSize.Width,
                                                          e.Bounds.Y,
                                                          e.Bounds.Width - this.textAreaControl.AutoListIcons.ImageSize.Width,
                                                          e.Bounds.Height);

                    Brush drawItemBrush = SystemBrushes.WindowText;
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        //   drawItemBrush = SystemBrushes.HighlightText;
                    }
                    if (!item.IsInCurrentPart)
                    {
                        drawItemBrush = SystemBrushes.ControlDark;
                    }
                    else if (e.State == DrawItemState.ComboBoxEdit && !item.IsInside(textAreaControl.ActiveViewControl.Caret.Position.Y))
                    {
                        drawItemBrush = SystemBrushes.ControlDark;
                    }
                    e.Graphics.DrawString(item.ToString(),
                                          s_font,
                                          drawItemBrush,
                                          drawingRect,
                                          s_drawStringFormat);
                }
                //e.DrawFocusRectangle();
            }
            catch (Exception ee) { }
        }

        private void ComboBoxDrawItems(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                e.DrawBackground();
                if (this.textAreaControl == null) return;
                if (e.Index == 0)
                {
                    ComboBoxItem item = (ComboBoxItem)comboBox.Items[e.Index];

                    if (CodeEditorControl.AutoListImages.Images.ContainsKey(item.imagekey) == false)
                        return;

                    e.Graphics.DrawImageUnscaled(CodeEditorControl.AutoListImages.Images[item.imagekey],
                                                 new Point(e.Bounds.X, e.Bounds.Y + (e.Bounds.Height - CodeEditorControl.AutoListImages.ImageSize.Height) / 2));
                    Rectangle drawingRect = new Rectangle(e.Bounds.X + CodeEditorControl.AutoListImages.ImageSize.Width,
                                                          e.Bounds.Y,
                                                          e.Bounds.Width - CodeEditorControl.AutoListImages.ImageSize.Width,
                                                          e.Bounds.Height);

                    Brush drawItemBrush = SystemBrushes.WindowText;
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        drawItemBrush = SystemBrushes.HighlightText;
                    }
                    if (!item.IsInCurrentPart)
                    {
                        drawItemBrush = SystemBrushes.ControlDark;
                    }
                    else if (e.State == DrawItemState.ComboBoxEdit && !item.IsInside(textAreaControl.ActiveViewControl.Caret.Position.Y))
                    {
                        drawItemBrush = SystemBrushes.ControlDark;
                    }
                    e.Graphics.DrawString(item.ToString(),
                                          s_font,
                                          drawItemBrush,
                                          drawingRect,
                                          s_drawStringFormat);
                }
                e.DrawFocusRectangle();
            }
            catch (Exception ee) { }
        }

        private void QuickClassBrowserPanelResize(object sender, System.EventArgs e)
        {
            try
            {
                namespaceComboBox = _comboBox1;
                Size comboBoxSize = new Size(Width / 3 - 4 * 3 + 8, 21);
                namespaceComboBox.Location = new Point(3, namespaceComboBox.Bounds.Top);
                namespaceComboBox.Size = comboBoxSize;

                //Size comboBoxSize = new Size(Width / 3 - 4 * 3, 21);
                _classComboBox.Location = new Point(namespaceComboBox.Bounds.Right + 4, _classComboBox.Bounds.Top);
                _classComboBox.Size = comboBoxSize;
                _membersComboBox.Location = new Point(_classComboBox.Bounds.Right + 4, _classComboBox.Bounds.Top);
                _membersComboBox.Size = comboBoxSize;
            }
            catch (Exception ee) { }
        }

        private void MeasureComboBoxItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                if (e.Index >= 0)
                {
                    ComboBoxItem item = (ComboBoxItem)comboBox.Items[e.Index];
                    SizeF size = e.Graphics.MeasureString(item.ToString(), s_font);
                    e.ItemWidth = (int)size.Width;
                    if (this.textAreaControl == null)
                        e.ItemHeight = (int)Math.Max(size.Height, 16);
                    else
                        e.ItemHeight = (int)Math.Max(size.Height, this.textAreaControl.AutoListIcons.ImageSize.Height);
                }
            }
            catch (Exception ee) { }
        }

        private void MeasureComboBoxItems(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
            try
            {
                ComboBox comboBox = (ComboBox)sender;
                if (e.Index >= 0)
                {
                    if (e.Index >= comboBox.Items.Count)
                        return;
                    ComboBoxItem item = (ComboBoxItem)comboBox.Items[e.Index];
                    SizeF size = e.Graphics.MeasureString(item.ToString(), s_font);
                    e.ItemWidth = (int)size.Width;
                    if (this.textAreaControl == null)
                        e.ItemHeight = (int)Math.Max(size.Height, 16);
                    else
                        e.ItemHeight = (int)Math.Max(size.Height, CodeEditorControl.AutoListImages.ImageSize.Height);
                }
            }
            catch (Exception ee) { }
        }

        private void QuickClassBrowserPanel_Load(object sender, EventArgs e)
        {
        }
    }
}