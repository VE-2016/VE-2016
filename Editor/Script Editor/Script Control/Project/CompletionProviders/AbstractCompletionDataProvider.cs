// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.Windows.Forms;

using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.VBNet;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    public abstract class AbstractCompletionDataProvider : ICompletionDataProvider
    {
        public virtual ImageList ImageList
        {
            get
            {
                return null;
            }
        }

        private int _defaultIndex = -1;

        /// <summary>
        /// Gets the index of the element in the list that is chosen by default.
        /// </summary>
        public int DefaultIndex
        {
            get
            {
                return _defaultIndex;
            }
            set
            {
                _defaultIndex = value;
            }
        }

        protected string preSelection = null;

        public string PreSelection
        {
            get
            {
                return preSelection;
            }
        }

        private bool _insertSpace;

        /// <summary>
        /// Gets/Sets if a space should be inserted in front of the completed expression.
        /// </summary>
        public bool InsertSpace
        {
            get
            {
                return _insertSpace;
            }
            set
            {
                _insertSpace = value;
            }
        }

        /// <summary>
        /// Gets if pressing 'key' should trigger the insertion of the currently selected element.
        /// </summary>
        public virtual CompletionDataProviderKeyResult ProcessKey(char key)
        {
            CompletionDataProviderKeyResult res;
            if (key == ' ' && _insertSpace)
            {
                _insertSpace = false; // insert space only once
                res = CompletionDataProviderKeyResult.BeforeStartKey;
            }
            else if (char.IsLetterOrDigit(key) || key == '_')
            {
                _insertSpace = false; // don't insert space if user types normally
                res = CompletionDataProviderKeyResult.NormalKey;
            }
            else
            {
                // do not reset insertSpace when doing an insertion!
                res = CompletionDataProviderKeyResult.InsertionKey;
            }
            return res;
        }

        public virtual bool InsertAction(ICompletionData data, EditViewControl textArea, int insertionOffset, char key)
        {
            if (InsertSpace)
            {
                {
                    TextPoint loc = textArea.Document.IntPosToPoint(insertionOffset++);
                    textArea.Document.InsertText(" ", loc.X, loc.Y);
                }
            }
            textArea.Caret.Position = textArea.Document.IntPosToPoint(insertionOffset);

            return data.InsertAction(textArea, key);
        }

        /// <summary>
        /// Generates the completion data. This method is called by the text editor control.
        /// </summary>
        public abstract ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped);
    }

    public abstract class AbstractCodeCompletionDataProvider : AbstractCompletionDataProvider
    {
        private Hashtable _insertedElements = new Hashtable();
        private Hashtable _insertedPropertiesElements = new Hashtable();
        private Hashtable _insertedEventElements = new Hashtable();

        protected int caretLineNumber;
        protected int caretColumn;
        protected string fileName;

        protected ArrayList completionData = null;
        protected ExpressionContext overrideContext;

        /// <summary>
        /// Generates the completion data. This method is called by the text editor control.
        /// </summary>
        public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
        {
            completionData = new ArrayList();
            this.fileName = fileName;
            SyntaxDocument document = textArea.Document;

            // the parser works with 1 based coordinates

            caretLineNumber = textArea.Caret.Position.Y + 1;
            caretColumn = textArea.Caret.Position.X + 1;

            GenerateCompletionData(textArea, charTyped);
            string cWord = "";
            Word w = textArea._CodeEditor.Caret.CurrentWord;
            if (w != null)
                cWord = w.Text;
            ExpressionResult ex = GetExpression(textArea);
            if (charTyped == '\0' && completionData.Count > 0 && cWord != "." && ex.ToString().IndexOf('.') <= 0 && ex.Context == ExpressionContext.Default)
            {
                ArrayList LanguageKeyWords = textArea.ExtractKeywords();

                ArrayList org = (ArrayList)completionData.Clone();
                org.Sort();
                foreach (string keyword in LanguageKeyWords)
                {
                    int i = org.BinarySearch(new CodeCompletionData(keyword, "", AutoListIcons.iStructure));
                    if (i < 0)
                        i = ~i;
                    if (i >= 0 && i < org.Count)
                        if (((ICompletionData)org[i]).Text != keyword)
                            completionData.Add(new CodeCompletionData(keyword, "Keyword " + keyword, AutoListIcons.iOperator));
                }
            }
            return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
            //return (ICompletionData[])completionData.ToArray();
        }

        protected ExpressionResult GetExpression(EditViewControl textArea)
        {
            SyntaxDocument document = textArea.Document;
            IExpressionFinder expressionFinder = null;

            if (Parser.ProjectParser.CurrentProjectContent.Language == LanguageProperties.CSharp)
                expressionFinder = new CSharpExpressionFinder(this.fileName);
            else
                expressionFinder = new VBExpressionFinder();
            if (expressionFinder == null)
            {
                return new ExpressionResult(textArea.Caret.CurrentWord.Text);
            }
            else
            {
                TextRange range = new TextRange(0, 0, textArea.Caret.Position.X, textArea.Caret.Position.Y);
                ExpressionResult res = expressionFinder.FindExpression(document.GetRange(range), textArea.Caret.Offset - 1);
                if (overrideContext != null)
                    res.Context = overrideContext;
                return res;
            }
        }

        protected abstract void GenerateCompletionData(EditViewControl textArea, char charTyped);

        protected void AddResolveResults(ICollection list, ExpressionContext context)
        {
            if (list == null)
            {
                return;
            }
            completionData.Capacity += list.Count;
            CodeCompletionData suggestedData = null;
            foreach (object o in list)
            {
                if (context != null && !context.ShowEntry(o))
                    continue;
                CodeCompletionData ccd = CreateItem(o, context);
                if (object.Equals(o, context.SuggestedItem))
                    suggestedData = ccd;
                if (ccd != null && !ccd.Text.StartsWith("___"))
                    completionData.Add(ccd);
            }
            if (context.SuggestedItem != null)
            {
                if (suggestedData == null)
                {
                    suggestedData = CreateItem(context.SuggestedItem, context);
                    if (suggestedData != null)
                    {
                        completionData.Add(suggestedData);
                    }
                }
                if (suggestedData != null)
                {
                    completionData.Sort();
                    this.DefaultIndex = completionData.IndexOf(suggestedData);
                }
            }
        }

        private CodeCompletionData CreateItem(object o, ExpressionContext context)
        {
            if (o is string)
            {
                return new CodeCompletionData(o.ToString(), "Namespace " + o.ToString(), AutoListIcons.iNamespace);
            }
            else if (o is IClass)
            {
                return new CodeCompletionData((IClass)o);
            }
            else if (o is IProperty)
            {
                IProperty property = (IProperty)o;
                if (property.Name != null && _insertedPropertiesElements[property.Name] == null)
                {
                    _insertedPropertiesElements[property.Name] = property;
                    return new CodeCompletionData(property);
                }
            }
            else if (o is IMethod)
            {
                IMethod method = (IMethod)o;
                if (method.Name != null && !method.IsConstructor)
                {
                    CodeCompletionData ccd = new CodeCompletionData(method);
                    if (_insertedElements[method.Name] == null)
                    {
                        _insertedElements[method.Name] = ccd;
                        return ccd;
                    }
                    else
                    {
                        CodeCompletionData oldMethod = (CodeCompletionData)_insertedElements[method.Name];
                        ++oldMethod.Overloads;
                    }
                }
            }
            else if (o is IField)
            {
                return new CodeCompletionData((IField)o);
            }
            else if (o is IEvent)
            {
                IEvent e = (IEvent)o;
                if (e.Name != null && _insertedEventElements[e.Name] == null)
                {
                    _insertedEventElements[e.Name] = e;
                    return new CodeCompletionData(e);
                }
            }
            else
            {
                throw new ApplicationException("Unknown object: " + o);
            }
            return null;
        }

        protected void AddResolveResults(ResolveResult results, ExpressionContext context)
        {
            _insertedElements.Clear();
            _insertedPropertiesElements.Clear();
            _insertedEventElements.Clear();

            if (results != null)
            {
                AddResolveResults(results.GetCompletionData(Parser.ProjectParser.CurrentProjectContent), context);
            }
        }
    }
}
