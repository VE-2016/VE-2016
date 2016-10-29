// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1611 $</version>
// </file>

using AIMS.Libraries.Scripting.NRefactory.Ast;

namespace AIMS.Libraries.Scripting.NRefactory.Parser.VB
{
    internal class ParamModifierList
    {
        private ParameterModifiers _cur;
        private Parser _parser;

        public ParameterModifiers Modifier
        {
            get
            {
                return _cur;
            }
        }

        public ParamModifierList(Parser parser)
        {
            _parser = parser;
            _cur = ParameterModifiers.None;
        }

        public bool isNone { get { return _cur == ParameterModifiers.None; } }

        public void Add(ParameterModifiers m)
        {
            if ((_cur & m) == 0)
            {
                _cur |= m;
            }
            else
            {
                _parser.Error("param modifier " + m + " already defined");
            }
        }

        public void Add(ParamModifierList m)
        {
            Add(m._cur);
        }

        public void Check()
        {
            if ((_cur & ParameterModifiers.In) != 0 &&
               (_cur & ParameterModifiers.Ref) != 0)
            {
                _parser.Error("ByRef and ByVal are not allowed at the same time.");
            }
        }
    }
}
