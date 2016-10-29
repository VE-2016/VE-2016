using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using AIMS.Libraries.CodeEditor.WinForms;
using System.Reflection;
using AIMS.Libraries.CodeEditor.Syntax;

namespace AIMS.Libraries.CodeEditor.SyntaxFiles
{
    public sealed class CodeEditorSyntaxLoader
    {
        private static LanguageList s_langList = null;

        private static string GetSyntaxFileName(SyntaxLanguage lang)
        {
            string file = Enum.GetName(typeof(SyntaxLanguage), lang);

            if (file == "RESX")
                file = "XML";
            file += ".syn";

            return "AIMS.Libraries.CodeEditor.SyntaxFiles.Syns." + file;
        }

        public static Language SetSyntax(CodeEditorControl editor, SyntaxLanguage language)
        {
            Stream xml = GetSyntaxStream(GetSyntaxFileName(language));
            Language lg = Language.FromSyntaxFile(xml);
            editor.Document.Parser.Init(lg);
            Stream xmls = GetSyntaxStream(GetSyntaxFileName(language));
            //Language lgs = Language.FromSyntaxFile(xmls);
            //editor.Document.Parser.Init(lg);

            return lg;
        }

        private static Stream GetSyntaxStream(string file)
        {
            Stream strm = typeof(CodeEditorSyntaxLoader).Assembly.GetManifestResourceStream(file);
            return strm;
        }

        public static void SetSyntax(CodeEditorControl editor, string filename)
        {
            editor.Document.Parser.Init(CodeEditorSyntaxLoader.LanguageList.GetLanguageFromFile(filename));
        }

        public static Language GetLanguageFrom(SyntaxLanguage syntax)
        {
            Stream xml = GetSyntaxStream(GetSyntaxFileName(syntax));

            return Language.FromSyntaxFile(xml);
        }

        public static LanguageList LanguageList
        {
            get
            {
                if (s_langList == null)
                {
                    s_langList = new LanguageList();

                    SyntaxLanguage[] languages = (SyntaxLanguage[])Enum.GetValues(typeof(SyntaxLanguage));

                    foreach (SyntaxLanguage current in languages)
                    {
                        Stream strm = GetSyntaxStream(GetSyntaxFileName(current));

                        s_langList.Add(Language.FromSyntaxFile(strm));
                    }
                }

                return s_langList;
            }
        }
    }

    public enum SyntaxLanguage
    {
        Lang6502,
        //ASP,
        CPP,
        Cobol,
        CSharp,
        CSS,
        DataFlex,
        Delphi,
        DOSBatch,
        Fortran90,
        // FoxPro,
        Java,
        JavaScript,
        JSP,
        LotusScript,
        MSIL,
        MySql_SQL,
        NPath,
        Oracle_SQL,
        Perl,
        PHP,
        Povray,
        Python,
        Rtf,
        SmallTalk,
        SqlServer2K5,
        SqlServer2K,
        SqlServer7,
        SystemPolicies,
        Template,
        Text,
        TurboPascal,
        VBNET,
        VB,
        VBScript,
        VRML97,
        RESX,
        XML,
        HTML,
        Nemerle,
        Gemix,
        AutoIt
    }
}
