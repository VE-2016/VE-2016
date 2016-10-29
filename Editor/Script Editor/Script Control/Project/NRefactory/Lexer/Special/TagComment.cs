// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.NRefactory.Parser
{
    /// <summary>
    /// Description of TagComment.	
    /// </summary>
    public class TagComment : Comment
    {
        private string _tag;

        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }

        public TagComment(string tag, string comment, Location startPosition, Location endPosition) : base(CommentType.SingleLine, comment, startPosition, endPosition)
        {
            _tag = tag;
        }
    }
}
