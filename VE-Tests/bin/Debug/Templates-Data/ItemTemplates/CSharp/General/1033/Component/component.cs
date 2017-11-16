using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Text;
$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$
namespace $rootnamespace$
{
    public partial class $safeitemrootname$: Component
    {    
        public $safeitemrootname$()
        {
            InitializeComponent();
        }

        public $safeitemrootname$(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
