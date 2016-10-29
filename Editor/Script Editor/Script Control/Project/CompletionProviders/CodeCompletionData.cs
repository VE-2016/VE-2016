using AIMS.Libraries.CodeEditor.Syntax;
using AIMS.Libraries.CodeEditor.WinForms;
using AIMS.Libraries.CodeEditor.WinForms.CompletionWindow;
using AIMS.Libraries.Scripting.Dom;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace AIMS.Libraries.Scripting.ScriptControl.CodeCompletion

{
    public class CodeCompletionData : ICompletionData
    {
        private IAmbience _ambience;
        private AutoListIcons _imageIndex;
        private int _overloads;
        private string _text;
        private string _description;
        private string _documentation;
        private IClass _c;
        private IMember _member;
        private bool _convertedDocumentation = false;
        private double _priority;

        /// <summary>
        /// Gets the class this CodeCompletionData object was created for.
        /// Returns null if the CodeCompletionData object was created for a method/property etc.
        /// </summary>

        public IClass Class
        {
            get
            {
                return _c;
            }
        }

        /// <summary>
        /// Gets the member this CodeCompletionData object was created for.
        /// Returns null if the CodeCompletionData object was created for a class or namespace.
        /// </summary>
        public IMember Member
        {
            get
            {
                return _member;
            }
        }

        public int Overloads
        {
            get
            {
                return _overloads;
            }
            set
            {
                _overloads = value;
            }
        }

        public double Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

        public AutoListIcons ImageIndex
        {
            get
            {
                return _imageIndex;
            }
            set
            {
                _imageIndex = value;
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public string Description
        {
            get
            {
                // don't give a description string, if no documentation or description is provided
                if (_description.Length == 0 && (_documentation == null || _documentation.Length == 0))
                {
                    return "";
                }
                if (!_convertedDocumentation && _documentation != null)
                {
                    _convertedDocumentation = true;
                    _documentation = GetDocumentation(_documentation);
                }

                return _description + (_overloads > 0 ? " " + "Overloads " + _overloads.ToString() : String.Empty) + "\n" + _documentation;
            }
            set
            {
                _description = value;
            }
        }

        private string _dotnetName;

        private void GetPriority(string dotnetName)
        {
            _dotnetName = dotnetName;
            _priority = CodeCompletionDataUsageCache.GetPriority(dotnetName, true);
        }

        public CodeCompletionData(string s, string desc, AutoListIcons imageIndex)
        {
            _ambience = Parser.ProjectParser.CurrentAmbience;
            _description = desc;
            _documentation = string.Empty;
            _text = s;
            _imageIndex = imageIndex;
            GetPriority(s);
        }

        public CodeCompletionData(IClass c)
        {
            _ambience = Parser.ProjectParser.CurrentAmbience;
            // save class (for the delegate description shortcut)
            _c = c;
            _imageIndex = ScriptControl.GetIcon(c);
            _ambience.ConversionFlags = ConversionFlags.None;
            _text = _ambience.Convert(c);
            _ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedNames | ConversionFlags.ShowReturnType | ConversionFlags.ShowModifiers;
            _description = _ambience.Convert(c);
            _documentation = c.Documentation;
            GetPriority(c.DotNetName);
        }

        public CodeCompletionData(IMethod method)
        {
            _member = method;
            _ambience = Parser.ProjectParser.CurrentAmbience;
            _ambience.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers;
            _imageIndex = ScriptControl.GetIcon(method);
            _text = method.Name;
            _description = _ambience.Convert(method);
            _documentation = method.Documentation;
            GetPriority(method.DotNetName);
        }

        public CodeCompletionData(IField field)
        {
            _member = field;
            _ambience = Parser.ProjectParser.CurrentAmbience;
            _ambience.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers;
            _imageIndex = ScriptControl.GetIcon(field);
            _text = field.Name;
            _description = _ambience.Convert(field);
            _documentation = field.Documentation;
            GetPriority(field.DotNetName);
        }

        public CodeCompletionData(IProperty property)
        {
            _member = property;
            _ambience = Parser.ProjectParser.CurrentAmbience;
            _ambience.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers;
            _imageIndex = ScriptControl.GetIcon(property);
            _text = property.Name;
            _description = _ambience.Convert(property);
            _documentation = property.Documentation;
            GetPriority(property.DotNetName);
        }

        public CodeCompletionData(IEvent e)
        {
            _member = e;
            _ambience = Parser.ProjectParser.CurrentAmbience;
            _ambience.ConversionFlags = ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterNames | ConversionFlags.ShowModifiers;
            _imageIndex = ScriptControl.GetIcon(e);
            _text = e.Name;
            _description = _ambience.Convert(e);
            _documentation = e.Documentation;
            GetPriority(e.DotNetName);
        }

        public bool InsertAction(EditViewControl textArea, char ch)
        {
            if (_dotnetName != null)
            {
                CodeCompletionDataUsageCache.IncrementUsage(_dotnetName);
            }
            if (_c != null && _text.Length > _c.Name.Length)
            {
                textArea.InsertText(_text.Substring(0, _c.Name.Length + 1));
                TextPoint start = textArea.Caret.Position;
                TextPoint end;
                int pos = _text.IndexOf(',');
                if (pos < 0)
                {
                    textArea.InsertText(_text.Substring(_c.Name.Length + 1));
                    end = textArea.Caret.Position;
                    end.X -= 1;
                }
                else
                {
                    textArea.InsertText(_text.Substring(_c.Name.Length + 1, pos - _c.Name.Length - 1));
                    end = textArea.Caret.Position;
                    textArea.InsertText(_text.Substring(pos));
                }
                textArea.Caret.Position = start;
                textArea.Selection.SelStart = start.X;
                textArea.Selection.SelEnd = end.X;
                //textArea.Selection.se.SelectionManager.SetSelection(start, end);
                if (!char.IsLetterOrDigit(ch))
                {
                    return true;
                }
            }
            else
            {
                textArea.InsertText(_text);
            }
            return false;
        }

        internal static Regex whitespace = new Regex(@"\s+");

        /// <summary>
        /// Converts the xml documentation string into a plain text string.
        /// </summary>
        public static string GetDocumentation(string doc)
        {
            System.IO.StringReader reader = new System.IO.StringReader("<docroot>" + doc + "</docroot>");
            XmlTextReader xml = new XmlTextReader(reader);
            StringBuilder ret = new StringBuilder();
            ////Regex whitespace    = new Regex(@"\s+");

            try
            {
                xml.Read();
                do
                {
                    if (xml.NodeType == XmlNodeType.Element)
                    {
                        string elname = xml.Name.ToLowerInvariant();
                        switch (elname)
                        {
                            case "filterpriority":
                                xml.Skip();
                                break;

                            case "remarks":
                                ret.Append(Environment.NewLine);
                                ret.Append("Remarks:");
                                ret.Append(Environment.NewLine);
                                break;

                            case "example":
                                ret.Append(Environment.NewLine);
                                ret.Append("Example:");
                                ret.Append(Environment.NewLine);
                                break;

                            case "exception":
                                ret.Append(Environment.NewLine);
                                ret.Append(GetCref(xml["cref"]));
                                ret.Append(": ");
                                break;

                            case "returns":
                                ret.Append(Environment.NewLine);
                                ret.Append("Returns: ");
                                break;

                            case "see":
                                ret.Append(GetCref(xml["cref"]));
                                ret.Append(xml["langword"]);
                                break;

                            case "seealso":
                                ret.Append(Environment.NewLine);
                                ret.Append("See also: ");
                                ret.Append(GetCref(xml["cref"]));
                                break;

                            case "paramref":
                                ret.Append(xml["name"]);
                                break;

                            case "param":
                                ret.Append(Environment.NewLine);
                                ret.Append(whitespace.Replace(xml["name"].Trim(), " "));
                                ret.Append(": ");
                                break;

                            case "value":
                                ret.Append(Environment.NewLine);
                                ret.Append("Value: ");
                                ret.Append(Environment.NewLine);
                                break;

                            case "br":
                            case "para":
                                ret.Append(Environment.NewLine);
                                break;
                        }
                    }
                    else if (xml.NodeType == XmlNodeType.Text)
                    {
                        ret.Append(whitespace.Replace(xml.Value, " "));
                    }
                } while (xml.Read());
            }
            catch
            {
                //LoggingService.Debug("Invalid XML documentation: " + ex.Message);
                return doc;
            }
            return ret.ToString();
        }

        private static string GetCref(string cref)
        {
            if (cref == null || cref.Trim().Length == 0)
            {
                return "";
            }
            if (cref.Length < 2)
            {
                return cref;
            }
            if (cref.Substring(1, 1) == ":")
            {
                return cref.Substring(2, cref.Length - 2);
            }
            return cref;
        }

        #region System.IComparable interface implementation

        public int CompareTo(object obj)
        {
            if (obj == null || !(obj is CodeCompletionData))
            {
                return -1;
            }
            return _text.CompareTo(((CodeCompletionData)obj)._text);
        }

        #endregion System.IComparable interface implementation
    }
}