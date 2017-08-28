using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Snippets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using VSParsers;
using VSProvider;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : ContentControl
    {

        //TextEditor textEditor { get; set; }

        public QuickClassBrowser qcb { get; set; }

        public string FileToLoad { get; set; }

        public string ProjectToLoad { get; set; }

        public string SolutionToLoad { get; set; }


        static public object LoadXaml(String Assemblypath)
        {

            Uri us = new Uri("AvalonEdit.Editor;component/editorwindow.xaml", System.UriKind.Relative);
            return Application.LoadComponent(us);

            var assembly = Assembly.LoadFile(Assemblypath);
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            var resourceReader = new ResourceReader(stream);

            foreach (DictionaryEntry resource in resourceReader)
            {
                if (new FileInfo(resource.Key.ToString()).FullName.ToLower().Contains("editorwindow"))
                {
                    Uri uri = new Uri("/" + assembly.GetName().Name + ";component/" + resource.Key.ToString().Replace(".baml", ".xaml"), UriKind.Relative);
                    return Application.LoadComponent(uri);
                   // ResourceDictionary obs = Application.LoadComponent(uri) as ResourceDictionary;
                   // return obs;
               //     this.Resources.MergedDictionaries.Add(skin);
                }
            }
            return null;
        }

        public EditorWindow()
        {

        }

        public void EditorWindows()
        {

            //var sa = AppDomain.CurrentDomain.BaseDirectory + "AvalonEdit.Editor.dll";

            //LoadXaml(sa);

            // System.Uri resourceLocater = new System.Uri("AvalonEdit.Editor;component/editorwindow.xaml", System.UriKind.Relative);//


            // var assembly = Assembly.LoadFile(sa);
            //var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".g.resources");
            //var resourceReader = new ResourceReader(stream);

            //foreach (DictionaryEntry resource in resourceReader)
            //{
            //    if (new FileInfo(resource.Key.ToString()).FullName.ToLower().Contains("editorwindow"))
            //    {
            //        Uri uri = new Uri("/" + assembly.GetName().Name + ";component/" + resource.Key.ToString().Replace(".baml", ".xaml"), UriKind.Relative);
            //        ResourceDictionary skin = Application.LoadComponent(uri) as ResourceDictionary;

                    
            ////        this.Resources.MergedDictionaries.Add(skin);
            //    }
            //}

            //System.Uri resourceLocater = new System.Uri("/AvalonEdit.Editor;component/editorwindow.baml", System.UriKind.Relative);

            //System.Windows.Application.LoadComponent(this, resourceLocater);



//            InitializeComponent();

           // this.LoadViewFromUri("/AvalonEdit.Editor;component/EditorWindow.xml");


            // textEditor = new TextEditor();
            //  textEditor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //  textEditor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            textEditor.FontFamily = new FontFamily(FontFamily.BaseUri, FontFamily.Source);
            textEditor.FontSize = this.FontSize;
            textEditor.HorizontalAlignment = HorizontalAlignment.Stretch;



           

            //string file = "C:\\MSBuildProjects-beta\\VStudio.sln";
            //VSSolutionLoader rw = new VSSolutionLoader();
            //System.Windows.Forms.TreeView vv = rw.LoadProject(file);

            //VSSolution vs = vv.Tag as VSSolution;

            //MessageBox.Show("VSolution loaded.." + vs.Name);


            //vs.CompileSolution();

            //editor.vs = vs;

           // FileToLoad = "C:\\MSBuildProjects-betas\\AvalonEdit.Editor\\EditorWindow.xaml.cs";

           // ProjectToLoad = "C:\\MSBuildProjects-betas\\AvalonEdit.Editor\\AvalonEdit.Editor.csproj";

           // SolutionToLoad = "C:\\MSBuildProjects-betas\\av.sln";

           // editor.SolutionToLoad = SolutionToLoad;
           // editor.ProjectToLoad = ProjectToLoad;
           // editor.FileToLoad = FileToLoad;
            

            //MemoryStream m = new MemoryStream();
            //var se = System.Text.Encoding.UTF8.GetBytes("a\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nda\nb\nc\nd");
            //m.Write(se, 0, se.Length);
            //m.Seek(0, SeekOrigin.Begin);
            //textEditor.Load(m);

            //var g = textEditor.Document.GetLineByNumber(2).Offset;
            //textEditor.Document.Insert(g, "References - \n", new object());
            
            
            
            //g = textEditor.Document.GetLineByNumber(10).Offset;
            //textEditor.Document.Insert(g, "\nReferences - \n", new object());

            //textEditor.Load(FileToLoad);


            qcb = new QuickClassBrowser();

            qcb.namespaceComboBox = namespaceComboBox;
            qcb.classComboBox = classComboBox;
            qcb.membersComboBox = memberComboBox;

            qcb.editorWindow = this;

            qcb.filename = FileToLoad;

            qcb.AttachEvents();


            
            //StartCompilation();
                      


            LoadMargins();

            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(EditorWindow).Assembly.GetManifestResourceStream("AvalonEdit.Editor.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                        HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            // and register it in the HighlightingManager
            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);



            textEditor.SyntaxHighlighting = customHighlighting;


            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;

            r = new BackgroundRenderer();

            r.v = textEditor.TextArea.TextView;
            r.textEditor = textEditor;

            textEditor.TextArea.TextView.BackgroundRenderers.Add(r);

            textEditor.TextArea.TextView.MouseMove += TextView_MouseMove;

            textEditor.TextArea.TextView.MouseDown += TextView_MouseDown;

            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;


            textEditor.TextArea.TextView.MouseHover += TextView_MouseHover;

            textEditor.KeyDown += TextEditor_KeyDown; ;

            //textEditor.PreviewKeyUp += TextEditor_PreviewKeyUp;  

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();
            if (foldingManager == null)
                foldingManager = FoldingManager.Install(textEditor.TextArea);
            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
            foldingStrategy = new BraceFoldingStrategy();

            Colorizer colorizer = new Colorizer();
            colorizer.editorWindow = this;
            textEditor.TextArea.TextView.LineTransformers.Add(colorizer);

            //textEditor.TextArea.TextView.ElementGenerators.Add(new FormatterGenerator());

            completionWindow = null;

            GetKeywords();


            //eTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //eTimer.Interval = 1000;
            //eTimer.Enabled = true;

            eTimer.Tick += new EventHandler(OnTimedEvent);
            eTimer.Interval = new TimeSpan(0, 0, 1);
            eTimer.Start();

            InitializeTextMarkerService();

        }
        bool Handled = false;

        ImageElementGenerator imageElementGenerator = new ImageElementGenerator("");

        public void LoadContent(string content)
        {

         
            textEditor.Document.Text = content;

            textEditor.IsReadOnly = true;

            EnableCaretMoveOverObject = true;

            //EnableLineNumberExtended(false);

            imageElementGenerator.editorWindow = this;

            //textEditor.TextArea.TextView.ElementGenerators.Add(imageElementGenerator);

            foldingStrategy = new CommentAndMethodFoldingStrategy();
            
            textEditor.SetCurrentValue(TextEditor.IsModifiedProperty, false);

            textEditor.TextArea.TextView.MouseDown += TextEditor_MouseDown;
        }

        private void TextEditor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            var v = textEditor.TextArea.TextView;
            TextViewPosition? pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
            if (pos != null)
            {
                var line = pos.Value.Line;
                var column = pos.Value.Column;
                

                
                //string wordHover = string.Empty;
                //var offset = textEditor.Document.GetOffset(line, column);
                //if (offset >= textEditor.Document.TextLength) offset--;
                //var textAtOffset = textEditor.Document.GetText(offset, 1);

                //while (!string.IsNullOrWhiteSpace(textAtOffset))
                //{
                //    if (offset < 1)
                //        break;
                //    wordHover = textAtOffset + wordHover;
                //    offset--;
                //    if (offset == textEditor.Document.TextLength) break;
                //    textAtOffset = textEditor.Document.GetText(offset, 1);
                //    if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                //        break;
                //}
                //offset = textEditor.Document.GetOffset(line, column);
                //if (offset < textEditor.Document.TextLength - 1)
                //{
                //    offset++;
                //    textAtOffset = textEditor.Document.GetText(offset, 1);

                //    while (!string.IsNullOrWhiteSpace(textAtOffset))
                //    {
                //        if (offset >= textEditor.Document.TextLength)
                //            break;
                //        wordHover = wordHover + textAtOffset;
                //        offset++;
                //        if (offset == textEditor.Document.TextLength) break;
                //        textAtOffset = textEditor.Document.GetText(offset, 1);
                //        if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                //            break;
                //    }
                //    r.word = wordHover;
                //}
                //if (wordHover == " . . . ")
                {
                    DocumentLine gv = textEditor.Document.GetLineByNumber(line);
                    if (gv.obs != null)
                        if (gv.obs is ISymbol)
                            if(column > 0 && column < 15)
                        {
                            string FileName = ProjectToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

                            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

                            editor.vp = editor.vs.GetProjectbyFileName(FileName);

                            ISymbol s = gv.obs as ISymbol;
                            var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp, StringReplace.Replace(textEditor.Document.Text));
                            if (r != null)
                            {
                                StringBuilder b = new StringBuilder();

                                List<LinkItem> bc = new List<LinkItem>();

                                foreach (var rs in r)
                                {
                                    foreach (var ls in rs.Locations)
                                    {
                                        var d = ls;// rs.Locations.FirstOrDefault();
                                        LinkItem bw = new LinkItem(d.Document.Name, d.Document.FilePath);
                                            bw.Location = ls;
                                        bc.Add(bw);

                                    }
                                }
                                //MessageBox.Show("Symbol name " + s.Name + "\n" + b.ToString());
                                LinkViewer vs = new LinkViewer();
                                vs.Load(bc);

                                if (toolTip != null)
                                    toolTip.IsOpen = false;

                                //vs.Left = gv.LineNumbertextEditor.TextArea.Caret.Location.Column;

                                bool? result = vs.ShowDialog();

                                if (result == null || result == false)
                                    return;

                                //MessageBox.Show(vs.SelectedLinkItem);

                                editor.vs.LoadFileFromProject("", null, vs.SelectedLinkItem, null, vs.GetLocation(vs.SelectedLinkItem));

                            }
                        }
                }
            }
        }
        public void LoadFile(string filename)
        {
            // FileToLoad = "C:\\MSBuildProjects-betas\\AvalonEdit.Editor\\EditorWindow.xaml.cs";

            // ProjectToLoad = "C:\\MSBuildProjects-betas\\AvalonEdit.Editor\\AvalonEdit.Editor.csproj";

            // SolutionToLoad = "C:\\MSBuildProjects-betas\\av.sln";

            

            FileToLoad = filename;
            editor.FileToLoad = FileToLoad;

            textEditor.Load(filename);

        }
        System.Windows.Threading.DispatcherTimer eTimer =  new System.Windows.Threading.DispatcherTimer();// new System.Timers.Timer(1000);
        private void OnTimedEvent(object source, /*Elapsed*/EventArgs e)
        {
            if (!textEditor.TextArea.TextView.IsFocused)
            {
                if (toolTip != null)
                    toolTip.IsOpen = false;
                return;
            }

            if (editor == null)
                return;
            if (editor.vs == null)
                return;
            if (editor.vp == null)
                return;
            if (Handled == false)
                return;
            // (!textEditor.IsFocused)
            //    return;
            
            Errors = editor.vs.GetParserErrors(editor.vp, FileToLoad, StringReplace.Replace(textEditor.Document.Text));
            AddDiagnosticErrors(Errors);
            Handled = false;
        }

        public List<Diagnostic> Errors { get; set; }

        private void TextEditor_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var loopCounter = new SnippetReplaceableTextElement { Text = "i" };
                Snippet snippet = new Snippet
                {
                    Elements = {
        new SnippetTextElement { Text = "for(int " },
        new SnippetBoundElement { TargetElement = loopCounter },
        new SnippetTextElement { Text = " = " },
        new SnippetReplaceableTextElement { Text = "0" },
        new SnippetTextElement { Text = "; " },
        loopCounter,
        new SnippetTextElement { Text = " < " },
        new SnippetReplaceableTextElement { Text = "end" },
        new SnippetTextElement { Text = "; " },
        new SnippetBoundElement { TargetElement = loopCounter },
        new SnippetTextElement { Text = "++) { \t" },
        new SnippetCaretElement(),
        new SnippetTextElement { Text = " }" }
    }
                };
                snippet.Insert(textEditor.TextArea);
            }
        }

        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            Handled = true;

            if (e.Key == Key.F8)
                AddMarkerFromSelectionClick(null, null);
            else
            if (e.Key == Key.F12)
            {
                
                this.Dispatcher.BeginInvoke(new Action(() => { GetMemberOverCaret(); }));
            }

        }

        public void Deactivate()
        {
            if (toolTip != null)
                toolTip.IsOpen = false;
        }

        public void GetMemberOverCaret()
        {
            //string FileName = "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            //string filename = "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            //VSSolution vs = editor.vs;

            //Document d = vs.GetDocument(FileName, filename);

            //Document c = Formatter.FormatAsync(d).Result;

            Tuple<string, string, ISymbol> t = editor.GetType(textEditor);

            string file = "";

            if (vs == null)
                return;

            

            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                this.Dispatcher.BeginInvoke(new Action(() => { vs.LoadFileFromContent(t.Item1, null, t.Item2, t.Item3); }));
            }));

            // set the apartment state  
            newWindowThread.SetApartmentState(ApartmentState.STA);

            // make the thread a background thread  
            newWindowThread.IsBackground = false;

            // start the thread  
            newWindowThread.Start();

        }

       

      
        public void FormatDocument()
        {
            string FileName = "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            VSSolution vs = editor.vs;

            //Document d = vs.GetDocument(FileName, filename);

            //Document c = Formatter.FormatAsync(d).Result;

            //textEditor.Document.Text = d.GetTextAsync().Result.ToString();
        }


        public SortedSet<int> sc = new SortedSet<int>();

        public Dictionary<int, string> dc = new Dictionary<int, string>();

        public void LoadProjectTypes(List<INamespaceOrTypeSymbol> symbols)
        {
           
            
            var currentLine = textEditor.Document.GetLineByOffset(textEditor.CaretOffset);
            List<string> ns = new List<string>();
            StringBuilder b = new StringBuilder();
            StringBuilder n = new StringBuilder();
            //b.Append("\\b(?>thises");
            //n.Append("\\b(?>thises");
            foreach (var cs in symbols)
            {
                if (cs.IsNamespace)
                    continue;
                if (cs.IsType)
                {
                   // b.Append("|" + cs.Name);
                    int hash = cs.Name.GetHashCode();
                    sc.Add(hash);
                    if(!dc.ContainsKey(hash))
                    dc.Add(hash, cs.Name);


                }
                if (!ns.Contains(cs.ContainingNamespace.Name))
                {
                    n.Append("|" + cs.ContainingNamespace.Name + ".");
                    ns.Add(cs.ContainingNamespace.Name);
                }

            }
            //b.Append(")\\b");
            //n.Append(")\\b");

            HighlightingRule hs = new HighlightingRule();

            //System.Text.RegularExpressions.Regex v = new System.Text.RegularExpressions.Regex("\b(?>this|base)\b");

            //hs.Regex = new System.Text.RegularExpressions.Regex("\\b(?>thises|bases)\\b");
            string ts = b.ToString();

            ts = ts.Replace("?", string.Empty);
            ts = ts.Replace("$", string.Empty);
            ts = ts.Replace("*", string.Empty);
            ts = ts.Replace("_", string.Empty);

            //ts = Regex.Escape(ts);

            b.Clear();

            b.Insert(0, "\\b(?>thises");
            n.Insert(0, "\\b(?>thises");
            b.Append(ts);
            n.Append(ts);
            b.Append(")\\b");
            n.Append(")\\b");

            ts = b.ToString();

            hs.Regex = new System.Text.RegularExpressions.Regex(ts);

            HighlightingColor c = new HighlightingColor();
            c.Foreground = new SimpleHighlightingBrush(System.Windows.Media.Color.FromRgb(111, 196, 220));
            hs.Color = c;



           // textEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(hs);



            hs = new HighlightingRule();

            hs.Regex = new System.Text.RegularExpressions.Regex(n.ToString());

            c = new HighlightingColor();
            c.Foreground = new SimpleHighlightingBrush(System.Windows.Media.Colors.Gray);
            hs.Color = c;

            textEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(hs);

            textEditor.CaretOffset = currentLine.Offset;

            textEditor.TextArea.Caret.BringCaretToView();


            //if (currentLine.LineNumber > 2)
            //    textEditor.ScrollTo(currentLine.LineNumber - 2, textEditor.TextArea.Caret.Column);
            //else
            //    textEditor.ScrollTo(currentLine.LineNumber, textEditor.TextArea.Caret.Column);
        }
        ITextMarkerService textMarkerService;

        void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        void RemoveAllClick(object sender, RoutedEventArgs e)
        {
            textMarkerService.RemoveAll(m => true);
        }

        void RemoveSelectedClick(object sender, RoutedEventArgs e)
        {
            textMarkerService.RemoveAll(IsSelected);
        }

        void AddMarkerFromSelectionClick(object sender, RoutedEventArgs e)
        {
            ITextMarker marker = textMarkerService.Create(textEditor.SelectionStart, textEditor.SelectionLength);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
        }
        void AddDiagnosticErrors(List<Diagnostic> errors)
        {
            RemoveAllClick(null, null);

            foreach (Diagnostic d in errors)
            {
                if (d.Location == null)
                    continue;
                if (d.Location.SourceSpan == null)
                    continue;
                int start = d.Location.SourceSpan.Start;
                int length = d.Location.SourceSpan.Length;

                ITextMarker marker = textMarkerService.Create(start, length);
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
            }
            vs.LoadErrors(null, Errors);
        }
        bool IsSelected(ITextMarker marker)
        {
            int selectionEndOffset = textEditor.SelectionStart + textEditor.SelectionLength;
            if (marker.StartOffset >= textEditor.SelectionStart && marker.StartOffset <= selectionEndOffset)
                return true;
            if (marker.EndOffset >= textEditor.SelectionStart && marker.EndOffset <= selectionEndOffset)
                return true;
            return false;
        }

        public void EnableLineNumberExtended(bool enable)
        {
            if (lineNumberMarginExtended != null)
                lineNumberMarginExtended.enableLineExtended = enable;
        }

        public LineNumberMarginExtended lineNumberMarginExtended { get; set; }

        public void LoadMargins()
        {
            BreakPointMargin bl = new BreakPointMargin();
            bl.Margin = new Thickness(0, 0, 0, 0);
            bl.background = System.Windows.Media.Brushes.LightGray;
            bl.MouseDown += Bl_MouseDown;


            LineNumberMarginExtended b = new LineNumberMarginExtended();
            b.Margin = new Thickness(5, 0, 0, 0);
            b.SetValue(Control.ForegroundProperty, new SolidColorBrush(Color.FromArgb(200, 111, 196, 220)));
            lineNumberMarginExtended = b;

            BreakPointMargin br = new BreakPointMargin();
            br.Margin = new Thickness(0, 0, 0, 0);
            br.pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.LightGreen, 3);
            br.margin = 25;
            //Line ls = DottedLineMargin.Create() as Line;
            //ls.Margin = new Thickness(5, 0, 0, 0);
            //br.LoadLine(ls);

            textEditor.TextArea.LeftMargins.Add(bl);
            textEditor.TextArea.LeftMargins.Add(b);
            textEditor.TextArea.LeftMargins.Add(br);
            textEditor.TextArea.TextView.Margin = new Thickness(15, 0, 0, 0);
            //textEditor.TextArea.LeftMargins.Add(ls);

            //textEditor.ShowLineNumbers = true;

            //textEditor.TextArea.SelectionBorder = null;

            //textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
            //textEditor.SyntaxHighlighting = customHighlighting;
            // initial highlighting now set by XAML
        }

        private void Bl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Insert breakpoint");
        }

        public Editor editor = new Editor();

        public ToolTip toolTip { get; set; }

        public void View(int offset)
        {
            textEditor.CaretOffset = offset;
            textEditor.TextArea.Caret.BringCaretToView();
        }

        private void TextView_MouseHover(object sender, MouseEventArgs e)
        {
            var v = textEditor.TextArea.TextView;
            var pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
            if (pos != null)
            {
                var line = pos.Value.Line;
                var column = pos.Value.Column;
                string wordHover = string.Empty;
                var offset = textEditor.Document.GetOffset(line, column);
                if (offset >= textEditor.Document.TextLength) offset = textEditor.Document.TextLength - 1;
                var textAtOffset = textEditor.Document.GetText(offset, 1);

                while (!string.IsNullOrWhiteSpace(textAtOffset))
                {
                    if (offset < 1)
                        break;
                    wordHover = textAtOffset + wordHover;
                    offset--;
                    if (offset == textEditor.Document.TextLength) break;
                    textAtOffset = textEditor.Document.GetText(offset, 1);
                    if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                        break;
                }
                offset = textEditor.Document.GetOffset(line, column);
                if (offset < textEditor.Document.TextLength - 1)
                {
                    offset++;
                    textAtOffset = textEditor.Document.GetText(offset, 1);

                    while (!string.IsNullOrWhiteSpace(textAtOffset))
                    {
                        if (offset >= textEditor.Document.TextLength)
                            break;
                        wordHover = wordHover + textAtOffset;
                        offset++;
                        if (offset == textEditor.Document.TextLength) break;
                        textAtOffset = textEditor.Document.GetText(offset, 1);
                        if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                            break;
                    }
                    //r.word = wordHover;
                }

                if (toolTip != null)
                    toolTip.IsOpen = false;
                if (string.IsNullOrEmpty(wordHover))
                    return;
                toolTip = new ToolTip();
                toolTip.Closed += ToolTip_Closed; ;

                toolTip.PlacementTarget = v;
                toolTip.PlacementRectangle = new Rect(e.GetPosition(textEditor), new System.Windows.Size(300, 40));
                toolTip.Content = new TextBlock
                {
                    Text = wordHover,
                    TextWrapping = TextWrapping.Wrap
                };
                toolTip.IsOpen = true;

                e.Handled = true;

            }

        }

        private void ToolTip_Closed(object sender, RoutedEventArgs e)
        {

        }

        int CaretCurrentLine = 0;

        public bool EnableCaretMoveOverObject = false;

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            var v = textEditor.TextArea.TextView;
            var pos = textEditor.TextArea.Caret.Position;
            if (pos != null)
            {
                var line = pos.Line;
                var column = pos.Column;
                string wordHover = string.Empty;
                var offset = textEditor.Document.GetOffset(line, column);
                if (offset >= 0)
                {

                    DocumentLine gv = textEditor.Document.GetLineByNumber(line);
                    if(EnableCaretMoveOverObject == false)
                    if (gv.obs != null)
                    {


                        if (CaretCurrentLine > line)
                        {
                            textEditor.TextArea.Caret.Position = new TextViewPosition(line - 1, 0);
                            CaretCurrentLine = line - 1;
                        }
                        else
                        {
                            textEditor.TextArea.Caret.Position = new TextViewPosition(line + 1, 0);
                            CaretCurrentLine = line + 1;
                        }

                        return;
                    }

                    if (offset >= textEditor.Document.TextLength) offset = textEditor.Document.TextLength - 1;
                    var textAtOffset = textEditor.Document.GetText(offset, 1);

                    while (!string.IsNullOrWhiteSpace(textAtOffset))
                    {
                        if (offset < 1)
                            break;
                        wordHover = textAtOffset + wordHover;
                        offset--;
                        if (offset == textEditor.Document.TextLength) break;
                        textAtOffset = textEditor.Document.GetText(offset, 1);
                        if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                            break;
                    }
                    offset = textEditor.Document.GetOffset(line, column);
                    if (offset < textEditor.Document.TextLength - 1)
                    {
                        offset++;
                        textAtOffset = textEditor.Document.GetText(offset, 1);

                        while (!string.IsNullOrWhiteSpace(textAtOffset))
                        {
                            if (offset >= textEditor.Document.TextLength)
                                break;
                            wordHover = wordHover + textAtOffset;
                            offset++;
                            if (offset == textEditor.Document.TextLength) break;
                            textAtOffset = textEditor.Document.GetText(offset, 1);
                            if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                                break;
                        }
                        r.word = wordHover;
                    }
                }
                CaretCurrentLine = line;

                v.InvalidateVisual();

            }

            r.shouldHighlight = false;

        }

        private void TextView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var v = textEditor.TextArea.TextView;
            TextViewPosition? pos = textEditor.GetPositionFromPoint(e.GetPosition(textEditor));
            if (pos != null)
            {
                var line = pos.Value.Line;
                var column = pos.Value.Column;
                string wordHover = string.Empty;
                var offset = textEditor.Document.GetOffset(line, column);
                if (offset >= textEditor.Document.TextLength) offset--;
                var textAtOffset = textEditor.Document.GetText(offset, 1);

                while (!string.IsNullOrWhiteSpace(textAtOffset))
                {
                    if (offset < 1)
                        break;
                    wordHover = textAtOffset + wordHover;
                    offset--;
                    if (offset == textEditor.Document.TextLength) break;
                    textAtOffset = textEditor.Document.GetText(offset, 1);
                    if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                        break;
                }
                offset = textEditor.Document.GetOffset(line, column);
                if (offset < textEditor.Document.TextLength - 1)
                {
                    offset++;
                    textAtOffset = textEditor.Document.GetText(offset, 1);

                    while (!string.IsNullOrWhiteSpace(textAtOffset))
                    {
                        if (offset >= textEditor.Document.TextLength)
                            break;
                        wordHover = wordHover + textAtOffset;
                        offset++;
                        if (offset == textEditor.Document.TextLength) break;
                        textAtOffset = textEditor.Document.GetText(offset, 1);
                        if (textAtOffset == "." || textAtOffset == "(" || textAtOffset == ")")
                            break;
                    }
                    r.word = wordHover;
                }
                if (wordHover == "References")
                {
                    DocumentLine gv = textEditor.Document.GetLineByNumber(line);
                    if (gv.obs != null)
                        if (gv.obs is ISymbol)
                        {
                            string FileName = ProjectToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

                            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

                            editor.vp = editor.vs.GetProjectbyFileName(FileName);

                            ISymbol s = gv.obs as ISymbol;
                            var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp, StringReplace.Replace(textEditor.Document.Text));
                            if (r != null)
                            {
                                StringBuilder b = new StringBuilder();

                                List<LinkItem> bc = new List<LinkItem>();

                                foreach (var rs in r)
                                {
                                    foreach (var ls in rs.Locations)
                                    {
                                        var d = ls;// rs.Locations.FirstOrDefault();
                                        LinkItem bw = new LinkItem(""/*d.Document.Name*/, d.Document.FilePath);
                                        bc.Add(bw);

                                    }
                                }
                                //MessageBox.Show("Symbol name " + s.Name + "\n" + b.ToString());
                                LinkViewer vs = new LinkViewer();
                                vs.Load(bc);

                                if (toolTip != null)
                                    toolTip.IsOpen = false;

                                //vs.Left = gv.LineNumbertextEditor.TextArea.Caret.Location.Column;

                                bool? result = vs.ShowDialog();
                                
                                if (result == null || result == false)
                                    return;

                                //MessageBox.Show(vs.SelectedLinkItem);

                                editor.vs.LoadFileFromProject("", null, vs.SelectedLinkItem);

                            }
                        }
                }

                //CaretCurrentLine = ((TextViewPosition)pos).Location.Line;

                //textEditor.TextArea.Caret.Position = (TextViewPosition)pos;

                //v.InvalidateVisual();


            }

            if (pos != null)
            {

                CaretCurrentLine = ((TextViewPosition)pos).Location.Line;

                textEditor.TextArea.Caret.Position = (TextViewPosition)pos;

            }
            v.InvalidateVisual();

            r.shouldHighlight = false;



        }

        BackgroundRenderer r { get; set; }

        private void TextView_MouseMove(object sender, MouseEventArgs e)
        {
            TextView v = textEditor.TextArea.TextView;

            //DrawingVisual drawingVisual = new DrawingVisual();
            //DrawingContext drawingContext = drawingVisual.RenderOpen();
            //drawingContext.DrawRectangle(System.Windows.Media.Brushes.Gray, new System.Windows.Media.Pen(System.Windows.Media.Brushes.Gray, 2), new Rect(0, 0, 600, 600));
            //v.BackgroundRenderers.Add()



            r.shouldHighlight = false;

            var c = e.GetPosition(v);

            c.Y += v.VerticalOffset;

            if (c.X < 10)
            {
                VisualLineText vle = v.GetVisualLineElementFromPosition(c) as VisualLineText;
                if (vle == null)
                    return;
                int offset = vle.ParentVisualLine.StartOffset;

                var f = foldingManager.GetFoldingsContaining(offset);

                if (f.Count <= 0)
                    return;

                TextViewPosition b = new TextViewPosition(v.Document.GetLineByOffset(f[f.Count - 1].StartOffset).LineNumber, 0);
                b.VisualColumn = 0;
                TextViewPosition d = new TextViewPosition(v.Document.GetLineByOffset(f[f.Count - 1].EndOffset).LineNumber, 0);
                d.VisualColumn = 0;
                var p = v.GetVisualPosition(b, VisualYPosition.Baseline);
                r.Position = p;
                var s = v.GetVisualPosition(d, VisualYPosition.Baseline);

                r.EndPosition = s;

                r.shouldHighlight = true;
            }
            v.InvalidateVisual();
        }

        //        string currentFileName;

        CompletionWindow completionWindow;

        CompletionControls completionControl { get; set; }

        public void UpdateReferences(List<int> mb, Dictionary<int, ISymbol> objects)
        {
            string d = textEditor.Document.Text;

            List<DocumentLine> ns = new List<DocumentLine>();
            int i = 0;
            foreach (int v in mb)
            {
              
                int g = textEditor.Document.GetLineByOffset(v).Offset;

                var c = textEditor.Document.GetLineByOffset(v);

                string s = textEditor.Document.Text.Substring(c.Offset, c.Length);

                //   if (s.Trim().StartsWith("string"))
                //       MessageBox.Show("c");

                textEditor.Document.Insert(g, "\n  References - \n", objects[v]);

                ns.Add(textEditor.Document.GetLineByOffset(g));

                //i+= 0;
            }

            foreach (var b in ns)
            {
             
               

                textEditor.Document.Update(b);

            }
        }

        public void UpdateReferencesForContent(List<int> mb, Dictionary<int, ISymbol> objects)
        {
            string d = textEditor.Document.Text;

            List<DocumentLine> ns = new List<DocumentLine>();
            int i = 0;
            foreach (int v in mb)
            {

                int g = textEditor.Document.GetLineByOffset(v).Offset;

                var c = textEditor.Document.GetLineByOffset(v);

                string s = textEditor.Document.Text.Substring(c.Offset, c.Length);

                //   if (s.Trim().StartsWith("string"))
                //       MessageBox.Show("c");

                //textEditor.Document.Insert(g, "\n  References - \n", objects[v]);

                c.obs = objects[v];

                ns.Add(textEditor.Document.GetLineByOffset(g));

                //i+= 0;
            }
            foreach(DocumentLine dd in textEditor.Document.Lines)

            //foreach (var b in ns)
            {



                textEditor.Document.Update(dd); // b

            }
        }

        public bool shouldUpdate = false;

        public async Task<int> CodeCompletionPreview()
        {
            string FileName = "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            string filename = "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            editor.vp = editor.vs.GetProjectbyFileName(FileName);

            var ts = editor.GetTypes();

            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionControl = new CompletionControls(completionWindow);

                completionWindow.SetContent(completionControl);
                completionWindow.completionList = completionControl.completionList;
                //completionWindow.AttachEvents();


            }
            completionWindow.Visibility = Visibility.Hidden;
            List<ICompletionData> data = new List<ICompletionData>();
            List<object> register = new List<object>();
            foreach (var b in ts)
            {

                object obs = b;

                CsCompletionData d = new CsCompletionData(b.Name);
                d.obs = obs;
                d.Image = CompletionControls.GetImageSource(b, register);
                d.Search = editor.WordUnderCaret;
                d.textBlock = d;

                data.Add(d);
            }
            data.AddRange(d);
            completionControl.completionList.CompletionData = data;
            completionControl.LoadStacks(register);
            completionControl.completionList.SaveState();
            return 0;
        }


        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            shouldUpdate = true;

            if (string.IsNullOrEmpty(e.Text))
                return;
            if (string.IsNullOrWhiteSpace(e.Text))
                return;

            string s = textEditor.Document.Text;

            Caret cc = textEditor.TextArea.Caret;

            string FileName = ProjectToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            editor.vp = editor.vs.GetProjectbyFileName(FileName);

            editor.TypesWithDelay(0, textEditor.Document, textEditor, filename, cc.Offset);

            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionControl = new CompletionControls(completionWindow);

                completionWindow.SetContent(completionControl);
                completionWindow.completionList = completionControl.completionList;

            }

            if (editor.WordUnderCaret == null)
                return;

            if (editor.WordUnderCaret.Length == 1 || editor.shouldUpdate || editor.WordUnderCaret.EndsWith("."))
            {
                completionWindow.LoadStartOffset();
                List<ICompletionData> data = new List<ICompletionData>();
                data.Clear();
                if (editor.sns == null)
                    return;
                List<object> register = new List<object>();
                foreach (var b in editor.sns)
                {
                    //object obs = b.Kind;
                    object obs = b;

                    CsCompletionData d = new CsCompletionData(b.Name);
                    d.obs = obs;

                    d.Image = CompletionControls.GetImageSource(b, register);
                    d.Search = editor.WordUnderCaret;
                    d.textBlock = d;

                    data.Add(d);
                }
                data.AddRange(d);
                completionControl.completionList.CompletionData = data;
                if (editor.WordUnderCaret.EndsWith("."))
                {
                    completionControl.completionList.ListBox.ItemsSource = data;
                }
                completionControl.LoadStacks(register);

            }
            else
            {
                //if(editor.WordUnderCaret == "ows")
                foreach (var q in completionControl.completionList.ListBox.ItemsSource)
                {
                    ((ICompletionData)q).Search = editor.WordUnderCaret;
                }
            }


            if (completionControl.completionList.CompletionData.Count > 0)
            {
                if (shouldUpdate == true)
                {
                    completionWindow.Visibility = Visibility.Visible;
                    completionWindow.MakeVisible(true);
                    completionWindow.Show();
                }
            }
            else
            {
                completionWindow.Visibility = Visibility.Hidden;
                completionWindow.MakeVisible(false);

            }

            completionControl.completionList.SaveState();


        }

        public static List<CsCompletionData> d = new List<CsCompletionData>();

        void GetKeywords()
        {
            string[] s = { "async", "await", "return", "base", "bool", "byte", "char", "const", "decimal", "double", "default", "false", "dynamic", "float", "global", "goto", "int", "long", "nameof", "new", "goto", "null", "ref", "out", "short", "sbyte", "sizeof", "string", "new", "this", "throw", "typeof", "uint", "ulong", "ushort", "void", "yield", "var", "true", "object" };

            d = new List<CsCompletionData>();
            foreach (string b in s)
            {
                CsCompletionData c = new CsCompletionData(b);
                c.Image = CompletionControls.ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.KeywordSnippet_16x);
                c.Search = "";
                c.obs = "Keywords";
                d.Add(c);
            }
        }

        void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    //completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        private void DoWork()
        {
            BackgroundWorker bw = new BackgroundWorker();

            Runner r = new Runner();

            r.editorWindow = this;

            bw.DoWork += (sender, args) =>
            {
                // do your lengthy stuff here -- this will happen in a separate thread
                //    Thread.Sleep(15000);
                r.Start();
            };

            bw.RunWorkerCompleted += (sender, args) =>
            {
                if (args.Error != null)  // if an exception occurred during DoWork,
                    MessageBox.Show(args.Error.ToString());  // do your error handling here

                // do any UI stuff after the long operation here

            };

            bw.RunWorkerAsync(); // start the background worker
        }

        public void Start()
        {

            DoWork();

            return;

            //this.Dispatcher.Invoke((Action)(() => {
            string file = "C:\\MSBuildProjects-beta\\VStudio.sln";
            VSSolutionLoader rw = new VSSolutionLoader();
            System.Windows.Forms.TreeView vv = rw.LoadProject(file);

            VSSolution vs = vv.Tag as VSSolution;

            //    MessageBox.Show("VSolution loaded.." + vs.Name);


            vs.CompileSolution();

            editor.vs = vs;
            //}));
        }

        public delegate void runner();


        public void StartCompilation()
        {

            runner run = new runner(DoWork);

            ThreadStart s = new ThreadStart(run);

            Thread thread = new Thread(s);

            thread.SetApartmentState(ApartmentState.STA);

            thread.Start();


            {



                // while (true)
                // {
                //this.Dispatcher.BeginInvoke((Action)(() => {

                //Application.Current.Dispatcher.BeginInvoke((Action)(() => {

                // textEditor.Dispatcher.BeginInvoke((Action)(() => {

                //var t = Task.Run(() => this.Dispatcher.BeginInvoke((Action)(() => { Start(); })));


                //     }));
            }
            // }).Start();
        }


        #region Folding
        FoldingManager foldingManager;
        object foldingStrategy;

        void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textEditor.SyntaxHighlighting == null)
            {
                //foldingStrategy = null;
            }
            else
            {
                switch (textEditor.SyntaxHighlighting.Name)
                {
                    case "XML":
                        foldingStrategy = new XmlFoldingStrategy();
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        break;
                    case "C#":
                    case "C++":
                    case "PHP":
                    case "Java":
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);
                        //foldingStrategy = new BraceFoldingStrategy();
                        break;
                    default:
                        textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
                        //foldingStrategy = null;
                        break;
                }
            }
            if (foldingStrategy != null)
            {
                if (foldingManager == null)
                    foldingManager = FoldingManager.Install(textEditor.TextArea);
                UpdateFoldings();
            }
            else
            {
                if (foldingManager != null)
                {
                    FoldingManager.Uninstall(foldingManager);
                    foldingManager = null;
                }
            }
        }

        public int GetLineExtended(int line)
        {
            return textEditor.Document.GetLineByNumber(line).LineNumberExtended;
        }

        public void SelectText(int start, int length)
        {


            textEditor.CaretOffset = start;

            textEditor.TextArea.Caret.BringCaretToView();

            textEditor.SelectionStart = start;
            textEditor.SelectionLength = length;
        }

        public void SetAllFoldings(bool folded = false)
        {
            
            foreach (var s in foldingManager.AllFoldings)
                s.IsFolded = folded;
        }
        void UpdateFoldings()
        {
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
            if (foldingStrategy is CommentAndMethodFoldingStrategy)
            {
                ((CommentAndMethodFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
        }
        #endregion

        public List<ISymbol> allMembers { get; set; }

        public List<int> mb { get; set; }

        public void UpdateSourceDocument()
        {

            textEditor.IsReadOnly = true;

            editor.GetMembers(textEditor.Document.Text);

            List<ISymbol> symbols = editor.symbols;

            allMembers = new List<ISymbol>();


            mb = new List<int>();
            // The selected class was changed.
            // Update the list of member items to be the list of members of the current class.
            foreach (ISymbol item in symbols)
            {
                INamedTypeSymbol selectedClass = item.Kind == SymbolKind.NamedType ? item as INamedTypeSymbol : null;
                if (selectedClass != null)
                {

                    //IClass compoundClass = selectedClass.GetCompoundClass();
                    foreach (var m in selectedClass.GetMembers())
                    {
                        if (!m.Locations[0].IsInSource)
                            continue;



                        //    AddMember(selectedClass, m, m.IsConstructor ? TYPE_CONSTRUCTOR : TYPE_METHOD);
                        //}
                        if (m.Kind == SymbolKind.Method)
                        {
                            IMethodSymbol f = m as IMethodSymbol;
                            if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind == SymbolKind.Property)
                        {

                            //AddMember(selectedClass, m, m.Kind);
                            //allMembers.Add(m);
                        }
                        else
                        if (m.Kind == SymbolKind.Field)
                        {
                            IFieldSymbol f = m as IFieldSymbol;
                            if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                //allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind == SymbolKind.Event)
                        {
                            //AddMember(selectedClass, m, m.Kind);
                            //allMembers.Add(m);
                        }
                        
                    }
                    
                }
            }
            Dictionary<int, ISymbol> dict = new Dictionary<int, ISymbol>();



            foreach (ISymbol s in allMembers)
            {
                if (!dict.ContainsKey(s.Locations[0].SourceSpan.Start))
                {


                    if (s.Locations[0].IsInSource)
                    {
                        if (s.Locations[0].SourceTree.FilePath == FileToLoad)
                        {
                            mb.Add(s.Locations[0].SourceSpan.Start);
                            dict.Add(s.Locations[0].SourceSpan.Start, s);
                        }
                    }
                }
              
            }

            mb.Sort();
            mb.Reverse();

            UpdateReferences(mb, dict);

            textEditor.IsReadOnly = false;
        }
        public void UpdateSourceDocumentForContent(ISymbol symbol = null)
        {

            textEditor.IsReadOnly = true;

            if (symbol == null)
                return;

            editor.GetMembers(textEditor.Document.Text, true);

            INamedTypeSymbol cc = symbol as INamedTypeSymbol;
            
            if(cc == null)
                return;

           // ISymbol symbolToLoad = vs.GetSymbol(symbol);

           


            List<ISymbol> symbols = cc.GetMembers().ToList();



            
            

            allMembers = new List<ISymbol>();


            mb = new List<int>();
            
            foreach (ISymbol item in symbols)
            {
                {
                    {
                        ISymbol m = item;

                        //    AddMember(selectedClass, m, m.IsConstructor ? TYPE_CONSTRUCTOR : TYPE_METHOD);
                        //}
                        if (m.Kind == SymbolKind.Method)
                        {
                            IMethodSymbol f = m as IMethodSymbol;
                            if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind == SymbolKind.Property)
                        {
                            //AddMember(selectedClass, m, m.Kind);
                            allMembers.Add(m);
                        }
                        else
                        if (m.Kind == SymbolKind.Field)
                        {
                            IFieldSymbol f = m as IFieldSymbol;
                            if (f.AssociatedSymbol == null)
                            {
                                //AddMember(selectedClass, m, m.Kind);
                                //allMembers.Add(m);
                            }
                        }
                        else
                        if (m.Kind == SymbolKind.Event)
                        {
                            //AddMember(selectedClass, m, m.Kind);
                            allMembers.Add(m);
                        }
                        
                    }
            
                }
            }
            Dictionary<int, ISymbol> dict = new Dictionary<int, ISymbol>();

            var cs = editor.symbols;

            foreach (ISymbol s in allMembers)
            {

            
                
                if(s is IEventSymbol)
                {

                    var pw = cs.Select(st => st).Where(gc => gc.Name == s.Name).FirstOrDefault();

                    if (pw != null)
                    {

                        IEventSymbol ev = pw as IEventSymbol;
                        if (ev.Locations != null)
                            if (ev.Locations.Count() > 0)
                                if (!dict.ContainsKey(ev.Locations[0].SourceSpan.Start))
                                {
                                    mb.Add(ev.Locations[0].SourceSpan.Start);
                                    dict.Add(ev.Locations[0].SourceSpan.Start, s);
                                }
                    }
                    continue;

                }

                if (s is IPropertySymbol)
                {

                  

                    IPropertySymbol p = s as IPropertySymbol;

                    var pr = cs.Select(sq => sq).Where(d => d.Name == s.Name).FirstOrDefault();
                    {

                        IPropertySymbol rr = pr as IPropertySymbol;

                        if (pr == null)
                            continue;

                        if(rr.GetMethod != null)
                        {
                            IMethodSymbol gs = rr.GetMethod as IMethodSymbol;
                            var pl = gs.Locations[0];
                        }

                        if (rr.Name.ToLower() != p.Name.ToLower())
                        {
                            continue;
                        }
                        if (rr.Type.Name.ToLower() != p.Type.Name.ToLower())
                        {
                            continue;
                        }
                        if (!dict.ContainsKey(rr.Locations[0].SourceSpan.Start))
                        {
                            mb.Add(rr.Locations[0].SourceSpan.Start);
                            dict.Add(rr.Locations[0].SourceSpan.Start, s);
                        }
                        continue;
                    }
                    
                }
                
                IMethodSymbol g = s as IMethodSymbol;

         
                {
                    if (cs.Any(d => d.Name == s.Name)  || cs.Any(d => d.Name == cc.Name && s.Name == ".ctor"))
                    {
                        var bc = cs.Select(d => d).Where(dd => dd.Name == s.Name).ToList();

                        var nw = cs.Select(d => d).Where(dd => s.Name == ".ctor" && dd != null && dd.Name != null && dd.Name == cc.Name).ToList();

                        bc.AddRange(nw);

                        if (bc.Count == 1)
                        {
                            if (!dict.ContainsKey(bc[0].Locations[0].SourceSpan.Start))
                            {
                                mb.Add(bc[0].Locations[0].SourceSpan.Start);
                                dict.Add(bc[0].Locations[0].SourceSpan.Start, s);
                            }
                            continue;
                        }

                        else 
                        if(bc.Count > 1)
                        {

                            foreach(var bb in bc)
                            {
                                IMethodSymbol f = bb as IMethodSymbol;

                                if (f == null)
                                    continue;

                                if (f.Parameters.Count() != g.Parameters.Count())
                                {
                                    continue;
                                }
                                bool shouldload = true;
                                foreach(IParameterSymbol ps in f.Parameters)
                                {
                                    bool found = false;
                                    foreach(IParameterSymbol rs in g.Parameters)
                                    {

                                        if (rs.Type is INamedTypeSymbol)
                                        {

                                            INamedTypeSymbol es = rs.Type as INamedTypeSymbol;

                                            INamedTypeSymbol ec = ps.Type as INamedTypeSymbol;

                                            if(ec == null)
                                            {
                                                continue;
                                            }

                                            if (es.Name != ec.Name)
                                                continue;

                                            if (es.IsGenericType)
                                                if (ec.IsGenericType)
                                                {
                                                    bool founds = true;

                                                    if (es.TypeArguments != null)
                                                    {
                                                        for (int i = 0; i < es.TypeArguments.Count(); i++)
                                                        {
                                                            if (es.TypeArguments[i].Name != ec.TypeArguments[i].Name)
                                                            {
                                                                founds = false;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (!founds)
                                                    {
                                                        continue;
                                                    }

                                                    found = true;
                                                    break;


                                                }
                                                else continue;
                                        }

                                        if (rs.Type is IArrayTypeSymbol)
                                        {
                                            IArrayTypeSymbol ep = ps.Type as IArrayTypeSymbol;
                                            if (ep == null)
                                            {
                                 //               shouldload = false;
                                                continue;
                                            }
                                            IArrayTypeSymbol es = rs.Type as IArrayTypeSymbol;

                                            if (es.ElementType.Name != ep.ElementType.Name)
                                            {
                                                continue;
                                            }
                                            if (ps.Name != rs.Name)
                                            {
                                                continue;
                                            }
                                            found = true;
                                            break;
                                        }
                                        if (rs.Type is IPointerTypeSymbol)
                                        {

                                            IPointerTypeSymbol bs = rs.Type as IPointerTypeSymbol;

                                            IPointerTypeSymbol gs = ps.Type as IPointerTypeSymbol;

                                            if(gs == null)
                                            {
                                                continue;
                                            }
                                            if(bs.PointedAtType.Name != gs.PointedAtType.Name)
                                            {
                                                continue;
                                            }
                                            if (ps.Name != rs.Name)
                                            {
                                                continue;
                                            }
                                            found = true;
                                            break;
                                        }


                                        if (ps.Name != rs.Name)
                                        {
                                   //         shouldload = false;
                                            continue;
                                        }
                                        if (ps.Type.TypeKind != rs.Type.TypeKind)
                                        {
                                            //shouldload = false;
                                          //  continue;
                                        }
                                        string []qq = ps.Type.Name.Split(".".ToCharArray());
                                        string q = qq[qq.Count() - 1];
                                        if (q != rs.Type.Name)
                                        {
                                            //shouldload = false;
                                            continue;
                                        }
                                        found = true;
                                        break;
                                    }
                                    if (!found)
                                    {
                                        shouldload = false;
                                        break;
                                    }
                                }

                                if (!shouldload)
                                    continue;
                                //var r = f.Parameters.All(y => g.Parameters.Any(z => z.Name == y.Name && z.Type.Name == y.Type.Name && z.Type.TypeKind == y.Type.TypeKind && (y.Type is IArrayTypeSymbol && z.Type is IArrayTypeSymbol ? (z.Type as IArrayTypeSymbol).ElementType.Name == (y.Type as IArrayTypeSymbol).ElementType.Name ? true: false : true)));

                                //if (r)
                                {

                                    if (!dict.ContainsKey(bb.Locations[0].SourceSpan.Start))
                                    {
                                        mb.Add(bb.Locations[0].SourceSpan.Start);
                                        dict.Add(bb.Locations[0].SourceSpan.Start, s);
                                    }
                                    break;
                                }

                            }

                            continue;

                        }
                        

                        //if (s.Locations[0].IsInSource)
                        {
                            var v = bc[0];

                            //if (s.Locations[0].SourceTree.FilePath == FileToLoad)
                            if (!dict.ContainsKey(v.Locations[0].SourceSpan.Start))
                            {
                                mb.Add(v.Locations[0].SourceSpan.Start);
                                dict.Add(v.Locations[0].SourceSpan.Start, s);
                            }
                        }
                    }
                }
            }

            mb.Sort();
            mb.Reverse();

            UpdateReferencesForContent(mb, dict);

            SetAllFoldings(true);

            textEditor.IsReadOnly = false;
        }

        public VSSolution vs { get; set; }

       public async void LoadFromProject(VSSolution vs, ISymbol symbol = null)
        {
            
            this.vs = vs;

            await Task.Run(() =>
            {

                var vp = vs.GetProjectbyCompileItem(FileToLoad);

                if (vp != null)
                {
                    
                    ProjectToLoad = vp.FileName;
                    SolutionToLoad = vs.solutionFileName;

                    editor.SolutionToLoad = SolutionToLoad;
                    editor.ProjectToLoad = ProjectToLoad;
                    editor.FileToLoad = FileToLoad;
                }

                this.editor.vs = vs;
                
                this.qcb.editorWindow = this;

                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (vp == null)
                        this.UpdateSourceDocumentForContent(symbol);
                    else this.UpdateSourceDocument();

                    this.qcb.DoUpdate(editor.symbols);

                    this.LoadProjectTypes(editor.GetTypes());

                    Task task = Task.Run(async () =>
                    {
                        await this.CodeCompletionPreview();
                    });




                }));
            });

        }

    }

    public class BackgroundRenderer : IBackgroundRenderer
    {

        public System.Windows.Point Position { get; set; }

        public System.Windows.Point EndPosition { get; set; }

        public bool shouldHighlight = false;

        public FrameworkElement v { get; set; }

        public TextEditor textEditor { get; set; }

        System.Windows.Media.Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 140, 140, 140));

        System.Windows.Media.Pen pen = new System.Windows.Media.Pen(new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 140, 140, 140)), 1);

        System.Windows.Media.Pen penOutline = new System.Windows.Media.Pen(new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 80, 80, 80)), 1);

        public string word { get; set; }

        int prevStart = 0;

        public KnownLayer Layer
        {
            get
            {
                return KnownLayer.Background;
            }
        }



        public void Draw(TextView textView, DrawingContext drawingContext)
        {

            if (shouldHighlight)
                drawingContext.DrawRectangle(brush, pen, new Rect(0, Position.Y - textView.VerticalOffset, v.ActualWidth, EndPosition.Y - Position.Y));

            if (string.IsNullOrEmpty(word))
                return;

            if (!textView.VisualLinesValid)
                return;

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
                return;

            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

            textView.EnsureVisualLines();
            var currentLine = textEditor.Document.GetLineByOffset(textEditor.CaretOffset);

            string text = textView.Document.Text.Substring(viewStart, viewEnd - viewStart);
            int i = text.IndexOf(word);
            while (i >= 0)
            {

                if (i + viewStart < currentLine.Offset || i + viewStart > currentLine.EndOffset + currentLine.DelimiterLength)
                {

                    BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
                    //geoBuilder.AlignToMiddleOfPixels = true;
                    geoBuilder.CornerRadius = 1;
                    TextSegment s = new TextSegment();
                    s.StartOffset = viewStart + i;
                    s.EndOffset = viewStart + i + word.Length;

                    geoBuilder.AddSegment(textView, s);
                    Geometry geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        drawingContext.DrawGeometry(brush, pen, geometry);
                    }
                }
                i = text.IndexOf(word, i + word.Length);

                if (i >= text.Length)
                    break;
            }
            penOutline.DashStyle = DashStyles.Dash;
            VisualLine ve = visualLines.First();
            int start = 0;
            i = 0;
            while (i < visualLines.Count() - 1)
            {
                ve = visualLines[i];

                VisualLine e = visualLines[i + 1];

                int viewFirst = ve.FirstDocumentLine.Offset;
                int viewLast = ve.LastDocumentLine.EndOffset;
                string s = textView.Document.Text.Substring(viewFirst, viewLast - viewFirst);
                int count = s.Length;
                int last = 0;

                start = 0;
                bool isWhitespace = true;
                int stride = 13;// (int)(ve.GetVisualPosition(4, VisualYPosition.LineTop).X - ve.GetVisualPosition(0, VisualYPosition.LineTop).X);
                while (isWhitespace)
                {
                    var p = ve.GetVisualPosition(start, VisualYPosition.LineTop);
                    p.Y -= textView.VerticalOffset;

                    var pe = e.GetVisualPosition(start, VisualYPosition.LineTop);
                    pe.Y -= textView.VerticalOffset;
                    pe.X = p.X;


                    if (string.IsNullOrEmpty(s))
                    {
                        if (start > prevStart)
                            isWhitespace = false;
                        else
                        {
                            if ((start % 4) == 0)
                            {
                                p.X = (start / 4) * stride;
                                pe.X = p.X;
                                drawingContext.DrawLine(penOutline, p, pe);
                                last = start;
                            }
                            start++;

                        }


                    }
                    else
                    {
                        if (char.IsWhiteSpace(s[start]))
                        {
                            if ((start % 4) == 0)
                            {
                                drawingContext.DrawLine(penOutline, p, pe);
                                last = start;
                            }

                            start++;
                        }
                        else isWhitespace = false;
                        if (start >= count)
                            break;
                    }
                }
                prevStart = last;
                i++;
            }


            foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            {

                drawingContext.DrawRectangle(
                    null, pen,
                    new Rect(new Point(rect.Location.X - 32, rect.Location.Y), new Size(textView.ActualWidth + 20, rect.Height)));
            }

        }


    }

    //public Word getCaretWord()
    //{
    //    Row row = _control.Document[_control.Caret.Position.Y];



    //    Word w = row.GetCaretWord(_control.Caret.Position.X);

    //    return w;
    //}



    //public string GetCaretWord()
    //{
    //    Row row = _control.Document[_control.Caret.Position.Y];



    //    Word w = row.GetCaretWord(_control.Caret.Position.X);

    //    if (w != null)
    //        return w.Text;
    //    else return "";
    //}
    //public string[] GetPrevCaretWord()
    //{
    //    Row row = _control.Document[_control.Caret.Position.Y];



    //    Word w = row.GetCaretWord(_control.Caret.Position.X);


    //    if (w == null)
    //        return null;

    //    string[] s = row.Words(w.Text);


    //    return s;
    //}
    //public List<string> GetCaretWords()
    //{
    //    Row row = _control.Document[_control.Caret.Position.Y];

    //    Word w = row.GetCaretWord(_control.Caret.Position.X);

    //    return row.GetWords(w);

    //}
    //public string GetCaretString()
    //{
    //    Row row = _control.Document[_control.Caret.Position.Y];

    //    return row.Text;

    //}

    public class Runner
    {

        public EditorWindow editorWindow { get; set; }

        public async void Start()
        {

            string file = editorWindow.SolutionToLoad;// "C:\\MSBuildProjects-betas\\av.sln";
            //VSSolution.CompileSolution(file);


            // editorWindow.Dispatcher.BeginInvoke((Action)(() => {
            //string file = "C:\\MSBuildProjects-beta\\VStudio.sln";

            VSSolutionLoader rw = new VSSolutionLoader();

            VSSolution vs = null;

            Task vs_task = new Task(delegate
            {

                System.Windows.Forms.TreeView vv = rw.LoadProject(file);

                vs = vv.Tag as VSSolution;

            });

            vs_task.Start();

            Task rs_task = new Task(delegate
            {

                //vs.CompileSolution();

                rw.CompileSolution(file);
            });
            rs_task.Start();
            await Task.WhenAll(new Task[] { vs_task, rs_task });

            vs.comp = rw.comp;
            vs.nts = rw.nts;
            vs.named = rw.named;
            vs.workspace = rw.workspace;
            vs.solution = rw.solution;

            Editor editor = editorWindow.editor;

            editorWindow.editor.vs = vs;




            // string FileName = "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

            // string filename = "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

            // editor.vp = editor.vs.GetProjectbyFileName(FileName);

            //editor.TypesWithDelay(0, editorWindow.textEditor.Document, new TextViewPosition(cc.Location), filename, cc.Offset);

            //editorWindow.UpdateSourceDocument();

            ///editor.GetMembers();

            editorWindow.qcb.editorWindow = editorWindow;

            await Task.Run(() =>
            {

                editorWindow.Dispatcher.Invoke((Action)(() =>
            {

                editorWindow.UpdateSourceDocument();

                editorWindow.qcb.DoUpdate(editor.symbols);

                //editorWindow.qcb.UpdateSourceDocument();

                editorWindow.LoadProjectTypes(editor.GetTypes());

                Task task = Task.Run(async () =>
                {
                    await editorWindow.CodeCompletionPreview();
                });




            }));
            });


            //}));
        }

       
    }
    public static class Helper
    {
        public static void LoadViewFromUri(this ContentControl userControl, string baseUri)
        {
            try
            {
                var resourceLocater = new Uri(baseUri, UriKind.Relative);
                var exprCa = (PackagePart)typeof(Application).GetMethod("GetResourceOrContentPart", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { resourceLocater });
                var stream = exprCa.GetStream();
                var uri = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), resourceLocater);
                var parserContext = new ParserContext
                {
                    BaseUri = uri
                };
                typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { stream, parserContext, userControl, true });
            }
            catch (Exception e)
            {
                //log
            }
        }

    }
}

   


