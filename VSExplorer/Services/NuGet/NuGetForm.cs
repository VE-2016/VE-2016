using NuGet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using WinExplorer.UI;

namespace WinExplorer.Services.NuGet
{
    public partial class NuGetForm : Form
    {
        public NuGetForm(VSProject vs)
        {
            InitializeComponent();
            string project = vs.Name;
            this.vs = vs;
            main = label1;
            main.Font = new Font(main.Font.FontFamily, 14);
            Font fnt = new Font(main.Font.FontFamily, 10, FontStyle.Underline | FontStyle.Regular);
            Font font = new Font(main.Font.FontFamily, 10, FontStyle.Regular);
            lbrowse = labelEx1;
            lbrowse.SetMouseLeaveFont(fnt);
            lbrowse.SetMouseOverFont(font);
            linstalled = labelEx2;
            linstalled.SetMouseLeaveFont(fnt);
            linstalled.SetMouseOverFont(font);
            lupdates = labelEx3;
            lupdates.SetMouseLeaveFont(fnt);
            lupdates.SetMouseOverFont(font);

            sp = splitContainer1;

            dg = flowLayoutPanel1;

            dg.AutoScroll = true;

            this.sp.Panel1.Resize += NuGetForm_Resize;

            dg.Resize += Dg_Resize;

            if (project != "")
            {
                main.Text = "NuGet Project Manager: " + project;
            }

            
            dg.Scroll += Dg_Scroll;

            cbSource = comboBox1;

            cbSource.Items.Add("All");
            cbSource.Items.Add("nuget.org");
            cbSource.Items.Add("VE Offline files");

            cbSource.SelectedIndexChanged += CbSource_SelectedIndexChanged;

            cbSource.SelectedIndex = 1;

            searchText = searchTextBox1;

            LoadInstalled(false);

            UnloadUpdate();

            dg.HorizontalScroll.Visible = false;
        }

        private void CbSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cbSource.SelectedIndex;
            if (i < 0)
                return;

        }

        ComboBox cbSource { get; set; }

        SearchTextBox searchText { get; set; }

        public enum task
        {
            browse,
            installed,
            update,
            search,
            none
        }

        task tasks = task.none;

        public bool GetPrerelease()
        {
            return checkBox1.Checked;
        }

        private void Dg_Scroll(object sender, ScrollEventArgs e)
        {
            VScrollProperties vs = dg.VerticalScroll;
            if (e.NewValue == vs.Maximum - vs.LargeChange + 1)
            {
                if (tasks == task.browse)
                {
                    LoadNextPackages();
                } else if( tasks == task.search)
                {
                    LoadNextPackages(searchText.GetText());
                }
            }
        }

        VSProject vs { get; set; }



        private void Dg_Resize(object sender, EventArgs e)
        {
            ////dg.Width = sp.Panel1.Width - 15;
            foreach (Control c in dg.Controls)
                c.Width = this.dg.ClientRectangle.Width - 10;
            //foreach (NuGetPacketShort c in dg.Controls)
            //    c.ResizeShortinfo();
        }

        private void NuGetForm_Resize(object sender, EventArgs e)
        {
            dg.Width = sp.Panel1.Width - 10;
        }

        Label main { get; set; }
        LabelEx lbrowse { get; set; }
        LabelEx linstalled { get; set; }

        LabelEx lupdates { get; set; }

        FlowPanel dg { get; set; }

        SplitContainer sp { get; set; }

        ImageList imgList = new ImageList();

        public void LoadForVSProject()
        {



            NuGetPacketShort s = new NuGetPacketShort();
            s.Width = this.Width - 1;
            //s.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dg.Controls.Add(s);
            s = new NuGetPacketShort();
            //s.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            s.Width = this.Width - 1;
            dg.Controls.Add(s);
            s = new NuGetPacketShort();
            s.Width = this.Width - 1;
            //s.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dg.Controls.Add(s);

        }

