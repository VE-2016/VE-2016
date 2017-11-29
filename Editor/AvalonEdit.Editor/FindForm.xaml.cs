using System.Windows.Controls;

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

            FindNext.Source = TreeViewer.Convert(WinExplorers.ve.Next_16x);
            ButtonClose.Source = TreeViewer.Convert(WinExplorers.ve.Close_16x);
            ButtonReplaceNext.Source = TreeViewer.Convert(WinExplorers.ve.ReplaceAll_16x);
            ButtonReplaceAll.Source = TreeViewer.Convert(WinExplorers.ve.ReplaceAll_16x);
            ButtonMatchCase.Source = TreeViewer.Convert(WinExplorers.ve.TextBlock_16x);
            ButtonMatchWord.Source = TreeViewer.Convert(WinExplorers.ve.TextBlock_16x);
            ButtonRegularExpression.Source = TreeViewer.Convert(WinExplorers.ve.RegularExpression_16x);
        }
    }
}