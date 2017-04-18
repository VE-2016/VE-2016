using System.Collections;
using System.Windows.Forms;

namespace WinExplorer.Services.NuGet
{
    public class FlowPanel : Panel
    {
        public FlowPanel() : base()
        {
            C = new ArrayList();
            this.HScroll = false;
            this.HorizontalScroll.Enabled = false;
        }

        public ArrayList C { get; set; }

        public void AddControl(Control c)
        {
            int w = this.ClientRectangle.Width - 10;
            int h = 0;
            if (C.Count > 0)
            {
                Control cc = C[C.Count - 1] as Control;

                h = cc.Location.Y + cc.Height + 1;
            }
            c.Location = new System.Drawing.Point(0, h);
            c.Size = new System.Drawing.Size(w, 80);
            this.Controls.Add(c);
            C.Add(c);
            this.HorizontalScroll.Visible = false;
        }

        public void ClearControls()
        {
            C = new ArrayList();
            this.Controls.Clear();
        }
    }
}