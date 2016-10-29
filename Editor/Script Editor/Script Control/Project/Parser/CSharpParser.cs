using System;
using System.Collections.Generic;
using System.IO;
using AIMS.Libraries.Scripting.Dom;
using AIMS.Libraries.Scripting.NRefactory;
using AIMS.Libraries.Scripting.ScriptControl.Project;
using AIMS.Libraries.Scripting.Dom.CSharp;

namespace AIMS.Libraries.Scripting.ScriptControl.Parser
{
    internal class CSharpParser : IParser
    {
        ///<summary>IParser Interface</summary>
        private string[] _lexerTags;
        private NRefactory.Parser.Errors _lastErrors = null;
        public string[] LexerTags
        {
            get
            {
                return _lexerTags;
            }
            set
            {
                _lexerTags = value;
            }
        }

        public LanguageProperties Language
        {
            get
            {
                return LanguageProperties.CSharp;
            }
        }

        public IExpressionFinder CreateExpressionFinder(string fileName)
        {
            return new CSharpExpressionFinder(fileName);
        }

        public bool CanParse(string fileName)
        {
            return Path.GetExtension(fileName).Equals(".CS", StringComparison.OrdinalIgnoreCase);
        }

        public bool CanParse(IProject project)
        {
            return true;
        }

        private void RetrieveRegions(ICompilationUnit cu, NRefactory.Parser.SpecialTracker tracker)
        {
            for (int i = 0; i < tracker.CurrentSpecials.Count; ++i)
            {
                NRefactory.PreprocessingDirective directive = tracker.CurrentSpecials[i] as NRefactory.PreprocessingDirective;
                if (directive != null)
                {
                    if (directive.Cmd == "#region")
                    {
                        int deep = 1;
                        for (int j = i + 1; j < tracker.CurrentSpecials.Count; ++j)
                        {
                            NRefactory.PreprocessingDirective nextDirective = tracker.CurrentSpecials[j] as NRefactory.PreprocessingDirective;
                            if (nextDirective != null)
                            {
                                switch (nextDirective.Cmd)
                                {
                                    case "#region":
                                        ++deep;
                                        break;
                                    case "#endregion":
                                        --deep;
                                        if (deep == 0)
                                        {
                                            cu.FoldingRegions.Add(new FoldingRegion(directive.Arg.Trim(), new DomRegion(directive.StartPosition, nextDirective.EndPosition)));
                                            goto end;
                                        }
                                        break;
                                }
                            }
                        }
                    end:;
                    }
                }
            }
        }

        public object obs = new object();

        public ICompilationUnit Parse(IProjectContent projectContent, string fileName, string fileContent)
        {
            //try
            {
                NRefactory.IParser p = null;

                ICompilationUnit cu = null;

                lock (obs)
                {
                    //using (NRefactory.IParser p = NRefactory.ParserFactory.CreateParser(NRefactory.SupportedLanguage.CSharp, new StringReader(fileContent)))

                    p = NRefactory.ParserFactory.CreateParser(NRefactory.SupportedLanguage.CSharp, new StringReader(fileContent));
                    {
                        cu = Parse(p, fileName, projectContent);
                    }
                }

                return cu;
            }
            //catch (Exception ex)
            //{


            //}

            //finally
            //{


            //}
            //return null;
        }

        private ICompilationUnit Parse(NRefactory.IParser p, string fileName, IProjectContent projectContent)
        {
            p.Lexer.SpecialCommentTags = _lexerTags;
            p.ParseMethodBodies = true;
            p.Parse();
            _lastErrors = p.Errors;
            Dom.NRefactoryResolver.NRefactoryASTConvertVisitor visitor = new Dom.NRefactoryResolver.NRefactoryASTConvertVisitor(projectContent);
            visitor.Specials = p.Lexer.SpecialTracker.CurrentSpecials;
            visitor.VisitCompilationUnit(p.CompilationUnit, null);
            visitor.Cu.FileName = fileName;
            visitor.Cu.ErrorsDuringCompile = p.Errors.Count > 0;
            RetrieveRegions(visitor.Cu, p.Lexer.SpecialTracker);
            AddCommentTags(visitor.Cu, p.Lexer.TagComments);
            return visitor.Cu;
        }

        private void AddCommentTags(ICompilationUnit cu, System.Collections.Generic.List<NRefactory.Parser.TagComment> tagComments)
        {
            foreach (NRefactory.Parser.TagComment tagComment in tagComments)
            {
                DomRegion tagRegion = new DomRegion(tagComment.StartPosition.Y, tagComment.StartPosition.X);
                Dom.TagComment tag = new Dom.TagComment(tagComment.Tag, tagRegion);
                tag.CommentString = tagComment.CommentText;
                cu.TagComments.Add(tag);
            }
        }

        public IResolver CreateResolver()
        {
            return new Dom.NRefactoryResolver.NRefactoryResolver(ProjectParser.CurrentProjectContent, LanguageProperties.CSharp);
        }

        public NRefactory.Parser.Errors LastErrors
        {
            get { return _lastErrors; }
        }

        ///////// IParser Interface END
    }
}
