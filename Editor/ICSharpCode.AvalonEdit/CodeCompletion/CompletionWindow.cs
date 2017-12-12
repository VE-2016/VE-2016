﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.AvalonEdit.CodeCompletion
{
    /// <summary>
    /// The code completion window.
    /// </summary>
    public class CompletionWindow : CompletionWindowBase
    {
        public CompletionList completionList = new CompletionList();
        private ToolTip toolTip = new ToolTip();

        /// <summary>
        /// Gets the completion list used in this completion window.
        /// </summary>
        public CompletionList CompletionList
        {
            get { return completionList; }
        }

        //public CompletionControl completionControl { get; set; }

        /// <summary>
        /// Creates a new code completion window.
        /// </summary>
        public CompletionWindow(TextArea textArea) : base(textArea)
        {
            // keep height automatic
            this.CloseAutomatically = true;
            this.SizeToContent = SizeToContent.Height;
            this.MaxHeight = 200;
            this.Width = 275;
            // prevent user from resizing window to 0x0
            this.MinHeight = 15;
            this.MinWidth = 30;

            toolTip.PlacementTarget = this;
            toolTip.Placement = PlacementMode.Right;
            toolTip.Closed += toolTip_Closed;

            this.Effect = null;

            this.BorderThickness = new Thickness(0, 0, 0, 0);
            this.BorderBrush = Brushes.Transparent;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.ResizeMode = ResizeMode.NoResize;

            this.completionWindow = this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="v"></param>
        public void MakeVisible(bool v = true)
        {
            if (v == true)
            {
                _AttachEvents();
                UpdatePosition();
            }
            else
            {
                _DetachEvents();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="completionControl"></param>
		public void SetContent(object completionControl)
        {
            this.Content = completionControl;
            this.Padding = new Thickness(0, 0, 0, 0);
        }

        #region ToolTip handling

        private void toolTip_Closed(object sender, RoutedEventArgs e)
        {
            // Clear content after tooltip is closed.
            // We cannot clear is immediately when setting IsOpen=false
            // because the tooltip uses an animation for closing.
            if (toolTip != null)
                toolTip.Content = null;
        }

        private void completionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = completionList.SelectedItem;
            if (item == null)
                return;
            object description = item.Description;
            if (description != null)
            {
                string descriptionText = description as string;
                if (descriptionText != null)
                {
                    toolTip.Content = new TextBlock
                    {
                        Text = descriptionText,
                        TextWrapping = TextWrapping.Wrap
                    };
                }
                else
                {
                    toolTip.Content = description;
                }
                toolTip.IsOpen = true;
            }
            else
            {
                toolTip.IsOpen = false;
            }
        }

        #endregion ToolTip handling

        private void completionList_InsertionRequested(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                this.Visibility = Visibility.Hidden;

                MakeVisible(false);
                //Close();
            }
            // The window must close before Complete() is called.
            // If the Complete callback pushes stacked input handlers, we don't want to pop those when the CC window closes.
            var item = completionList.SelectedItem;
            if (item != null)
                item.Complete(this.TextArea, new AnchorSegment(this.TextArea.Document, this.StartOffset - 1, this.EndOffset - this.StartOffset + 1), e);
        }

        /// <summary>
        ///
        /// </summary>
        public void _AttachEvents()
        {
            this.completionList.InsertionRequested += completionList_InsertionRequested;
            this.completionList.SelectionChanged += completionList_SelectionChanged;
            this.TextArea.Caret.PositionChanged += CaretPositionChanged;
            this.TextArea.MouseWheel += textArea_MouseWheel;
            this.TextArea.PreviewTextInput += textArea_PreviewTextInput;
            base._AttachEvents();
        }

        /// <summary>
        ///
        /// </summary>
		public void _DetachEvents()
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
            }
            this.completionList.InsertionRequested -= completionList_InsertionRequested;
            this.completionList.SelectionChanged -= completionList_SelectionChanged;
            this.TextArea.Caret.PositionChanged -= CaretPositionChanged;
            this.TextArea.MouseWheel -= textArea_MouseWheel;
            this.TextArea.PreviewTextInput -= textArea_PreviewTextInput;
            base._DetachEvents();
        }

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                toolTip = null;
            }
        }

        /// <inheritdoc/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (/*!e.Handled && */e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Visibility = Visibility.Hidden;
                MakeVisible(false);
                //Close();
            }
            if (!e.Handled)
            {
                completionList.HandleKey(e);
            }
        }

        private void textArea_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = RaiseEventPair(this, PreviewTextInputEvent, TextInputEvent,
                                       new TextCompositionEventArgs(e.Device, e.TextComposition));
        }

        private void textArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = RaiseEventPair(GetScrollEventTarget(),
                                       PreviewMouseWheelEvent, MouseWheelEvent,
                                       new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta));
        }

        private UIElement GetScrollEventTarget()
        {
            if (completionList == null)
                return this;
            return completionList.ScrollViewer ?? completionList.ListBox ?? (UIElement)completionList;
        }

        /// <summary>
        /// Gets/Sets whether the completion window should close automatically.
        /// The default value is true.
        /// </summary>
        public bool CloseAutomatically { get; set; }

        /// <inheritdoc/>
        protected override bool CloseOnFocusLost
        {
            get { return this.CloseAutomatically; }
        }

        /// <summary>
        /// When this flag is set, code completion closes if the caret moves to the
        /// beginning of the allowed range. This is useful in Ctrl+Space and "complete when typing",
        /// but not in dot-completion.
        /// Has no effect if CloseAutomatically is false.
        /// </summary>
        public bool CloseWhenCaretAtBeginning { get; set; }

        private void CaretPositionChanged(object sender, EventArgs e)
        {
            CloseWhenCaretAtBeginning = false;

            int offset = this.TextArea.Caret.Offset;
            //if (offset == this.StartOffset) {
            //	if (CloseAutomatically && CloseWhenCaretAtBeginning) {
            //                 if (this.Visibility == Visibility.Visible)
            //                 {
            //                     this.Visibility = Visibility.Hidden;
            //                     MakeVisible(false);
            //                 }
            //                         //Close();
            //	} else {
            //		completionList.SelectItem(string.Empty);
            //	}
            //	return;
            //}
            //if (offset < this.StartOffset || offset > this.EndOffset) {
            //		if (CloseAutomatically) {
            //                  if (this.Visibility == Visibility.Visible)
            //                  {
            //                      this.Visibility = Visibility.Hidden;
            //                      MakeVisible(false);
            //                  }
            //                          //Close();
            //              }
            //	} else {
            TextDocument document = this.TextArea.Document;
            if (document != null)
            {
                if (offset - this.StartOffset <= 0)
                {
                    this.Visibility = Visibility.Hidden;
                    MakeVisible(false);
                }
                else
                {
                    completionList.SelectItem(document.GetText(this.StartOffset, offset - this.StartOffset));
                    if (completionList.ListBox.Items.Count <= 0)
                    {
                        this.Visibility = Visibility.Hidden;
                        MakeVisible(false);
                    }
                }
            }
        }
    }
}