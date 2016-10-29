// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1966 $</version>
// </file>

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    /// <summary>
    /// Description of PrettyPrintOptions.	
    /// </summary>
    public class AbstractPrettyPrintOptions
    {
        private char _indentationChar = '\t';
        private int _tabSize = 4;
        private int _indentSize = 4;

        public char IndentationChar
        {
            get
            {
                return _indentationChar;
            }
            set
            {
                _indentationChar = value;
            }
        }

        public int TabSize
        {
            get
            {
                return _tabSize;
            }
            set
            {
                _tabSize = value;
            }
        }

        public int IndentSize
        {
            get
            {
                return _indentSize;
            }
            set
            {
                _indentSize = value;
            }
        }
    }
}
