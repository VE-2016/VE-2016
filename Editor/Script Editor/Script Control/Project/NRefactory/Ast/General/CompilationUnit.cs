﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
    public class CompilationUnit : AbstractNode
    {
        // Children in C#: UsingAliasDeclaration, UsingDeclaration, AttributeSection, NamespaceDeclaration
        // Children in VB: OptionStatements, ImportsStatement, AttributeSection, NamespaceDeclaration

        private Stack _blockStack = new Stack();

        public CompilationUnit()
        {
            _blockStack.Push(this);
        }

        public void BlockStart(INode block)
        {
            _blockStack.Push(block);
        }

        public void BlockEnd()
        {
            _blockStack.Pop();
        }

        public INode CurrentBock
        {
            get
            {
                return _blockStack.Count > 0 ? (INode)_blockStack.Peek() : null;
            }
        }

        public override void AddChild(INode childNode)
        {
            if (childNode != null)
            {
                INode parent = (INode)_blockStack.Peek();
                parent.Children.Add(childNode);
                childNode.Parent = parent;
            }
        }

        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return visitor.VisitCompilationUnit(this, data);
        }

        public override string ToString()
        {
            return String.Format("[CompilationUnit]");
        }
    }
}
