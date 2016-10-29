// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2087 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
    /// <summary>
    /// A line/column position.
    /// </summary>
    public struct Location : IComparable<Location>, IEquatable<Location>
    {
        public static readonly Location Empty = new Location(-1, -1);

        public Location(int column, int line)
        {
            _x = column;
            _y = line;
        }

        private int _x,_y;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Line
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Column
        {
            get { return _x; }
            set { _x = value; }
        }

        public bool IsEmpty
        {
            get
            {
                return _x <= 0 && _y <= 0;
            }
        }

        public override string ToString()
        {
            return string.Format("(Line {1}, Col {0})", _x, _y);
        }

        public override int GetHashCode()
        {
            return unchecked(87 * _x.GetHashCode() ^ _y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location)) return false;
            return (Location)obj == this;
        }

        public bool Equals(Location other)
        {
            return this == other;
        }

        public static bool operator ==(Location a, Location b)
        {
            return a._x == b._x && a._y == b._y;
        }

        public static bool operator !=(Location a, Location b)
        {
            return a._x != b._x || a._y != b._y;
        }

        public static bool operator <(Location a, Location b)
        {
            if (a._y < b._y)
                return true;
            else if (a._y == b._y)
                return a._x < b._x;
            else
                return false;
        }

        public static bool operator >(Location a, Location b)
        {
            if (a._y > b._y)
                return true;
            else if (a._y == b._y)
                return a._x > b._x;
            else
                return false;
        }

        public static bool operator <=(Location a, Location b)
        {
            return !(a > b);
        }

        public static bool operator >=(Location a, Location b)
        {
            return !(a < b);
        }

        public int CompareTo(Location other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            else
                return 1;
        }
    }
}
