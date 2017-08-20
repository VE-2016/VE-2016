//Coded by Rajneesh Noonia 2007

#region using...

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using AIMS.Libraries.CodeEditor.Core;
using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.TextDraw;
using AIMS.Libraries.CodeEditor.Core.Timers;
using AIMS.Libraries.CodeEditor.Win32;

using VSProvider;
using GACProject;
using ICSharpCode.NRefactory;
using System.Runtime.InteropServices;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using System.Threading.Tasks;
//using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.DotNet.CodeFormatting;
using System.Threading;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using AIMS.Libraries.CodeEditor.Editors;

#endregion

namespace AIMS.Libraries.CodeEditor
{
    /// <summary>
    /// Syntaxbox control that can be used as a pure text editor or as a code editor when a syntaxfile is used.
    /// </summary>
    [Designer(typeof(CodeEditorDesigner), typeof(IDesigner))]
    public class CodeEditorControl : Widget
    {
        protected internal bool DisableIntelliMouse = false;
        protected internal bool DisableFindForm = false;
        protected internal bool DisableAutoList = false;
        protected internal bool DisableInfoTip = false;
        protected internal bool DisableSplitView = false;
        protected internal bool DisableScrollBars = false;

        public event EventHandler FileNameChanged;
        public event EventHandler FileSavedChanged;


        #region General Declarations

        private IContainer _components;
        private ArrayList _Views = null;
        private SyntaxDocument _Document = null;
        private int _TooltipDelay = 240;
        private int _TabSize = 4;
        private int _GutterMarginWidth = 19;
        private int _SmoothScrollSpeed = 2;
        private int _RowPadding = 0;
        private long _ticks = 0; //splitter doubleclick timer
        private bool _ShowWhitespace = false;
        private bool _ShowTabGuides = false;
        private bool _ShowLineNumbers = false;
        private bool _ShowGutterMargin = true;//false;
        private bool _ReadOnly = false;
        private bool _HighLightActiveLine = true;
        private bool _VirtualWhitespace = false;
        private bool _BracketMatching = true;
        //private bool _OverWrite = false;
        private bool _ParseOnPaste = false;
        private bool _SmoothScroll = false;
        private bool _AllowBreakPoints = true;
        private bool _LockCursorUpdate = false;
        private Color _BracketBorderColor = Color.Transparent;// Color.DarkBlue;
        private Color _TabGuideColor = ControlPaint.Light(SystemColors.ControlLight);
        private Color _OutlineColor = SystemColors.ControlDark;
        private Color _WhitespaceColor = SystemColors.ControlDark;
        private Color _SeparatorColor = SystemColors.Control;
        private Color _SelectionBackColor = Color.FromArgb(50, 173, 216, 230);
        private Color _SelectionForeColor = Color.Black;// SystemColors.HighlightText;
        private Color _InactiveSelectionBackColor = SystemColors.ControlDark;
        private Color _InactiveSelectionForeColor = SystemColors.ControlLight;
        private Color _BreakPointBackColor = Color.DarkRed;
        private Color _BreakPointForeColor = Color.White;
        private Color _BackColor = Color.White;
        private Color _HighLightedLineColor = Color.LightBlue;
        private Color _GutterMarginColor = SystemColors.Control;
        private Color _LineNumberBackColor = SystemColors.Window;
        private Color _LineNumberForeColor = Color.Teal;
        private Color _GutterMarginBorderColor = SystemColors.ControlDark;
        private Color _LineNumberBorderColor = Color.Teal;
        private Color _BracketForeColor = Color.Black;
        private Color _BracketBackColor = Color.LightGray;// Color.LightSteelBlue;
        private Color _ScopeBackColor = Color.Transparent;
        private Color _ScopeIndicatorColor = Color.Transparent;
        private TextDrawType _TextDrawStyle = 0;
        private IndentStyle _Indent = IndentStyle.LastRow;
        private string _FontName = "Consolas";//"Courier New";
        private float _FontSize = 10f;//10f;
        public EditViewControl _ActiveView = null;
        private KeyboardActionList _KeyboardActions = new KeyboardActionList();


        public static LineMarginRender defaultLineMarginRender = new LineMarginRender();


        private string[] _textBorderStyles = new string[]
            {
                "****** * ******* * ******",
                "+---+| | |+-+-+| | |+---+",
                "+---+\u00A6 \u00A6 \u00A6\u00A6-+-\u00A6\u00A6 \u00A6 \u00A6+---+",
                "+---+\u00A6 \u00A6 \u00A6+-+-\u00A6\u00A6 \u00A6 \u00A6+---+"
            };

        #endregion

        #region Internal Components/Controls

        private SplitViewWidget _splitView1;
        private EditViewControl _upperLeft;
        private EditViewControl _upperRight;
        private EditViewControl _lowerLeft;
        private EditViewControl _lowerRight;
        private ImageList _GutterIcons;
        private WeakTimer _parseTimer;
        private ImageList _AutoListIcons;

        #endregion

        #region Public Events

        /// <summary>
        /// An event that is fired when the cursor hovers a pattern;
        /// </summary>
        public event WordMouseHandler WordMouseHover = null;

        /// <summary>
        /// An event that is fired when the cursor hovers a pattern;
        /// </summary>
        public event WordMouseHandler WordMouseDown = null;

        /// <summary>
        /// An event that is fired when the control has updated the clipboard
        /// </summary>
        public event CopyHandler ClipboardUpdated = null;

        /// <summary>
        /// Event fired when the caret of the active view have moved.
        /// </summary>
        public event EventHandler CaretChange = null;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler SelectionChange = null;

        /// <summary>
        /// Event fired when the user presses the up or the down button on the infotip.
        /// </summary>
        public event EventHandler InfoTipSelectedIndexChanged = null;

        /// <summary>
        /// Event fired when a row is rendered.
        /// </summary>
        public event RowPaintHandler RenderRow = null;

        /// <summary>
        /// An event that is fired when mouse down occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseDown = null;

        /// <summary>
        /// An event that is fired when mouse move occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseMove = null;

        /// <summary>
        /// An event that is fired when mouse up occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseUp = null;

        /// <summary>
        /// An event that is fired when a click occurs on a row
        /// </summary>
        public event RowMouseHandler RowClick = null;

        /// <summary>
        /// An event that is fired when a double click occurs on a row
        /// </summary>
        public event RowMouseHandler RowDoubleClick = null;

        #endregion //END PUBLIC EGENTS

        #region Public Properties

        #region PUBLIC PROPERTY SHOWEOLMARKER

        private bool _ShowEOLMarker = false;

        [Category("Appearance"),
            Description("Determines if a \u00B6 should be displayed at the end of a line")]
        [DefaultValue(false)]
        public bool ShowEOLMarker
        {
            get { return _ShowEOLMarker; }
            set
            {
                _ShowEOLMarker = value;
                this.Redraw();
            }
        }

        #endregion

        #region PUBLIC PROPERTY EOLMARKERCOLOR

        private Color _EOLMarkerColor = Color.Red;

        [Category("Appearance"),
            Description("The color of the EOL marker")]
        [DefaultValue(typeof(Color), "Red")]
        public Color EOLMarkerColor
        {
            get { return _EOLMarkerColor; }
            set
            {
                _EOLMarkerColor = value;
                this.Redraw();
            }
        }

        #endregion

        #region PUBLIC PROPERTY AUTOLISTAUTOSELECT

        private bool _AutoListAutoSelect = true;

        [DefaultValue(true)]
        public bool AutoListAutoSelect
        {
            get { return _AutoListAutoSelect; }
            set { _AutoListAutoSelect = value; }
        }

        #endregion

        #region PUBLIC PROPERTY COPYASRTF

        private bool _CopyAsRTF = false;

        [Category("Behavior - Clipboard"),
            Description("determines if the copy actions should be stored as RTF")]
        [DefaultValue(typeof(Color), "false")]
        public bool CopyAsRTF
        {
            get { return _CopyAsRTF; }
            set { _CopyAsRTF = value; }
        }

        #endregion

