using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Snippets;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
//using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            //StartCompilation();

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
                    foreach(var c in xshd.Elements)
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

            r = new BackgroundRenderer();

            r.v = textEditor.TextArea.TextView;
            r.textEditor = textEditor;

            textEditor.TextArea.TextView.BackgroundRenderers.Add(r);
            textEditor.TextArea.TextView.MouseMove += TextView_MouseMove;
            textEditor.TextArea.TextView.MouseDown += TextView_MouseDown;
            textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            textEditor.TextArea.TextView.MouseHover += TextView_MouseHover;
            textEditor.KeyDown += TextEditor_KeyDown;
            this.SizeChanged += EditorWindow_SizeChanged;

            this.textEditor.TextArea.TextView.ScrollOffsetChanged += TextView_ScrollOffsetChanged;

            //textEditor.PreviewKeyUp += TextEditor_PreviewKeyUp;  

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
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

        private void EditorWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(popup != null)
            {
                popup.Element_SizeChanged();
                textEditor.TextArea.TextView.Redraw();
            }
        }

        ImageElementGenerator elementGenerator { get; set; }
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
        bool shouldPopupUpdate = false;
        void PopupUpdate()
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

        public void Activated(bool isActive)
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
            }
            else
            {
                if (popup != null)
                    if (popup.Visibility != Visibility.Hidden)
                        popup.Visibility = Visibility.Hidden;
            }
        }

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
        bool IsZoom = true;

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
        bool Handled = false;

        ImageElementGenerator imageElementGenerator = new ImageElementGenerator("");

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
                                var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp);
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

            return contextMenu;

        }

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

                var r = editor.vs.GetAllSymbolReferences(s, FileToLoad, vp/* StringReplace.Replace(textEditor.Document.Text)*/);

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




                var r = editor.vs.GetAllSymbolReferences(s, FileToLoad, vp/*, StringReplace.Replace(textEditor.Document.Text)*/);

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

            IEnumerable<ReferencedSymbol> r = vs.GetAllSymbolReferences(s, FileToLoad, vp);

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
        EditorWindow embedEditorWindow { get; set; }

        StackPanel stackPanel { get; set; }

        StackPanel buttonPanel { get; set; }

        Button filenameButton { get; set; }

        double prevWidth = 0;

        Tuple<int, int> GetWidthAfterTransform()
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

            int width = (int)( zoomScaleFactor * ((double)(textEditor.TextArea.ActualWidth - allmargins + left)));

            return new Tuple<int,int>(width, margins);
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

            foreach(FrameworkElement cc in this.textEditor.TextArea.LeftMargins)
            {
                allmargins += (int)cc.ActualWidth;
            }
            Point point = new Point(allmargins - left, 0);
            point = this.textEditor.TextArea.LayoutTransform.Transform(point);
            int margins = (int)(( zoomScaleFactor) * (double) (allmargins - left));
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
           
            if((bool)e.NewValue == false)
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

        Popups popup = null;
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
        System.Windows.Threading.DispatcherTimer eTimer =  new System.Windows.Threading.DispatcherTimer();// new System.Timers.Timer(1000);
        private void OnTimedEvent(object source, /*Elapsed*/EventArgs e)
        {
            if (!textEditor.TextArea.IsFocused)
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
           
            if(e.Key == Key.F2)
            {
               
                textEditor.Document.Insert(textEditor.Document.Text.Length, textEditor.Document.GetTreeAsString());
            }
            else 
            if (e.Key == Key.Escape)
            {
                if (popup == null)
                    return;
               
                {
                    RemoveVirtualLines();
                    popup.Visibility = Visibility.Hidden;
                    if(embedEditorWindow != null)
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

                this.Dispatcher.BeginInvoke(new Action(() => {
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

        string PeekDefinition_Content { get; set; }

        string PeekDefinition_FileName { get; set; }

        Tuple<string, string, ISymbol> PeekDefinition_Tuple { get; set; }


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
                embedEditorWindow.LoadFromProject(vs, t.Item3);

                

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
                embedEditorWindow.LoadFromProject(vs, t.Item3);
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
            if(popup != null)
                popup.Visibility = Visibility.Visible;
            textEditor.TextArea.TextView.Redraw();
        }

        public EditorWindow sourceEditor = null;

        void RemoveVirtualLines()
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


            GetTypesNames();

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

        public LineNumberMargin lineNumberMarginExtended { get; set; }

        public BreakPointMargin breakPointMargin { get; set; }

        BreakPointMargin bl { get; set; }

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

        private void TextView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.RightButton == MouseButtonState.Pressed)
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
                            string FileName = ProjectToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\VSExplorer.csproj";

                            string filename = FileToLoad;// "C:\\MSBuildProjects-beta\\VSExplorer\\ExplorerForms.cs";

                            editor.vp = editor.vs.GetProjectbyFileName(FileName);

                            ISymbol s = gv.obs as ISymbol;
                            var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp/*, StringReplace.Replace(textEditor.Document.Text)*/);
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

        public int GetReferenceNumber(int line)
        {
            
            {
                DocumentLine gv = textEditor.Document.GetLineByNumber(line);
                if (gv.obs != null)
                    if (gv.obs is ISymbol)
                    {
                        string FileName = ProjectToLoad;

                        string filename = FileToLoad;

                        editor.vp = editor.vs.GetProjectbyFileName(FileName);

                        ISymbol s = gv.obs as ISymbol;
                        var r = editor.vs.GetAllSymbolReferences(s, filename, editor.vp/*, StringReplace.Replace(textEditor.Document.Text)*/);

                        if (r == null)
                            return 0;

                        int n = 0;
                        foreach(ReferencedSymbol b in r)
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

               
                textEditor.Document.Insert(g, "  References -          \n", objects[v]);

                ns.Add(textEditor.Document.GetLineByOffset(g));

               
            }

            foreach (var b in ns)
            {
                 textEditor.Document.Update(b);
            }
            
            vs.UpdateSyntaxTree(FileToLoad, StringReplace.Replace(textEditor.Document.Text));

            

            this.Dispatcher.BeginInvoke(new Action(() => {


                List<int> offsets = new List<int>();

                Dictionary<int,int> data = new Dictionary<int,int>();

                
                for(int j = 1; j <= textEditor.Document.Lines.Count; j++)
                {
                    var c = textEditor.Document.GetLineByNumber(j);

                    int line = c.LineNumber;

                    int g = c.Offset;

                    if (c.obs == null)
                        continue;

                    int n = GetReferenceNumber(line);

                    offsets.Add(line);
                    data.Add(line, n);

                    //textEditor.Document.Text = textEditor.Document.Text.Remove(g + 15, 5).Insert(g + 15, n.ToString().PadRight(5));

                    //vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);
                }

                offsets.Sort();

                offsets.Reverse();

                string content = textEditor.Document.Text;

                foreach(int v in offsets)
                {

                    var c = textEditor.Document.GetLineByNumber(v);

                    var obs = c.obs;

                    int n = data[v];

                    int g = c.Offset;

                    //content = content.Remove(g + 15, 5).Insert(g + 15, n.ToString().PadRight(5));

                    var cc = textEditor.Document.GetLineByNumber(v);

                    string cs = textEditor.Document.Text.Substring(cc.Offset, cc.Length);

                    cs = cs.Insert(15, n.ToString().PadRight(5));

                    textEditor.Document.Remove(g, cc.Length);
                    textEditor.Document.Insert(g, cs, cc.obs);


                    //                    textEditor.Document.Remove(g + 15, 5);
                    //                    textEditor.Document.Insert(g + 15, n.ToString().PadRight(5), cc.obs);



                    //cc.obs = obs;
                }

                //textEditor.Document.Text = content;

                vs.UpdateSyntaxTree(FileToLoad, textEditor.Document.Text);

               

            }));
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

            var t = Task.Run(async delegate
            {
                foreach(int v in objects.Keys)
                {
                    int g = textEditor.Document.GetLineByOffset(v).Offset;

                    int line = textEditor.Document.GetLineByOffset(v).LineNumber;
                    
                    int n = GetReferenceNumber(line);

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

            if (textEditor.IsReadOnly)
                return;

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
            if (shouldPopupUpdate)
                PopupUpdate();

            if (foldingStrategy is BraceFoldingStrategy)
            {
                ((BraceFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            }else
            if (foldingStrategy is XmlFoldingStrategy)
            {
                ((XmlFoldingStrategy)foldingStrategy).UpdateFoldings(foldingManager, textEditor.Document);
            } else
            if ( foldingStrategy is CommentAndMethodFoldingStrategy)
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
        public List<ISymbol> UpdateSourceDocumentForContent(ISymbol symbol = null)
        {

            List<ISymbol> list = new List<ISymbol>();

            textEditor.IsReadOnly = true;

            if (symbol == null)
                return list;

            editor.GetMembers(textEditor.Document.Text, true);

            INamedTypeSymbol cc = symbol as INamedTypeSymbol;

            if (cc == null)
                cc = symbol.ContainingType as INamedTypeSymbol;
            
            if(cc == null)
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

            return cs;
        }

        public VSSolution vs { get; set; }

       public async void LoadFromProject(VSSolution vs, ISymbol symbol = null)
        {
            
            this.vs = vs;

              await Task.Run(() =>
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

                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (vp == null)
                    {
                        cs = this.UpdateSourceDocumentForContent(symbol);
                        this.qcb.DoUpdateForContent(symbol, editor.symbols);
                    }
                    else
                    {

                        this.UpdateSourceDocument();

                        this.qcb.DoUpdate(editor.symbols);
                    }
                    this.LoadProjectTypes(editor.GetTypes());

                    ISymbol s = symbol;

                    if (cs == null)
                     
                    if (cs != null || symbol != null)
                    {
                            if(cs != null && cs.Count > 0)
                        s = cs.Select(x => x).Where(y => y.Name == s.Name).FirstOrDefault();

                        if (s != null)
                        {
                            if (s.Locations != null && s.Locations.Count() > 0 && s.Locations[0].IsInSource)
                            {
                                int line = this.textEditor.Document.GetLineByOffset(s.Locations[0].SourceSpan.Start).LineNumber;
                                this.textEditor.ScrollToLine(line);
                                    this.textEditor.TextArea.Caret.Line = line;
                                    this.textEditor.TextArea.TextView.Focus();
                            }
                        }
                    }
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
        #endregion
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
            if(_scrollViewer.scrollViewer != null)
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