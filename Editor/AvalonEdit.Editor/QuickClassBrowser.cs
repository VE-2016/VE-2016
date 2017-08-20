using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Windows;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Panel with two combo boxes. Used to quickly navigate to entities in the current file.
    /// </summary>
    public partial class QuickClassBrowser
    {

        public EditorWindow editorWindow { get; set; }

        public List<ISymbol> symbols { get; set; }

        public string filename { get; set; }
        public ComboBox namespaceComboBox { get; set; }
        public ComboBox classComboBox { get; set; }
        public ComboBox membersComboBox { get; set; }

        // type codes are used for sorting entities by type
        const int TYPE_CLASS = 0;
        const int TYPE_CONSTRUCTOR = 1;
        const int TYPE_METHOD = 2;
        const int TYPE_PROPERTY = 3;
        const int TYPE_FIELD = 4;
        const int TYPE_EVENT = 5;

        /// <summary>
        /// ViewModel used for combobox items.
        /// </summary>
        class EntityItem : IComparable<EntityItem>
        {
            ISymbol entity;
            public ImageSource image { get; set; }
            public string text { get; set; }
            public SymbolKind typeCode; // type code is used for sorting entities by type

            public ISymbol Entity
            {
                get { return entity; }
            }

            public EntityItem(ISymbol entity, SymbolKind typeCode)
            {
                this.IsInSamePart = true;
                this.entity = entity;
                this.typeCode = typeCode;
                
                
                text = entity.Name;
                //image = CompletionControls.GetImageSource(entity);
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
        public void Update(List<ISymbol> symbols)
        {
            this.symbols = symbols;
            runUpdateWhenDropDownClosed = true;
            //runUpdateWhenDropDownClosedCU = compilationUnit;
            if (!IsDropDownOpen)
                ComboBox_DropDownClosed(null, null);
        }

        // The lists of items currently visible in the combo boxes.
        // These should never be null.
        List<EntityItem> classItems = new List<EntityItem>();
        List<EntityItem> memberItems = new List<EntityItem>();

        public void DoUpdate(List<ISymbol> symbols)
        {

            this.symbols = symbols;

            classItems = new List<EntityItem>();

            var c = symbols.ToArray().Select(s => s).Where(s => s.Kind == SymbolKind.NamedType);

            foreach (ISymbol s in c)
                classItems.Add(new EntityItem(s, s.Kind));
            
            classItems.Sort();
            classComboBox.ItemsSource = classItems;

            var namespaceItems = new List<EntityItem>();

            c = symbols.ToArray().Select(s => s).Where(s => s.Kind == SymbolKind.Namespace);

            foreach (ISymbol s in c)
                namespaceItems.Add(new EntityItem(s, s.Kind));

            namespaceItems.Sort();
            namespaceComboBox.ItemsSource = namespaceItems;
        }

        bool IsDropDownOpen
        {
            get { return classComboBox.IsDropDownOpen || membersComboBox.IsDropDownOpen; }
        }

        // Delayed execution - avoid changing combo boxes while the user is browsing the dropdown list.
        bool runUpdateWhenDropDownClosed;
       List<ISymbol> runUpdateWhenDropDownClosedCU;
        bool runSelectItemWhenDropDownClosed;
        Location runSelectItemWhenDropDownClosedLocation;

        void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (runUpdateWhenDropDownClosed)
            {
                runUpdateWhenDropDownClosed = false;
                DoUpdate(runUpdateWhenDropDownClosedCU);
                runUpdateWhenDropDownClosedCU = null;
            }
            if (runSelectItemWhenDropDownClosed)
            {
                runSelectItemWhenDropDownClosed = false;
                DoSelectItem(runSelectItemWhenDropDownClosedLocation);
            }
        }

       

        /// <summary>
        /// Selects the class and member closest to the specified location.
        /// </summary>
        public void SelectItemAtCaretPosition(Location location)
        {
            runSelectItemWhenDropDownClosed = true;
            runSelectItemWhenDropDownClosedLocation = location;
            if (!IsDropDownOpen)
                ComboBox_DropDownClosed(null, null);
        }

        void DoSelectItem(Location location)
        {
            EntityItem matchInside = null;
            EntityItem nearestMatch = null;
            int nearestMatchDistance = int.MaxValue;
            foreach (EntityItem item in classItems)
            {
                if (item.IsInSamePart)
                {
                    ISymbol c = (ISymbol)item.Entity;
                    if (c.Locations[0].SourceSpan.IntersectsWith(location.SourceSpan))
                    {
                        matchInside = item;
                        // when there are multiple matches inside (nested classes), use the last one
                    }
                    else
                    {
                        // Not a perfect match?
                        // Try to first the nearest match. We want the classes combo box to always
                        // have a class selected if possible.
                        int matchDistance = Math.Min(Math.Abs(location.GetLineSpan().StartLinePosition.Line - c.Locations[0].GetLineSpan().StartLinePosition.Line),
                                                     location.GetLineSpan().EndLinePosition.Line - c.Locations[0].GetLineSpan().EndLinePosition.Line);
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
                    
                    ISymbol member = (ISymbol)item.Entity;
                    if (member.Locations[0].SourceSpan.OverlapsWith(location.SourceSpan))
                    {
                        matchInside = item;
                    }
                }
            }
            jumpOnSelectionChange = false;
            try
            {
                membersComboBox.SelectedItem = matchInside;
            }
            finally
            {
                jumpOnSelectionChange = true;
            }
        }

        bool jumpOnSelectionChange = true;

        public List<ISymbol> allMembers { get; set; }

        public List<int> mb { get; set; }

        public void UpdateSourceDocument()
        {

           

            allMembers = new List<ISymbol>();
            

            mb = new List<int>();
            // The selected class was changed.
            // Update the list of member items to be the list of members of the current class.
            foreach (EntityItem item in classComboBox.Items)
            {
                INamedTypeSymbol selectedClass = item.typeCode == SymbolKind.NamedType ? item.Entity as INamedTypeSymbol : null;
                if (selectedClass != null)
                {

                    //IClass compoundClass = selectedClass.GetCompoundClass();
                    foreach (var m in selectedClass.GetMembers())
                    {
                        if (!m.Locations[0].IsInSource)
                            continue;



                        //    AddMember(selectedClass, m, m.IsConstructor ? TYPE_CONSTRUCTOR : TYPE_METHOD);
                        //}
                        if (m.Kind == SymbolKind.Method)
                        {
                            IMethodSymbol f = m as IMethodSymbol;
                            if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind == SymbolKind.Property)
                        {

                            //AddMember(selectedClass, m, m.Kind);
                            //allMembers.Add(m);
                        }
                        else
                        if (m.Kind == SymbolKind.Field)
                        {
                            IFieldSymbol f = m as IFieldSymbol;
                            if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                //allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind == SymbolKind.Event)
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

        void classComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            allMembers = new List<ISymbol>();

            

            mb = new List<int>();
            // The selected class was changed.
            // Update the list of member items to be the list of members of the current class.
            EntityItem item = classComboBox.SelectedItem as EntityItem;
            INamedTypeSymbol selectedClass = item.typeCode == SymbolKind.NamedType ? item.Entity as INamedTypeSymbol : null;
            memberItems = new List<EntityItem>();
            if (selectedClass != null)
            {
              
                //IClass compoundClass = selectedClass.GetCompoundClass();
                foreach (var m in selectedClass.GetMembers())
                {
                    if (!m.Locations[0].IsInSource)
                        continue;

                    

                    //    AddMember(selectedClass, m, m.IsConstructor ? TYPE_CONSTRUCTOR : TYPE_METHOD);
                    //}
                    if (m.Kind == SymbolKind.Method)
                    {
                        IMethodSymbol f = m as IMethodSymbol;
                        if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m, m.Kind);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind == SymbolKind.Property)
                    {

                        AddMember(selectedClass, m, m.Kind);
                        allMembers.Add(m);
                    }
                    else
                    if (m.Kind == SymbolKind.Field)
                    {
                        IFieldSymbol f = m as IFieldSymbol;
                        if (f.AssociatedSymbol == null)
                        {
                            AddMember(selectedClass, m, m.Kind);
                            allMembers.Add(m);
                        }
                    }
                    else
                    if (m.Kind == SymbolKind.Event)
                    {
                        AddMember(selectedClass, m, m.Kind);
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

        void AddMember(ISymbol c, ISymbol member, SymbolKind typeCode)
        {
                bool isInSamePart = true;// (member.DeclaringType == selectedClass);
            memberItems.Add(new EntityItem(member, typeCode) { IsInSamePart = isInSamePart });
        }

        void membersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EntityItem item = membersComboBox.SelectedItem as EntityItem;
            if (item != null)
            {
                ISymbol member = item.Entity as ISymbol;
                if (member != null && jumpOnSelectionChange)
                {
                    //AnalyticsMonitorService.TrackFeature(GetType(), "JumpToMember");
                    //JumpTo(item, member.Region);
                    editorWindow.View(member.Locations[0].SourceSpan.Start);
                }
            }
        }

        void JumpTo(EntityItem item, Location region)
        {
            if (region == Location.None)
                return;
            Action<int, int> jumpAction = this.JumpAction;
            if (item.IsInSamePart && jumpAction != null)
            {
                //jumpAction(region.BeginLine, region.BeginColumn);
            }
            else
            {
                //FileService.JumpToFilePosition(item.Entity.CompilationUnit.FileName, region.BeginLine, region.BeginColumn);
            }
        }

        /// <summary>
        /// Action used for jumping to a position inside the current file.
        /// </summary>
        public Action<int, int> JumpAction { get; set; }
    }
}