using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    /// <summary>
    ///
    /// </summary>
    public class TextBlockExtended : TextBlock
    {
        /// <summary>
        ///
        /// </summary>
        public TextBlockExtended()
        {
            this.Initialized += TextBlockExtended_Initialized;
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty SearchProperty =
             DependencyProperty.Register("Search", typeof(string),
             typeof(TextBlockExtended), new FrameworkPropertyMetadata(default(string), OnItemsPropertyChanged));

        private static void OnItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // AutocompleteTextBox source = d as AutocompleteTextBox;
            // Do something...
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty DataProperty =
             DependencyProperty.Register("Data", typeof(object),
             typeof(TextBlockExtended), new FrameworkPropertyMetadata(default(string)));

        /// <summary>
        ///
        /// </summary>
        public string Search
        {
            get { return (string)GetValue(SearchProperty); }
            set { SetValue(SearchProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        private void TextBlockExtended_Initialized(object sender, EventArgs e)
        {
            if (Data != null)
                if (sender != null)
                {
                    ICompletionData d = Data as ICompletionData;
                    if (d != null)
                    {
                        d.textBlock = this;
                        Search = d.Search;
                    }
                }

            if (string.IsNullOrEmpty(Text))
                return;
            if (string.IsNullOrEmpty(Search))
                return;

            this.Inlines.Clear();

            int i = Text.IndexOf(Search, StringComparison.InvariantCultureIgnoreCase);

            if (i < 0)
            {
                this.Inlines.Add(new Run(Text));

                return;
            }

            string name = "";

            string text = Text.Substring(0, i);

            string bold = Text.Substring(i, Search.Length);

            this.Inlines.Add(new Run(text));

            this.Inlines.Add(new Bold(new Run(bold)));

            if (Text.Length - i - Search.Length > 0)
            {
                string ns = Text.Substring(i + bold.Length, Text.Length - i - bold.Length);
                this.Inlines.Add(new Run(ns));
            }

            //this.InvalidateVisual();
        }

        /// <summary>
        ///
        /// </summary>
        public void Update(string Search)
        {
            TextBlockExtended_Initialized(null, null);
        }
    }
}