        [Category("Appearance - Scopes"),
            Description("The color of the active scope")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color ScopeBackColor
        {
            get { return _ScopeBackColor; }
            set
            {
                _ScopeBackColor = value;
                this.Redraw();
            }
        }

        [Category("Appearance - Scopes"),
            Description("The color of the scope indicator")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color ScopeIndicatorColor
        {
            get { return _ScopeIndicatorColor; }
            set
            {
                _ScopeIndicatorColor = value;
                this.Redraw();
            }
        }

        #region PUBLIC PROPERTY SHOWSCOPEINDICATOR

        private bool _ShowScopeIndicator;

        [Category("Appearance - Scopes"), Description(
            "Determines if the scope indicator should be shown")
            ]
        [DefaultValue(true)]
        public bool ShowScopeIndicator
        {
            get { return _ShowScopeIndicator; }
            set
            {
                _ShowScopeIndicator = value;
                this.Redraw();
            }
        }

        #endregion

        /// <summary>
        /// Positions the AutoList
        /// </summary>
        [Category("Behavior")]
        [Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextPoint AutoListPosition
        {
            get { return _ActiveView.AutoListPosition; }
            set
            {
                if (_ActiveView == null)
                    return;

                _ActiveView.AutoListPosition = value;
            }
        }

        /// <summary>
        /// Positions the InfoTip
        /// </summary>
        [Category("Behavior")]
        [Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextPoint InfoTipPosition
        {
            get { return _ActiveView.InfoTipPosition; }
            set
            {
                if (_ActiveView == null)
                    return;

                _ActiveView.InfoTipPosition = value;
            }
        }

        [Browsable(false)]
        public int SplitviewV
        {
            get { return _splitView1.SplitviewV; }
            set
            {
                if (_splitView1 == null)
                    return;

                _splitView1.SplitviewV = value;
            }
        }

        [Browsable(false)]
        public int SplitviewH
        {
            get { return _splitView1.SplitviewH; }
            set
            {
                if (_splitView1 == null)
                    return;
                _splitView1.SplitviewH = value;
            }
        }


        /// <summary>
        /// Gets or Sets the active view
        /// </summary>
        [Browsable(false)]
        // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ActiveView ActiveView
        {
            get
            {
                if (_ActiveView == _upperLeft)
                    return ActiveView.TopLeft;

                if (_ActiveView == _upperRight)
                    return ActiveView.TopRight;

                if (_ActiveView == _lowerLeft)
                    return ActiveView.BottomLeft;

                if (_ActiveView == _lowerRight)
                    return ActiveView.BottomRight;

                return (ActiveView)0;
            }
            set
            {
                if (value != ActiveView.BottomRight)
                {
                    ActivateSplits();
                }


                if (value == ActiveView.TopLeft)
                    _ActiveView = _upperLeft;

                if (value == ActiveView.TopRight)
                    _ActiveView = _upperRight;

                if (value == ActiveView.BottomLeft)
                    _ActiveView = _lowerLeft;

                if (value == ActiveView.BottomRight)
                    _ActiveView = _lowerRight;
            }
        }

        /// <summary>
        /// Prevents the control from changing the cursor.
        /// </summary>
        [Description("Prevents the control from changing the cursor.")]
        [Category("Appearance")]
        [Browsable(false)]
        public bool LockCursorUpdate
        {
            get { return _LockCursorUpdate; }
            set { _LockCursorUpdate = value; }
        }

        /// <summary>
        /// The row padding in pixels.
        /// </summary>
        [Category("Appearance"),
            Description("The number of pixels to add between rows")]
        [DefaultValue(0)]
        public int RowPadding
        {
            get { return _RowPadding; }
            set { _RowPadding = value; }
        }


        ///// <summary>
        ///// The selected index in the infotip.
        ///// </summary>
        //[Category("Appearance - Infotip"),
        //	Description("The currently active selection in the infotip")]
        //[Browsable(false)]
        //public int InfoTipSelectedIndex
        //{
        //	get { return _ActiveView.InfoTip.SelectedIndex; }
        //	set
        //	{
        //		if (_ActiveView == null || _ActiveView.InfoTip == null)
        //			return;

        //		_ActiveView.InfoTip.SelectedIndex = value;
        //	}
        //}

        ///// <summary>
        ///// Gets or Sets the image used in the infotip.
        ///// </summary>
        //[Category("Appearance - InfoTip"),
        //	Description("An image to show in the infotip")]
        //[DefaultValue(null)]
        //     // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public Image InfoTipImage
        //{
        //	get { return _ActiveView.InfoTip.Image; }
        //	set
        //	{
        //		if (_ActiveView == null || _ActiveView.InfoTip == null)
        //			return;


        //		_ActiveView.InfoTip.Image = value;
        //	}
        //}

        ///// <summary>
        ///// Get or Sets the number of choices that could be made in the infotip.
        ///// </summary>
        //[Category("Appearance"),
        //	Description("Get or Sets the number of choices that could be made in the infotip")]
        //[Browsable(false)]
        //public int InfoTipCount
        //{
        //	get { return _ActiveView.InfoTip.Count; }
        //	set
        //	{
        //		if (_ActiveView == null || _ActiveView.InfoTip == null)
        //			return;

        //		_ActiveView.InfoTip.Count = value;
        //		_ActiveView.InfoTip.Init();
        //	}
        //}

        /// <summary>
        /// The text in the Infotip.
        /// </summary>
        /// <remarks><br/>
        /// The text uses a HTML like syntax.<br/>
        /// <br/>
        /// Supported tags are:<br/>
        /// <br/>
        /// &lt;Font Size="Size in Pixels" Face="Font Name" Color="Named color" &gt;&lt;/Font&gt; Set Font size,color and fontname.<br/>
        /// &lt;HR&gt; : Inserts a horizontal separator line.<br/>
        /// &lt;BR&gt; : Line break.<br/>
        /// &lt;B&gt;&lt;/B&gt; : Activate/Deactivate Bold style.<br/>
        /// &lt;I&gt;&lt;/I&gt; : Activate/Deactivate Italic style.<br/>
        /// &lt;U&gt;&lt;/U&gt; : Activate/Deactivate Underline style.	<br/>			
        /// </remarks>	
        /// <example >
        /// <code>
        /// MySyntaxBox.InfoTipText="public void MyMethod ( &lt;b&gt; string text &lt;/b&gt; );"; 		
        /// </code>
        /// </example>	
        //[Category("Appearance - InfoTip"),
        //	Description("The infotip text")]
        //[DefaultValue("")]
        //public string InfoTipText
        //{
        //	get { return _ActiveView.InfoTip.Data; }
        //	set
        //	{
        //		if (_ActiveView == null || _ActiveView.InfoTip == null)
        //			return;

        //		_ActiveView.InfoTip.Data = value;
        //	}
        //}

        /// <summary>
        /// Gets the Selection object from the active view.
        /// </summary>
        [Browsable(false)]
        //   [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Selection Selection
        {
            get
            {
                if (_ActiveView != null)
                {
                    return _ActiveView.Selection;
                }
                return null;
            }
        }

        /// <summary>
        /// Collection of KeyboardActions that is used by the control.
        /// Keyboard actions to add shortcut key combinations to certain tasks.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KeyboardActionList KeyboardActions
        {
            get { return _KeyboardActions; }
            set { _KeyboardActions = value; }
        }

        /// <summary>
        /// Gets or Sets if the AutoList is visible in the active view.
        /// </summary>
        [Category("Appearance"),
            Description("Gets or Sets if the AutoList is visible in the active view.")]
        [Browsable(false)]
        public bool AutoListVisible
        {
            get
            {
                if (_ActiveView != null)
                    return _ActiveView.AutoListVisible;
                else
                    return false;
            }
            set
            {
                if (_ActiveView != null)
                    _ActiveView.AutoListVisible = value;
            }
        }

        ///// <summary>
        ///// Gets or Sets if the InfoTip is visible in the active view.
        ///// </summary>
        //[Category("Appearance"),
        //	Description("Gets or Sets if the InfoTip is visible in the active view.")]
        //[Browsable(false)]
        //public bool InfoTipVisible
        //{
        //	get
        //	{
        //		if (_ActiveView != null)
        //			return _ActiveView.InfoTipVisible;
        //		else
        //			return false;
        //	}
        //	set
        //	{
        //		if (_ActiveView != null)
        //			_ActiveView.InfoTipVisible = value;
        //	}
        //}

        /// <summary>
        /// Gets if the control can perform a Copy action.
        /// </summary>
        [Browsable(false)]
        public bool CanCopy
        {
            get { return _ActiveView.CanCopy; }
        }

        /// <summary>
        /// Gets if the control can perform a Paste action.
        /// (if the clipboard contains a valid text).
        /// </summary>
        [Browsable(false)]
        public bool CanPaste
        {
            get { return _ActiveView.CanPaste; }
        }


        /// <summary>
        /// Gets if the control can perform a ReDo action.
        /// </summary>
        [Browsable(false)]
        public bool CanRedo
        {
            get { return _ActiveView.CanRedo; }
        }

        /// <summary>
        /// Gets if the control can perform an Undo action.
        /// </summary>
        [Browsable(false)]
        public bool CanUndo
        {
            get { return _ActiveView.CanUndo; }
        }

        /// <summary>
        /// Gets or Sets the imagelist to use in the gutter margin.
        /// </summary>
        /// <remarks>
        /// Image Index 0 is used to display the Breakpoint icon.
        /// Image Index 1 is used to display the Bookmark icon.
        /// </remarks>		
        [Category("Appearance - Gutter Margin"),
            Description("Gets or Sets the imagelist to use in the gutter margin.")]
        public ImageList GutterIcons
        {
            get { return _GutterIcons; }
            set
            {
                _GutterIcons = value;
                this.Redraw();
            }
        }

        private static ImageList s_autoListImages;

        /// <summary>
        /// Gets or Sets the imagelist to use in the autolist.
        /// </summary>
        [Category("Appearance"),
            Description("Gets or Sets the imagelist to use in the autolist.")]
        [DefaultValue(null)]
        static public ImageList AutoListImages
        {
            get { return s_autoListImages; }
            set
            {
                s_autoListImages = value;

                //foreach (EditViewControl ev in Views)
                //{
                //    if (ev != null && ev.AutoList != null)
                //        ev.AutoList.Images = value;
                //}
                ////this.Redraw();
            }
        }


        /// <summary>
        /// Gets or Sets the imagelist to use in the autolist.
        /// </summary>
        [Category("Appearance"),
            Description("Gets or Sets the imagelist to use in the autolist.")]
        [DefaultValue(null)]
        public ImageList AutoListIcons
        {
            get { return _AutoListIcons; }
            set
            {
                _AutoListIcons = value;


                foreach (EditViewControl ev in Views)
                {
                    if (ev != null && ev.AutoList != null)
                        ev.AutoList.Images = value;
                }
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the border styles of the split views.
        /// </summary>
        [Category("Appearance - Borders")]
        [Description("Gets or Sets the border styles of the split views.")]
        [DefaultValue(ControlBorderStyle.FixedSingle)]
        public ControlBorderStyle ChildBorderStyle
        {
            get { return ((EditViewControl)Views[0]).BorderStyle; }
            set
            {
                foreach (EditViewControl ev in this.Views)
                {
                    ev.BorderStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the border color of the split views.
        /// </summary>
        [Category("Appearance - Borders")]
        [Description("Gets or Sets the border color of the split views.")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color ChildBorderColor
        {
            get { return ((EditViewControl)Views[0]).BorderColor; }
            set
            {
                foreach (EditViewControl ev in this.Views)
                {
                    if (ev != null)
                    {
                        ev.BorderColor = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the color to use when rendering Tab guides.
        /// </summary>
        [Category("Appearance - Tabs")]
        [Description("Gets or Sets the color to use when rendering Tab guides.")]
        [DefaultValue(typeof(Color), "Control")]
        public Color TabGuideColor
        {
            get { return _TabGuideColor; }
            set
            {
                _TabGuideColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the color of the bracket match borders.
        /// </summary>
        /// <remarks>
        /// NOTE: use Color.Transparent to turn off the bracket match borders.
        /// </remarks>
        [Category("Appearance - Bracket Match")]
        [Description("Gets or Sets the color of the bracket match borders.")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        public Color BracketBorderColor
        {
            get { return _BracketBorderColor; }
            set
            {
                _BracketBorderColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets if the control should render Tab guides.
        /// </summary>
        [Category("Appearance - Tabs")]
        [Description("Gets or Sets if the control should render Tab guides.")]
        [DefaultValue(false)]
        public bool ShowTabGuides
        {
            get { return _ShowTabGuides; }
            set
            {
                _ShowTabGuides = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the color to use when rendering whitespace characters
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or Sets the color to use when rendering whitespace characters.")]
        [DefaultValue(typeof(Color), "Control")]
        public Color WhitespaceColor
        {
            get { return _WhitespaceColor; }
            set
            {
                _WhitespaceColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the color of the code Outlining (both folding lines and collapsed blocks).
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or Sets the color of the code Outlining (both folding lines and collapsed blocks).")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color OutlineColor
        {
            get { return _OutlineColor; }
            set
            {
                _OutlineColor = value;
                InitGraphics();
                this.Redraw();
            }
        }


        /// <summary>
        /// Determines if the control should use a smooth scroll when scrolling one row up or down.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines if the control should use a smooth scroll when scrolling one row up or down.")]
        [DefaultValue(typeof(Color), "False")]
        public bool SmoothScroll
        {
            get { return _SmoothScroll; }
            set { _SmoothScroll = value; }
        }

        /// <summary>
        /// Gets or Sets the speed of the vertical scroll when SmoothScroll is activated
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or Sets the speed of the vertical scroll when SmoothScroll is activated")]
        [DefaultValue(2)]
        public int SmoothScrollSpeed
        {
            get { return _SmoothScrollSpeed; }
            set
            {
                if (value <= 0)
                {
                    throw (new Exception("Scrollsped may not be less than 1"));
                }
                else
                    _SmoothScrollSpeed = value;
            }
        }

        /// <summary>
        /// Gets or Sets if the control can display breakpoints or not.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or Sets if the control can display breakpoints or not.")]
        [DefaultValue(true)]
        public bool AllowBreakPoints
        {
            get { return _AllowBreakPoints; }
            set { _AllowBreakPoints = value; }
        }

        /// <summary>
        /// Gets or Sets if the control should perform a full parse of the document when content is drag dropped or pasted into the control
        /// </summary>
        [Category("Behavior - Clipboard")]
        [Description("Gets or Sets if the control should perform a full parse of the document when content is drag dropped or pasted into the control")]
        [DefaultValue(false)]
        public bool ParseOnPaste
        {
            get { return _ParseOnPaste; }
            set
            {
                _ParseOnPaste = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Returns true if the control is in overwrite mode.
        /// </summary>
        [Browsable(false)]
        public bool OverWrite
        {
            get { return this._ActiveView.OverWrite; }
        }


        /// <summary>
        /// Gets or Sets the Size of the font.
        /// <seealso cref="FontName"/>
        /// </summary>
        [Category("Appearance - Font")]
        [Description("The size of the font")]
        [DefaultValue(10f)]
        public float FontSize
        {
            get { return _FontSize; }
            set
            {
                _FontSize = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Determines what indentstyle to use on a new line.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines how the the control indents a new line")]
        [DefaultValue(IndentStyle.LastRow)]
        public IndentStyle Indent
        {
            get { return _Indent; }
            set { _Indent = value; }
        }

        /// <summary>
        /// Gets or Sets the SyntaxDocument the control is currently attatched to.
        /// </summary>
        [Category("Content")]
        [Description("The SyntaxDocument that is attatched to the contro")]
        public SyntaxDocument Document
        {
            get { return _Document; }
            set { AttachDocument(value); }
        }

       

        /// <summary>
        /// Get or Set the delay before the tooltip is displayed over a collapsed block
        /// </summary>
        [Category("Behavior")]
        [Description("The delay before the tooltip is displayed over a collapsed block")]
        [DefaultValue(240)]
        public int TooltipDelay
        {
            get { return _TooltipDelay; }
            set { _TooltipDelay = value; }
        }

        /// <summary>
        /// Get or Set the delay before the tooltip is displayed over a collapsed block
        /// </summary>
        [Category("Behavior")]
        [Description("Determines if the control is readonly or not")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }

        /// <summary>
        /// Gets or Sets the name of the font.
        /// <seealso cref="FontSize"/>
        /// </summary>
        [Category("Appearance - Font")]
        [Description("The name of the font that is used to render the control")]
        [Editor(typeof(FontList), typeof(UITypeEditor))]
        [DefaultValue("Courier New")]
        public string FontName
        {
            get { return _FontName; }
            set
            {
                if (this.Views == null)
                    return;

                _FontName = value;
                InitGraphics();
                foreach (EditViewControl evc in this.Views)
                    evc.CalcMaxCharWidth();

                this.Redraw();
            }
        }

        /// <summary>
        /// Determines the style to use when painting with alt+arrow keys.
        /// </summary>
        [Category("Behavior")]
        [Description("Determines what type of chars to use when painting with ALT+arrow keys")]
        [DefaultValue(TextDrawType.StarBorder)]
        public TextDrawType TextDrawStyle
        {
            get { return _TextDrawStyle; }
            set { _TextDrawStyle = value; }
        }

        /// <summary>
        /// Gets or Sets if bracketmatching is active
        /// <seealso cref="BracketForeColor"/>
        /// <seealso cref="BracketBackColor"/>
        /// </summary>
        [Category("Appearance - Bracket Match")]
        [Description("Determines if the control should highlight scope patterns")]
        [DefaultValue(true)]
        public bool BracketMatching
        {
            get { return _BracketMatching; }
            set
            {
                _BracketMatching = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets if Virtual Whitespace is active.
        /// <seealso cref="ShowWhitespace"/>
        /// </summary>
        [Category("Behavior")]
        [Description("Determines if virtual Whitespace is active")]
        [DefaultValue(false)]
        public bool VirtualWhitespace
        {
            get { return _VirtualWhitespace; }
            set
            {
                _VirtualWhitespace = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the separator Color.
        /// <seealso cref="BracketMatching"/>
        /// <seealso cref="BracketBackColor"/>
        /// </summary>
        [Category("Appearance")]
        [Description("The separator color")]
        [DefaultValue(typeof(Color), "Control")]
        public Color SeparatorColor
        {
            get { return _SeparatorColor; }
            set
            {
                _SeparatorColor = value;
                this.Redraw();
            }
        }


        /// <summary>
        /// Gets or Sets the foreground Color to use when BracketMatching is activated.
        /// <seealso cref="BracketMatching"/>
        /// <seealso cref="BracketBackColor"/>
        /// </summary>
        [Category("Appearance - Bracket Match")]
        [Description("The foreground color to use when BracketMatching is activated")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BracketForeColor
        {
            get { return _BracketForeColor; }
            set
            {
                _BracketForeColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the background Color to use when BracketMatching is activated.
        /// <seealso cref="BracketMatching"/>
        /// <seealso cref="BracketForeColor"/>
        /// </summary>
        [Category("Appearance - Bracket Match")]
        [Description("The background color to use when BracketMatching is activated")]
        [DefaultValue(typeof(Color), "LightSteelBlue")]
        public Color BracketBackColor
        {
            get { return _BracketBackColor; }
            set
            {
                _BracketBackColor = value;
                this.Redraw();
            }
        }


        /// <summary>
        /// The inactive selection background color.
        /// </summary>
        [Category("Appearance - Selection")]
        [Description("The inactive selection background color.")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color InactiveSelectionBackColor
        {
            get { return _InactiveSelectionBackColor; }
            set
            {
                _InactiveSelectionBackColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// The inactive selection foreground color.
        /// </summary>
        [Category("Appearance - Selection")]
        [Description("The inactive selection foreground color.")]
        [DefaultValue(typeof(Color), "ControlLight")]
        public Color InactiveSelectionForeColor
        {
            get { return _InactiveSelectionForeColor; }
            set
            {
                _InactiveSelectionForeColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// The selection background color.
        /// </summary>
        [Category("Appearance - Selection")]
        [Description("The selection background color.")]
        [DefaultValue(typeof(Color), "Highlight")]
        public Color SelectionBackColor
        {
            get { return _SelectionBackColor; }
            set
            {
                _SelectionBackColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// The selection foreground color.
        /// </summary>
        [Category("Appearance - Selection")]
        [Description("The selection foreground color.")]
        [DefaultValue(typeof(Color), "HighlightText")]
        public Color SelectionForeColor
        {
            get { return _SelectionForeColor; }
            set
            {
                _SelectionForeColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the border Color of the gutter margin.
        /// <seealso cref="GutterMarginColor"/>
        /// </summary>
        [Category("Appearance - Gutter Margin")]
        [Description("The border color of the gutter margin")]
        [DefaultValue(typeof(Color), "ControlDark")]
        public Color GutterMarginBorderColor
        {
            get { return _GutterMarginBorderColor; }
            set
            {
                _GutterMarginBorderColor = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the border Color of the line number margin
        /// <seealso cref="LineNumberForeColor"/>
        /// <seealso cref="LineNumberBackColor"/>
        /// </summary>
        [Category("Appearance - Line Numbers")]
        [Description("The border color of the line number margin")]
        [DefaultValue(typeof(Color), "Teal")]
        public Color LineNumberBorderColor
        {
            get { return _LineNumberBorderColor; }
            set
            {
                _LineNumberBorderColor = value;
                InitGraphics();
                this.Redraw();
            }
        }


        /// <summary>
        /// Gets or Sets the foreground Color of a Breakpoint.
        /// <seealso cref="BreakPointBackColor"/>
        /// </summary>
        [Category("Appearance - BreakPoints")]
        [Description("The foreground color of a Breakpoint")]
        [DefaultValue(typeof(Color), "White")]
        public Color BreakPointForeColor
        {
            get { return _BreakPointForeColor; }
            set
            {
                _BreakPointForeColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the background Color to use for breakpoint rows.
        /// <seealso cref="BreakPointForeColor"/>
        /// </summary>
        [Category("Appearance - BreakPoints")]
        [Description("The background color to use when BracketMatching is activated")]
        [DefaultValue(typeof(Color), "DarkRed")]
        public Color BreakPointBackColor
        {
            get { return _BreakPointBackColor; }
            set
            {
                _BreakPointBackColor = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the foreground Color of line numbers.
        /// <seealso cref="LineNumberBorderColor"/>
        /// <seealso cref="LineNumberBackColor"/>
        /// </summary>
        [Category("Appearance - Line Numbers")]
        [Description("The foreground color of line numbers")]
        [DefaultValue(typeof(Color), "Teal")]
        public Color LineNumberForeColor
        {
            get { return _LineNumberForeColor; }
            set
            {
                _LineNumberForeColor = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the background Color of line numbers.
        /// <seealso cref="LineNumberForeColor"/>
        /// <seealso cref="LineNumberBorderColor"/>
        /// </summary>
        [Category("Appearance - Line Numbers")]
        [Description("The background color of line numbers")]
        [DefaultValue(typeof(Color), "Window")]
        public Color LineNumberBackColor
        {
            get { return _LineNumberBackColor; }
            set
            {
                _LineNumberBackColor = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the Color of the gutter margin
        /// <seealso cref="GutterMarginBorderColor"/>
        /// </summary>
        [Category("Appearance - Gutter Margin")]
        [Description("The color of the gutter margin")]
        [DefaultValue(typeof(Color), "Control")]
        public Color GutterMarginColor
        {
            get { return _GutterMarginColor; }
            set
            {
                _GutterMarginColor = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the background Color of the client area.
        /// </summary>
        [Category("Appearance")]
        [Description("The background color of the client area")]
        [DefaultValue(typeof(Color), "Window")]
        new public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                _BackColor = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the background Color of the active line.
        /// <seealso cref="HighLightActiveLine"/>
        /// </summary>
        [Category("Appearance - Active Line")]
        [Description("The background color of the active line")]
        [DefaultValue(typeof(Color), "White")]
        public Color HighLightedLineColor
        {
            get { return _HighLightedLineColor; }
            set
            {
                _HighLightedLineColor = value;
                InitGraphics();
                this.Redraw();
            }
        }

        /// <summary>
        /// Determines if the active line should be highlighted.
        /// </summary>
        [Category("Appearance - Active Line")]
        [Description("Determines if the active line should be highlighted")]
        [DefaultValue(false)]
        public bool HighLightActiveLine
        {
            get { return _HighLightActiveLine; }
            set
            {
                _HighLightActiveLine = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Determines if Whitespace should be rendered as symbols.
        /// </summary>
        [Category("Appearance")]
        [Description("Determines if Whitespace should be rendered as symbols")]
        [DefaultValue(false)]
        public bool ShowWhitespace
        {
            get { return _ShowWhitespace; }
            set
            {
                _ShowWhitespace = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Determines if the line number margin should be visible.
        /// </summary>
        [Category("Appearance - Line Numbers")]
        [Description("Determines if the line number margin should be visible")]
        [DefaultValue(true)]
        public bool ShowLineNumbers
        {
            get { return _ShowLineNumbers; }
            set
            {
                _ShowLineNumbers = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Determines if the gutter margin should be visible.
        /// </summary>
        [Category("Appearance - Gutter Margin")]
        [Description("Determines if the gutter margin should be visible")]
        [DefaultValue(true)]
        public bool ShowGutterMargin
        {
            get { return _ShowGutterMargin; }
            set
            {
                _ShowGutterMargin = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Gets or Sets the witdth of the gutter margin in pixels.
        /// </summary>
        [Category("Appearance - Gutter Margin")]
        [Description("Determines the witdth of the gutter margin in pixels")]
        [DefaultValue(19)]
        public int GutterMarginWidth
        {
            get { return _GutterMarginWidth; }
            set
            {
                _GutterMarginWidth = value;
                this.Redraw();
            }
        }

        /// <summary>
        /// Get or Sets the size of a TAB char in number of SPACES.
        /// </summary>
        [Category("Appearance - Tabs")]
        [Description("Determines the size of a TAB in number of SPACE chars")]
        [DefaultValue(4)]
        public int TabSize
        {
            get { return _TabSize; }
            set
            {
                _TabSize = value;
                this.Redraw();
            }
        }

        #region public property ScrollBars

        private ScrollBars _ScrollBars;
        private IContainer components;

        [Category("Appearance"),
            Description("Determines what Scrollbars should be visible")]
        [DefaultValue(ScrollBars.Both)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollBars ScrollBars
        {
            get { return _ScrollBars; }

            set
            {
                if (_Views == null)
                    return;

                if (DisableScrollBars)
                    value = ScrollBars.None;

                foreach (EditViewControl evc in _Views)
                {
                    evc.ScrollBars = value;
                }
                _ScrollBars = value;
            }
        }

        #endregion

        #region public property SplitView

        //member variable
        private bool _SplitView;

        [Category("Appearance"),
            Description("Determines if the controls should use splitviews")]
        [DefaultValue(true)]
        public bool SplitView
        {
            get { return _SplitView; }

            set
            {
                _SplitView = value;

                if (_splitView1 == null)
                    return;

                if (!SplitView)
                {
                    _splitView1.Visible = false;
                    this.Controls.Add(_lowerRight);
                    _lowerRight.HideThumbs();
                    _lowerRight.Dock = DockStyle.Fill;
                }
                else
                {
                    _splitView1.Visible = true;
                    _splitView1.LowerRight = _lowerRight;
                    _lowerRight.Dock = DockStyle.None;
                    _lowerRight.ShowThumbs();
                }
            }
        }

        #endregion //END PROPERTY SplitView

        #endregion // PUBLIC PROPERTIES

        #region Public Methods

        /// <summary>
        /// Resets the Splitview.
        /// </summary>
        public void ResetSplitview()
        {
            _splitView1.ResetSplitview();
        }

        public void ScrollIntoView(int RowIndex)
        {
            _ActiveView.ScrollIntoView(RowIndex);
        }

        /// <summary>
        /// Disables painting while loading data into the Autolist
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <example>
        /// <code>
        /// MySyntaxBox.AutoListClear();
        /// MySyntaxBox.AutoListBeginLoad();
        /// MySyntaxBox.AutoListAdd ("test",1);
        /// MySyntaxBox.AutoListAdd ("test",2);
        /// MySyntaxBox.AutoListAdd ("test",3);
        /// MySyntaxBox.AutoListAdd ("test",4);
        /// MySyntaxBox.AutoListEndLoad();
        /// </code>
        /// </example>
        public void AutoListBeginLoad()
        {
            this._ActiveView.AutoListBeginLoad();
        }

        /// <summary>
        /// Resumes painting and autosizes the Autolist.			
        /// </summary>		
        public void AutoListEndLoad()
        {
            this._ActiveView.AutoListEndLoad();
        }

        /// <summary>
        /// Clears the content in the autolist.
        /// </summary>
        public void AutoListClear()
        {
            this._ActiveView.AutoList.Clear();
        }

        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <example>
        /// <code>
        /// MySyntaxBox.AutoListClear();
        /// MySyntaxBox.AutoListBeginLoad();
        /// MySyntaxBox.AutoListAdd ("test",1);
        /// MySyntaxBox.AutoListAdd ("test",2);
        /// MySyntaxBox.AutoListAdd ("test",3);
        /// MySyntaxBox.AutoListAdd ("test",4);
        /// MySyntaxBox.AutoListEndLoad();
        /// </code>
        /// </example>
        /// <param name="Text">The text to display in the autolist</param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string Text, AutoListIcons ImageIndex)
        {
            this._ActiveView.AutoList.Add(Text, ImageIndex);
        }

        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <param name="Text">The text to display in the autolist</param>
        /// <param name="InsertText">The text to insert in the code</param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string Text, string InsertText, AutoListIcons ImageIndex)
        {
            this._ActiveView.AutoList.Add(Text, InsertText, ImageIndex);
        }

        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <param name="Text">The text to display in the autolist</param>
        /// <param name="InsertText">The text to insert in the code</param>
        /// <param name="ToolTip"></param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string Text, string InsertText, string ToolTip, AutoListIcons ImageIndex)
        {
            this._ActiveView.AutoList.Add(Text, InsertText, ToolTip, ImageIndex);
        }

        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <param name="Text">The text to display in the autolist</param>
        /// <param name="InsertText">The text to insert in the code</param>
        /// <param name="ToolTip">
        /// <param name="ExtendedData">
        /// </param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string Text, string InsertText, string ToolTip, string ExtendedData, AutoListIcons ImageIndex)
        {
            this._ActiveView.AutoList.Add(Text, InsertText, ToolTip, ExtendedData, ImageIndex);
        }

        /// <summary>
        /// Converts a Client pixel coordinate into a TextPoint (Column/Row)
        /// </summary>
        /// <param name="x">Pixel x position</param>
        /// <param name="y">Pixel y position</param>
        /// <returns>The row and column at the given pixel coordinate.</returns>
        public TextPoint CharFromPixel(int x, int y)
        {
            return _ActiveView.CharFromPixel(x, y);
        }

        /// <summary>
        /// Clears the selection in the active view.
        /// </summary>
        public void ClearSelection()
        {
            _ActiveView.ClearSelection();
            if (ins != null)
                if (ins.Visible == true)
                    ins.Hide();
        }

        /// <summary>
        /// Executes a Copy action on the selection in the active view.
        /// </summary>
        public void Copy()
        {
            _ActiveView.Copy();
            this.Saved = false;
        }

        /// <summary>
        /// Executes a Cut action on the selection in the active view.
        /// </summary>
        public void Cut()
        {
            _ActiveView.Cut();
            this.Saved = false;
        }

        /// <summary>
        /// Executes a Delete action on the selection in the active view.
        /// </summary>
        public void Delete()
        {
            _ActiveView.Delete();
        }

        /// <summary>
        /// Moves the caret of the active view to a specific row.
        /// </summary>
        /// <param name="RowIndex">the row to jump to</param>
        public void GotoLine(int RowIndex)
        {
            _ActiveView.GotoLine(RowIndex);
        }

        /// <summary>
        /// Moves the caret of the active view to the next bookmark.
        /// </summary>
        public void GotoNextBookmark()
        {
            _ActiveView.GotoNextBookmark();
        }

        /// <summary>
        /// Moves the caret of the active view to the previous bookmark.
        /// </summary>
        public void GotoPreviousBookmark()
        {
            _ActiveView.GotoPreviousBookmark();
        }


        /// <summary>
        /// Takes a pixel position and returns true if that position is inside the selected text.
        /// 
        /// </summary>
        /// <param name="x">Pixel x position.</param>
        /// <param name="y">Pixel y position</param>
        /// <returns>true if the position is inside the selection.</returns>
        public bool IsOverSelection(int x, int y)
        {
            return _ActiveView.IsOverSelection(x, y);
        }

        /// <summary>
        /// Execute a Paste action if possible.
        /// </summary>
        public void Paste()
        {
            _ActiveView.Paste();
            this.Saved = false;
        }

        /// <summary>
        /// Execute a ReDo action if possible.
        /// </summary>
        public void Redo()
        {
            _ActiveView.Redo();
            this.Saved = false;
        }

        /// <summary>
        /// Makes the caret in the active view visible on screen.
        /// </summary>
        public void ScrollIntoView()
        {
            _ActiveView.ScrollIntoView();
        }

        /// <summary>
        /// Scrolls the active view to a specific position.
        /// </summary>
        /// <param name="Pos"></param>
        public void ScrollIntoView(TextPoint Pos)
        {
            _ActiveView.ScrollIntoView(Pos);
        }

        /// <summary>
        /// Select all the text in the active view.
        /// </summary>
        public void SelectAll()
        {
            _ActiveView.SelectAll();
        }

        /// <summary>
        /// Selects the next word (from the current caret position) that matches the parameter criterias.
        /// </summary>
        /// <param name="Pattern">The pattern to find</param>
        /// <param name="MatchCase">Match case , true/false</param>
        /// <param name="WholeWords">Match whole words only , true/false</param>
        /// <param name="UseRegEx">To be implemented</param>
        public void FindNext(string Pattern, bool MatchCase, bool WholeWords, bool UseRegEx)
        {
            _ActiveView.SelectNext(Pattern, MatchCase, WholeWords, UseRegEx);
        }

        /// <summary>
        /// Finds the next occurance of the pattern in the find/replace dialog
        /// </summary>
        public void FindNext()
        {
            _ActiveView.FindNext();
        }

        /// <summary>
        /// Shows the default GotoLine dialog.
        /// </summary>
        /// <example>
        /// <code>
        /// //Display the Goto Line dialog
        /// MySyntaxBox.ShowGotoLine();
        /// </code>
        /// </example>
        public void ShowGotoLine()
        {
            _ActiveView.ShowGotoLine();
        }


        public void RedrawControl()
        {
            this.Redraw();
        }

        /// <summary>
        /// Not yet implemented
        /// </summary>
        public void ShowSettings()
        {
            _ActiveView.ShowSettings();
        }

        /// <summary>
        /// Toggles a bookmark on the active row of the active view.
        /// </summary>
        public void ToggleBookmark()
        {
            _ActiveView.ToggleBookmark();
        }

        /// <summary>
        /// Executes an undo action if possible.
        /// </summary>
        public void Undo()
        {
            _ActiveView.Undo();

            this.Saved = false;
        }


        /// <summary>
        /// Shows the Find dialog
        /// </summary>
        /// <example>
        /// <code>
        /// //Show FindReplace dialog
        /// MySyntaxBox.ShowFind();
        /// </code>
        /// </example>
        public void ShowFind()
        {
            _ActiveView.ShowFind();
        }

        /// <summary>
        /// Shows the Replace dialog
        /// </summary>
        /// <example>
        /// <code>
        /// //Show FindReplace dialog
        /// MySyntaxBox.ShowReplace();
        /// </code>
        /// </example>
        public void ShowReplace()
        {
            _ActiveView.ShowReplace();
        }


        /// <summary>
        /// Gets the Caret object from the active view.
        /// </summary>
        [Browsable(false)]
        public Caret Caret
        {
            get
            {
                if (_ActiveView != null)
                {
                    return _ActiveView.Caret;
                }
                return null;
            }
        }

        #endregion //END Public Methods

        public static DataGridView dg { get; set; }

      

        private bool _Saved = false;

        public bool Saved
        {
            get
            {
                return _Saved;
            }
            set
            {
                _Saved = value;

                this.OnFileSavedChanged(EventArgs.Empty);
            }
        }

        public LineMarginRender LineMarginRender
        {
            get
            {
                return _ActiveView.LineMarginRender;
            }
            set
            {
                _ActiveView.LineMarginRender = value;
            }
        }


        private string _FileName;

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set { _FileName = value; }
        }

        private void SetFileName(string filename)
        {
            _FileName = filename;
            this.OnFileNameChanged(new EventArgs());
        }


        protected virtual void OnFileNameChanged(EventArgs e)
        {
            if (FileNameChanged != null)
                FileNameChanged(null, e);
        }

        protected virtual void OnFileSavedChanged(EventArgs e)
        {
            if (FileSavedChanged != null)
                FileSavedChanged(this, e);
        }


        public void Save(string filename)
        {
            string text = this.Document.Text;

            StreamWriter swr = new StreamWriter(filename, false,
                System.Text.Encoding.Default);

            swr.Write(text);

            swr.Flush();

            swr.Close();

            SetFileName(filename);

            this.Saved = true;
        }


        public void Save()
        {
            if (this.FileName == null)
                throw new IOException("Invalid Filename.");

            this.Save(this.FileName);
        }

        public void Open(string filename)
        {
            if (this.Document == null)
                throw new NullReferenceException("CodeEditorControl.Document");

            StreamReader swr = new StreamReader(filename);

            this.Document.Text = swr.ReadToEnd();

            swr.Close();

            SetFileName(filename);



            this.Saved = true;
        }

        public void ReLoadFile()
        {
            if (this.FileName == null)
                throw new IOException("Invalid Filename.");

            this.Open(this.FileName);
     
        }


        public EditViewControl ActiveViewControl
        {
            get
            {
                return _ActiveView;
            }
        }

        public VSProvider.VSProject vp { get; set; }

        public static Proxy proxy = new Proxy();

        public static Intellisense IntErrors { get; set; }

        #region Constructor

        /// <summary>
        /// Default constructor for the SyntaxBoxControl
        /// </summary>
        public CodeEditorControl() : base()
        {
            this.Document = new SyntaxDocument();
            //this.OnCreate();



            if (!DisableSplitView)
            {
                _splitView1 = new SplitViewWidget();

                _lowerRight = new EditViewControl(this);
            }
            else
            {
                _lowerRight = new EditViewControl(this);
                this.Controls.Add(_lowerRight);
                _lowerRight.HideThumbs();
                _lowerRight.Dock = DockStyle.Fill;
            }

            //			this.UpperLeft.Visible = false;
            //			this.UpperRight.Visible = false;
            //			this.LowerLeft.Visible = false;
            //			this.LowerRight.Visible = false;
            //			this.splitView1.Visible = false;


            this.SuspendLayout();
            // 
            // splitView1
            // 

            if (!this.DisableSplitView)
            {
                _splitView1.SuspendLayout();
                _splitView1.BackColor = SystemColors.Control;
                _splitView1.Controls.AddRange(new Control[]
                    {
                        _lowerRight
                    });
                _splitView1.Dock = DockStyle.Fill;
                _splitView1.Location = new Point(0, 0);
                _splitView1.LowerRight = _lowerRight;
                _splitView1.Name = "splitView1";
                _splitView1.Size = new Size(500, 364);
                _splitView1.TabIndex = 0;
            }
            // 
            // LowerRight
            // 
            //this.LowerRight.AllowDrop = true;
            _lowerRight.BorderColor = Color.White;
            _lowerRight.BorderStyle = ControlBorderStyle.None;
            _lowerRight.Location = new Point(148, 124);
            _lowerRight.Name = "LowerRight";
            _lowerRight.Size = new Size(352, 240);
            _lowerRight.TabIndex = 3;
            _lowerRight.TextDrawStyle = TextDrawType.StarBorder;

            // 
            // SyntaxBoxControl
            // 

            if (!this.DisableSplitView)
                this.Controls.AddRange(new Control[]
                    {
                        _splitView1
                    });

            this.Name = "SyntaxBoxControl";
            this.Size = new Size(504, 368);
            if (!this.DisableSplitView)
            {
                _splitView1.ResumeLayout(false);
                _splitView1.Resizing += new EventHandler(this.SplitView_Resizing);
                _splitView1.HideLeft += new EventHandler(this.SplitView_HideLeft);
                _splitView1.HideTop += new EventHandler(this.SplitView_HideTop);
            }

            this.ResumeLayout(false);


            this.Views = new ArrayList();
            CreateViews();
            _ActiveView = _lowerRight;


            InitializeComponent();
            this.SetStyle(ControlStyles.Selectable, true);

            //assign keys
            KeyboardActions.Add(new KeyboardAction(Keys.Z, false, true, false, false, new ActionDelegate(this.Undo)));
            KeyboardActions.Add(new KeyboardAction(Keys.Y, false, true, false, false, new ActionDelegate(this.Redo)));

            KeyboardActions.Add(new KeyboardAction(Keys.F3, false, false, false, true, new ActionDelegate(this.FindNext)));

            KeyboardActions.Add(new KeyboardAction(Keys.C, false, true, false, true, new ActionDelegate(this.Copy)));
            KeyboardActions.Add(new KeyboardAction(Keys.X, false, true, false, false, new ActionDelegate(this.CutClear)));
            KeyboardActions.Add(new KeyboardAction(Keys.V, false, true, false, false, new ActionDelegate(this.Paste)));

            KeyboardActions.Add(new KeyboardAction(Keys.Insert, false, true, false, true, new ActionDelegate(this.Copy)));
            KeyboardActions.Add(new KeyboardAction(Keys.Delete, true, false, false, false, new ActionDelegate(this.Cut)));
            KeyboardActions.Add(new KeyboardAction(Keys.Insert, true, false, false, false, new ActionDelegate(this.Paste)));

            KeyboardActions.Add(new KeyboardAction(Keys.A, false, true, false, true, new ActionDelegate(this.SelectAll)));

            KeyboardActions.Add(new KeyboardAction(Keys.F, false, true, false, false, new ActionDelegate(this.ShowFind)));
            KeyboardActions.Add(new KeyboardAction(Keys.H, false, true, false, false, new ActionDelegate(this.ShowReplace)));
            KeyboardActions.Add(new KeyboardAction(Keys.G, false, true, false, true, new ActionDelegate(this.ShowGotoLine)));
            KeyboardActions.Add(new KeyboardAction(Keys.T, false, true, false, false, new ActionDelegate(this.ShowSettings)));

            KeyboardActions.Add(new KeyboardAction(Keys.F2, false, true, false, true, new ActionDelegate(this.ToggleBookmark)));
            KeyboardActions.Add(new KeyboardAction(Keys.F2, false, false, false, true, new ActionDelegate(this.GotoNextBookmark)));
            KeyboardActions.Add(new KeyboardAction(Keys.F2, true, false, false, true, new ActionDelegate(this.GotoPreviousBookmark)));

            KeyboardActions.Add(new KeyboardAction(Keys.Escape, false, false, false, true, new ActionDelegate(this.ClearSelection)));

            KeyboardActions.Add(new KeyboardAction(Keys.Tab, false, false, false, false, new ActionDelegate(Selection.Indent)));
            KeyboardActions.Add(new KeyboardAction(Keys.Tab, true, false, false, false, new ActionDelegate(Selection.Outdent)));
            KeyboardActions.Add(new KeyboardAction(Keys.F11, false, false, false, false, new ActionDelegate(FindWords)));
            KeyboardActions.Add(new KeyboardAction(Keys.F12, false, false, false, false, new ActionDelegate(GetCurrentMember)));
            KeyboardActions.Add(new KeyboardAction(Keys.F10, false, false, false, false, new ActionDelegate(LoadKeywords)));
            KeyboardActions.Add(new KeyboardAction(Keys.F2, false, false, false, false, new ActionDelegate(FormatCode)));
            KeyboardActions.Add(new KeyboardAction(Keys.F9, false, false, false, false, new ActionDelegate(BuildMaps)));
            KeyboardActions.Add(new KeyboardAction(Keys.K, false, true, false, false, new ActionDelegate(LoadComment)));
            KeyboardActions.Add(new KeyboardAction(Keys.C, false, true, false, false, new ActionDelegate(LoadComments)));
            KeyboardActions.Add(new KeyboardAction(Keys.U, false, true, false, false, new ActionDelegate(LoadUnComments)));

            AutoListIcons = CodeEditorControl.AutoListImages;
            this.SplitView = true;
            this.ScrollBars = ScrollBars.Both;
            this.BorderStyle = ControlBorderStyle.None;
            this.ChildBorderColor = SystemColors.ControlDark;
            this.ChildBorderStyle = ControlBorderStyle.FixedSingle;
            //this.BackColor = SystemColors.Window;
            this.BackColor = Color.White;

            //			this.UpperLeft.Visible = true;
            //			this.UpperRight.Visible = true;
            //			this.LowerLeft.Visible = true;
            //			this.LowerRight.Visible = true;
            //			this.splitView1.Visible = true;

            _ShowLineNumbers = settings.ShowLineNumbers;

            //WordMouseHover += CodeEditorControl_WordMouseHover;
        }

        private void CodeEditorControl_WordMouseHover(object sender, ref WordMouseEventArgs e)
        {
            MessageBox.Show("Mouse hover");
        }


        #endregion //END Constructor

        public event EventHandler Comments = null;

        public event EventHandler UnComments = null;

        private bool _ctrlK = false;

        public void LoadComment()
        {
            _ctrlK = true;
        }
        public void LoadComments()
        {
            if (_ctrlK == false)
                return;

            Comments(this, new EventArgs());

            _ctrlK = false;
        }

        public void LoadUnComments()
        {
            if (_ctrlK == false)
                return;

            UnComments(this, new EventArgs());

            _ctrlK = false;
        }

        private TypeForm tf { get; set; }

        public void FindWords()
        {
            ActiveViewControl.ShadowedWords();




            return;


            if (tf == null)
            {
                tf = new TypeForm();


                TreeView tw = tf.tw;

                if (vp == null)
                    return;

                TreeNode node = new TreeNode();
                node.Text = vp.Name;

                tw.Nodes.Add(node);

                TypeBuilder b = new TypeBuilder();

                b.dict = new System.Collections.Generic.Dictionary<string, string>();
                b.dlls = new System.Collections.Generic.Dictionary<string, string>();
                b.maps = new System.Collections.Generic.Dictionary<string, ClassMapper>();

                TreeNode nodes = b.AddProjectReferences(vp, node);

                foreach (TreeNode ns in nodes.Nodes)
                {
                    b.GetProjectReference(vp, ns.Text, ns);
                }

                b.AddBuiltInTypes();

                //if (GACForm.dicts.ContainsKey("System"))
                //{
                //    string s = GACForm.dicts["System"];

                //    TreeNode ns = new TreeNode();
                //    ns.Text = "System";

                //    nodes.Nodes.Add(ns);

                //    b.GetProjectReference(s, ns);



                //}

                //vp.dicts = b.dict;

                tf.b = b;
            }
            else if (tf.Visible == false) tf.Show();
            else tf.Visible = false;
        }


        private static readonly string[] CopyrightHeader =
        {
            "// Licensed to the .NET Foundation under one or more agreements.",
            "// The .NET Foundation licenses this file to you under the MIT license.",
            "// See the LICENSE file in the project root for more information."
        };

        public async void FormatCode()
        {
            string[] args = new string[1];

            args[0] = vp.FileName;

            //args[1] = "/file:" + FileName;

            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            //var result = CommandLineParser.Parse(args);

            //var options = result.Options;

            FormattingEngineImplementation engine = FormattingEngine.Create() as FormattingEngineImplementation;
           // engine.PreprocessorConfigurations = options.PreprocessorConfigurations;
            //engine.FileNames = options.FileNames;
            engine.CopyrightHeader = ImmutableArray.CreateRange(CopyrightHeader);
            engine.AllowTables = true;
            engine.Verbose = true;
            
           
            //{
                


                //using (var workspace = CSharpWorkspace.Create())
                //{

                //    string folder = AppDomain.CurrentDomain.BaseDirectory;

                //    workspace.LoadMetadataForReferencedProjects = true;
                    

                //    var s = await workspace.OpenProjectAsync(folder + "TemplateSolution\\Empty\\Empty\\Empty.csproj");

                //    Microsoft.CodeAnalysis.Document c = s.AddDocument("documents2.cs", Document.Text, null,"document2.cs");
                   

                //    Microsoft.CodeAnalysis.Document d = null;
                    

                //    Microsoft.CodeAnalysis.Solution g = await engine.FormatCoreAsync(c.Project.Solution, c.Project.DocumentIds, ct);

                //    Microsoft.CodeAnalysis.Project p = g.Projects.First();

                //    foreach (Microsoft.CodeAnalysis.Document dd in p.Documents)
                //    {
                //        if (dd.Name == "documents2.cs")
                //            d = dd;
                //    }


                //    //SourceText text;

                //    //st = d.GetSyntaxTreeAsync(new CancellationToken()).Result as Microsoft.CodeAnalysis.SyntaxTree;

                //    //text = st.GetTextAsync(new CancellationToken()).Result as SourceText;


                //    Microsoft.CodeAnalysis.SyntaxNode n = await d.GetSyntaxRootAsync(new CancellationToken()) as Microsoft.CodeAnalysis.SyntaxNode;

                //    //n.ToString();

                //    Document.Text = n.ToString();

                    

                //    //foreach (Microsoft.CodeAnalysis.Project pp in s.Projects)
                //    //    if (pp.FilePath == vp.FileName)
                //    //        p = pp;
                //    //if (p == null)
                //    //    return;

                //    //foreach (Microsoft.CodeAnalysis.Document dd in p.Documents)
                //    //    {

                //    //    Microsoft.CodeAnalysis.SyntaxTree b = await dd.GetSyntaxTreeAsync(new CancellationToken());

                //    //    string sc = b.ToString();

                //    //    File.WriteAllText(dd.FilePath, sc);

                //    //}


                //}

            //}

            

    }

        //  this.BeginInvoke(new Action(() => { CodeFormatter.Program.Format(args); }));
        //CodeFormatter.Program.Format(args);
        //}

        public ArrayList AddBaseTypes(ArrayList BC)
        {
            ArrayList L = new ArrayList();

            if (BC == null)
                return L;

            foreach (ClassMapper maps in BC)
            {
                if (maps == null)
                    continue;

                if (maps.baseclasses == null)
                    continue;

                int pi = 0;

                while (pi < maps.baseclasses.Count)
                {
                    string bc = maps.baseclasses[pi] as string;

                    ClassMapper m = tf.GetBaseTypes(bc);

                    if (m != null)
                    {
                        L.Add(m);

                        //BC.Add(m);
                    }
                    pi++;
                }
            }

            if (L.Count > 0)
            {
                BC.AddRange(L);

                ArrayList P = AddBaseTypes(L);

                BC.AddRange(P);
            }

            return L;
        }

        public void LoadKeywords()
        {
            if (tf == null)
                return;

            Dictionary<string, string> dict = tf.b.dict;

            PatternList pp = null;

            BlockType blc = null;


            foreach (BlockType bl in Document.Parser.Language.Blocks)
            {
                if (bl.Name == "CS Code")
                {
                    blc = bl;

                    foreach (PatternList pl in bl.KeywordsList)
                    {
                        if (pl.Name == "Keywords")

                        {
                            pp = new PatternList();

                            pp.Style.ForeColor = Color.LightBlue;

                            //bl.KeywordsList.Add(pp);

                            foreach (string key in dict.Keys)
                            {
                                Pattern p = new Pattern(key, false, false, true);

                                pp.Add(p);
                            }
                        }
                    }
                }
            }

            if (blc != null)
                blc.KeywordsList.Add(pp);

            string s = Document.Text;

            Document.Text = s;

            AIMS.Libraries.CodeEditor.WinForms.CSDemo.Parse(this.Document.Text);

            //int i = 0;
            //while (i < this.Document.Count)
            //{
            //    Row r = this.Document[i];

            //    foreach (Word w in r.FormattedWords)
            //    {
            //        TextLocation b = new TextLocation(i + 1, w.Column + 1);

            //        string text = AIMS.Libraries.CodeEditor.WinForms.CSDemo.Resolve(b,"","");

            //        if (text != "")
            //            w.InfoTip = text;
            //    }

            //    i++;
            //}
        }


        public void LoadKeywordTypes()
        {
            if (vp == null)
                return;

           if(csd == null)// if (vp.dicts == null)
            {
                csd = vp.CSParsers();
                vp.dicts = vp.csd.dict;
                
            }
        
            Document.csd = csd;

            Dictionary<string, ITypeDefinition> dict = vp.dicts;

            PatternList pp = null;

            BlockType blc = null;
         
            foreach (BlockType bl in Document.Parser.Language.Blocks)
            {
                if (bl.Name == "CS Code")
                {
                    blc = bl;

                    Color c = Color.FromArgb(222, 225, 231);

                    foreach (PatternList pl in bl.KeywordsList)
                    {
                        if (pl.Name == "Keywords")

                        {
                            pp = new PatternList();

                            pp.Style.ForeColor = c;

                            pp.Name = "Keywords Types";

                            if(dict != null)

                            foreach (string key in dict.Keys)
                            {
                                string[] b = key.Split(".".ToCharArray());

                                string keys = b[b.Length - 1];


                                Pattern p = new Pattern(keys, false, false, true);

                                pp.Add(p);
                            }
                        }
                    }
                }
            }

         
        }


        public Dictionary<string, ClassMapper> dict { get; set; }

        static public event EventHandler ParserDataChanged;

        public void ParserDataChangedEvent()
        {
            ParserDataChanged(this, new EventArgs());
        }

        public void BuildMaps()
        {
            return;

            dict = new Dictionary<string, ClassMapper>();

            VSSolution vs = vp.solution;

            foreach (VSProject pp in vs.Projects)
            {
                ArrayList L = pp.GetCompileItems();

                foreach (string s in L)
                {
                    if (File.Exists(s) == false)
                        continue;

                    string contents = File.ReadAllText(s);

                    ClassMapper c = MainTest.analysecode(contents, "file.cs");

                    MainTest.AnalyzeSyntaxTree(c.syntax, c);

                    if (c.mappers == null)
                        continue;

                    c.project = pp.FileName;

                    foreach (ClassMapper p in c.mappers)
                    {
                        p.project = pp.FileName;
                        if (dict.ContainsKey(p.classname) == false)
                            dict.Add(p.classname, p);
                    }
                }
            }

            ////if (tf == null)
            //{

            //    tf = new TypeForm();
            //    tf.Show();

            //    tf.maps = dict;

            //    tf.LoadTypeMapping();

            //}
        }

        public void CreateProjects(TreeNode tvs)
        {
            Dictionary<string, ClassMapper> maps = dict;

            //tvs.ImageList = AndersLiu.Reflector.Program.UI.AssemblyTreeNode.NodeImages.CreateImageList();


            foreach (string key in maps.Keys)
            {
                //ListViewItem v = new ListViewItem();
                //v.Text = key;

                ClassMapper m = maps[key] as ClassMapper;

                string project = Path.GetFileNameWithoutExtension(m.project);

                TreeNode[] ns = tvs.Nodes.Find(project, false);

                TreeNode node;

                if (ns == null || ns.Length <= 0)
                {
                    node = new TreeNode();
                    node.Text = project;
                    node.Name = project;

                    tvs.Nodes.Add(node);
                }
                else node = ns[0];

                string name = m.Namespace;

                TreeNode nodes;

                TreeNode[] ng = node.Nodes.Find(name, true);

                if (ng == null || ng.Length <= 0)
                {
                    nodes = new TreeNode();
                    nodes.Text = name;
                    nodes.Name = name;
                    node.Nodes.Add(nodes);
                    node.ImageKey = "Class_489_24";// NodeImages.Keys.AssemblyImage;

                    TreeNode b = new TreeNode();
                    b.Text = m.classname;
                    b.Name = m.classname;
                    b.ImageKey = "Class_489_24";
                    b.Tag = m;


                    nodes.Nodes.Add(b);
                }
                else
                {
                    nodes = ng[0];

                    TreeNode b = new TreeNode();
                    b.Text = m.classname;
                    b.Name = m.classname;
                    b.ImageKey = "Class_489_24";
                    b.Tag = m;

                    nodes.Nodes.Add(b);
                }
            }
        }

        //public void BuildMapsNode(VSProject pp, TreeNode node)
        //{
        //    //VSSolution vs = vp.solution;

        //    //foreach (VSProject pp in vs.Projects)


        //    ArrayList L = pp.GetCompileItems();

        //    foreach (string s in L)
        //    {
        //        if (File.Exists(s) == false)
        //            continue;

        //        string contents = File.ReadAllText(s);

        //        ClassMapper c = MainTest.analysecode(contents, "file.cs");

        //        MainTest.AnalyzeSyntaxTree(c.syntax, c);

        //        if (c.mappers == null)
        //            continue;

        //        c.project = pp.FileName;

        //        foreach (ClassMapper p in c.mappers)
        //        {
        //            p.project = pp.FileName;
        //            if (dict.ContainsKey(p.classname) == false)
        //                dict.Add(p.classname, p);
        //        }
        //    }

        //    CreateProjects(node);
        //}

        public string prev { get; set; }

        public string next { get; set; }


        public int GetOffset(int X, int Y)
        {
            int offset = 0;
            int i = 0;
            while (i < Y)
            {
                Row r = Document[i];
                offset += r.Text.Length + 1;
                i++;
            }
            offset += X;
            return offset;
        }

        static public PanelIns ins { get; set; }

        public void Scroll()
        {
            if (ins == null)
                return;

            if (ins.Visible == false)
                return;

            if (this.Height == 0)
            {
                form.Hide();
                ins.Hide();
                return;
            }

            if (Selection == null)
                return;

            Point pp = Selection.GetCursor();

            if (ActiveViewControl.View.FirstVisibleRow > pp.Y || ActiveViewControl.View.FirstVisibleRow + ActiveViewControl.View.VisibleRowCount < pp.Y)
            {
                form.Hide();
                ins.Hide();
                return;
            }

            pp.X = pp.X * ActiveViewControl.View.CharWidth;

            pp.Y = (pp.Y - ActiveViewControl.View.FirstVisibleRow + 1) * ActiveViewControl.View.RowHeight;

            form.Location = this.PointToScreen(pp);
            // ins.Location = pp;

        }

        public bool SendKeys(int c)
        {
            if (ins == null)
                return false;
            if (ins.Visible == false)
                return false;
            ins.SendKeys(c);
            return true;
        }

        public bool _afterdot = false;

        public bool EnterHandled(KeyEventArgs e)
        {
            if (ins == null)
                return false;
            if (ins.Visible == false)
                return false;
            ins.Cec_KeyDown(this, e);

            return true;
        }
        public static DefinitionForm dfs { get; set; }
        public bool ArrowHandled(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                TypesWithDelay((char)e.KeyCode);
                return true;
            }
            if (dfs == null)
                return false;
            if (dfs.Visible == false)
                return false;
         
            bool r = dfs.Dfs_KeyDown(this, e);
            if (r == false)
                dfs.Close();
            return r;

            
        }
        public void ExpandAll()
        {
            int i = 0;
            while (i < Document.Count)
            {
                Document[i].Expanded = true;

                i++;
            }
        }

        public VSParsers.CSParsers csd { get; set; }

        public TextLocation txt { get; set; }

        public int _shouldwait = 0;

        public void GetCurrentMember()
        {
           

            if (csd == null)
            {
                //csd = new CSDemo();
                //ArrayList L = vp.GetCompileItems();
                //csd.AddProjectFiles(L);
                if(vp != null)
                    csd = vp.CSParsers();
            }


            Point p = Selection.GetCursor();

            int offset = GetOffset(p.X, p.Y);

            string w = Selection.GetCaretWord();


            var data = csd.GetCurrentMember(FileName, this.Document.Text, offset, p.X + 1, p.Y + 1);


            if (data != null)
            {
                //MessageBox.Show(data.GetDefinitionRegion().ToString());

                DomRegion dr = data.GetDefinitionRegion();

                if (dr.FileName != null)
                {
                    if (dr.FileName != "")
                        proxy.LoadProjectFile("", vp, dr.FileName, dr);

                    return;
                }
            }

            string word = Selection.GetCaretWord();

            if (word == "")
                return;

            ITypeDefinition d = csd.GetAllTypes(word);

            if (d == null)
                return;

            //MessageBox.Show(d.FullName);

            foreach (IMember m in d.Members)
            {
                //Console.WriteLine(m.ToString());

                //if(m.Documentation != null)
                //Console.WriteLine(m.Documentation.ToString());

                //Console.WriteLine(m.MemberDefinition.ToString());

                //Console.WriteLine(m.ToReference().ToString());

            }

            string c = "";// csd.TypeToString(d);

            string file = d.Name;

            proxy.LoadFile(c, vp, file);
        }

        public void GetCurrentMembers()
        {
            if (csd == null)
            {
                csd = vp.CSParsers();
            }


            Point p = Selection.GetCursor();

            int offset = GetOffset(p.X, p.Y);

            string w = Selection.GetCaretWord();


            var data = csd.GetCurrentMember(FileName, this.Document.Text, offset, p.X + 1, p.Y + 1);


            if (data != null)
            {
                MessageBox.Show(data.GetDefinitionRegion().ToString());

                DomRegion dr = data.GetDefinitionRegion();

                if (dr.FileName != null)
                {
                    if (dr.FileName != "")
                        proxy.LoadProjectFile("", vp, dr.FileName, dr);

                    return;
                }
            }

            string word = Selection.GetCaretWord();

            if (word == "")
                return;

            ITypeDefinition d = csd.GetAllTypes(word);

            if (d == null)
                return;

            //MessageBox.Show(d.FullName);

            //foreach (IMember m in d.Members)
            //{
            //Console.WriteLine(m.ToString());

            //if(m.Documentation != null)
            //Console.WriteLine(m.Documentation.ToString());

            //Console.WriteLine(m.MemberDefinition.ToString());

            //Console.WriteLine(m.ToReference().ToString());

            //}

            string c = "";// csd.TypeToString(d);

            string file = d.Name;

            proxy.LoadFile(c, vp, file);
        }

        public bool _running = false;

        public async Task SendWithDelay()
        {
            if (vp == null)
                return;

            _running = true;

            
            this.BeginInvoke(new Action( async () =>
            {

                while (_shouldwait > 0)
                {
                    await Task.Delay(50);
                    _shouldwait--;
                }

                Intellisense ie = CodeEditorControl.IntErrors;

                //ArrayList ESC = csd.ResolveAt(txt, Document.Text, FileName, vp);

            if (csd.snx == null)
                csd.snx = new VSParsers.Syntaxer();
                if (csd.snx.dg == null)
                    csd.snx.dg = CodeEditorControl.dg;
                ie.hc = csd.snx.ResolveAt(txt, Document.Text, FileName, vp);



                ie.Content();// LoadErrors(ESC, vp);

                CodeEditorControl.ParserDataChanged(this, new EventArgs());

                _running = false;
            }));
        }
        public bool _runnings = false;

        Stack<int> stack { get; set; }

        void Parse(string c, int offset)
        {
            stack = new Stack<int>();
            List<int> sc = new List<int>();
            List<int> ec = new List<int>();
            List<string> nc = new List<string>();
            List<string> ns = new List<string>();

            int p = 0;
            do
            {
                p = c.IndexOf("(", p);
                if (p >= 0)
                {
                    sc.Add(p);
                    int i = p - 1;
                    string name = "";
                    while (i >= 0 && i < c.Length && c[i] != ')' && c[i] != '(' && !Char.IsWhiteSpace(c[i]))
                    {
                        name = name + c[i];
                        i--;
                    }
                    nc.Add(string.Concat(Enumerable.Reverse(name)));
                }
                if (p < 0)
                    break;
                p++;
            }
            while (p >= 0 && p <= offset);
            p = 0;
            do
            {

                p = c.IndexOf(")", p);
                if (p >= 0)
                {
                    ec.Add(p);
                }
                if (p < 0)
                    break;
                p++;
            }
            while (p >= 0 && p <= offset);
            p = 0;
            int r = 0;
            int level = 0;
            int current = 0;    
            while ((p >= 0 || r >= 0 ) && p <= offset) 
            {
        //        p = c.IndexOf("(", p);
        //        r = c.IndexOf(")", r);
        //        if (p >= 0 || r>= 0)
                {
          //          p++;
                    r = p;
        //            level++;
                    //while(p < offset && p < c.Length - 1)
                    {
                        p = c.IndexOf("(", p);
                        r = c.IndexOf(")", r);
                        if (p > 0)
                        {
                            //                if (p < r)
                            
                            if (p > r && r > 0)
                            {
                                stack.Pop();
                              //  level = stack.Peek();
                                p = r;
                            } else
                            {
                                //level++;
                                current++;
                                stack.Push(current);
                            }
                            p++;
                            r = p;
                        }
                        else if (r > 0)
                        {
                            r++;
                            p = r;
                            stack.Pop();

                            //level = stack.Peek();
                        }
                        else break;
                    }
                }
              
                    
               
            }
  
        }


        public List<Microsoft.CodeAnalysis.ISymbol> methods { get; set; }

        public async Task TypesWithDelay(int KeyChar)
        {
            _runnings = true;

            this.BeginInvoke(new Action(() =>
            {
                //
                Point p = Selection.GetCursor();
                //
                int offset = Caret.Offset;
                //
                Row r = Document[p.Y];
                //
                string w = Selection.GetCaretWord();

                {
                    if (ins != null)
                    {
                        ins.Show();
                        form.Show();
                    }

                    var c = Selection.GetCaretWords();
                    
                    string cc = "";
                    foreach(string s in c)
                    {
                        cc += s;
                    }
                    cc = cc.Trim();
                    if (cc.EndsWith("="))
                    {
                        
                        string[] nc = Selection.GetCaretString().Trim().Split("=".ToCharArray());
                        if (nc.Length > 1)
                        {
                            List<Microsoft.CodeAnalysis.ISymbol> symbols = new List<Microsoft.CodeAnalysis.ISymbol>();
                            string data = nc[nc.Length - 2].Trim();
                            Microsoft.CodeAnalysis.ISymbol symbol = ins.GetSymbol(data);
                            if (symbol is IParameterSymbol )
                            {
                                IParameterSymbol ps = symbol as IParameterSymbol;
                                MessageBox.Show("template = detected for " + symbol.Name + " of type " + ps.Type.Name);
                                Selection.AppendToCaretString(" " + ps.Type.Name);
                                symbols.Add(symbol);
                            } else if (symbol is IPropertySymbol)
                            {
                                IPropertySymbol ps = symbol as IPropertySymbol;
                                MessageBox.Show("template = detected for " + symbol.Name + " of type " + ps.Type.Name);
                                Selection.AppendToCaretString(" " + ps.Type.Name);
                                symbols.Add(symbol);
                            }
                            else if (symbol is IFieldSymbol)
                            {
                                IFieldSymbol ps = symbol as IFieldSymbol;
                                MessageBox.Show("template = detected for " + symbol.Name + " of type " + ps.Type.Name);
                                Selection.AppendToCaretString(" " + ps.Type.Name);
                                symbols.Add(symbol);
                            }
                            else if(symbol != null)
                                MessageBox.Show("template = detected for " + symbol.Name);

                            ins.LoadFast(symbols);

                            ins.SetVirtualMode();

                            
                            ins.AutoSize();

                            ins.Refresh();

                            return;
                        }
                    }
                    string[] words = cc.Trim().Split(".".ToCharArray()).Select(s => s).Where(t => t != "").ToArray();

                    string name = "";

                    Parse(Selection.GetCaretString(),offset);

                    if (cc.StartsWith(".this"))
                        name = "this";
                    else
                    if (KeyChar == 8 && words.Length > 1)
                    {
                        ins.Hide();
                        form.Hide();
                        if (dfs != null)
                            dfs.Close();
                        return;
                    }
                    else if (cc.StartsWith("."))
                    {
                        name = words[0];
                        if (/*name.Contains(")") && */name.Contains("("))
                        {   string[] dd = name.Split("(".ToCharArray());

                            //name = "." + dd[0];
                            name = dd[dd.Length - 1];//dd[0]
                        }
                    }
                    else if (cc.StartsWith("("))
                    {
                        name = words[0];
                        if (name.Contains("("))
                            if (name.Contains(")"))
                            {
                                string[] dd = name.Split(",".ToCharArray());
                                if (Selection.IsCaretInWord())
                                    name = "";
                            }
                    }
                    else if (cc.StartsWith(")"))
                    {
                        name = words[0];
                        if (name.Contains("("))
                            if (name.Contains(")"))
                            {
                                if (Selection.IsCaretInWord())
                                    name = "";
                            }
                    }
                    Point pp = Selection.GetCursor();
                    pp.X = pp.X * ActiveViewControl.View.CharWidth;
                    pp.Y = (ActiveViewControl.Document[p.Y].VisibleIndex - ActiveViewControl.View.FirstVisibleRow + 1) * ActiveViewControl.View.RowHeight + 3;
                    form.Location = this.PointToScreen(pp);
                    List<Microsoft.CodeAnalysis.ISymbol> sn = ins.LoadProjectTypes(vp, FileName, Document.Text, offset, name, cc);
                    if (name.StartsWith("("))
                    {
                        ins.Hide();
                        form.Hide();
                        LoadDefinition(sn, this.PointToScreen(pp));
                        methods = sn;
                        return;
                    }
                    else if ( name.StartsWith(")"))
                    {
                        ins.Hide();
                        form.Hide();
                        if(dfs != null)
                        dfs.Hide();
                        return;
                    }
                    ins.SetVirtualMode();

                    ins.FindSimilarWords(w);

                    ins.AutoSize();

                    ins.Refresh();

                }



                if (form.Visible == true)
                    form.SetTopMost();

                this.Focus();

                _runnings = false;
            }));
        }

        private void CodeEditorControl_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        DefinitionForm df { get; set; }
        void LoadDefinition(List<Microsoft.CodeAnalysis.ISymbol> sn, Point pp)
        {
            if (dfs != null)
                dfs.Close();
            df = new DefinitionForm();
            df.control = this;
            df.SetTopMost();
            df.LoadSymbols(sn);
            dfs = df;
            df.Show();
            this.Focus();
            df.Location = pp;
            df.Refresh();
        }

        public static TopForm form { get; set; }

        public void HideIntellisense()
        {
            if (form != null)
                form.Hide();
            if (ins != null)
                ins.Hide();

            if (ActiveViewControl.InfoTip != null)
                ActiveViewControl.InfoTip.Hide();
        }

        public async Task<int> FindWord(int KeyChar)
        {
            if (csd == null)
            {
                csd = vp.CSParsers();
            }
            _shouldwait++;
            if (ins == null || ins.cec != this)
            {
                ins = new PanelIns(vp);
                ins.Hide();
                ins.cec = this;
                //ins.FastLoader(vp);
                //if (form == null)
                {
                    form = new TopForm();
                    form.Hide();
                    form.Load(ins);
                    ins.form = form;
                }
                ins.Hide();
                form.Hide();
            }
            
            Point p = Selection.GetCursor();
            
            if (txt != null)
                if (p.Y + 1 != txt.Line)
                    _runnings = false;

            txt = new TextLocation(p.Y + 1, p.X);
         
           TypesWithDelay(KeyChar);
         
            return 1;
        }


        private void ActivateSplits()
        {
            if (_upperLeft == null)
            {
                _upperLeft = new EditViewControl(this);
                _upperRight = new EditViewControl(this);
                _lowerLeft = new EditViewControl(this);


                // 
                // UpperLeft
                // 
                _upperLeft.AllowDrop = true;
                _upperLeft.Name = "UpperLeft";
                _upperLeft.TabIndex = 6;
                // 
                // UpperRight
                // 
                _upperRight.AllowDrop = true;
                _upperRight.Name = "UpperRight";
                _upperRight.TabIndex = 4;
                // 
                // LowerLeft
                // 
                _lowerLeft.AllowDrop = true;
                _lowerLeft.Name = "LowerLeft";
                _lowerLeft.TabIndex = 5;


                _splitView1.Controls.AddRange(new Control[]
                    {
                        _upperLeft,
                        _lowerLeft,
                        _upperRight
                    });

                _splitView1.UpperRight = _lowerLeft;
                _splitView1.UpperLeft = _upperLeft;
                _splitView1.LowerLeft = _upperRight;

                CreateViews();

                this.AutoListIcons = this.AutoListIcons;
                //this.InfoTipImage = this.InfoTipImage;
                this.ChildBorderStyle = this.ChildBorderStyle;
                this.ChildBorderColor = this.ChildBorderColor;
                this.BackColor = this.BackColor;
                this.Document = this.Document;
                this.Redraw();
            }
        }

        #region EventHandlers

        protected virtual void OnClipboardUpdated(CopyEventArgs e)
        {
            if (ClipboardUpdated != null)
                ClipboardUpdated(this, e);
        }

        protected virtual void OnRowMouseDown(RowMouseEventArgs e)
        {
            if (RowMouseDown != null)
                RowMouseDown(this, e);
        }

        protected virtual void OnRowMouseMove(RowMouseEventArgs e)
        {
            if (RowMouseMove != null)
                RowMouseMove(this, e);
        }

        protected virtual void OnRowMouseUp(RowMouseEventArgs e)
        {
            if (RowMouseUp != null)
                RowMouseUp(this, e);
        }

        protected virtual void OnRowClick(RowMouseEventArgs e)
        {
            if (RowClick != null)
                RowClick(this, e);
        }

        protected virtual void OnRowDoubleClick(RowMouseEventArgs e)
        {
            if (RowDoubleClick != null)
                RowDoubleClick(this, e);
        }


        private void ParseTimer_Tick(object sender, EventArgs e)
        {
            Document.ParseSome();
        }

        protected virtual void OnInfoTipSelectedIndexChanged()
        {
            if (InfoTipSelectedIndexChanged != null)
                InfoTipSelectedIndexChanged(this, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (_ActiveView != null)
            {
                _ActiveView.Focus();
            }
        }

        private void TopThumb_DoubleClick(object sender, EventArgs e)
        {
            //splitView1.Split5050h ();
        }

        private void LeftThumb_DoubleClick(object sender, EventArgs e)
        {
            //splitView1.Split5050v ();
        }

        private void TopThumb_MouseDown(object sender, MouseEventArgs e)
        {
            this.ActivateSplits();

            long t = DateTime.Now.Ticks - _ticks;
            _ticks = DateTime.Now.Ticks;


            if (t < 3000000)
            {
                _splitView1.Split5050h();
            }
            else
            {
                _splitView1.InvokeMouseDownh();
            }
        }

        private void LeftThumb_MouseDown(object sender, MouseEventArgs e)
        {
            this.ActivateSplits();

            long t = DateTime.Now.Ticks - _ticks;
            _ticks = DateTime.Now.Ticks;


            if (t < 3000000)
            {
                _splitView1.Split5050v();
            }
            else
            {
                _splitView1.InvokeMouseDownv();
            }
        }

        private void SplitView_Resizing(object sender, EventArgs e)
        {
            _lowerRight.TopThumbVisible = false;
            _lowerRight.LeftThumbVisible = false;
        }

        private void SplitView_HideTop(object sender, EventArgs e)
        {
            _lowerRight.TopThumbVisible = true;
        }

        private void SplitView_HideLeft(object sender, EventArgs e)
        {
            _lowerRight.LeftThumbVisible = true;
        }

        private void View_Enter(object sender, EventArgs e)
        {
            this._ActiveView = (EditViewControl)sender;
        }

        private void View_Leave(object sender, EventArgs e)
        {
            //	((EditViewControl)sender).RemoveFocus ();
        }

        private void View_RowClick(object sender, RowMouseEventArgs e)
        {
            OnRowClick(e);
        }

        private void View_RowDoubleClick(object sender, RowMouseEventArgs e)
        {
            OnRowDoubleClick(e);
        }

        private void View_RowMouseDown(object sender, RowMouseEventArgs e)
        {
            OnRowMouseDown(e);
        }

        private void View_RowMouseMove(object sender, RowMouseEventArgs e)
        {
            OnRowMouseMove(e);
        }

        private void View_RowMouseUp(object sender, RowMouseEventArgs e)
        {
            OnRowMouseUp(e);
        }

        private void View_ClipboardUpdated(object sender, CopyEventArgs e)
        {
            this.OnClipboardUpdated(e);
        }


        public void OnRenderRow(RowPaintEventArgs e)
        {
            if (RenderRow != null)
                RenderRow(this, e);
        }

        public void OnWordMouseHover(ref WordMouseEventArgs e)
        {
            if (WordMouseHover != null)
                WordMouseHover(this, ref e);
        }

        public void OnWordMouseDown(ref WordMouseEventArgs e)
        {
            if (WordMouseDown != null)
                WordMouseDown(this, ref e);
        }

        protected virtual void OnCaretChange(object sender)
        {
            if (CaretChange != null)
                CaretChange(this, null);
        }

        protected virtual void OnSelectionChange(object sender)
        {
            if (SelectionChange != null)
                SelectionChange(this, null);
        }

        private void View_CaretChanged(object s, EventArgs e)
        {
            OnCaretChange(s);
        }

        private void View_SelectionChanged(object s, EventArgs e)
        {
            OnSelectionChange(s);
        }

        private void View_DoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(e);
        }

        private void View_MouseUp(object sender, MouseEventArgs e)
        {
            EditViewControl ev = (EditViewControl)sender;
            MouseEventArgs ea = new MouseEventArgs(e.Button, e.Clicks, e.X + ev.Location.X + ev.BorderWidth, e.Y + ev.Location.Y + ev.BorderWidth, e.Delta);
            OnMouseUp(ea);
        }

        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            EditViewControl ev = (EditViewControl)sender;
            MouseEventArgs ea = new MouseEventArgs(e.Button, e.Clicks, e.X + ev.Location.X + ev.BorderWidth, e.Y + ev.Location.Y + ev.BorderWidth, e.Delta);
            OnMouseMove(ea);
        }

        private void View_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void View_MouseHover(object sender, EventArgs e)
        {
            OnMouseHover(e);
        }

        private void View_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            EditViewControl ev = (EditViewControl)sender;
            MouseEventArgs ea = new MouseEventArgs(e.Button, e.Clicks, e.X + ev.Location.X + ev.BorderWidth, e.Y + ev.Location.Y + ev.BorderWidth, e.Delta);
            OnMouseDown(ea);
            if (ins == null)
                return;
            form.Hide();
            ins.Hide();
        }

        private void View_KeyUp(object sender, KeyEventArgs e)
        {
            OnKeyUp(e);
            if (e.Control == false)
                _ctrlK = false;
            this.Saved = false;
        }

        private void View_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnKeyPress(e);
        }

        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        private void View_Click(object sender, EventArgs e)
        {
            OnClick(e);
        }

        private void View_DragOver(object sender, DragEventArgs e)
        {
            OnDragOver(e);
        }

        private void View_DragLeave(object sender, EventArgs e)
        {
            OnDragLeave(e);
        }

        private void View_DragEnter(object sender, DragEventArgs e)
        {
            OnDragEnter(e);
        }

        private void View_DragDrop(object sender, DragEventArgs e)
        {
            OnDragDrop(e);
            this.Saved = false;
        }

        private void View_InfoTipSelectedIndexChanged(object sender, EventArgs e)
        {
            OnInfoTipSelectedIndexChanged();
        }

        #endregion

        #region Private Properties

        private ArrayList Views
        {
            get { return _Views; }
            set { _Views = value; }
        }

        #endregion

        #region DISPOSE()

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                    _components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion //END DISPOSE

        /// <summary>
        /// 
        /// </summary>
        ~CodeEditorControl()
        {
        }

        #region Private/Protected/Internal methods

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeEditorControl));
            this._GutterIcons = new System.Windows.Forms.ImageList(this.components);
            this._AutoListIcons = new System.Windows.Forms.ImageList(this.components);
            this._parseTimer = new AIMS.Libraries.CodeEditor.Core.Timers.WeakTimer(this.components);
            this.SuspendLayout();
            // 
            // _GutterIcons
            // 
            this._GutterIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_GutterIcons.ImageStream")));
            this._GutterIcons.TransparentColor = System.Drawing.Color.Fuchsia;
            this._GutterIcons.Images.SetKeyName(0, "break_point.png");
            this._GutterIcons.Images.SetKeyName(1, "BookMarkForGuttor.bmp");
            this._GutterIcons.Images.SetKeyName(2, "");
            this._GutterIcons.Images.SetKeyName(3, "");
            this._GutterIcons.Images.SetKeyName(4, "");
            this._GutterIcons.Images.SetKeyName(5, "");
            this._GutterIcons.Images.SetKeyName(6, "");
            this._GutterIcons.Images.SetKeyName(7, "");
            this._GutterIcons.Images.SetKeyName(8, "");
            this._GutterIcons.Images.SetKeyName(9, "");
            this._GutterIcons.Images.SetKeyName(10, "");
            this._GutterIcons.Images.SetKeyName(11, "");
            this._GutterIcons.Images.SetKeyName(12, "");
            // 
            // _AutoListIcons
            // 
            this._AutoListIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_AutoListIcons.ImageStream")));
            this._AutoListIcons.TransparentColor = System.Drawing.Color.Magenta;
            this._AutoListIcons.Images.SetKeyName(0, "VSObject_Class.bmp");
            this._AutoListIcons.Images.SetKeyName(1, "VSObject_Class_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(2, "VSObject_Class_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(3, "VSObject_Class_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(4, "VSObject_Class_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(5, "VSObject_Class_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(6, "VSObject_Constant.bmp");
            this._AutoListIcons.Images.SetKeyName(7, "VSObject_Constant_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(8, "VSObject_Constant_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(9, "VSObject_Constant_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(10, "VSObject_Constant_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(11, "VSObject_Constant_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(12, "VSObject_Delegate.bmp");
            this._AutoListIcons.Images.SetKeyName(13, "VSObject_Delegate_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(14, "VSObject_Delegate_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(15, "VSObject_Delegate_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(16, "VSObject_Delegate_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(17, "VSObject_Delegate_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(18, "VSObject_Enum.bmp");
            this._AutoListIcons.Images.SetKeyName(19, "VSObject_Enum_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(20, "VSObject_Enum_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(21, "VSObject_Enum_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(22, "VSObject_Enum_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(23, "VSObject_Enum_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(24, "VSObject_EnumItem.bmp");
            this._AutoListIcons.Images.SetKeyName(25, "VSObject_EnumItem_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(26, "VSObject_EnumItem_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(27, "VSObject_EnumItem_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(28, "VSObject_EnumItem_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(29, "VSObject_EnumItem_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(30, "VSObject_Event.bmp");
            this._AutoListIcons.Images.SetKeyName(31, "VSObject_Event_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(32, "VSObject_Event_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(33, "VSObject_Event_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(34, "VSObject_Event_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(35, "VSObject_Event_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(36, "VSObject_Exception.bmp");
            this._AutoListIcons.Images.SetKeyName(37, "VSObject_Exception_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(38, "VSObject_Exception_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(39, "VSObject_Exception_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(40, "VSObject_Exception_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(41, "VSObject_Exception_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(42, "VSObject_Field.bmp");
            this._AutoListIcons.Images.SetKeyName(43, "VSObject_Field_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(44, "VSObject_Field_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(45, "VSObject_Field_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(46, "VSObject_Field_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(47, "VSObject_Field_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(48, "VSObject_Interface.bmp");
            this._AutoListIcons.Images.SetKeyName(49, "VSObject_Interface_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(50, "VSObject_Interface_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(51, "VSObject_Interface_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(52, "VSObject_Interface_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(53, "VSObject_Interface_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(54, "VSObject_Macro.bmp");
            this._AutoListIcons.Images.SetKeyName(55, "VSObject_Macro_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(56, "VSObject_Macro_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(57, "VSObject_Macro_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(58, "VSObject_Macro_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(59, "VSObject_Macro_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(60, "VSObject_Map.bmp");
            this._AutoListIcons.Images.SetKeyName(61, "VSObject_Map_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(62, "VSObject_Map_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(63, "VSObject_Map_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(64, "VSObject_Map_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(65, "VSObject_Map_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(66, "VSObject_MapItem.bmp");
            this._AutoListIcons.Images.SetKeyName(67, "VSObject_MapItem_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(68, "VSObject_MapItem_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(69, "VSObject_MapItem_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(70, "VSObject_MapItem_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(71, "VSObject_MapItem_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(72, "VSObject_Method.bmp");
            this._AutoListIcons.Images.SetKeyName(73, "VSObject_Method_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(74, "VSObject_Method_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(75, "VSObject_Method_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(76, "VSObject_Method_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(77, "VSObject_Method_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(78, "VSObject_MethodOverload.bmp");
            this._AutoListIcons.Images.SetKeyName(79, "VSObject_MethodOverload_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(80, "VSObject_MethodOverload_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(81, "VSObject_MethodOverload_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(82, "VSObject_MethodOverload_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(83, "VSObject_MethodOverload_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(84, "VSObject_Module.bmp");
            this._AutoListIcons.Images.SetKeyName(85, "VSObject_Module_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(86, "VSObject_Module_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(87, "VSObject_Module_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(88, "VSObject_Module_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(89, "VSObject_Module_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(90, "VSObject_Namespace.bmp");
            this._AutoListIcons.Images.SetKeyName(91, "VSObject_Namespace_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(92, "VSObject_Namespace_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(93, "VSObject_Namespace_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(94, "VSObject_Namespace_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(95, "VSObject_Namespace_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(96, "VSObject_Object.bmp");
            this._AutoListIcons.Images.SetKeyName(97, "VSObject_Object_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(98, "VSObject_Object_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(99, "VSObject_Object_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(100, "VSObject_Object_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(101, "VSObject_Object_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(102, "VSObject_Operator.bmp");
            this._AutoListIcons.Images.SetKeyName(103, "VSObject_Operator_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(104, "VSObject_Operator_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(105, "VSObject_Operator_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(106, "VSObject_Operator_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(107, "VSObject_Operator_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(108, "VSObject_Properties.bmp");
            this._AutoListIcons.Images.SetKeyName(109, "VSObject_Properties_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(110, "VSObject_Properties_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(111, "VSObject_Properties_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(112, "VSObject_Properties_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(113, "VSObject_Properties_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(114, "VSObject_Structure.bmp");
            this._AutoListIcons.Images.SetKeyName(115, "VSObject_Structure_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(116, "VSObject_Structure_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(117, "VSObject_Structure_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(118, "VSObject_Structure_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(119, "VSObject_Structure_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(120, "VSObject_Template.bmp");
            this._AutoListIcons.Images.SetKeyName(121, "VSObject_Template_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(122, "VSObject_Template_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(123, "VSObject_Template_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(124, "VSObject_Template_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(125, "VSObject_Template_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(126, "VSObject_Type.bmp");
            this._AutoListIcons.Images.SetKeyName(127, "VSObject_Type_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(128, "VSObject_Type_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(129, "VSObject_Type_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(130, "VSObject_Type_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(131, "VSObject_Type_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(132, "VSObject_TypeDef.bmp");
            this._AutoListIcons.Images.SetKeyName(133, "VSObject_TypeDef_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(134, "VSObject_TypeDef_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(135, "VSObject_TypeDef_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(136, "VSObject_TypeDef_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(137, "VSObject_TypeDef_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(138, "VSObject_Union.bmp");
            this._AutoListIcons.Images.SetKeyName(139, "VSObject_Union_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(140, "VSObject_Union_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(141, "VSObject_Union_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(142, "VSObject_Union_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(143, "VSObject_Union_Shortcut.bmp");
            this._AutoListIcons.Images.SetKeyName(144, "VSObject_ValueType.bmp");
            this._AutoListIcons.Images.SetKeyName(145, "VSObject_ValueType_Friend.bmp");
            this._AutoListIcons.Images.SetKeyName(146, "VSObject_ValueType_Private.bmp");
            this._AutoListIcons.Images.SetKeyName(147, "VSObject_ValueType_Protected.bmp");
            this._AutoListIcons.Images.SetKeyName(148, "VSObject_ValueType_Sealed.bmp");
            this._AutoListIcons.Images.SetKeyName(149, "VSObject_ValueType_Shortcut.bmp");
            // 
            // _parseTimer
            // 
            this._parseTimer.Enabled = true;
            this._parseTimer.Interval = 1;
            this._parseTimer.Tick += new System.EventHandler(this.ParseTimer_Tick);
            this.ResumeLayout(false);

        }


        protected override void OnLoad(EventArgs e)
        {
            this.Refresh();
        }

        private void Redraw()
        {
            if (this.Views == null)
                return;

            foreach (EditViewControl ev in this.Views)
            {
                if (ev != null)
                {
                    ev.Refresh();
                }
            }
        }

        private void InitGraphics()
        {
            if (this.Views == null || this.Parent == null)
                return;

            foreach (EditViewControl ev in this.Views)
            {
                ev.InitGraphics();
            }
        }


        private bool _doOnce = false;

        private void CreateViews()
        {
            if (_upperRight != null)
            {
                Views.Add(_upperRight);
                Views.Add(_upperLeft);
                Views.Add(_lowerLeft);
            }

            if (!_doOnce)
            {
                Views.Add(_lowerRight);
                _lowerRight.TopThumbVisible = true;
                _lowerRight.LeftThumbVisible = true;
                _lowerRight.TopThumb.DoubleClick += new EventHandler(TopThumb_DoubleClick);
                _lowerRight.LeftThumb.DoubleClick += new EventHandler(LeftThumb_DoubleClick);

                _lowerRight.TopThumb.MouseDown += new MouseEventHandler(TopThumb_MouseDown);
                _lowerRight.LeftThumb.MouseDown += new MouseEventHandler(LeftThumb_MouseDown);
            }


            foreach (EditViewControl ev in Views)
            {
                if (_doOnce && ev == _lowerRight)
                    continue;

                //attatch events to views
                ev.Enter += new EventHandler(this.View_Enter);
                ev.Leave += new EventHandler(this.View_Leave);
                ev.GotFocus += new EventHandler(this.View_Enter);
                ev.LostFocus += new EventHandler(this.View_Leave);
                ev.CaretChange += new EventHandler(this.View_CaretChanged);
                ev.SelectionChange += new EventHandler(this.View_SelectionChanged);
                ev.Click += new EventHandler(this.View_Click);
                ev.DoubleClick += new EventHandler(this.View_DoubleClick);
                ev.MouseDown += new MouseEventHandler(this.View_MouseDown);
                ev.MouseEnter += new EventHandler(this.View_MouseEnter);
                ev.MouseHover += new EventHandler(this.View_MouseHover);
                ev.MouseLeave += new EventHandler(this.View_MouseLeave);
                ev.MouseMove += new MouseEventHandler(this.View_MouseMove);
                ev.MouseUp += new MouseEventHandler(this.View_MouseUp);
                ev.KeyDown += new System.Windows.Forms.KeyEventHandler(this.View_KeyDown);
                ev.KeyPress += new KeyPressEventHandler(this.View_KeyPress);
                ev.KeyUp += new System.Windows.Forms.KeyEventHandler(this.View_KeyUp);
                ev.DragDrop += new DragEventHandler(this.View_DragDrop);
                ev.DragOver += new DragEventHandler(this.View_DragOver);
                ev.DragLeave += new EventHandler(this.View_DragLeave);
                ev.DragEnter += new DragEventHandler(this.View_DragEnter);

                //if (ev.InfoTip != null)
                //{
                //	ev.InfoTip.Data = "";
                //	ev.InfoTip.SelectedIndexChanged += new EventHandler(this.View_InfoTipSelectedIndexChanged);
                //}

                ev.RowClick += new RowMouseHandler(this.View_RowClick);
                ev.RowDoubleClick += new RowMouseHandler(this.View_RowDoubleClick);

                ev.RowMouseDown += new RowMouseHandler(this.View_RowMouseDown);
                ev.RowMouseMove += new RowMouseHandler(this.View_RowMouseMove);
                ev.RowMouseUp += new RowMouseHandler(this.View_RowMouseUp);
                ev.ClipboardUpdated += new CopyHandler(this.View_ClipboardUpdated);
            }

            _doOnce = true;
            this.Redraw();
        }

        #endregion //END Private/Protected/Internal methods

        public void AttachDocument(SyntaxDocument document)
        {

            if (_Document != null)
                MessageBox.Show("New document is being loaded..");

            _Document=document;

            if (_Document != null)
            {
                _Document.ParsingCompleted -= new EventHandler(this.OnParsingCompleted);
                _Document.Parsing -= new EventHandler(this.OnParse);
                _Document.Change -= new EventHandler(this.OnChange);
            }

            if (document == null)
                document = new SyntaxDocument();

            _Document = document;

            if (_Document != null)
            {
                _Document.ParsingCompleted += new EventHandler(this.OnParsingCompleted);
                _Document.Parsing += new EventHandler(this.OnParse);
                _Document.Change += new EventHandler(this.OnChange);
            }

            this.Redraw();
        }


        protected virtual void OnParse(object Sender, EventArgs e)
        {
            foreach (EditViewControl ev in Views)
            {
                ev.OnParse();
            }
        }

        protected virtual void OnParsingCompleted(object Sender, EventArgs e)
        {
            foreach (EditViewControl ev in Views)
            {
                ev.Invalidate();
            }
        }

        protected virtual void OnChange(object Sender, EventArgs e)
        {
            if (Views == null)
                return;

            foreach (EditViewControl ev in Views)
            {
                ev.OnChange();
            }
            this.OnTextChanged(EventArgs.Empty);
        }

        public void RemoveCurrentRow()
        {
            _ActiveView.RemoveCurrentRow();
        }

        public void CutClear()
        {
            _ActiveView.CutClear();
        }


        [Browsable(false)]
        [Obsolete("Use .FontName and .FontSize", true)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }



        //		[Browsable(true)]
        //		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
        //		[RefreshProperties (RefreshProperties.All)]
        //		public override string Text
        //		{
        //			get
        //			{
        //				return this.Document.Text;
        //			}
        //			set
        //			{
        //				this.Document.Text=value;
        //			}
        //		}

        [Browsable(false)]
        [Obsolete("Apply a syntax instead", true)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        public void AutoListInsertSelectedText()
        {
            _ActiveView.InsertAutolistText();
        }

        /// <summary>
        /// The currently highlighted text in the autolist.
        /// </summary>
        [Browsable(false)]
        public string AutoListSelectedText
        {
            get { return _ActiveView.AutoList.SelectedText; }
            set
            {
                if (_ActiveView == null || _ActiveView.AutoList == null)
                    return;

                _ActiveView.AutoList.SelectItem(value);
            }
        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)WindowMessage.WM_SETFOCUS)
            {
                if (_ActiveView != null)
                    _ActiveView.Focus();
            }
            // WM_SYSCOMMAND
            if (m.Msg == 0x0112)
            {
                if (m.WParam == new IntPtr(0xF030) // Maximize event - SC_MAXIMIZE from Winuser.h
                    || m.WParam == new IntPtr(0xF120)) // Restore event - SC_RESTORE from Winuser.h
                {
                    ActiveViewControl.HideAndRedraw(); //; MessageBox.Show("maximized intercepted");
                }
            }
        }

        static public Settings settings { get; set; }

        public void LoadSettings(Settings settings)
        {
            _ShowLineNumbers = settings.ShowLineNumbers;

            GutterMarginWidth = settings.GutterMarginWidth;
            _SmoothScrollSpeed = settings.SmoothScrollSpeed;
            _RowPadding = settings.RowPadding;
            _ticks = settings.Ticks;
            _ShowWhitespace = settings.ShowWhitespace;
            _ShowTabGuides = settings.ShowTabGuides;
            _ShowLineNumbers = settings.ShowLineNumbers;
            _ShowGutterMargin = settings.ShowGutterMargin;
            _ReadOnly = settings.ReadOnly;
            _HighLightActiveLine = settings.HighLightActiveLine;
            _VirtualWhitespace = settings.VirtualWhitespace;
            _BracketMatching = settings.BracketMatching;
            //_OverWrite = settings._OverWrite;
            //_ParseOnPaste = settings.ParseOnPaste;
            _SmoothScroll = settings.SmoothScroll;
            _AllowBreakPoints = settings.AllowBreakPoints;
            _LockCursorUpdate = settings.LockCursorUpdate;
            // _BracketBorder = settings.BracketBorder;
            // _TabGuide = ControlPa.Light(Systems.ControlLight);
            // _Outline = Systems.ControlDark;
            // _Whitespace = Systems.ControlDark;
            // _Separator = Systems.Control;
            // _SelectionBack = Systems.Highlight;
            // _SelectionFore = Systems.HighlightText;
            // _InactiveSelectionBack = Systems.ControlDark;
            // _InactiveSelectionFore = Systems.ControlLight;
            // _BreakPoBack = .DarkRed;
            // _BreakPoFore = .White;
            // _Back = .White;
            HighLightedLineColor = Color.White;
            // _GutterMargin = Systems.Control;
            // _LineNumberBack = Systems.Window;
            // _LineNumberFore = .Teal;
            // _GutterMarginBorder = Systems.ControlDark;
            // _LineNumberBorder = .Teal;
            // _BracketFore = .Black;
            // _BracketBack = .LightSteelBlue;
            // _ScopeBack = .Transparent;
            // _ScopeIndicator = .Transparent;
            //TextDrawType _TextDrawStyle = 0;
            //IndentStyle _Indent = IndentStyle.LastRow;
            //string _FontName = "Courier New";
            //float _FontSize = 10f;

        }

        public void InsertText(string text, int offset)
        {
            TextPoint p = Caret.Position;

            Document.InsertText(text, p.X, p.Y);

            Caret.Position.X += text.Length;

            int c = Selection.GetCursorLine();

            string w = Selection.GetCaretWord();

            if (w != "")
            {

                Row r = Document[c];

                string s = r.Text;
                
                s = s.Replace(w, text);

                r.SetText(s);

            }
        }
    }

    [Serializable]
    public class Settings
    {
        public string ResumeAtStartup { get; set; }

        public string Theme { get; set; }


        private int _GutterMarginWidth = 0;

        public int GutterMarginWidth
        {
            get { return _GutterMarginWidth; }
            set { _GutterMarginWidth = value; }
        }
        private int _SmoothScrollSpeed = 2;

        public int SmoothScrollSpeed
        {
            get { return _SmoothScrollSpeed; }
            set { _SmoothScrollSpeed = value; }
        }
        private int _RowPadding = 0;

        public int RowPadding
        {
            get { return _RowPadding; }
            set { _RowPadding = value; }
        }
        private long _ticks = 0;

        public long Ticks
        {
            get { return _ticks; }
            set { _ticks = value; }
        }
        private bool _ShowWhitespace = false;

        public bool ShowWhitespace
        {
            get { return _ShowWhitespace; }
            set { _ShowWhitespace = value; }
        }
        private bool _ShowTabGuides = false;

        public bool ShowTabGuides
        {
            get { return _ShowTabGuides; }
            set { _ShowTabGuides = value; }
        }
        private bool _ShowLineNumbers = false;

        public bool ShowLineNumbers
        {
            get { return _ShowLineNumbers; }
            set { _ShowLineNumbers = value; }
        }
        private bool _ShowGutterMargin = true;

        public bool ShowGutterMargin
        {
            get { return _ShowGutterMargin; }
            set { _ShowGutterMargin = value; }
        }
        private bool _ReadOnly = false;

        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }
        private bool _HighLightActiveLine = true;

        public bool HighLightActiveLine
        {
            get { return _HighLightActiveLine; }
            set { _HighLightActiveLine = value; }
        }
        private bool _VirtualWhitespace = false;

        public bool VirtualWhitespace
        {
            get { return _VirtualWhitespace; }
            set { _VirtualWhitespace = value; }
        }
        private bool _BracketMatching = true;

        public bool BracketMatching
        {
            get { return _BracketMatching; }
            set { _BracketMatching = value; }
        }
        private bool _OverWrite = false;

        public bool OverWrite
        {
            get { return _OverWrite; }
            set { _OverWrite = value; }
        }
        private bool _ParseOnPaste = false;

        public bool ParseOnPaste
        {
            get { return _ParseOnPaste; }
            set { _ParseOnPaste = value; }
        }
        private bool _SmoothScroll = false;

        public bool SmoothScroll
        {
            get { return _SmoothScroll; }
            set { _SmoothScroll = value; }
        }
        private bool _AllowBreakPoints = true;

        public bool AllowBreakPoints
        {
            get { return _AllowBreakPoints; }
            set { _AllowBreakPoints = value; }
        }
        private bool _LockCursorUpdate = false;

        public bool LockCursorUpdate
        {
            get { return _LockCursorUpdate; }
            set { _LockCursorUpdate = value; }
        }
        private Color _BracketBorderColor = Color.DarkBlue;

        public Color BracketBorderColor
        {
            get { return _BracketBorderColor; }
            set { _BracketBorderColor = value; }
        }
        private Color _TabGuideColor = ControlPaint.Light(SystemColors.ControlLight);

        public Color TabGuideColor
        {
            get { return _TabGuideColor; }
            set { _TabGuideColor = value; }
        }
        private Color _OutlineColor = SystemColors.ControlDarkDark;

        public Color OutlineColor
        {
            get { return _OutlineColor; }
            set { _OutlineColor = value; }
        }
        private Color _WhitespaceColor = SystemColors.ControlDark;

        public Color WhitespaceColor
        {
            get { return _WhitespaceColor; }
            set { _WhitespaceColor = value; }
        }
        private Color _SeparatorColor = SystemColors.Control;

        public Color SeparatorColor
        {
            get { return _SeparatorColor; }
            set { _SeparatorColor = value; }
        }
        private Color _SelectionBackColor = SystemColors.Highlight;

        public Color SelectionBackColor
        {
            get { return _SelectionBackColor; }
            set { _SelectionBackColor = value; }
        }
        private Color _SelectionForeColor = SystemColors.HighlightText;

        public Color SelectionForeColor
        {
            get { return _SelectionForeColor; }
            set { _SelectionForeColor = value; }
        }
        private Color _InactiveSelectionBackColor = SystemColors.ControlDark;

        public Color InactiveSelectionBackColor
        {
            get { return _InactiveSelectionBackColor; }
            set { _InactiveSelectionBackColor = value; }
        }
        private Color _InactiveSelectionForeColor = SystemColors.ControlLight;

        public Color InactiveSelectionForeColor
        {
            get { return _InactiveSelectionForeColor; }
            set { _InactiveSelectionForeColor = value; }
        }
        private Color _BreakPointBackColor = Color.DarkRed;

        public Color BreakPointBackColor
        {
            get { return _BreakPointBackColor; }
            set { _BreakPointBackColor = value; }
        }
        private Color _BreakPointForeColor = Color.White;

        public Color BreakPointForeColor
        {
            get { return _BreakPointForeColor; }
            set { _BreakPointForeColor = value; }
        }
        private Color _BackColor = Color.White;

        public Color BackColor
        {
            get { return _BackColor; }
            set { _BackColor = value; }
        }
        private Color _HighLightedLineColor = Color.LightBlue;

        public Color HighLightedLineColor
        {
            get { return _HighLightedLineColor; }
            set { _HighLightedLineColor = value; }
        }
        private Color _GutterMarginColor = SystemColors.Control;

        public Color GutterMarginColor
        {
            get { return _GutterMarginColor; }
            set { _GutterMarginColor = value; }
        }
        private Color _LineNumberBackColor = SystemColors.Window;

        public Color LineNumberBackColor
        {
            get { return _LineNumberBackColor; }
            set { _LineNumberBackColor = value; }
        }
        private Color _LineNumberForeColor = Color.Teal;

        public Color LineNumberForeColor
        {
            get { return _LineNumberForeColor; }
            set { _LineNumberForeColor = value; }
        }
        private Color _GutterMarginBorderColor = SystemColors.ControlDark;

        public Color GutterMarginBorderColor
        {
            get { return _GutterMarginBorderColor; }
            set { _GutterMarginBorderColor = value; }
        }
        private Color _LineNumberBorderColor = Color.Teal;

        public Color LineNumberBorderColor
        {
            get { return _LineNumberBorderColor; }
            set { _LineNumberBorderColor = value; }
        }
        private Color _BracketForeColor = Color.Black;

        public Color BracketForeColor
        {
            get { return _BracketForeColor; }
            set { _BracketForeColor = value; }
        }
        private Color _BracketBackColor = Color.LightSteelBlue;

        public Color BracketBackColor
        {
            get { return _BracketBackColor; }
            set { _BracketBackColor = value; }
        }
        private Color _ScopeBackColor = Color.Transparent;

        public Color ScopeBackColor
        {
            get { return _ScopeBackColor; }
            set { _ScopeBackColor = value; }
        }
        private Color _ScopeIndicatorColor = Color.Transparent;

        public Color ScopeIndicatorColor
        {
            get { return _ScopeIndicatorColor; }
            set { _ScopeIndicatorColor = value; }
        }
        private TextDrawType _TextDrawStyle = 0;

        public TextDrawType TextDrawStyle
        {
            get { return _TextDrawStyle; }
            set { _TextDrawStyle = value; }
        }
        private IndentStyle _Indent = IndentStyle.LastRow;

        public IndentStyle Indent
        {
            get { return _Indent; }
            set { _Indent = value; }
        }
        private string _FontName = "Courier New";

        public string FontName
        {
            get { return _FontName; }
            set { _FontName = value; }
        }
        private float _FontSize = 10f;

        public float FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }
    }

    public class ListViewExt : ListView
    {
        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;
        private const int WM_ERASEBKGND = 0x14;
        public event EventHandler Scroll;

        public ListViewExt() : base()
        {
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_ERASEBKGND)
            {
                m.Result = (IntPtr)0;
            }
            else if (m.Msg == 0x0F)
            {
                HandlePaint(ref m);
            }
        }

        protected void HandlePaint(ref Message m)
        {
            // base.WndProc(ref m);

            using (Graphics g = this.CreateGraphics())
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.Trimming = StringTrimming.EllipsisCharacter;
                g.DrawString("Some text", new Font("Tahoma", 13),
                    SystemBrushes.ControlDark, this.ClientRectangle, sf);
            }
        }
    }

    public class TopForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr s_HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;


        public PanelIns panel { get; set; }

        public TopForm()
        {
        }

        public void Load(PanelIns p)
        {
            SuspendLayout();

            panel = p;

            this.FormBorderStyle = FormBorderStyle.None;

            this.TopLevel = true;

            this.Controls.Add(p);

            p.Dock = DockStyle.Fill;

            ResumeLayout();
        }

        public void SetTopMost()
        {
            SetWindowPos(this.Handle, s_HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }
    }

    public class SortComparer : IComparer<ListViewItem>
    {

        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer<ListViewItem>.Compare(ListViewItem x, ListViewItem y)
        {
            ListViewItem v0 = x as ListViewItem;
            ListViewItem v1 = y as ListViewItem;
            string s0 = v0.Text;
            string s1 = v1.Text;
            return ((new CaseInsensitiveComparer()).Compare(s0, s1));
        }

    }
    public class EqualityComparers : IEqualityComparer<Object>
    {
        new public bool Equals(Object x, Object y)
        {
            ListViewItem v0 = x as ListViewItem;
            ListViewItem v1 = y as ListViewItem;
            string s0 = v0.Text;
            string s1 = v1.Text;
            return s0.Equals(s1); 
        }

        public int GetHashCode(Object obj)
        {
            return obj.GetHashCode();
        }
    

    }
    public class PanelIns : Panel
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr s_HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_SHOWWINDOW = 0x0040;


        [DllImport("user32")]
        private static extern long ShowScrollBar(long hwnd, long wBar, long bShow);
        private long _SB_HORZ = 0;
        private long _SB_VERT = 1;
        private long _SB_BOTH = 3;
        //private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        // private const UInt32 SWP_NOSIZE = 0x0001;
        //private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        // [DllImport("user32.dll")]
        // [return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public Label label { get; set; }


        public List<ListViewItem> cc { get; set; }

        public ImageList img { get; set; }

        public CodeEditorControl cec { get; set; }

        public TopForm form { get; set; }

        public ArrayList LL { get; set; }

        public PanelIns(VSProject vp) : base()
        {
            SuspendLayout();

            this.BackColor = SystemColors.Control;

            this.Size = new Size(353 + 14, 315);

            v = new ListView();

            v.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right/* | AnchorStyles.Bottom*/;

            v.BackColor = SystemColors.Control;

            v.View = View.Details;

            v.Size = new Size(353, 315);

            v.Location = new Point(0, 0);

            v.FullRowSelect = true;
            
            this.Controls.Add(v);

            this.BorderStyle = BorderStyle.None;

            this.BorderColor = SystemColors.ControlDark;

            b = new TextBox();

            b.Multiline = true;

            b.Size = new Size(14, 315);

            b.Location = new Point(354, 0);

            b.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            s = new Button();
            s.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            s.Size = new Size(14, 14);
            s.Location = new Point(354, 14 + 1);

            su = new Button();
            su.Image = resource.arrowup;
            su.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            su.Size = new Size(14, 14);
            su.Location = new Point(354, 0);
            this.Controls.Add(su);

            du = new Button();
            du.Image = resource.arrowdw;
            du.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            du.Size = new Size(14, 14);
            du.Location = new Point(354, 315 - 14);
            this.Controls.Add(du);

            s.MouseDown += S_MouseDown;

            s.MouseUp += S_MouseUp;

            s.MouseMove += S_MouseMove;

            this.Controls.Add(s);

            this.Controls.SetChildIndex(s, 0);

            this.Controls.Add(b);

            v.BorderStyle = BorderStyle.None;
            b.BorderStyle = BorderStyle.None;

            b.BackColor = Color.FromKnownColor(KnownColor.Control);

            v.RetrieveVirtualItem += V_RetrieveVirtualItem;

            ResumeLayout();
            
            Init();

//            ClearLocalItems();
            
            HideHorizontalScrollBar();

//            SetTopMost();

            if (vp == null)
                return;

            symbols = vp.vs.GetProjectTypes(vp.FileName);

            LoadSymbols(ToList(symbols));

            S = null;
        }
        List<INamespaceOrTypeSymbol> symbols { get; set; }
        new public void AutoSize()
        {
            int h = form.Height;

            if (T == null || T.Count <= 0)
                return;

            ListViewItem b = T[0] as ListViewItem;

            int hh = 17;

            int itemHeight = hh;

            int number = 11;
            
           

            if (number > cc.Count)
                number = cc.Count;

            if (number > 11)
                number = 11;

            if (cc.Count > number)
                HandleScrollbarVisibility(true);
            else
                HandleScrollbarVisibility(false);

            int Height = number * itemHeight;

            v.Height = Height;

            this.Height = Height;

            form.Height = Height;

            

            
        }

        private void V_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            int i = e.ItemIndex;
            
            if (i < 0)
                return;
            if (cc.Count <= 0)
                return;
            if (i + _topitem >= cc.Count)
                e.Item = cc[cc.Count - 1] as ListViewItem;
            else
                e.Item = cc[i + _topitem] as ListViewItem;
        }

        //public ArrayList LI { get; set; }

        //public void ClearLocalItems()
        //{
        //    if (cc == null)
        //        cc = new List<ListViewItem>();

        //    CC = new ArrayList();

        //    cc.Clear();

           
        //}

        //public void SetTopMost()
        //{
        //    //       SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        //}

        private ArrayList T { get; set; }

        public void LoadTypes(VSParsers.CSParsers cs)
        {
            if (cs == null)
                return;

            T = new ArrayList();

//            ArrayList S = cs.GetAllTypes();


        }

        public List<Microsoft.CodeAnalysis.ISymbol> GetCodeCompletion(VSProject vp, string filename, string content, int offset, string name, string names)
        {
            return vp.vs.GetCodeCompletion(vp, filename, content,offset, name, names);
        }

        Dictionary<string, Microsoft.CodeAnalysis.INamedTypeSymbol> Dict { get; set; }

        List<Microsoft.CodeAnalysis.ISymbol> sns { get; set; }

        List<Microsoft.CodeAnalysis.ISymbol> ToList(List<INamespaceOrTypeSymbol> s)
        {
            List<Microsoft.CodeAnalysis.ISymbol> c = new List<Microsoft.CodeAnalysis.ISymbol>();
            foreach (INamespaceOrTypeSymbol d in s)
                c.Add(d);
            return c;

        }

        public List<Microsoft.CodeAnalysis.ISymbol> LoadProjectTypes(VSProject vp, string filename, string content, int offset, string name, string names)
        {
            List<Microsoft.CodeAnalysis.ISymbol> sn = GetCodeCompletion(vp, filename, content, offset, name, names);
            sn.Remove(sn.Find(x => x.Name == ".ctor"));
           
            if (name.StartsWith("("))
                return sn;
            v.Items.Clear();
           
                        
            LoadSymbols(sn);

            if (names.Contains("."))
                ReloadTypes(false);
            else ReloadTypes(true);

            sns = sn;

            return sn;
        }
        public List<Microsoft.CodeAnalysis.ISymbol> LoadFast(List<Microsoft.CodeAnalysis.ISymbol> sn)
        {
            v.Items.Clear();
            
            LoadSymbols(sn);

            ReloadTypes(false);

            sns = sn;

            return sn;
        }
        public Microsoft.CodeAnalysis.ISymbol GetSymbol(string data)
        {
            Microsoft.CodeAnalysis.ISymbol symbol = sns.Select(s => s).Where(t => t.Name == data).FirstOrDefault();

            return symbol;
        }

        public void FastLoader(VSProject vp)
        {
            List<Microsoft.CodeAnalysis.INamespaceOrTypeSymbol> sn = vp.vs.GetAllTypes(vp.FileName);
            sn.Remove(sn.Find(x => x.Name == ".ctor"));

          
            v.Items.Clear();

            LoadSymbols(ToList(sn));

            ReloadTypes(true);
            
            
        }
        ArrayList S { get; set; }

        ArrayList E { get; set; }

     void LoadSymbols(List<Microsoft.CodeAnalysis.ISymbol> sn, bool alls = false)
        {

            if (S != null)
                foreach (ListViewItem s in S)
                    T.Remove(s);
            
            S = new ArrayList();

            if (T == null)
                T = new ArrayList();

            if(E == null)
            {
                E = new ArrayList();
                E.Add(new ListViewItem("this"));
            }

            foreach (Microsoft.CodeAnalysis.ISymbol dd in sn)
            {

                if (dd != null)
                {
                    ListViewItem v;
                    int kind = 0;

                    if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Method)
                        kind = 1;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Property)
                        kind = 3;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Field)
                        kind = 2;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Event)
                        kind = 4;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Local)
                        kind = 2;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.NamedType)
                        kind = 0;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Parameter)
                        kind = 2;
                    else if (dd.Kind == Microsoft.CodeAnalysis.SymbolKind.Namespace)
                        kind = 7;
                    else
                        kind = 5;
                    v = new ListViewItem(dd.Name);
                    v.ImageIndex = kind;
                    T.Add(v);
                    S.Add(v);
                    
                }
          }
  
        }

        public void SetVirtualMode()
        {
            if (cc == null)
                return;

            v.VirtualMode = true;

            v.VirtualListSize = cc.Count;

            Y = cc.Count;
        }


        public void ReloadTypes(bool alls = true)
        {
 
            SuspendLayout();
            cc = new List<ListViewItem>();
            //T.Sort(new SortComparer());
            if (alls == true)
            {

                foreach (ListViewItem b in T)
                    cc.Add(b);
                int i = 0;
                
                
                foreach (ListViewItem c in E)
                  if (!cc.Contains(c))
                        cc.Add(c);
                cc.Sort(new SortComparer());
                while(i < cc.Count - 1)
                {
                    ListViewItem v0 = cc[i] as ListViewItem;
                    ListViewItem v1 = cc[i+1] as ListViewItem;
                    if (v0.Text == v1.Text)
                        cc.Remove(v0);
                    else i++;
                }

                i = 0;
                foreach (ListViewItem c in cc)
                    c.Tag = i++;

                //foreach (ListViewItem b in T)
                //{
                //    b.Tag = i++;
                //    cc.Add(b);

                //}
                //foreach (ListViewItem c in E)
                //{
                //    if (!cc.Contains(c))
                //        cc.Add(c);
                //    c.Tag = i++;
                //}
            }
            else
            {
                int i = 0;
                //cc.AddRange(S.ToArray());
                foreach (ListViewItem b in S)
                    cc.Add(b);
                foreach (ListViewItem c in E)
                    if (!cc.Contains(c))
                        cc.Add(c);
                cc.Sort(new SortComparer());
                foreach (ListViewItem c in cc)
                    c.Tag = i++;
                //int i = 0;
                //foreach (ListViewItem b in S)
                //{
                //    b.Tag = i++;
                //    cc.Add(b);
                //}
                //foreach (ListViewItem c in E)
                //{
                //    if (!cc.Contains(c))
                //        cc.Add(c);
                //    c.Tag = i++;
                //}
            }
            //foreach (ListViewItem c in E)
            //{
            //    if (!cc.Contains(c))
            //        cc.Add(c);
            //    c.Tag = cc.Count - 1;
            //}
            
            ResumeLayout();
        }

        public void SendKeys(int c)
        {
            

            if (this.Visible == false)
                return;

            if (v.SelectedIndices == null)
                return;

            if (v.Items.Count <= 0)
                return;

            int i = -2;

            if (v.SelectedIndices.Count > 0)

                i = v.SelectedIndices[0];

           
            bool up = false;

            if (c == (int)Keys.Down)
            {
                //if (i >= v.Items.Count - 1)
                if (i >= cc.Count - 1)
                    return;

                i++;
            }
            else
            {
                if(i == 0)
                if(cc.Count > 0)
                if( v.Items[0] == cc[0])
                    return;
                i--;
                up = true;
            }

            // if (i < 0)
            //     i = 0;

            if (i >= v.Items.Count)
                return;

            // v.Items[i].Selected = true;



            ScrollToPosition(up, i);
        }

        public void SetHandler()
        {
            if (cec == null)
                return;
            cec.KeyDown += Cec_KeyDown;
        }

        public void Cec_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                ListView.SelectedIndexCollection cs = v.SelectedIndices;

                if (cs == null)
                    return;
                if (cs.Count <= 0)
                    return;

                int i = cs[0];

                ListViewItem b = v.Items[i];

                string text = b.Text;

                if (cec == null)
                    return;

                cec.InsertText(text, offset + 1);

                this.form.Hide();
                this.Hide();
            }
        }

        private void Cec_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Up)
            {
            }
            else if (e.KeyChar == (char)Keys.Down)
            {
            }
        }

        private void V_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (v.SelectedItems == null)
                    return;
                if (v.SelectedItems.Count <= 0)
                    return;
                ListViewItem b = v.SelectedItems[0];

                string text = b.Text;

                if (cec == null)
                    return;

                cec.InsertText(text, offset);
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (v.SelectedItems == null)
                    return;
                if (v.SelectedItems.Count <= 0)
                    return;
                ListViewItem b = v.SelectedItems[0];

                string text = b.Text;

                if (cec == null)
                    return;

                cec.InsertText(text, offset);
            }
        }

        public void HandleScrollbarVisibility(bool visible)
        {
            du.Visible = visible;
            su.Visible = visible;
            s.Visible = visible;

        }

        public int offset = 0;

        public bool afterdot = false;

        public ArrayList CC { get; set; }

        public void FindSimilarWords(string word)
        {
            if (word == "")
                return;

            word = word.ToLower();

            ListViewItem b = null;
            if (word.Length <= 1)
                b = cc.ToArray().FirstOrDefault(x => ((ListViewItem)x).Text.StartsWith(word, StringComparison.InvariantCultureIgnoreCase)) as ListViewItem;
            else
            {
                List<ListViewItem> cs = cc.ToArray().Select(s => s).Where(x => ((ListViewItem)x).Text.ToLower().Contains(word) == true).ToList();
                if (cs.Count <= 0)
                    return;
                cc = cs;
                //cc = cc.ToArray().Select(s => s).Where(x => ((ListViewItem)x).Text.ToLower().Contains(word) == true).ToList();
                int g = 0;
                foreach (ListViewItem c in cc)
                    c.Tag = g++;
                
            }
            Y = cc.Count;
            int last = 13;
            
            int i = 0;

            if (b != null)
                i = (int)b.Tag;
            if (b == null)
                if(cc.Count > 0)
                b = cc[0] as ListViewItem;
 
            if (cc.Count - 1 < last)
                last = cc.Count - 1;
            if (last < 0)
                return;

            _topitem = i;
            if(b != null)
            v.TopItem = b;
            v.RedrawItems(0, last, false);
            return;

            //int i = 0;
            //while(i < cc.Count)
            //{
            //    ListViewItem c = cc[i] as ListViewItem;
            //    if (c.Text.ToLower().StartsWith(word))
            //    {
            //        _topitem = i;
            //        v.TopItem = c;
            //        v.RedrawItems(0, 17, false);
            //        return;
            //    }
            //    i++;
            //}

           
            
            
        }

        public void Find(string w, int offsets, bool hasdots)
        {
            offset = offsets;

            if (w == ".")
            {
                afterdot = true;
                SetVirtualMode();
                return;
            }

            ArrayList L = new ArrayList();

            //if (cc == null)



            //if (w.Length > 2)
            {
                cc = new List<ListViewItem>();

                _topitem = 0;

                for (int i = 0; i < CC.Count; i++)
                {
                    ListViewItem c = CC[i] as ListViewItem;
                    if (c.Text.StartsWith(w))
                        if (hasdots == false)
                            cc.Add(c);
                        else if (w.Length <= 2)
                            // if(hasdots == false)
                            cc.Add(c);
                }
            }
            //v.Items.Clear();

            //foreach (ListViewItem c in L)
            //{
            //    v.Items.Add(c);
            //}

            SetVirtualMode();
        }

        private void HideHorizontalScrollBar()
        {
            ShowScrollBar(v.Handle.ToInt64(), _SB_VERT, 0);
            ShowScrollBar(v.Handle.ToInt64(), _SB_HORZ, 0);
        }


        private int _topitem = 0;


        private void S_MouseMove(object sender, MouseEventArgs e)
        {
            if (_state == 0)
                return;
            int y = s.Location.Y;

            int h = b.Height;

            int r = s.Location.Y + e.Y;///*s.Location*/ = new Point(s.Location.X, s.Location.Y + e.Y);

            if (s.Location.Y <= 15 || s.Location.Y >= h - 14 - 15 + 5)
            {
                {
                    if (s.Location.Y <= 15)
                    {
                        s.Location = new Point(s.Location.X, 15);
                        if (r <= 15)
                        {
                            s.Location = new Point(s.Location.X, 15);
                            s.Refresh();
                            su.Refresh();
                            return;
                        }
                        s.Location = new Point(s.Location.X, r);
                        //s.Location = new Point(s.Location.X, 15);
                        s.Refresh();
                        su.Refresh();
                        //  return;
                    }
                }
                //else s.Location = new Point(s.Location.X, r);

                if (s.Location.Y >= h - 14 - 15 + 5)
                {
                    s.Location = new Point(s.Location.X, h - 14 - 15 + 5);
                    if (r >= h - 14 - 15 + 5)
                    {
                        s.Location = new Point(s.Location.X, h - 14 - 15 + 5);
                        s.Refresh();
                        du.Refresh();
                        return;
                    }
                    s.Location = new Point(s.Location.X, r);
                    //s.Location = new Point(s.Location.X, 15);
                    s.Refresh();
                    du.Refresh();
                }
            }
            else s.Location = new Point(s.Location.X, r);
            
            int p = (int)((double)Y * ((double)s.Location.Y - 15) / (double)(b.Height - s.Height - 15 - 15 + 1));

            if (p >= Y)
                p = Y - 1;
            if (p < 0)
                p = 0;


            v.SuspendLayout();

            //if (v.Items.Count <= 0)
            //    return;
            if (cc.Count <= 0)
                return;
            int itemHeight = v.GetItemRect(0).Height;

            int listHeight = v.Height;

            int nmb = (int)((double)listHeight / (double)itemHeight);
            //if (p >= v.Items.Count - nmb)
            //    p = v.Items.Count - nmb;
            if (p >= cc.Count - nmb)
                p = cc.Count - nmb;
            _topitem = p;

            v.TopItem = cc[p];// cc[0];// v.Items[0];

            v.RedrawItems(0, nmb, false);

            v.ResumeLayout();
        }

        public void ScrollToPosition(bool up, int select)
        {
            v.SuspendLayout();

            int itemHeight = v.GetItemRect(0).Height;

            int listHeight = v.Height;

            int nmb = (int)((double)listHeight / (double)itemHeight);

            if (select >= nmb || select <= 0)
            {
                if (up == true)
                    _topitem--;
                else if (select > -1)
                {
                    //if (_topitem >= v.Items.Count - nmb)
                    //{
                    //    _topitem = v.Items.Count - nmb;
                    //    return;
                    //}
                    if (_topitem >= cc.Count - nmb)
                    {
                        _topitem = cc.Count - nmb;
                        return;
                    }
                    _topitem++;

                }
            
                if (_topitem < 0)
                    _topitem = 0;
            }
            bool haschanged = false;
            if (select < 0)
            {
                haschanged = true;
                select = 0;
            }

            v.Items[select].Selected = true;

            v.TopItem = v.Items[0];

            if (nmb >= v.Items.Count)
                nmb = v.Items.Count - 1;

            v.RedrawItems(0, nmb, false);

            if (v.SelectedIndices != null)
                if (v.SelectedIndices.Count > 0)
                {
                    int s = v.SelectedIndices[0];

                    //if (nmb == s)
                    //    if (nmb != v.Items.Count - 1)
                    //    {
                    //        if (s > 0)
                    //        {
                    //            v.Items[s - 1].Selected = true;
                    //            v.Items[s].Selected = false;
                    //        }
                    //    }
                    if (nmb == s)
                    if (nmb != cc.Count - 1)
                    {
                        if (s > 0)
                        {
                            v.Items[s - 1].Selected = true;
                            v.Items[s].Selected = false;
                        }
                    }
                    if (nmb < s)
                    {
                        if (s > 0)
                        {
                            v.Items[s - 1].Selected = true;
                            v.Items[s].Selected = false;
                        }
                    }
                }

            if (haschanged == true)
                ScrollTo(select);

            v.ResumeLayout();
        }


        private void S_MouseUp(object sender, MouseEventArgs e)
        {
            _state = 0;
            cec.Focus();
        }

        private void S_MouseDown(object sender, MouseEventArgs e)
        {
            _state = 1;
        }

        private TextBox b { get; set; }
        public ListView v { get; set; }
        private Button s { get; set; }
        private Button su { get; set; }
        private Button du { get; set; }

        private int _state = 0;

        public void Init()
        {
            img = new ImageList();

            img.Images.Add("class", resource._class);
            img.Images.Add("method", resource.methods);
            img.Images.Add("field", resource.field);
            img.Images.Add("property", resource.property);
            img.Images.Add("event", resource._event);
            img.Images.Add("list", resource.keywords);
            img.Images.Add("enum", resource._event);
            img.Images.Add("namespace", resource._namespace);


            v.SmallImageList = img;

            v.View = View.Details;

            v.MultiSelect = false;

            v.Scrollable = false;

            v.Columns.Clear();

            v.Columns.Add("");

            v.HeaderStyle = ColumnHeaderStyle.None;

            v.HideSelection = false;

            v.Resize += Cb_Resize;

            v.SelectedIndexChanged += V_SelectedIndexChanged;
        }

        private void V_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (v.SelectedIndices == null)
                return;
            if (v.SelectedIndices.Count <= 0)
                return;
            int i = v.SelectedIndices[0];

            ScrollTo(i);
            if(_state == 0)
            cec.Focus();
        }

        public void ScrollTo(int i)
        {
            v.EnsureVisible(i);

            int y = s.Location.Y;

            int h = b.Height;

            int c = h - 14 - 14 - 10;

            int ccc = cc.Count;// v.Items.Count;

            int p = (int)(14 + (((double)i + _topitem) / (double)ccc) * (double)c);

            s.Location = new Point(s.Location.X, p);
        }

        public int Y { get; set; }

        public void resize(int x, int y)
        {
            if (v.Items.Count <= 0)
            {
                form.Hide();
                this.Hide();
                return;
            }
            else
            {
                // form.Show();
                // this.Show();
            }

            int itemHeight = v.GetItemRect(0).Height;

            int itemWidth = 0;

            int c = v.Items[0].GetBounds(ItemBoundsPortion.Icon).Width;

            int i = 0;
            while (i < v.Items.Count)
            {
                ListViewItem b = v.Items[i];



                Size d = TextRenderer.MeasureText(b.Text, v.Font);

                if (itemWidth < d.Width)
                    itemWidth = d.Width;
                i++;
            }
            int h = v.Items.Count;

            int w = itemWidth + c + 10;

            if (h * itemHeight < y)
            {
                y = h * itemHeight;

                s.Hide();
                su.Hide();
                du.Hide();
                b.Hide();
            }
            else
            {
                s.Show();
                su.Show();
                du.Show();
                b.Show();
            }

            if (w < 180)
                w = 180;

            v.Width = w - 20;



            this.Size = new Size(w, y);

            HideHorizontalScrollBar();

            form.Size = new Size(w, y);

            v.Width = w - 20;




            form.Show();

            this.Show();

            HideHorizontalScrollBar();

        
            v.Refresh();

        
        }

        private void Cb_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
        
        }

        private void Cb_Resize(object sender, EventArgs e)
        {
            Size s = v.Size;
            v.Columns[0].Width = v.Width - 5;
            HideHorizontalScrollBar();
          
        }

        
        public Color BorderColor { get; set; }
    }

    public class Proxy
    {
        public event EventHandler<OpenFileEventArgs> OpenFile;


        public void LoadFile(string content, VSProject vp, string name)
        {
            OpenFileEventArgs args = new OpenFileEventArgs();
            args.Threshold = 10;
            args.filename = name;
            args.content = content;
            args.vp = vp;

            OpenNewFile(args);
        }
        public void LoadProjectFile(string content, VSProject vp, string name, DomRegion dr)
        {
            OpenFileEventArgs args = new OpenFileEventArgs();
            args.Threshold = 10;
            args.filename = name;
            args.content = content;
            args.vp = vp;
            args.dr = dr;
            OpenNewFile(args);
            
        }

        public bool IsBbound()
        {
            EventHandler<OpenFileEventArgs> handler = OpenFile;
            if (handler != null)
            {
                return true;
            }
            else return false;
        }

        public void OpenNewFile(OpenFileEventArgs e)
        {
            EventHandler<OpenFileEventArgs> handler = OpenFile;
            if (handler != null)
            {
                handler(this, e);
            }
            
        }


        public class OpenFileEventArgs : EventArgs
        {
            public int Threshold { get; set; }
            public string filename { get; set; }
            public string content { get; set; }
            public VSProject vp { get; set; }
            public DomRegion dr { get; set; }
        }
    }

   

}