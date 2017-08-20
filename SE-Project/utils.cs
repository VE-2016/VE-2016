using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using VSProvider;

namespace SE_Project
{
    public class Navigator
    {
        public TreeView v { get; set; }

        public ArrayList N { get; set; }

        public int act = -1;

        public int maxsize = 10;

        public Navigator(TreeView w)
        {
            N = new ArrayList();
            v = w;
        }

        public void Add(TreeNode node)
        {
            N.Add(node);
            act++;
            if (act > maxsize)
            {
                N.RemoveAt(0);
                act--;
            }
        }

        public void Prev()
        {
            if (v == null)
                return;
            if (N == null)
                return;
            if (act <= 0)
                return;

            act--;

            TreeNode node = N[act] as TreeNode;

            v.SelectedNode = node;

            node.EnsureVisible();
        }

        public void Next()
        {
            if (v == null)
                return;
            if (N == null)
                return;
            //if (act <= 0)
            //    return;

            act++;

            if (act >= N.Count)
                act = N.Count - 1;

            if (act < 0)
                act = 0;

            if (N.Count > 0)
            {
                TreeNode node = N[act] as TreeNode;

                v.SelectedNode = node;

                node.EnsureVisible();
            }
        }
    }
    public class TreeViewEx : TreeView
    {

        public ContextMenuStrip context1 { get; set; }

        public ContextMenuStrip context2 { get; set; }


        public TreeViewEx() : base()
        {
            Init();
        }
        ImageList g = new ImageList();
        void Init()
        {

            g.Images.Add("forms", new Bitmap(20, 20));/*Resources.WindowsForm_256x, new Size(25, 25)))*/;
            this.ImageList = g;

            arrowpen = new Pen(Color.Black, 1);
            arrowfill = Brushes.Black;
        }

        bool IsBitSet(Int32 b, byte nPos)
        {
            return new BitArray(new[] { b })[(int)Math.Log(nPos, 2)];
        }

        int ColumnWidth = 300;

        Pen arrowpen { get; set; }

        Brush arrowfill { get; set; }

