using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NuGet;
using VSProvider;
using System.IO;

namespace WinExplorer.Services.NuGet
{
    public partial class NuGetPacketInfo : UserControl
    {
        public NuGetPacketInfo(VSProject vp)
        {
            InitializeComponent();
            this.vp = vp;
            pLabel = label1;
            labelLine1.SetParent(this);
            labelLine2.SetParent(this);
            pbIcon = pictureBox1;
            opPanel = panel1;
            rbDesc = richTextBox1;
            
            cbDep = comboBox1;
            cbCon = comboBox2;
            cbUpdate = comboBox3;

            cbDep.SelectedIndex = 0;
            cbCon.SelectedIndex = 0;

            tbInstalled = textBox1;

            this.AutoScroll = false;
            this.cb = this.comboBox3;
        }

        public IPackage package { get; set; }
        Label pLabel;
        PictureBox pbIcon { get; set; }
        RichTextBox rbDesc { get; set; }

        ComboBox cbDep { get; set; }

        ComboBox cbCon { get; set; }

        ComboBox cbUpdate { get; set; }

        TextBox tbInstalled { get; set; }

        VSProject vp { get; set; }

        ComboBox cb { get; set; }

        NuGetForm.task tasks { get; set; }
        public void LoadPackage(IPackage package, IPackageRepository repo, NuGetForm.task task, bool isInstalled)
        {
            if (package == null)
                return;
            this.Dock = DockStyle.Fill;

            if(task == NuGetForm.task.browse)
            {
                SetBrowse(isInstalled);
            }
            if (task == NuGetForm.task.installed)
            {
                SetInstalled(package);
            }
            this.tasks = task;
            this.package = package;
            pLabel.Text = package.GetFullName();
            LoadIcon();
            LoadDescription();
            List<IPackage> packages = NuGets.FindPackageById(package.Id, repo);
            cb.Items.Clear();
            foreach (IPackage p in packages)
            {
                cb.Items.Add(p.Version.ToFullString());
            }


        }

