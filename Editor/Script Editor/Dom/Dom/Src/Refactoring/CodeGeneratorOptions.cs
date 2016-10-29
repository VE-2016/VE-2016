// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2500 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom.Refactoring
{
	public class CodeGeneratorOptions
	{
		public bool BracesOnSameLine = true;
		public bool EmptyLinesBetweenMembers = true;
		string indentString = "\t";
		
		public string IndentString {
			get { return indentString; }
			set {
				if (string.IsNullOrEmpty(value)) {
					throw new ArgumentNullException("value");
				}
				indentString = value;
			}
		}
	}
}