        public virtual void OnDrawTreeNode(object sender, DrawTreeNodeEventArgs e)
        {
            string text = e.Node.Text;
            Rectangle itemRect = e.Bounds;
            if (e.Bounds.Height < 1 || e.Bounds.Width < 1)
                return;
            int cIndentBy = 19;
            int cMargin = 6;
            int cTwoMargins = cMargin * 2;
            int indent = (e.Node.Level * cIndentBy) + cMargin;
            int iconLeft = indent;
            int textLeft = iconLeft + 16;
            Color leftColour = e.Node.BackColor;
            Color textColour = e.Node.ForeColor;
            if (IsBitSet((int)e.State, (int)TreeNodeStates.Grayed))
                textColour = Color.FromArgb(255, 128, 128, 128);
            Brush backBrush = new SolidBrush(leftColour);
            if (e.State != TreeNodeStates.Focused)
                e.Graphics.FillRectangle(backBrush, itemRect);
            Color separatorColor = Color.Gray;
            Pen separatorPen = new Pen(separatorColor);
            Color cs = Color.FromArgb(255, 51, 153, 255);
            if (!HideSelection)
            {
                if (/*IsBitSet((int)e.State, (int)TreeNodeStates.Selected) || */IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                {
                    Rectangle selRect = new Rectangle(textLeft, itemRect.Top, itemRect.Right - textLeft, itemRect.Height);
                    VisualStyleRenderer renderer = new VisualStyleRenderer((ContainsFocus) ? VisualStyleElement.Button.PushButton.Hot
                                                                                           : VisualStyleElement.Button.PushButton.Normal);
                    //renderer.DrawBackground(e.Graphics, e.Bounds);
                    Brush bodge = new SolidBrush(Color.FromArgb((IsBitSet((int)e.State, (int)TreeNodeStates.Hot)) ? 255 : 128, 255, 255, 255));
                    if (IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                        bodge = new SolidBrush(cs);
                    e.Graphics.FillRectangle(bodge, e.Bounds);
                }
            }
            Pen dotPen = new Pen(Color.FromArgb(128, 128, 128));
            dotPen.DashStyle = DashStyle.Dot;
            int midY = (itemRect.Top + itemRect.Bottom) / 2;
            if (ShowLines)
            {
                int x = cMargin * 2;
                if (e.Node.Level == 0 && e.Node.PrevNode == null)
                {
                    e.Graphics.DrawLine(dotPen, x, midY, x, itemRect.Bottom);
                }
                else
                {
                    TreeNode testNode = e.Node;
                    for (int iLine = e.Node.Level; iLine >= 0; iLine--)
                    {
                        if (testNode.NextNode != null)
                        {
                            x = (iLine * cIndentBy) + (cMargin * 2);
                            e.Graphics.DrawLine(dotPen, x, itemRect.Top, x, itemRect.Bottom);
                        }
                        testNode = testNode.Parent;
                    }
                    x = (e.Node.Level * cIndentBy) + cTwoMargins;
                    e.Graphics.DrawLine(dotPen, x, itemRect.Top, x, midY);
                }
                e.Graphics.DrawLine(dotPen, iconLeft + cMargin, midY, iconLeft + cMargin + 10, midY);
            }
            if (ShowPlusMinus && e.Node.Nodes.Count > 0)
            {
                Rectangle expandRect = new Rectangle(iconLeft - 1, midY - 7, 16, 16);
                Point pe = expandRect.Location;

                Point a = new Point(new Size(pe));
                a.Offset(3, 3);
                Point b = new Point(new Size(pe));
                b.Offset(3, 11);
                Point c = new Point(new Size(pe));
                c.Offset(7, 7);
                Point d = new Point(new Size(pe));
                d.Offset(1, 9);
                Point f = new Point(new Size(pe));
                f.Offset(7, 3);
                Point g = new Point(new Size(pe));
                g.Offset(7, 9);
                if (e.Node.IsExpanded)
                {
                    List<Point> s = new List<Point>();
                    s.Add(d);
                    s.Add(f);
                    s.Add(g);
                    s.Add(d);
                    e.Graphics.FillPolygon(arrowfill, s.ToArray());
                }
                else
                {
                    List<Point> s = new List<Point>();
                    s.Add(a);
                    s.Add(b);
                    s.Add(c);
                    s.Add(a);
                    e.Graphics.DrawLines(arrowpen, s.ToArray());
                }

            }
            Point textStartPos = new Point(itemRect.Left + textLeft, itemRect.Top);
            Point textPos = new Point(textStartPos.X, textStartPos.Y);

            Font textFont = e.Node.NodeFont;
            if (textFont == null)
                textFont = Font;

            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Near;
            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.FormatFlags = StringFormatFlags.NoWrap;

            Point p = new Point(e.Bounds.Location.X, e.Bounds.Location.Y);
            p.Offset(iconLeft + 1 + 16/* + e.Node.Level * 30*/, 0);
            if (e.Node.ImageKey == "" || !this.ImageList.Images.ContainsKey(e.Node.ImageKey))
                e.Graphics.DrawImage(this.g.Images[0], p);
            else
                e.Graphics.DrawImage(this.ImageList.Images[e.Node.ImageKey], p);
            p.X += 20;

            string[] columnTextList = text.Split('|');

            for (int iCol = 0; iCol < columnTextList.GetLength(0); iCol++)
            {
                Rectangle textRect = new Rectangle(textPos.X, textPos.Y, itemRect.Right - textPos.X, itemRect.Bottom - textPos.Y);
                if (IsBitSet((int)e.State, (int)TreeNodeStates.Focused))
                    TextRenderer.DrawText(e.Graphics, text, textFont, p, Color.White);
                else
                    TextRenderer.DrawText(e.Graphics, text, textFont, p, SystemColors.ControlDarkDark);
                textPos.X += ColumnWidth;
            }

            // Draw Focussing box around the text
            if (e.State == TreeNodeStates.Focused)
            {
                SizeF size = e.Graphics.MeasureString(text, textFont);
                size.Width = (ClientRectangle.Width - 2) - textStartPos.X;
                size.Height += 1;
                Rectangle rect = new Rectangle(textStartPos, size.ToSize());
                e.Graphics.DrawRectangle(dotPen, rect);
            }
        }

    }

    public class SELoader
    {

        public ImageList img { get; set; }

        public void Load(string filename)
        {



        }
        string slnpath { get; set; }

        public msbuilder_alls ms { get; set; }
        public TreeView LoadProject(string sln)
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            string file = sln;

            img = CreateImageList(s);

            string b = AppDomain.CurrentDomain.BaseDirectory;

            s = sln;

            TreeView msv = new TreeView();

            msv.Nodes.Clear();

            msv.ImageList = img;


            ms = new msbuilder_alls();

            string p = Path.GetDirectoryName(file);

            Directory.SetCurrentDirectory(p);

            string g = Path.GetFileName(file);

            loads_vs_dict();

            ms.dc = dc;

           

            VSParsers.CSParsers.df = new Dictionary<string, ICSharpCode.NRefactory.TypeSystem.IUnresolvedFile>();

            VSSolution vs = ms.tester(msv, g, "");

            slnpath = s;

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);


            foreach (ProjectItemInfo prs in ms.PL)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = prs.ps.Name;

                nodes.Tag = prs;
            }

            foreach (VSProject vp in vs.Projects)
            {
                vp.vs = vs;
            }

