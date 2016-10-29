// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    /// <summary>
    /// Provides code completion for attribute names.
    /// </summary>
    public class AttributesDataProvider : CtrlSpaceCompletionDataProvider
    {
        public AttributesDataProvider(IProjectContent pc)
            : this(ExpressionContext.TypeDerivingFrom(pc.GetClass("System.Attribute"), true))
        {
        }

        public AttributesDataProvider(ExpressionContext context) : base(context)
        {
            this.ForceNewExpression = true;
        }

        private bool _removeAttributeSuffix = true;

        public bool RemoveAttributeSuffix
        {
            get
            {
                return _removeAttributeSuffix;
            }
            set
            {
                _removeAttributeSuffix = value;
            }
        }

        public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
        {
            ICompletionData[] data = base.GenerateCompletionData(fileName, textArea, charTyped);
            if (_removeAttributeSuffix)
            {
                foreach (ICompletionData d in data)
                {
                    if (d.Text.EndsWith("Attribute"))
                    {
                        d.Text = d.Text.Substring(0, d.Text.Length - 9);
                    }
                }
            }
            return data;
        }
    }
}