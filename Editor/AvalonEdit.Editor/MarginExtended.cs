using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Utils;
using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Differencing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using VSParsers;
using VSProvider;

namespace AvalonEdit.Editor
{
	public partial class MarginExtended : UserControl
	{


	}
	public class BreakPointMargin : AbstractMargin
	{
		public BreakPointMargin()
		{
			//Margin = new Thickness(30, 0, 0, 0);
			background = System.Windows.Media.Brushes.White;
		}
		public int margin = 20;
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
		{
			return new System.Windows.Size(margin, 0);
		}
		//Size RenderSize = new Size(70, 1000);

		public SolidColorBrush background { get; set; }

		public System.Windows.Media.Pen pen { get; set; }

		protected override void OnRender(DrawingContext drawingContext)
		{
			System.Windows.Size renderSize = new System.Windows.Size(margin, ((FrameworkElement)this).RenderSize.Height);
			drawingContext.DrawRectangle(background, null,
										 new Rect(0, 0, renderSize.Width, renderSize.Height));
			if (pen != null)
				drawingContext.DrawLine(pen, new System.Windows.Point(renderSize.Width - 5, 0), new System.Windows.Point(renderSize.Width - 5, renderSize.Height));

			base.OnRender(drawingContext);
		}
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{

        //}

        //public Line line {get; set;}

        //public void LoadLine(Line ls)
        //{
        //	line = ls;
        //	line.Margin = new Thickness(45, 0, 0, 0);
        //	this.AddVisualChild(line);
        //}
    }
	public class Editor
	{

        //Stack<int> stack = null;
        //void Parse(string c, int offset)
        //{
        //    stack = new Stack<int>();
        //    List<int> sc = new List<int>();
        //    List<int> ec = new List<int>();
        //    List<string> nc = new List<string>();
        //    List<string> ns = new List<string>();

        //    int p = 0;
        //    do
        //    {
        //        p = c.IndexOf("(", p);
        //        if (p >= 0)
        //        {
        //            sc.Add(p);
        //            int i = p - 1;
        //            string name = "";
        //            while (i >= 0 && i < c.Length && c[i] != ')' && c[i] != '(' && !Char.IsWhiteSpace(c[i]))
        //            {
        //                name = name + c[i];
        //                i--;
        //            }
        //            nc.Add(string.Concat(Enumerable.Reverse(name)));
        //        }
        //        if (p < 0)
        //            break;
        //        p++;
        //    }
        //    while (p >= 0 && p <= offset);
        //    p = 0;
        //    do
        //    {

        //        p = c.IndexOf(")", p);
        //        if (p >= 0)
        //        {
        //            ec.Add(p);
        //        }
        //        if (p < 0)
        //            break;
        //        p++;
        //    }
        //    while (p >= 0 && p <= offset);
        //    p = 0;
        //    int r = 0;
        //    int level = 0;
        //    int current = 0;
        //    while ((p >= 0 || r >= 0) && p <= offset)
        //    {
        //        //        p = c.IndexOf("(", p);
        //        //        r = c.IndexOf(")", r);
        //        //        if (p >= 0 || r>= 0)
        //        {
        //            //          p++;
        //            r = p;
        //            //            level++;
        //            //while(p < offset && p < c.Length - 1)
        //            {
        //                p = c.IndexOf("(", p);
        //                r = c.IndexOf(")", r);
        //                if (p > 0)
        //                {
        //                    //                if (p < r)

        //                    if (p > r && r > 0)
        //                    {
        //                        Stack.Pop();
        //                        //  level = stack.Peek();
        //                        p = r;
        //                    }
        //                    else
        //                    {
        //                        //level++;
        //                        current++;
        //                        stack.Push(current);
        //                    }
        //                    p++;
        //                    r = p;
        //                }
        //                else if (r > 0)
        //                {
        //                    r++;
        //                    p = r;
        //                    stack.Pop();

        //                    //level = stack.Peek();
        //                }
        //                else break;
        //            }
        //        }



        //    }

        //}
        
        public List<Microsoft.CodeAnalysis.ISymbol> methods { get; set; }

