using System;
using System.Collections.Generic;
using System.Text;
using AIMS.Libraries.Scripting.Dom;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
    internal class ProjectContentItem
    {
        private string _filename = "";
        private string _content = "";
        private bool _isopened = false;
        private ParseInformation _parseInfo = null;
        public ProjectContentItem(string fileName) : this(fileName, "", false)
        {
        }

        public ProjectContentItem(string fileName, bool Isopened) : this(fileName, "", Isopened)
        {
        }

        public ProjectContentItem(string fileName, string filecontent) : this(fileName, filecontent, false)
        {
        }

        public ProjectContentItem(string fileName, string filecontent, bool Isopened)
        {
            _filename = fileName;
            _content = filecontent;
            _isopened = Isopened;
        }

        public string FileName
        {
            get { return _filename; }
            set { _filename = value; }
        }
        public bool IsOpened
        {
            get { return _isopened; }
            set { _isopened = value; }
        }
        public string Contents
        {
            get { return _content; }
            set { _content = value; }
        }

        public ParseInformation ParsedContents
        {
            get { return _parseInfo; }
            set { _parseInfo = value; }
        }
    }
}