        private void NuGetForm_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string text = searchText.GetText();
            if (string.IsNullOrEmpty(text))
                return;
            tasks = task.search;
            EmptyNuGetInfo();
            EmptyNuGet();
            currentPackage = 0;
            LoadNextPackages(text, GetPrerelease());
        }

        int currentPackage = 0;

        List<IPackage> q = null;

        private void labelEx1_Click(object sender, EventArgs e)
        {
            EmptyNuGetInfo();
            NuGets nuget = new NuGets();
            tasks = task.browse;
            if (repo == null)
                repo = nuget.GetRepository();
             //IQueryable<IPackage> q = nuget.repo.GetPackages();
            currentPackage = 0;

            UnloadUpdate(tasks);

            dg.ClearControls();
            dg.Refresh();

            this.BeginInvoke(new Action(() =>
            {
                q = NuGets.Select(currentPackage, 10, repo);
                int i = 0;
                while (i < 10)
                {
                    if (i >= q.Count)
                        break;
                    IPackage p = q[i];
                    AddNugetPackageInfo(p);
                    if (i > 10)
                        break;
                    i++;
                    currentPackage++;
                }

            }));
        }



        IPackageRepository repo = null;

        void LoadNextPackages(string text = "Microsoft", bool prerelease = false)
        {
            NuGets nuget = new NuGets();
            if (repo == null)
                repo = nuget.GetRepository();

            this.BeginInvoke(new Action(() =>
            {


                q = NuGets.Select(currentPackage, 10, repo, text, prerelease);

                int i = 0;
                while (i < 10)
                {
                    if (i >= q.Count)
                        break;

                    IPackage p = q[i];

                    AddNugetPackageInfo(p);
                    if (i > 10)
                        break;
                    i++;
                    currentPackage++;
                }

            }));
        }

        public void AddNugetPackageInfo(IPackage package, task tasks = task.none)
        {
            NuGetPacketShort s = new NuGetPacketShort(tasks);
            //s.Width = this.sp.Panel1.Width - 5;
            string text = "";
            foreach (string b in package.Authors)
                text += " " + b;

            s.SetMainText("" + package.GetFullName() + "  by " + text + "\n" + package.Description);
            s.SetAsBold(package.GetFullName());
            if (package.IconUrl != null)
            {
                try
                {
                    Bitmap icon = NuGets.GetIcon(package.IconUrl);//NuGets.NuGetUrl(package.IconUrl);
                    if (icon == null || icon.Width == 0)
                        icon = Resources.VSO_NugetLogo_52x;
                    s.SetIcon(icon);
                }
                catch (Exception ex)
                {

                }
            } else
            {
                Bitmap icon = Resources.VSO_NugetLogo_52x;
                s.SetIcon(icon);
            }
            if (package.Version != null)
                s.SetVersion(package.Version.ToFullString());
            else s.SetVersion("");
            s.nugetShortSelectedEvent += S_nugetShortSelectedEvent;

            s.SetPackage(package);
            s.AutoScroll = false;
            dg.AddControl(s);
            //dg.Refresh();
        }

        void UnloadUpdate(NuGetForm.task tasks = task.none)
        {
            return;
            if (tasks != task.update)
            {
                sp.Panel1.Controls.Remove(panel1);
                dg.Location = new Point(0, 0);
            }
            else
            {
                sp.Panel1.Controls.Add(panel1);
                panel1.Location = new Point(0, 0);
                panel1.Width = sp.Panel1.Width - 1;
                panel1.BorderStyle = BorderStyle.FixedSingle;
                dg.Location = new Point(0, panel1.Height + 5);
            }
        }

        NuGetPacketInfo ns { get; set; }

        private void S_nugetShortSelectedEvent(NuGetPacketShort sender)
        {
            NuGetPacketShort ng = sender as NuGetPacketShort;
            if (ng == null)
                return;
            IPackage package = ng.package;
            if (package == null)
                return;
            if (ns == null) { 
                ns = new NuGetPacketInfo(vs);
            ns.Dock = DockStyle.Fill;
            //sp.Panel2.Controls.Clear();
            sp.Panel2.Controls.Add(ns);
        }
            bool isInstalled = false;
            foreach(IPackage p in InstalledPackages)
                if(p.GetFullName() == package.GetFullName())
                {
                    isInstalled = true;
                    break;
                }
            
            ns.LoadPackage(package, repo, tasks, isInstalled);

            
        }

        List<IPackage> InstalledPackages = null;

        private void labelEx2_Click(object sender, EventArgs e)
        {
            EmptyNuGetInfo();
            InstalledPackages = new List<IPackage>();
            if(repo == null)
            {
                NuGets nuget = new NuGets();
                repo = nuget.GetRepository();
            }
            if (vs == null)
                return;
            tasks = task.installed;
            UnloadUpdate(tasks);

            dg.Controls.Clear();
            dg.Refresh();
            ArrayList L = vs.GetReferences();
            foreach (string p in L)
            {

                string hint = vs.GetPathHint(p);
                if (String.IsNullOrEmpty(hint))
                    continue;
                string folder = vs.vs.SolutionPath + hint;

                if (File.Exists(folder))
                {

                }

                string pp = "";

                string bg = vs.GetProjectFolder() + "\\" + hint;

                string[] cc = bg.Split("\\".ToCharArray());

                List<string> bb = new List<string>(cc);

                int i = bb.IndexOf("packages");

                int j = 0;
                foreach (string s in bb)
                {
                    if (j != 0)
                        pp += "\\" + s;
                    else pp = s;

                    if (j > i)
                        break;
                    j++;
                }


                pp = Path.GetFullPath(pp);

                if (!Directory.Exists(pp))
                    continue;

                string[] dd = Directory.GetFiles(pp);

                IPackage package = null;

                foreach (string d in dd)
                {
                    if (d.EndsWith(".nupkg"))
                    {
                        package = NuGets.ReadNuSpec(d);

                        break;
                    }
                }

                if (package == null)
                    continue;

                InstalledPackages.Add(package);
                
                {
                    IPackage packages = NuGets.GetNuGetPackage(package.Id, package.Version.ToFullString());
                    if (packages != null)
                        AddNugetPackageInfo(packages);
                }

            }
        }

        private void LoadInstalled(bool loadInfo = false)
        {
            InstalledPackages = new List<IPackage>();
            if (vs == null)
                return;
            
            dg.Controls.Clear();
            dg.Refresh();
            ArrayList L = vs.GetReferences();
            foreach (string p in L)
            {

                string hint = vs.GetPathHint(p);
                if (String.IsNullOrEmpty(hint))
                    continue;
                string folder = vs.vs.SolutionPath + hint;

                if (File.Exists(folder))
                {

                }

                string pp = "";

                string bg = vs.GetProjectFolder() + "\\" + hint;

                string[] cc = bg.Split("\\".ToCharArray());

                List<string> bb = new List<string>(cc);

                int i = bb.IndexOf("packages");

                int j = 0;
                foreach (string s in bb)
                {
                    if (j != 0)
                        pp += "\\" + s;
                    else pp = s;

                    if (j > i)
                        break;
                    j++;
                }




                if (!Directory.Exists(pp))
                    continue;

                string[] dd = Directory.GetFiles(pp);

                IPackage package = null;

                foreach (string d in dd)
                {
                    if (d.EndsWith(".nupkg"))
                    {
                        package = NuGets.ReadNuSpec(d);

                        break;
                    }
                }

                if (package == null)
                    continue;

                InstalledPackages.Add(package);
                if (loadInfo == true)
                {
                    IPackage packages = NuGets.GetNuGetPackage(package.Id,package.Version.ToFullString());
                    if (packages != null)
                        AddNugetPackageInfo(packages);
                }
            }
        }


        private void labelEx3_Click(object sender, EventArgs e)
        {
            EmptyNuGetInfo();
            tasks = task.update;

            UnloadUpdate(tasks);


            if (InstalledPackages == null)
            {
                LoadInstalled();
            }
                

            NuGets nuget = new NuGets();
            if (repo == null)
                repo = nuget.GetRepository();
            List<IPackage> updates = NuGets.GetUpdates(InstalledPackages, repo);
            dg.Controls.Clear();
            dg.Refresh();

            foreach (IPackage package in updates)
            {
                AddNugetPackageInfo(package, tasks);
            }
        }
        void EmptyNuGetInfo()
        {
            sp.Panel2.Controls.Clear();
            sp.Panel2.Refresh();
            ns = null;
        }
        void EmptyNuGet()
        {
            dg.Controls.Clear();
            dg.Refresh();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadOptions();
        }
        private OptionsForms opsf { get; set; }

        private void LoadOptions()
        {
            opsf = new OptionsForms();
            opsf.scr = ExplorerForms.ef.scr;
            opsf.Show();
        }
    }
}
