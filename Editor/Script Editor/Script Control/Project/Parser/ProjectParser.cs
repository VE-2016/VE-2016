using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.CodeCompletion;
using AIMS.Libraries.Scripting.Dom.CSharp;
using AIMS.Libraries.Scripting.Dom.VBNet;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
    internal class ProjectParser
    {
        private static IProjectContent s_projectContent = null;
        private static ProjectContentRegistry s_projectContentRegistry = null;
        private static volatile Dictionary<string, ProjectContentItem> s_projContentInfo = null;

        private static Object s_obs = new object();

        private static string s_domPersistencePath;
        private static string s_projectPath;
        private static SupportedLanguage s_language;
        private static NRefactory.Parser.Errors s_lastParserError = null;
        static public void Initilize(SupportedLanguage lang)
        {
            s_language = lang;
            s_projContentInfo = new Dictionary<string, ProjectContentItem>();
            s_projectPath = AppDomain.CurrentDomain.BaseDirectory;
            s_projectContentRegistry = new ProjectContentRegistry();
            s_domPersistencePath = Path.Combine(Path.GetTempPath(), "AIMSDomCache");
            Directory.CreateDirectory(s_domPersistencePath);
            s_projectContentRegistry.ActivatePersistence(s_domPersistencePath);
            s_projectContent = new DefaultProjectContent();
            s_projectContent.ReferencedContents.Add(s_projectContentRegistry.Mscorlib);
        }


        public static string DomPersistencePath
        {
            get
            {
                return s_domPersistencePath;
            }
        }

        public static string ProjectPath
        {
            get
            {
                return s_projectPath;
            }
        }

        public static IProjectContent CurrentProjectContent
        {
            get { return s_projectContent; }
        }

        public static ProjectContentRegistry ProjectContentRegistry
        {
            get
            {
                return s_projectContentRegistry;
            }
        }

        public static SupportedLanguage Language
        {
            get { return s_language; }
            set
            {
                ConvertToLanguage(s_language, value);
                s_language = value;
            }
        }

        private static void ConvertToLanguage(SupportedLanguage OldLang, SupportedLanguage NewLang)
        {
            Dictionary<string, ProjectContentItem> projInfo = new Dictionary<string, ProjectContentItem>();
            foreach (ProjectContentItem pc in s_projContentInfo.Values)
            {
                string fileName = pc.FileName;
                ClearParseInformation(fileName); //Remove last unit from project
                pc.FileName = Path.GetFileNameWithoutExtension(fileName) + (NewLang == SupportedLanguage.CSharp ? ".cs" : ".vb");
                //Change Contents
                pc.Contents = Converter.CodeConverter.ConvertCode(pc.Contents, (OldLang == SupportedLanguage.CSharp ? ScriptLanguage.CSharp : ScriptLanguage.VBNET), (NewLang == SupportedLanguage.CSharp ? ScriptLanguage.CSharp : ScriptLanguage.VBNET));
                projInfo.Add(pc.FileName, pc);
            }
            s_language = NewLang; //Set New Language
            s_projContentInfo = projInfo; //Reset new proj Contents
            foreach (ProjectContentItem pc in s_projContentInfo.Values)
            {
                //Now parse
                ParseProjectContents(pc.FileName, pc.Contents, pc.IsOpened);
            }
        }
        public static IParser GetParser(string fileName)
        {
            if (Path.GetExtension(fileName).ToLower().Trim() == ".cs")
                return new CSharpParser();
            else
                return new VbParser();
        }

        public static IResolver CreateResolver(string fileName)
        {
            IParser parser = GetParser(fileName);
            if (parser != null)
            {
                return parser.CreateResolver();
            }
            return null;
        }

        public static ResolveResult Resolve(ExpressionResult expressionResult,
                                            int caretLineNumber,
                                            int caretColumn,
                                            string fileName,
                                            string fileContent)
        {
            IResolver resolver = CreateResolver(fileName);
            if (resolver != null)
            {
                return resolver.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, fileContent);
            }
            return null;
        }

        public static string GetFileContents(string fileName)
        {
            if (s_projContentInfo.ContainsKey(fileName))
            {
                return s_projContentInfo[fileName].Contents;
            }
            else
                return string.Empty;
        }

        public static Dom.NRefactoryResolver.NRefactoryResolver GetResolver()
        {
            Dom.NRefactoryResolver.NRefactoryResolver resolver = new Dom.NRefactoryResolver.NRefactoryResolver(s_projectContent, (s_language == SupportedLanguage.CSharp ? LanguageProperties.CSharp : LanguageProperties.VBNet));
            return resolver;
        }

        public static Dictionary<string, ProjectContentItem> ProjectFiles
        {
            get { return s_projContentInfo; }
        }

        public static void RemoveContentFile(string fileName)
        {
            if (s_projContentInfo.ContainsKey(fileName))
            {
                ClearParseInformation(fileName);
                s_projContentInfo.Remove(fileName);
            }
        }

        public static ParseInformation ParseProjectContents(string fileName, string Content)
        {
            return Parser.ProjectParser.ParseProjectContents(fileName, Content, false);
        }

        public static ParseInformation ParseProjectContents(string fileName, string Content, bool IsOpened)
        {
            //try
            //   {

            lock (s_obs)
            {
                if (s_projContentInfo == null)
                    s_projContentInfo = new Dictionary<string, ProjectContentItem>();

                if (s_projContentInfo.ContainsKey(fileName) == false)
                {
                    s_projContentInfo[fileName] = new ProjectContentItem(fileName, Content, IsOpened);
                }

                s_projContentInfo[fileName].Contents = Content;

                IParser parser = GetParser(fileName);
                if (parser == null)
                {
                    MessageBox.Show("Parser errors..");
                    return null;
                }

                ICompilationUnit parserOutput = null;
                parserOutput = parser.Parse(s_projectContent, fileName, Content);

                if (parserOutput == null)
                {
                    MessageBox.Show("Parser errors..");
                    return null;
                }

                s_lastParserError = parser.LastErrors;

                if (s_projContentInfo.ContainsKey(fileName))
                {
                    ParseInformation parseInformation = s_projContentInfo[fileName].ParsedContents;
                    if (parseInformation == null)
                    {
                        parseInformation = new ParseInformation();
                        s_projContentInfo[fileName].ParsedContents = parseInformation;
                    }
                    s_projectContent.UpdateCompilationUnit(parseInformation.MostRecentCompilationUnit, parserOutput, fileName);
                }
                else
                {
                    s_projectContent.UpdateCompilationUnit(null, parserOutput, fileName);
                }



                return UpdateParseInformation(parserOutput, fileName);

                //   }
                //}
                //catch (Exception ex)
                //{

                //}
                //finally
                //{


                //}
                //return null;
            }
        }

        public static NRefactory.Parser.Errors LastParserErrors
        {
            get { return s_lastParserError; }
        }

        public static ParseInformation GetParseInformation(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
            {
                return null;
            }
            if (!s_projContentInfo.ContainsKey(fileName))
            {
                return ParseProjectContents(fileName, s_projContentInfo[fileName].Contents);
            }
            return s_projContentInfo[fileName].ParsedContents;
        }

        public static void ClearParseInformation(string fileName)
        {
            if (fileName == null || fileName.Length == 0)
            {
                return;
            }
            if (s_projContentInfo.ContainsKey(fileName))
            {
                ParseInformation parseInfo = s_projContentInfo[fileName].ParsedContents;
                if (parseInfo != null && parseInfo.MostRecentCompilationUnit != null)
                {
                    parseInfo.MostRecentCompilationUnit.ProjectContent.RemoveCompilationUnit(parseInfo.MostRecentCompilationUnit);
                }
                s_projContentInfo[fileName].ParsedContents = null;
            }
        }

        public static ParseInformation UpdateParseInformation(ICompilationUnit parserOutput, string fileName)
        {
            ParseInformation parseInformation = s_projContentInfo[fileName].ParsedContents;

            if (parserOutput.ErrorsDuringCompile)
            {
                parseInformation.DirtyCompilationUnit = parserOutput;
            }
            else
            {
                parseInformation.ValidCompilationUnit = parserOutput;
                parseInformation.DirtyCompilationUnit = null;
            }
            s_projContentInfo[fileName].ParsedContents = parseInformation;
            return parseInformation;
        }

        public static AmbienceReflectionDecorator CurrentAmbience
        {
            get
            {
                IAmbience defAmbience = null;
                if (s_language == SupportedLanguage.CSharp)
                    defAmbience = new CSharpAmbience();
                else
                    defAmbience = new VBNetAmbience();

                return new AmbienceReflectionDecorator(defAmbience);
            }
        }
    }
}
