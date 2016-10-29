// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2066 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.Dom
{
	public interface IMethodOrProperty : IMember
	{
		IList<IParameter> Parameters {
			get;
		}
		
		bool IsExtensionMethod {
			get;
		}
	}
	
	public interface IMethod : IMethodOrProperty
	{
		IList<ITypeParameter> TypeParameters {
			get;
		}
		
		bool IsConstructor {
			get;
		}
	}
}
