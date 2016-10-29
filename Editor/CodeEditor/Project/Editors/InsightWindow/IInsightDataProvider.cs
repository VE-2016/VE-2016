using System;

namespace AIMS.Libraries.CodeEditor.WinForms.InsightWindow
{
	public interface IInsightDataProvider
	{
        void SetupDataProvider(string fileName, EditViewControl textArea);
		
		bool CaretOffsetChanged();
		bool CharTyped();
		
		string GetInsightData(int number);
		
		int InsightDataCount {
			get;
		}
		
		int DefaultIndex {
			get;
		}
	}
}
