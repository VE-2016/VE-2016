using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic for FindForm.xaml
    /// </summary>
    public partial class FindForm : UserControl
    {
        public FindForm()
        {
            InitializeComponent();

            ButtonExpand.Source = TreeViewer.Convert(WinExplorers.ve.ExpandChevronUp_16x);
            FindNext.Source = TreeViewer.Convert(WinExplorers.ve.Next_16x);
            FindNext.MouseDown += FindNext_MouseDown;
            ButtonClose.Source = TreeViewer.Convert(WinExplorers.ve.Close_16x);
            ButtonClose.MouseUp += ButtonClose_MouseUp;
            ButtonReplaceNext.Source = TreeViewer.Convert(WinExplorers.ve.ReplaceAll_16x);
            ButtonReplaceAll.Source = TreeViewer.Convert(WinExplorers.ve.ReplaceAll_16x);
            CaseSensitiveHover = TreeViewer.Convert(ves.Service.GetTransparentOverlay(WinExplorers.ve.CaseSensitive_16x, Color.LightBlue));
            CaseSensitiveNoHover = TreeViewer.Convert(WinExplorers.ve.CaseSensitive_16x);
            ButtonMatchCase.Source = TreeViewer.Convert(WinExplorers.ve.CaseSensitive_16x);
            ButtonMatchCase.MouseEnter += ButtonMatchCase_MouseEnter;
            ButtonMatchCase.MouseLeave += ButtonMatchCase_MouseLeave;
            ButtonMatchWord.Source = TreeViewer.Convert(WinExplorers.ve.TextBlock_16x);
            MatchWordHover = TreeViewer.Convert(ves.Service.GetTransparentOverlay(WinExplorers.ve.TextBlock_16x, Color.LightBlue));
            MatchWordNoHover = TreeViewer.Convert(WinExplorers.ve.TextBlock_16x);
            ButtonMatchWord.MouseEnter += ButtonMatchWord_MouseEnter;
            ButtonMatchWord.MouseLeave += ButtonMatchWord_MouseLeave;

            ButtonRegularExpression.Source = TreeViewer.Convert(WinExplorers.ve.RegularExpression_16x);
            ButtonResize.Source = TreeViewer.Convert(WinExplorers.ve.ResizeGripLeft_16x);


            Search_Combobox.Text = "Find...";

            //Search_Combobox.PreviewKeyDown += Search_Combobox_PreviewKeyDown;

            Search_Combobox.KeyUp += Search_Combobox_KeyDown;

            Range_Combobox.DropDownOpened += Range_Combobox_DropDownOpened;

            Search_Combobox.Focus();
        }

        async private void FindNext_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (searcher == null)
                return;
            await Task.Run(() =>
            {
                searcher.NextSearchResult();
                
            });
            this.Focus();
        }

        private void ButtonMatchWord_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonMatchWord.Source = MatchWordNoHover;
            ButtonMatchWord.InvalidateVisual();
        }

        private void ButtonMatchWord_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonMatchWord.Source = MatchWordHover;
            ButtonMatchWord.InvalidateVisual();
        }

        private void ButtonMatchCase_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonMatchCase.Source = CaseSensitiveNoHover;
            ButtonMatchCase.InvalidateVisual();
        }

        BitmapImage CaseSensitiveHover { get; set; }
        BitmapImage CaseSensitiveNoHover { get; set; }
        BitmapImage MatchWordHover { get; set; }
        BitmapImage MatchWordNoHover { get; set; }

        private void ButtonMatchCase_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ButtonMatchCase.Source = CaseSensitiveHover;
            ButtonMatchCase.InvalidateVisual();
        }
        Searcher searcher { get; set; }
        async private void Search_Combobox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (string.IsNullOrEmpty(Search_Combobox.Text) || string.IsNullOrWhiteSpace(Search_Combobox.Text))
                return;
            
                if(Range_Combobox.SelectedIndex == 0)
            {
                searcher = editorWindow.Search(Search_Combobox.Text, VSParsers.SearchDomain.solution);
            }
                else searcher = editorWindow.Search(Search_Combobox.Text, VSParsers.SearchDomain.file);
                await Task.Run(()=> {
                    
                    if (searcher == null)
                        return;
                    
                    int r = searcher.NextSearchResult();
                    if (r == 0)
                    {
                        popup.Dispatcher.Invoke(new Action(() => { this.popup.IsOpen = false; }));
                    }
                    
                });
                this.Focus();
            
        }

        private void Search_Combobox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
        }

        private void Range_Combobox_DropDownOpened(object sender, System.EventArgs e)
        {
            if (editorWindow == null)
                return;
            string block = editorWindow.GetCurrentBlock();

            SetCurrentBlock(block);
        }

        public Popup popup { get; set; }

        public EditorWindow editorWindow { get; set; }

        private void ButtonClose_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (popup != null)
                popup.IsOpen = false;
        }
        public void SetCurrentBlock(string blockname)
        {
            ComboBox c = Range_Combobox;
            c.Items[2] = "Current Block ( " + blockname + " )";
        }
        async public void OpenSearchResults(Searcher searcher)
        {
            this.searcher = searcher;

            editorWindow.Search(searcher);

            searcher.Active = -1;

            await Task.Run(() => {

                if (searcher == null)
                    return;

                int r = searcher.NextSearchResult();
                if (r == 0)
                {
                    popup.Dispatcher.Invoke(new Action(() => { this.popup.IsOpen = false; }));
                }

            });
            this.Focus();
        }
    }
}