            return msv;
        }
        static public ImageList CreateImageList(string s)
        {
            Dictionary<string, object> dict;

            Dictionary<string, object> dict2;

            Dictionary<string, object> dict3;

            ImageList imageList1 = new ImageList();

            dict = null;
            if (dict == null)
            {
                dict = Load(s + "\\" + "SE-Project.exe", "SE_Project.resources-vs.resources");

                foreach (string str in dict.Keys)
                {
                    if (dict[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict[str], str, imageList1);
                }
            }

            dict2 = null;
            if (dict2 == null)
            {
                dict2 = Load(s + "\\" + "SE-Project.exe", "SE_Project.resource-vsc.resources");

                foreach (string str in dict2.Keys)
                {
                    if (dict2[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict2[str], str, imageList1);
                }
            }
            dict3 = null;
            if (dict3 == null)
            {
                dict3 = Load(s + "\\" + "SE-Project.exe", "SE_Project.Resources_refs.resources");

                foreach (string str in dict3.Keys)
                {
                    if (dict3[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict3[str], str, imageList1);
                }

                //addImage(resource_alls.Folder, "Folder", imageList1);
                //addImage(resource_alls.FolderOpen, "FolderOpen", imageList1);
                //addImage(Resources.DataSourceView_16x, "DataSourceView_16x", imageList1);

        
                //imageList1.Images.Add("Class", Resources._class);
                //imageList1.Images.Add("ImageClass", Resources.Class_yellow_256x);
                //imageList1.Images.Add("ClassImage", Resources.Class_yellow_256x);
            }

            return imageList1;
        }
        static public ImageList addImage(Bitmap bmp, string name, ImageList imgL)
        {
            if (imgL == null)
                imgL = new ImageList();

            if (bmp != null)
            {
                imgL.Images.Add(name, (Image)bmp);
                //listView1.BeginUpdate();
                //listView1.Items.Add(imageToLoad, baseValue++);
                //listView1.EndUpdate();
            }

            return imgL;
        }
        public Dictionary<string, string> dc { get; set; }

        public void loads_vs_dict()
        {
            if (dc == null)
                dc = new Dictionary<string, string>();
            else
                return;

            Dictionary<string, string> d = dc;
            d.Add(".xsd", "DataSourceView_16x");
            d.Add(".xsc", "filesource");
            d.Add(".xss", "filesource");
            d.Add(".vcproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".vcxproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".csproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".shfbproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wxs", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wixproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".cpp", "CPPFile_SolutionExplorerNode_24");
            d.Add(".h", "Include_13490_24");
            d.Add(".txt", "resource_32xMD");
            d.Add(".md", "resource_32xMD");
            d.Add(".rtf", "resource_32xMD");
            d.Add(".targets", "resource_32xMD");
            d.Add(".dll", "Library_6213");
            d.Add(".xml", "XMLFile_828_16x");
            d.Add(".xaml", "XMLFile_828_16x");
            d.Add(".json", "resourcefile");
            d.Add(".cur", "cursor");
            d.Add(".shproj", "shared");
            d.Add(".cd", "classgray");
            d.Add(".resx", "filesource");
            d.Add(".bmp", "images");
            d.Add(".png", "images");
            d.Add(".jpg", "images");
            d.Add(".gif", "images");
            d.Add(".ico", "icon");
            d.Add(".wav", "audio");
            d.Add(".csx", "script");
            d.Add(".sh", "script");
            d.Add(".bat", "script");
            d.Add(".config", "config");
            d.Add(".vsixmanifest", "config");
            d.Add(".ruleset", "rule");
            d.Add(".pfx", "certificate");
            d.Add(".datasource", "datasource");
            d.Add(".cs", "CSharpFile_SolutionExplorerNode");
            d.Add(".vb", "vb");


        }

        public static Dictionary<string, object> Load(string ase, string res)
        {
            Dictionary<string, object> dicAttributes = new Dictionary<string, object>();

            ArrayList L = new ArrayList();

            ArrayList V = new ArrayList();

            Assembly assem = Assembly.LoadFile(ase);

            string[] rs = assem.GetManifestResourceNames();

            Stream r = assem.GetManifestResourceStream(res/*"SE-Project.resources-vs.resources"*/);

            System.Resources.ResourceReader mg = new System.Resources.ResourceReader(r);

            string tempFile = "";

            foreach (var item in mg)
            {
                dicAttributes.Add(((System.Collections.DictionaryEntry)(item)).Key.ToString(), ((System.Collections.DictionaryEntry)(item)).Value);

                L.Add(((System.Collections.DictionaryEntry)(item)).Key.ToString());

                V.Add(((System.Collections.DictionaryEntry)(item)).Value);
            }

            return dicAttributes;
        }

    }
  
}

