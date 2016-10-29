using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace WinExplorer.UI
{
    public class ListViewOwnerDraw : Form
    {
        private ListView _listView1 = new ListView();
        private ContextMenu _contextMenu1 = new ContextMenu();

        public ListViewOwnerDraw()
        {
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
            node.Tag = false;
            // Create items and add them to the ListView control.
            ListViewItem listViewItem1 = new ListViewItem(new string[] { "", "this", "this", "-" }, -1);
            Tuple<int, object, object, TreeNode> T = new Tuple<int, object, object, TreeNode>(0, this, this.GetType(), node);
            listViewItem1.Tag = T;

            _listView1.Items.AddRange(new ListViewItem[] { listViewItem1 });

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

            // Initialize the form and add the ListView control to it.
            this.ClientSize = new Size(450, 150);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            this.Text = "ListView OwnerDraw Example";
            this.Controls.Add(_listView1);
            lv = _listView1;
            _listView1.MouseClick += ListView1_MouseDown;
        }

        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            var info = lv.HitTest(e.X, e.Y);
            if (info.Item == null)
                return;
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

                //  this.Invoke(new Action(()=> { LoadFields(row); }));

                LoadFields(row);
            }
        }

        private ListView lv { get; set; }

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _contextMenu1.Dispose();
            }
            base.Dispose(disposing);
        }

        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.Run(new ListViewOwnerDraw());
        //}

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

            int w = resource_vsc.PropertyIcon_24.Width + Resources.Expand.Width + 3;

            ListViewItem v = e.Item;

            int we = Resources.Expand.Width;

            Tuple<int, object, object, TreeNode> T = v.Tag as Tuple<int, object, object, TreeNode>;

            TreeNode node = T.Item4;

            int b = T.Item1;

            Rectangle bb = e.Bounds;
            bb.X += b * w;
            Rectangle cc = e.Bounds;
            cc.X += b * w + we;
            Rectangle bc = e.Bounds;
            bc.X += b * w + w;

            using (StringFormat sf = new StringFormat())
            {
                //// Store the column text alignment, letting it default
                //// to Left if it has not been set to Center or Right.
                //switch (e.Header.TextAlign)
                //{
                //    case HorizontalAlignment.Center:
                //        sf.Alignment = StringAlignment.Center;
                //        flags = TextFormatFlags.HorizontalCenter;
                //        break;
                //    case HorizontalAlignment.Right:
                //        sf.Alignment = StringAlignment.Far;
                //        flags = TextFormatFlags.Right;
                //        break;
                //}

                e.DrawBackground();

                if (e.ColumnIndex == 1)
                {
                    //   e.DrawDefault = false;
                    //    e.DrawBackground();
                    if (node.Text == "c")
                        e.Graphics.DrawImage(Resources.Expand, bb.Location);
                    if (e.SubItem.Text == "Static members")
                        e.Graphics.DrawImage(Resources._class, cc.Location);
                    else if (e.SubItem.Text == "Non-Public members")
                        e.Graphics.DrawImage(Resources._class, cc.Location);
                    else e.Graphics.DrawImage(resource_vsc.PropertyIcon_24, cc.Location/*e.SubItem.Bounds.Location*/);
                    //e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, new SolidBrush(e.SubItem.ForeColor), (e.SubItem.Bounds.Location.X + Resources.Aspx.Width), e.SubItem.Bounds.Location.Y);

                    e.Graphics.DrawString(e.SubItem.Text, _listView1.Font, Brushes.Black, bc/*e.Bounds*/, sf);
                }

                // Draw the text and background for a subitem with a
                // negative value.
                //double subItemValue;
                //if (e.ColumnIndex == 1)
                {
                    // Unless the item is selected, draw the standard
                    // background to make it stand out from the gradient.
                    if ((e.ItemState & ListViewItemStates.Selected) == 0)
                    {
                        //     e.DrawBackground();
                        // Draw the subitem text in red to highlight it.
                        e.Graphics.DrawString(e.SubItem.Text,
                            _listView1.Font, Brushes.Black, bc/*e.Bounds*/, sf);
                    }
                    else
                    {
                        //       e.DrawBackground();
                        // Draw the subitem text in red to highlight it.
                        e.Graphics.DrawString(e.SubItem.Text,
                            _listView1.Font, Brushes.Blue, bc/*e.Bounds*/, sf);
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
                //item.Tag = "tagged";
            }
        }

        // Resets the item tags.
        private void listView1_Invalidated(object sender, InvalidateEventArgs e)
        {
            foreach (ListViewItem item in _listView1.Items)
            {
                if (item == null) return;
                // item.Tag = null;
            }
        }

        // Forces the entire control to repaint if a column width is changed.
        private void listView1_ColumnWidthChanged(object sender,
            ColumnWidthChangedEventArgs e)
        {
            _listView1.Invalidate();
        }

        private object obs { get; set; }

        public void LoadFields(int row)
        {
            lv.BeginUpdate();

            ListViewItem c = lv.Items[row];

            //object b = c.Tag;
            //if (b == null)
            //    b = obs.GetType();

            Tuple<int, object, object, TreeNode> T = (Tuple<int, object, object, TreeNode>)c.Tag;

            TreeNode main = T.Item4;

            if (main.Text == "ce")
            {
                foreach (TreeNode nodes in main.Nodes)
                {
                    TreeNode n = nodes;
                    ListViewItem v = n.Tag as ListViewItem;
                    int i = lv.Items.IndexOf(v);
                    if (i >= 0)
                        lv.Items.RemoveAt(i);
                }

                main.Text = "uc";
                lv.EndUpdate();
                lv.Invalidate();
                return;
            }
            else if (main.Text == "uc")
            {
                int acts = (int)T.Item1;

                foreach (TreeNode nodes in main.Nodes)
                {
                    TreeNode n = nodes;
                    ListViewItem v = n.Tag as ListViewItem;

                    lv.Items.Insert(acts + 1, v);
                    acts = lv.Items.IndexOf(v);
                }

                main.Text = "ce";
                lv.EndUpdate();
                lv.Invalidate();
                return;
            }
            main.Text = "ce";

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
                return;
            }

            main.Tag = true;

            TreeNode node = new TreeNode();

            TreeNode node_vs = new TreeNode();

            main.Nodes.Add(node_vs);
            ListViewItem vs = new ListViewItem();
            vs.Text = "Static members";
            vs.SubItems.Add("Static members");
            vs.SubItems.Add("--");
            Tuple<int, object, object, TreeNode> tc = new Tuple<int, object, object, TreeNode>(T.Item1 + 1, T.Item2, T.Item3, node_vs);
            vs.Tag = tc;
            node_vs.Tag = vs;
            node_vs.Text = "ce";

            TreeNode node_vp = new TreeNode();

            main.Nodes.Add(node_vp);
            ListViewItem vp = new ListViewItem();
            vp.Text = "Non-Public members";
            vp.SubItems.Add("Non-Public members");
            vp.SubItems.Add("---");
            tc = new Tuple<int, object, object, TreeNode>(T.Item1 + 1, T.Item2, T.Item3, node_vp);
            vp.Tag = tc;
            node_vp.Tag = vp;
            node_vp.Text = "ce";
            ArrayList S = new ArrayList();

            S.Add(vs);

            ArrayList P = new ArrayList();

            P.Add(vp);

            FieldInfo[] f = ((Type)b).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

            int act = row;

            foreach (FieldInfo s in f)
            {
                node = new TreeNode();
                node.Text = "f";

                ListViewItem v = new ListViewItem();
                object dd = null;
                try
                {
                    dd = s.GetValue(T.Item2 as object);
                }
                catch (Exception e) { };

                node.Text = "f";
                if (s.FieldType.IsValueType == true)
                    node.Text = "";
                if (s.FieldType.IsPrimitive == true)
                    node.Text = "";
                if (s.FieldType.IsGenericType == true)
                    node.Text = "";
                v.SubItems.Add(s.Name);
                if (dd != null)
                    v.SubItems.Add(dd.ToString());
                else v.SubItems.Add("");
                v.SubItems.Add(s.FieldType.ToString());
                Tuple<int, object, object, TreeNode> t = new Tuple<int, object, object, TreeNode>(T.Item1 + 1, dd, s, node);
                v.Tag = t;
                node.Tag = v;
                if (s.IsStatic == true)
                {
                    node_vs.Nodes.Add(node);

                    //main.Nodes.Add(node);
                    Tuple<int, object, object, TreeNode> tt = new Tuple<int, object, object, TreeNode>(T.Item1 + 2, dd, s, node);
                    v.Tag = tt;

                    S.Add(v);
                }
                else if (s.IsPrivate == true)
                {
                    node_vp.Nodes.Add(node);
                    Tuple<int, object, object, TreeNode> tt = new Tuple<int, object, object, TreeNode>(T.Item1 + 2, dd, s, node);
                    v.Tag = tt;

                    P.Add(v);
                }
                else
                {
                    main.Nodes.Add(node);
                    lv.Items.Insert(act + 1, v);
                    act = lv.Items.IndexOf(v);
                }
            }
            PropertyInfo[] p = ((Type)b).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (PropertyInfo s in p)
            {
                node = new TreeNode();

                ListViewItem v = new ListViewItem();
                object dd = null;

                try
                {
                    dd = s.GetValue(T.Item2 as object);
                }
                catch (Exception e) { };

                node.Text = "f";

                //if (s.PropertyType.IsValueType == false)
                //    node.Text = "c";
                //if (s.PropertyType.IsClass == true)
                //    node.Text = "c";

                v.SubItems.Add(s.Name);
                if (dd != null)
                    v.SubItems.Add(dd.ToString());
                else v.SubItems.Add("");
                v.SubItems.Add(s.PropertyType.ToString());
                Tuple<int, object, object, TreeNode> t = new Tuple<int, object, object, TreeNode>(T.Item1 + 1, dd, s, node);
                v.Tag = t;
                node.Tag = v;
                lv.Items.Insert(act + 1, v);
                act = lv.Items.IndexOf(v);
                main.Nodes.Add(node);
            }

            //lv.Items.Insert(row + 1, vs);

            foreach (ListViewItem v in S)
            {
                lv.Items.Insert(act + 1, v);
                act = lv.Items.IndexOf(v);
            }

            //lv.Items.Insert(row + 1, vp);

            foreach (ListViewItem v in P)
            {
                lv.Items.Insert(act + 1, v);
                act = lv.Items.IndexOf(v);
            }

            lv.Items[row].Selected = true;

            lv.Items[row].Focused = true;

            lv.EndUpdate();

            lv.Invalidate();

            lv.Refresh();
        }
    }
}