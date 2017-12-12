using System;
using System.Collections.Generic;
$if$ ($targetframeworkversion$ >= 3.5)using System.Linq;
$endif$using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace $rootnamespace$
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "$safeitemrootname$" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select $itemrootname$.svc or $itemrootname$.svc.cs at the Solution Explorer and start debugging.
	public class $safeitemrootname$ : $contractName$
	{
		public void DoWork()
		{
		}
	}
}
