using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$
namespace $rootnamespace$
{
    [RunInstaller(true)]
    public partial class $safeitemrootname$: System.Configuration.Install.Installer
    {
        public $safeitemrootname$()
        {
            InitializeComponent();
        }
    }
}
