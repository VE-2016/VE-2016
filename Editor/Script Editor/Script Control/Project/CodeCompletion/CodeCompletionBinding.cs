using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.Scripting.ScriptControl.CodeCompletion;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
    /// <summary>
    /// Interface that gives backend bindings the possibility to control what characters and
    /// keywords invoke code completion.
    /// </summary>
    public interface ICodeCompletionBinding
    {
        bool HandleKeyPress(CodeEditorControl editor, char ch);
    }

    /// <summary>
    /// Creates code completion bindings that manage code completion for one language.
    /// </summary>
    /// <attribute name="class" use="required">
    /// Name of the ICodeCompletionBinding class (normally deriving from DefaultCodeCompletionBinding).
    /// </attribute>
    /// <attribute name="extensions" use="optional">
    /// List of semicolon-separated entries of the file extensions handled by the binding.
    /// If no extensions attribute is specified, the binding is activated in all files.
    /// </attribute>
    /// <usage>Only in /AddIns/DefaultTextEditor/CodeCompletion</usage>
    /// <returns>
    /// The ICodeCompletionBinding class specified with the 'class' attribute, or a
    /// wrapper that lazy-loads the actual class when it is used in a file with the specified
    /// extension.
    /// </returns>

    public class DefaultCodeCompletionBinding : ICodeCompletionBinding
    {
        private bool _enableMethodInsight = true;
        private bool _enableIndexerInsight = true;
        private bool _enableXmlCommentCompletion = true;
        private bool _enableDotCompletion = true;

        public bool EnableMethodInsight
        {
            get
            {
                return _enableMethodInsight;
            }
            set
            {
                _enableMethodInsight = value;
            }
        }

        public bool EnableIndexerInsight
        {
            get
            {
                return _enableIndexerInsight;
            }
            set
            {
                _enableIndexerInsight = value;
            }
        }

        public bool EnableXmlCommentCompletion
        {
            get
            {
                return _enableXmlCommentCompletion;
            }
            set
            {
                _enableXmlCommentCompletion = value;
            }
        }

        public bool EnableDotCompletion
        {
            get
            {
                return _enableDotCompletion;
            }
            set
            {
                _enableDotCompletion = value;
            }
        }

        public virtual bool HandleKeyPress(CodeEditorControl editor, char ch)
        {
            switch (ch)
            {
                case '(':
                    if (_enableMethodInsight)
                    {
                        editor.ActiveViewControl.ShowInsightWindow(new MethodInsightDataProvider());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case '[':
                    if (_enableIndexerInsight)
                    {
                        editor.ActiveViewControl.ShowInsightWindow(new IndexerInsightDataProvider());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case '<':
                    if (_enableXmlCommentCompletion)
                    {
                        editor.ActiveViewControl.ShowCompletionWindow(new CommentCompletionDataProvider(), ch);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case '.':

                    if (_enableDotCompletion)
                    {
                        editor.ActiveViewControl.ShowCompletionWindow(new CodeCompletionDataProvider(), ch);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case ' ':

                    string word = editor.ActiveViewControl.Caret.CurrentWord.Text; //GetWordBeforeCaret();
                    if (word != null)
                        return HandleKeyword(editor, word);
                    else
                        return false;

                default:
                    return false;
            }
        }

        public virtual bool HandleKeyword(CodeEditorControl editor, string word)
        {
            // DefaultCodeCompletionBinding does not support Keyword handling, but this
            // method can be overridden
            return false;
        }
    }
}