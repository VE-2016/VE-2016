// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
    public abstract class Statement : AbstractNode, INullable
    {
        public static NullStatement Null
        {
            get
            {
                return NullStatement.Instance;
            }
        }

        public virtual bool IsNull
        {
            get
            {
                return false;
            }
        }

        public static Statement CheckNull(Statement statement)
        {
            return statement ?? NullStatement.Instance;
        }
    }

    public abstract class StatementWithEmbeddedStatement : Statement
    {
        private Statement _embeddedStatement;

        public Statement EmbeddedStatement
        {
            get
            {
                return _embeddedStatement;
            }
            set
            {
                _embeddedStatement = Statement.CheckNull(value);
                if (value != null)
                    value.Parent = this;
            }
        }
    }

    public class NullStatement : Statement
    {
        private static NullStatement s_nullStatement = new NullStatement();

        public override bool IsNull
        {
            get { return true; }
        }

        public static NullStatement Instance
        {
            get { return s_nullStatement; }
        }

        private NullStatement()
        {
        }

        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return data;
        }

        public override string ToString()
        {
            return String.Format("[NullStatement]");
        }
    }
}
