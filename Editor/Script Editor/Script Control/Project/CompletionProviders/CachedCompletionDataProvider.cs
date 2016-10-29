using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using System;
using System.Windows.Forms;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion
{
    public class CachedCompletionDataProvider : AbstractCompletionDataProvider
    {
        private ICompletionDataProvider _baseProvider;

        public CachedCompletionDataProvider(ICompletionDataProvider baseProvider)
        {
            _baseProvider = baseProvider;
        }

        private ICompletionData[] _completionData;

        public ICompletionData[] CompletionData
        {
            get
            {
                return _completionData;
            }
            set
            {
                _completionData = value;
            }
        }

        public override ImageList ImageList
        {
            get
            {
                return _baseProvider.ImageList;
            }
        }

        public override CompletionDataProviderKeyResult ProcessKey(char key)
        {
            return _baseProvider.ProcessKey(key);
        }

        public override bool InsertAction(ICompletionData data, EditViewControl textArea, int insertionOffset, char key)
        {
            return _baseProvider.InsertAction(data, textArea, insertionOffset, key);
        }

        public override ICompletionData[] GenerateCompletionData(string fileName, EditViewControl textArea, char charTyped)
        {
            if (_completionData == null)
            {
                _completionData = _baseProvider.GenerateCompletionData(fileName, textArea, charTyped);
                preSelection = _baseProvider.PreSelection;
                this.DefaultIndex = _baseProvider.DefaultIndex;
            }
            return _completionData;
        }

        [Obsolete("Cannot use InsertSpace on CachedCompletionDataProvider, please set it on the underlying provider!")]
        public new bool InsertSpace
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}