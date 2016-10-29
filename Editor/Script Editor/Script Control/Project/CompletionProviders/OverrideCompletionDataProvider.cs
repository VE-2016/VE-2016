using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.Dom.Refactoring;
using System;
using System.Collections.Generic;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    public class OverrideCompletionDataProvider : AbstractCompletionDataProvider
    {
        /// <summary>
        /// Gets a list of overridable methods from the specified class.
        /// A better location for this method is in the DefaultClass
        /// class and the IClass interface.
        /// </summary>
        public static IMethod[] GetOverridableMethods(IClass c)
        {
            if (c == null)
            {
                throw new ArgumentException("c");
            }

            List<IMethod> methods = new List<IMethod>();
            foreach (IMethod m in c.DefaultReturnType.GetMethods())
            {
                if (m.IsOverridable && !m.IsConst && !m.IsPrivate)
                {
                    if (m.DeclaringType.FullyQualifiedName != c.FullyQualifiedName)
                    {
                        methods.Add(m);
                    }
                }
            }
            return methods.ToArray();
        }

        /// <summary>
        /// Gets a list of overridable properties from the specified class.
        /// </summary>
        public static IProperty[] GetOverridableProperties(IClass c)
        {
            if (c == null)
            {
                throw new ArgumentException("c");
            }

            List<IProperty> properties = new List<IProperty>();
            foreach (IProperty p in c.DefaultReturnType.GetProperties())
            {
                if (p.IsOverridable && !p.IsConst && !p.IsPrivate)
                {
                    if (p.DeclaringType.FullyQualifiedName != c.FullyQualifiedName)
                    {
                        properties.Add(p);
                    }
                }
            }
            return properties.ToArray();
        }

        public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
        {
            IClass c = Parser.ProjectParser.GetParseInformation(fileName).ValidCompilationUnit.GetInnermostClass(textArea.Caret.Position.Y, textArea.Caret.Position.X);
            if (c == null) return null;
            List<ICompletionData> result = new List<ICompletionData>();
            foreach (IMethod m in GetOverridableMethods(c))
            {
                result.Add(new OverrideCompletionData(m));
            }
            foreach (IProperty p in GetOverridableProperties(c))
            {
                result.Add(new OverrideCompletionData(p));
            }
            return result.ToArray();
        }
    }

    public class OverrideCompletionData : DefaultCompletionData
    {
        private IMember _member;

        private static string GetName(IMethod method, ConversionFlags flags)
        {
            Parser.ProjectParser.CurrentAmbience.ConversionFlags = flags | ConversionFlags.ShowParameterNames;
            return Parser.ProjectParser.CurrentAmbience.Convert(method);
        }

        public OverrideCompletionData(IMethod method)
            : base(GetName(method, ConversionFlags.None),
                   "override " + GetName(method, ConversionFlags.ShowReturnType
                                         | ConversionFlags.ShowAccessibility)
                   + "\n\n" + method.Documentation,
                   ScriptControl.GetIcon(method))
        {
            _member = method;
        }

        public OverrideCompletionData(IProperty property)
            : base(property.Name, "override " + property.Name + "\n\n" + property.Documentation,
                   ScriptControl.GetIcon(property))
        {
            _member = property;
        }

        //public override bool InsertAction(EditViewControl textArea, char ch)
        //{
        //    ClassFinder context = new ClassFinder(textArea.FileName,
        //                                          textArea.Caret.Position.Y + 1, textArea.Caret.Position.X + 1);
        //    //int caretPosition = textArea.Caret.Offset;
        //    //LineSegment line = textArea.Document.GetLineSegment(textArea.Caret.Line);
        //    //string lineText = textArea.Document.GetText(line.Offset, caretPosition - line.Offset);
        //    string lineText = textArea.Caret.CurrentRow.Text;
        //    foreach (char c in lineText)
        //    {
        //        if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c))
        //        {
        //            return base.InsertAction(textArea, ch);
        //        }
        //    }
        //    string indentation = lineText.Substring(0, lineText.Length - lineText.TrimStart().Length);

        //    CodeGenerator codeGen = Parser.ProjectParser.CurrentProjectContent.Language.CodeGenerator;

        //    string text = codeGen.GenerateCode(codeGen.GetOverridingMethod(_member, context), indentation);
        //    text = text.TrimEnd(); // remove newline from end
        //    //TODO:Rajneesh
        //    TextRange tr = textArea.Document.GetRangeFromText(lineText, 0, textArea.Caret.Position.Y);

        //    tr = textArea.Document.ReplaceRange(tr, text, true);

        //    textArea.Caret.SetPos(new TextPoint(tr.LastColumn, tr.LastRow));
        //    textArea.ScrollIntoView();
        //    return true;
        //}
    }
}