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

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using WinExplorers;
using static AvalonEdit.Editor.CompletionControls;
using Microsoft.CodeAnalysis;
using VSProvider;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;

namespace AvalonEdit.Editor
{
	/// <summary>
	/// This class can be used to embed images inside AvalonEdit like this: <img src="Images/Save.png"/>
	/// </summary>
	public class ImageElementGenerator : VisualLineElementGenerator
	{
		readonly static Regex imageRegex = new Regex(@"<img src=""([\.\/\w\d]+)""/?>",
		                                             RegexOptions.IgnoreCase);
		readonly string basePath;

        public BitmapImage bitmapImage { get; set; }
		
        public EditorWindow editorWindow { get; set; }

		public ImageElementGenerator(string basePath)
		{
            bitmapImage = ConvertBitmapToBitmapImage.Convert(ve.folding);
			//if (basePath == null)
			//	throw new ArgumentNullException("basePath");
			this.basePath = basePath;
            
		}
		
		Match FindMatch(int startOffset)
		{
			// fetch the end offset of the VisualLine being generated
			int endOffset = CurrentContext.VisualLine.LastDocumentLine.EndOffset;
            ICSharpCode.AvalonEdit.Document.TextDocument document = CurrentContext.Document;
			string relevantText = document.GetText(startOffset, endOffset - startOffset);
			return imageRegex.Match(relevantText);
		}

        public int LineOfInterest = 0;

        public FrameworkElement frameworkElement { get; set; }

		/// <summary>
		/// Gets the first offset >= startOffset where the generator wants to construct
		/// an element.
		/// Return -1 to signal no interest.
		/// </summary>
		public override int GetFirstInterestedOffset(int startOffset)
		{
            if (LineOfInterest <= 0)
                return -1;
            ICSharpCode.AvalonEdit.Document.TextDocument document = CurrentContext.Document;
            int start = document.GetLineByNumber(LineOfInterest).Offset;
//            int length = document.GetLineByOffset(startOffset).Length;
//            string text = document.Text.Substring(start, length);
                
            

            if (start  == startOffset )
                return start;
            else
            {
            //    DocumentLine nextLine = document.GetLineByOffset(startOffset).NextLine;

              //  if (nextLine == null)
                    return -1;

            //    return nextLine.Offset;
            }
            
            //int firstOffset = CurrentContext.VisualLine.FirstDocumentLine.Offset;
            ////Match m = FindMatch(startOffset);
            //if (firstOffset >= startOffset)
            //    return firstOffset;
            //else return -1;
		}
		
		/// <summary>
		/// Constructs an element at the specified offset.
		/// May return null if no element should be constructed.
		/// </summary>
		public override VisualLineElement ConstructElement(int offset)
		{
            //Match m = FindMatch(offset);
            // check whether there's a match exactly at offset
            //if (m.Success && m.Index == 0) {
            ICSharpCode.AvalonEdit.Document.TextDocument document = CurrentContext.Document;
            int start = document.GetLineByOffset(offset).Offset;

            if (frameworkElement == null)
                return null;

            if (start  == offset )
            {
                DocumentLine documentLine = document.GetLineByOffset(offset);

           
                int height = (int)frameworkElement.Height;// (int)CurrentContext.VisualLine.Height;
                
    //            BitmapImage bitmap = bitmapImage;// LoadBitmap(m.Groups[1].Value);
				////if (bitmap != null) {
				//	Image image = new Image();
				//	image.Source = bitmap;
				//	image.Width = bitmap.PixelWidth;
    //            image.Height = height;// bitmap.PixelHeight;
    //                              // Pass the length of the match to the 'documentLength' parameter
    //                              // of InlineObjectElement.
                                  
                var es = new InlineObjectElementExtended(1/*m.Length*/, frameworkElement);

                //es.obs = documentLine.obs;

                //image.Tag = es;

                //es.Element.MouseDown += Element_MouseDown;
                

                    return es;
				}
			//}
			return null;
		}

        private void Element_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Image image = sender as Image;
            if (image == null)
                return;

            editorWindow.textEditor.SelectionLength = 0;

            InlineObjectElementExtended es = image.Tag as InlineObjectElementExtended;
            if (editorWindow == null)
                return;
            if (es == null)
                return;
            if(es.obs != null)
            {

                ISymbol symbol = es.obs as ISymbol;

                if(symbol != null)
                {

                    VSSolution vs = editorWindow.vs;

                    if (vs == null)
                        return;

                    var r = vs.GetAllSymbolReferences(symbol, null, null);

                    List<ReferencedSymbol> b = new List<ReferencedSymbol>();
                    foreach (var c in r)
                        b.Add(c);

                    MessageBox.Show("Items found ... " + b.Count);

                }

            }
            MessageBox.Show("Clicked ...");
        }

        //BitmapImage LoadBitmap(string fileName)
        //{
        //	// TODO: add some kind of cache to avoid reloading the image whenever the
        //	// VisualLine is reconstructed
        //	try {
        //		string fullFileName = Path.Combine(basePath, fileName);
        //		if (File.Exists(fullFileName)) {
        //			BitmapImage bitmap = new BitmapImage(new Uri(fullFileName));
        //			bitmap.Freeze();
        //			return bitmap;
        //		}
        //	} catch (ArgumentException) {
        //		// invalid filename syntax
        //	} catch (IOException) {
        //		// other IO error
        //	}
        //	return null;
        //}
    }

    public class InlineObjectElementExtended : InlineObjectElement
    {

        public object obs { get; set; }

        /// <summary>
		/// Creates a new InlineObjectElement.
		/// </summary>
		/// <param name="documentLength">The length of the element in the document. Must be non-negative.</param>
		/// <param name="element">The element to display.</param>
		public InlineObjectElementExtended(int documentLength, UIElement element) : base(documentLength, element)
		{
            if (element == null)
                throw new ArgumentNullException("element");
            
        }
    }
}
