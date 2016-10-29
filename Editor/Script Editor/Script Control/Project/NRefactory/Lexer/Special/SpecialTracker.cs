// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
    public class SpecialTracker
    {
        private List<ISpecial> _currentSpecials = new List<ISpecial>();

        private CommentType _currentCommentType;
        private StringBuilder _sb = new StringBuilder();
        private Location _startPosition;

        public List<ISpecial> CurrentSpecials
        {
            get
            {
                return _currentSpecials;
            }
        }

        public void InformToken(int kind)
        {
        }

        /// <summary>
        /// Gets the specials from the SpecialTracker and resets the lists.
        /// </summary>
        public List<ISpecial> RetrieveSpecials()
        {
            List<ISpecial> tmp = _currentSpecials;
            _currentSpecials = new List<ISpecial>();
            return tmp;
        }

        public void AddEndOfLine(Location point)
        {
            _currentSpecials.Add(new BlankLine(point));
        }

        public void AddPreprocessingDirective(string cmd, string arg, Location start, Location end)
        {
            _currentSpecials.Add(new PreprocessingDirective(cmd, arg, start, end));
        }

        // used for comment tracking
        public void StartComment(CommentType commentType, Location startPosition)
        {
            _currentCommentType = commentType;
            _startPosition = startPosition;
            _sb.Length = 0;
        }

        public void AddChar(char c)
        {
            _sb.Append(c);
        }

        public void AddString(string s)
        {
            _sb.Append(s);
        }

        public void FinishComment(Location endPosition)
        {
            _currentSpecials.Add(new Comment(_currentCommentType, _sb.ToString(), _startPosition, endPosition));
        }
    }
}
