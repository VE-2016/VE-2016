using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using System.Collections;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    /// <summary>
    /// Data provider for code completion.
    /// </summary>
    public class CommentCompletionDataProvider : AbstractCompletionDataProvider
    {
        private string[][] _commentTags = new string[][] {
            new string[] {"c", "marks text as code"},
            new string[] {"code", "marks text as code"},
            new string[] {"example", "A description of the code example\n(must have a <code> tag inside)"},
            new string[] {"exception cref=\"\"", "description to an exception thrown"},
            new string[] {"list type=\"\"", "A list"},
            new string[] {"listheader", "The header from the list"},
            new string[] {"item", "A list item"},
            new string[] {"term", "A term in a list"},
            new string[] {"description", "A description to a term in a list"},
            new string[] {"para", "A text paragraph"},
            new string[] {"param name=\"\"", "A description for a parameter"},
            new string[] {"paramref name=\"\"", "A reference to a parameter"},
            new string[] {"permission cref=\"\"", ""},
            new string[] {"remarks", "Gives description for a member"},
            new string[] {"include file=\"\" path=\"\"", "Includes comments from other files"},
            new string[] {"returns", "Gives description for a return value"},
            new string[] {"see cref=\"\"", "A reference to a member"},
            new string[] {"seealso cref=\"\"", "A reference to a member in the seealso section"},
            new string[] {"summary", "A summary of the object"},
            new string[] {"value", "A description of a property"}
        };

        /// <remarks>
        /// Returns true, if the given coordinates (row, column) are in the region.
        /// </remarks>
        private bool IsBetween(int row, int column, DomRegion region)
        {
            return row >= region.BeginLine && (row <= region.EndLine || region.EndLine == -1);
        }

        public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
        {
            //caretLineNumber = textArea.Caret.Line;
            //caretColumn     = textArea.Caret.Column;
            //LineSegment caretLine = textArea.Document.GetLineSegment(caretLineNumber);
            //string lineText = textArea.Document.GetText(caretLine.Offset, caretLine.Length);
            string lineText = textArea.Caret.CurrentRow.Text;

            if (!lineText.Trim().StartsWith("///") && !lineText.Trim().StartsWith("'''"))
            {
                return null;
            }

            ArrayList completionData = new ArrayList();
            foreach (string[] tag in _commentTags)
            {
                completionData.Add(new CommentCompletionData(tag[0], tag[1]));
            }
            return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
        }

        private class CommentCompletionData : ICompletionData
        {
            private string _text;
            private string _description;

            public AutoListIcons ImageIndex
            {
                get
                {
                    return AutoListIcons.iMethod;
                }
            }

            public string Text
            {
                get
                {
                    return _text;
                }
                set
                {
                    _text = value;
                }
            }

            public string Description
            {
                get
                {
                    return _description;
                }
            }

            public double Priority
            {
                get
                {
                    return 0;
                }
            }

            public bool InsertAction(EditViewControl textArea, char ch)
            {
                textArea.InsertText(_text);
                return false;
            }

            public CommentCompletionData(string text, string description)
            {
                _text = text;
                _description = description;
            }

            #region System.IComparable interface implementation

            public int CompareTo(object obj)
            {
                if (obj == null || !(obj is CommentCompletionData))
                {
                    return -1;
                }
                return _text.CompareTo(((CommentCompletionData)obj)._text);
            }

            #endregion System.IComparable interface implementation
        }
    }
}