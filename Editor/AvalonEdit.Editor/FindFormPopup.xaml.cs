using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Interaction logic for FindForm.xaml
    /// </summary>
    public partial class FindFormPopup : Popup
    {
        public FindFormPopup()
        {
            InitializeComponent();
            
            forms.popup = this;
            PreviewKeyDown += FindFormPopup_PreviewKeyDown;

        }

        private void FindFormPopup_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Escape)
            {
                this.IsOpen = false;
            }
        }
        public void SetCurrentBlock(string blockname)
        {
            forms.SetCurrentBlock(blockname);
        }
    }
  
}