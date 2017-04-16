using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinExplorer;

using Microsoft.Samples.Tools.Mdbgs;

namespace WinExplorers
{
    public class Debuggers
    {
        public ToolStrip ts { get; set; }

        public Debuggers()
        {
            ts = CreateToolstrip();
        }

        ToolStripButton breakall { get; set; }

        ToolStripButton stop { get; set; }

        ToolStripButton restart { get; set; }

        ToolStripButton refresh { get; set; }

        ToolStripButton stepin { get; set; }

        ToolStripButton stepout { get; set; }

        ToolStripButton stepover { get; set; }

        ToolStripButton thread { get; set; }

        ToolStripSeparator s0 { get; set; }

        ToolStripSeparator s1 { get; set; }

        ToolStripSeparator s2 { get; set; }

        ToolStripDropDownButton dd { get; set; }

        public ToolStrip CreateToolstrip()
        {
            ToolStrip t = new ToolStrip();
            // break all, stop debugging, restart. Refresh Windoes app, Show next statement
            // continue, break all

            breakall = new ToolStripButton();
            breakall.DisplayStyle = ToolStripItemDisplayStyle.Image;
            breakall.Image = Resources.Breakall;
            breakall.ImageTransparentColor = Color.Magenta;
            breakall.Click += new System.EventHandler(BreakAllHandler);

            stop = new ToolStripButton();
            stop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            stop.Image = Resources.Stop;
            stop.ImageTransparentColor = Color.Magenta;
            stop.Click += new System.EventHandler(BreakAllHandler);

            restart = new ToolStripButton();
            restart.DisplayStyle = ToolStripItemDisplayStyle.Image;
            restart.Image = Resources.Restart_16x;
            restart.ImageTransparentColor = Color.Magenta;
            restart.Click += new System.EventHandler(RestartHandler);

            s0 = new ToolStripSeparator();

            refresh = new ToolStripButton();
            refresh.DisplayStyle = ToolStripItemDisplayStyle.Image;
            refresh.Image = Resources.Refresh_16x;
            refresh.ImageTransparentColor = Color.Magenta;
            refresh.Click += new System.EventHandler(BreakAllHandler);

            s1 = new ToolStripSeparator();

            stepin = new ToolStripButton();
            stepin.DisplayStyle = ToolStripItemDisplayStyle.Image;
            stepin.Image = Resources.StepIn_16x;
            stepin.ImageTransparentColor = Color.Magenta;
            stepin.Click += new System.EventHandler(BreakAllHandler);

            stepover = new ToolStripButton();
            stepover.DisplayStyle = ToolStripItemDisplayStyle.Image;
            stepover.Image = Resources.StepOver_16x;
            stepover.ImageTransparentColor = Color.Magenta;
            stepover.Click += new System.EventHandler(BreakAllHandler);

            stepout = new ToolStripButton();
            stepout.DisplayStyle = ToolStripItemDisplayStyle.Image;
            stepout.Image = Resources.StepOut_16x;
            stepout.ImageTransparentColor = Color.Magenta;
            stepout.Click += new System.EventHandler(BreakAllHandler);

            s2 = new ToolStripSeparator();

            thread = new ToolStripButton();
            thread.DisplayStyle = ToolStripItemDisplayStyle.Image;
            thread.Image = Resources.Thread_256x;
            thread.ImageTransparentColor = Color.Magenta;
            thread.Click += new System.EventHandler(BreakAllHandler);

            dd = new ToolStripDropDownButton();
            //dd.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //dd.Image = Resources.Thread_256x;
            dd.ImageTransparentColor = Color.Magenta;
            dd.Click += new System.EventHandler(BreakAllHandler);
           
           
            ToolStripDropDownItem b = dd.DropDownItems.Add("Add or Remove Items") as ToolStripDropDownItem;
            ToolStripMenuItem v = b.DropDownItems.Add("       Debugger 1") as ToolStripMenuItem;
            b.DropDownItems.Add("       Debugger 2");
            v.Checked = true;
            v.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            v.Image = Resources.StepOver_16x;
           


            t.Items.Add(breakall);
            t.Items.Add(stop);
            t.Items.Add(restart);
            t.Items.Add(s0);
            t.Items.Add(refresh);
            t.Items.Add(s1);
            t.Items.Add(stepin);
            t.Items.Add(stepover);
            t.Items.Add(stepout);
            t.Items.Add(s2);
            t.Items.Add(thread);
            t.Items.Add(dd);

            ProfessionalColorTableExtended pc = new ProfessionalColorTableExtended();
           
            t.Renderer = new Renderer(pc);

            return t;
        }


        public class ProfessionalColorTableExtended : ProfessionalColorTable {

            public override Color CheckBackground { get { return Color.DimGray; } }

            public override Color MenuItemBorder { get { return Color.DimGray; } }

        }


        public class Renderer : ToolStripProfessionalRenderer
        {

            ProfessionalColorTable c { get; set; }

            public Renderer(ProfessionalColorTable c): base(c)
            {
                this.c = c;
                brush = new SolidBrush(Color.FromArgb(150, c.CheckSelectedBackground));
            }

            Brush brush { get; set; }



            Image image { get; set; }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                base.OnRenderMenuItemBackground(e);
               
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
                //if (!e.Item.Selected) base.OnRenderItemCheck(e);
                //else
                {


                    e.Graphics.DrawImage(Resources.CheckBox_256x, 3, 1, 18, 18);

                    if (e.Item.Selected == true)
                        e.Graphics.FillRectangle(brush, 3, 1, 18, 18);
                
                }
                
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {

                base.OnRenderItemText(e);
                
                
            }

            protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
            {

                if (e.Item.Text.StartsWith("     ") == false)
                    base.OnRenderItemImage(e);
                else 
                

                    e.Graphics.DrawImage(e.Image, 21 + 5, 3, 17, 17);
            }

        }
        public void BreakAllHandler(object sender, EventArgs e)
        {

        }

        MainForm mf { get; set; }
        public void RestartHandler(object sender, EventArgs e)
        {
            mf = new MainForm(new string[0]);
            mf.Show();

        }
    }
}
