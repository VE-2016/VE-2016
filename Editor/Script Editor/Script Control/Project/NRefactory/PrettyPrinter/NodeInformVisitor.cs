// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    public delegate void InformNode(INode node);

    public class NodeTracker
    {
        private IAstVisitor _callVisitor;

        public IAstVisitor CallVisitor
        {
            get
            {
                return _callVisitor;
            }
        }

        public NodeTracker(IAstVisitor callVisitor)
        {
            _callVisitor = callVisitor;
        }

        public void BeginNode(INode node)
        {
            if (NodeVisiting != null)
            {
                NodeVisiting(node);
            }
        }

        public void EndNode(INode node)
        {
            if (NodeVisited != null)
            {
                NodeVisited(node);
            }
        }

        public object TrackedVisit(INode node, object data)
        {
            BeginNode(node);
            object ret = node.AcceptVisitor(_callVisitor, data);
            EndNode(node);
            return ret;
        }

        public object TrackedVisitChildren(INode node, object data)
        {
            foreach (INode child in node.Children)
            {
                TrackedVisit(child, data);
            }
            if (NodeChildrenVisited != null)
            {
                NodeChildrenVisited(node);
            }
            return data;
        }

        public event InformNode NodeVisiting;
        public event InformNode NodeChildrenVisited;
        public event InformNode NodeVisited;
    }
}