        public VSSolution vs { get; set; }

        public VSProject vp { get; set; }

        public static string GetWordUnderMouse(ICSharpCode.AvalonEdit.Document.TextDocument document, TextViewPosition position)
        {
            string wordHovered = string.Empty;

            var line = position.Line;
            var column = position.Column - 1;

            var offset = document.GetOffset(line, column);
            if (offset >= document.TextLength)
                offset--;

            var textAtOffset = document.GetText(offset, 1);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(textAtOffset))
            {
                wordHovered = textAtOffset + wordHovered;

                offset--;

                if (offset < 0)
                    break;

                textAtOffset = document.GetText(offset, 1);

                if (textAtOffset == "(" || textAtOffset == ")" || textAtOffset == "<" || textAtOffset == ">" || textAtOffset == "," || textAtOffset == ";")
                    break;
            }

            // Get text forward the mouse position, until the first space
            offset = document.GetOffset(line, column);
            if (offset < document.TextLength - 1)
            {
                offset++;

                textAtOffset = document.GetText(offset, 1);

                while (!string.IsNullOrWhiteSpace(textAtOffset))
                {
                    wordHovered = wordHovered + textAtOffset;

                    offset++;

                    if (offset >= document.TextLength)
                        break;

                    textAtOffset = document.GetText(offset, 1);

                    if (textAtOffset == "(" || textAtOffset == ")" || textAtOffset == "<" || textAtOffset == ">" || textAtOffset == "," || textAtOffset == ";")
                        break;
                }
            }

            return wordHovered;
        }

        public static string GetWordBeforeDot(TextEditor textEditor)
        {
            var wordBeforeDot = string.Empty;

            var caretPosition = textEditor.CaretOffset - 2;

            var lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(caretPosition));

            string text = textEditor.Document.GetText(lineOffset, 1);

            // Get text backward of the mouse position, until the first space
            while (!string.IsNullOrWhiteSpace(text) && text.CompareTo(".") > 0)
            {
                wordBeforeDot = text + wordBeforeDot;

                if (caretPosition == 0)
                    break;

                lineOffset = textEditor.Document.GetOffset(textEditor.Document.GetLocation(--caretPosition));

                text = textEditor.Document.GetText(lineOffset, 1);
            }

            return wordBeforeDot;
        }
        public List<ISymbol> symbols { get; set; }

        public string SolutionToLoad { get; set; }

        public string ProjectToLoad { get; set; }

        public string FileToLoad { get; set; }

        public void GetMembers(string text = null, bool addProperties = false)
        {
            string FileName = SolutionToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            string content = text;

            if ((!string.IsNullOrEmpty(filename)))
            {

                if (string.IsNullOrEmpty(text))
                    content = File.ReadAllText(filename);

                vp = vs.GetProjectbyCompileItem(filename);
            }

            if (vp == null)
                symbols = vs.GetMembers(text, addProperties);
            else

            symbols = vp.vs.GetMembers(vp, filename, content);
        }
        public List<INamespaceOrTypeSymbol> GetTypes()
        {
            string FileName = ProjectToLoad;// FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";
            

            vp = vs.GetProjectbyCompileItem(filename);

            if (vp == null)
                return vs.GetAllTypes("");

            return vp.vs.GetAllTypes(FileName);
        }


