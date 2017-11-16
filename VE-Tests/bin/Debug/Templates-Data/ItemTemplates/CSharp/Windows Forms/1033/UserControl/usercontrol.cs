using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$using System.Windows.Forms;

namespace $rootnamespace$
{
    public partial class $safeitemrootname$: UserControl
    {
        public $safeitemrootname$()
        {
            InitializeComponent();
        }
    }
}
