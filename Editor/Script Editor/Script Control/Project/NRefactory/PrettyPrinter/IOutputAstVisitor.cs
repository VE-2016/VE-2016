﻿// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2198 $</version>
// </file>

using System;
using AIMS.Libraries.Scripting.NRefactory.Parser;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    /// <summary>
    /// Description of IOutputASTVisitor.
    /// </summary>
    public interface IOutputAstVisitor : IAstVisitor
    {
        NodeTracker NodeTracker
        {
            get;
        }

        string Text
        {
            get;
        }

        Errors Errors
        {
            get;
        }

        AbstractPrettyPrintOptions Options
        {
            get;
        }
        IOutputFormatter OutputFormatter
        {
            get;
        }
    }
    public interface IOutputFormatter
    {
        int IndentationLevel
        {
            get;
            set;
        }
        string Text
        {
            get;
        }
        void NewLine();
        void Indent();
        void PrintComment(Comment comment, bool forceWriteInPreviousBlock);
        void PrintPreprocessingDirective(PreprocessingDirective directive, bool forceWriteInPreviousBlock);
        void PrintBlankLine(bool forceWriteInPreviousBlock);
    }
}
