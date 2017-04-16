using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        PropertyGrid pg { get; set; }
        public void SetPropertyObject(object obs)
        {
            pg.SelectedObject = obs;
        }
    }
}
