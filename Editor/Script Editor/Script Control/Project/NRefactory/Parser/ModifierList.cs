// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1611 $</version>
// </file>

using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
    internal class ModifierList
    {
        private Modifiers _cur;
        private Location _location = new Location(-1, -1);

        public Modifiers Modifier
        {
            get
            {
                return _cur;
            }
        }

        public Location GetDeclarationLocation(Location keywordLocation)
        {
            if (_location.X == -1 && _location.Y == -1)
            {
                return keywordLocation;
            }
            return _location;
        }

        //		public Location Location {
        //			get {
        //				return location;
        //			}
        //			set {
        //				location = value;
        //			}
        //		}

        public bool isNone { get { return _cur == Modifiers.None; } }

        public bool Contains(Modifiers m)
        {
            return ((_cur & m) != 0);
        }

        public void Add(Modifiers m, Location tokenLocation)
        {
            if (_location.X == -1 && _location.Y == -1)
            {
                _location = tokenLocation;
            }

            if ((_cur & m) == 0)
            {
                _cur |= m;
            }
            else
            {
                //				parser.Error("modifier " + m + " already defined");
            }
        }

        //		public void Add(Modifiers m)
        //		{
        //			Add(m.cur, m.Location);
        //		}

        public void Check(Modifiers allowed)
        {
            Modifiers wrong = _cur & ~allowed;
            if (wrong != Modifiers.None)
            {
                //				parser.Error("modifier(s) " + wrong + " not allowed here");
            }
        }
    }
}
