// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2615 $</version>
// </file>

using System;
using System.Collections.Generic;
using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    public class SpecialOutputVisitor : ISpecialVisitor
    {
        private IOutputFormatter _formatter;

        public SpecialOutputVisitor(IOutputFormatter formatter)
        {
            _formatter = formatter;
        }

        public bool ForceWriteInPreviousLine;

        public object Visit(ISpecial special, object data)
        {
            Console.WriteLine("Warning: SpecialOutputVisitor.Visit(ISpecial) called with " + special);
            return data;
        }

        public object Visit(BlankLine special, object data)
        {
            _formatter.PrintBlankLine(ForceWriteInPreviousLine);
            return data;
        }

        public object Visit(Comment special, object data)
        {
            _formatter.PrintComment(special, ForceWriteInPreviousLine);
            return data;
        }

        public object Visit(PreprocessingDirective special, object data)
        {
            _formatter.PrintPreprocessingDirective(special, ForceWriteInPreviousLine);
            return data;
        }
    }

    /// <summary>
    /// This class inserts specials between INodes.
    /// </summary>
    public sealed class SpecialNodesInserter : IDisposable
    {
        private IEnumerator<ISpecial> _enumerator;
        private SpecialOutputVisitor _visitor;
        private bool _available; // true when more specials are available

        public SpecialNodesInserter(IEnumerable<ISpecial> specials, SpecialOutputVisitor visitor)
        {
            if (specials == null) throw new ArgumentNullException("specials");
            if (visitor == null) throw new ArgumentNullException("visitor");
            _enumerator = specials.GetEnumerator();
            _visitor = visitor;
            _available = _enumerator.MoveNext();
        }

        private void WriteCurrent()
        {
            _enumerator.Current.AcceptVisitor(_visitor, null);
            _available = _enumerator.MoveNext();
        }

        private AttributedNode _currentAttributedNode;

        /// <summary>
        /// Writes all specials up to the start position of the node.
        /// </summary>
        public void AcceptNodeStart(INode node)
        {
            if (node is AttributedNode)
            {
                _currentAttributedNode = node as AttributedNode;
                if (_currentAttributedNode.Attributes.Count == 0)
                {
                    AcceptPoint(node.StartLocation);
                    _currentAttributedNode = null;
                }
            }
            else
            {
                AcceptPoint(node.StartLocation);
            }
        }

        /// <summary>
        /// Writes all specials up to the end position of the node.
        /// </summary>
        public void AcceptNodeEnd(INode node)
        {
            _visitor.ForceWriteInPreviousLine = true;
            AcceptPoint(node.EndLocation);
            _visitor.ForceWriteInPreviousLine = false;
            if (_currentAttributedNode != null)
            {
                if (node == _currentAttributedNode.Attributes[_currentAttributedNode.Attributes.Count - 1])
                {
                    AcceptPoint(_currentAttributedNode.StartLocation);
                    _currentAttributedNode = null;
                }
            }
        }

        /// <summary>
        /// Writes all specials up to the specified location.
        /// </summary>
        public void AcceptPoint(Location loc)
        {
            while (_available && _enumerator.Current.StartPosition <= loc)
            {
                WriteCurrent();
            }
        }

        /// <summary>
        /// Outputs all missing specials to the writer.
        /// </summary>
        public void Finish()
        {
            while (_available)
            {
                WriteCurrent();
            }
        }

        void IDisposable.Dispose()
        {
            Finish();
        }

        /// <summary>
        /// Registers a new SpecialNodesInserter with the output visitor.
        /// Make sure to call Finish() (or Dispose()) on the returned SpecialNodesInserter
        /// when the output is finished.
        /// </summary>
        public static SpecialNodesInserter Install(IEnumerable<ISpecial> specials, IOutputAstVisitor outputVisitor)
        {
            SpecialNodesInserter sni = new SpecialNodesInserter(specials, new SpecialOutputVisitor(outputVisitor.OutputFormatter));
            outputVisitor.NodeTracker.NodeVisiting += sni.AcceptNodeStart;
            outputVisitor.NodeTracker.NodeVisited += sni.AcceptNodeEnd;
            return sni;
        }
    }
}
