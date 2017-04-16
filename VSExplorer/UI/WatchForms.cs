using GACProject;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using VSParsers;
using WinExplorer.Services;

namespace WinExplorer.UI
{
    public class ListViewOwnerDraw : Form
    {
        private ListView _listView1 = new ListView();
        private ContextMenu _contextMenu1 = new ContextMenu();

        private ServiceBitmaps service { get; set; }

        int editor = 0;

        private TextBox editbox = new TextBox();

        public TreeNode MainNode = new TreeNode();

        public void AddNewNode(string name = "")
        {
            TreeNode nodes = new TreeNode();
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "editor", "editor", "editor", "-" }, -1);
            Tuple<int, object, object, TreeNode, Image> T = new Tuple<int, object, object, TreeNode, Image>(0, editor, editor.GetType(), nodes, Resources.Property_16x);
            listViewItem1.Tag = T;
            G.Add(listViewItem1);
            GV.Add(listViewItem1);
            _listView1.VirtualListSize = GV.Count;
            _listView1.Refresh();
            nodes.Tag = listViewItem1;
            MainNode.Nodes.Add(nodes);
        }
        public void ChangeNode(ListViewItem v, string name)
        {

            Type type = GACForm.FindTypeFromAssemblies("." + name);

            if (type == null)
                type = CSParsers.GetTypeForName(name);

            Tuple<int, object, object, TreeNode, Image> TT = v.Tag as Tuple<int, object, object, TreeNode, Image>;

            if (type != null)
            {
                Tuple<int, object, object, TreeNode, Image> T = new Tuple<int, object, object, TreeNode, Image>(TT.Item1, type, type, TT.Item4, Resources.Class_yellow_16x);
                T.Item4.Nodes.Add(new TreeNode("placeholder"));
                T.Item4.Tag = v;
                T.Item4.Text = "f";
                v.Tag = T;

                v.SubItems[3].Text = type.FullName;

            }
            else
            {
                Tuple<int, object, object, TreeNode, Image> T = new Tuple<int, object, object, TreeNode, Image>(TT.Item1, TT.Item2, TT.Item3, TT.Item4, Resources.Errors);
                T.Item4.Nodes.Add(new TreeNode("placeholder"));
                T.Item4.Tag = v;
                T.Item4.Text = "f";
                v.Tag = T;
            }
 
        }
        public ListViewOwnerDraw()
        {

            service = new ServiceBitmaps();

            // Initialize the ListView control.
            _listView1.BackColor = Color.White;
            _listView1.ForeColor = Color.Black;
            _listView1.Dock = DockStyle.Fill;
            _listView1.View = View.Details;
            _listView1.FullRowSelect = true;

            // Add columns to the ListView control.
            _listView1.Columns.Add("", 15, HorizontalAlignment.Center);
            _listView1.Columns.Add("Name", 400, HorizontalAlignment.Center);
            _listView1.Columns.Add("Value", 100, HorizontalAlignment.Center);
            _listView1.Columns.Add("Type", 100, HorizontalAlignment.Center);

            TreeNode node = new TreeNode();
            node.Nodes.Add(new TreeNode("placeholder"));
            node.Tag = false;
            // Create items and add them to the ListView control.
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "", "this", "this", "-" }, -1);
            Tuple<int, object, object, TreeNode, Image> T = new Tuple<int, object, object, TreeNode, Image>(0, this, this.GetType(), node, Resources.Property_16x);
            listViewItem1.Tag = T;
            node.Tag = listViewItem1;

            G.Add(listViewItem1);
            GV.Add(listViewItem1);
            MainNode.Nodes.Add(node);

            TreeNode nodes = new TreeNode();
            nodes.Tag = false;
            listViewItem1 = new ListViewItem(new string[] { "editor", "editor", "editor", "-" }, -1);
            T = new Tuple<int, object, object, TreeNode, Image>(0, editor, editor.GetType(), nodes, Resources.Property_16x);
            listViewItem1.Tag = T;
            G.Add(listViewItem1);
            GV.Add(listViewItem1);
            nodes.Tag = listViewItem1;
            MainNode.Nodes.Add(nodes);

            _listView1.GridLines = true;

            // Initialize the shortcut menu and
            // assign it to the ListView control.
            _contextMenu1.MenuItems.Add("List",
                new EventHandler(menuItemList_Click));
            _contextMenu1.MenuItems.Add("Details",
                new EventHandler(menuItemDetails_Click));
            _listView1.ContextMenu = _contextMenu1;

            // Configure the ListView control for owner-draw and add
            // handlers for the owner-draw events.
            _listView1.OwnerDraw = true;
            _listView1.DrawItem += new
                DrawListViewItemEventHandler(listView1_DrawItem);
            _listView1.DrawSubItem += new
                DrawListViewSubItemEventHandler(listView1_DrawSubItem);
            _listView1.DrawColumnHeader += new
                DrawListViewColumnHeaderEventHandler(listView1_DrawColumnHeader);

            // Add a handler for the MouseUp event so an item can be
            // selected by clicking anywhere along its width.
            _listView1.MouseUp += new MouseEventHandler(listView1_MouseUp);

            // Add handlers for various events to compensate for an
            // extra DrawItem event that occurs the first time the mouse
            // moves over each row.
            _listView1.MouseMove += new MouseEventHandler(listView1_MouseMove);
            _listView1.ColumnWidthChanged += new ColumnWidthChangedEventHandler(listView1_ColumnWidthChanged);
            _listView1.Invalidated += new InvalidateEventHandler(listView1_Invalidated);

           // _listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSizeColumnContent);
            _listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // Initialize the form and add the ListView control to it.
            this.ClientSize = new Size(450, 150);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            this.Text = "ListView OwnerDraw Example";
            this.Controls.Add(_listView1);
            lv = _listView1;
            _listView1.MouseClick += ListView1_MouseDown;
            this.Resize += ListViewOwnerDraw_ResizeEnd;
            DoubleBuffer(_listView1);
            lv.VirtualListSize = 2;
            lv.VirtualMode = true;
            lv.RetrieveVirtualItem += Lv_RetrieveVirtualItem;
            editbox.Parent = _listView1;
            editbox.Hide();
            editbox.LostFocus += new EventHandler(editbox_LostFocus);
            editbox.KeyPress += Editbox_KeyPress;
            _listView1.MouseDoubleClick += new MouseEventHandler(listView_MouseDoubleClick);
            _listView1.KeyDown += _listView1_KeyPress;
            

        }

        private void _listView1_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                //hitinfo = _listView1.HitTest(e.X, e.Y);
                if (hitinfo.Item == null)
                    return;
                ListViewItem v = hitinfo.Item;

                foreach(TreeNode node in MainNode.Nodes)
                {
                    ListViewItem b = node.Tag as ListViewItem;

                    if (b == null)
                        continue;

                    if (b.Equals(v))
                    {
                        RemoveMainNode(node);
                        MainNode.Nodes.Remove(node);
                        _listView1.Invalidate();
                        _listView1.Refresh();
                        return;
                    }
                }

            }
                
        }

        private void Editbox_KeyPress(object sender, KeyPressEventArgs e)  
        {
            if (e.KeyChar == (Char)Keys.Enter)
                editbox_LostFocus(null, null);
        }

        private ListViewHitTestInfo hitinfo;

        private ListViewItem edited;
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            hitinfo = _listView1.HitTest(e.X, e.Y);
            if (hitinfo.Item == null)
                return;
            var row = hitinfo.Item.Index;
            var col = hitinfo.Item.SubItems.IndexOf(hitinfo.SubItem);
            if (row < 0)
                return;
            if (col < 0)
                return;
            if (col > 1)
                return;
            if (row != GV.Count - 1)
                return;
            edited = GV[row] as ListViewItem;
            editbox.Bounds = hitinfo.SubItem.Bounds;
            editbox.Text = hitinfo.SubItem.Text;
            editbox.Focus();
            editbox.Show();
        }
        void editbox_LostFocus(object sender, EventArgs e)
        {
            if (sender == null)
            {
                hitinfo.SubItem.Text = editbox.Text;

                ChangeNode(edited, editbox.Text);
                editbox.Hide();
                AddNewNode();
                _listView1.Refresh();
            }
        }
        private void Lv_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= GV.Count)
                e.Item = GV[GV.Count - 1] as ListViewItem;
            else
            e.Item = GV[e.ItemIndex] as ListViewItem;
        }

        public static void DoubleBuffer(Control control)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession) return;
            System.Reflection.PropertyInfo dbProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            dbProp.SetValue(control, true, null);
        }
        private void ListViewOwnerDraw_ResizeEnd(object sender, EventArgs e)
        {
            ResizeColumnHeaders();
        }
        private void ResizeColumnHeaders()
        {
            int c = this.Width / (_listView1.Columns.Count - 1);
            for (int i = 1; i < this._listView1.Columns.Count; i++) this._listView1.Columns[i].Width = c;
           // this._listView1.Columns[this._listView1.Columns.Count - 1].Width = -2;
        }

        delegate void threadWorker(int row);

        public void workerFunction(int row)
        {
            this.BeginInvoke(new Action(() => { LoadFields(row); lv.Focus(); }));
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            var info = lv.HitTest(e.X, e.Y);
            if (info.Item == null)
                return;
            hitinfo = info;
            var row = info.Item.Index;
            var col = info.Item.SubItems.IndexOf(info.SubItem);
            if (row < 0)
                return;
            if (col < 0)
                return;
            var value = info.Item.SubItems[col].Text;
            //MessageBox.Show(string.Format("R{0}:C{1} val '{2}'", row, col, value));
            if (col == 1)
            {
                obs = this.GetType();

                //this.BeginInvoke(new Action(()=> { LoadFields(row); lv.Focus(); }));

                //Task taskA = Task.Run(() => { this.BeginInvoke(new Action(() => { LoadFields(row); lv.Focus(); })); });

                ThreadWorkers dw = new ThreadWorkers();
                dw.d = this;
                dw.row = row;
                dw.StartWork();

                // LoadFields(row);

                //lv.Focus();

                //threadWorker w = workerFunction;
                //w.BeginInvoke(row, null, null);
            }
        }

        public ListView lv { get; set; }

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contextMenu1.Dispose();
            }
            base.Dispose(disposing);
        }

    
        // Sets the ListView control to the List view.
        private void menuItemList_Click(object sender, EventArgs e)
        {
            _listView1.View = View.List;
            _listView1.Invalidate();
        }

        // Sets the ListView control to the Details view.
        private void menuItemDetails_Click(object sender, EventArgs e)
        {
            _listView1.View = View.Details;

            // Reset the tag on each item to re-enable the workaround in
            // the MouseMove event handler.
            foreach (ListViewItem item in _listView1.Items)
            {
                // item.Tag = null;
            }
        }

        // Selects and focuses an item when it is clicked anywhere along
        // its width. The click must normally be on the parent item text.
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewItem clickedItem = _listView1.GetItemAt(5, e.Y);
            if (clickedItem != null)
            {
                clickedItem.Selected = true;
                clickedItem.Focused = true;
            }
        }

        // Draws the backgrounds for entire ListView items.
        private void listView1_DrawItem(object sender,
            DrawListViewItemEventArgs e)
        {
            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Draw the background and focus rectangle for a selected item.
                e.Graphics.FillRectangle(Brushes.Blue, e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                // Draw the background for an unselected item.
                using (LinearGradientBrush brush =
                    new LinearGradientBrush(e.Bounds, Color.White,
                    Color.White, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }

            // Draw the item text for views other than the Details view.
            if (_listView1.View != View.Details)
            {
                e.DrawText();
            }
        }

        // Draws subitem text and applies content-based formatting.
        private void listView1_DrawSubItem(object sender,
            DrawListViewSubItemEventArgs e)
        {
            //TextFormatFlags flags = TextFormatFlags.Left;

            int w = resource_vsc.PropertyIcon_24.Width + Resources.Expand.Width + 3 - 5 ;

            ListViewItem v = e.Item;

            int we = Resources.Expand.Width;

            Tuple<int, object, object, TreeNode, Image> T = v.Tag as Tuple<int, object, object, TreeNode, Image>;

            TreeNode node = T.Item4;

            int b = T.Item1;

            w -= 13;

            Rectangle bb = e.Bounds;
            bb.X += (b * w);
            Rectangle cc = e.Bounds;
            cc.X += (b * w + we);
            Rectangle bc = e.Bounds;
            bc.X += (b * w + we + 16);

            using (StringFormat sf = new StringFormat())
            {
      

                e.DrawBackground();

                if (e.ColumnIndex == 1)
                {
                    if ((e.ItemState & ListViewItemStates.Selected) != 0)
                        e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);

                    if (node.Nodes.Count > 0)
                    {
                        if (node.Text == "ce")
                        {
                          
                            VisualStyleRenderer treeOpen = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Opened);
                            treeOpen.DrawBackground(e.Graphics, new Rectangle(bb.X, bb.Y, 16, 16));
                            

                        }
                        else
                        {
                            VisualStyleRenderer treeClose = new VisualStyleRenderer(VisualStyleElement.TreeView.Glyph.Closed);
                            treeClose.DrawBackground(e.Graphics, new Rectangle(bb.X, bb.Y, 16, 16));
                            


                        }

                    }

                    

                    //   e.DrawDefault = false;
                    //    e.DrawBackground();
                    if (node.Text == "c")
                        e.Graphics.DrawImage(Resources.Expand, bb.Location);

                    Image image = T.Item5 as Image;

                    if (image == null)
                        image = Resources.PropertySealed_16x;

                    e.Graphics.DrawImage(image, cc.Location);

                    //if (e.SubItem.Text == "Static members")
                    //    e.Graphics.DrawImage(Resources._class, cc.Location);
                    //else if (e.SubItem.Text == "Non-Public members")
                    //    e.Graphics.DrawImage(Resources._class, cc.Location);
                    //else if(node.Text == "f")
                    //    e.Graphics.DrawImage(Resources.Field_blue_16x, cc.Location/*e.SubItem.Bounds.Location*/);

                    //else e.Graphics.DrawImage(resource_vsc.PropertyIcon_24, cc.Location/*e.SubItem.Bounds.Location*/);




                    //e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + Resources.Aspx.Width), e.SubItem.Bounds.Location.Y);



                    e.Graphics.DrawString(e.SubItem.Text, _listView1.Font, Brushes.Black, bc/*e.Bounds*/, sf);
                }

                // Draw the text and background for a subitem with a
                // negative value.
                //double subItemValue;
                if (e.ColumnIndex > 1)
                {
                    if ((e.ItemState & ListViewItemStates.Selected) != 0)
                        e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);

                    // Unless the item is selected, draw the standard
                    // background to make it stand out from the gradient.
                    if ((e.ItemState & ListViewItemStates.Selected) == 0)
                    {
                             
                        // Draw the subitem text in red to highlight it.
                        e.Graphics.DrawString(e.SubItem.Text,
                            _listView1.Font, Brushes.Black, e.Bounds, sf);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
                        e.Graphics.DrawString(e.SubItem.Text,
                            _listView1.Font, Brushes.White, e.Bounds, sf);
                    }

                    //   return;
                }

                // Draw normal text for a subitem with a nonnegative
                // or nonnumerical value.
                //e.DrawText(flags);
            }
        }

        // Draws column headers.
        private void listView1_DrawColumnHeader(object sender,
            DrawListViewColumnHeaderEventArgs e)
        {
            using (StringFormat sf = new StringFormat())
            {
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;

                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                // Draw the standard header background.
                e.DrawBackground();

                // Draw the header text.
                using (Font headerFont =
                            new Font("Helvetica", 10, FontStyle.Regular))
                {
                    e.Graphics.DrawString(e.Header.Text, headerFont,
                        Brushes.Black, e.Bounds, sf);
                }
            }
            return;
        }

        // Forces each row to repaint itself the first time the mouse moves over
        // it, compensating for an extra DrawItem event sent by the wrapped
        // Win32 control. This issue occurs each time the ListView is invalidated.
        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = _listView1.GetItemAt(e.X, e.Y);
            if (item != null)
            {
                _listView1.Invalidate(item.SubItems[1].Bounds);
                
            }
        }

        // Resets the item tags.
        private void listView1_Invalidated(object sender, InvalidateEventArgs e)
        {
            //foreach (ListViewItem item in _listView1.Items)
            {
               // if (item == null) return;
                // item.Tag = null;
            }
        }

        // Forces the entire control to repaint if a column width is changed.
        private void listView1_ColumnWidthChanged(object sender,
            ColumnWidthChangedEventArgs e)
        {
            _listView1.Invalidate();
        }


        void RemoveMainNode(TreeNode node)
        {
            RemoveNodes(node, true);
            _listView1.VirtualListSize = GV.Count;
        }

        private object obs { get; set; }


        public void RemoveNodes(TreeNode main, bool hide = false)
        {

            foreach (TreeNode nodes in main.Nodes)
            {
                TreeNode n = nodes;
                ListViewItem v = n.Tag as ListViewItem;

                G.Remove(v);
                GV.Remove(v);

                RemoveNodes(n, true);
            }

            if (hide == true)
            {

                ListViewItem v = main.Tag as ListViewItem;
                G.Remove(v);
                GV.Remove(v);
            }

        }

        public void HideNodes(TreeNode main, bool hide = false)
        {

            foreach (TreeNode nodes in main.Nodes)
            {
                TreeNode n = nodes;
                ListViewItem v = n.Tag as ListViewItem;


                GV.Remove(v);  

                HideNodes(n, true);
            }

            if(hide == true)
            {

                ListViewItem v = main.Tag as ListViewItem;
                GV.Remove(v);
            }

        }
        public void ExpandNodes(TreeNode main, ref int act, bool expand = true)
        {

            if (main.IsExpanded == true)
                expand = true;

            foreach (TreeNode nodes in main.Nodes)
            {

                ListViewItem v = nodes.Tag as ListViewItem;

                if (nodes.IsExpanded || expand == true)
                {

                    
                    if (v == null)
                        continue;
                    //lv.Items.Insert(act + 1, v);
                    //act = lv.Items.IndexOf(v);

                    GV.Insert(act + 1, v);
                    act = GV.IndexOf(v);
                    
                    if(nodes.IsExpanded)
                    ExpandNodes(nodes, ref act, false);

                    //nodes.Expand();
                }
                
            }

           

        }

        public Type GetTypeFor(object obs)
        {
            Type T = obs.GetType();
            if (T == typeof(Type) || T == Type.GetType("System.Reflection.RuntimePropertyInfo") || T == Type.GetType("System.RuntimeType") || T == Type.GetType("System.Reflection.PropertyInfo"))
                return obs as Type;
            else return obs.GetType();
        }

        ArrayList G = new ArrayList();

        ArrayList GV = new ArrayList();

        public void LoadFields(int row)
        {
            lv.BeginUpdate();

            //ListViewItem c = lv.Items[row];

            ListViewItem c = GV[row] as ListViewItem;

            //object b = c.Tag;
            //if (b == null)
            //    b = obs.GetType();

            Tuple<int, object, object, TreeNode, Image> T = (Tuple<int, object, object, TreeNode, Image>)c.Tag;

            TreeNode main = T.Item4;

            int w = T.Item1;

            TreeNode ns = null;

            foreach(TreeNode ng in main.Nodes)
            {

                if (ng.Text == "placeholder")
                {
                    ns = ng;
                    break;
                }

            }

            if (ns != null)
                main.Nodes.Remove(ns);
            

            if (main.Text == "ce")
            {

                HideNodes(main);

                //foreach (TreeNode nodes in main.Nodes)
                //{
                //    TreeNode n = nodes;
                //    ListViewItem v = n.Tag as ListViewItem;
                //    //int i = lv.Items.IndexOf(v);
                //    //if (i >= 0)
                //    //    lv.Items.RemoveAt(i);
                //    lv.Items.Remove(v);
                //}

                main.Text = "uc";

                main.Collapse();

                lv.VirtualListSize = GV.Count;

                lv.EndUpdate();
                lv.Invalidate();
                lv.Refresh();
                return;
            }
            else if (main.Text == "uc")
            {
                ListViewItem bb = c;// main.Tag as ListViewItem;

                int acts = GV.IndexOf(bb);// (int)lv.Items.IndexOf(bb);

                Tuple<int, object, object, TreeNode, Image> tc = bb.Tag as Tuple<int, object, object, TreeNode, Image>;

                if (tc != null)
                {

                    TreeNode ng = tc.Item4 as TreeNode;

                    //foreach (TreeNode nodes in ng.Nodes)
                    //{
                    //    TreeNode n = nodes;
                    //    ListViewItem v = n.Tag as ListViewItem;
                    //    if (v == null)
                    //        continue;
                    //    lv.Items.Insert(acts + 1, v);
                    //    acts = lv.Items.IndexOf(v);
                    //    //n.Text = "ce";
                    //}

                    ExpandNodes(main, ref acts);

                    //ng.Expand();

                }
                //ng.Text = "ce";
                main.Text = "ce";
                main.Expand();
                lv.VirtualListSize = GV.Count;
                lv.EndUpdate();
                lv.Invalidate();
                lv.Refresh();
                return;
            }
           // else
           // main.Text = "ce";

            Type b = T.Item3 as Type;

            string g = T.Item3.GetType().FullName;

            //string gg = b.FullName;

            if ((Type)T.Item3.GetType() == typeof(FieldInfo))
            {
                FieldInfo d = T.Item3 as FieldInfo;
                b = d.FieldType;
            }
            else if ((Type)T.Item3.GetType() == typeof(PropertyInfo))
            {
                PropertyInfo d = T.Item3 as PropertyInfo;
                b = d.PropertyType;
            }
            else if ((Type)T.Item3.GetType() == Type.GetType("System.Reflection.RuntimeFieldInfo"))
            {
                FieldInfo d = T.Item3 as FieldInfo;
                b = d.FieldType;
            }
            else if ((Type)T.Item3.GetType() == Type.GetType("System.Reflection.RtFieldInfo"))
            {
                FieldInfo d = T.Item3 as FieldInfo;
                b = d.FieldType;
            }
            else if ((Type)T.Item3.GetType() == Type.GetType("System.Reflection.RuntimePropertyInfo"))
            {
                PropertyInfo d = T.Item3 as PropertyInfo;
                b = d.PropertyType;
            }

            if (b == null)
            {
                lv.EndUpdate();
                lv.Refresh();
                return;
            }

            //main.Tag = true;

            TreeNode node = new TreeNode();

            bool r = false;

            Type baseType = null;

            if (T.Item2 != null)
            {
                
                {
                    if (GetTypeFor(T.Item2).IsPrimitive == true /*|| T.Item2.GetType().IsValueType == true*/ /*||  T.Item2.GetType().IsPointer == true*/)
                        r = true;
                }

                baseType = GetTypeFor(T.Item2).BaseType;

            }

            TreeNode node_vs = new TreeNode();
            TreeNode node_vp = new TreeNode();
            TreeNode node_bs = new TreeNode();

            ListViewItem vp = new ListViewItem();
            ListViewItem vs = new ListViewItem();
            ListViewItem bs = new ListViewItem();

            ArrayList S = new ArrayList();
            ArrayList P = new ArrayList();
            ArrayList B = new ArrayList();

            int act = G.IndexOf(c);

            int sa = row;


            if (r == false)
            {
                main.Nodes.Add(node_vs);
                
                vs.Text = "Static members";
                vs.SubItems.Add("Static members" + " for " + c.SubItems[1].Text);
                vs.SubItems.Add(" ");
                vs.SubItems.Add(" ");
                Tuple<int, object, object, TreeNode, Image> tc = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, T.Item2, T.Item3, node_vs, Resources.Class_yellow_16x);
                vs.Tag = tc;
                node_vs.Tag = vs;
                node_vs.Text = "uc";
                //node_vs.ImageKey = "Class_yellow_256x";

                main.Nodes.Add(node_vp);
                
                vp.Text = "Non-Public members";
                vp.SubItems.Add("Non-Public members");
                vp.SubItems.Add(" ");
                vp.SubItems.Add(" ");
                tc = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, T.Item2, T.Item3, node_vp, Resources.Class_yellow_16x);
                vp.Tag = tc;
                node_vp.Tag = vp;
                node_vp.Text = "uc";
             

                if(baseType != null)
                {

                    main.Nodes.Add(node_bs);

                    bs.Text = "Base";
                    bs.SubItems.Add("Base");
                    bs.SubItems.Add(" ");
                    bs.SubItems.Add(baseType.FullName);
                    tc = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, baseType, T.Item3, node_bs, Resources.Class_yellow_16x);
                    bs.Tag = tc;
                    node_bs.Tag = bs;
                    node_bs.Text = "nc";
                    node_bs.Nodes.Add(new TreeNode("placeholder"));


                }


                int cc = G.IndexOf(c);
                G.Insert(cc + 1, vs);
                cc = G.IndexOf(vs);
                

                //lv.Items.Insert(cc + 1, vp);
                G.Insert(cc + 1, vp);
                act = cc + 1;

                if (baseType != null)
                {
                    G.Insert(cc + 2, bs);
                    act = cc + 2;
                }

                cc = GV.IndexOf(c);
                GV.Insert(cc + 1, vs);
                cc = GV.IndexOf(vs);
                GV.Insert(cc + 1, vp);
                sa = cc + 1;

                if(baseType != null)
                {
                    GV.Insert(cc + 2, bs);
                    sa = cc + 2;
                }


            }
                   
            //FieldInfo[] f = ((Type)b).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            

            if (T.Item2 != null)
                
            {

                r = false;
                if(T.Item2.GetType().IsPrimitive == true/* || T.Item2.GetType().IsValueType == true*/)
                    r = true;

                if (r == false)
                {

                    FieldInfo[] f = ((Type)GetTypeFor(T.Item2)).GetFields(BindingFlags.DeclaredOnly | (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));


                    foreach (FieldInfo s in f)
                    {
                        if (s.Name == main.Text)
                            continue;
                        node = new TreeNode();
                        node.Text = "f";
                        HasItems(s.FieldType, node);

                        ListViewItem v = new ListViewItem();
                        object dd = null;
                        try
                        {
                            dd = s.GetValue(T.Item2 as object);
                        }
                        catch (Exception e) { };
                        if (dd == null)
                            dd = s.FieldType;
                        node.Text = "f";
                        //if (s.FieldType.IsValueType == true)
                        //    node.Text = "";
                        //if (s.FieldType.IsPrimitive == true)
                        //    node.Text = "";
                        //if (s.FieldType.IsGenericType == true)
                        //    node.Text = "";
                        v.SubItems.Add(s.Name);
                        if (dd != null)
                            v.SubItems.Add(dd.ToString());
                        else v.SubItems.Add("");
                        v.SubItems.Add(s.FieldType.ToString());
                        Tuple<int, object, object, TreeNode, Image> t = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, dd, s, node, Resources.Field_blue_16x);
                        v.Tag = t;
                        node.Tag = v;
                       // node.ImageKey = service.GetKeyForField(s);
                        if (s.IsStatic == true)
                        {
                            node_vs.Nodes.Add(node);

                            
                            Tuple<int, object, object, TreeNode, Image> tt = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 2, dd, s, node, service.GetImage("s_Field_blue_16x"));
                            v.Tag = tt;

                            S.Add(v);
                           // node.Text = "ce";
                        }
                        else if (s.IsPrivate == true)
                        {
                            node_vp.Nodes.Add(node);
                            Tuple<int, object, object, TreeNode, Image> tt = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 2, dd, s, node, Resources.FieldPrivate_16x);
                            v.Tag = tt;
                            //node.Text = "ce";
                             P.Add(v);
                        }
                        else if (s.IsPublic == false)
                        {
                            node_vp.Nodes.Add(node);
                            Tuple<int, object, object, TreeNode, Image> tt = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 2, dd, s, node, Resources.FieldProtect_16x);
                            v.Tag = tt;
                            //node.Text = "ce";
                            P.Add(v);
                        }
                        else
                        {
                            main.Nodes.Add(node);
                //            lv.Items.Insert(act + 1, v);
                //            act = lv.Items.IndexOf(v);
                            G.Insert(act + 1, v);
                            act = G.IndexOf(v);
                            GV.Insert(sa + 1, v);
                            sa = GV.IndexOf(v);
                        }
                    }
                }
            }
            //PropertyInfo[] p = ((Type)b).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            

            if (T.Item2 != null)
            {

                r = false;
                if (T.Item2.GetType().IsPrimitive == true/* || T.Item2.GetType().IsValueType == true*/)
                    r = true;

                if (r == false)
                {

                    PropertyInfo[] p = ((Type)GetTypeFor(T.Item2)).GetProperties(BindingFlags.DeclaredOnly | (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static));

                    foreach (PropertyInfo s in p)
                    {
                        if (s.Name == main.Text)
                            continue;

                        node = new TreeNode();

                        HasItems(s.PropertyType, node);

                        ListViewItem v = new ListViewItem();
                        object dd = null;

                        try
                        {
                            dd = s.GetValue(T.Item2 as object);
                        }
                        catch (Exception e)
                        {
                           // Console.WriteLine(e.Message);
                        };
                        if (dd == null)
                            dd = s.PropertyType;
                        node.Text = "p";//f

                        //if (s.PropertyType.IsValueType == false)
                        //    node.Text = "c";
                        //if (s.PropertyType.IsClass == true)
                        //    node.Text = "c";

                        v.SubItems.Add(s.Name);
                        if (dd != null)
                            v.SubItems.Add(dd.ToString());
                        else v.SubItems.Add("");
                        v.SubItems.Add(s.PropertyType.ToString());
                        Tuple<int, object, object, TreeNode, Image> t = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, dd, s, node, Resources.Property_16x);
                        v.Tag = t;
                        node.Tag = v;
                        //node.ImageKey = service.GetKeyForProperty(s);
                        //lv.Items.Insert(act + 1, v);
                        //act = lv.Items.IndexOf(v);
                        //main.Nodes.Add(node);
                        MethodInfo mf = s.GetGetMethod();

                        if (mf == null)
                            mf = s.GetSetMethod();
                        
                        if(s.GetGetMethod(true) != null || s.GetSetMethod(true) != null)
                        {
                            t = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, dd, s, node, Resources.PropertyPrivate_16x);
                            v.Tag = t;
                            
                            if(mf != null && mf.IsPrivate == false)
                            {
                                t = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 1, dd, s, node, Resources.PropertyProtect_16x);
                                v.Tag = t;
                                
                            }
                        }
                        if (mf != null && mf.IsStatic)
                        {

                            //node.ImageKey = service.GetKeyForProperty(s, "static");

                            node_vs.Nodes.Add(node);


                            Tuple<int, object, object, TreeNode, Image> tt = new Tuple<int, object, object, TreeNode, Image>(T.Item1 + 2, dd, s, node, service.GetImage("s_Property_16x"));
                            v.Tag = tt;

                            S.Add(v);
                            // node.Text = "ce";
                        }
                        else
                        {
                            main.Nodes.Add(node);
                  //          lv.Items.Insert(act + 1, v);
                  //          act = lv.Items.IndexOf(v);
                            G.Insert(act + 1, v);
                            act = G.IndexOf(v);
                            GV.Insert(sa + 1, v);
                            sa = GV.IndexOf(v);
                        }
                    }
                }
            }

            main.Text = "ce";

            //lv.Items.Insert(row + 1, vs);


            if (S.Count/*node_vs.Nodes.Count*/ <= 0)
            {
                main.Nodes.Remove(node_vs);
                //lv.Items.Remove(vs);
                G.Remove(vs);
                GV.Remove(vs);
            }
            else
            {
                act = lv.Items.IndexOf(vs);
                foreach (ListViewItem v in S)
                {
                    //lv.Items.Insert(act + 1, v);
                    act = lv.Items.IndexOf(v);
                }
            }

            //lv.Items.Insert(row + 1, vp);

            if (P.Count /*node_vp.Nodes.Count*/ <= 0)
            {
                main.Nodes.Remove(node_vp);
                //lv.Items.Remove(vp);
                G.Remove(vp);
                GV.Remove(vp);
            }
            else
            {

                act = lv.Items.IndexOf(vp);

                foreach (ListViewItem v in P)
                {
                    //lv.Items.Insert(act + 1, v);
                    act = lv.Items.IndexOf(v);
                }
            }

            main.Expand();

            lv.VirtualListSize = GV.Count;

            lv.Items[row].Selected = true;

            lv.Items[row].Focused = true;

            lv.EndUpdate();

            lv.Invalidate();

            lv.Refresh();
        }
        public bool HasItems(Type T, TreeNode node)
        {

            Type b = T;
       
          
            ArrayList P = new ArrayList();

            

            FieldInfo[] f = ((Type)b).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

            if(f.Length >= 1)
            {

                

                node.Nodes.Add(new TreeNode("placeholder"));

                return true;
            }

            
      
            PropertyInfo[] p = ((Type)b).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

            if(p.Length >= 1)
            {
                node.Nodes.Add(new TreeNode("placeholder"));

                return true;
            }

            return false;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ListViewOwnerDraw
            // 
            this.ClientSize = new System.Drawing.Size(686, 456);
            this.Name = "ListViewOwnerDraw";
            this.ResumeLayout(false);

        }
    }
    class ThreadWorkers
    {
        public event EventHandler<EventArgs> Done = (s, e) => { };

        public ListViewOwnerDraw d { get; set; }

        public int row = 0;

        public void StartWork()
        {
            var thread = new Thread(Work);
            thread.IsBackground = true;
            thread.Start();
        }

        // Seam for extension and testability
        public Thread CreateThread()
        {
            return new Thread(Work) { Name = "Worker Thread" };
        }

        private void Work()
        {
            d.Invoke(new Action(() => { d.LoadFields(row); d.lv.Focus(); }));
        }
    }
}