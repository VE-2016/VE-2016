// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

namespace AIMS.Libraries.Scripting.Dom
{
    public sealed class FoldingRegion
    {
        private string _name;
        private DomRegion _region;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public DomRegion Region
        {
            get
            {
                return _region;
            }
        }

        public FoldingRegion(string name, DomRegion region)
        {
            _name = name;
            _region = region;
        }
    }
}