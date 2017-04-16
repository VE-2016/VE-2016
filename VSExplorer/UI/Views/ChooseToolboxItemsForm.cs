using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GACManagerApi;
using GACProject;

namespace WinExplorer.UI.Views
{
    public partial class ChooseToolboxItemsForm : Form
    {

        Module[] modules = new Module[10];

        TabControl tc { get; set; }

        public ChooseToolboxItemsForm()
        {
            InitializeComponent();
            tc = tabControl1;
            tc.SelectedIndexChanged += Tc_SelectedIndexChanged;

            modules[0] = new Module();
            modules[0].v = listView1;
            modules[1] = new Module();
            modules[1].v = listView2;
            modules[2] = new Module();
            modules[2].v = listView3;


            InitListView(modules[0].v);
            InitListView(modules[1].v);
            InitListView(modules[2].v);
        }

        private void Tc_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = tc.SelectedIndex;
            if (i < 0)
                return;
            if (i >= modules.Length)
                return;
            if(i == 0)
            {
                ArrayList C = LoadNETFrameowrkComponents();
                modules[0].Init(C);
            }
            else if (i == 1)
            {
                ArrayList C = LoadCOMComponents();
                modules[1].Init(C);
            }
            else if (i == 2)
            {
                ArrayList C = LoadWPFComponents();
                modules[2].Init(C);
            }

        }

        public void InitListView(ListView v)
        {
            v.View = View.Details;
            v.ForeColor = Color.Blue;
            v.Sorting = SortOrder.Ascending;
            v.Items.Clear();
            v.Columns.Add("Name");
            v.Columns.Add("Namespace");
            v.Columns.Add("Assembly Name");
            v.Columns.Add("Version");
            v.Columns.Add("Directory");
        }
        public ArrayList LoadNETFrameowrkComponents()
        {
            Task<ArrayList> F = GACManagerApi.GacUtil.GetFramework("");
            ArrayList G = F.Result;
            ArrayList C = Toolbox.LoadNETFrameworkComponents(G);
            return C;

        }
        public ArrayList LoadCOMComponents()
        {
            ArrayList F = GACProject.GACForm.GetTLB();
            ArrayList C = Toolbox.LoadCOMComponents(F);
            return C;

        }
        public ArrayList LoadWPFComponents()
        {
            ArrayList F = GACManagerApi.GacUtil.GetWPF();
            ArrayList C = Toolbox.LoadWPFFrameworkComponents(F);
            return C;
        }
    }
    public class Module
    {
        public ListView v { get; set; }

        public TabPage tp { get; set; }
        
        public TabControl tc { get; set; }

        public bool active = false;

        public void Init(ArrayList C)
        {
            if (active)
                return;
            v.Items.Clear();
            foreach (Type T in C)
            {
                ListViewItem b = new ListViewItem();
                b.Text = T.Name;
                b.SubItems.Add(T.Namespace);
                b.SubItems.Add(T.Assembly.FullName);
                v.Items.Add(b);
            }

        }   

    }
}
