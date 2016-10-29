// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

namespace AIMS.Libraries.Scripting.Dom
{
    public class DefaultComment : IComment
    {
        private bool _isBlockComment;
        private string _commentTag;
        private string _commentText;
        private DomRegion _region;

        public DefaultComment(bool isBlockComment, string commentTag, string commentText, DomRegion region)
        {
            _isBlockComment = isBlockComment;
            _commentTag = commentTag;
            _commentText = commentText;
            _region = region;
        }

        public bool IsBlockComment
        {
            get
            {
                return _isBlockComment;
            }
        }

        public string CommentTag
        {
            get
            {
                return _commentTag;
            }
        }

        public string CommentText
        {
            get
            {
                return _commentText;
            }
        }

        public DomRegion Region
        {
            get
            {
                return _region;
            }
        }
    }
}