// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.NRefactory
{
    public class Comment : AbstractSpecial
    {
        private CommentType _commentType;
        private string _comment;

        public CommentType CommentType
        {
            get
            {
                return _commentType;
            }
            set
            {
                _commentType = value;
            }
        }

        public string CommentText
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        public Comment(CommentType commentType, string comment, Location startPosition, Location endPosition)
            : base(startPosition, endPosition)
        {
            _commentType = commentType;
            _comment = comment;
        }

        public override string ToString()
        {
            return String.Format("[{0}: Type = {1}, Text = {2}, Start = {3}, End = {4}]",
                                 GetType().Name, CommentType, CommentText, StartPosition, EndPosition);
        }

        public override object AcceptVisitor(ISpecialVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}
