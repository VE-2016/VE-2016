// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
    public abstract class AbstractNode : INode
    {
        private INode _parent;
        private List<INode> _children = new List<INode>();

        private Location _startLocation;
        private Location _endLocation;

        public INode Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public Location StartLocation
        {
            get
            {
                return _startLocation;
            }
            set
            {
                _startLocation = value;
            }
        }

        public Location EndLocation
        {
            get
            {
                return _endLocation;
            }
            set
            {
                _endLocation = value;
            }
        }

        public List<INode> Children
        {
            get
            {
                return _children;
            }
            set
            {
                Debug.Assert(value != null);
                _children = value;
            }
        }

        public virtual void AddChild(INode childNode)
        {
            Debug.Assert(childNode != null);
            _children.Add(childNode);
        }

        public abstract object AcceptVisitor(IAstVisitor visitor, object data);

        public virtual object AcceptChildren(IAstVisitor visitor, object data)
        {
            foreach (INode child in _children)
            {
                Debug.Assert(child != null);
                child.AcceptVisitor(visitor, data);
            }
            return data;
        }

        public static string GetCollectionString(ICollection collection)
        {
            StringBuilder output = new StringBuilder();
            output.Append('{');

            if (collection != null)
            {
                IEnumerator en = collection.GetEnumerator();
                bool isFirst = true;
                while (en.MoveNext())
                {
                    if (!isFirst)
                    {
                        output.Append(", ");
                    }
                    else
                    {
                        isFirst = false;
                    }
                    output.Append(en.Current == null ? "<null>" : en.Current.ToString());
                }
            }
            else
            {
                return "null";
            }

            output.Append('}');
            return output.ToString();
        }
    }
}