        public void LoadDescription()
        {
            rbDesc.AppendText("Description");
            rbDesc.AppendText("r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText(package.Description);
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            
            rbDesc.SelectionHangingIndent = 110;
            rbDesc.AppendText("Version:");
            rbDesc.AppendText(package.Version.ToFullString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("Author(s):");
            foreach (string s in package.Authors)
            {
                rbDesc.AppendText(s + ", ");
            }
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("License:   ");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            if(package.LicenseUrl != null)
            rbDesc.AppendText(package.LicenseUrl.ToString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("Date published:   ");
            rbDesc.AppendText(package.Published.ToString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("Project URL:");
            if(package.ProjectUrl != null)
            rbDesc.AppendText(package.ProjectUrl.ToString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("Report_Abuse:   ");
            rbDesc.AppendText(package.ReportAbuseUrl.ToString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("Tags:   ");
            rbDesc.AppendText(package.Tags.ToString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("Dependencies:   ");
            foreach(var s in package.DependencySets)
            rbDesc.AppendText(s.ToString());
            rbDesc.AppendText("\r\n");
            rbDesc.AppendText("\r\n");
            SetAsBold("Description");
            SetAsBold("Version:");
            SetAsBold("Author(s):");
            SetAsBold("License:");
            SetAsBold("Date published:");
            SetAsBold("Project URL:");
            SetAsBold("Report_Abuse:");
            SetAsBold("Tags:");
            SetAsBold("Dependency:");
        }
        public void SetAsBold(string s)
        {
            rbDesc.Find(s, RichTextBoxFinds.MatchCase);

            rbDesc.SelectionFont = new Font("Verdana", 8.25f, FontStyle.Bold);
            rbDesc.SelectionColor = Color.Black;
        }
        public void LoadIcon()
        {
            
            if (package.IconUrl != null)
            {
                try
                {
                    Bitmap icon = NuGets.NuGetUrl(package.IconUrl);
                    if (icon != null)
                        pbIcon.Image = icon;
                }
                catch (Exception ex)
                {

                }
            }
        }

        Panel opPanel { get; set; }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (opPanel.Visible == true)
            {
                opPanel.Visible = false;
                labelLine2.Location = new Point(labelLine1.Location.X, opPanel.Location.Y  + 25);
                rbDesc.Location = new Point(rbDesc.Location.X, labelLine2.Location.Y + 10);
            }
            else
            {
                opPanel.Visible = true;
                labelLine2.Location = new Point(labelLine1.Location.X, opPanel.Location.Y + opPanel.Height + 5);
                rbDesc.Location = new Point(rbDesc.Location.X, labelLine2.Location.Y + 25);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void SetBrowse(bool isInstalled)
        {
            if (isInstalled == false)
            {
                this.Controls.Remove(label2);
                this.Controls.Remove(textBox1);
                //this.Controls.Remove(comboBox3);
                this.Controls.Remove(button2);
                this.button3.Text = "Install";
            }
            else button2.Text = "Uninstall";
        }
        private void SetInstalled(IPackage package)
        {
            tbInstalled.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
            tbInstalled.Text = package.Version.ToFullString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ready to uninstall");

            string path = vp.vs.SolutionPath + "\\packages";

            PackageManager p = NuGets.UninstallPackage(package.Id, path, package.Version.ToFullString(), false);

            //string s = p.PathResolver.GetPackageFileName;

            // var c = Directory.GetFiles(path + "\\" + package.AssemblyReferences.ToList()[0].EffectivePath, SearchOption.AllDirectories);


            string file = path + "\\" + package.Id + "." + package.Version.ToFullString() + "\\" + package.AssemblyReferences.ToList()[0].Path;

            vp.RemoveReference(file);

            //foreach (var d in package.DependencySets)
            //{

            //    foreach (var de in d.Dependencies)
            //    {

            //        IPackage package = p.LocalRepository.FindPackage(de.Id);
            //        MessageBox.Show("Package " + package.GetFullName() + "found");

            //        file = path + "\\" + package.Id + "." + package.Version.ToFullString() + "\\" + package.AssemblyReferences.ToList()[0].Path;

            //        vp.RemoveReference(file);
            //    }
            //}

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tasks == NuGetForm.task.browse || tasks == NuGetForm.task.installed)
                InstallPackage();
            else
                UpdatePackage();
        }

        void InstallPackage()
        {
            if (vp == null)
                return;

            string path = vp.vs.SolutionPath + "\\packages";
            PackageManager p = NuGets.InstallPackage(package.Id, path, package.Version.ToFullString());

            //string s = p.PathResolver.GetPackageFileName;

            // var c = Directory.GetFiles(path + "\\" + package.AssemblyReferences.ToList()[0].EffectivePath, SearchOption.AllDirectories);

            string file = path + "\\" + package.Id + "." + package.Version.ToFullString() + "\\" + package.AssemblyReferences.ToList()[0].Path;

            vp.SetReference(file);

            foreach (var d in package.DependencySets)
            {

                foreach (var de in d.Dependencies)
                {

                    IPackage package = p.LocalRepository.FindPackage(de.Id);
                    MessageBox.Show("Package " + package.GetFullName() + "found");

                    file = path + "\\" + package.Id + "." + package.Version.ToFullString() + "\\" + package.AssemblyReferences.ToList()[0].Path;

                    vp.SetReference(file);
                }
            }

        }
        void UpdatePackage()
        {
            if (vp == null)
                return;

            string path = vp.vs.SolutionPath + "\\packages";

            string version = cbUpdate.Text;

            if (string.IsNullOrEmpty(version))
                return;

            //Connect to the official package repository
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            PackageManager pp  = new PackageManager(repo, path);
            string files = path + "\\" + package.Id + "." + package.Version.ToFullString() + "\\" + package.AssemblyReferences.ToList()[0].Path;
            vp.RemoveReference(files);

            PackageManager p = NuGets.UpdatePackage(package.Id, path, version, false);
            string file = path + "\\" + package.Id + "." + version + "\\" + package.AssemblyReferences.ToList()[0].Path;
            

            vp.SetReference(file);

            //foreach (var d in package.DependencySets)
            //{

            //    foreach (var de in d.Dependencies)
            //    {

            //        IPackage package = p.LocalRepository.FindPackage(de.Id);
            //        MessageBox.Show("Package " + package.GetFullName() + "found");

            //        file = path + "\\" + package.Id + "." + package.Version.ToFullString() + "\\" + package.AssemblyReferences.ToList()[0].Path;

            //        vp.SetReference(file);
            //    }
            //}

        }
    }
}
