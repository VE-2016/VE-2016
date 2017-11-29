// Copyright (c) 2009 Daniel Grunwald
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.AvalonEdit.Rendering;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace AvalonEdit.Editor
{
    /// <summary>
    /// Finds the references line and makes it gray.
    /// </summary>
    public class Colorizer : DocumentColorizingTransformer
    {
        private MatchCollection matches = null;

        private List<int> lines { get; set; }

        public TextView textView { get; set; }

        public void Comments()
        {
            var blockComments = @"/\\*.*?\\*/";

            matches = Regex.Matches(editorWindow.textEditor.Document.Text, blockComments, RegexOptions.Multiline);

            lines = new List<int>();

            foreach (Match match in matches)
            {
                int start = match.Index;
                int end = match.Index + match.Length;
                int starts = editorWindow.textEditor.Document.GetLineByOffset(start).LineNumber;
                int ends = editorWindow.textEditor.Document.GetLineByOffset(end).LineNumber;
                for (int i = starts; i <= ends; i++)
                    if (!lines.Contains(i))
                        lines.Add(i);
            }
            lines.Sort();
        }

        public Colorizer(EditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;
            this.textView = editorWindow.textEditor.TextArea.TextView;
            // textView.VisualLineConstructionStarting += TextView_VisualLineConstructionStarting;
            //textView.Document.TextChanged += TextView_VisualLineConstructionStarting;
        }

        private void TextView_VisualLineConstructionStarting(object sender, VisualLineConstructionStartEventArgs e)
        {
            // Comments();
        }

        public EditorWindow editorWindow { get; set; }

        public static System.Windows.Media.Brush brush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(111, 196, 220));

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            if (!line.IsDeleted)
            {
                if (lines != null)
                    if (lines.Count > 0)
                        if (lines.BinarySearch(line.LineNumber) >= 0)
                            return;

                string text = editorWindow.textEditor.Document.Text.Substring(line.Offset, line.EndOffset - line.Offset);

                var testusing = text.Trim();

                if (line.obs != null && testusing.StartsWith("  Reference"))

                //editorWindow.Errors[0].Location.SourceSpan.Start

                {
                    ChangeLinePart(line.Offset, line.EndOffset, ApplyChanges);
                }
                else
                {
                    SortedSet<int> sc = editorWindow.sc;

                    Dictionary<int, string> dc = editorWindow.dc;

                    if (sc.Count <= 0)
                        return;

                    if (testusing.StartsWith("using"))
                    {
                        ChangeLinePart(line.Offset + 5, line.EndOffset, ApplyChangesForUsing);
                        return;
                    }
                    if (testusing.StartsWith("//"))
                        return;

                    var matches = Regex.Matches(text, @"\b[a-zA-Z]{2,}\b");

                    foreach (Match s in matches)
                    {
                        if (sc.Contains(s.Value.GetHashCode()))
                        {
                            ChangeLinePart(line.Offset + s.Index, line.Offset + s.Index + s.Length, ApplyChangesForType);
                        }
                    }
                }
            }
        }

        private void ApplyChanges(VisualLineElement element)
        {
            element.TextRunProperties.SetForegroundBrush(Brushes.Gray);
        }

        private void ApplyChangesForUsing(VisualLineElement element)
        {
            element.TextRunProperties.SetForegroundBrush(Brushes.Black);
        }

        private void ApplyChangesForType(VisualLineElement element)
        {
            if (((SolidColorBrush)element.TextRunProperties.ForegroundBrush).Color != Colors.Green && ((SolidColorBrush)element.TextRunProperties.ForegroundBrush).Color != Colors.Maroon)
                element.TextRunProperties.SetForegroundBrush(brush);
        }
    }
}