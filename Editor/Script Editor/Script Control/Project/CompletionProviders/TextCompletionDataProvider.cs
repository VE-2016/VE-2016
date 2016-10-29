using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    /// <summary>
    /// Data provider for code completion.
    /// </summary>
    public class TextCompletionDataProvider : AbstractCompletionDataProvider
    {
        private string[] _texts;

        public TextCompletionDataProvider(params string[] texts)
        {
            _texts = texts;
        }

        public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
        {
            ICompletionData[] data = new ICompletionData[_texts.Length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new DefaultCompletionData(_texts[i], null, AutoListIcons.iClassShortCut);
            }
            return data;
        }
    }
}