
using System;

namespace AIMS.Libraries.CodeEditor.WinForms.CompletionWindow
{
    public interface ICompletionData : IComparable
    {
        AutoListIcons ImageIndex
        {
            get;
        }

        string Text
        {
            get;
            set;
        }

        string Description
        {
            get;
        }

        /// <summary>
        /// Gets a priority value for the completion data item.
        /// When selecting items by their start characters, the item with the highest
        /// priority is selected first.
        /// </summary>
        double Priority
        {
            get;
        }

        /// <summary>
        /// Insert the element represented by the completion data into the text
        /// editor.
        /// </summary>
        /// <param name="textArea">TextArea to insert the completion data in.</param>
        /// <param name="ch">Character that should be inserted after the completion data.
        /// \0 when no character should be inserted.</param>
        /// <returns>Returns true when the insert action has processed the character
        /// <paramref name="ch"/>; false when the character was not processed.</returns>
        bool InsertAction(EditViewControl textArea, char ch);
    }

    public class DefaultCompletionData : ICompletionData
    {
        private string _text;
        private string _description;
        private AutoListIcons _imageIndex;

        public AutoListIcons ImageIndex
        {
            get
            {
                return _imageIndex;
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

        private double _priority;

        public double Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

        public virtual bool InsertAction(EditViewControl textArea, char ch)
        {
            textArea.InsertText(_text);
            return false;
        }

        public DefaultCompletionData(string text, string description, AutoListIcons imageIndex)
        {
            _text = text;
            _description = description;
            _imageIndex = imageIndex;
        }

        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is DefaultCompletionData))
            {
                return -1;
            }
            return _text.CompareTo(((DefaultCompletionData)obj).Text);
        }
    }
}