        public Tuple<string, string, ISymbol> GetType(TextEditor textEditor)
        {
            int position = textEditor.TextArea.Caret.Offset;

            string word = GetWordUnderMouse(textEditor.Document, textEditor.TextArea.Caret.Position);

            if (string.IsNullOrEmpty(word))
                return new Tuple<string, string, ISymbol>(null,null,null);

            string FileName = SolutionToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            string content = StringReplace.Replace(textEditor.Document.Text);

            vp = vs.GetProjectbyCompileItem(FileToLoad);

            string file = null;

            if (vp != null)
                file = vp.FileName;

            ISymbol r = vs.GetSymbolAtLocation(vp, FileToLoad, position, content);

            if (word.Contains("."))
            {

                

                if(r != null)
                    if(r.Kind == SymbolKind.Method)
                {
                        return new Tuple<string, string, ISymbol>(CSParsers.TypeToString(r.ContainingType), "", r);
                }

            }
            else
            {
                if (r != null)
                    if (r.Kind == SymbolKind.Method)
                    {
                        ISymbol containigType = r.ContainingType;

                        if (containigType != null)
                        {

                            if (containigType.Locations != null)
                                if (containigType.Locations[0] != null)
                                    if (containigType.Locations[0].IsInSource)
                                    {
                                        return new Tuple<string, string, ISymbol>("", containigType.Locations[0].SourceTree.FilePath, r);

                                    }


                        }
                    }
            }
             List<INamespaceOrTypeSymbol> s = /*vp.*/vs.GetAllTypes(file);

            word = word.ToLower();

           // var data = s.Select(bc => bc).Where(cd => cd.Name.ToLower().Contains(word)).ToList();

            foreach (INamespaceOrTypeSymbol b in s)
            {
                if (word.Contains("."))
                {

                }
                else if(b.Name.ToLower() ==  word)
                {
                    if (b.IsType)
                    {
                        if (b.Locations != null)
                            if (b.Locations[0] != null)
                                if(b.Locations[0].IsInSource )
                            {
                                    return new Tuple<string, string, ISymbol>("", b.Locations[0].SourceTree.FilePath, b);
                                
                            }
                        return new Tuple<string, string, ISymbol>(CSParsers.TypeToString(b), "", b);
                        
                    }
                    
                }
            }
            return new Tuple<string, string, ISymbol>("","", null);
        }

        public TextEditor textEditor { get; set; }

        public string WordUnderCaret { get; set; }

        public bool shouldUpdate = false;

        int RealOffset(int offset)
        {
            int realLineNumber = -1;
            int Offset = 0;
            DocumentLine current = textEditor.Document.GetLineByOffset(0);
            while (current != null)
            {


                if (!(current.obs is ISymbol))
                    realLineNumber++;
                else
                {

                    Offset += (current.Offset + current.Length + current.DelimiterLength);

                }
                

                current = current.NextLine;

                if (current.Offset >= offset)
                    return Offset;
            }
            return Offset;
        }



        public void CodeCompletionPreview()
        {

        }


