using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using System.Runtime.InteropServices;
using GACProject;
using System.Xml;

namespace AIMS.Libraries.CodeEditor.Editors
{
    public partial class DefinitionForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr s_HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;
        public DefinitionForm()
        {
            InitializeComponent();
            this.Paint += DefinitionForm_Paint;
            rb = richTextBox1;
            rb.Width = 500;
            rb.ReadOnly = true;
            rb.WordWrap = false;
            rb.ContentsResized += Rb_ContentsResized;
            rb.Enter += Rb_Enter;
            rb.GotFocus += Rb_GotFocus;
            LoadNavigation();
        }

        private void DefinitionForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.LightGray, e.ClipRectangle);
        }

        private void Rb_GotFocus(object sender, EventArgs e)
        {
            //if (control != null)
            //    control.Focus();
            //else
            panel1.Focus();

            

        }

        public Control control { get; set; }
        private void Rb_Enter(object sender, EventArgs e)
        {
            if (control != null)
                control.Focus();
            else panel1.Focus();
        }

        PictureBox pbdown { get; set; }
        PictureBox pbup { get; set; }

        Label label { get; set; }

        void LoadNavigation()
        {
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.Image = Properties.Resources.Triangle_16x;
            pbup = pictureBox2;
            pbup.Click += Pbup_Click;

            Bitmap bmp = new Bitmap(Properties.Resources.Triangle_16x);
            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = bmp;
            pbdown = pictureBox1;
            pbdown.Click += Pbdown_Click;

            label = label1;

        }

        private void Pbup_Click(object sender, EventArgs e)
        {
            
            if (sn == null)
                return;
            if (sym_index <= 0)
                return;
            sym_index--;
            label.Text = (1 + sym_index).ToString() + " " + "of" + " " + sn.Count.ToString();
            LoadSymbols(sym_index);
            if (control != null)
                control.Focus();
        }
        public bool Pbup_Click()
        {

            if (sn == null)
                return false;
            if (sym_index <= 0)
                return false;
            sym_index--;
            label.Text = (1 + sym_index).ToString() + " " + "of" + " " + sn.Count.ToString();
            LoadSymbols(sym_index);
            if (control != null)
                control.Focus();
            return true;
        }
        public void HidePanel()
        {
            this.Controls.Remove(panel1);
            this.rb.Location = new Point(5, 5);
        }
        public void ShowPanel()
        {
            
            this.Controls.Add(panel1);
            panel1.Location = new Point(0, 5);
            this.rb.Location = new Point(panel1.Width, 5);
        }

        int sym_index = 0;

        private void Pbdown_Click(object sender, EventArgs e)
        {
            if (sn == null)
                return;
            if (sym_index >= sn.Count - 1)
                return;
            sym_index++;
            label.Text = (1 + sym_index).ToString() + " " + "of" + " " + sn.Count.ToString();
            LoadSymbols(sym_index);
            if (control != null)
                control.Focus();
        }
        private bool Pbdown_Click()
        {
            if (sn == null)
                return false;
            if (sym_index >= sn.Count - 1)
                return false;
            sym_index++;
            label.Text = (1 + sym_index).ToString() + " " + "of" + " " + sn.Count.ToString();
            LoadSymbols(sym_index);
            if (control != null)
                control.Focus();
            return true;
        }
        private void Rb_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            var richTextBox = (RichTextBox)sender;
            int w = e.NewRectangle.Width;

            if (w < 500)
                w = 500;
            richTextBox.Width = w + 15;
            richTextBox.Height = e.NewRectangle.Height + 5;
            this.Width = e.NewRectangle.Width + panel1.Width + 10 + 1;
            this.Height = e.NewRectangle.Height + 5 + 1;
        }

        RichTextBox rb { get; set; }

        List<ISymbol> sn { get; set; }
        public void LoadSymbols(List<ISymbol> sn)
        {
            this.sn = sn;

            if (sn == null)
                return;

            if (sn.Count <= 1)
                HidePanel();
            else ShowPanel();

            sym_index = 0;

            label.Text = (1 + sym_index).ToString() + " " + "of" + " " + sn.Count.ToString();

            //int start = 0;
            
            LoadSymbols(0);

            //foreach (ISymbol s in sn)
            //{

            //    if (s is INamedTypeSymbol)
            //    {
            //        rb.AppendText("\nNamed " + s.Name + "\n");
            //    }
            //    else if (s is IMethodSymbol)
            //    {
                     
            //        IMethodSymbol method = s as IMethodSymbol;
            //        if (method.ReturnsVoid)
            //        {
            //            rb.AppendText("void" + " " + s.Name + "(");
            //            SetAsBlue("void", start);
            //        }
            //        else
            //        {
            //            rb.AppendText( method.ReturnType.Name + " " + method.ContainingType.Name + "." + s.Name + "(");
            //            SetAsBlue(method.ContainingType.Name, start);
            //            SetAsBlue(method.ReturnType.Name, start);
            //        }
            //        int i = 0;
            //        foreach (IParameterSymbol p in method.Parameters)
            //        {
            //            if (i > 0)
            //                rb.AppendText(", ");

            //            if (p.IsParams)
            //            {
            //                rb.AppendText("params" + " " + p.Type.Name + " " + p.Name);
            //                SetAsBlue("params", start);
            //                SetAsBlueBold(p.Name, start);
            //            }
            //            else
            //            {
            //                rb.AppendText(p.Type.Name + " " + p.Name);
            //                SetAsBlueBold(p.Type.Name, start);
            //                SetAsBold(p.Name, start);
            //            }
            //                i++;
            //        }
            //        rb.AppendText(")");
                    
            //       XmlElement xml = GACForm.GetFrameworkXmlDoc(method.ContainingAssembly.Identity.Name, method.ContainingNamespace + "." + method.ContainingType.Name + "." + method.Name,"M:" );

            //        if (xml == null)
            //            continue;

            //        string doc = "\r";

            //        if (xml["summary"] != null)
            //        {
            //            doc += xml["summary"].InnerText.Replace("\n","");
            //        }
            //        foreach (XmlNode node in xml.SelectNodes("param"))
            //        {
            //            doc += "\n" + node.Attributes["name"].InnerText + " : " + node.InnerText.Replace("\n", "");
            //            SetAsBold(node.Attributes["name"].InnerText, start);
            //        }
            //        if (xml["returns"] != null)
            //        {
            //            doc += xml["returns"].InnerText.Replace("\n","");
            //        }
            //        rb.AppendText(doc);
            //    }
            //    else
            //    {
            //        rb.AppendText("\n" + s.Name + "\n");
            //    }
            //    rb.AppendText("\r");
            //    start = rb.Text.Length;
            //}
        }

        public bool Dfs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Up)
            {
                return Pbup_Click();
            }
            else if (e.KeyData == Keys.Down)
            {
                return Pbdown_Click();
            }
            return true;
        }

        public void LoadSymbols(int sym_index)
        {
            if (sn == null)
                return;
            if (sym_index >= sn.Count)
                return;
            
            rb.Text = "";

            int start = 0;
            ISymbol s = sn[sym_index];
            {

                if (s is INamedTypeSymbol)
                {
                    rb.AppendText("\nNamed " + s.Name + "\n");
                }
                else if (s is IMethodSymbol)
                {

                    IMethodSymbol method = s as IMethodSymbol;
                    if (method.ReturnsVoid)
                    {
                        rb.AppendText("void" + " " + s.Name + "(");
                        SetAsBlue("void", start);
                    }
                    else
                    {
                        rb.AppendText(method.ReturnType.Name + " " + method.ContainingType.Name + "." + s.Name + "(");
                        SetAsBlue(method.ContainingType.Name, start);
                        SetAsBlue(method.ReturnType.Name, start);
                    }
                    int i = 0;
                    foreach (IParameterSymbol p in method.Parameters)
                    {
                        if (i > 0)
                            rb.AppendText(", ");

                        if (p.IsParams)
                        {
                            rb.AppendText("params" + " " + p.Type.Name + " " + p.Name);
                            SetAsBlue("params", start);
                            SetAsBlueBold(p.Name, start);
                        }
                        else
                        {
                            rb.AppendText(p.Type.Name + " " + p.Name);
                            SetAsBlueBold(p.Type.Name, start);
                            SetAsBold(p.Name, start);
                        }
                        i++;
                    }
                    rb.AppendText(")");

                    start = rb.Text.Length + 1;

                    XmlElement xml = GACForm.GetFrameworkXmlDoc(method.ContainingAssembly.Identity.Name, method.ContainingNamespace + "." + method.ContainingType.Name + "." + method.Name, "M:");

                    if (xml != null)
                    {
                        string doc = "\r";

                        if (xml["summary"] != null)
                        {
                            doc += xml["summary"].InnerText.Replace("\n", "");
                        }
                        rb.AppendText(doc);

                        foreach (XmlNode node in xml.SelectNodes("param"))
                        {
                            doc = "";
                            doc += "\r" + node.Attributes["name"].InnerText + ": " + node.InnerText.Replace("\n", "");
                            rb.AppendText(doc);
                            SetAsBold(node.Attributes["name"].InnerText, start);
                        }
                        if (xml["returns"] != null)
                        {
                            doc = "";
                            doc += xml["returns"].InnerText.Replace("\n", "");
                            rb.AppendText(doc);
                        }
                    }
                }
                else
                {
                    rb.AppendText("\n" + s.Name + "\n");
                }
                rb.AppendText("\r");
                start = rb.Text.Length;
            }
        }
        public void SetTopMost()
        {
            
            SetWindowPos(this.Handle, s_HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        Font BF = new Font("Verdana", 8.25f, FontStyle.Bold);
        Font RF = new Font("Verdana", 8.25f, FontStyle.Regular);
        

        public void SetAsBold(string s, int start)
        {
            rb.Find(s, start, RichTextBoxFinds.MatchCase);
            //foreach (char r in rb.SelectedText)
            //{
                rb.SelectionFont = BF;
               rb.SelectionColor = Color.Black;
            //}
                
            //rb.Refresh();
            rb.SelectionProtected = true;
            rb.SelectionLength = 0;
            
          
        }
        public void SetAsBlue(string s, int start)
        {
            rb.Find(s, start, RichTextBoxFinds.MatchCase);
            rb.SelectionFont = RF;
            rb.SelectionColor = Color.LightBlue;
            rb.SelectionProtected = true;
            rb.SelectionLength = 0;


        }
        public void SetAsBlueBold(string s, int start)
        {
            rb.Find(s, start, RichTextBoxFinds.MatchCase);
            rb.SelectionFont = BF;
            rb.SelectionColor = Color.Blue;
            rb.SelectionProtected = true;

            rb.SelectionLength = 0;

        }
    }
}
