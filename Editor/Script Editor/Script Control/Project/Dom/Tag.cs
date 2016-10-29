// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.Dom
{
    public sealed class TagComment
    {
        private string _key;

        public string Key
        {
            get
            {
                return _key;
            }
        }

        private string _commentString;
        private DomRegion _region;

        public string CommentString
        {
            get
            {
                return _commentString;
            }
            set
            {
                _commentString = value;
            }
        }

        public DomRegion Region
        {
            get
            {
                return _region;
            }
            set
            {
                _region = value;
            }
        }

        public TagComment(string key, DomRegion region)
        {
            _key = key;
            _region = region;
        }
    }
}
