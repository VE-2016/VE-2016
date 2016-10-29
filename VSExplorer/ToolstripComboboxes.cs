using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer
{
    public class ToolStripComboBoxes : ToolStripComboBox
    {
        public ToolStripComboBoxes(ComboBoxEx c)
        {
            this.ComboBox.DrawItem += new DrawItemEventHandler(DrawItems);
            this.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public ToolStripComboBoxes()
        {
            this.ComboBox.DrawItem += new DrawItemEventHandler(DrawItems);
            this.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public ComboBoxEx ce { get; set; }

        public Color HighlightColor = Color.White;

        public void SetDrawMode(bool ownerdraw)
        {
            // if (ownerdraw == true)
            this.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            // else
            //     this.ComboBox.DrawMode = DrawMode.Normal;
        }

        //    protected override void OnPaint(PaintEventArgs e)
        //    {
        //        Rectangle rect = new Rectangle(this.ComboBox.Location.X - 1, this.ComboBox.Location.Y - 1, this.ComboBox.Width + 1, this.ComboBox.Height + 1);

        //        Pen pen = new Pen(Color.Orange, 4);
        //        Graphics g = e.Graphics;

        //        g.DrawRectangle(pen, rect);
        //        this.ComboBox.Refresh();

        //}

        private static Color s_specialyellow = Color.FromArgb(0xFF, 255, 242, 157);

        static public void DrawItems(object sender, DrawItemEventArgs e)
        {
            try
            {
                if (e.Index < 0)
                    return;

                // DrawItemEventArgs s = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State, Color.Green, Color.Gray);

                //e = s;

                var combo = sender as ComboBox;

                e.Graphics.FillRectangle(new SolidBrush(SystemColors.ControlLight), e.Bounds);

                if ((e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit)
                {
                    Rectangle r = e.Bounds;
                    r.Width += 10;
                    r.X = -3;
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), r);
                    //e.Graphics.DrawRectangle(Pens.Orange, e.Bounds);
                }
                else

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected/* && combo.DroppedDown == false*/)
                {
                    e.Graphics.FillRectangle(new SolidBrush(s_specialyellow), e.Bounds);
                    //e.Graphics.DrawRectangle(Pens.Orange, e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.ControlLight), e.Bounds);
                }

                e.Graphics.DrawString(combo.Items[e.Index].ToString(),
                                              e.Font,
                                              new SolidBrush(Color.Black),
                                              new Point(e.Bounds.X, e.Bounds.Y));

                // e.DrawFocusRectangle();
            }
            catch (Exception ee) { }
        }
    }

    public class ComboBoxEx : ComboBox
    {
        public string name()
        {
            return "ComboBoxEx";
        }
    }

    public class ToolStripControlHost2 : ToolStripControlHost
    {
        public ToolStripControlHost2(ComboBoxEx c) : base(c)
        {
            ce = c;
        }

        public ComboBoxEx ce
        {
            get; set;
        }

        public class ToolStripComboBox2 : ToolStripControlHost2
        {
            public ToolStripComboBox2(ComboBoxEx c) : base(c)
            {
                ce = new ComboBoxEx();
            }
        }
    }
}