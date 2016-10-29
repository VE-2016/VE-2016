// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
	[Serializable]
	[Flags]
	public enum ParameterModifiers : byte
	{
		// Values must be the same as in NRefactory's ParamModifiers
		None = 0,
		In  = 1,
		Out = 2,
		Ref = 4,
		Params = 8,
		Optional = 16
	}
}
