// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace AIMS.Libraries.Scripting.NRefactory.Ast
{
    public class PrimitiveExpression : Expression
    {
        private object _val;
        private string _stringValue;

        public object Value
        {
            get
            {
                return _val;
            }
            set
            {
                _val = value;
            }
        }

        public string StringValue
        {
            get
            {
                return _stringValue;
            }
            set
            {
                _stringValue = value == null ? String.Empty : value;
            }
        }

        public PrimitiveExpression(object val, string stringValue)
        {
            this.Value = val;
            this.StringValue = stringValue;
        }

        public override object AcceptVisitor(IAstVisitor visitor, object data)
        {
            return visitor.VisitPrimitiveExpression(this, data);
        }

        public override string ToString()
        {
            return String.Format("[PrimitiveExpression: Value={1}, ValueType={2}, StringValue={0}]",
                                 _stringValue,
                                 Value,
                                 Value == null ? "null" : Value.GetType().FullName
                                );
        }
    }
}