        public void TypesWithDelay(int KeyChar, ICSharpCode.AvalonEdit.Document.TextDocument textDocument, TextEditor textEditor, string FileName, int offset)
        {
            //_runnings = true;

            //	this.BeginInvoke(new Action(() =>
            {
                //
                //Point p = Selection.GetCursor();
                //
                //int offset = Caret.Offset;
                //
                //Row r = Document[p.Y];
                //

                if (vp == null)
                    return;

                shouldUpdate = false;

                this.textEditor = textEditor;
                
                TextViewPosition textViewPosition = new TextViewPosition(textEditor.TextArea.Caret.Location);

                string name = GetWordUnderMouse(textDocument, textViewPosition);

                WordUnderCaret = name;

                //if (WordUnderCaret == "pp")
                //    MessageBox.Show("");

                string names = name;

                if (name == "this.")
                {
                    name = "this";
                    shouldUpdate = true;
                }
                else
                if (name.EndsWith("."))
                {
                    
                    shouldUpdate = true;
                }
                else if (name.Length == 1)
                {
                    shouldUpdate = true;
                    name = "";
                }
                
                else
                if (name.Length > 1 && !name.Contains("."))
                {
                    //sns = new List<ISymbol>();
                    return;
                }
                else name = "";

                // offset = RealOffset(offset);


                List<Microsoft.CodeAnalysis.ISymbol> sn = LoadProjectTypes(vp, FileName, textDocument.Text, offset, name, "");
                

                sns = sn;

                if(sn.Count <= 0)
                {
                    if (name.Contains("."))
                    {
                        //MessageBox.Show("Check for static class members - " + WordUnderCaret);
                    }
                }

                //ArrayList L = new ArrayList();

                foreach (var b in sn)
                {
                    //if (b.Kind == SymbolKind.Local)
                      //  if (b is INamedTypeSymbol)

                    //        if (((INamedTypeSymbol)b).TypeKind == TypeKind.Class)
                    //            sn.Remove(b);

                }

                {
                    //			if (ins != null)
                    //			{
                    //				ins.Show();
                    //				form.Show();
                    //			}

                    //var c = Selection.GetCaretWords();

                    //string cc = "";
                    //foreach (string s in c)
                    //{
                    //    cc += s;
                    //}
                    //cc = cc.Trim();
                    //if (cc.EndsWith("="))
                    //{

                    //    string[] nc = Selection.GetCaretString().Trim().Split("=".ToCharArray());
                    //    if (nc.Length > 1)
                    //    {
                    //        List<Microsoft.CodeAnalysis.ISymbol> symbols = new List<Microsoft.CodeAnalysis.ISymbol>();
                    //        string data = nc[nc.Length - 2].Trim();
                    //        Microsoft.CodeAnalysis.ISymbol symbol = ins.GetSymbol(data);
                    //        if (symbol is IParameterSymbol)
                    //        {
                    //            IParameterSymbol ps = symbol as IParameterSymbol;
                    //            MessageBox.Show("template = detected for " + symbol.Name + " of type " + ps.Type.Name);
                    //            //Selection.AppendToCaretString(" " + ps.Type.Name);
                    //            symbols.Add(symbol);
                    //        }
                    //        else if (symbol is IPropertySymbol)
                    //        {
                    //            IPropertySymbol ps = symbol as IPropertySymbol;
                    //            MessageBox.Show("template = detected for " + symbol.Name + " of type " + ps.Type.Name);
                    //            //Selection.AppendToCaretString(" " + ps.Type.Name);
                    //            symbols.Add(symbol);
                    //        }
                    //        else if (symbol is IFieldSymbol)
                    //        {
                    //            IFieldSymbol ps = symbol as IFieldSymbol;
                    //            MessageBox.Show("template = detected for " + symbol.Name + " of type " + ps.Type.Name);
                    //            //Selection.AppendToCaretString(" " + ps.Type.Name);
                    //            symbols.Add(symbol);
                    //        }
                    //        else if (symbol != null)
                    //            MessageBox.Show("template = detected for " + symbol.Name);

                    //        ins.LoadFast(symbols);

                    //        //				ins.SetVirtualMode();


                    //        //				ins.AutoSize();

                    //        //				ins.Refresh();

                    //        return;
                    //    }
                    //}
             //       string[] words = cc.Trim().Split(".".ToCharArray()).Select(s => s).Where(t => t != "").ToArray();

             //       string name = "";

             //       Parse(Selection.GetCaretString(), offset);

             //       if (cc.StartsWith(".this"))
             //           name = "this";
             //       else
             //       if (KeyChar == 8 && words.Length > 1)
             //       {
             //           //ins.Hide();
             //           //form.Hide();
             //           //if (dfs != null)
             //           //	dfs.Close();
             //           return;
             //       }
             //       else if (cc.StartsWith("."))
             //       {
             //           name = words[0];
             //           if (/*name.Contains(")") && */name.Contains("("))
             //           {
             //               string[] dd = name.Split("(".ToCharArray());

             //               //name = "." + dd[0];
             //               name = dd[dd.Length - 1];//dd[0]
             //           }
             //       }
             //       else if (cc.StartsWith("("))
             //       {
             //           name = words[0];
             //           if (name.Contains("("))
             //               if (name.Contains(")"))
             //               {
             //                   string[] dd = name.Split(",".ToCharArray());
             //                   if (Selection.IsCaretInWord())
             //                       name = "";
             //               }
             //       }
             //       else if (cc.StartsWith(")"))
             //       {
             //           name = words[0];
             //           if (name.Contains("("))
             //               if (name.Contains(")"))
             //               {
             //                   if (Selection.IsCaretInWord())
             //                       name = "";
             //               }
             //       }
             //       Point pp = Selection.GetCursor();
             //       pp.X = pp.X * ActiveViewControl.View.CharWidth;
             //       pp.Y = (ActiveViewControl.Document[p.Y].VisibleIndex - ActiveViewControl.View.FirstVisibleRow + 1) * ActiveViewControl.View.RowHeight + 3;
             //  //     form.Location = this.PointToScreen(pp);
             //       List<Microsoft.CodeAnalysis.ISymbol> sn = ins.LoadProjectTypes(vp, FileName, Document.Text, offset, name, cc);
             //       if (name.StartsWith("("))
             //       {
             //           //ins.Hide();
             //           //form.Hide();
             ////           LoadDefinition(sn, this.PointToScreen(pp));
             //           methods = sn;
             //           return;
             //       }
             //       else if (name.StartsWith(")"))
             //       {
             //           //ins.Hide();
             //           //form.Hide();
             //           //if (dfs != null)
             //           //	dfs.Hide();
             //           return;
             //       }
             //       //ins.SetVirtualMode();

           //         ins.FindSimilarWords(w);

                    //ins.AutoSize();

                    //ins.Refresh();

                }
                


                //			if (form.Visible == true)
                //				form.SetTopMost();

                //		this.Focus();

                //		_runnings = false;

            }
        }

