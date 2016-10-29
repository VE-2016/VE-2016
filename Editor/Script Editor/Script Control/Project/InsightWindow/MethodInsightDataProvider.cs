
using System;
using System.Collections;
using System.Collections.Generic;


using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.VBNet;
using AIMS.Libraries.CodeEditor;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms.InsightWindow;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    public class MethodInsightDataProvider : IInsightDataProvider
    {
        private string _fileName = null;
        private SyntaxDocument _document = null;
        private EditViewControl _textArea = null;
        protected List<IMethodOrProperty> methods = new List<IMethodOrProperty>();

        public List<IMethodOrProperty> Methods
        {
            get
            {
                return methods;
            }
        }

        public int InsightDataCount
        {
            get
            {
                return methods.Count;
            }
        }

        private int _defaultIndex = -1;

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

        public string GetInsightData(int number)
        {
            IMember method = methods[number];
            IAmbience conv = Parser.ProjectParser.CurrentAmbience;
            conv.ConversionFlags = ConversionFlags.StandardConversionFlags;
            string documentation = method.Documentation;
            string text;
            if (method is IMethod)
            {
                text = conv.Convert(method as IMethod);
            }
            else if (method is IProperty)
            {
                text = conv.Convert(method as IProperty);
            }
            else
            {
                text = method.ToString();
            }
            return text + "\n" + CodeCompletionData.GetDocumentation(documentation);
        }

        private int _lookupOffset;
        private bool _setupOnlyOnce;

        /// <summary>
        /// Creates a MethodInsightDataProvider looking at the caret position.
        /// </summary>
        public MethodInsightDataProvider()
        {
            _lookupOffset = -1;
        }

        /// <summary>
        /// Creates a MethodInsightDataProvider looking at the specified position.
        /// </summary>
        public MethodInsightDataProvider(int lookupOffset, bool setupOnlyOnce)
        {
            _lookupOffset = lookupOffset;
            _setupOnlyOnce = setupOnlyOnce;
        }

        private int _initialOffset;

        public void SetupDataProvider(string fileName, EditViewControl textArea)
        {
            if (_setupOnlyOnce && _textArea != null) return;
            SyntaxDocument document = textArea.Document;
            _fileName = fileName;
            _document = document;
            _textArea = textArea;
            int useOffset = (_lookupOffset < 0) ? textArea.Caret.Offset : _lookupOffset;
            _initialOffset = useOffset;


            IExpressionFinder expressionFinder = null;
            if (Parser.ProjectParser.CurrentProjectContent.Language == LanguageProperties.CSharp)
                expressionFinder = new CSharpExpressionFinder(fileName);
            else
                expressionFinder = new VBExpressionFinder();

            ExpressionResult expressionResult;
            if (expressionFinder == null)
                expressionResult = new ExpressionResult(textArea.Caret.CurrentWord.Text);//TextUtilities.GetExpressionBeforeOffset(textArea, useOffset));
            else
                expressionResult = expressionFinder.FindExpression(Parser.ProjectParser.GetFileContents(fileName), useOffset - 1);
            if (expressionResult.Expression == null) // expression is null when cursor is in string/comment
                return;
            expressionResult.Expression = expressionResult.Expression.Trim();

            // the parser works with 1 based coordinates
            TextPoint tp = document.IntPosToPoint(useOffset + 1);
            int caretLineNumber = tp.Y;
            int caretColumn = tp.X;
            SetupDataProvider(fileName, document, expressionResult, caretLineNumber, caretColumn);
        }

        protected virtual void SetupDataProvider(string fileName, SyntaxDocument document, ExpressionResult expressionResult, int caretLineNumber, int caretColumn)
        {
            bool constructorInsight = false;
            if (expressionResult.Context.IsAttributeContext)
            {
                constructorInsight = true;
            }
            else if (expressionResult.Context.IsObjectCreation)
            {
                constructorInsight = true;
                expressionResult.Context = ExpressionContext.Type;
            }

            ResolveResult results = Parser.ProjectParser.GetResolver().Resolve(expressionResult, caretLineNumber, caretColumn, fileName, document.Text);
            LanguageProperties language = Parser.ProjectParser.CurrentProjectContent.Language;


            IReturnType type = results.ResolvedType;
            if (type != null)
            {
                foreach (IProperty i in type.GetProperties())
                {
                    if (i.IsIndexer)
                    {
                        methods.Add(i);
                    }
                }
            }

            TypeResolveResult trr = results as TypeResolveResult;
            if (trr == null && language.AllowObjectConstructionOutsideContext)
            {
                if (results is MixedResolveResult)
                    trr = (results as MixedResolveResult).TypeResult;
            }
            if (trr != null && !constructorInsight)
            {
                if (language.AllowObjectConstructionOutsideContext)
                    constructorInsight = true;
            }
            if (constructorInsight)
            {
                if (trr == null)
                    return;
                foreach (IMethod method in trr.ResolvedType.GetMethods())
                {
                    if (method.IsConstructor && !method.IsStatic)
                    {
                        methods.Add(method);
                    }
                }

                if (methods.Count == 0 && trr.ResolvedClass != null && !trr.ResolvedClass.IsAbstract && !trr.ResolvedClass.IsStatic)
                {
                    // add default constructor
                    methods.Add(Constructor.CreateDefault(trr.ResolvedClass));
                }
            }
            else
            {
                MethodResolveResult result = results as MethodResolveResult;
                if (result == null)
                    return;
                bool classIsInInheritanceTree = false;
                if (result.CallingClass != null)
                    classIsInInheritanceTree = result.CallingClass.IsTypeInInheritanceTree(result.ContainingType.GetUnderlyingClass());
                foreach (IMethod method in result.ContainingType.GetMethods())
                {
                    if (language.NameComparer.Equals(method.Name, result.Name))
                    {
                        if (method.IsAccessible(result.CallingClass, classIsInInheritanceTree))
                        {
                            methods.Add(method);
                        }
                    }
                }
                if (methods.Count == 0 && result.CallingClass != null && language.SupportsExtensionMethods)
                {
                    ArrayList list = new ArrayList();
                    ResolveResult.AddExtensions(language, list, result.CallingClass, result.ContainingType);
                    foreach (IMethodOrProperty mp in list)
                    {
                        if (language.NameComparer.Equals(mp.Name, result.Name) && mp is IMethod)
                        {
                            IMethod m = (IMethod)mp.Clone();
                            m.Parameters.RemoveAt(0);
                            methods.Add(m);
                        }
                    }
                }
            }
        }

        public bool CaretOffsetChanged()
        {
            bool closeDataProvider = _textArea.Caret.Offset <= _initialOffset;
            int brackets = 0;
            int curlyBrackets = 0;
            if (!closeDataProvider)
            {
                bool insideChar = false;
                bool insideString = false;
                for (int offset = _initialOffset; offset < Math.Min(_textArea.Caret.Offset, _document.Text.Length); ++offset)
                {
                    char ch = (char)_document.Text.Substring(offset, 1).ToCharArray(0, 1)[0];
                    switch (ch)
                    {
                        case '\'':
                            insideChar = !insideChar;
                            break;
                        case '(':
                            if (!(insideChar || insideString))
                            {
                                ++brackets;
                            }
                            break;
                        case ')':
                            if (!(insideChar || insideString))
                            {
                                --brackets;
                            }
                            if (brackets <= 0)
                            {
                                return true;
                            }
                            break;
                        case '"':
                            insideString = !insideString;
                            break;
                        case '}':
                            if (!(insideChar || insideString))
                            {
                                --curlyBrackets;
                            }
                            if (curlyBrackets < 0)
                            {
                                return true;
                            }
                            break;
                        case '{':
                            if (!(insideChar || insideString))
                            {
                                ++curlyBrackets;
                            }
                            break;
                        case ';':
                            if (!(insideChar || insideString))
                            {
                                return true;
                            }
                            break;
                    }
                }
            }

            return closeDataProvider;
        }

        public bool CharTyped()
        {
            //			int offset = document.Caret.Offset - 1;
            //			if (offset >= 0) {
            //				return document.GetCharAt(offset) == ')';
            //			}
            return false;
        }
    }
}
