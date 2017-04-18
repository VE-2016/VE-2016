using System.Windows.Forms;

namespace WinExplorer.UI
{
    public partial class AdvancedProperiesForm : Form
    {
        public AdvancedProperiesForm()
        {
            InitializeComponent();
            pg = propertyGrid1;
        }

        private PropertyGrid pg { get; set; }

        public void SetPropertyObject(object obs)
        {
            pg.SelectedObject = obs;
        }
    }
}