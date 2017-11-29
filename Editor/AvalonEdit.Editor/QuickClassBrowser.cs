using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using VSProvider;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Panel with two combo boxes. Used to quickly navigate to entities in the current file.
    /// </summary>
    public partial class QuickClassBrowser
    {
        public EditorWindow editorWindow { get; set; }

        public List<TypeDeclarationSyntax> symbols { get; set; }

        public string filename { get; set; }
        public ComboBox namespaceComboBox { get; set; }
        public ComboBox classComboBox { get; set; }
        public ComboBox membersComboBox { get; set; }

        // type codes are used for sorting entities by type
        private const int TYPE_CLASS = 0;

        private const int TYPE_CONSTRUCTOR = 1;
        private const int TYPE_METHOD = 2;
        private const int TYPE_PROPERTY = 3;
        private const int TYPE_FIELD = 4;
        private const int TYPE_EVENT = 5;

        /// <summary>
        /// ViewModel used for combobox items.
        /// </summary>
        private class EntityItem : IComparable<EntityItem>
        {
            private MemberDeclarationSyntax entity;
            private INamespaceSymbol entityNamespace { get; set; }
            public ImageSource image { get; set; }
            public string text { get; set; }
            public SymbolKind typeCode; // type code is used for sorting entities by type

            public MemberDeclarationSyntax Entity
            {
                get { return entity; }
            }

            public EntityItem(MemberDeclarationSyntax entity)
            {
                this.IsInSamePart = true;
                this.entity = entity;

                if (entity is TypeDeclarationSyntax)
                    text = ((TypeDeclarationSyntax)entity).Identifier.ToString();
                else if (entity is ConstructorDeclarationSyntax)
                    text = ((ConstructorDeclarationSyntax)entity).Identifier.ToString();
                else if (entity is MethodDeclarationSyntax)
                    text = ((MethodDeclarationSyntax)entity).Identifier.ToString();
                else if (entity is PropertyDeclarationSyntax)
                    text = ((PropertyDeclarationSyntax)entity).Identifier.ToString();
                else if (entity is FieldDeclarationSyntax)
                {
                    FieldDeclarationSyntax f = ((FieldDeclarationSyntax)entity);
                    if (f.Declaration.Variables != null)
                    {
                        if (f.Declaration.Variables.Count > 0)
                            text = f.Declaration.Variables[0].Identifier.ToString();
                    }
                }
                else if (entity is NamespaceDeclarationSyntax)
                {
                    text = ((NamespaceDeclarationSyntax)entity).Name.ToString();
                }
                image = CompletionControls.GetImageData(entity).Source;
            }

            public EntityItem(INamespaceSymbol entity)
            {
                this.IsInSamePart = true;
                this.entityNamespace = entity;
                text = entity.Name;
                image = CompletionControls.GetImageData(entity).Source;
            }

            /// <summary>
            /// Text to display in combo box.
            /// </summary>
            public string Text
            {
                get { return text; }
            }

            /// <summary>
            /// Image to use in combox box
            /// </summary>
            public ImageSource Image
            {
                get
                {
                    return image;
                }
            }

            /// <summary>
            /// Gets/Sets whether the item is in the current file.
            /// </summary>
            /// <returns>
            /// <c>true</c>: item is in current file;
            /// <c>false</c>: item is in another part of the partial class
            /// </returns>
            public bool IsInSamePart { get; set; }

            public int CompareTo(EntityItem other)
            {
                int r = this.typeCode.CompareTo(other.typeCode);
                if (r != 0)
                    return r;
                r = string.Compare(text, other.text, StringComparison.OrdinalIgnoreCase);
                if (r != 0)
                    return r;
                return string.Compare(text, other.text, StringComparison.Ordinal);
            }

            /// <summary>
            /// ToString override is necessary to support keyboard navigation in WPF
            /// </summary>
            public override string ToString()
            {
                return text;
            }
        }

        public QuickClassBrowser()
        {
            //InitializeComponent();
        }

        public SemanticModel model { get; set; }

        public void Load(VSSolution vs, string FileToLoad, string content)
        {
            SyntaxTree synTree = vs.GetSyntaxTree(FileToLoad, content);
            if (synTree == null)
                return;

            model = vs.GetSemanticModel(FileToLoad);

            List<TypeDeclarationSyntax> s = synTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().ToList();

            List<NamespaceDeclarationSyntax> n = synTree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

            DoUpdate(s, n);
        }

        public void AttachEvents()
        {
            classComboBox.SelectionChanged += classComboBoxSelectionChanged;
            membersComboBox.SelectionChanged += membersComboBoxSelectionChanged;
        }

        /// <summary>
        /// Updates the list of available classes.
        /// This causes the classes combo box to lose its current selection,
        /// so the members combo box will be cleared.
        /// </summary>
        public void Update(List<TypeDeclarationSyntax> symbols)
        {
            this.symbols = symbols;
            runUpdateWhenDropDownClosed = true;
            //runUpdateWhenDropDownClosedCU = compilationUnit;
            if (!IsDropDownOpen)
                ComboBox_DropDownClosed(null, null);
        }

        // The lists of items currently visible in the combo boxes.
        // These should never be null.
        private List<EntityItem> classItems = new List<EntityItem>();

        private List<EntityItem> memberItems = new List<EntityItem>();

        public void DoUpdateForContent(TypeDeclarationSyntax symbol, List<TypeDeclarationSyntax> symbols)
        {
            if (symbols == null)
                return;

            this.symbols = symbols;

            this.symbolsForContent = symbols;

            classItems = new List<EntityItem>();

            //var c = symbols.ToArray().Select(s => s).Where(s => s.Kind == SymbolKind.NamedType);

            foreach (TypeDeclarationSyntax s in symbols)
                classItems.Add(new EntityItem(s));

            classItems.Sort();
            classComboBox.ItemsSource = classItems;

            var namespaceItems = new List<EntityItem>();

            if (symbol == null)
                return;

            var c = model.GetTypeInfo(symbol).Type.ContainingNamespace;

            //foreach (ISymbol s in c)
            namespaceItems.Add(new EntityItem(c));

            namespaceItems.Sort();
            namespaceComboBox.ItemsSource = namespaceItems;
        }

        private List<TypeDeclarationSyntax> symbolsForContent { get; set; }

        public void DoUpdate(List<TypeDeclarationSyntax> symbols, List<NamespaceDeclarationSyntax> n)
        {
            if (symbols == null)
                return;

            this.symbols = symbols;

            classItems = new List<EntityItem>();

            var c = symbols.ToArray().Select(s => s).Where(s => s is TypeDeclarationSyntax);

            foreach (TypeDeclarationSyntax s in c)
                classItems.Add(new EntityItem(s));

            classItems.Sort();
            classComboBox.ItemsSource = classItems;

            var namespaceItems = new List<EntityItem>();

            //var dg = model.GetDiagnostics();

            //foreach (TypeDeclarationSyntax s in c)
            //{
            //    var ts = model.GetTypeInfo(s);
            //    if (ts.Type != null)
            //    {
            //        var ns = ts.Type.ContainingNamespace;
            //        namespaceItems.Add(new EntityItem(ns));
            //    }
            //}
            foreach (NamespaceDeclarationSyntax s in n)
            {
                namespaceItems.Add(new EntityItem(s));
            }
            namespaceItems.Sort();
            namespaceComboBox.ItemsSource = namespaceItems;
        }

        private bool IsDropDownOpen
        {
            get { return classComboBox.IsDropDownOpen || membersComboBox.IsDropDownOpen; }
        }

        // Delayed execution - avoid changing combo boxes while the user is browsing the dropdown list.
        private bool runUpdateWhenDropDownClosed;

        private List<ISymbol> runUpdateWhenDropDownClosedCU;
        private bool runSelectItemWhenDropDownClosed;
        private Location runSelectItemWhenDropDownClosedLocation;

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (runUpdateWhenDropDownClosed)
            {
                runUpdateWhenDropDownClosed = false;
                //DoUpdate(runUpdateWhenDropDownClosedCU);
                runUpdateWhenDropDownClosedCU = null;
            }
            if (runSelectItemWhenDropDownClosed)
            {
                runSelectItemWhenDropDownClosed = false;
                DoSelectItem(runSelectItemWhenDropDownClosedLocation);
            }
        }

        public bool CaretPositionAdjustNeeded = true;

        /// <summary>
        /// Selects the class and member closest to the specified location.
        /// </summary>
        public void SelectItemAtCaretPosition(Location location)
        {
            runSelectItemWhenDropDownClosed = true;
            //runSelectItemWhenDropDownClosedLocation = location;
            //if (!IsDropDownOpen)
            //    ComboBox_DropDownClosed(null, null);
            DoSelectItem(location);
        }

        private void DoSelectItem(Location location)
        {
            EntityItem matchInside = null;
            EntityItem nearestMatch = null;
            int nearestMatchDistance = int.MaxValue;
            foreach (EntityItem item in classItems)
            {
                if (item.IsInSamePart)
                {
                    TypeDeclarationSyntax c = (TypeDeclarationSyntax)item.Entity;
                    if (c.Span.IntersectsWith(location.SourceSpan))
                    {
                        matchInside = item;
                        // when there are multiple matches inside (nested classes), use the last one
                    }
                    else
                    {
                        // Not a perfect match?
                        // Try to first the nearest match. We want the classes combo box to always
                        // have a class selected if possible.
                        int matchDistance = Math.Min(Math.Abs(location.GetLineSpan().StartLinePosition.Line - c.Span.Start),
                                                     location.GetLineSpan().EndLinePosition.Line - c.Span.End);
                        if (matchDistance < nearestMatchDistance)
                        {
                            nearestMatchDistance = matchDistance;
                            nearestMatch = item;
                        }
                    }
                }
            }
            jumpOnSelectionChange = false;
            try
            {
                classComboBox.SelectedItem = matchInside ?? nearestMatch;
                // the SelectedItem setter will update the list of member items
                if (classComboBox.SelectedIndex < 0)
                    if (classComboBox.Items.Count > 0)
                        classComboBox.SelectedIndex = 0;
            }
            finally
            {
                jumpOnSelectionChange = true;
            }
            matchInside = null;
            foreach (EntityItem item in memberItems)
            {
                if (item.IsInSamePart)
                {
                    MemberDeclarationSyntax member = (MemberDeclarationSyntax)item.Entity;
                    if (member.Span.OverlapsWith(location.SourceSpan))
                    {
                        matchInside = item;
                    }
                }
            }
            jumpOnSelectionChange = false;
            try
            {
                membersComboBox.SelectedItem = matchInside;
                if (matchInside == null)
                    if (membersComboBox.Items.Count > 0)
                        membersComboBox.SelectedIndex = 0;
            }
            finally
            {
                jumpOnSelectionChange = true;
            }
            if (namespaceComboBox.Items.Count > 0)
                namespaceComboBox.SelectedIndex = 0;
        }

        public bool jumpOnSelectionChange = false;

        public bool jumpOnSelectionChanged = false;

        public List<MemberDeclarationSyntax> allMembers { get; set; }

        public List<int> mb { get; set; }

        public void UpdateSourceDocument()
        {
            allMembers = new List<MemberDeclarationSyntax>();

            mb = new List<int>();
            // The selected class was changed.
            // Update the list of member items to be the list of members of the current class.
            foreach (EntityItem item in classComboBox.Items)
            {
                TypeDeclarationSyntax selectedClass = item is TypeDeclarationSyntax ? item.Entity as TypeDeclarationSyntax : null;
                if (selectedClass != null)
                {
                    //IClass compoundClass = selectedClass.GetCompoundClass();
                    foreach (var m in selectedClass.Members)
                    {
                        //    AddMember(selectedClass, m, m.IsConstructor ? TYPE_CONSTRUCTOR : TYPE_METHOD);
                        //}
                        if (m.Kind() == SyntaxKind.MethodDeclaration)
                        {
                            MethodDeclarationSyntax f = m as MethodDeclarationSyntax;
                            //if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind() == SyntaxKind.PropertyDeclaration)
                        {
                            //AddMember(selectedClass, m, m.Kind);
                            //allMembers.Add(m);
                        }
                        else
                        if (m.Kind() == SyntaxKind.FieldDeclaration)
                        {
                            FieldDeclarationSyntax f = m as FieldDeclarationSyntax;
                            //if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                //allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind() == SyntaxKind.EventDeclaration)
                        {
                            //AddMember(selectedClass, m, m.Kind);
                            //                allMembers.Add(m);
                        }
                        //memberItems.Sort();
                        if (jumpOnSelectionChange)
                        {
                            //AnalyticsMonitorService.TrackFeature(GetType(), "JumpToClass");
                            //JumpTo(item, selectedClass.Region);
                        }
                    }
                    //membersComboBox.ItemsSource = memberItems;
                }
            }
            Dictionary<int, ISymbol> dict = new Dictionary<int, ISymbol>();

            foreach (ISymbol s in allMembers)
            {
                if (!dict.ContainsKey(s.Locations[0].SourceSpan.Start))
                {
                    if (s.Locations[0].IsInSource)
                    {
                        if (s.Locations[0].SourceTree.FilePath == filename)
                        {
                            mb.Add(s.Locations[0].SourceSpan.Start);
                            dict.Add(s.Locations[0].SourceSpan.Start, s);
                        }
                    }
                }
            }

            mb.Sort();
            mb.Reverse();

            editorWindow.UpdateReferences(mb, dict);
        }

        private void classComboBoxSelectionChangedForContent()
        {
            allMembers = new List<MemberDeclarationSyntax>();

            mb = new List<int>();

            EntityItem item = classComboBox.SelectedItem as EntityItem;
            if (item == null)
                return;
            TypeDeclarationSyntax selectedClass = item.Entity is TypeDeclarationSyntax ? item.Entity as TypeDeclarationSyntax : null;
            memberItems = new List<EntityItem>();
            if (selectedClass != null)
            {
                foreach (var m in selectedClass.Members)
                {
                    if (m.Kind() == SyntaxKind.ConstructorDeclaration)
                    {
                        ConstructorDeclarationSyntax f = m as ConstructorDeclarationSyntax;
                        //if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind() == SyntaxKind.MethodDeclaration)
                    {
                        MethodDeclarationSyntax f = m as MethodDeclarationSyntax;
                        //if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind() == SyntaxKind.PropertyDeclaration)
                    {
                        AddMember(selectedClass, m);
                        allMembers.Add(m);
                    }
                    else
                    if (m.Kind() == SyntaxKind.FieldDeclaration)
                    {
                        FieldDeclarationSyntax f = m as FieldDeclarationSyntax;
                        //if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind() == SyntaxKind.EventDeclaration)
                    {
                        AddMember(selectedClass, m);
                        allMembers.Add(m);
                    }
                    //memberItems.Sort();
                    if (jumpOnSelectionChange)
                    {
                        //AnalyticsMonitorService.TrackFeature(GetType(), "JumpToClass");
                        //JumpTo(item, selectedClass.Region);
                    }
                }
                membersComboBox.ItemsSource = memberItems;
            }

            Dictionary<int, ISymbol> dict = new Dictionary<int, ISymbol>();

            foreach (ISymbol s in allMembers)
            {
                if (dict.ContainsKey(s.Locations[0].SourceSpan.Start))
                    continue;
                mb.Add(s.Locations[0].SourceSpan.Start);
                dict.Add(s.Locations[0].SourceSpan.Start, s);
            }
            mb.Sort();
            mb.Reverse();
            string d = "";
            // foreach (int i in mb)
            //     d += i.ToString() + " - ";
            //MessageBox.Show("Update " + d);

            //editorWindow.UpdateReferences(mb,dict);
        }

        private void classComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (symbolsForContent != null)
            {
                classComboBoxSelectionChangedForContent();
                return;
            }
            allMembers = new List<MemberDeclarationSyntax>();
            mb = new List<int>();

            EntityItem item = classComboBox.SelectedItem as EntityItem;
            if (item == null)
                return;
            TypeDeclarationSyntax selectedClass = item.Entity is TypeDeclarationSyntax ? item.Entity as TypeDeclarationSyntax : null;
            memberItems = new List<EntityItem>();
            if (selectedClass != null)
            {
                foreach (var m in selectedClass.Members)
                {
                    if (m.Kind() == SyntaxKind.ConstructorDeclaration)
                    {
                        ConstructorDeclarationSyntax f = m as ConstructorDeclarationSyntax;
                        //if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m);
                            allMembers.Add(m);
                        }
                    }
                    else
                   if (m.Kind() == SyntaxKind.MethodDeclaration)
                    {
                        MethodDeclarationSyntax f = m as MethodDeclarationSyntax;
                        //if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind() == SyntaxKind.PropertyDeclaration)
                    {
                        AddMember(selectedClass, m);
                        allMembers.Add(m);
                    }
                    else
                    if (m.Kind() == SyntaxKind.FieldDeclaration)
                    {
                        FieldDeclarationSyntax f = m as FieldDeclarationSyntax;
                        //if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind() == SyntaxKind.EventDeclaration)
                    {
                        AddMember(selectedClass, m);
                        allMembers.Add(m);
                    }
                    //memberItems.Sort();
                    if (jumpOnSelectionChange)
                    {
                        //AnalyticsMonitorService.TrackFeature(GetType(), "JumpToClass");
                        //JumpTo(item, selectedClass.Region);
                    }
                }
                membersComboBox.ItemsSource = memberItems;
            }

            Dictionary<int, MemberDeclarationSyntax> dict = new Dictionary<int, MemberDeclarationSyntax>();

            foreach (MemberDeclarationSyntax s in allMembers)
            {
                if (dict.ContainsKey(s.Span.Start))
                    continue;
                mb.Add(s.Span.Start);
                dict.Add(s.Span.Start, s);
            }
            mb.Sort();
            mb.Reverse();
            string d = "";
            // foreach (int i in mb)
            //     d += i.ToString() + " - ";
            //MessageBox.Show("Update " + d);

            //editorWindow.UpdateReferences(mb,dict);
        }

        private void AddMember(TypeDeclarationSyntax c, MemberDeclarationSyntax member)
        {
            bool isInSamePart = true;// (member.DeclaringType == selectedClass);
            memberItems.Add(new EntityItem(member));
        }

        private void membersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CaretPositionAdjustNeeded = false;
            jumpOnSelectionChanged = true;
            EntityItem item = membersComboBox.SelectedItem as EntityItem;
            if (item != null)
            {
                //ISymbol member = item.Entity as ISymbol;
                MemberDeclarationSyntax member = item.Entity as MemberDeclarationSyntax;
                if (member != null && jumpOnSelectionChange)
                {
                    //AnalyticsMonitorService.TrackFeature(GetType(), "JumpToMember");
                    //JumpTo(item, member.Region);
                    editorWindow.View(member.SpanStart);
                }
            }
        }

        //void JumpTo(EntityItem item, Location region)
        //{
        //    if (region == Location.None)
        //        return;
        //    Action<int, int> jumpAction = this.JumpAction;
        //    if (item.IsInSamePart && jumpAction != null)
        //    {
        //        //jumpAction(region.BeginLine, region.BeginColumn);
        //    }
        //    else
        //    {
        //        //FileService.JumpToFilePosition(item.Entity.CompilationUnit.FileName, region.BeginLine, region.BeginColumn);
        //    }
        //}

        ///// <summary>
        ///// Action used for jumping to a position inside the current file.
        ///// </summary>
        //public Action<int, int> JumpAction { get; set; }

        public void Dispose()
        {
            symbols = null;
        }
    }
}