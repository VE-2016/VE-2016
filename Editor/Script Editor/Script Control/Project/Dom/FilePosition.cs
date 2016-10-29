// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1661 $</version>
// </file>

using AIMS.Libraries.Scripting.NRefactory;
using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public struct FilePosition
    {
        private string _filename;
        private Location _position;
        private ICompilationUnit _compilationUnit;

        public static readonly FilePosition Empty = new FilePosition(null, Location.Empty);

        public FilePosition(ICompilationUnit compilationUnit, int line, int column)
        {
            _position = new Location(column, line);
            _compilationUnit = compilationUnit;
            if (compilationUnit != null)
            {
                _filename = compilationUnit.FileName;
            }
            else
            {
                _filename = null;
            }
        }

        public FilePosition(string filename)
            : this(filename, Location.Empty)
        {
        }

        public FilePosition(string filename, int line, int column)
            : this(filename, new Location(column, line))
        {
        }

        public FilePosition(string filename, Location position)
        {
            _compilationUnit = null;
            _filename = filename;
            _position = position;
        }

        public string FileName
        {
            get
            {
                return _filename;
            }
        }

        public ICompilationUnit CompilationUnit
        {
            get
            {
                return _compilationUnit;
            }
        }

        public Location Position
        {
            get
            {
                return _position;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} : (line {1}, col {2})",
                                 _filename,
                                 Line,
                                 Column);
        }

        public int Line
        {
            get
            {
                return _position.Y;
            }
        }

        public int Column
        {
            get
            {
                return _position.X;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _filename == null;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FilePosition)) return false;
            FilePosition b = (FilePosition)obj;
            return this.FileName == b.FileName && this.Position == b.Position;
        }

        public override int GetHashCode()
        {
            return _filename.GetHashCode() ^ _position.GetHashCode();
        }
    }
}