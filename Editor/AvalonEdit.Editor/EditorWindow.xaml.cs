using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;

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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml;
using VSParsers;
using VSProvider;
using static AvalonEdit.Editor.CompletionControls;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : UserControl
    {
        //TextEditor textEditor { get; set; }

        public QuickClassBrowser qcb { get; set; }

        public string FileToLoad { get; set; }

        public string ProjectToLoad { get; set; }

        public string SolutionToLoad { get; set; }

        public bool IsQuickClassBrowser { get; set; }

        public static List<EditorCommand> commands = new List<EditorCommand>();

        public static List<EditorCommand> recommands = new List<EditorCommand>();

        public static EditorWindow staticEditorWindow { get; set; }

        static EditorWindow()
        {
            staticEditorWindow = new EditorWindow();
            staticEditorWindow.HideQuickClassBrowser();
            staticEditorWindow.HideAllMargin();
            staticEditorWindow.HideBottom();
            staticEditorWindow.Width = 250;
            staticEditorWindow.Height = 110;
            staticEditorWindow.textEditor.TextArea.FontSize = 9.6;
            staticEditorWindow.textEditor.TextArea.Background = Brushes.White;
            staticEditorWindow.Visibility = Visibility.Visible;
            staticEditorWindow.textEditor.IsReadOnly = true;
            //staticEditorWindow.Margin = new Thickness(1, 1, 1, 1);
            staticEditorWindow.BorderThickness = new Thickness(1, 1, 1, 1);
            staticEditorWindow.BorderBrush = Brushes.LightGray;
            staticEditorWindow.Opacity = 1;
        }

        public void StartSearchResults(Searcher searcher)
        {
            commandFind.Execute(searcher);
        }

        static public void LoadStaticContent(string content, VSSolution vs)
        {
            staticEditorWindow.LoadContent(content);
            staticEditorWindow.vs = vs;
            staticEditorWindow.LoadFromProject(vs);
        }

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

        public void EditorWindows2()
        {
        }

        public EditorWindow()
        {
            InitializeComponent();

            IsQuickClassBrowser = true;

            referenced = true;

            textEditor.FontFamily = new FontFamily(FontFamily.BaseUri, FontFamily.Source);
            textEditor.FontSize = this.FontSize;
            textEditor.HorizontalAlignment = HorizontalAlignment.Stretch;

            qcb = new QuickClassBrowser();

            qcb.namespaceComboBox = namespaceComboBox;
            qcb.classComboBox = classComboBox;
            qcb.membersComboBox = memberComboBox;

            qcb.editorWindow = this;

            qcb.filename = FileToLoad;

            qcb.AttachEvents();

            LoadMargins();

            IHighlightingDefinition customHighlighting;
            using (Stream s = typeof(EditorWindow).Assembly.GetManifestResourceStream("AvalonEdit.Editor.CustomHighlighting.xshd"))
            {
                if (s == null)
                    throw new InvalidOperationException("Could not find embedded resource");
                using (XmlReader reader = new XmlTextReader(s))
                {
                    Tuple<IHighlightingDefinition, XshdSyntaxDefinition> Highlighters = (Tuple<IHighlightingDefinition, XshdSyntaxDefinition>)ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    customHighlighting = Highlighters.Item1;
                    var xshd = Highlighters.Item2;
                    foreach (var c in xshd.Elements)
                    {
                        if (c is ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdRuleSet)
                        {
                            ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdRuleSet rs = c as ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdRuleSet;
                            foreach (var d in rs.Elements)
                                if (d is ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdKeywords)
                                {
                                    ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdKeywords keywords = d as ICSharpCode.AvalonEdit.Highlighting.Xshd.XshdKeywords;
                                    Words = keywords.Words.ToList();
                                    break;
                                }
                        }
                    }
                }
            }

            HighlightingManager.Instance.RegisterHighlighting("Custom Highlighting", new string[] { ".cool" }, customHighlighting);
            textEditor.SyntaxHighlighting = customHighlighting;

            textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            textEditor.TextChanged += TextEditor_TextChanged;

            r = new BackgroundRenderer();

            r.v = textEditor.TextArea.TextView;
            r.textEditor = textEditor;

            textEditor.TextArea.TextView.BackgroundRenderers.Add(r);
            textEditor.TextArea.TextView.MouseMove += TextView_MouseMove;
            textEditor.TextArea.TextView.MouseDown += TextView_MouseDown;
            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            textEditor.TextArea.TextView.MouseHover += TextView_MouseHover;
            textEditor.TextArea.PreviewKeyDown += TextArea_PreviewKeyDown;
            textEditor.KeyDown += TextEditor_KeyDown;
            this.SizeChanged += EditorWindow_SizeChanged;

            this.textEditor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

            //textEditor.PreviewKeyUp += TextEditor_PreviewKeyUp;

            foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += delegate { UpdateFoldings(); };
            foldingUpdateTimer.Start();

            if (foldingManager == null)
                foldingManager = FoldingManager.Install(textEditor.TextArea);
            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(textEditor.Options);

            foldingStrategy = new BraceFoldingStrategy();

            Colorizer colorizer = new Colorizer(this);

            textEditor.TextArea.TextView.LineTransformers.Add(colorizer);

            //textEditor.TextArea.TextView.ElementGenerators.Add(new FormatterGenerator());

            elementGenerator = new ImageElementGenerator("");

            textEditor.TextArea.TextView.ElementGenerators.Add(elementGenerator);

            textEditor.TextArea.Options.EnableVirtualSpace = false;
            textEditor.TextArea.SelectionBorder = null;

            completionWindow = null;

            GetKeywords();

            eTimer.Tick += new EventHandler(OnTimedEvent);
            eTimer.Interval = new TimeSpan(0, 0, 1);
            eTimer.Start();

            InitializeTextMarkerService();
            ContextMenu = CreateContextMenu(this, this.textEditor);
            Mouse.OverrideCursor = Cursors.Arrow;
            textEditor.TextArea.SelectionChanged += TextArea_SelectionChanged;
            zoom.SelectionChanged += Zoom_SelectionChanged;
            hs._scrollViewer = this.textEditor;
            hs.Orientation = Orientation.Horizontal;
            hs.LoadEvents();
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            Handled = true;
        }

        private void TextArea_TextCopied(object sender, TextEventArgs e)
        {
            Handled = true;
        }

        private void TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
                Handled = true;
            if (e.Key == Key.Enter)
                shouldUpdate = true;
            if (e.Key == Key.Insert)
                UpdateInsert.Invoke();
        }

        private DispatcherTimer foldingUpdateTimer { get; set; }

        public void Dispose()
        {
            eTimer.Stop();
            eTimer = null;
            foldingUpdateTimer.Stop();
            foldingUpdateTimer = null;
            textEditor = null;
            //textEditor.TextArea.LeftMargins.Clear();
            elementGenerator = null;
            typeNames = null;
            sc = null;
            dc = null;
            d = null;
            if (qcb != null)
                qcb.Dispose();
            qcb = null;
        }

        public string GetSelectedText()
        {
            return textEditor.SelectedText;
        }

        private void EditorWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (popup != null)
            {
                popup.Element_SizeChanged();
                textEditor.TextArea.TextView.Redraw();
            }
        }

        private ImageElementGenerator elementGenerator { get; set; }

        private void TextView_ScrollOffsetChanged(object sender, EventArgs e)
        {
            if (popup == null)
                return;
            var location = this.PointToScreen(new Point(0, 0));

            // popup.Top = 30 - textEditor.TextArea.TextView.VerticalOffset;
            // popup.InvalidateVisual();
        }

        public List<string> Words { get; set; }

        public double zoomScaleFactor = 1.0;

        public List<string> typeNames { get; set; }

        public List<string> GetTypesNames()
        {
            if (vs == null || string.IsNullOrEmpty(FileToLoad))
                return new List<string>();
            typeNames = vs.TypesNames(FileToLoad);
            return typeNames;
        }

        private void Zoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems == null || e.AddedItems.Count <= 0)
                return;
            ComboBoxItem b = (ComboBoxItem)e.AddedItems[0];
            var s = b.Content.ToString().Replace("%", "");
            double scale = Convert.ToDouble(s) / 100.0;
            Transform c = new ScaleTransform(scale, scale);
            textEditor.TextArea.LayoutTransform = c;
            textEditor.InvalidateVisual();
            // double p = zoomScaleFactor;
            zoomScaleFactor = scale;
            if (popup != null)
            {
                shouldPopupUpdate = true;
                popup.Element_SizeChanged();
                textEditor.TextArea.TextView.Redraw();
            }
        }

        private bool shouldPopupUpdate = false;

        private void PopupUpdate()
        {
            if (embedEditorWindow != null)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var d = GetWidthAfterTransform();

                    embedEditorWindow.Width = d.Item1;
                    popup.Width = d.Item1;
                    //popup.Top = d.Item2;
                    popup.LayoutTransform = new ScaleTransform(1.0 / zoomScaleFactor, 1.0 / zoomScaleFactor);
                    embedEditorWindow.textEditor.TextArea.LayoutTransform = textEditor.TextArea.LayoutTransform;
                    shouldPopupUpdate = false;
                }));
            }
        }

        private void TextArea_SelectionChanged(object sender, EventArgs e)
        {
        }

        //HighlightingRule hsd { get; set; }
        async public Task<int> UpdateTypes(string d, VSSolution vs, HighlightingRule hs)
        {
            editor.ProjectTypes(vs);
            editor.GetMembers(d);
            LoadProjectTypesForUpdate(hs);
            GetTypesNames();

            return 0;
        }

        public bool referenced_done = false;

        async public Task<bool> Activated(bool isActive)
        {
            if ((bool)isActive)
            {
                if (popup != null)
                    if (popup.Visibility != Visibility.Visible)
                    {
                        Size sz = GetPopupSize();
                        popup.Visibility = Visibility.Visible;
                        popup.ReloadIfNeeded(sz);
                    }

                if (!referenced)
                {
                    referenced = true;
                    var d = textEditor.Document.Text;
                    try
                    {
                        await Task.Run(async () =>
                         {
                             try
                             {
                                 HighlightingRule hs = new HighlightingRule();
                                 await UpdateTypes(d, vs, hs);

                                 textEditor.Dispatcher.Invoke(new Action(() =>
                                 {
                                     textEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(hs);

                                     textEditor.TextArea.Caret.BringCaretToView();
                                     textEditor.TextArea.InvalidateVisual();
                                 }));
                             }
                             catch (Exception ex)
                             {
                                 referenced = false;
                             }
                         });

                        await Task.Run(async () => { References(); });
                        qcb.CaretPositionAdjustNeeded = true;
                        shouldUpdate = true;
                    }
                    catch (Exception ex)
                    {
                        referenced = false;
                    }
                }
                //if (!areTypesLoaded)
                //{
                //    if (vs != null)
                //    {
                //        if (editor.typesofproject == null)
                //            editor.GetTypes(vs);
                //        if (editor.symbols == null)
                //            editor.GetMembers();
                //    }
                //    if (editor.symbols != null)
                //    {
                //        this.qcb.DoUpdate(editor.symbols);
                //        areTypesLoaded = true;
                //    }
                //}
            }
            else
            {
                if (popup != null)
                    if (popup.Visibility != Visibility.Hidden)
                        popup.Visibility = Visibility.Hidden;
            }
            return true;
        }

        public async Task<bool> Referencer(string d)
        {
            bool ref0 = Task.Run(async () =>
             {
                 ////     await this.Dispatcher.BeginInvoke(new Action(() =>
                 ////     {
                 //         editor.ProjectTypes(vs);
                 //         editor.GetMembers(d);
                 //         LoadProjectTypes();
                 //  //   }));
                 HighlightingRule hs = new HighlightingRule();
                 await UpdateTypes(d, vs, hs);

                 textEditor.Dispatcher.Invoke(new Action(() =>
                 {
                     textEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(hs);
                     //textEditor.CaretOffset = currentLine.Offset;
                     textEditor.TextArea.Caret.BringCaretToView();
                     textEditor.TextArea.InvalidateVisual();
                 }));
                 return true;
             }).Result;

            //  await UpdateTypes(d, vs);

            bool ref1 = Task.Run(async () => { References(); return true; }).Result;

            return true;
        }

        public static Func<int, int, int, string, string> func { get; set; }

        public static Action UpdateInsert { get; set; }

        //private void EditorWindow_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!this.IsFocused)
        //        if (embedEditorWindow != null)
        //            if (!embedEditorWindow.IsFocused)
        //                if (popup != null)
        //                    popup.IsOpen = false;
        //}

        public void HideQuickClassBrowser()
        {
            IsQuickClassBrowser = false;

            dockPanel.Children.Remove(quickviewer);
            this.InvalidateVisual();
        }

        private bool IsZoom = true;

        public void HideZoom()
        {
            IsZoom = false;

            dockPanel.Children.Remove(quickviewer);

            editorGrid.Children.Remove(zoom);

            Grid.SetColumn(hs, 0);
            Grid.SetColumnSpan(hs, 2);

            hs.Opacity = 1;

            this.InvalidateVisual();
        }

        public void HideBottom()
        {
            IsZoom = false;

            dockPanel.Children.Remove(quickviewer);

            editorGrid.Children.Remove(zoom);

            editorGrid.Children.Remove(hs);

            editorGrid.RowDefinitions[1].Height = new GridLength(0);

            hs.Opacity = 1;

            this.InvalidateVisual();
        }

        private bool Handled = false;

        private ImageElementGenerator imageElementGenerator = new ImageElementGenerator("");

        public void LoadContent(string content)
        {
            textEditor.Document.Text = content;

            textEditor.IsReadOnly = true;

            EnableCaretMoveOverObject = true;

            imageElementGenerator.editorWindow = this;

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
                            if (column > 0 && column < 15)
                            {
                                string FileName = ProjectToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

                                string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

                                editor.vp = editor.vs.GetProjectbyFileName(FileName);

                                ISymbol s = gv.obs as ISymbol;
                                var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp, References).Result;
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
            textEditor.SelectedText = "";
        }

        public class CommandViewDesigner : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                TextEditor textEditor = (TextEditor)Context;
                MessageBox.Show("It Worked - Shift+F7 for " + textEditor.Document.FileName);
                EventHandler invoker = AvalonEditorCommands.ViewDesignerCommand;
                if (invoker != null)
                    invoker(null, null);
            }

            public event EventHandler CanExecuteChanged;

            public TextEditor Context { get; set; }

            public CommandViewDesigner(TextEditor Context)
            {
                this.Context = Context;
            }
        }

        public class CommandQuickActionsAndRefactorings : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl++");
            }

            public event EventHandler CanExecuteChanged;

            public CommandQuickActionsAndRefactorings()
            {
            }
        }

        public class CommandRename : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+R,Ctrl+R");
            }

            public event EventHandler CanExecuteChanged;

            public CommandRename()
            {
            }
        }

        public class CommandRemoveAndSortUsings : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+r, Ctrl+G");
            }

            public event EventHandler CanExecuteChanged;

            public CommandRemoveAndSortUsings()
            {
            }
        }

        public class CommandPeekDefinition : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Alt+F12");
            }

            public event EventHandler CanExecuteChanged;

            public CommandPeekDefinition()
            {
            }
        }

        public class CommandGoToDefinition : ICommand
        {
            public EditorWindow editorWindow { get; set; }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (editorWindow != null)
                    editorWindow.Dispatcher.BeginInvoke(new Action(() => { editorWindow.GetMemberOverCaret(); }));
            }

            public event EventHandler CanExecuteChanged;

            public CommandGoToDefinition(EditorWindow editorWindow)
            {
                this.editorWindow = editorWindow;
            }
        }

        public class CommandGoToImplementation : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+F12");
            }

            public event EventHandler CanExecuteChanged;

            public CommandGoToImplementation()
            {
            }
        }

        public class CommandFindAllReferences : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Shift+F12");
            }

            public event EventHandler CanExecuteChanged;

            public CommandFindAllReferences()
            {
            }
        }

        public class CommandViewCallHierarchy : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+K, Ctrl+T");
            }

            public event EventHandler CanExecuteChanged;

            public CommandViewCallHierarchy()
            {
            }
        }

        public class CommandRunToCursor : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+F10");
            }

            public event EventHandler CanExecuteChanged;

            public CommandRunToCursor()
            {
            }
        }

        public class CommandExecuteInInteractive : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+E, Ctrl+E");
            }

            public event EventHandler CanExecuteChanged;

            public CommandExecuteInInteractive()
            {
            }
        }

        public class CommandCut : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+X");
            }

            public event EventHandler CanExecuteChanged;

            public CommandCut()
            {
            }
        }

        public class CommandCopy : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+C");
            }

            public event EventHandler CanExecuteChanged;

            public CommandCopy()
            {
            }
        }

        public class CommandPaste : ICommand
        {
            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                MessageBox.Show("It Worked - Ctrl+V");
            }

            public event EventHandler CanExecuteChanged;

            public CommandPaste()
            {
            }
        }
        public class CommandFind : ICommand
        {

            public EditorWindow fe { get; set; }

            public bool CanExecute(object parameter)
            {
                return true;
            }
            FindFormPopup ff { get; set; }
            public void Execute(object parameter)
            {
                //MessageBox.Show("It Worked - Ctrl+F");
                ff = new FindFormPopup();
                
                ff.Placement = PlacementMode.Relative;
                ff.HorizontalOffset = fe.ActualWidth - 300/*ff.ActualWidth*/ - 15; 

                ff.VerticalOffset = 21;
                ff.PlacementTarget = fe;
                
                ff.IsOpen = true;
                ff.forms.editorWindow = fe;   

                if(parameter is Searcher)
                {
                    ff.forms.OpenSearchResults((Searcher)parameter);
                }
            }

            public event EventHandler CanExecuteChanged;

            public CommandFind(EditorWindow t)
            {
                fe = t;
            }
        }
        public class ShowCommandGesture : KeyGesture
        {
            private readonly Key _key;
            private bool _gotFirstGesture;
            private InputGesture _ctrlWGesture { get; set; }

            public ShowCommandGesture(Key fkey, ModifierKeys fmodifiersKeys, Key key, ModifierKeys modifierKeys) : base(key, modifierKeys)
            {
                _ctrlWGesture = new KeyGesture(fkey, fmodifiersKeys);
                _key = key;
            }

            public override bool Matches(object obj, InputEventArgs inputEventArgs)
            {
                KeyEventArgs keyArgs = inputEventArgs as KeyEventArgs;
                if (keyArgs == null || keyArgs.IsRepeat)
                    return false;

                if (_gotFirstGesture)
                {
                    _gotFirstGesture = false;

                    if (keyArgs.Key == _key)
                    {
                        inputEventArgs.Handled = true;
                    }

                    return keyArgs.Key == _key;
                }
                else
                {
                    _gotFirstGesture = _ctrlWGesture.Matches(null, inputEventArgs);
                    if (_gotFirstGesture)
                    {
                        inputEventArgs.Handled = true;
                    }

                    return false;
                }
            }
        }

        public static ContextMenu ContextMenus { get; set; }

        static public ContextMenu CreateContextMenu(EditorWindow editorWindow, TextEditor textEditor)
        {
            ContextMenu contextMenu = new ContextMenu();

            if (ContextMenus != null)
                ContextMenus = contextMenu;
            else
            {
                MenuItem item = new MenuItem();
                item.Header = "View Designer";
                item.Click += Item_Click;
                Image image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;

                item.InputGestureText = "Shift+F7";

                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandViewDesigner(), new KeyGesture(Key.F7, ModifierKeys.Shift)));

                item = new MenuItem();
                item.Header = "Quick Actions And Refactorings";
                item.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;

                item.InputGestureText = "Ctrl+";
                // item.InputBindings.Add(b);
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandQuickActionsAndRefactorings(), new KeyGesture(Key.OemPlus, ModifierKeys.Control)));

                item = new MenuItem();
                item.Header = "Rename";
                item.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Rename_16x);
                item.Icon = image;

                //CommandRename CommandRename = new CommandRename();
                item.InputGestureText = "Ctrl+R, Ctrl+R";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandRename(), new ShowCommandGesture(Key.R, ModifierKeys.Control, Key.R, ModifierKeys.Control)));

                item = new MenuItem();
                item.Header = "Remove And Sort Usings";
                item.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;

                //CommandRemoveAndSortUsings CommandRemoveAndSortUsings = new CommandRemoveAndSortUsings();
                item.InputGestureText = "Ctrl+R, Ctrl+G";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(CommandRemoveAndSortUsings, new ShowCommandGesture(Key.R, ModifierKeys.Control, Key.G, ModifierKeys.Control)));

                Separator separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Peek Definition";
                item.Click += editorWindow.Item_Click_PeekDefinition;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.ViewDefinition_16x);
                item.Icon = image;
                item.InputGestureText = "Alt+F12";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandPeekDefinition(), new KeyGesture(Key.F12, ModifierKeys.Alt)));

                item = new MenuItem();
                item.Header = "Go To Definition";
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.GoToDefinition_16x);
                item.Icon = image;
                item.InputGestureText = "F12";
                item.Click += editorWindow.Item_Click_GoToDefinition;
                contextMenu.Items.Add(item);

                //textEditor.InputBindings.Add(new InputBinding(new CommandGoToDefinition(), new KeyGesture(Key.F12, ModifierKeys.None)));

                item = new MenuItem();
                item.Header = "Go To Implementation";
                item.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;
                item.InputGestureText = "Ctrl+F12";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandGoToImplementation(), new KeyGesture(Key.F12, ModifierKeys.Control)));

                item = new MenuItem();
                item.Header = "Find All References";
                item.Click += editorWindow.Item_Click_FindReferences;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;
                item.InputGestureText = "Shift+F12";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandFindAllReferences(), new KeyGesture(Key.F12, ModifierKeys.Shift)));

                item = new MenuItem();
                item.Header = "View Call Hierarchy";
                item.Click += editorWindow.Item_Click_CallHierarchy;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.CallHierarchyView_16x);
                item.Icon = image;
                item.InputGestureText = "Ctrl+K, Ctrl+T";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandViewCallHierarchy(), new ShowCommandGesture(Key.K, ModifierKeys.Control, Key.T, ModifierKeys.Control)));

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Create Unit Tests";
                item.Click += Item_Click;
                contextMenu.Items.Add(item);

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Breakpoint";
                item.Click += Item_Click;
                contextMenu.Items.Add(item);

                MenuItem subitem = new MenuItem();
                subitem.Header = "Insert Breakpoint";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                subitem = new MenuItem();
                subitem.Header = "Insert Tracepoint";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Run To Cursor";
                item.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandRunToCursor(), new KeyGesture(Key.F10, ModifierKeys.Control)));

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Execute In Interactive";
                item.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandExecuteInInteractive(), new ShowCommandGesture(Key.E, ModifierKeys.Control, Key.E, ModifierKeys.Control)));

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Snippet";
                contextMenu.Items.Add(item);

                subitem = new MenuItem();
                subitem.Header = "Surround With";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Snippet_16x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                subitem = new MenuItem();
                subitem.Header = "Insert Snippet";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Snippet_16x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Cut";
                item.Click += editorWindow.Item_Click_Cut;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Cut_24x);
                item.Icon = image;
                item.InputGestureText = "Ctrl+X";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandCut(), new KeyGesture(Key.X, ModifierKeys.Control)));

                item = new MenuItem();
                item.Header = "Copy";
                item.Click += editorWindow.Item_Click_Copy;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                item.Icon = image;
                item.InputGestureText = "Ctrl+C";
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandCopy(), new KeyGesture(Key.C, ModifierKeys.Control)));

                item = new MenuItem();
                item.Header = "Paste";
                item.Click += editorWindow.Item_Click_Paste;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Paste_16x);
                item.Icon = image;
                item.InputGestureText = "Ctrl+V";
                //item.IsEnabled = false;
                contextMenu.Items.Add(item);
                //textEditor.InputBindings.Add(new InputBinding(new CommandPaste(), new KeyGesture(Key.V, ModifierKeys.Control)));

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Annotation";
                contextMenu.Items.Add(item);

                subitem = new MenuItem();
                subitem.Header = "Show Line Annotations";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Outlining";
                contextMenu.Items.Add(item);

                subitem = new MenuItem();
                subitem.Header = "Toggle Outlining Expansion";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                subitem = new MenuItem();
                subitem.Header = "Toggle All Outlining";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                subitem.Icon = image;
                item.Items.Add(subitem);

                separator = new Separator();
                contextMenu.Items.Add(separator);

                item = new MenuItem();
                item.Header = "Source Control";
                contextMenu.Items.Add(item);

                subitem = new MenuItem();
                subitem.Header = "Undo";
                subitem.Click += Item_Click;
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Copy_32x);
                subitem.Icon = image;
                item.Items.Add(subitem);
            }

            textEditor.InputBindings.Add(new InputBinding(new CommandViewDesigner(textEditor), new KeyGesture(Key.F7, ModifierKeys.Shift)));
            textEditor.InputBindings.Add(new InputBinding(new CommandQuickActionsAndRefactorings(), new KeyGesture(Key.OemPlus, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandRename(), new ShowCommandGesture(Key.R, ModifierKeys.Control, Key.R, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandRemoveAndSortUsings(), new ShowCommandGesture(Key.R, ModifierKeys.Control, Key.G, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandPeekDefinition(), new KeyGesture(Key.F12, ModifierKeys.Alt)));
            textEditor.InputBindings.Add(new InputBinding(new CommandGoToDefinition(editorWindow), new KeyGesture(Key.F12, ModifierKeys.None)));
            textEditor.InputBindings.Add(new InputBinding(new CommandGoToImplementation(), new KeyGesture(Key.F12, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandFindAllReferences(), new KeyGesture(Key.F12, ModifierKeys.Shift)));
            textEditor.InputBindings.Add(new InputBinding(new CommandViewCallHierarchy(), new ShowCommandGesture(Key.K, ModifierKeys.Control, Key.T, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandRunToCursor(), new KeyGesture(Key.F10, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandExecuteInInteractive(), new ShowCommandGesture(Key.E, ModifierKeys.Control, Key.E, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandCut(), new KeyGesture(Key.X, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandCopy(), new KeyGesture(Key.C, ModifierKeys.Control)));
            textEditor.InputBindings.Add(new InputBinding(new CommandPaste(), new KeyGesture(Key.V, ModifierKeys.Control)));

            CommandFind commandFind = new CommandFind(editorWindow);
            editorWindow.commandFind = commandFind;
            textEditor.InputBindings.Add(new InputBinding(commandFind, new KeyGesture(Key.F, ModifierKeys.Control)));

            return contextMenu;
        }
        static public Action<Searcher> FindResultsRequired { get; set; }

        List<int> SearchResultsToIndices(Searcher searcher)
        {
            List<LocationSource> source = searcher.source;
            List<int> r = new List<int>();
            List<LocationSource> s = new List<LocationSource>();
            foreach (var c in source)
                if(c.Source == null || c.Source.FilePath == FileToLoad)
                s.Add(c);
            s = s.OrderBy(p => p.textSpan.Start).ToList();
            foreach (var c in s)
                r.Add(c.textSpan.Start);
            searcher.sources = s;
            
            return r;
        }
        public void ShowOffset(int offset)
        {
            textEditor.TextArea.Caret.Offset = offset;
            textEditor.TextArea.Caret.BringCaretToView();
        }
        public Searcher searcher { get; set; }
        public void FirstVisibleOffset(Searcher s)
        {
            var v = textEditor.TextArea.TextView.VisualLines;
            if (v.Count <= 0)
                return;
            int first = v[0].FirstDocumentLine.Offset;
            int last = v[v.Count - 1].LastDocumentLine.EndOffset;

            var b = s.Offsets.Where(d => d >= first).FirstOrDefault();
            s.Id/*Active*/ = b;

        }
        public Searcher Search(string texttofind, SearchDomain searchDomain = SearchDomain.file)
        {
            if (vs == null)
                return null;
            List<LocationSource> s = null;
            if(searchDomain == SearchDomain.solution)
                s = vs.FindString(texttofind, textEditor.Document.Text, FileToLoad, "", "", searchDomain);
            else
                s = vs.FindString(texttofind, textEditor.Document.Text, FileToLoad, "", "", searchDomain);

            searcher = new Searcher();
            searcher.texttofind = texttofind;
            searcher.source = s;
            searcher.Active = -1;
            searcher.editorWindow = this;

            if(s.Count <= 0)
            {
                r.searchResultIndices = null;
                textEditor.TextArea.InvalidateVisual();
                return null;
            }

            r.searchResultIndices = SearchResultsToIndices(searcher);
            searcher.Offsets = r.searchResultIndices;
            r.searchResultLength = texttofind.Length;

            FirstVisibleOffset(searcher);

            textEditor.TextArea.InvalidateVisual();

            return searcher;
        }
        public Searcher Search(Searcher searcher)
        {
            
            List<LocationSource> s = searcher.source;

            searcher.editorWindow = this;

            if (s.Count <= 0)
            {
                r.searchResultIndices = null;
                textEditor.TextArea.InvalidateVisual();
                return null;
            }

            r.searchResultIndices = SearchResultsToIndices(searcher);
            searcher.Offsets = r.searchResultIndices;
            r.searchResultLength = searcher.texttofind.Length;

            FirstVisibleOffset(searcher);

            textEditor.TextArea.InvalidateVisual();

            return searcher;
        }
        static SyntaxNode GetNode(SyntaxTree tree, int lineNumber)
        {
            var lineSpan = tree.GetText().Lines[lineNumber - 1].Span;
            return tree.GetRoot().DescendantNodes(null, true)
                .LastOrDefault(n => lineSpan.IntersectsWith(n.Span));
        }
        public string GetCurrentBlock()
        {

            string block = "";

            int line = textEditor.TextArea.Caret.Line;

            SyntaxTree s = vs.GetSyntaxTree(FileToLoad, textEditor.Document.Text, false);

            SyntaxNode node = GetNode(s, line);

            if (node == null)
                return "";

            var c = node.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().ToList();

            foreach(var b in c)
            {

                if(b is ConstructorDeclarationSyntax)
                {
                    ConstructorDeclarationSyntax d = b as ConstructorDeclarationSyntax;
                    if (block == "")
                        block = d.Identifier.ToString();
                    else block = d.Identifier.ToString() + "." + block;

                } else if(b is MethodDeclarationSyntax)
                {
                    MethodDeclarationSyntax d = b as MethodDeclarationSyntax;
                    if (block == "")
                        block = d.Identifier.ToString();
                    else block = d.Identifier.ToString() + "." + block;
                }
                else if(b is PropertyDeclarationSyntax)
                {
                    PropertyDeclarationSyntax d = b as PropertyDeclarationSyntax;
                    if (block == "")
                        block = d.Identifier.ToString();
                    else block = d.Identifier.ToString() + "." + block;

                }
                else if(b is FieldDeclarationSyntax)
                {
                    //FieldDeclarationSyntax d = b as FieldDeclarationSyntax;
                    //if (block == "")
                    //    block = d.Identifier.ToString();
                    //else block = d.Identifier.ToString() + "." + block;
                }
                else if(b is EventDeclarationSyntax)
                {
                    EventDeclarationSyntax d = b as EventDeclarationSyntax;
                    if (block == "")
                        block = d.Identifier.ToString();
                    else block = d.Identifier.ToString() + "." + block;
                }
                else if(b is DelegateDeclarationSyntax)
                {
                    DelegateDeclarationSyntax d = b as DelegateDeclarationSyntax;
                    if (block == "")
                        block = d.Identifier.ToString();
                    else block = d.Identifier.ToString() + "." + block;
                }
                else if(b is ClassDeclarationSyntax)
                {
                    ClassDeclarationSyntax d = b as ClassDeclarationSyntax;
                    if (block == "")
                        block = d.Identifier.ToString();
                    else block = d.Identifier.ToString() + "." + block;
                }
                else if (b is NamespaceDeclarationSyntax)
                {
                    NamespaceDeclarationSyntax d = b as NamespaceDeclarationSyntax;
                    if (block == "")
                        block = d.Name.ToString();
                    else block = d.Name.ToString() + "." + block;
                }
                
            }

            //var b = vs.GetSymbolAtLocation(offset, FileToLoad);
            //block = b.Name;
            //while(b != null)
            //{
            //    b = b.ContainingSymbol;
            //    if(b != null)
            //    {
            //        block += b.Name + "." + block;
            //    }
            //}
            
            return block;
        }

        public CommandFind commandFind { get; set; }

        public static TreeViewer treeViewer { get; set; }

        public static CallHierarchyViewer hierarchyViewer { get; set; }

        private void Item_Click_GoToDefinition(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() => { GetMemberOverCaret(); }));
        }

        private void Item_Click_CallHierarchy(object sender, RoutedEventArgs e)
        {
            ContextMenu.Visibility = Visibility.Collapsed;
            if (toolTip != null)
                toolTip.IsOpen = false;

            _worker = new BackgroundWorker();
            _worker.DoWork += CallHierarchy;

            _worker.RunWorkerAsync();
        }

        private BackgroundWorker _worker;

        private void Item_Click_FindReferences(object sender, RoutedEventArgs e)
        {
            ContextMenu.Visibility = Visibility.Collapsed;
            if (toolTip != null)
                toolTip.IsOpen = false;

            _worker = new BackgroundWorker();
            _worker.DoWork += FindReferences;

            _worker.RunWorkerAsync();
        }

        private void CallHierarchy(object sender, DoWorkEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                vs.OpenCallHierarchyTool("", null, "", null);

                hierarchyViewer.editorWindow = this;

                string word = Editor.GetWordUnderMouse(textEditor.Document, textEditor.TextArea.Caret.Position);

                if (string.IsNullOrEmpty(word))
                    return;

                var vp = editor.vs.GetProjectbyCompileItem(FileToLoad);

                Tuple<string, string, ISymbol> t = editor.GetType(textEditor);

                ISymbol s = t.Item3 as ISymbol;

                if (s == null)
                {
                    s = vs.GetSymbolAtLocation(vp, FileToLoad, textEditor.TextArea.Caret.Offset);
                    if (s == null)
                        return;
                }

                var r = editor.vs.GetAllSymbolReferences(s, FileToLoad, vp, References/* StringReplace.Replace(textEditor.Document.Text)*/).Result;

                if (r != null)
                {
                    List<ReferencedSymbol> list = r.ToList();

                    hierarchyViewer.LoadData(s, list, vs);
                }
            }));
        }

        private void FindReferences(object sender, DoWorkEventArgs e)
        {
            //ContextMenu.Visibility = Visibility.Hidden;
            //ContextMenu.InvalidateVisual();

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                vs.OpenReferencesTool("", null, "", null);

                treeViewer.editorWindow = this;

                string word = Editor.GetWordUnderMouse(textEditor.Document, textEditor.TextArea.Caret.Position);

                if (string.IsNullOrEmpty(word))
                    return;

                var vp = editor.vs.GetProjectbyCompileItem(FileToLoad);

                Tuple<string, string, ISymbol> t = editor.GetType(textEditor);

                ISymbol s = t.Item3 as ISymbol;

                if (s == null)
                {
                    s = vs.GetSymbolAtLocation(vp, FileToLoad, textEditor.TextArea.Caret.Offset);
                    if (s == null)
                        return;
                }

                var r = editor.vs.GetAllSymbolReferences(s, FileToLoad, vp, References/*, StringReplace.Replace(textEditor.Document.Text)*/).Result;

                if (r != null)
                {
                    List<ReferencedSymbol> list = r.ToList();

                    treeViewer.LoadData(s, list, vs);
                }
            }));
        }

        public static IEnumerable<ReferencedSymbol> FindReferences(int offset, string FileToLoad, VSSolution vs)
        {
            VSProject vp = vs.GetProjectbyCompileItem(FileToLoad);

            ISymbol s = vs.GetSymbolAtLocation(vp, FileToLoad, offset);
            if (s == null)
                return null;

            IEnumerable<ReferencedSymbol> r = vs.GetAllSymbolReferences(s, FileToLoad, vp).Result;

            if (r != null)
            {
                List<ReferencedSymbol> list = r.ToList();

                treeViewer.LoadData(s, list, vs);
            }

            return r;
        }

        public void OpenFile(SourceRefData data)
        {
            var vp = vs.GetProjectbyCompileItem(FileToLoad);
            vs.LoadFileFromProject("", vp, data.FileName, null, data.refs);
        }

        static private void Item_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("item clicked");
        }

        private void Item_Click_Copy(object sender, RoutedEventArgs e)
        {
            textEditor.Copy();
        }

        private void Item_Click_Paste(object sender, RoutedEventArgs e)
        {
            textEditor.Paste();
        }

        private void Item_Click_Cut(object sender, RoutedEventArgs e)
        {
            textEditor.Cut();
        }

        private EditorWindow embedEditorWindow { get; set; }

        private StackPanel stackPanel { get; set; }

        private StackPanel buttonPanel { get; set; }

        private Button filenameButton { get; set; }

        private double prevWidth = 0;

        private Tuple<int, int> GetWidthAfterTransform()
        {
            int left = 0;

            int allmargins = 0;

            var v = this.textEditor.TextArea.LeftMargins.OfType<FoldingMargin>().FirstOrDefault();

            if (v != null)
                left = (int)v.ActualWidth;

            foreach (FrameworkElement cc in this.textEditor.TextArea.LeftMargins)
            {
                allmargins += (int)cc.ActualWidth;
            }
            int margins = (int)((zoomScaleFactor) * (double)(allmargins - left));

            int width = (int)(zoomScaleFactor * ((double)(textEditor.TextArea.ActualWidth - allmargins + left)));

            return new Tuple<int, int>(width, margins);
        }

        private void Item_Click_PeekDefinition(object sender, RoutedEventArgs e)
        {
            double w = this.textEditor.TextArea.TextView.ActualWidth;
            double h = this.ActualHeight;

            prevWidth = textEditor.TextArea.ActualWidth;

            if (sender != null)
            {
                embedEditorWindow = new EditorWindow();
                embedEditorWindow.sourceEditor = this;
                embedEditorWindow.HideQuickClassBrowser();
                embedEditorWindow.HideLeftMargin();
                embedEditorWindow.HideZoom();
                embedEditorWindow.Width = w;
                embedEditorWindow.Height = h - 240;
                embedEditorWindow.Visibility = Visibility.Visible;
                embedEditorWindow.textEditor.IsReadOnly = true;
                embedEditorWindow.Margin = new Thickness(0, 1, 0, 0);
                embedEditorWindow.BorderThickness = new Thickness(0, 1, 0, 1);
                embedEditorWindow.BorderBrush = Brushes.Gray;
                embedEditorWindow.textEditor.Background = Brushes.LemonChiffon;
                embedEditorWindow.Opacity = 1;
                embedEditorWindow.HorizontalAlignment = HorizontalAlignment.Stretch;
                embedEditorWindow.VerticalAlignment = VerticalAlignment.Stretch;
                if (embedEditorWindow.breakPointMargin != null)
                    embedEditorWindow.breakPointMargin.pen = new Pen(Brushes.LightBlue, 4);
                embedEditorWindow.lineNumberMarginExtended.background = Brushes.LemonChiffon;

                embedEditorWindow.textEditor.KeyDown += TextEditor_KeyDown1;

                stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Background = Brushes.Transparent;
                stackPanel.Opacity = 1;

                StackPanel b = new StackPanel();
                b.Orientation = Orientation.Horizontal;
                b.Background = Brushes.White;
                b.Height = 18;
                b.Opacity = 1;
                b.HorizontalAlignment = HorizontalAlignment.Right;

                StackPanel button = new StackPanel();
                button.Orientation = Orientation.Horizontal;
                button.Background = Brushes.Gold;
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.Height = 18;
                Button lb = new Button();
                lb.Height = 16;
                lb.FontSize = 11.5;
                lb.Content = "File";
                lb.HorizontalAlignment = HorizontalAlignment.Left;
                lb.MinWidth = 100;
                lb.Background = Brushes.Gold;
                button.Children.Add(lb);
                filenameButton = lb;
                Image image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.PromoteTab_16x);
                image.Height = 16;
                image.Width = 16;
                button.Children.Add(image);
                image = new Image();
                image.Source = ConvertBitmapToBitmapImage.Convert(WinExplorers.ve.Close_16x);
                image.Height = 16;
                image.Width = 16;
                image.MouseDown += PeekDefinition_Close_MouseDown;
                button.Children.Add(image);

                buttonPanel = button;

                b.Children.Add(button);
                stackPanel.Children.Add(b);
                stackPanel.Children.Add(embedEditorWindow);
            }

            int left = 0;

            int allmargins = 0;

            var v = this.textEditor.TextArea.LeftMargins.OfType<FoldingMargin>().FirstOrDefault();

            if (v != null)
                left = (int)v.ActualWidth;

            foreach (FrameworkElement cc in this.textEditor.TextArea.LeftMargins)
            {
                allmargins += (int)cc.ActualWidth;
            }
            Point point = new Point(allmargins - left, 0);
            point = this.textEditor.TextArea.LayoutTransform.Transform(point);
            int margins = (int)((zoomScaleFactor) * (double)(allmargins - left));
            int width = (int)((zoomScaleFactor) * (double)(textEditor.TextArea.ActualWidth - allmargins + left));
            if (sender != null)
            {
                popup = new Popups();
                popup.Content = stackPanel;
                popup.LayoutTransform = new ScaleTransform(1.0 / zoomScaleFactor, 1.0 / zoomScaleFactor);
                embedEditorWindow.textEditor.TextArea.LayoutTransform = textEditor.TextArea.LayoutTransform;
                popup.Element_SizeChanged();
                popup.KeyDown += Popup_KeyDown;
                elementGenerator.frameworkElement = popup;
            }
            else
            {
                popup.Height = /*this.ActualHeight + popupHeightPoint.X;// */h - 200 + 2;
                popup.Width = width - 15;// textEditor.TextArea.ActualWidth - margins; /*- allmargins + left*/;
            }
            popup.editorWindow = this;
            popup.LoadHandlers();

            embedEditorWindow.Width = width;// textEditor.TextArea.ActualWidth - margins;// - allmargins + left;

            if (sender != null)
            {
                embedEditorWindow.textEditor.IsKeyboardFocusWithinChanged += TextEditor_IsKeyboardFocusWithinChanged;
                embedEditorWindow.breakPointMargin.background = Brushes.LightGoldenrodYellow;
                ((FrameworkElement)embedEditorWindow.breakPointMargin).InvalidateVisual();
                PeekDefinition_GetMemberOverCaret();
            }
        }

        public Size GetPopupSize()
        {
            Size size = new Size();

            int left = 0;

            int allmargins = 0;

            double h = this.ActualHeight;

            var v = this.textEditor.TextArea.LeftMargins.OfType<FoldingMargin>().FirstOrDefault();

            if (v != null)
                left = (int)v.ActualWidth;

            foreach (FrameworkElement cc in this.textEditor.TextArea.LeftMargins)
            {
                allmargins += (int)cc.ActualWidth;
            }
            if (h <= 0)
                return size;
            size.Height = h - 200 + 2;
            size.Width = (int)(zoomScaleFactor * (textEditor.TextArea.ActualWidth - allmargins + left));

            embedEditorWindow.Width = zoomScaleFactor * (textEditor.TextArea.ActualWidth - allmargins + left) - 35;

            return size;
        }

        private void PeekDefinition_Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (popup == null)
                return;

            popup.Visibility = Visibility.Hidden;

            popup.Visibility = Visibility.Collapsed;
            embedEditorWindow.Visibility = Visibility.Collapsed;
            popup = null;
        }

        private void TextEditor_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                embedEditorWindow.textEditor.Background = Brushes.LightGray;
                embedEditorWindow.breakPointMargin.background = Brushes.LightGray;
                ((FrameworkElement)embedEditorWindow.breakPointMargin).InvalidateVisual();
                embedEditorWindow.lineNumberMarginExtended.background = Brushes.LightGray;
                buttonPanel.Background = Brushes.LightGray;
                filenameButton.Background = Brushes.LightGray;
                embedEditorWindow.lineNumberMarginExtended.InvalidateVisual();
            }
            else
            {
                if (popup != null)
                    if (popup.Visibility != Visibility.Visible)
                        popup.Visibility = Visibility.Visible;
                embedEditorWindow.textEditor.Background = Brushes.LemonChiffon;
                embedEditorWindow.breakPointMargin.background = Brushes.LemonChiffon;
                ((FrameworkElement)embedEditorWindow.breakPointMargin).InvalidateVisual();
                embedEditorWindow.lineNumberMarginExtended.background = Brushes.LemonChiffon;
                buttonPanel.Background = Brushes.Gold;
                filenameButton.Background = Brushes.Gold;
                embedEditorWindow.lineNumberMarginExtended.InvalidateVisual();
            }
            textEditor.InvalidateVisual();
        }

        private void TextEditor_KeyDown1(object sender, KeyEventArgs e)
        {
            if (popup == null)
                return;
            if (e.Key == Key.Escape)
            {
                RemoveVirtualLines();
                popup.Visibility = Visibility.Hidden;
                //popup.StaysOpen = false;
                popup.Visibility = Visibility.Collapsed;
                embedEditorWindow.Visibility = Visibility.Collapsed;
                popup = null;
                elementGenerator.LineOfInterest = 0;
                textEditor.TextArea.TextView.Redraw();
            }
        }

        private Popups popup = null;

        private void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            if (popup == null)
                return;
            if (e.Key == Key.Escape)
            {
                RemoveVirtualLines();
                popup.Visibility = Visibility.Hidden;
                //popup.StaysOpen = false;
                popup.Visibility = Visibility.Collapsed;
                embedEditorWindow.Visibility = Visibility.Collapsed;
                popup = null;
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

            textEditor.Document.FileName = filename;
        }

        public static Action<EditorWindow> OnModified { get; set; }

        public void Save()
        {
            textEditor.Save(FileToLoad);
            modified = false;
        }

        private bool areTypesLoaded = false;

        private System.Windows.Threading.DispatcherTimer eTimer = new System.Windows.Threading.DispatcherTimer();// new System.Timers.Timer(1000);

        private void OnTimedEvent(object source, /*Elapsed*/EventArgs e)
        {
            if (shouldUpdate == true)
            {
                //LoadReferences(vs);
                if (vs != null)
                    this.qcb.Load(vs, FileToLoad, textEditor.Document.Text, false);
                shouldUpdate = false;
            }

            if (textEditor.TextArea.IsVisible)
            {
                if (qcb.CaretPositionAdjustNeeded)
                {
                    int start = textEditor.TextArea.Caret.Offset;

                    int length = 1;
                    LinePosition se = new LinePosition(textEditor.TextArea.Caret.Line, 0);
                    LinePosition ee = new LinePosition(textEditor.TextArea.Caret.Line, 1);
                    Location b = Location.Create("", new Microsoft.CodeAnalysis.Text.TextSpan(start, length), new Microsoft.CodeAnalysis.Text.LinePositionSpan(se, ee));
                    this.qcb.SelectItemAtCaretPosition(b);
                    qcb.CaretPositionAdjustNeeded = false;
                }
                VerifyReferences();
            }

            if (!textEditor.TextArea.IsFocused)
            {
                if (toolTip != null)
                    toolTip.IsOpen = false;

                return;
            }

            if (modified) OnModified.Invoke(this);

            //int end =

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

            //if (Errors.Count > 0)
            //    textEditor.TextArea.InvalidateVisual();

            Errors = editor.vs.GetParserErrors(editor.vp, FileToLoad, StringReplace.Replace(textEditor.Document.Text));

            AddDiagnosticErrors(Errors);

            Handled = false;
        }

        public List<Diagnostic> Errors = new List<Diagnostic>();

        private void TextEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
                Handled = true;

            //        if (e.Key == Key.Tab)
            //        {
            //            var loopCounter = new SnippetReplaceableTextElement { Text = "i" };
            //            Snippet snippet = new Snippet
            //            {
            //                Elements = {
            //    new SnippetTextElement { Text = "for(int " },
            //    new SnippetBoundElement { TargetElement = loopCounter },
            //    new SnippetTextElement { Text = " = " },
            //    new SnippetReplaceableTextElement { Text = "0" },
            //    new SnippetTextElement { Text = "; " },
            //    loopCounter,
            //    new SnippetTextElement { Text = " < " },
            //    new SnippetReplaceableTextElement { Text = "end" },
            //    new SnippetTextElement { Text = "; " },
            //    new SnippetBoundElement { TargetElement = loopCounter },
            //    new SnippetTextElement { Text = "++) { \t" },
            //    new SnippetCaretElement(),
            //    new SnippetTextElement { Text = " }" }
            //}
            //            };
            //            snippet.Insert(textEditor.TextArea);
            //        }
        }

        private string Get(string s)
        {
            return "success";
        }

        async private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                await References();
            }
            else
            if (e.Key == Key.F2)
            {
                if (vs == null)
                    return;
                string infos = "";
                string word = Editor.GetWordUnderMouse(textEditor.Document, textEditor.CaretOffset);
                var c = vs.FindString(word, textEditor.Document.Text, FileToLoad, "", "", SearchDomain.solution);
                var d = vs.FindReferences(textEditor.CaretOffset, FileToLoad);
                int p = 0;
                foreach (LocationSource b in c)
                {
                    string cc = "";
                    ISymbol s = vs.GetSymbolAtLocation(b.textSpan.Start, b.Source.FilePath);
                    if (s == null)
                        continue;
                    cc = s.Name;
                    IMethodSymbol method = s as IMethodSymbol;
                    if (method != null)
                    {
                        if (method.ContainingType != null)
                        {
                            cc += " - " + method.ContainingType.Name;
                        }
                    }
                    infos += cc + "\n";
                }
                int f = 0;
                foreach (var b in d)
                {
                    if (b.Locations != null)
                        f += b.Locations.Count();
                }
                MessageBox.Show(f + " - references, " + c.Count + " - found in solution files");
            }
            else
            if (e.Key == Key.Escape)
            {
                if (popup == null)
                    return;
                {
                    RemoveVirtualLines();
                    popup.Visibility = Visibility.Hidden;
                    if (embedEditorWindow != null)
                        embedEditorWindow.Visibility = Visibility.Hidden;
                    elementGenerator.LineOfInterest = 0;
                    textEditor.TextArea.TextView.Redraw();
                }
            }
            else if (e.Key == Key.F8)
                AddMarkerFromSelectionClick(null, null);
            else
            if (e.Key == Key.F12)
            {
                this.Dispatcher.BeginInvoke(new Action(() => { GetMemberOverCaret(); }));
            }
            //Handled = true;
        }

        public void Deactivate()
        {
            if (toolTip != null)
                toolTip.IsOpen = false;
        }

        public void GetMemberOverCaret()
        {
            Tuple<string, string, ISymbol> t = editor.GetType(textEditor);
            if (vs == null)
                return;
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                object obs = new object();
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    vs.LoadFileFromContent(t.Item1, null, t.Item2, t.Item3);
                    //Task.Factory.StartNew(() =>
                    // {
                    //this.Dispatcher.BeginInvoke(new Action(() =>
                    // {
                    if (this.foldingStrategy is CommentAndMethodFoldingStrategy)
                    {
                        Thread.Sleep(2000);
                        lock (obs)
                        {
                            this.SetAllFoldings(true);
                            ((CommentAndMethodFoldingStrategy)this.foldingStrategy).UpdateFoldings(this.foldingManager, this.textEditor.Document);
                            //else if (this.foldingStrategy is BraceFoldingStrategy)
                            //    ((BraceFoldingStrategy)this.foldingStrategy).UpdateFoldings(this.foldingManager, this.textEditor.Document);
                            this.InvalidateVisual();
                        }
                    }
                    // }));
                    // });
                }));
            }));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = false;
            newWindowThread.Start();
        }

        private string PeekDefinition_Content { get; set; }

        private string PeekDefinition_FileName { get; set; }

        private Tuple<string, string, ISymbol> PeekDefinition_Tuple { get; set; }

        public void PeekDefinition_GetMemberOverCaret()
        {
            Tuple<string, string, ISymbol> t = editor.GetType(textEditor);
            PeekDefinition_Tuple = t;
            if (vs == null)
                return;
            if (!string.IsNullOrEmpty(t.Item1))
            {
                popup.Visibility = Visibility.Visible;
                PeekDefinition_Content = t.Item1;
                embedEditorWindow.LoadContent(PeekDefinition_Content);
                embedEditorWindow.vs = vs;
                embedEditorWindow.LoadFromProject(vs, true, t.Item3);
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);
                    embedEditorWindow.Dispatcher.BeginInvoke(new Action(() =>

                    {
                        embedEditorWindow.SetAllFoldings(true);
                        ((CommentAndMethodFoldingStrategy)embedEditorWindow.foldingStrategy).UpdateFoldings(embedEditorWindow.foldingManager, embedEditorWindow.textEditor.Document);
                        embedEditorWindow.InvalidateVisual();
                    }));
                });
                string name = "";
                if (t.Item3 != null)
                    name = t.Item3.Name;
                filenameButton.Content = name + " [from metadata]";
            }
            else if (!string.IsNullOrEmpty(t.Item2))
            {
                popup.Visibility = Visibility.Visible;

                PeekDefinition_FileName = t.Item2;
                embedEditorWindow.LoadFile(PeekDefinition_FileName);
                embedEditorWindow.vs = vs;
                embedEditorWindow.LoadFromProject(vs, true, t.Item3);
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2000);
                    embedEditorWindow.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //embedEditorWindow.SetAllFoldings(true);
                        //((BraceFoldingStrategy)embedEditorWindow.foldingStrategy).UpdateFoldings(embedEditorWindow.foldingManager, embedEditorWindow.textEditor.Document);
                        //embedEditorWindow.InvalidateVisual();
                    }));
                });
                filenameButton.Content = System.IO.Path.GetFileName(PeekDefinition_FileName);
                popup.InvalidateVisual();
            }
            else
            {
                if (popup == null)
                    return;

                popup.Visibility = Visibility.Hidden;
                //popup.StaysOpen = false;
                popup.Visibility = Visibility.Collapsed;
                embedEditorWindow.Visibility = Visibility.Collapsed;
                popup = null;
            }
            int Line = this.textEditor.TextArea.Caret.Line;
            if (Line < textEditor.Document.Lines.Count - 2)
                Line++;
            int c = 1;
            int offset = textEditor.Document.GetLineByNumber(Line).Offset;
            int i = 0;
            while (i < c)
            {
                textEditor.Document.Insert(offset, " \n", "");
                i++;
            }
            elementGenerator.LineOfInterest = Line;
            if (popup != null)
                popup.Visibility = Visibility.Visible;
            textEditor.TextArea.TextView.Redraw();
        }

        public EditorWindow sourceEditor = null;

        private void RemoveVirtualLines()
        {
            if (embedEditorWindow.sourceEditor == null)
                return;
            List<DocumentLine> Lines = embedEditorWindow.sourceEditor.textEditor.Document.Lines.ToList();
            Lines.Reverse();

            foreach (DocumentLine d in Lines)
                if (d.obs is string)
                    embedEditorWindow.sourceEditor.textEditor.Document.Remove(d.Offset, d.Length + d.DelimiterLength);
            embedEditorWindow.sourceEditor.InvalidateVisual();
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

        public void LoadProjectTypes(List<INamespaceOrTypeSymbol> symbols = null)
        {
            symbols = editor.typesofproject;

            if (symbols == null)
                return;
            DocumentLine currentLine = null;
            //textEditor.Dispatcher.BeginInvoke(new Action(() => { currentLine = textEditor.Document.GetLineByOffset(textEditor.CaretOffset); }));
            List<string> ns = new List<string>();
            StringBuilder b = new StringBuilder();
            StringBuilder n = new StringBuilder();
            foreach (var cs in symbols)
            {
                if (cs.IsNamespace)
                    continue;
                if (cs.IsType)
                {
                    // b.Append("|" + cs.Name);
                    int hash = cs.Name.GetHashCode();
                    sc.Add(hash);
                    if (!dc.ContainsKey(hash))
                        dc.Add(hash, cs.Name);
                }
                if (!ns.Contains(cs.ContainingNamespace.Name))
                {
                    n.Append("|" + cs.ContainingNamespace.Name + ".");
                    ns.Add(cs.ContainingNamespace.Name);
                }
            }

            HighlightingRule hs = new HighlightingRule();

            string ts = b.ToString();

            ts = ts.Replace("?", string.Empty);
            ts = ts.Replace("$", string.Empty);
            ts = ts.Replace("*", string.Empty);
            ts = ts.Replace("_", string.Empty);

            b.Clear();

            n.Insert(0, "\\b(?>thises");
            b.Append(ts);
            n.Append(ts);
            b.Append(")\\b");
            n.Append(")\\b");

            ts = b.ToString();

            //hs.Regex = new System.Text.RegularExpressions.Regex(ts);

            HighlightingColor c = new HighlightingColor();
            c.Foreground = new SimpleHighlightingBrush(System.Windows.Media.Color.FromRgb(111, 196, 220));
            hs.Color = c;

            hs.Regex = new System.Text.RegularExpressions.Regex(n.ToString());

            c = new HighlightingColor();
            c.Foreground = new SimpleHighlightingBrush(System.Windows.Media.Colors.Gray);
            hs.Color = c;

            textEditor.Dispatcher.BeginInvoke(new Action(() =>
            {
                textEditor.SyntaxHighlighting.MainRuleSet.Rules.Add(hs);
                //textEditor.CaretOffset = currentLine.Offset;
                textEditor.TextArea.Caret.BringCaretToView();
                GetTypesNames();
            }));
        }

        public void LoadProjectTypesForUpdate(HighlightingRule hs)
        {
            List<INamespaceOrTypeSymbol> symbols = editor.typesofproject;

            if (symbols == null)
                return;
            DocumentLine currentLine = null;
            //textEditor.Dispatcher.BeginInvoke(new Action(() => { currentLine = textEditor.Document.GetLineByOffset(textEditor.CaretOffset); }));
            List<string> ns = new List<string>();
            StringBuilder b = new StringBuilder();
            StringBuilder n = new StringBuilder();
            foreach (var cs in symbols)
            {
                if (cs.IsNamespace)
                    continue;
                if (cs.IsType)
                {
                    // b.Append("|" + cs.Name);
                    int hash = cs.Name.GetHashCode();
                    sc.Add(hash);
                    if (!dc.ContainsKey(hash))
                        dc.Add(hash, cs.Name);
                }
                if (!ns.Contains(cs.ContainingNamespace.Name))
                {
                    n.Append("|" + cs.ContainingNamespace.Name + ".");
                    ns.Add(cs.ContainingNamespace.Name);
                }
            }

            string ts = b.ToString();

            ts = ts.Replace("?", string.Empty);
            ts = ts.Replace("$", string.Empty);
            ts = ts.Replace("*", string.Empty);
            ts = ts.Replace("_", string.Empty);

            b.Clear();

            n.Insert(0, "\\b(?>thises");
            b.Append(ts);
            n.Append(ts);
            b.Append(")\\b");
            n.Append(")\\b");

            ts = b.ToString();

            //hs.Regex = new System.Text.RegularExpressions.Regex(ts);

            HighlightingColor c = new HighlightingColor();
            c.Foreground = new SimpleHighlightingBrush(System.Windows.Media.Color.FromRgb(111, 196, 220));
            hs.Color = c;

            //hsd = new HighlightingRule();

            hs.Regex = new System.Text.RegularExpressions.Regex(n.ToString());

            c = new HighlightingColor();
            c.Foreground = new SimpleHighlightingBrush(System.Windows.Media.Colors.Gray);
            hs.Color = c;
        }

        private ITextMarkerService textMarkerService;

        private void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            IServiceContainer services = (IServiceContainer)textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            this.textMarkerService = textMarkerService;
        }

        private void RemoveAllClick(object sender, RoutedEventArgs e)
        {
            List<ITextMarker> ns = new List<ITextMarker>();
            foreach (var c in textMarkerService.TextMarkers)
            {
                ns.Add(c);
            }
            textMarkerService.RemoveAll(m => true);
            foreach (var c in ns)
            {
                c.MarkerColor = Colors.Transparent;
                var d = textMarkerService.Create(c.StartOffset, c.Length);
                d.MarkerColor = Colors.White;
            }
            textMarkerService.RemoveAll(m => true);
        }

        private void RemoveSelectedClick(object sender, RoutedEventArgs e)
        {
            textMarkerService.RemoveAll(IsSelected);
        }

        private void AddMarkerFromSelectionClick(object sender, RoutedEventArgs e)
        {
            ITextMarker marker = textMarkerService.Create(textEditor.SelectionStart, textEditor.SelectionLength);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
        }

        private void AddDiagnosticErrors(List<Diagnostic> errors)
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
                if (d.Severity == DiagnosticSeverity.Error)
                    marker.MarkerColor = Colors.Red;
                else
                    marker.MarkerColor = Colors.Green;
            }
            vs.LoadErrors(null, Errors, FileToLoad);
        }

        private bool IsSelected(ITextMarker marker)
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

        public LineNumberMargin lineNumberMarginExtended { get; set; }

        public BreakPointMargin breakPointMargin { get; set; }

        private BreakPointMargin bl { get; set; }

        public void HideLeftMargin()
        {
            textEditor.TextArea.LeftMargins.Remove(bl);
            textEditor.InvalidateVisual();
        }

        public void HideAllMargin()
        {
            textEditor.TextArea.LeftMargins.Clear();
            textEditor.InvalidateVisual();
        }

        public void LoadMargins()
        {
            bl = new BreakPointMargin();
            bl.Margin = new Thickness(0, 0, 0, 0);
            bl.background = System.Windows.Media.Brushes.LightGray;
            bl.MouseDown += Bl_MouseDown;

            LineNumberMargin b = new LineNumberMargin();
            b.Margin = new Thickness(5, 0, 0, 0);
            b.SetValue(Control.ForegroundProperty, new SolidColorBrush(Color.FromArgb(200, 111, 196, 220)));
            lineNumberMarginExtended = b;

            BreakPointMargin br = new BreakPointMargin();
            br.Margin = new Thickness(0, 0, 0, 0);
            br.pen = new System.Windows.Media.Pen(System.Windows.Media.Brushes.LightGreen, 3);
            br.margin = 25;
            breakPointMargin = br;

            textEditor.TextArea.LeftMargins.Add(bl);
            textEditor.TextArea.LeftMargins.Add(b);
            textEditor.TextArea.LeftMargins.Add(br);
            textEditor.TextArea.TextView.Margin = new Thickness(1, 0, 0, 0);
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
                if (offset < 0)
                    return;
                if (offset >= textEditor.Document.TextLength) offset = textEditor.Document.TextLength - 1;
                if (offset < 0)
                    return;
                var textAtOffset = textEditor.Document.GetText(offset, 1);

                if (textAtOffset == "\n")
                {
                    if (toolTip != null)
                        toolTip.IsOpen = false;
                    return;
                }

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

        private int CaretCurrentLine = 0;

        public bool EnableCaretMoveOverObject = false;

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            if (qcb.jumpOnSelectionChanged)
                qcb.jumpOnSelectionChanged = false;
            else qcb.CaretPositionAdjustNeeded = true;
            var v = textEditor.TextArea.TextView;
            var pos = textEditor.TextArea.Caret.Position;
            var cs = this.textEditor.Document.GetLineByOffset(textEditor.TextArea.Caret.Offset);
            func.Invoke(cs.LineNumberExtended, pos.Column, pos.Column, FileToLoad);
            if (pos != null)
            {
                var line = pos.Line;
                var column = pos.Column;
                string wordHover = string.Empty;
                var offset = textEditor.Document.GetOffset(line, column);
                if (offset >= 0)
                {
                    DocumentLine gv = textEditor.Document.GetLineByNumber(line);
                    if (EnableCaretMoveOverObject == false)
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

                    if (offset <= 0)
                        return;
                    if (offset >= textEditor.Document.TextLength)

                        offset = textEditor.Document.TextLength - 1;
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

        private async void TextView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (toolTip != null)
                    toolTip.IsOpen = false;
                ContextMenu.Visibility = Visibility.Visible;
                ContextMenu.PlacementTarget = textEditor.TextArea.TextView;
                ContextMenu.IsOpen = true;
                return;
            }

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
                            string FileName = ProjectToLoad;

                            string filename = FileToLoad;

                            editor.vp = editor.vs.GetProjectbyFileName(FileName);

                            ISymbol s = gv.obs as ISymbol;

                            if (s.Locations[0].GetLineSpan().StartLinePosition.Line != line + 1)
                            {
                                //     await References_Update();
                                await References();
                                gv = textEditor.Document.GetLineByNumber(line);
                                s = gv.obs as ISymbol;
                            }

                            var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp, References/*, StringReplace.Replace(textEditor.Document.Text)*/).Result;
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

                                bool? result = vs.ShowDialog();

                                if (result == null || result == false)
                                    return;

                                this.Dispatcher.BeginInvoke(new Action(() => { editor.vs.LoadFileFromProject("", null, vs.SelectedLinkItem); }));

                                vs.Close();
                            }
                        }
                }
            }

            if (pos != null)
            {
                CaretCurrentLine = ((TextViewPosition)pos).Location.Line;

                textEditor.TextArea.Caret.Position = (TextViewPosition)pos;
            }
            v.InvalidateVisual();

            r.shouldHighlight = false;
        }

        public int GetReferenceNumber(int line, VSProject vp)
        {
            {
                DocumentLine gv = textEditor.Document.GetLineByNumber(line);
                if (gv.obs != null)
                    if (gv.obs is ISymbol)
                    {
                        string FileName = ProjectToLoad;

                        string filename = FileToLoad;

                        editor.vp = vp;// editor.vs.GetProjectbyFileName(FileName);

                        ISymbol s = gv.obs as ISymbol;
                        var r = editor.vs.GetAllSymbolReferences(s, filename, vp/*, StringReplace.Replace(textEditor.Document.Text)*/).Result;

                        if (r == null)
                            return 0;

                        int n = 0;
                        foreach (ReferencedSymbol b in r)
                        {
                            int cc = b.Locations.Count();
                            if (cc == 0)
                                n += 0;
                            else n += cc;
                        }

                        return n;
                    }
                return 0;
            }
        }

        private BackgroundRenderer r { get; set; }

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

        private CompletionWindow completionWindow;

        private CompletionControls completionControl { get; set; }

        async public Task<int> UpdateReferences(List<int> mb, Dictionary<int, ISymbol> objects, bool shoulupdate = true)
        {
            string d = textEditor.Document.Text;

            List<DocumentLine> ns = new List<DocumentLine>();
            int i = 0;
            foreach (int v in mb)
            {
                int g = textEditor.Document.GetLineByOffset(v).Offset;

                var c = textEditor.Document.GetLineByOffset(v);

                string s = textEditor.Document.Text.Substring(c.Offset, c.Length);

                textEditor.Document.Insert(g, "  References -          \n", objects[v]);

                ns.Add(textEditor.Document.GetLineByOffset(g));
            }

            foreach (var b in ns)
            {
                textEditor.Document.Update(b);
            }
            if (shoulupdate)
                vs.UpdateSyntaxTree(FileToLoad, StringReplace.Replace(textEditor.Document.Text));

            await this.Dispatcher.BeginInvoke(new Action(() =>
            {
                List<int> offsets = new List<int>();

                Dictionary<int, int> data = new Dictionary<int, int>();

                VSProject vp = vs.GetProjectbyCompileItem(FileToLoad);

                for (int j = 1; j <= textEditor.Document.Lines.Count; j++)
                {
                    var c = textEditor.Document.GetLineByNumber(j);

                    int line = c.LineNumber;

                    int g = c.Offset;

                    if (c.obs == null)
                        continue;

                    int n = GetReferenceNumber(line, vp);

                    offsets.Add(line);
                    data.Add(line, n);

                    //textEditor.Document.Text = textEditor.Document.Text.Remove(g + 15, 5).Insert(g + 15, n.ToString().PadRight(5));

                    //vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
                }

                offsets.Sort();

                offsets.Reverse();

                string content = textEditor.Document.Text;

                foreach (int v in offsets)
                {
                    var c = textEditor.Document.GetLineByNumber(v);

                    var obs = c.obs;

                    int n = data[v];

                    int g = c.Offset;

                    var cc = textEditor.Document.GetLineByNumber(v);

                    string cs = textEditor.Document.Text.Substring(cc.Offset, cc.Length);

                    cs = cs.Insert(15, n.ToString().PadRight(5));

                    textEditor.Document.Remove(g, cc.Length);
                    textEditor.Document.Insert(g, cs, cc.obs);
                }

                vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
            }));

            return 0;
        }

        public void References_Update(List<int> offsets, Dictionary<int, int> data, ICSharpCode.AvalonEdit.Document.TextDocument document)
        {
            string content = document.Text;

            foreach (int v in offsets)
            {
                var c = document.GetLineByNumber(v);

                var obs = c.obs;

                int n = data[v];

                int g = c.Offset;

                var cc = document.GetLineByNumber(v);

                string cs = document.Text.Substring(cc.Offset, cc.Length);
                if (cc.Length > 15)
                    cs = cs.Remove(15, cc.Length - 15);
                cs = cs.Insert(15, n.ToString().PadRight(5));

                document.Remove(g, cc.Length);
                document.Insert(g, cs, cc.obs);
            }

            vs.UpdateSyntaxTree(FileToLoad, document.Text);
        }

        public void References_Insert(ICSharpCode.AvalonEdit.Document.TextDocument document, List<int> mb, Dictionary<int, ISymbol> objects, List<int> offsets, Dictionary<int, int> data, bool shoulupdate = true)
        {
            string d = document.Text;// textEditor.Document.Text;

            List<DocumentLine> ns = new List<DocumentLine>();
            int i = 0;
            foreach (int v in mb)
            {
                int g = document.GetLineByOffset(v).Offset;

                var c = document.GetLineByOffset(v);

                var prev = c.PreviousLine;

                string prevText = document.Text.Substring(prev.Offset, prev.Length);

                string s = document.Text.Substring(c.Offset, c.Length);

                if (!prevText.Contains("References"))
                {
                    //document.Insert(g, "//References -          \n", objects[v]);
                    document.Insert(g, "//References -  " + data[v].ToString() + "        \n", objects[v]);
                }
                else
                {
                    var obs = vs.GetSymbolAtLocation(v, FileToLoad);
                    objects[v] = obs;
                    prev.obs = obs;
                }
                ns.Add(document.GetLineByOffset(g));
            }
            foreach (var b in ns)
            {
                document.Update(b);
            }
        }

        public void References_Insert(ICSharpCode.AvalonEdit.Document.TextDocument document, List<int> mb, Dictionary<int, ISymbol> objects)
        {
            string d = document.Text;// textEditor.Document.Text;

            List<DocumentLine> ns = new List<DocumentLine>();
            int i = 0;
            foreach (int v in mb)
            {
                int g = document.GetLineByOffset(v).Offset;

                var c = document.GetLineByOffset(v);

                var prev = c.PreviousLine;

                string prevText = document.Text.Substring(prev.Offset, prev.Length);

                string s = document.Text.Substring(c.Offset, c.Length);

                if (!prevText.Contains("References"))
                {
                    //document.Insert(g, "//References -          \n", objects[v]);
                    //document.Insert(g, "//References -  " + data[v].ToString() + "        \n", objects[v]);
                }
                else
                {
                    var obs = vs.GetSymbolAtLocation(FileToLoad, v, d);
                    objects[v] = obs;
                    prev.obs = obs;
                    document.Replace(prev.Offset, 2, "  ", obs);
                }
                ns.Add(document.GetLineByOffset(g));
            }
            foreach (var b in ns)
            {
                document.Update(b);
            }
        }

        async public Task<int> UpdateReferenceses(List<int> mb, Dictionary<int, ISymbol> objects, bool shoulupdate = true)
        {
            string d = textEditor.Document.Text;

            List<DocumentLine> ns = new List<DocumentLine>();
            int i = 0;
            foreach (int v in mb)
            {
                int g = textEditor.Document.GetLineByOffset(v).Offset;

                var c = textEditor.Document.GetLineByOffset(v);

                string s = textEditor.Document.Text.Substring(c.Offset, c.Length);

                textEditor.Document.Insert(g, "  References -          \n", objects[v]);

                ns.Add(textEditor.Document.GetLineByOffset(g));
            }
            foreach (var b in ns)
            {
                textEditor.Document.Update(b);
            }
            if (shoulupdate)
                vs.UpdateSyntaxTree(FileToLoad, StringReplace.Replace(textEditor.Document.Text));

            //this.Dispatcher.BeginInvoke(new Action(() => {
            ICSharpCode.AvalonEdit.Document.TextDocument document = textEditor.Document;

            VSProject vp = vs.GetProjectbyCompileItem(FileToLoad);

            List<int> offsets = new List<int>();
            Dictionary<int, int> data = new Dictionary<int, int>();
            for (int j = 1; j <= textEditor.Document.Lines.Count; j++)
            {
                //var c = textEditor.Document.GetLineByNumber(j);

                var c = document.GetLineByNumber(j);

                int line = c.LineNumber;

                int g = c.Offset;

                if (c.obs == null)
                    continue;
                int n = GetReferenceNumber(line, vp);

                offsets.Add(line);
                data.Add(line, n);

                //textEditor.Document.Text = textEditor.Document.Text.Remove(g + 15, 5).Insert(g + 15, n.ToString().PadRight(5));

                //vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
            }

            offsets.Sort();

            offsets.Reverse();

            string content = textEditor.Document.Text;
            await this.Dispatcher.BeginInvoke(new Action(async () =>
            {
                foreach (int v in offsets)
                {
                    if (v > textEditor.Document.LineCount)
                        continue;

                    var c = textEditor.Document.GetLineByNumber(v);

                    var obs = c.obs;

                    int n = data[v];

                    int g = c.Offset;

                    var cc = textEditor.Document.GetLineByNumber(v);

                    string cs = textEditor.Document.Text.Substring(cc.Offset, cc.Length);

                    cs = cs.Insert(15, n.ToString().PadRight(5));

                    textEditor.Document.Remove(g, cc.Length);
                    textEditor.Document.Insert(g, cs, cc.obs);
                }
                vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
            }));

            return 0;
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
            foreach (DocumentLine dd in textEditor.Document.Lines)

            //foreach (var b in ns)
            {
                textEditor.Document.Update(dd); // b
            }
            VSProject vp = vs.GetProjectbyCompileItem(FileToLoad);
            var t = Task.Run(async delegate
            {
                foreach (int v in objects.Keys)
                {
                    int g = textEditor.Document.GetLineByOffset(v).Offset;

                    int line = textEditor.Document.GetLineByOffset(v).LineNumber;

                    int n = GetReferenceNumber(line, vp);

                    textEditor.Document.Text = textEditor.Document.Text.Remove(g + 10, 5).Insert(g + 10, n.ToString().PadRight(5));
                }
            });
        }

        public bool shouldUpdate = false;

        public async Task<int> CodeCompletionPreview()
        {
            string FileName = "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";
            string filename = "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";
            editor.vp = editor.vs.GetProjectbyFileName(FileName);

            var ts = editor.GetTypes(vs);
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

        void VerifyReferences()
        {
            foreach(var d in textEditor.Document.Lines)
            {
                if (d.NextLine == null)
                    break;
                if(d.obs != null)
                    
                    if(d.NextLine.Length == 0)
                    {
                        d.NextLine.obs = d.obs;
                        d.obs = null;
                        textEditor.TextArea.InvalidateVisual();
                    }
            }
        }

        public bool modified = false;

        private void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            modified = true;


         
            // shouldUpdate = true;

            if (string.IsNullOrEmpty(e.Text))
                return;
            if (string.IsNullOrWhiteSpace(e.Text))
                return;

            string s = textEditor.Document.Text;

            Caret cc = textEditor.TextArea.Caret;

            EditorCommand ec = new EditorCommand();
            ec.name = "Text Inserted";
            ec.Offset = cc.Offset;
            ec.chars = e.Text;
            ec.editorWindow = this;
            commands.Insert(0, ec);

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
                if (modified/*shouldUpdate*/ == true)
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

            //Handled = true;
        }

        public static List<CsCompletionData> d = new List<CsCompletionData>();

        private void GetKeywords()
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

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
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

        private FoldingManager foldingManager;
        private object foldingStrategy;

        private void HighlightingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private bool EnableFolding = true;

        private void UpdateFoldings()
        {
            if (shouldPopupUpdate)
                PopupUpdate();
            if (!EnableFolding)
                return;
            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
            else
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
            else
            if (foldingStrategy is CommentAndMethodFoldingStrategy)
            {
                ((CommentAndMethodFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }
        }

        #endregion Folding

        public List<ISymbol> allMembers { get; set; }

        public List<int> mb { get; set; }

        private Dictionary<int, ISymbol> dict { get; set; }

        public int UpdateSourceDocument(bool shouldupdate = true)
        {
            //textEditor.IsReadOnly = true;

            //editor.GetMembers(textEditor.Document.Text);

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
            dict = new Dictionary<int, ISymbol>();

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

            //UpdateReferences(mb, dict, shouldupdate);

            textEditor.IsReadOnly = false;

            return 0;
        }

        public int UpdateSourceDocumentAsync(bool shouldupdate = true, string content = "")
        {
            //textEditor.IsReadOnly = true;

            //editor.GetMembers(textEditor.Document.Text);

            //        List<INamespaceOrTypeSymbol> symbols = editor.typesofproject;

            List<ISymbol> symbols = vs.GetMembers(null, FileToLoad, content/* textEditor.Document.Text*/);//editor.typesofproject;

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
            dict = new Dictionary<int, ISymbol>();
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
            //UpdateReferences(mb, dict, shouldupdate);
            textEditor.IsReadOnly = false;
            return 0;
        }

        public int UpdateSourceDocumented(bool shouldupdate = true)
        {
            //textEditor.IsReadOnly = true;

            //editor.GetMembers(textEditor.Document.Text);

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
            dict = new Dictionary<int, ISymbol>();

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

            //UpdateReferences(mb, dict, shouldupdate);

            //textEditor.IsReadOnly = false;

            return 0;
        }

        public List<ISymbol> UpdateSourceDocumentForContent(ISymbol symbol = null)
        {
            List<ISymbol> list = new List<ISymbol>();

            //textEditor.IsReadOnly = true;

            if (symbol == null)
                return list;

            editor.GetMembers(textEditor.Document.Text, true);

            INamedTypeSymbol cc = symbol as INamedTypeSymbol;

            if (cc == null)
                cc = symbol.ContainingType as INamedTypeSymbol;

            if (cc == null)
                return list;

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
                if (s is IEventSymbol)
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

                        if (rr.GetMethod != null)
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
                    if (cs.Any(d => d.Name == s.Name) || cs.Any(d => d.Name == cc.Name && s.Name == ".ctor"))
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
                        if (bc.Count > 1)
                        {
                            foreach (var bb in bc)
                            {
                                IMethodSymbol f = bb as IMethodSymbol;

                                if (f == null)
                                    continue;

                                if (f.Parameters.Count() != g.Parameters.Count())
                                {
                                    continue;
                                }
                                bool shouldload = true;
                                foreach (IParameterSymbol ps in f.Parameters)
                                {
                                    bool found = false;
                                    foreach (IParameterSymbol rs in g.Parameters)
                                    {
                                        if (rs.Type is INamedTypeSymbol)
                                        {
                                            INamedTypeSymbol es = rs.Type as INamedTypeSymbol;

                                            INamedTypeSymbol ec = ps.Type as INamedTypeSymbol;

                                            if (ec == null)
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

                                            if (gs == null)
                                            {
                                                continue;
                                            }
                                            if (bs.PointedAtType.Name != gs.PointedAtType.Name)
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
                                        string[] qq = ps.Type.Name.Split(".".ToCharArray());
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

            return cs;
        }

        public VSSolution vs { get; set; }

        async public void LoadFromProject(VSSolution vs, bool shouldUpdate = true, ISymbol symbol = null)
        {
            shouldUpdate = true;

            //await Task.Run(async () => editor.GetTypes());

            this.vs = vs;
            await Task.Run(async () =>
         {
             VSProject vp = null;
             if (FileToLoad != null)
                 vp = vs.GetProjectbyCompileItem(FileToLoad);
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
             List<ISymbol> cs = null;
             await this.Dispatcher.BeginInvoke((Action)(() =>
               {
                   //editor.GetMembers(textEditor.Document.Text);

                   //var r = editor.GetTypes();

                   if (vp == null)
                   {
                       cs = this.UpdateSourceDocumentForContent(symbol);
                       //this.qcb.DoUpdateForContent(symbol, editor.symbols);
                   }
                   else
                   {
                       editor.ProjectTypes(vs);

                       editor.GetMembers(textEditor.Document.Text);
                       LoadProjectTypes();

                       //this.UpdateSourceDocument(shouldUpdate);
                       // this.qcb.DoUpdate(editor.symbols);
                   }

                   List<INamespaceOrTypeSymbol> r = null;

                   Task task = Task.Run(async () =>
                {
                    this.CodeCompletionPreview();
                });

                   this.shouldUpdate = true;
               }));
         });
            shouldUpdate = true;
        }

        async public Task<int> LoadFromProjectAsync(VSSolution vs, bool shouldUpdate = true, ISymbol symbol = null)
        {
            shouldUpdate = true;

            //await Task.Run(async () => editor.GetTypes());

            this.vs = vs;
            await Task.Run(async () =>
            {
                VSProject vp = null;
                if (FileToLoad != null)
                    vp = vs.GetProjectbyCompileItem(FileToLoad);
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
                List<ISymbol> cs = null;
                //await this.Dispatcher.BeginInvoke((Action)(() =>
                //{
                //editor.GetMembers(textEditor.Document.Text);

                //var r = editor.GetTypes();

                if (vp == null)
                {
                    cs = this.UpdateSourceDocumentForContent(symbol);
                    //this.qcb.DoUpdateForContent(symbol, editor.symbols);
                }
                else
                {
                    //LoadReferencesAsync(vs);

                    //this.UpdateSourceDocument(shouldUpdate);
                    // this.qcb.DoUpdate(editor.symbols);
                }

                List<INamespaceOrTypeSymbol> r = null;

                Task task = Task.Run(async () =>
                {
                    this.CodeCompletionPreview();
                });

                this.shouldUpdate = true;
                //}));
            });
            shouldUpdate = true;
            referenced = false;
            return 0;
        }

        public async void LoadReferences(VSSolution vs, bool shouldUpdate = true, ISymbol symbol = null)
        {
            //this.shouldUpdate = false;

            this.vs = vs;
            await Task.Run(async () =>
            {
                VSProject vp = null;
                if (FileToLoad != null)
                    vp = vs.GetProjectbyCompileItem(FileToLoad);
                //if (vp != null)
                //{
                //    ProjectToLoad = vp.FileName;
                //    SolutionToLoad = vs.solutionFileName;
                //    editor.SolutionToLoad = SolutionToLoad;
                //    editor.ProjectToLoad = ProjectToLoad;
                //    editor.FileToLoad = FileToLoad;
                //}
                this.editor.vs = vs;
                this.qcb.editorWindow = this;
                List<ISymbol> cs = null;
                await this.Dispatcher.BeginInvoke(new Action(async () =>
              {
                    //editor.GetTypes(vs);

                    editor.ProjectTypes(vs);

                  editor.GetMembers(textEditor.Document.Text);
                  LoadProjectTypes();
                  if (vp == null)
                  {
                      cs = this.UpdateSourceDocumentForContent(symbol);
                        // this.qcb.DoUpdateForContent(symbol, editor.symbols);
                    }
                  else
                  {
                        //Task<int> tasked = Task.Run(async () =>
                        {
                          this.UpdateSourceDocument(true);

                            //var t = vs.GetSyntaxTree(FileToLoad).GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().ToList();
                            //t[0].DescendantNodes().OfType<MemberDeclarationSyntax>().ToList();

                            //await this.UpdateReferenceses(mb, dict, true);

                            // this.References_Insert(textEditor.Document, mb, dict, true);

                            AvalonReferencer r = new AvalonReferencer();
                          r.document = textEditor.Document;
                          r.vp = vp;
                          r.vs = vs;
                          r.FileToLoad = FileToLoad;

                            //await r.LoadReferences();

                            this.References_Update(mb, r.data, r.document);

                            //return 0;
                        }//);

                        //    await tasked;

                        //this.UpdateSourceDocument(shouldUpdate);
                        // this.qcb.DoUpdate(editor.symbols);
                    }

                    //ISymbol s = symbol;
                    //if (cs == null)
                    //if (cs != null || symbol != null)
                    //{
                    //    if(cs != null && cs.Count > 0)
                    //        s = cs.Select(x => x).Where(y => y.Name == s.Name).FirstOrDefault();
                    //    if (s != null)
                    //    {
                    //        if (s.Locations != null && s.Locations.Count() > 0 && s.Locations[0].IsInSource)
                    //        {
                    //            int line = this.textEditor.Document.GetLineByOffset(s.Locations[0].SourceSpan.Start).LineNumber;
                    //            this.textEditor.ScrollToLine(line);
                    //                this.textEditor.TextArea.Caret.Line = line;
                    //                this.textEditor.TextArea.TextView.Focus();
                    //        }
                    //    }
                    //}

                    //Task task = Task.Run(async () =>
                    //{
                    //    await this.CodeCompletionPreview();
                    //});
                    //if (!string.IsNullOrEmpty(FileToLoad))
                    //{
                    //    //ManualResetEvent auto = vs.SynchronizedEventForCompileFile(FileToLoad);
                    //    //Task.Run(() => auto.WaitOne()).Wait();
                    //}
                    //this.LoadProjectTypes(editor.GetTypes());
                }));
            });
        }

        public int LoadReferencesAsync(VSSolution vs, bool shouldUpdate = true, ISymbol symbol = null)
        {
            this.vs = vs;
            //await Task.Run(async () =>
            //{
            VSProject vp = null;
            if (FileToLoad != null)
                vp = vs.GetProjectbyCompileItem(FileToLoad);

            this.editor.vs = vs;
            this.qcb.editorWindow = this;
            List<ISymbol> cs = null;

            //await this.Dispatcher.BeginInvoke(new Action(async () =>
            //{
            //editor.GetTypes(vs);

            //editor.ProjectTypes(vs);
            //editor.GetMembers(textEditor.Document.Text);
            //LoadProjectTypes();

            //}));
            if (vp == null)
            {
                //cs = this.UpdateSourceDocumentForContent(symbol);
            }
            else
            {
                //{
                //    this.UpdateSourceDocument(true);

                //    this.References_Insert(mb, dict, true);

                //    AvalonReferencer r = new AvalonReferencer();
                //    r.document = textEditor.Document;
                //    r.vp = vp;
                //    r.vs = vs;
                //    r.FileToLoad = FileToLoad;

                //    await r.LoadReferences();

                //    this.References_Update(r.offsets, r.data, r.document);
                //}
            }

            //}));
            //});
            referenced = false;
            return 0;
        }

        public bool referenced = false;

        public async Task<int> References()
        {
            double offset = 0.0;

            AvalonReferencer r = new AvalonReferencer();

            ICSharpCode.AvalonEdit.Document.TextDocument document = null;

            string content = "";

            this.Dispatcher.Invoke(new Action(() =>
            {
                offset = textEditor.VerticalOffset;
                string c = StringReplace.Replace(textEditor.Document.Text);
                textEditor.Document.Text = c;
                //    vs.UpdateSyntaxTree(FileToLoad, c);

                this.UpdateSourceDocumentAsync(true, c);
            }));

            this.Dispatcher.Invoke(new Action(() =>
            {
                content = textEditor.Document.Text;
                document = textEditor.Document;// new ICSharpCode.AvalonEdit.Document.TextDocument(content);
                r.document = document;
                r.vp = editor.vp;
                r.vs = vs;
                r.FileToLoad = FileToLoad;
            }));
            //document = textEditor.Document;// new ICSharpCode.AvalonEdit.Document.TextDocument(content);
            //r.document = document;
            //r.vp = editor.vp;
            //r.vs = vs;
            //r.FileToLoad = FileToLoad;
            // this.References_Insert(document, mb, dict, true);

            await r.LoadReferences(dict);

            //this.References_Update(r.offsets, r.data, r.document);

            content = document.Text;

            this.Dispatcher.Invoke(new Action(() =>
            {
                EnableFolding = false;
                //this.textEditor.Document.Text = content;
                //this.textEditor.Document = document;
                ////EnableFolding = true;
                string cs = StringReplace.Replace(textEditor.Document.Text);
                //vs.UpdateSyntaxTree(FileToLoad, cs);
                //textEditor.Document.Text = StringReplace.ReplaceUndo(cs);

                this.UpdateSourceDocumentAsync(true, cs);
                this.References_Insert(document, mb, dict, r.offsets, r.data, true);
                cs = StringReplace.Replace(textEditor.Document.Text);
                this.UpdateSourceDocumentAsync(true, cs);
                this.References_Insert(document, mb, dict);

                //string cs = StringReplace.Replace(textEditor.Document.Text);

                //vs.UpdateSyntaxTree(FileToLoad, this.textEditor.Document.Text);

                textEditor.ScrollToVerticalOffset(offset);
            }));

            return 0;
        }
        public async Task<int> References_Update()
        {
            double offset = 0.0;

            AvalonReferencer r = new AvalonReferencer();

            ICSharpCode.AvalonEdit.Document.TextDocument document = null;

            string content = "";

            this.Dispatcher.Invoke(new Action(() =>
            {
                offset = textEditor.VerticalOffset;
                string c = StringReplace.Replace(textEditor.Document.Text);
                textEditor.Document.Text = c;
                //    vs.UpdateSyntaxTree(FileToLoad, c);

                this.UpdateSourceDocumentAsync(true, c);
            }));

            this.Dispatcher.Invoke(new Action(() =>
            {
                content = textEditor.Document.Text;
                document = textEditor.Document;// new ICSharpCode.AvalonEdit.Document.TextDocument(content);
                r.document = document;
                r.vp = editor.vp;
                r.vs = vs;
                r.FileToLoad = FileToLoad;
            }));


            await r.LoadReferences(dict);


            content = document.Text;

            this.Dispatcher.Invoke(new Action(() =>
            {
                EnableFolding = false;
                //this.textEditor.Document.Text = content;
//                this.textEditor.Document = document;
                ////EnableFolding = true;
                string cs = StringReplace.Replace(textEditor.Document.Text);
                //vs.UpdateSyntaxTree(FileToLoad, cs);
                textEditor.Document.Text = StringReplace.ReplaceUndo(cs);

                this.UpdateSourceDocumentAsync(true, cs);

                this.References_Insert(document, mb, dict, r.offsets, r.data, true);

                cs = StringReplace.Replace(textEditor.Document.Text);
                this.UpdateSourceDocumentAsync(true, cs);
                this.References_Insert(document, mb, dict);

                //string cs = StringReplace.Replace(textEditor.Document.Text);

                //vs.UpdateSyntaxTree(FileToLoad, this.textEditor.Document.Text);

                textEditor.ScrollToVerticalOffset(offset);
            }));

            return 0;
        }
        public async Task<int> References_Updates()
        {
            AvalonReferencer r = new AvalonReferencer();

            this.Dispatcher.Invoke(new Action(() =>
            {
                string c = StringReplace.Replace(textEditor.Document.Text);
                vs.UpdateSyntaxTree(FileToLoad, c);

                this.UpdateSourceDocumentAsync(true);
            }));

            ICSharpCode.AvalonEdit.Document.TextDocument document = null;

            string content = "";

            this.Dispatcher.Invoke(new Action(() =>
            {
                content = textEditor.Document.Text;
            }));
            document = new ICSharpCode.AvalonEdit.Document.TextDocument(content);
            r.document = document;
            r.vp = editor.vp;
            r.vs = vs;
            r.FileToLoad = FileToLoad;
            //this.References_Insert(document, mb, dict, true);

            await r.LoadReferences(dict);

            // this.References_Update(r.offsets, r.data, r.document);

            content = document.Text;

            this.Dispatcher.Invoke(new Action(() =>
            {
                EnableFolding = false;
                //this.textEditor.Document.Text = content;
                this.textEditor.Document = document;
                ////EnableFolding = true;
                this.UpdateSourceDocumentAsync(true);
                this.References_Insert(document, mb, dict, r.offsets, r.data, true);
                //string cs = StringReplace.Replace(textEditor.Document.Text);
                vs.UpdateSyntaxTree(FileToLoad, this.textEditor.Document.Text);
                //textEditor.Document.Text = StringReplace.ReplaceUndo(cs);
            }));

            return 0;
        }
    }

    public class BackgroundRenderer : IBackgroundRenderer
    {
        public System.Windows.Point Position { get; set; }

        public System.Windows.Point EndPosition { get; set; }

        public bool shouldHighlight = false;



        public FrameworkElement v { get; set; }

        public TextEditor textEditor { get; set; }

        private System.Windows.Media.Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 140, 140, 140));

        private System.Windows.Media.Pen pen = new System.Windows.Media.Pen(new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 140, 140, 140)), 1);

        private System.Windows.Media.Pen penOutline = new System.Windows.Media.Pen(new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 80, 80, 80)), 1);

        public string word { get; set; }

        public List<int> searchResultIndices { get; set; }

        public int searchResultLength = 0;

        private int prevStart = 0;

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

            if (string.IsNullOrEmpty(word) && searchResultIndices == null)
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
            if (word != null && word != "")
            {
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
            }
            if (searchResultIndices != null)
            {

                var r = searchResultIndices.Where(p => p >= viewStart && p <= viewEnd);
                foreach(var ch in r)
                {
                    BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
                    geoBuilder.CornerRadius = 1;
                    TextSegment s = new TextSegment();
                    s.StartOffset = ch;
                    s.EndOffset = ch + searchResultLength;

                    geoBuilder.AddSegment(textView, s);
                    Geometry geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        drawingContext.DrawGeometry(Brushes.Orange, pen, geometry);
                    }
                }
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

                rw.CompileSolution(file, vs);
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

                ///////editorWindow.qcb.DoUpdate(editor.symbols);

                //editorWindow.qcb.UpdateSourceDocument();

                editorWindow.LoadProjectTypes(editor.GetTypes(vs));

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

    public class Popups : ContentControl
    {
        public Popups()
        {
            //            this.Loaded += Popups_Loaded;
            //            this.SourceUpdated += Popups_SourceUpdated;
        }

        private void Popups_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        }

        private void Popups_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public EditorWindow editorWindow { get; set; }

        public void LoadHandlers()
        {
            //if(this.PlacementTarget != null)
            {
                //if(this.PlacementTarget is FrameworkElement)
                {
                    //FrameworkElement element = this.Content as FrameworkElement;
                    FrameworkElement element = this as FrameworkElement;
                    element.SizeChanged += Element_SizeChanged;
                }
            }
        }

        public void Element_SizeChanged()
        {
            Element_SizeChanged(null, null);
        }

        private void Element_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (editorWindow == null)
                return;
            Size size = editorWindow.GetPopupSize();
            if (size.Width <= 0 || size.Height <= 0)
                return;
            this.Width = size.Width - 25;
            this.Height = size.Height;

            this.InvalidateVisual();
        }

        public void ReloadIfNeeded(Size sz)
        {
            if (editorWindow == null)
                return;
            Size size = editorWindow.GetPopupSize();
            this.Width = sz.Width;
            this.Height = sz.Height;

            this.InvalidateVisual();
        }

        //public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popups), new FrameworkPropertyMetadata(false, OnTopmostChanged));
        //public bool Topmost
        //{
        //    get { return (bool)GetValue(TopmostProperty); }
        //    set { SetValue(TopmostProperty, value); }
        //}
        private static void OnTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            (obj as Popups).UpdateWindow();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        private void UpdateWindow()
        {
            if (((HwndSource)PresentationSource.FromVisual(this)) == null)
                return;
            var hwnd = ((HwndSource)PresentationSource.FromVisual(this as UIElement)).Handle;
            RECT rect;
            if (GetWindowRect(hwnd, out rect))
            {
                // SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
            }
        }

        #region imports definitions

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        #endregion imports definitions
    }

    public class ScrollBoundScrollBar : ScrollBar
    {
        public TextEditor _scrollViewer;
        public TextEditor BoundScrollViewer { get { return _scrollViewer; } set { _scrollViewer = value; /*_scrollViewer.TextArea.TextView.UpdateBindings(); */} }

        public ScrollBoundScrollBar(TextEditor scrollViewer, Orientation o) : base()
        {
            this.Orientation = o;
            BoundScrollViewer = _scrollViewer;
        }

        public ScrollBoundScrollBar() : base()
        {
        }

        public void LoadEvents()
        {
            _scrollViewer.Loaded += _scrollViewer_Initialized;
        }

        private void _scrollViewer_Initialized(object sender, EventArgs e)
        {
            UpdateBindings();
        }

        public void UpdateBindings()
        {
            this.AddHandler(ScrollBar.ScrollEvent, new ScrollEventHandler(OnScroll));
            if (_scrollViewer.scrollViewer != null)
                _scrollViewer.scrollViewer.handler += BoundScrollChanged;
            this.Minimum = 0;
        }

        public void BoundScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.Maximum = e.ExtentWidth;

            switch (this.Orientation)
            {
                case Orientation.Horizontal:
                    this.Value = e.HorizontalOffset;
                    break;

                case Orientation.Vertical:
                    this.Value = e.VerticalOffset;
                    break;

                default:
                    break;
            }
        }

        public void OnScroll(object sender, ScrollEventArgs e)
        {
            switch (this.Orientation)
            {
                case Orientation.Horizontal:
                    this.BoundScrollViewer.ScrollToHorizontalOffset(e.NewValue);
                    break;

                case Orientation.Vertical:
                    this.BoundScrollViewer.ScrollToVerticalOffset(e.NewValue);
                    break;

                default:
                    break;
            }
        }
    }

    public class EditorCommand
    {
        public string name { get; set; }
        public string chars { get; set; }
        public int Offset { get; set; }
        public EditorWindow editorWindow { get; set; }

        public void Command(bool undo = true)
        {
            if (editorWindow == null || editorWindow.textEditor == null)
                return;
            var d = editorWindow.textEditor.Document;

            if (undo)
            {
                d.Remove(Offset - 1, chars.Length);
                editorWindow.textEditor.CaretOffset = Offset - 1;
            }
            else
            {
                d.Insert(Offset - 1, chars);
                editorWindow.textEditor.CaretOffset = Offset;
            }
            editorWindow.textEditor.InvalidateVisual();
        }
    }

    public class AvalonReferencer
    {
        public string FileToLoad { get; set; }

        public ICSharpCode.AvalonEdit.Document.TextDocument document { get; set; }

        public VSProject vp { get; set; }

        public VSSolution vs { get; set; }

        public Dictionary<int, int> data = new Dictionary<int, int>();

        public bool HasSymbolChanged()
        {
            return true;
        }

        public int GetReferenceNumber2(int line)
        {
            {
                DocumentLine gv = document.GetLineByNumber(line);
                if (gv.obs != null)
                    if (gv.obs is ISymbol)
                    {
                        //string FileName = ProjectToLoad;

                        string filename = FileToLoad;

                        //editor.vp = editor.vs.GetProjectbyFileName(FileName);

                        ISymbol s = gv.obs as ISymbol;

                        var r = vs.GetAllSymbolReferences(s, filename, vp/*, StringReplace.Replace(textEditor.Document.Text)*/).Result;

                        if (r == null)
                            return 0;

                        int n = 0;
                        foreach (ReferencedSymbol b in r)
                        {
                            int cc = b.Locations.Count();
                            if (cc == 0)
                                n += 0;
                            else n += cc;
                        }

                        return n;
                    }
                return 0;
            }
        }

        public int GetReferenceNumber(int offset, ISymbol s, int line)
        {
            DocumentLine gv = document.GetLineByOffset(offset);
            //if (gv.obs != null)
            //    if (gv.obs is ISymbol)
            //                        {
            //string FileName = ProjectToLoad;

            string filename = FileToLoad;

            //editor.vp = editor.vs.GetProjectbyFileName(FileName);

            //      ISymbol s = gv.obs as ISymbol;

            var r = vs.GetAllSymbolReferences(s, filename, vp/*, StringReplace.Replace(textEditor.Document.Text)*/).Result;

            if (r == null)
                return 0;

            int n = 0;
            foreach (ReferencedSymbol b in r)
            {
                int cc = b.Locations.Count();
                if (cc == 0)
                    n += 0;
                else n += cc;
            }

            return n;

            //                       }
            //                   return 0;
        }

        public List<int> offsets = new List<int>();

        async public Task<int> LoadReferences2()
        {
            for (int j = 1; j <= document.Lines.Count; j++)
            {
                var c = document.GetLineByNumber(j);

                int line = c.LineNumber;

                int g = c.Offset;

                if (c.obs == null)
                    continue;

                int n = 0;// GetReferenceNumber(line);

                offsets.Add(line);
                data.Add(line, n);

                //textEditor.Document.Text = textEditor.Document.Text.Remove(g + 15, 5).Insert(g + 15, n.ToString().PadRight(5));
                //vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
            }

            offsets.Sort();

            offsets.Reverse();

            return 0;
        }

        async public Task<int> LoadReferences(Dictionary<int, ISymbol> dict)
        {
            //for (int j = 1; j <= document.Lines.Count; j++)
            foreach (int fs in dict.Keys)
            {
                var c = document.GetLineByOffset(fs);

                int line = c.LineNumber;

                //int line = c.LineNumber;

                //int g = c.Offset;

                //if (c.obs == null)
                //    continue;

                int n = GetReferenceNumber(fs, dict[fs], 0);

                offsets.Add(line);
                data.Add(fs, n);

                //textEditor.Document.Text = textEditor.Document.Text.Remove(g + 15, 5).Insert(g + 15, n.ToString().PadRight(5));
                //vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
            }

            offsets.Sort();

            offsets.Reverse();

            return 0;
        }
    }
    public class Searcher
    {
        public string texttofind { get; set; }
        public List<int> Offsets { get; set; }
        public List<LocationSource> source { get; set; }
        public List<LocationSource> sources { get; set; }
        public int Active { get; set; }
        public int Id { get; set; }
        public LocationSource ActiveSource { get; set; }
        public EditorWindow editorWindow { get; set; }
        public int NextSearchResult()
        {
            Active++;
            if (Active >= Offsets.Count)
            {
                Active--;
                //   Active = 0;
                if (EditorWindow.FindResultsRequired == null)
                    return 1;
                
                EditorWindow.FindResultsRequired.Invoke(this);
                return 0;
            }
            editorWindow.Dispatcher.Invoke(new Action(() =>
            {
                editorWindow.Focus();
                editorWindow.ShowOffset(Offsets[Active]);
                editorWindow.textEditor.TextArea.InvalidateVisual();
                
            }));
            return 1;
        }
        public int NextResultFileIndex()
        {
            var r = sources[Active].next.id;
            return r;
        }
    }
}

//<!--<avalonEdit:TextEditor.Resources>
//                            <Style TargetType = "ScrollBar" >
//                                < Style.Triggers >
//                                    < Trigger Property="Orientation" Value="Vertical">
//                                        <Setter Property = "Width" Value="14"/>

//                                    </Trigger>

//                                </Style.Triggers>

//                            </Style>-->
//                        </avalonEdit:TextEditor.Resources>