using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WinExplorer
{
    public partial class CustomizeForm : Form
    {

       
        ToolCommands tc { get; set; }

        CheckedListBox chTB { get; set; }
        public CustomizeForm()
        {
            InitializeComponent();

            tc = new ToolCommands();

            chTB = checkedListBox1;

            LoadToolbars();

            comboBox2.Items.Add("Menubar");
            comboBox2.SelectedIndex = 0;

            LoadToolboxList();

            comboBox4.Items.Add("Tfs");
            comboBox4.SelectedIndex = 0;


            radioButton1.CheckedChanged += RadioButton1_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton2_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton3_CheckedChanged;


            panel1.Resize += Panel1_Resize;

            //this.PreviewKeyDown += CustomizeForm_PreviewKeyDown;

            //this.KeyPreview = true;

            

            modContext = contextMenuStrip1;

            textOnlyinMenusToolStripMenuItem.Enabled = false;

            modButton = button11;



            modButton.Click += ModButton_Click;

            modContext.Closing += ModContext_Closing;

            cs = new ToolStripCombos();
            cs.Text = "Name:";
            cs.drawarrow = false;
            cs.font = new Font("Times New Roman", 7); ;

            modContext.Items.RemoveAt(1);
            modContext.Items.Insert(1,  cs);

            modContext.KeyDown += ModContext_KeyDown;

            modContext.PreviewKeyDown += ModContext_PreviewKeyDown;

            posContext = contextMenuStrip2;

            posButton = button12;

            posButton.MouseClick += PosButton_Click;

            this.Click += CustomizeForm_MouseClick;

            WinExplorers.Debuggers.ProfessionalColorTableExtended pc = new WinExplorers.Debuggers.ProfessionalColorTableExtended();

            modContext.Renderer = new WinExplorers.Debuggers.Renderer(pc);

            posContext.Renderer = new WinExplorers.Debuggers.Renderer(pc);

            posContext.Closing += PosContext_Closing;

            posContext.LostFocus += PosContext_LostFocus;

            posContext.MouseClick += PosContext_MouseClick;

            posContext.MouseLeave += PosContext_MouseLeave;

            //posContext.ItemClicked += PosContext_ItemClicked;

            //posContext.MouseCaptureChanged += PosContext_MouseCaptureChanged;

            this.Capture = true;

        }

        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
                AdjustLists();
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
                AdjustLists();
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                AdjustLists();
        }

        private void ModContext_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void ModContext_KeyDown(object sender, KeyEventArgs e)
        {
            if (modContext.Items[1].Selected == false)
                return;

            if (e.KeyCode == Keys.Right)
            {
                cs.MoveCaret(true);
            }
            else if (e.KeyCode == Keys.Left)
                cs.MoveCaret(false);
            else if(e.KeyCode == Keys.Back)
            {
                cs.DeleteAtCaret();
            } else if(e.KeyCode == Keys.Delete)
            {
                cs.DeleteAtCaret(true);
            }
            else if (e.KeyCode == Keys.Home)
            {
                cs.SetCaret(0);
            }
            else if (e.KeyCode == Keys.End)
            {
                cs.SetCaret();
            } else
            {
                if((int)e.KeyCode >= 32)
                cs.InsertAtCaret(e.KeyData.ToString());
                //cs.SetCaret();
            }

            cs.Invalidate();
            cc.Update();
            cc.Refresh();
        }

        ToolStripCombos cs { get; set; }

        private void ModContext_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (shouldclose == false)
            {
                e.Cancel = true;
                modContext.Focus();
                modContext.Capture = true;
                modContext.Items[0].Select();
            }
            else shouldclose = false;
        }

        public void LoadToolboxList()
        {
            Dictionary<string, ToolStrip> regToolstrips = ExplorerForms.regToolstrips;

            foreach(string s in regToolstrips.Keys)
            comboBox3.Items.Add(s);
            
            comboBox3.SelectedIndex = 0;

        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Do something.
        }
        private void PosContext_MouseCaptureChanged(object sender, EventArgs e)
        {
            
        }

        private void PosContext_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void PosContext_MouseLeave(object sender, EventArgs e)
        {
            posContext.Select();
            posContext.Items[0].Select();
            this.Capture = true;
        }

        private void CustomizeForm_MouseClick(object sender, EventArgs e)
        {
            shouldclose = true;
            posContext.Close();
        }

        private void PosContext_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void CustomizeForm_Click(object sender, EventArgs e)
        {
            
        }

        private void PosContext_Click(object sender, EventArgs e)
        {
           
        }

        bool shouldclose = false;

        private void PosContext_LostFocus(object sender, EventArgs e)
        {
          
            

            //if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            //{

            //    Rectangle location = posContext.RectangleToScreen(posContext.ClientRectangle);


            //    Point p = this.PointToScreen(Cursor.Position);


            //    if (location.Contains(p) == false)
            //    {

            //        shouldclose = true;
            //        posContext.Close();
                    

            //        //MessageBox.Show("Mouse");

            //    } else posContext.Select();
            //}

            //else posContext.Select();

            //this.Capture = true;

        }

        private void PosContext_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (shouldclose == false)
            {
                e.Cancel = true;
                posContext.Focus();
                posContext.Capture = true;
                posContext.Items[0].Select();
            }
            else shouldclose = false;
            
        }

        private void PosButton_Click(object sender, EventArgs e)
        {
            
            posContext.Show(button12, 0, 20);
            thread = new Thread(Track);
            thread.Start(posContext);
        }

        Thread thread { get; set; }

        void Track(object obs)
        {

            ContextMenuStrip c = obs as ContextMenuStrip;

            completed = false;

            while (true)
            {

                try
                {
                    if (this.IsDisposed == false) this.Invoke(new Action(() => { if (this.IsDisposed == false) OutsideMenuContext(c); }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }

                if (completed == true)
                    return;

                Thread.Sleep(100);
            }
        }

        bool completed = false;
        string OutsideMenuContext(ContextMenuStrip posContexts)
        {

            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {

                //Point c = (this.Location);

                Point d = posContexts.Location;

                //c.X += d.X;
                //c.Y += d.Y;

                Rectangle r = new Rectangle(d, posContexts.Size);
                

                //location = this.RectangleToScreen(location);

                //Rectangle location = (posContext.ClientRectangle);

                Point p = (Cursor.Position);

                //Point p = Cursor.Position;


                if (r.Contains(p) == false)
                {

                    shouldclose = true;
                    posContexts.Close();

                    completed = true;

                    return "true";


                }
                else posContexts.Select();
            }

            else posContexts.Select();

            completed = false;

            return "false";
        }
        private void ModButton_Click(object sender, EventArgs e)
        {
            int i = cc.highlight;
            if(i >= 0)
            {

                ToolStripItem b = cc.Items[i];

                string name = b.Text;

                cs.ctext = name;

                int k = GetItemIndex(name);

                if(k >= 0)
                {


                    string names = module.temp[k] as string;

                    if (names.Contains("@@"))
                        imageAndTextToolStripMenuItem.Checked = true;
                    else if (names.Contains("@"))
                        textOnlyToolStripMenuItem.Checked = true;
                    else defaultStyleToolStripMenuItem.Checked = true;



                }

            }
            modContext.Show(button11, 0, 20);
            thread = new Thread(Track);
            thread.Start(modContext);
        }

        Button modButton { get; set; }

        Button posButton { get; set; }

        ContextMenuStrip modContext { get; set; }

        ContextMenuStrip posContext { get; set; }

        private void CustomizeForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.Control.GetType() == typeof(ContextMenuStrips))
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                    case Keys.Up:
                        e.IsInputKey = true;
                        break;
                }
            }
        }

        private void Panel1_Resize(object sender, EventArgs e)
        {
            if (cc == null)
                return;
            cc.Size = panel1.Size;
            foreach (ToolStripItem b in cc.Items)
                if(b.GetType() == typeof(ToolStripCombo))
                b.Width = panel1.Size.Width - 50;
            else b.Width = panel1.Size.Width;
            cc.Invalidate();
            this.Refresh();
        }

        Module module { get; set; }

        public void LoadView(string c)
        {
            string[] cc = c.Split("-".ToCharArray());

            if (cc[0] == "Commands")
            {
                tabControl1.SelectedIndex = 1;

                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;

                ComboBox cb = comboBox2;

                if (cc[1] == "Toolbar")
                {
                    radioButton2.Checked = true;
                    comboBox3.SelectedIndex = comboBox3.Items.IndexOf(cc[2]);
                    string s = cc[2];

                    module = Module.GetModule(s);


                    
                    LoadPanel(module);
                }
                else if (cc[1] == "Menubar")
                    radioButton1.Checked = true;
                else if (cc[1] == "ContextMenu")
                    radioButton3.Checked = true;




            }
            Panel1_Resize(this, null);
        }

        public void AdjustLists()
        {
            if(radioButton1.Checked == true)
            {
                comboBox2.Enabled = true;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
            }
            else if(radioButton2.Checked == true)
            {
                comboBox2.Enabled = false;
                comboBox3.Enabled = true;
                comboBox4.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                comboBox4.Enabled = true;
            }
        }

        ContextMenuStrips cc = null;
        public void LoadPanel(Module module)
        {

            if (module == null)
                return;

            ArrayList temp = module.temp;

            //Dictionary<string, Command> dict = module.dict;

            OrderedDictionary dict = module.dict;

            Font font = new Font("Times New Roman", 7);

            panel1.Controls.Clear();

            Size size = panel1.Size;

            cc = new ContextMenuStrips();

            Renderers r = new Renderers(new ColorTable());
            r.cs = cc;
            cc.Renderer = r;

            

            cc.PreviewKeyDown += Cc_PreviewKeyDown;

           // cc.Renderer = new WinExplorers.Debuggers.Renderer(new ProfessionalColorTable());

            foreach (string s in temp)
            {

                if (s == "Separator")
                {



                    ToolStripSeparator b = new ToolStripSeparator();
                    cc.Items.Add(b);
                    b.AutoSize = false;
                    b.Width = size.Width;

                    continue;
                }

                Command cmd = null;

                if (dict.Contains(s) == true)
                cmd = dict[s] as Command;

                if (cmd != null && cmd.Name == "Gui")
                {

                    gui.Command_Gui cmds = cmd as gui.Command_Gui;

                    ToolStripCombos cb = new ToolStripCombos();
                    cb.Click += D_Click;
                    cb.AutoSize = false;
                    cb.MouseEnter += Dd_MouseLeave;
                    //cb.Font = font;
                    //cb.KeyPress += Cb_KeyPress;

                    cb.Text = cmds.Names;

                    cc.Items.Add(cb);

                    cb.Width = size.Width - 30;

                    cb.Enabled = true;
                }
                else
                {
                    var d = cc.Items.Add(s);
                    ToolStripItem dd = d as ToolStripItem;
                    d.Click += D_Click;
                    dd.MouseEnter += Dd_MouseLeave;
                    
                    dd.AutoSize = false;
                    d.Width = size.Width;
                    if(cmd != null)
                    dd.Image = cmd.image;
                }



            }


            cc.TopLevel = false;

            // Attach an event handler for the 
            // ContextMenuStrip control's Opening event.
            //fruitContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);


            ToolStripMenuItem c = new ToolStripMenuItem();
            //c.DropDown = cc;



            panel1.Controls.Add(cc);
            cc.BackColor = Color.FromKnownColor(KnownColor.Control);
            cc.AutoSize = false;
            cc.Width = 500;

            cc.Show(panel1, 0, 0);
            cc.Dock = DockStyle.Fill;
            cc.Click += Cc_Click;
            cc.AutoClose = false;
            
            cc.Closing += Cc_Closing;
            cc.Closed += Cc_Closed;


            //cc.KeyPress += Cc_KeyPress;

            cc.KeyDown += Cc_KeyDown;
        }

        private void Cc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {

                int highlight = cc.highlight;

                if (highlight >= 1)
                {
                    cc.Items[highlight - 1].Select();
                    cc.highlight--;
                    if (cc.Items[cc.highlight].GetType() == typeof(ToolStripSeparator))
                    {
                        if (cc.highlight >= 1)
                        {

                            cc.Items[cc.highlight - 1].Select();

                            cc.highlight--;


                        }
                    }
                }

                cc.Refresh();
            }
            else if (e.KeyCode == Keys.Down)
            {

                int highlight = cc.highlight;

                if (highlight < cc.Items.Count - 1)
                {
                    cc.Items[highlight + 1].Select();
                    cc.highlight++;
                    if (cc.Items[cc.highlight].GetType() == typeof(ToolStripSeparator))
                    {
                        if (highlight < cc.Items.Count - 1)
                        {

                            cc.Items[cc.highlight + 1].Select();

                            cc.highlight++;


                        }
                    }
                }
                cc.Refresh();
            }
        }

        private void Cb_KeyPress(object sender, KeyPressEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Cc_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                    case Keys.Up:
                        e.IsInputKey = true;
                        break;
                }
            }
        }

        private void Cc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Up)
            {

                int highlight = cc.highlight;

                if (highlight >= 1)
                {


                    cc.Items[highlight - 1].Select();

                    cc.highlight--;

                    if (cc.Items[cc.highlight].GetType() == typeof(ToolStripSeparator))
                    {
                        if (cc.highlight >= 1)
                        {

                            cc.Items[cc.highlight - 1].Select();

                            cc.highlight--;


                        }
                    }
                }
            }
        }

        private void Dd_MouseLeave(object sender, EventArgs e)
        {
            
            //cc.highlight = cc.Items.IndexOf((ToolStripItem)sender);
            //cc.Items[cc.highlight].Select();
        }

        private void Cc_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            
        }

        private void Cc_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {

            e.Cancel = true;

        }

        private void Cc_Click(object sender, EventArgs e)
        {

        }

        private AddCommandForm adf { get; set; }

        private void button5_Click(object sender, EventArgs e)
        {

            if (module == null)
                return;

            int highlight = cc.highlight;

            if (highlight < 0)
                return;

            adf = new AddCommandForm();
            DialogResult r = adf.ShowDialog();
            if (r != DialogResult.OK)
                return;
            string c = adf.command;

            Command cmd = Command.FromName(c);

            module.dict.Insert(highlight, cmd.Name, cmd);

            module.temp.Insert(highlight, cmd.Name);

            LoadPanel(module);


        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        public void LoadToolbars()
        {
            chTB.Items.Clear();

            if (tc == null)
                return;

            foreach (string s in tc.TF)
            {
                chTB.Items.Add(s);
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == false)
                return;
            comboBox2.SelectedIndex = 0;

            // Create a new ContextMenuStrip control.
            ContextMenuStrip cc = new ContextMenuStrip();
            foreach (string s in tc.MF)
            {


                var d = cc.Items.Add(s);

                ToolStripMenuItem dd = d as ToolStripMenuItem;
                //dd.DropDownItems.Add("");
                dd.AutoSize = false;

                d.Click += D_Click;

                d.Width = 500;
            }


            cc.TopLevel = false;

            // Attach an event handler for the 
            // ContextMenuStrip control's Opening event.
            //fruitContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);


            ToolStripMenuItem c = new ToolStripMenuItem();
            //c.DropDown = cc;

            panel1.Controls.Add(cc);
            cc.BackColor = System.Drawing.Color.LightGray;
            cc.AutoSize = false;
            cc.Width = 500;

            cc.Show(panel1, 0, 0);
            cc.Dock = DockStyle.Fill;
        }

        private void D_Click(object sender, EventArgs e)
        {
            cc.highlight = cc.Items.IndexOf((ToolStripItem)sender);
            cc.Items[cc.highlight].Select();
            cc.Refresh();
        }

     
        private void button8_Click(object sender, EventArgs e)
        {
            int i = -1;
            bool found = false;
            foreach (ToolStripItem b in cc.Items)
            {
                i++;
                if (b.Selected == true)
                {
                    found = true;
                    break;
                }
             
            }

            if (found == false)
                return;

          
            if (i < 0)
                return;

            if (module == null)
                return;

            ToolStripItem c = cc.Items[i] as ToolStripItem;

            string name = c.Text;

            ArrayList GT = module.temp;

            int p = GT.IndexOf(name);

            if (p < 0)
                return;

            if (i <= 0)
                return;

            GT.RemoveAt(p);

            GT.Insert(p - 1, name);

            LoadPanel(module);

            cc.Items[p - 1].Select();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int i = -1;
            bool found = false;
            foreach (ToolStripItem b in cc.Items)
            {
                i++;
                if (b.Selected == true)
                {
                    found = true;
                    break;
                }

            }

            if (found == false)
                return;


            if (i < 0 || i >= cc.Items.Count - 1)
                return;

            if (module == null)
                return;

            ToolStripItem c = cc.Items[i] as ToolStripItem;

            string name = c.Text;

            ArrayList GT = module.temp;

            int p = GT.IndexOf(name);

            if (p < 0)
                return;

           

            GT.RemoveAt(p);

            GT.Insert(p + 1, name);

            LoadPanel(module);

            cc.Items[p + 1].Select();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int i = -1;
            bool found = false;
            foreach (ToolStripItem b in cc.Items)
            {
                i++;
                if (b.Selected == true)
                {
                    found = true;
                    break;
                }

            }

            if (found == false)
                return;


            if (i < 0 || i >= cc.Items.Count )
                return;

            if (module == null)
                return;

            ToolStripItem c = cc.Items[i] as ToolStripItem;

            string name = c.Text;

            ArrayList GT = module.temp;

            int p = GT.IndexOf(name);

            if (p < 0)
                return;

            GT.RemoveAt(p);
              
            LoadPanel(module);

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int i = -1;
            bool found = false;
            foreach (ToolStripItem b in cc.Items)
            {
                i++;
                if (b.Selected == true)
                {
                    found = true;
                    break;
                }

            }

            if (found == false)
                return;


            if (i < 0 || i >= cc.Items.Count)
                return;

            if (module == null)
                return;

            ToolStripItem c = cc.Items[i] as ToolStripItem;

            string name = c.Text;

            ArrayList GT = module.temp;

            int p = GT.IndexOf(name);

            if (p < 0)
                return;

            string names = "New Item " + cc.Items.Count;

            var d = cc.Items.Add(names);
            ToolStripMenuItem dd = d as ToolStripMenuItem;
            d.Click += D_Click;
            dd.MouseEnter += Dd_MouseLeave;
            dd.AutoSize = false;
            d.Width = panel1.Size.Width;
           

            GT.Insert(p, names);

            LoadPanel(module);

            cc.Items[p].Select();

            int w = comboBox3.SelectedIndex;

            string s = comboBox3.Items[w].ToString();

            comboBox3.Items.Insert(w, s + " " + "|" +" " + names);
            

        }

        private void dockTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExplorerForms ef = ExplorerForms.ef;

            if (ef == null)
                return;
            
                ToolStrip c = ToolCommands.TFF[0] as ToolStrip;
                ef.Command_ChangeToolstrip(c, AnchorStyles.Top);
            ClearPositionItems();
            dockTopToolStripMenuItem.Checked = true;
            posContext.Focus();
            posContext.Capture = true;


        }

        public void ClearPositionItems()
        {
            dockTopToolStripMenuItem.Checked = false;
            dockBottomToolStripMenuItem.Checked = false;
            dockLeftToolStripMenuItem.Checked = false;
            dockRightToolStripMenuItem.Checked = false;

        }

        private void dockBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExplorerForms ef = ExplorerForms.ef;

            if (ef == null)
                return;

            ToolStrip c = ToolCommands.TFF[0] as ToolStrip;
            ef.Command_ChangeToolstrip(c, AnchorStyles.Bottom);
            ClearPositionItems();
            dockBottomToolStripMenuItem.Checked = true;
            
            
        }

        private void dockLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExplorerForms ef = ExplorerForms.ef;

            if (ef == null)
                return;

            ToolStrip c = ToolCommands.TFF[0] as ToolStrip;
            ef.Command_ChangeToolstrip(c, AnchorStyles.Left);
            ClearPositionItems();
            dockLeftToolStripMenuItem.Checked = true;

        }

        private void dockRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExplorerForms ef = ExplorerForms.ef;

            if (ef == null)
                return;

            ToolStrip c = ToolCommands.TFF[0] as ToolStrip;
            ef.Command_ChangeToolstrip(c, AnchorStyles.Right);
            ClearPositionItems();
            dockRightToolStripMenuItem.Checked = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = comboBox3.SelectedIndex;
            if (i < 0)
                return;
            string s = comboBox3.Items[i].ToString();
            Dictionary<string, ToolStrip> regToolstrips = ExplorerForms.regToolstrips;
            if (regToolstrips.ContainsKey(s) == false)
                return;
            ToolStrip ts = regToolstrips[s];
            module = Module.GetModule(s);
            LoadPanel(module);

        }
        

        private void beginGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (module == null)
                return;
            int i = cc.highlight;
            if (i < 0)
                return;
            module.temp.Insert(i, "Separator");
            LoadPanel(module);
        }

        private void CustomizeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            shouldclose = true;
            modContext.Close();
            shouldclose = true;
            posContext.Close();
        }

        public string GetActiveItem()
        {
            string name = "";

            if (cc.highlight < 0)
                return name;

            name = cc.Items[cc.highlight].Text;

            return name;
        }

        private void imageAndTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool imageandtext = true;

            if (imageAndTextToolStripMenuItem.Checked == true)
            {
                imageAndTextToolStripMenuItem.Checked = false;
                textOnlyToolStripMenuItem.Checked = false;
                defaultStyleToolStripMenuItem.Checked = false;
                imageandtext = false;
            }
            else
            {
                imageAndTextToolStripMenuItem.Checked = true;
                textOnlyToolStripMenuItem.Checked = false;
                defaultStyleToolStripMenuItem.Checked = false;
            }

            if (module == null)
                return;

            string name = GetActiveItem();

            if (name == "")
                return;

            int i = GetItemIndex(name);

            if (i < 0)
                return;

            if (imageandtext == true)
                module.temp[i] = name + "@@";
            else module.temp[i] = name;
        }

        private void textOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool textonly = true;
            if (textOnlyToolStripMenuItem.Checked == true)
            {
                defaultStyleToolStripMenuItem.Checked = false;
                textOnlyToolStripMenuItem.Checked = false;
                imageAndTextToolStripMenuItem.Checked = false;
                textonly = false;
            }
            else
            {
                defaultStyleToolStripMenuItem.Checked = false;
                textOnlyToolStripMenuItem.Checked = true;
                imageAndTextToolStripMenuItem.Checked = false;
            }

            if (module == null)
                return;

            string name = GetActiveItem();

            if (name == "")
                return;
            
           
            int i = GetItemIndex(name);

            if (i < 0)
                return;

            if(textonly == true)
            module.temp[i] = name + "@";
            else module.temp[i] = name;

        }

        private void defaultStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (defaultStyleToolStripMenuItem.Checked == true)
            {
                defaultStyleToolStripMenuItem.Checked = false;
                textOnlyToolStripMenuItem.Checked = false;
                imageAndTextToolStripMenuItem.Checked = false;
            }
            else
            {
                defaultStyleToolStripMenuItem.Checked  = true;
                textOnlyToolStripMenuItem.Checked = false;
                imageAndTextToolStripMenuItem.Checked = false;
            }
            if (module == null)
                return;

            string name = GetActiveItem();

            if (name == "")
                return;

            int i = module.temp.IndexOf(name);

            if (i < 0)
                return;

            module.temp[i] = name + "@";
        }

        int GetItemIndex(string name)
        {

            int i = -1;
            int k = 0;
            while (k < module.temp.Count)
                if (((string)module.temp[k++]).Contains(name))
                    i = k - 1;
            return i;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (module == null)
                return;
            string s = module.Name;

            if (ExplorerForms.regToolstrips.ContainsKey(s) == false)
                return;

            ToolStrip ts = ExplorerForms.regToolstrips[s];

            if (ts == null)
                return;

            ExplorerForms.ef.Command_ReloadToolbar(ts, s);

            this.Close();

        }
    }

    public class ToolCommands
    {

        static public ArrayList TFF { get; set; }

        public ArrayList MF { get; set; }

        public ArrayList TF { get; set; }

        public ArrayList BF { get; set; }

        public ToolCommands()
        {

            GetToolbars();
            GetMenubars();

        }



        public ArrayList GetToolbars()
        {

            TF = new ArrayList();

            TF.Add("Standard");
            TF.Add("Build");
            TF.Add("Debug");

            return TF;

        }
        public ArrayList GetMenubars()
        {

            MF = new ArrayList();

            MF.Add("File");
            MF.Add("Edit");
            MF.Add("View");
            MF.Add("Project");
            MF.Add("Build");
            MF.Add("Debug");
            MF.Add("Tools");
            MF.Add("Test");
            MF.Add("Analyze");
            MF.Add("Window");
            MF.Add("Help");
            return MF;




        }

    }
    public class ContextMenuStrips : ContextMenuStrip
    {


        protected override void OnMouseLeave(EventArgs e)
        {
            //base.OnMouseLeave(e);
            if(highlight >= 0)
            this.Items[highlight].Select();
            this.Focus();

            
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Focus();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            //base.OnLostFocus(e);
        }
        protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
        {
            
            //base.OnClosing(e);
            e.Cancel = true;
        }
        protected override void OnLeave(EventArgs e)
        {
            //base.OnLeave(e);
        }

        public int highlight = -1;

        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            //base.OnClosed(e);
            
        }
        protected override void OnMouseHover(EventArgs e)
        {
            //base.OnMouseHover(e);
          
        }
        protected override void OnMouseEnter(EventArgs e)
        {
           // base.OnMouseEnter(e);

        }
        //protected override void OnMouseMove(EventArgs e)
        //{
            // base.OnMouseMove(e);

        //}
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                    e.IsInputKey = true;
                    break;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    // left arrow key pressed
                    return true;
                case Keys.Right:
                    // right arrow key pressed
                    return true;
                case Keys.Up:
                    // up arrow key pressed
                    return true;
                case Keys.Down:
                    // down arrow key pressed
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
            
        }
        
    }


    public class Combos : UserControl
    {

        public Combos()
        {
            this.Size = new Size(400, 20);
            this.text = new Label();
            this.text.Location = new Point(0, 0);
            this.cb = new TextBox();
            this.AutoSize = false;
            this.cb.Height = 15;
            this.cb.Width = 300;
            this.cb.Location = new Point(200, 0);
            //this.cb.DropDown += (o, e) => ((ComboBox)o).DroppedDown = false;
            this.cb.Dock = DockStyle.Right;
            this.Click += Cb_GotFocus;    
            this.Controls.Add(text);
            this.Controls.Add(cb);
            brush = new SolidBrush(Color.FromArgb(150, SystemColors.Highlight));
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);

        }

        private void Cb_GotFocus(object sender, EventArgs e)
        {
            this.Focus();
        }

        public Label text { get; set; }

        public TextBox cb { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Rectangle r = new Rectangle(-100, 0, this.Width, this.Height);
            r.Inflate(-1, -1);
            pe.ClipRectangle.Inflate(-1, -1);
            pe.Graphics.FillRectangle(brush, r);
        }

        Brush brush { get; set; }

    }

    //Declare a class that inherits from ToolStripControlHost.
    public class ToolStripCombo : ToolStripControlHost
    {
        
        public ToolStripCombo() : base(new Label()) {


           
            this.BackColor = Color.FromKnownColor(KnownColor.Control);
           
            brush = new SolidBrush(Color.FromArgb(150, SystemColors.Highlight));
            
            
        }

        private void Cb_GotFocus(object sender, EventArgs e)
        {
            
        }
        public Control control
        {
            get
            {
                return Control as Combos;
            }
        }

        Brush brush { get; set; }
        protected override void OnPaint(PaintEventArgs pe)
        {
           
            base.OnPaint(pe);
            Rectangle r = new Rectangle(-100, 0, this.Width, this.Height);
            r.Inflate(-1, -1);
            pe.ClipRectangle.Inflate(-1, -1);
            pe.Graphics.FillRectangle(brush, r);
            
            
        }

    }

    public class Threads
    {

        public void Track()
        {

        }


    }

    public class ToolStripCombos : ToolStripMenuItem
    {
      
        

        public ToolStripCombos()
        {
            b = new ComboBox();
            b.Size = new Size(400, 16);
            c = Color.FromArgb(255, Color.Black);
            brush = new SolidBrush(c);
            bb = new SolidBrush(Color.Black);
            bw = new SolidBrush(Color.White);
            TextBox tb = new TextBox();
            tb.Size = new Size(300, 20);
            tb.Location = new Point(100, 0);


            Blink();



        }

        private async void Blink()
        {
            while (this.IsDisposed == false)
            {
                await System.Threading.Tasks.Task.Delay(500);
                color = color == Color.Black ? Color.White : Color.Black;
                this.Invalidate();
                this.Parent.Refresh();
            }
        }

        public Color color = Color.Black;

        ComboBox b { get; set; }

        Color c { get; set; }

        Brush bw { get; set; }

        Brush bb { get; set; }

        Brush brush { get; set; }

        public ContextMenuStrips cs { get; set; }

        public int caret = 0;

        public bool drawarrow = true;

        public string ctext {
            get; set;
        }

        public Font font { get; set; }



        public Brush GetBrush()
        {
            if (color == Color.White)
                return bw;
            else return bb;

        }

        public void SetCaret()
        {

            caret = ctext.Length - 1;

            if (caret < 0)
                caret = 0;

        }

        public void SetCaret(int c)
        {
            caret = c;
        }

        int GetCaret(Graphics g)
        {
            if (font == null)
                return 0;

            if (caret > ctext.Length)
                caret = ctext.Length;

            string c = ctext.Substring(0, caret);

            SizeF s = g.MeasureString(c, font);

            return (int)s.Width;
        }

        public void MoveCaret(bool right)
        {
            if(right == true)
            {
                if (caret < ctext.Length)
                    caret++;
            }
            else
            {
                if (caret > 0)
                    caret--;
            }
        }

        public void DeleteAtCaret(bool delete = false)
        {
            if (delete == false)
            {
                if (caret > 0)
                {
                    ctext = ctext.Remove(caret - 1, 1);
                    caret--;
                }
            }
            else if (caret < ctext.Length - 1)
                ctext = ctext.Remove(caret, 1);
        }

        public void InsertAtCaret(string c)
        {

            ctext = ctext.Insert(caret, c);

            caret++;

        }

        public bool IsSelected()
        {
            if (cs == null)
                return false;

            int i = cs.Items.IndexOf(this);

            if (i == cs.highlight)
                return true;


            return false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int w = this.Width;

            if (ctext == null)
                ctext = "";



            b.Text = this.Text;

            int text = (int)e.Graphics.MeasureString(Text, b.Font).Width;

            int x = w - text -15 - 50;

            if(drawarrow == false)
                if (this.IsSelected())
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);

            if (ComboBoxRenderer.IsSupported)

                if (font == null)
                    ComboBoxRenderer.DrawTextBox(e.Graphics, new Rectangle(text + 10 + 50, 2, x - 5, 16), System.Windows.Forms.VisualStyles.ComboBoxState.Pressed);

                else
                {
                    int start = 0;
                    int ax = GetCaret(e.Graphics);
                    int ay = 8;

                    if (ax > x)
                        start = ax - x;

                    ComboBoxRenderer.DrawTextBox(e.Graphics, new Rectangle(text + 10 + 50, 2, x - 5, 17), ctext.Substring(start, ctext.Length - start), font, TextFormatFlags.VerticalCenter, System.Windows.Forms.VisualStyles.ComboBoxState.Pressed);

                  

                    Point p = new Point(ax, ay);

                    p.X += text + 10 + 50;
                    string c = Char.ConvertFromUtf32('\u258F');
                    Brush br = GetBrush();
                    e.Graphics.DrawString(c, font, br, p);

                    
                }
            if (drawarrow == true)
            {
                ComboBoxRenderer.DrawDropDownButton(e.Graphics, new Rectangle(w - 22, 2, 20, 16), System.Windows.Forms.VisualStyles.ComboBoxState.Normal);


                if (this.IsSelected())
                    e.Graphics.FillRectangle(brush, e.ClipRectangle);
                
            }
        }
    }

    public class ColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected { get { return Color.Transparent; } }

        public override Color MenuItemBorder { get { return Color.Transparent; } }
    }

    public class Renderers : ToolStripProfessionalRenderer
    {
        public Renderers() : base() {
        }
        public Renderers(ProfessionalColorTable c): base(c)
         {
            this.c = c;
           
            brush = new SolidBrush(Color.FromArgb(150, c.CheckSelectedBackground));
        }

        public ProfessionalColorTable c { get; }
      
        public ContextMenuStrips cs { get; set; }

        public Brush brush { get; set; }


          protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);

            int i = cs.Items.IndexOf(e.Item);

          

            if (i < 0)
                return;

            if (cs.highlight == i)
                
            e.Graphics.FillRectangle(brush, 0,0, cs.Width, 20);

            //else e.Graphics.FillRectangle(Brushes.LightGray, e.Item.Bounds);

        }
      
    }
}