// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1966 $</version>
// </file>

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    public enum BraceStyle
    {
        EndOfLine,
        NextLine,
        NextLineShifted,
        NextLineShifted2
    }

    /// <summary>
    /// Description of PrettyPrintOptions.	
    /// </summary>
    public class PrettyPrintOptions : AbstractPrettyPrintOptions
    {
        #region BraceStyle
        private BraceStyle _namespaceBraceStyle = BraceStyle.NextLine;
        private BraceStyle _classBraceStyle = BraceStyle.NextLine;
        private BraceStyle _interfaceBraceStyle = BraceStyle.NextLine;
        private BraceStyle _structBraceStyle = BraceStyle.NextLine;
        private BraceStyle _enumBraceStyle = BraceStyle.NextLine;

        private BraceStyle _constructorBraceStyle = BraceStyle.NextLine;
        private BraceStyle _destructorBraceStyle = BraceStyle.NextLine;
        private BraceStyle _methodBraceStyle = BraceStyle.NextLine;

        private BraceStyle _propertyBraceStyle = BraceStyle.EndOfLine;
        private BraceStyle _propertyGetBraceStyle = BraceStyle.EndOfLine;
        private BraceStyle _propertySetBraceStyle = BraceStyle.EndOfLine;

        private BraceStyle _eventAddBraceStyle = BraceStyle.EndOfLine;
        private BraceStyle _eventRemoveBraceStyle = BraceStyle.EndOfLine;

        private BraceStyle _statementBraceStyle = BraceStyle.EndOfLine;

        public BraceStyle StatementBraceStyle
        {
            get
            {
                return _statementBraceStyle;
            }
            set
            {
                _statementBraceStyle = value;
            }
        }

        public BraceStyle NamespaceBraceStyle
        {
            get
            {
                return _namespaceBraceStyle;
            }
            set
            {
                _namespaceBraceStyle = value;
            }
        }

        public BraceStyle ClassBraceStyle
        {
            get
            {
                return _classBraceStyle;
            }
            set
            {
                _classBraceStyle = value;
            }
        }

        public BraceStyle InterfaceBraceStyle
        {
            get
            {
                return _interfaceBraceStyle;
            }
            set
            {
                _interfaceBraceStyle = value;
            }
        }

        public BraceStyle StructBraceStyle
        {
            get
            {
                return _structBraceStyle;
            }
            set
            {
                _structBraceStyle = value;
            }
        }

        public BraceStyle EnumBraceStyle
        {
            get
            {
                return _enumBraceStyle;
            }
            set
            {
                _enumBraceStyle = value;
            }
        }


        public BraceStyle ConstructorBraceStyle
        {
            get
            {
                return _constructorBraceStyle;
            }
            set
            {
                _constructorBraceStyle = value;
            }
        }

        public BraceStyle DestructorBraceStyle
        {
            get
            {
                return _destructorBraceStyle;
            }
            set
            {
                _destructorBraceStyle = value;
            }
        }

        public BraceStyle MethodBraceStyle
        {
            get
            {
                return _methodBraceStyle;
            }
            set
            {
                _methodBraceStyle = value;
            }
        }

        public BraceStyle PropertyBraceStyle
        {
            get
            {
                return _propertyBraceStyle;
            }
            set
            {
                _propertyBraceStyle = value;
            }
        }
        public BraceStyle PropertyGetBraceStyle
        {
            get
            {
                return _propertyGetBraceStyle;
            }
            set
            {
                _propertyGetBraceStyle = value;
            }
        }
        public BraceStyle PropertySetBraceStyle
        {
            get
            {
                return _propertySetBraceStyle;
            }
            set
            {
                _propertySetBraceStyle = value;
            }
        }

        public BraceStyle EventAddBraceStyle
        {
            get
            {
                return _eventAddBraceStyle;
            }
            set
            {
                _eventAddBraceStyle = value;
            }
        }
        public BraceStyle EventRemoveBraceStyle
        {
            get
            {
                return _eventRemoveBraceStyle;
            }
            set
            {
                _eventRemoveBraceStyle = value;
            }
        }
        #endregion

        #region Before Parentheses
        private bool _beforeMethodCallParentheses = false;
        private bool _beforeDelegateDeclarationParentheses = false;
        private bool _beforeMethodDeclarationParentheses = false;
        private bool _beforeConstructorDeclarationParentheses = false;

        private bool _ifParentheses = true;
        private bool _whileParentheses = true;
        private bool _forParentheses = true;
        private bool _foreachParentheses = true;
        private bool _catchParentheses = true;
        private bool _switchParentheses = true;
        private bool _lockParentheses = true;
        private bool _usingParentheses = true;
        private bool _fixedParentheses = true;
        private bool _sizeOfParentheses = false;
        private bool _typeOfParentheses = false;
        private bool _checkedParentheses = false;
        private bool _uncheckedParentheses = false;
        private bool _newParentheses = false;

        public bool CheckedParentheses
        {
            get
            {
                return _checkedParentheses;
            }
            set
            {
                _checkedParentheses = value;
            }
        }
        public bool NewParentheses
        {
            get
            {
                return _newParentheses;
            }
            set
            {
                _newParentheses = value;
            }
        }
        public bool SizeOfParentheses
        {
            get
            {
                return _sizeOfParentheses;
            }
            set
            {
                _sizeOfParentheses = value;
            }
        }
        public bool TypeOfParentheses
        {
            get
            {
                return _typeOfParentheses;
            }
            set
            {
                _typeOfParentheses = value;
            }
        }
        public bool UncheckedParentheses
        {
            get
            {
                return _uncheckedParentheses;
            }
            set
            {
                _uncheckedParentheses = value;
            }
        }

        public bool BeforeConstructorDeclarationParentheses
        {
            get
            {
                return _beforeConstructorDeclarationParentheses;
            }
            set
            {
                _beforeConstructorDeclarationParentheses = value;
            }
        }

        public bool BeforeDelegateDeclarationParentheses
        {
            get
            {
                return _beforeDelegateDeclarationParentheses;
            }
            set
            {
                _beforeDelegateDeclarationParentheses = value;
            }
        }

        public bool BeforeMethodCallParentheses
        {
            get
            {
                return _beforeMethodCallParentheses;
            }
            set
            {
                _beforeMethodCallParentheses = value;
            }
        }

        public bool BeforeMethodDeclarationParentheses
        {
            get
            {
                return _beforeMethodDeclarationParentheses;
            }
            set
            {
                _beforeMethodDeclarationParentheses = value;
            }
        }

        public bool IfParentheses
        {
            get
            {
                return _ifParentheses;
            }
            set
            {
                _ifParentheses = value;
            }
        }

        public bool WhileParentheses
        {
            get
            {
                return _whileParentheses;
            }
            set
            {
                _whileParentheses = value;
            }
        }
        public bool ForeachParentheses
        {
            get
            {
                return _foreachParentheses;
            }
            set
            {
                _foreachParentheses = value;
            }
        }
        public bool LockParentheses
        {
            get
            {
                return _lockParentheses;
            }
            set
            {
                _lockParentheses = value;
            }
        }
        public bool UsingParentheses
        {
            get
            {
                return _usingParentheses;
            }
            set
            {
                _usingParentheses = value;
            }
        }

        public bool CatchParentheses
        {
            get
            {
                return _catchParentheses;
            }
            set
            {
                _catchParentheses = value;
            }
        }
        public bool FixedParentheses
        {
            get
            {
                return _fixedParentheses;
            }
            set
            {
                _fixedParentheses = value;
            }
        }
        public bool SwitchParentheses
        {
            get
            {
                return _switchParentheses;
            }
            set
            {
                _switchParentheses = value;
            }
        }
        public bool ForParentheses
        {
            get
            {
                return _forParentheses;
            }
            set
            {
                _forParentheses = value;
            }
        }

        #endregion

        #region AroundOperators
        private bool _aroundAssignmentParentheses = true;
        private bool _aroundLogicalOperatorParentheses = true;
        private bool _aroundEqualityOperatorParentheses = true;
        private bool _aroundRelationalOperatorParentheses = true;
        private bool _aroundBitwiseOperatorParentheses = true;
        private bool _aroundAdditiveOperatorParentheses = true;
        private bool _aroundMultiplicativeOperatorParentheses = true;
        private bool _aroundShiftOperatorParentheses = true;

        public bool AroundAdditiveOperatorParentheses
        {
            get
            {
                return _aroundAdditiveOperatorParentheses;
            }
            set
            {
                _aroundAdditiveOperatorParentheses = value;
            }
        }
        public bool AroundAssignmentParentheses
        {
            get
            {
                return _aroundAssignmentParentheses;
            }
            set
            {
                _aroundAssignmentParentheses = value;
            }
        }
        public bool AroundBitwiseOperatorParentheses
        {
            get
            {
                return _aroundBitwiseOperatorParentheses;
            }
            set
            {
                _aroundBitwiseOperatorParentheses = value;
            }
        }
        public bool AroundEqualityOperatorParentheses
        {
            get
            {
                return _aroundEqualityOperatorParentheses;
            }
            set
            {
                _aroundEqualityOperatorParentheses = value;
            }
        }
        public bool AroundLogicalOperatorParentheses
        {
            get
            {
                return _aroundLogicalOperatorParentheses;
            }
            set
            {
                _aroundLogicalOperatorParentheses = value;
            }
        }
        public bool AroundMultiplicativeOperatorParentheses
        {
            get
            {
                return _aroundMultiplicativeOperatorParentheses;
            }
            set
            {
                _aroundMultiplicativeOperatorParentheses = value;
            }
        }
        public bool AroundRelationalOperatorParentheses
        {
            get
            {
                return _aroundRelationalOperatorParentheses;
            }
            set
            {
                _aroundRelationalOperatorParentheses = value;
            }
        }
        public bool AroundShiftOperatorParentheses
        {
            get
            {
                return _aroundShiftOperatorParentheses;
            }
            set
            {
                _aroundShiftOperatorParentheses = value;
            }
        }
        #endregion

        #region SpacesInConditionalOperator
        private bool _conditionalOperatorBeforeConditionSpace = true;
        private bool _conditionalOperatorAfterConditionSpace = true;

        private bool _conditionalOperatorBeforeSeparatorSpace = true;
        private bool _conditionalOperatorAfterSeparatorSpace = true;

        public bool ConditionalOperatorAfterConditionSpace
        {
            get
            {
                return _conditionalOperatorAfterConditionSpace;
            }
            set
            {
                _conditionalOperatorAfterConditionSpace = value;
            }
        }
        public bool ConditionalOperatorAfterSeparatorSpace
        {
            get
            {
                return _conditionalOperatorAfterSeparatorSpace;
            }
            set
            {
                _conditionalOperatorAfterSeparatorSpace = value;
            }
        }
        public bool ConditionalOperatorBeforeConditionSpace
        {
            get
            {
                return _conditionalOperatorBeforeConditionSpace;
            }
            set
            {
                _conditionalOperatorBeforeConditionSpace = value;
            }
        }
        public bool ConditionalOperatorBeforeSeparatorSpace
        {
            get
            {
                return _conditionalOperatorBeforeSeparatorSpace;
            }
            set
            {
                _conditionalOperatorBeforeSeparatorSpace = value;
            }
        }
        #endregion

        #region OtherSpaces
        private bool _spacesWithinBrackets = false;
        private bool _spacesAfterComma = true;
        private bool _spacesBeforeComma = false;
        private bool _spacesAfterSemicolon = true;
        private bool _spacesAfterTypecast = false;

        public bool SpacesAfterComma
        {
            get
            {
                return _spacesAfterComma;
            }
            set
            {
                _spacesAfterComma = value;
            }
        }
        public bool SpacesAfterSemicolon
        {
            get
            {
                return _spacesAfterSemicolon;
            }
            set
            {
                _spacesAfterSemicolon = value;
            }
        }
        public bool SpacesAfterTypecast
        {
            get
            {
                return _spacesAfterTypecast;
            }
            set
            {
                _spacesAfterTypecast = value;
            }
        }
        public bool SpacesBeforeComma
        {
            get
            {
                return _spacesBeforeComma;
            }
            set
            {
                _spacesBeforeComma = value;
            }
        }
        public bool SpacesWithinBrackets
        {
            get
            {
                return _spacesWithinBrackets;
            }
            set
            {
                _spacesWithinBrackets = value;
            }
        }
        #endregion
    }
}
