using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.ServiceProcess;
using System.Text;
$if$ ($targetframeworkversion$ >= 4.5)using System.Threading.Tasks;
$endif$
namespace $safeprojectname$
{
    public partial class Service1: ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }
 
        protected override void OnStop()
        {
        }
    }
}