        //DefinitionForm df { get; set; }
        //void LoadDefinition(List<Microsoft.CodeAnalysis.ISymbol> sn, Point pp)
        //{
        //    if (dfs != null)
        //        dfs.Close();
        //    df = new DefinitionForm();
        //    df.control = this;
        //    df.SetTopMost();
        //    df.LoadSymbols(sn);
        //    dfs = df;
        //    df.Show();
        //    this.Focus();
        //    df.Location = pp;
        //    df.Refresh();
        //}
        
        ArrayList S { get; set; }
        ArrayList T { get; set; }

        public List<Microsoft.CodeAnalysis.ISymbol> sns { get; set; }
        public List<Microsoft.CodeAnalysis.ISymbol> LoadProjectTypes(VSProject vp, string filename, string content, int offset, string name, string names)
        {
            List<Microsoft.CodeAnalysis.ISymbol> sn = GetCodeCompletion(vp, filename, content, offset, name, names);
            sn.Remove(sn.Find(x => x.Name == ".ctor"));

            if (name.StartsWith("("))
                return sn;
            //v.Items.Clear();


            LoadSymbols(sn);

            if (names.Contains("."))
                ReloadTypes(false);
            else ReloadTypes(true);

            sns = sn;
            
            return sn;
        }
     
        public List<Microsoft.CodeAnalysis.ISymbol> GetCodeCompletion(VSProject vp, string filename, string content, int offset, string name, string names)
        {
            return vp.vs.GetCodeCompletion(vp, filename, content, offset, name, names);
        }
        ArrayList E = new ArrayList();
        void LoadSymbols(List<Microsoft.CodeAnalysis.ISymbol> sn, bool alls = false)
        {
            
            //if (S != null)
            //    foreach (ListViewItem s in S)
            //        T.Remove(s);

            //S = new ArrayList();

            //if (T == null)
            //    T = new ArrayList();

            //if (E == null)
            //{
            //    E = new ArrayList();
            //    //E.Add(new ListViewItem("this"));
            //}

            //foreach (Microsoft.CodeAnalysis.ISymbol dd in sn)
            //{

            //    if (dd != null)
            //    {
            //        ListViewItem v;
            //        int kind = 0;

            //        if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Method)
            //            kind = 1;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Property)
            //            kind = 3;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Field)
            //            kind = 2;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Event)
            //            kind = 4;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Local)
            //            kind = 2;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.NamedType)
            //            kind = 0;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Parameter)
            //            kind = 2;
            //        else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Namespace)
            //            kind = 7;
            //        else
            //            kind = 5;
            //      //  v = new ListViewItem(dd.Name);
            //      //  v.ImageIndex = kind;
            //        //T.Add(v);
            //        //S.Add(v);

            //    }
            //}

        }
        public void ReloadTypes(bool alls = true)
        {

            //SuspendLayout();
            //cc = new List<ListViewItem>();
            ////T.Sort(new SortComparer());
            //if (alls == true)
            //{

            //    foreach (ListViewItem b in T)
            //        cc.Add(b);
            //    int i = 0;


            //    foreach (ListViewItem c in E)
            //        if (!cc.Contains(c))
            //            cc.Add(c);
            //    cc.Sort(new SortComparer());
            //    while (i < cc.Count - 1)
            //    {
            //        ListViewItem v0 = cc[i] as ListViewItem;
            //        ListViewItem v1 = cc[i + 1] as ListViewItem;
            //        if (v0.Text == v1.Text)
            //            cc.Remove(v0);
            //        else i++;
            //    }

            //    i = 0;
            //    foreach (ListViewItem c in cc)
            //        c.Tag = i++;

            //    //foreach (ListViewItem b in T)
            //    //{
            //    //    b.Tag = i++;
            //    //    cc.Add(b);

            //    //}
            //    //foreach (ListViewItem c in E)
            //    //{
            //    //    if (!cc.Contains(c))
            //    //        cc.Add(c);
            //    //    c.Tag = i++;
            //    //}
            //}
            //else
            //{
            //    int i = 0;
            //    //cc.AddRange(S.ToArray());
            //    foreach (ListViewItem b in S)
            //        cc.Add(b);
            //    foreach (ListViewItem c in E)
            //        if (!cc.Contains(c))
            //            cc.Add(c);
            //    cc.Sort(new SortComparer());
            //    foreach (ListViewItem c in cc)
            //        c.Tag = i++;
            //    //int i = 0;
            //    //foreach (ListViewItem b in S)
            //    //{
            //    //    b.Tag = i++;
            //    //    cc.Add(b);
            //    //}
            //    //foreach (ListViewItem c in E)
            //    //{
            //    //    if (!cc.Contains(c))
            //    //        cc.Add(c);
            //    //    c.Tag = i++;
            //    //}
            //}
            ////foreach (ListViewItem c in E)
            ////{
            ////    if (!cc.Contains(c))
            ////        cc.Add(c);
            ////    c.Tag = cc.Count - 1;
            ////}

            ////ResumeLayout();
        }


    }
    /// <summary>
    /// Makes all text after a colon (until the end of line) upper-case.
    /// </summary>
    public class FormatterGenerator : VisualLineElementGenerator
    {
        public override int GetFirstInterestedOffset(int startOffset)
        {
            ICSharpCode.AvalonEdit.Document.TextDocument document = CurrentContext.Document;
            int firstOffset = CurrentContext.VisualLine.FirstDocumentLine.Offset;

            if (firstOffset == startOffset)
                return firstOffset;
            else return -1;

            int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            for (int i = startOffset; i < endOffset; i++)
            {
                char c = document.GetCharAt(i);
                if (c == ':')
                    return i + 1;
            }
            return -1;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            DocumentLine line = CurrentContext.Document.GetLineByOffset(offset);
            if (line.Offset == offset)
                if (offset != line.EndOffset)
            return new UppercaseText(CurrentContext.VisualLine, line.EndOffset - offset);
            return null;
        }

        /// <summary>
        /// Displays a portion of the document text, but upper-cased.
        /// </summary>
        class UppercaseText : VisualLineText
        {
            public UppercaseText(VisualLine parentVisualLine, int length) : base(parentVisualLine, length)
            {
            }

            protected override VisualLineText CreateInstance(int length)
            {
                return new UppercaseText(ParentVisualLine, length);
            }

            public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                
                int relativeOffset = startVisualColumn - VisualColumn;
                StringSegment text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + RelativeTextOffset + relativeOffset, DocumentLength - relativeOffset);
                char[] uppercase = new char[text.Count];
                
                
               for (int i = 0; i < text.Count; i++)
                {
                    uppercase[i] = text.Text[text.Offset + i];
                   
                }
                if (RelativeTextOffset == 0)
                    if (char.IsWhiteSpace(uppercase[0]))
                    {
                        //uppercase[0] = (char)124;
                        int j = 0;
                        while (j < text.Count)
                        {
                            if (!char.IsWhiteSpace(uppercase[j]))
                                break;
                            if((j % 4) == 0)
                            uppercase[j] = (char)124;

                            j++;
                        }


                    }
                return new TextCharacters(uppercase, 0, uppercase.Length, this.TextRunProperties);
            }
        }
    }
    public class LinkGenerator : VisualLineElementGenerator
    {
        readonly static Regex imageRegex = new Regex(@"<mylink>", RegexOptions.IgnoreCase);

        public LinkGenerator()
        { }

        Match FindMatch(int startOffset)
        {
            // fetch the end offset of the VisualLine being generated
            int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            ICSharpCode.AvalonEdit.Document.TextDocument document = CurrentContext.Document;
            string relevantText = document.GetText(startOffset, endOffset - startOffset);
            return imageRegex.Match(relevantText);
        }

        /// Gets the first offset >= startOffset where the generator wants to construct
        /// an element.
        /// Return -1 to signal no interest.
        public override int GetFirstInterestedOffset(int startOffset)
        {
            Match m = FindMatch(startOffset);
            return m.Success ? (startOffset + m.Index) : -1;
        }

        /// Constructs an element at the specified offset.
        /// May return null if no element should be constructed.
        public override VisualLineElement ConstructElement(int offset)
        {
            Match m = FindMatch(offset);
            // check whether there's a match exactly at offset
            if (m.Success && m.Index == 0)
            {
                var line = new VisualLineLinkText(CurrentContext.VisualLine, m.Length);

                line.NavigateUri = new Uri("http://google.com");
                return line;
            }
            return null;
        }
    }
    /// <summary>
    /// VisualLineElement that represents a piece of text and is a clickable link.
    /// </summary>
    public class CustomLinkVisualLineText : VisualLineText
    {

        public delegate void CustomLinkClickHandler(string link);

        public event CustomLinkClickHandler CustomLinkClicked;

        private string Link { get; set; }

        /// <summary>
        /// Gets/Sets whether the user needs to press Control to click the link.
        /// The default value is true.
        /// </summary>
        public bool RequireControlModifierForClick { get; set; }

        /// <summary>
        /// Creates a visual line text element with the specified length.
        /// It uses the <see cref="ITextRunConstructionContext.VisualLine"/> and its
        /// <see cref="VisualLineElement.RelativeTextOffset"/> to find the actual text string.
        /// </summary>
        public CustomLinkVisualLineText(string theLink, VisualLine parentVisualLine, int length)
            : base(parentVisualLine, length)
        {
            RequireControlModifierForClick = true;
            Link = theLink;
        }


        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetForegroundBrush(System.Windows.Media.Brushes.GreenYellow);
            TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            return base.CreateTextRun(startVisualColumn, context);
        }

        bool LinkIsClickable()
        {
            if (string.IsNullOrEmpty(Link))
                return false;
            if (RequireControlModifierForClick)
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            else
                return true;
        }


        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            if (LinkIsClickable())
            {
                e.Handled = true;
                e.Cursor = Cursors.Hand;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !e.Handled && LinkIsClickable())
            {

                if (CustomLinkClicked != null)
                {
                    CustomLinkClicked(Link);
                    e.Handled = true;
                }

            }
        }

        protected override VisualLineText CreateInstance(int length)
        {

            var a = new CustomLinkVisualLineText(Link, ParentVisualLine, length)
            {
                RequireControlModifierForClick = RequireControlModifierForClick
            };

            //a.CustomLinkClicked += link => ApplicationViewModel.Instance.ActiveCodeViewDocument.HandleLinkClicked(Link);
            return a;
        }
    }

    public class LineNumberMarginExtended : LineNumberMargin
    {

        public bool enableLineExtended = true;

        public System.Windows.Media.Brush background = System.Windows.Media.Brushes.White;

        /// <inheritdoc/>
		protected override void OnRender(DrawingContext drawingContext)
        {
            System.Windows.Size renderSize = new System.Windows.Size(((FrameworkElement)this).Width, ((FrameworkElement)this).RenderSize.Height);
            drawingContext.DrawRectangle(background, null,
                                         new Rect(0, 0, renderSize.Width, renderSize.Height));
            
            //base.OnRender(drawingContext);

            TextView textView = this.TextView;
            renderSize = this.RenderSize;
            if (textView != null && textView.VisualLinesValid)
            {
                var foreground = (System.Windows.Media.Brush)GetValue(Control.ForegroundProperty);
                foreach (VisualLine line in textView.VisualLines)
                {
                    if (enableLineExtended == true)
                    
                    if (line.FirstDocumentLine.obs != null)
                        continue;
                       
                    int lineNumber = line.FirstDocumentLine.LineNumberExtended;

                    if (enableLineExtended == false)
                    {
                        lineNumber = line.FirstDocumentLine.LineNumber;
                    }

                        FormattedText text = TextFormatterFactory.CreateFormattedText(
                        this,
                        lineNumber.ToString(CultureInfo.CurrentCulture),
                        typeface, emSize, foreground
                    );
                    double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.TextTop);
                    
                        
                    drawingContext.DrawText(text, new System.Windows.Point(renderSize.Width - text.Width, y - textView.VerticalOffset));
                }
            }
        }
        
        protected void OnMouseLeftButtonDowns(MouseButtonEventArgs e)
        {

           
           // var position = Mouse.GetPosition(this);

            //if (position.X > 50)
            //    return;

            //base.OnMouseLeftButtonDown(e);

           // e.Handled = false;




            //if (!e.Handled && TextView != null && textArea != null)
            //{
            //    e.Handled = true;
            //    textArea.Focus();

            //    SimpleSegment currentSeg = GetTextLineSegment(e);
            //    if (currentSeg == SimpleSegment.Invalid)
            //        return;
            //    textArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
            //    if (CaptureMouse())
            //    {
            //        selecting = true;
            //        selectionStart = new AnchorSegment(Document, currentSeg.Offset, currentSeg.Length);
            //        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            //        {
            //            SimpleSelection simpleSelection = textArea.Selection as SimpleSelection;
            //            if (simpleSelection != null)
            //                selectionStart = new AnchorSegment(Document, simpleSelection.SurroundingSegment);
            //        }
            //        textArea.Selection = Selection.Create(textArea, selectionStart);
            //        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            //        {
            //            ExtendSelection(currentSeg);
            //        }
            //        textArea.Caret.BringCaretToView(5.0);
            //    }
            //}
        }
        void ExtendSelection(SimpleSegment currentSeg)
        {
            if (currentSeg.Offset < selectionStart.Offset)
            {
                textArea.Caret.Offset = currentSeg.Offset;
                textArea.Selection = Selection.Create(textArea, currentSeg.Offset, selectionStart.Offset + selectionStart.Length);
            }
            else
            {
                textArea.Caret.Offset = currentSeg.Offset + currentSeg.Length;
                textArea.Selection = Selection.Create(textArea, selectionStart.Offset, currentSeg.Offset + currentSeg.Length);
            }
        }
        SimpleSegment GetTextLineSegment(MouseEventArgs e)
        {
            System.Windows.Point pos = e.GetPosition(TextView);
            pos.X = 0;
            pos.Y = pos.Y.CoerceValue(0, TextView.ActualHeight);
            pos.Y += TextView.VerticalOffset;
            VisualLine vl = TextView.GetVisualLineFromVisualTop(pos.Y);
            if (vl == null)
                return SimpleSegment.Invalid;
            TextLine tl = vl.GetTextLineByVisualYPosition(pos.Y);
            int visualStartColumn = vl.GetTextLineVisualStartColumn(tl);
            int visualEndColumn = visualStartColumn + tl.Length;
            int relStart = vl.FirstDocumentLine.Offset;
            int startOffset = vl.GetRelativeOffset(visualStartColumn) + relStart;
            int endOffset = vl.GetRelativeOffset(visualEndColumn) + relStart;
            if (endOffset == vl.LastDocumentLine.Offset + vl.LastDocumentLine.Length)
                endOffset += vl.LastDocumentLine.DelimiterLength;
            return new SimpleSegment(startOffset, endOffset - startOffset);
        }
    }
    
}