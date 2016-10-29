// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{

	public interface IParameter: IComparable
	{
		string Name {
			get;
		}

		IReturnType ReturnType {
			get;
			set;
		}

		IList<IAttribute> Attributes {
			get;
		}

		ParameterModifiers Modifiers {
			get;
		}
		
		DomRegion Region {
			get;
		}
		
		string Documentation {
			get;
		}

		bool IsOut {
			get;
		}

		bool IsRef {
			get;
		}

		bool IsParams {
			get;
		}
		
		bool IsOptional {
			get;
		}
	}
}
