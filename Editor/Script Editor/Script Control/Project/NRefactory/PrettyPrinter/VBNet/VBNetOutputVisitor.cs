// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2200 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.NRefactory.Parser.VB;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    public class VBNetOutputVisitor : IOutputAstVisitor
    {
        private Errors _errors = new Errors();
        private VBNetOutputFormatter _outputFormatter;
        private VBNetPrettyPrintOptions _prettyPrintOptions = new VBNetPrettyPrintOptions();
        private NodeTracker _nodeTracker;
        private TypeDeclaration _currentType;
        private bool _printFullSystemType;

        private Stack<int> _exitTokenStack = new Stack<int>();

        public string Text
        {
            get
            {
                return _outputFormatter.Text;
            }
        }

        public Errors Errors
        {
            get
            {
                return _errors;
            }
        }

        AbstractPrettyPrintOptions IOutputAstVisitor.Options
        {
            get { return _prettyPrintOptions; }
        }

        public VBNetPrettyPrintOptions Options
        {
            get { return _prettyPrintOptions; }
        }

        public NodeTracker NodeTracker
        {
            get
            {
                return _nodeTracker;
            }
        }

        public IOutputFormatter OutputFormatter
        {
            get
            {
                return _outputFormatter;
            }
        }

        public VBNetOutputVisitor()
        {
            _outputFormatter = new VBNetOutputFormatter(_prettyPrintOptions);
            _nodeTracker = new NodeTracker(this);
        }

        private void Error(string text, Location position)
        {
            _errors.Error(position.Y, position.X, text);
        }

        private void UnsupportedNode(INode node)
        {
            Error(node.GetType().Name + " is unsupported", node.StartLocation);
        }

        #region AIMS.Libraries.Scripting.NRefactory.Parser.IASTVisitor interface implementation
        public object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
        {
            _nodeTracker.TrackedVisitChildren(compilationUnit, data);
            _outputFormatter.EndFile();
            return null;
        }

        /// <summary>
        /// Converts type name to primitive type name. Returns null if typeString is not
        /// a primitive type.
        /// </summary>
        private static string ConvertTypeString(string typeString)
        {
            string primitiveType;
            TypeReference.PrimitiveTypesVBReverse.TryGetValue(typeString, out primitiveType);
            return primitiveType;
        }

        public object VisitTypeReference(TypeReference typeReference, object data)
        {
            if (typeReference == TypeReference.ClassConstraint)
            {
                _outputFormatter.PrintToken(Tokens.Class);
            }
            else if (typeReference == TypeReference.StructConstraint)
            {
                _outputFormatter.PrintToken(Tokens.Structure);
            }
            else if (typeReference == TypeReference.NewConstraint)
            {
                _outputFormatter.PrintToken(Tokens.New);
            }
            else
            {
                PrintTypeReferenceWithoutArray(typeReference);
                if (typeReference.IsArrayType)
                {
                    PrintArrayRank(typeReference.RankSpecifier, 0);
                }
            }
            return null;
        }

        private void PrintTypeReferenceWithoutArray(TypeReference typeReference)
        {
            if (typeReference.IsGlobal)
            {
                _outputFormatter.PrintToken(Tokens.Global);
                _outputFormatter.PrintToken(Tokens.Dot);
            }
            if (typeReference.Type == null || typeReference.Type.Length == 0)
            {
                _outputFormatter.PrintText("Void");
            }
            else if (_printFullSystemType || typeReference.IsGlobal)
            {
                _outputFormatter.PrintIdentifier(typeReference.SystemType);
            }
            else
            {
                string shortTypeName = ConvertTypeString(typeReference.SystemType);
                if (shortTypeName != null)
                {
                    _outputFormatter.PrintText(shortTypeName);
                }
                else
                {
                    _outputFormatter.PrintIdentifier(typeReference.Type);
                }
            }
            if (typeReference.GenericTypes != null && typeReference.GenericTypes.Count > 0)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                _outputFormatter.PrintToken(Tokens.Of);
                _outputFormatter.Space();
                AppendCommaSeparatedList(typeReference.GenericTypes);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            for (int i = 0; i < typeReference.PointerNestingLevel; ++i)
            {
                _outputFormatter.PrintToken(Tokens.Times);
            }
        }

        private void PrintArrayRank(int[] rankSpecifier, int startRank)
        {
            for (int i = startRank; i < rankSpecifier.Length; ++i)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                for (int j = 0; j < rankSpecifier[i]; ++j)
                {
                    _outputFormatter.PrintToken(Tokens.Comma);
                }
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
        }

        public object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
        {
            _nodeTracker.TrackedVisit(innerClassTypeReference.BaseType, data);
            _outputFormatter.PrintToken(Tokens.Dot);
            return VisitTypeReference((TypeReference)innerClassTypeReference, data);
        }

        #region Global scope
        public object VisitAttributeSection(AttributeSection attributeSection, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintText("<");
            if (attributeSection.AttributeTarget != null && attributeSection.AttributeTarget.Length > 0)
            {
                _outputFormatter.PrintIdentifier(attributeSection.AttributeTarget);
                _outputFormatter.PrintToken(Tokens.Colon);
                _outputFormatter.Space();
            }
            Debug.Assert(attributeSection.Attributes != null);
            AppendCommaSeparatedList(attributeSection.Attributes);

            _outputFormatter.PrintText(">");

            if ("assembly".Equals(attributeSection.AttributeTarget, StringComparison.InvariantCultureIgnoreCase)
                || "module".Equals(attributeSection.AttributeTarget, StringComparison.InvariantCultureIgnoreCase))
            {
                _outputFormatter.NewLine();
            }
            else
            {
                _outputFormatter.PrintLineContinuation();
            }

            return null;
        }

        public object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
        {
            _outputFormatter.PrintIdentifier(attribute.Name);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(attribute.PositionalArguments);

            if (attribute.NamedArguments != null && attribute.NamedArguments.Count > 0)
            {
                if (attribute.PositionalArguments.Count > 0)
                {
                    _outputFormatter.PrintToken(Tokens.Comma);
                    _outputFormatter.Space();
                }
                AppendCommaSeparatedList(attribute.NamedArguments);
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
        {
            _outputFormatter.PrintIdentifier(namedArgumentExpression.Name);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Colon);
            _outputFormatter.PrintToken(Tokens.Assign);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(namedArgumentExpression.Expression, data);
            return null;
        }

        public object VisitUsing(Using @using, object data)
        {
            Debug.Fail("Should never be called. The usings should be handled in Visit(UsingDeclaration)");
            return null;
        }

        public object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Imports);
            _outputFormatter.Space();
            for (int i = 0; i < usingDeclaration.Usings.Count; ++i)
            {
                _outputFormatter.PrintIdentifier(((Using)usingDeclaration.Usings[i]).Name);
                if (((Using)usingDeclaration.Usings[i]).IsAlias)
                {
                    _outputFormatter.Space();
                    _outputFormatter.PrintToken(Tokens.Assign);
                    _outputFormatter.Space();
                    _printFullSystemType = true;
                    _nodeTracker.TrackedVisit(((Using)usingDeclaration.Usings[i]).Alias, data);
                    _printFullSystemType = false;
                }
                if (i + 1 < usingDeclaration.Usings.Count)
                {
                    _outputFormatter.PrintToken(Tokens.Comma);
                    _outputFormatter.Space();
                }
            }
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Namespace);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(namespaceDeclaration.Name);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisitChildren(namespaceDeclaration, data);
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Namespace);
            _outputFormatter.NewLine();
            return null;
        }

        private static int GetTypeToken(TypeDeclaration typeDeclaration)
        {
            switch (typeDeclaration.Type)
            {
                case ClassType.Class:
                    return Tokens.Class;
                case ClassType.Enum:
                    return Tokens.Enum;
                case ClassType.Interface:
                    return Tokens.Interface;
                case ClassType.Struct:
                    return Tokens.Structure;
                default:
                    return Tokens.Class;
            }
        }

        private void PrintTemplates(List<TemplateDefinition> templates)
        {
            if (templates != null && templates.Count > 0)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                _outputFormatter.PrintToken(Tokens.Of);
                _outputFormatter.Space();
                AppendCommaSeparatedList(templates);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
        }

        public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            VisitAttributes(typeDeclaration.Attributes, data);

            _outputFormatter.Indent();
            OutputModifier(typeDeclaration.Modifier, true);

            int typeToken = GetTypeToken(typeDeclaration);
            _outputFormatter.PrintToken(typeToken);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(typeDeclaration.Name);

            PrintTemplates(typeDeclaration.Templates);

            if (typeDeclaration.Type == ClassType.Enum
                && typeDeclaration.BaseTypes != null && typeDeclaration.BaseTypes.Count > 0)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                foreach (TypeReference baseTypeRef in typeDeclaration.BaseTypes)
                {
                    _nodeTracker.TrackedVisit(baseTypeRef, data);
                }
            }

            _outputFormatter.NewLine();
            ++_outputFormatter.IndentationLevel;

            if (typeDeclaration.BaseTypes != null && typeDeclaration.Type != ClassType.Enum)
            {
                foreach (TypeReference baseTypeRef in typeDeclaration.BaseTypes)
                {
                    _outputFormatter.Indent();

                    string baseType = baseTypeRef.Type;
                    if (baseType.IndexOf('.') >= 0)
                    {
                        baseType = baseType.Substring(baseType.LastIndexOf('.') + 1);
                    }
                    bool baseTypeIsInterface = baseType.Length >= 2 && baseType[0] == 'I' && Char.IsUpper(baseType[1]);

                    if (!baseTypeIsInterface || typeDeclaration.Type == ClassType.Interface)
                    {
                        _outputFormatter.PrintToken(Tokens.Inherits);
                    }
                    else
                    {
                        _outputFormatter.PrintToken(Tokens.Implements);
                    }
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(baseTypeRef, data);
                    _outputFormatter.NewLine();
                }
            }

            TypeDeclaration oldType = _currentType;
            _currentType = typeDeclaration;

            if (typeDeclaration.Type == ClassType.Enum)
            {
                OutputEnumMembers(typeDeclaration, data);
            }
            else
            {
                _nodeTracker.TrackedVisitChildren(typeDeclaration, data);
            }
            _currentType = oldType;

            --_outputFormatter.IndentationLevel;


            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(typeToken);
            _outputFormatter.NewLine();
            return null;
        }

        private void OutputEnumMembers(TypeDeclaration typeDeclaration, object data)
        {
            foreach (FieldDeclaration fieldDeclaration in typeDeclaration.Children)
            {
                _nodeTracker.BeginNode(fieldDeclaration);
                VariableDeclaration f = (VariableDeclaration)fieldDeclaration.Fields[0];
                VisitAttributes(fieldDeclaration.Attributes, data);
                _outputFormatter.Indent();
                _outputFormatter.PrintIdentifier(f.Name);
                if (f.Initializer != null && !f.Initializer.IsNull)
                {
                    _outputFormatter.Space();
                    _outputFormatter.PrintToken(Tokens.Assign);
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(f.Initializer, data);
                }
                _outputFormatter.NewLine();
                _nodeTracker.EndNode(fieldDeclaration);
            }
        }

        public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
        {
            _outputFormatter.PrintIdentifier(templateDefinition.Name);
            if (templateDefinition.Bases.Count > 0)
            {
                _outputFormatter.PrintText(" As ");
                if (templateDefinition.Bases.Count == 1)
                {
                    _nodeTracker.TrackedVisit(templateDefinition.Bases[0], data);
                }
                else
                {
                    _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
                    AppendCommaSeparatedList(templateDefinition.Bases);
                    _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
                }
            }
            return null;
        }

        public object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
        {
            VisitAttributes(delegateDeclaration.Attributes, data);

            _outputFormatter.Indent();
            OutputModifier(delegateDeclaration.Modifier, true);
            _outputFormatter.PrintToken(Tokens.Delegate);
            _outputFormatter.Space();

            bool isFunction = (delegateDeclaration.ReturnType.Type != "void");
            if (isFunction)
            {
                _outputFormatter.PrintToken(Tokens.Function);
                _outputFormatter.Space();
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Sub);
                _outputFormatter.Space();
            }
            _outputFormatter.PrintIdentifier(delegateDeclaration.Name);

            PrintTemplates(delegateDeclaration.Templates);

            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(delegateDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            if (isFunction)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(delegateDeclaration.ReturnType, data);
            }
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
        {
            _outputFormatter.PrintToken(Tokens.Option);
            _outputFormatter.Space();
            switch (optionDeclaration.OptionType)
            {
                case OptionType.Strict:
                    _outputFormatter.PrintToken(Tokens.Strict);
                    if (!optionDeclaration.OptionValue)
                    {
                        _outputFormatter.Space();
                        _outputFormatter.PrintToken(Tokens.Off);
                    }
                    break;
                case OptionType.Explicit:
                    _outputFormatter.PrintToken(Tokens.Explicit);
                    _outputFormatter.Space();
                    if (!optionDeclaration.OptionValue)
                    {
                        _outputFormatter.Space();
                        _outputFormatter.PrintToken(Tokens.Off);
                    }
                    break;
                case OptionType.CompareBinary:
                    _outputFormatter.PrintToken(Tokens.Compare);
                    _outputFormatter.Space();
                    _outputFormatter.PrintToken(Tokens.Binary);
                    break;
                case OptionType.CompareText:
                    _outputFormatter.PrintToken(Tokens.Compare);
                    _outputFormatter.Space();
                    _outputFormatter.PrintToken(Tokens.Text);
                    break;
            }
            _outputFormatter.NewLine();
            return null;
        }
        #endregion

        #region Type level
        private TypeReference _currentVariableType;
        public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
        {
            VisitAttributes(fieldDeclaration.Attributes, data);
            _outputFormatter.Indent();
            if (fieldDeclaration.Modifier == Modifiers.None)
            {
                _outputFormatter.PrintToken(Tokens.Private);
                _outputFormatter.Space();
            }
            else if (fieldDeclaration.Modifier == Modifiers.Dim)
            {
                _outputFormatter.PrintToken(Tokens.Dim);
                _outputFormatter.Space();
            }
            else
            {
                OutputModifier(fieldDeclaration.Modifier);
            }
            _currentVariableType = fieldDeclaration.TypeReference;
            AppendCommaSeparatedList(fieldDeclaration.Fields);
            _currentVariableType = null;

            _outputFormatter.NewLine();

            return null;
        }

        public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
        {
            _outputFormatter.PrintIdentifier(variableDeclaration.Name);

            TypeReference varType = _currentVariableType;
            if (varType != null && varType.IsNull)
                varType = null;
            if (varType == null && !variableDeclaration.TypeReference.IsNull)
                varType = variableDeclaration.TypeReference;

            if (varType != null)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                ObjectCreateExpression init = variableDeclaration.Initializer as ObjectCreateExpression;
                if (init != null && TypeReference.AreEqualReferences(init.CreateType, varType))
                {
                    _nodeTracker.TrackedVisit(variableDeclaration.Initializer, data);
                    return null;
                }
                else
                {
                    _nodeTracker.TrackedVisit(varType, data);
                }
            }

            if (!variableDeclaration.Initializer.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Assign);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(variableDeclaration.Initializer, data);
            }
            return null;
        }

        public object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data)
        {
            VisitAttributes(propertyDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(propertyDeclaration.Modifier);

            if ((propertyDeclaration.Modifier & (Modifiers.ReadOnly | Modifiers.WriteOnly)) == Modifiers.None)
            {
                if (propertyDeclaration.IsReadOnly)
                {
                    _outputFormatter.PrintToken(Tokens.ReadOnly);
                    _outputFormatter.Space();
                }
                else if (propertyDeclaration.IsWriteOnly)
                {
                    _outputFormatter.PrintToken(Tokens.WriteOnly);
                    _outputFormatter.Space();
                }
            }

            _outputFormatter.PrintToken(Tokens.Property);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(propertyDeclaration.Name);

            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(propertyDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.As);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(propertyDeclaration.TypeReference, data);

            PrintInterfaceImplementations(propertyDeclaration.InterfaceImplementations);

            _outputFormatter.NewLine();

            if (!IsAbstract(propertyDeclaration))
            {
                ++_outputFormatter.IndentationLevel;
                _exitTokenStack.Push(Tokens.Property);
                _nodeTracker.TrackedVisit(propertyDeclaration.GetRegion, data);
                _nodeTracker.TrackedVisit(propertyDeclaration.SetRegion, data);
                _exitTokenStack.Pop();
                --_outputFormatter.IndentationLevel;

                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.End);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Property);
                _outputFormatter.NewLine();
            }

            return null;
        }

        public object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
        {
            VisitAttributes(propertyGetRegion.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(propertyGetRegion.Modifier);
            _outputFormatter.PrintToken(Tokens.Get);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(propertyGetRegion.Block, data);
            --_outputFormatter.IndentationLevel;
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Get);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
        {
            VisitAttributes(propertySetRegion.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(propertySetRegion.Modifier);
            _outputFormatter.PrintToken(Tokens.Set);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(propertySetRegion.Block, data);
            --_outputFormatter.IndentationLevel;
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Set);
            _outputFormatter.NewLine();
            return null;
        }

        private TypeReference _currentEventType = null;
        public object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
        {
            bool customEvent = eventDeclaration.HasAddRegion || eventDeclaration.HasRemoveRegion;

            VisitAttributes(eventDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(eventDeclaration.Modifier);
            if (customEvent)
            {
                _outputFormatter.PrintText("Custom");
                _outputFormatter.Space();
            }

            _outputFormatter.PrintToken(Tokens.Event);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(eventDeclaration.Name);

            if (eventDeclaration.Parameters.Count > 0)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                this.AppendCommaSeparatedList(eventDeclaration.Parameters);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.As);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(eventDeclaration.TypeReference, data);

            PrintInterfaceImplementations(eventDeclaration.InterfaceImplementations);

            if (!eventDeclaration.Initializer.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Assign);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(eventDeclaration.Initializer, data);
            }

            _outputFormatter.NewLine();

            if (customEvent)
            {
                ++_outputFormatter.IndentationLevel;
                _currentEventType = eventDeclaration.TypeReference;
                _exitTokenStack.Push(Tokens.Sub);
                _nodeTracker.TrackedVisit(eventDeclaration.AddRegion, data);
                _nodeTracker.TrackedVisit(eventDeclaration.RemoveRegion, data);
                _exitTokenStack.Pop();
                --_outputFormatter.IndentationLevel;

                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.End);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Event);
                _outputFormatter.NewLine();
            }
            return null;
        }

        private void PrintInterfaceImplementations(IList<InterfaceImplementation> list)
        {
            if (list == null || list.Count == 0)
                return;
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Implements);
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                    _outputFormatter.PrintToken(Tokens.Comma);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(list[i].InterfaceType, null);
                _outputFormatter.PrintToken(Tokens.Dot);
                _outputFormatter.PrintIdentifier(list[i].MemberName);
            }
        }

        public object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
        {
            VisitAttributes(eventAddRegion.Attributes, data);
            _outputFormatter.Indent();
            _outputFormatter.PrintText("AddHandler(");
            if (eventAddRegion.Parameters.Count == 0)
            {
                _outputFormatter.PrintToken(Tokens.ByVal);
                _outputFormatter.Space();
                _outputFormatter.PrintIdentifier("value");
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(_currentEventType, data);
            }
            else
            {
                this.AppendCommaSeparatedList(eventAddRegion.Parameters);
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(eventAddRegion.Block, data);
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintText("AddHandler");
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
        {
            VisitAttributes(eventRemoveRegion.Attributes, data);
            _outputFormatter.Indent();
            _outputFormatter.PrintText("RemoveHandler");
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            if (eventRemoveRegion.Parameters.Count == 0)
            {
                _outputFormatter.PrintToken(Tokens.ByVal);
                _outputFormatter.Space();
                _outputFormatter.PrintIdentifier("value");
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(_currentEventType, data);
            }
            else
            {
                this.AppendCommaSeparatedList(eventRemoveRegion.Parameters);
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(eventRemoveRegion.Block, data);
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintText("RemoveHandler");
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
        {
            VisitAttributes(eventRaiseRegion.Attributes, data);
            _outputFormatter.Indent();
            _outputFormatter.PrintText("RaiseEvent");
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            if (eventRaiseRegion.Parameters.Count == 0)
            {
                _outputFormatter.PrintToken(Tokens.ByVal);
                _outputFormatter.Space();
                _outputFormatter.PrintIdentifier("value");
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(_currentEventType, data);
            }
            else
            {
                this.AppendCommaSeparatedList(eventRaiseRegion.Parameters);
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(eventRaiseRegion.Block, data);
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintText("RaiseEvent");
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
        {
            VisitAttributes(parameterDeclarationExpression.Attributes, data);
            OutputModifier(parameterDeclarationExpression.ParamModifier, parameterDeclarationExpression.StartLocation);
            _outputFormatter.PrintIdentifier(parameterDeclarationExpression.ParameterName);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.As);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(parameterDeclarationExpression.TypeReference, data);
            return null;
        }

        public object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
        {
            VisitAttributes(methodDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(methodDeclaration.Modifier);

            bool isSub = methodDeclaration.TypeReference.IsNull ||
                methodDeclaration.TypeReference.SystemType == "System.Void";

            if (isSub)
            {
                _outputFormatter.PrintToken(Tokens.Sub);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Function);
            }
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(methodDeclaration.Name);

            PrintTemplates(methodDeclaration.Templates);

            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(methodDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            if (!isSub)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(methodDeclaration.TypeReference, data);
            }

            PrintInterfaceImplementations(methodDeclaration.InterfaceImplementations);

            _outputFormatter.NewLine();

            if (!IsAbstract(methodDeclaration))
            {
                _nodeTracker.BeginNode(methodDeclaration.Body);
                ++_outputFormatter.IndentationLevel;
                _exitTokenStack.Push(isSub ? Tokens.Sub : Tokens.Function);
                methodDeclaration.Body.AcceptVisitor(this, data);
                _exitTokenStack.Pop();
                --_outputFormatter.IndentationLevel;

                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.End);
                _outputFormatter.Space();
                if (isSub)
                {
                    _outputFormatter.PrintToken(Tokens.Sub);
                }
                else
                {
                    _outputFormatter.PrintToken(Tokens.Function);
                }
                _outputFormatter.NewLine();
                _nodeTracker.EndNode(methodDeclaration.Body);
            }
            return null;
        }

        public object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
        {
            throw new InvalidOperationException();
        }

        private bool IsAbstract(AttributedNode node)
        {
            if ((node.Modifier & Modifiers.Abstract) == Modifiers.Abstract)
                return true;
            return _currentType != null && _currentType.Type == ClassType.Interface;
        }

        public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
        {
            VisitAttributes(constructorDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(constructorDeclaration.Modifier);
            _outputFormatter.PrintToken(Tokens.Sub);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.New);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(constructorDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _exitTokenStack.Push(Tokens.Sub);

            _nodeTracker.TrackedVisit(constructorDeclaration.ConstructorInitializer, data);

            _nodeTracker.TrackedVisit(constructorDeclaration.Body, data);
            _exitTokenStack.Pop();
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Sub);
            _outputFormatter.NewLine();

            return null;
        }

        public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
        {
            _outputFormatter.Indent();
            if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.This)
            {
                _outputFormatter.PrintToken(Tokens.Me);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.MyBase);
            }
            _outputFormatter.PrintToken(Tokens.Dot);
            _outputFormatter.PrintToken(Tokens.New);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(constructorInitializer.Arguments);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            _outputFormatter.NewLine();
            return null;
        }

        public object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
        {
            VisitAttributes(indexerDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(indexerDeclaration.Modifier);
            _outputFormatter.PrintToken(Tokens.Default);
            _outputFormatter.Space();
            if (indexerDeclaration.IsReadOnly)
            {
                _outputFormatter.PrintToken(Tokens.ReadOnly);
                _outputFormatter.Space();
            }
            else if (indexerDeclaration.IsWriteOnly)
            {
                _outputFormatter.PrintToken(Tokens.WriteOnly);
                _outputFormatter.Space();
            }

            _outputFormatter.PrintToken(Tokens.Property);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier("Item");

            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(indexerDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.As);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(indexerDeclaration.TypeReference, data);
            PrintInterfaceImplementations(indexerDeclaration.InterfaceImplementations);

            _outputFormatter.NewLine();
            ++_outputFormatter.IndentationLevel;
            _exitTokenStack.Push(Tokens.Property);
            _nodeTracker.TrackedVisit(indexerDeclaration.GetRegion, data);
            _nodeTracker.TrackedVisit(indexerDeclaration.SetRegion, data);
            _exitTokenStack.Pop();
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Property);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintText("Protected Overrides Sub Finalize()");
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _exitTokenStack.Push(Tokens.Sub);

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Try);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(destructorDeclaration.Body, data);
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Finally);
            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _outputFormatter.Indent();
            _outputFormatter.PrintText("MyBase.Finalize()");
            _outputFormatter.NewLine();
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Try);
            _outputFormatter.NewLine();

            _exitTokenStack.Pop();
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Sub);
            _outputFormatter.NewLine();

            return null;
        }

        public object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data)
        {
            VisitAttributes(operatorDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(operatorDeclaration.Modifier);

            if (operatorDeclaration.IsConversionOperator)
            {
                if (operatorDeclaration.ConversionType == ConversionType.Implicit)
                {
                    _outputFormatter.PrintToken(Tokens.Widening);
                }
                else
                {
                    _outputFormatter.PrintToken(Tokens.Narrowing);
                }
                _outputFormatter.Space();
            }

            _outputFormatter.PrintToken(Tokens.Operator);
            _outputFormatter.Space();

            int op = -1;

            switch (operatorDeclaration.OverloadableOperator)
            {
                case OverloadableOperatorType.Add:
                    op = Tokens.Plus;
                    break;
                case OverloadableOperatorType.Subtract:
                    op = Tokens.Minus;
                    break;
                case OverloadableOperatorType.Multiply:
                    op = Tokens.Times;
                    break;
                case OverloadableOperatorType.Divide:
                    op = Tokens.Div;
                    break;
                case OverloadableOperatorType.Modulus:
                    op = Tokens.Mod;
                    break;
                case OverloadableOperatorType.Concat:
                    op = Tokens.ConcatString;
                    break;
                case OverloadableOperatorType.Not:
                    op = Tokens.Not;
                    break;
                case OverloadableOperatorType.BitNot:
                    op = Tokens.Not;
                    break;
                case OverloadableOperatorType.BitwiseAnd:
                    op = Tokens.And;
                    break;
                case OverloadableOperatorType.BitwiseOr:
                    op = Tokens.Or;
                    break;
                case OverloadableOperatorType.ExclusiveOr:
                    op = Tokens.Xor;
                    break;
                case OverloadableOperatorType.ShiftLeft:
                    op = Tokens.ShiftLeft;
                    break;
                case OverloadableOperatorType.ShiftRight:
                    op = Tokens.ShiftRight;
                    break;
                case OverloadableOperatorType.GreaterThan:
                    op = Tokens.GreaterThan;
                    break;
                case OverloadableOperatorType.GreaterThanOrEqual:
                    op = Tokens.GreaterEqual;
                    break;
                case OverloadableOperatorType.Equality:
                    op = Tokens.Assign;
                    break;
                case OverloadableOperatorType.InEquality:
                    op = Tokens.NotEqual;
                    break;
                case OverloadableOperatorType.LessThan:
                    op = Tokens.LessThan;
                    break;
                case OverloadableOperatorType.LessThanOrEqual:
                    op = Tokens.LessEqual;
                    break;
                case OverloadableOperatorType.Increment:
                    Error("Increment operator is not supported in Visual Basic", operatorDeclaration.StartLocation);
                    break;
                case OverloadableOperatorType.Decrement:
                    Error("Decrement operator is not supported in Visual Basic", operatorDeclaration.StartLocation);
                    break;
                case OverloadableOperatorType.IsTrue:
                    _outputFormatter.PrintText("IsTrue");
                    break;
                case OverloadableOperatorType.IsFalse:
                    _outputFormatter.PrintText("IsFalse");
                    break;
                case OverloadableOperatorType.Like:
                    op = Tokens.Like;
                    break;
                case OverloadableOperatorType.Power:
                    op = Tokens.Power;
                    break;
                case OverloadableOperatorType.CType:
                    op = Tokens.CType;
                    break;
                case OverloadableOperatorType.DivideInteger:
                    op = Tokens.DivInteger;
                    break;
            }



            if (operatorDeclaration.IsConversionOperator)
            {
                _outputFormatter.PrintToken(Tokens.CType);
            }
            else
            {
                if (op != -1) _outputFormatter.PrintToken(op);
            }

            PrintTemplates(operatorDeclaration.Templates);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(operatorDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            if (!operatorDeclaration.TypeReference.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(operatorDeclaration.TypeReference, data);
            }

            _outputFormatter.NewLine();

            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(operatorDeclaration.Body, data);
            --_outputFormatter.IndentationLevel;

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Operator);
            _outputFormatter.NewLine();

            return null;
        }

        public object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
        {
            VisitAttributes(declareDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(declareDeclaration.Modifier);
            _outputFormatter.PrintToken(Tokens.Declare);
            _outputFormatter.Space();

            switch (declareDeclaration.Charset)
            {
                case CharsetModifier.Auto:
                    _outputFormatter.PrintToken(Tokens.Auto);
                    _outputFormatter.Space();
                    break;
                case CharsetModifier.Unicode:
                    _outputFormatter.PrintToken(Tokens.Unicode);
                    _outputFormatter.Space();
                    break;
                case CharsetModifier.Ansi:
                    _outputFormatter.PrintToken(Tokens.Ansi);
                    _outputFormatter.Space();
                    break;
            }

            bool isVoid = declareDeclaration.TypeReference.IsNull || declareDeclaration.TypeReference.SystemType == "System.Void";
            if (isVoid)
            {
                _outputFormatter.PrintToken(Tokens.Sub);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Function);
            }
            _outputFormatter.Space();

            _outputFormatter.PrintIdentifier(declareDeclaration.Name);

            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Lib);
            _outputFormatter.Space();
            _outputFormatter.PrintText('"' + ConvertString(declareDeclaration.Library) + '"');
            _outputFormatter.Space();

            if (declareDeclaration.Alias.Length > 0)
            {
                _outputFormatter.PrintToken(Tokens.Alias);
                _outputFormatter.Space();
                _outputFormatter.PrintText('"' + ConvertString(declareDeclaration.Alias) + '"');
                _outputFormatter.Space();
            }

            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(declareDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            if (!isVoid)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(declareDeclaration.TypeReference, data);
            }

            _outputFormatter.NewLine();

            return null;
        }
        #endregion

        #region Statements
        public object VisitBlockStatement(BlockStatement blockStatement, object data)
        {
            VisitStatementList(blockStatement.Children);
            return null;
        }

        private void PrintIndentedBlock(Statement stmt)
        {
            _outputFormatter.IndentationLevel += 1;
            if (stmt is BlockStatement)
            {
                _nodeTracker.TrackedVisit(stmt, null);
            }
            else
            {
                _outputFormatter.Indent();
                _nodeTracker.TrackedVisit(stmt, null);
                _outputFormatter.NewLine();
            }
            _outputFormatter.IndentationLevel -= 1;
        }

        private void PrintIndentedBlock(IEnumerable statements)
        {
            _outputFormatter.IndentationLevel += 1;
            VisitStatementList(statements);
            _outputFormatter.IndentationLevel -= 1;
        }

        private void VisitStatementList(IEnumerable statements)
        {
            foreach (Statement stmt in statements)
            {
                if (stmt is BlockStatement)
                {
                    _nodeTracker.TrackedVisit(stmt, null);
                }
                else
                {
                    _outputFormatter.Indent();
                    _nodeTracker.TrackedVisit(stmt, null);
                    _outputFormatter.NewLine();
                }
            }
        }

        public object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.AddHandler);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(addHandlerStatement.EventExpression, data);
            _outputFormatter.PrintToken(Tokens.Comma);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(addHandlerStatement.HandlerExpression, data);
            return null;
        }

        public object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.RemoveHandler);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(removeHandlerStatement.EventExpression, data);
            _outputFormatter.PrintToken(Tokens.Comma);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(removeHandlerStatement.HandlerExpression, data);
            return null;
        }

        public object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.RaiseEvent);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(raiseEventStatement.Arguments);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitEraseStatement(EraseStatement eraseStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Erase);
            _outputFormatter.Space();
            AppendCommaSeparatedList(eraseStatement.Expressions);
            return null;
        }

        public object VisitErrorStatement(ErrorStatement errorStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Error);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(errorStatement.Expression, data);
            return null;
        }

        public object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.On);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Error);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(onErrorStatement.EmbeddedStatement, data);
            return null;
        }

        public object VisitReDimStatement(ReDimStatement reDimStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.ReDim);
            _outputFormatter.Space();
            if (reDimStatement.IsPreserve)
            {
                _outputFormatter.PrintToken(Tokens.Preserve);
                _outputFormatter.Space();
            }

            AppendCommaSeparatedList(reDimStatement.ReDimClauses);
            return null;
        }

        public object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
        {
            _nodeTracker.TrackedVisit(expressionStatement.Expression, data);
            return null;
        }

        public object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
        {
            if (localVariableDeclaration.Modifier != Modifiers.None)
            {
                OutputModifier(localVariableDeclaration.Modifier);
            }
            if (!_isUsingResourceAcquisition)
            {
                if ((localVariableDeclaration.Modifier & Modifiers.Const) == 0)
                {
                    _outputFormatter.PrintToken(Tokens.Dim);
                }
                _outputFormatter.Space();
            }
            _currentVariableType = localVariableDeclaration.TypeReference;

            AppendCommaSeparatedList(localVariableDeclaration.Variables);
            _currentVariableType = null;

            return null;
        }

        public object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
        {
            _outputFormatter.NewLine();
            return null;
        }

        public virtual object VisitYieldStatement(YieldStatement yieldStatement, object data)
        {
            UnsupportedNode(yieldStatement);
            return null;
        }

        public object VisitReturnStatement(ReturnStatement returnStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Return);
            if (!returnStatement.Expression.IsNull)
            {
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(returnStatement.Expression, data);
            }
            return null;
        }

        public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.If);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(ifElseStatement.Condition, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Then);
            _outputFormatter.NewLine();

            PrintIndentedBlock(ifElseStatement.TrueStatement);

            foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections)
            {
                _nodeTracker.TrackedVisit(elseIfSection, data);
            }

            if (ifElseStatement.HasElseStatements)
            {
                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.Else);
                _outputFormatter.NewLine();
                PrintIndentedBlock(ifElseStatement.FalseStatement);
            }

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.If);
            return null;
        }

        public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
        {
            _outputFormatter.PrintToken(Tokens.ElseIf);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(elseIfSection.Condition, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Then);
            _outputFormatter.NewLine();
            PrintIndentedBlock(elseIfSection.EmbeddedStatement);
            return null;
        }

        public object VisitForStatement(ForStatement forStatement, object data)
        {
            // Is converted to {initializer} while <Condition> {Embedded} {Iterators} end while
            _exitTokenStack.Push(Tokens.While);
            bool isFirstLine = true;
            foreach (INode node in forStatement.Initializers)
            {
                if (!isFirstLine)
                    _outputFormatter.Indent();
                isFirstLine = false;
                _nodeTracker.TrackedVisit(node, data);
                _outputFormatter.NewLine();
            }
            if (!isFirstLine)
                _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.While);
            _outputFormatter.Space();
            if (forStatement.Condition.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.True);
            }
            else
            {
                _nodeTracker.TrackedVisit(forStatement.Condition, data);
            }
            _outputFormatter.NewLine();

            PrintIndentedBlock(forStatement.EmbeddedStatement);
            PrintIndentedBlock(forStatement.Iterator);

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.While);
            _exitTokenStack.Pop();
            return null;
        }

        public object VisitLabelStatement(LabelStatement labelStatement, object data)
        {
            _outputFormatter.PrintIdentifier(labelStatement.Label);
            _outputFormatter.PrintToken(Tokens.Colon);
            return null;
        }

        public object VisitGotoStatement(GotoStatement gotoStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.GoTo);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(gotoStatement.Label);
            return null;
        }

        public object VisitSwitchStatement(SwitchStatement switchStatement, object data)
        {
            _exitTokenStack.Push(Tokens.Select);
            _outputFormatter.PrintToken(Tokens.Select);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Case);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(switchStatement.SwitchExpression, data);
            _outputFormatter.NewLine();
            ++_outputFormatter.IndentationLevel;
            foreach (SwitchSection section in switchStatement.SwitchSections)
            {
                _nodeTracker.TrackedVisit(section, data);
            }
            --_outputFormatter.IndentationLevel;
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Select);
            _exitTokenStack.Pop();
            return null;
        }

        public object VisitSwitchSection(SwitchSection switchSection, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Case);
            _outputFormatter.Space();
            this.AppendCommaSeparatedList(switchSection.SwitchLabels);
            _outputFormatter.NewLine();

            PrintIndentedBlock(switchSection.Children);

            return null;
        }

        public object VisitCaseLabel(CaseLabel caseLabel, object data)
        {
            if (caseLabel.IsDefault)
            {
                _outputFormatter.PrintToken(Tokens.Else);
            }
            else
            {
                if (caseLabel.BinaryOperatorType != BinaryOperatorType.None)
                {
                    switch (caseLabel.BinaryOperatorType)
                    {
                        case BinaryOperatorType.Equality:
                            _outputFormatter.PrintToken(Tokens.Assign);
                            break;
                        case BinaryOperatorType.InEquality:
                            _outputFormatter.PrintToken(Tokens.LessThan);
                            _outputFormatter.PrintToken(Tokens.GreaterThan);
                            break;

                        case BinaryOperatorType.GreaterThan:
                            _outputFormatter.PrintToken(Tokens.GreaterThan);
                            break;
                        case BinaryOperatorType.GreaterThanOrEqual:
                            _outputFormatter.PrintToken(Tokens.GreaterEqual);
                            break;
                        case BinaryOperatorType.LessThan:
                            _outputFormatter.PrintToken(Tokens.LessThan);
                            break;
                        case BinaryOperatorType.LessThanOrEqual:
                            _outputFormatter.PrintToken(Tokens.LessEqual);
                            break;
                    }
                    _outputFormatter.Space();
                }

                _nodeTracker.TrackedVisit(caseLabel.Label, data);
                if (!caseLabel.ToExpression.IsNull)
                {
                    _outputFormatter.Space();
                    _outputFormatter.PrintToken(Tokens.To);
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
                }
            }

            return null;
        }

        public object VisitBreakStatement(BreakStatement breakStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Exit);
            if (_exitTokenStack.Count > 0)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(_exitTokenStack.Peek());
            }
            return null;
        }

        public object VisitStopStatement(StopStatement stopStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Stop);
            return null;
        }

        public object VisitResumeStatement(ResumeStatement resumeStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Resume);
            _outputFormatter.Space();
            if (resumeStatement.IsResumeNext)
            {
                _outputFormatter.PrintToken(Tokens.Next);
            }
            else
            {
                _outputFormatter.PrintIdentifier(resumeStatement.LabelName);
            }
            return null;
        }

        public object VisitEndStatement(EndStatement endStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.End);
            return null;
        }

        public object VisitContinueStatement(ContinueStatement continueStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Continue);
            _outputFormatter.Space();
            switch (continueStatement.ContinueType)
            {
                case ContinueType.Do:
                    _outputFormatter.PrintToken(Tokens.Do);
                    break;
                case ContinueType.For:
                    _outputFormatter.PrintToken(Tokens.For);
                    break;
                case ContinueType.While:
                    _outputFormatter.PrintToken(Tokens.While);
                    break;
                default:
                    _outputFormatter.PrintToken(_exitTokenStack.Peek());
                    break;
            }
            return null;
        }

        public object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
        {
            _outputFormatter.PrintText("goto case ");
            if (gotoCaseStatement.IsDefaultCase)
            {
                _outputFormatter.PrintText("default");
            }
            else
            {
                _nodeTracker.TrackedVisit(gotoCaseStatement.Expression, null);
            }
            return null;
        }

        public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
        {
            if (doLoopStatement.ConditionPosition == ConditionPosition.None)
            {
                Error(String.Format("Unknown condition position for loop : {0}.", doLoopStatement), doLoopStatement.StartLocation);
            }

            if (doLoopStatement.ConditionPosition == ConditionPosition.Start)
            {
                switch (doLoopStatement.ConditionType)
                {
                    case ConditionType.DoWhile:
                        _exitTokenStack.Push(Tokens.Do);
                        _outputFormatter.PrintToken(Tokens.Do);
                        _outputFormatter.Space();
                        _outputFormatter.PrintToken(Tokens.While);
                        break;
                    case ConditionType.While:
                        _exitTokenStack.Push(Tokens.While);
                        _outputFormatter.PrintToken(Tokens.While);
                        break;
                    case ConditionType.Until:
                        _exitTokenStack.Push(Tokens.Do);
                        _outputFormatter.PrintToken(Tokens.Do);
                        _outputFormatter.Space();
                        _outputFormatter.PrintToken(Tokens.While);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
            }
            else
            {
                _exitTokenStack.Push(Tokens.Do);
                _outputFormatter.PrintToken(Tokens.Do);
            }

            _outputFormatter.NewLine();

            PrintIndentedBlock(doLoopStatement.EmbeddedStatement);

            _outputFormatter.Indent();
            if (doLoopStatement.ConditionPosition == ConditionPosition.Start && doLoopStatement.ConditionType == ConditionType.While)
            {
                _outputFormatter.PrintToken(Tokens.End);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.While);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Loop);
            }

            if (doLoopStatement.ConditionPosition == ConditionPosition.End && !doLoopStatement.Condition.IsNull)
            {
                _outputFormatter.Space();
                switch (doLoopStatement.ConditionType)
                {
                    case ConditionType.While:
                    case ConditionType.DoWhile:
                        _outputFormatter.PrintToken(Tokens.While);
                        break;
                    case ConditionType.Until:
                        _outputFormatter.PrintToken(Tokens.Until);
                        break;
                }
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
            }
            _exitTokenStack.Pop();
            return null;
        }

        public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
        {
            _exitTokenStack.Push(Tokens.For);
            _outputFormatter.PrintToken(Tokens.For);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Each);
            _outputFormatter.Space();

            // loop control variable
            _outputFormatter.PrintIdentifier(foreachStatement.VariableName);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.As);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(foreachStatement.TypeReference, data);

            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.In);
            _outputFormatter.Space();

            _nodeTracker.TrackedVisit(foreachStatement.Expression, data);
            _outputFormatter.NewLine();

            PrintIndentedBlock(foreachStatement.EmbeddedStatement);

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Next);
            if (!foreachStatement.NextExpression.IsNull)
            {
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(foreachStatement.NextExpression, data);
            }
            _exitTokenStack.Pop();
            return null;
        }

        public object VisitLockStatement(LockStatement lockStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.SyncLock);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(lockStatement.LockExpression, data);
            _outputFormatter.NewLine();

            PrintIndentedBlock(lockStatement.EmbeddedStatement);

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.SyncLock);
            return null;
        }

        private bool _isUsingResourceAcquisition;

        public object VisitUsingStatement(UsingStatement usingStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Using);
            _outputFormatter.Space();

            _isUsingResourceAcquisition = true;
            _nodeTracker.TrackedVisit(usingStatement.ResourceAcquisition, data);
            _isUsingResourceAcquisition = false;
            _outputFormatter.NewLine();

            PrintIndentedBlock(usingStatement.EmbeddedStatement);

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Using);

            return null;
        }

        public object VisitWithStatement(WithStatement withStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.With);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(withStatement.Expression, data);
            _outputFormatter.NewLine();

            PrintIndentedBlock(withStatement.Body);

            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.With);
            return null;
        }

        public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
        {
            _exitTokenStack.Push(Tokens.Try);
            _outputFormatter.PrintToken(Tokens.Try);
            _outputFormatter.NewLine();

            PrintIndentedBlock(tryCatchStatement.StatementBlock);

            foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
            {
                _nodeTracker.TrackedVisit(catchClause, data);
            }

            if (!tryCatchStatement.FinallyBlock.IsNull)
            {
                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.Finally);
                _outputFormatter.NewLine();
                PrintIndentedBlock(tryCatchStatement.FinallyBlock);
            }
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.End);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Try);
            _exitTokenStack.Pop();
            return null;
        }

        public object VisitCatchClause(CatchClause catchClause, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Catch);

            if (!catchClause.TypeReference.IsNull)
            {
                _outputFormatter.Space();
                if (catchClause.VariableName.Length > 0)
                {
                    _outputFormatter.PrintIdentifier(catchClause.VariableName);
                }
                else
                {
                    _outputFormatter.PrintIdentifier("generatedExceptionName");
                }
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _outputFormatter.PrintIdentifier(catchClause.TypeReference.Type);
            }

            if (!catchClause.Condition.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.When);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(catchClause.Condition, data);
            }
            _outputFormatter.NewLine();

            PrintIndentedBlock(catchClause.StatementBlock);

            return null;
        }

        public object VisitThrowStatement(ThrowStatement throwStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Throw);
            if (!throwStatement.Expression.IsNull)
            {
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(throwStatement.Expression, data);
            }
            return null;
        }

        public object VisitFixedStatement(FixedStatement fixedStatement, object data)
        {
            UnsupportedNode(fixedStatement);
            return _nodeTracker.TrackedVisit(fixedStatement.EmbeddedStatement, data);
        }

        public object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
        {
            UnsupportedNode(unsafeStatement);
            return _nodeTracker.TrackedVisit(unsafeStatement.Block, data);
        }

        public object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
        {
            UnsupportedNode(checkedStatement);
            return _nodeTracker.TrackedVisit(checkedStatement.Block, data);
        }

        public object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
        {
            UnsupportedNode(uncheckedStatement);
            return _nodeTracker.TrackedVisit(uncheckedStatement.Block, data);
        }

        public object VisitExitStatement(ExitStatement exitStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Exit);
            if (exitStatement.ExitType != ExitType.None)
            {
                _outputFormatter.Space();
                switch (exitStatement.ExitType)
                {
                    case ExitType.Sub:
                        _outputFormatter.PrintToken(Tokens.Sub);
                        break;
                    case ExitType.Function:
                        _outputFormatter.PrintToken(Tokens.Function);
                        break;
                    case ExitType.Property:
                        _outputFormatter.PrintToken(Tokens.Property);
                        break;
                    case ExitType.Do:
                        _outputFormatter.PrintToken(Tokens.Do);
                        break;
                    case ExitType.For:
                        _outputFormatter.PrintToken(Tokens.For);
                        break;
                    case ExitType.Try:
                        _outputFormatter.PrintToken(Tokens.Try);
                        break;
                    case ExitType.While:
                        _outputFormatter.PrintToken(Tokens.While);
                        break;
                    case ExitType.Select:
                        _outputFormatter.PrintToken(Tokens.Select);
                        break;
                    default:
                        Error(String.Format("Unsupported exit type : {0}", exitStatement.ExitType), exitStatement.StartLocation);
                        break;
                }
            }

            return null;
        }

        public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
        {
            _exitTokenStack.Push(Tokens.For);
            _outputFormatter.PrintToken(Tokens.For);
            _outputFormatter.Space();

            _outputFormatter.PrintIdentifier(forNextStatement.VariableName);

            if (!forNextStatement.TypeReference.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(forNextStatement.TypeReference, data);
            }

            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Assign);
            _outputFormatter.Space();

            _nodeTracker.TrackedVisit(forNextStatement.Start, data);

            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.To);
            _outputFormatter.Space();

            _nodeTracker.TrackedVisit(forNextStatement.End, data);

            if (!forNextStatement.Step.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Step);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(forNextStatement.Step, data);
            }
            _outputFormatter.NewLine();

            PrintIndentedBlock(forNextStatement.EmbeddedStatement);

            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Next);

            if (forNextStatement.NextExpressions.Count > 0)
            {
                _outputFormatter.Space();
                AppendCommaSeparatedList(forNextStatement.NextExpressions);
            }
            _exitTokenStack.Pop();
            return null;
        }
        #endregion

        #region Expressions

        public object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.MyClass);
            return null;
        }


        private static string ConvertCharLiteral(char ch)
        {
            if (Char.IsControl(ch))
            {
                return "Chr(" + ((int)ch) + ")";
            }
            else
            {
                if (ch == '"')
                {
                    return "\"\"\"\"C";
                }
                return String.Concat("\"", ch.ToString(), "\"C");
            }
        }

        private static string ConvertString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in str)
            {
                if (char.IsControl(ch))
                {
                    sb.Append("\" & Chr(" + ((int)ch) + ") & \"");
                }
                else if (ch == '"')
                {
                    sb.Append("\"\"");
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        public object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
        {
            object val = primitiveExpression.Value;
            if (val == null)
            {
                _outputFormatter.PrintToken(Tokens.Nothing);
                return null;
            }
            if (val is bool)
            {
                if ((bool)primitiveExpression.Value)
                {
                    _outputFormatter.PrintToken(Tokens.True);
                }
                else
                {
                    _outputFormatter.PrintToken(Tokens.False);
                }
                return null;
            }

            if (val is string)
            {
                _outputFormatter.PrintText('"' + ConvertString((string)val) + '"');
                return null;
            }

            if (val is char)
            {
                _outputFormatter.PrintText(ConvertCharLiteral((char)primitiveExpression.Value));
                return null;
            }

            if (val is decimal)
            {
                _outputFormatter.PrintText(((decimal)primitiveExpression.Value).ToString(NumberFormatInfo.InvariantInfo) + "D");
                return null;
            }

            if (val is float)
            {
                _outputFormatter.PrintText(((float)primitiveExpression.Value).ToString(NumberFormatInfo.InvariantInfo) + "F");
                return null;
            }

            if (val is IFormattable)
            {
                _outputFormatter.PrintText(((IFormattable)val).ToString(null, NumberFormatInfo.InvariantInfo));
            }
            else
            {
                _outputFormatter.PrintText(val.ToString());
            }

            return null;
        }

        public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
        {
            int op = 0;
            switch (binaryOperatorExpression.Op)
            {
                case BinaryOperatorType.Add:
                    op = Tokens.Plus;
                    break;

                case BinaryOperatorType.Subtract:
                    op = Tokens.Minus;
                    break;

                case BinaryOperatorType.Multiply:
                    op = Tokens.Times;
                    break;

                case BinaryOperatorType.Divide:
                    op = Tokens.Div;
                    break;

                case BinaryOperatorType.Modulus:
                    op = Tokens.Mod;
                    break;

                case BinaryOperatorType.ShiftLeft:
                    op = Tokens.ShiftLeft;
                    break;

                case BinaryOperatorType.ShiftRight:
                    op = Tokens.ShiftRight;
                    break;

                case BinaryOperatorType.BitwiseAnd:
                    op = Tokens.And;
                    break;
                case BinaryOperatorType.BitwiseOr:
                    op = Tokens.Or;
                    break;
                case BinaryOperatorType.ExclusiveOr:
                    op = Tokens.Xor;
                    break;

                case BinaryOperatorType.LogicalAnd:
                    op = Tokens.AndAlso;
                    break;
                case BinaryOperatorType.LogicalOr:
                    op = Tokens.OrElse;
                    break;
                case BinaryOperatorType.ReferenceEquality:
                    op = Tokens.Is;
                    break;
                case BinaryOperatorType.ReferenceInequality:
                    op = Tokens.IsNot;
                    break;

                case BinaryOperatorType.Equality:
                    op = Tokens.Assign;
                    break;
                case BinaryOperatorType.GreaterThan:
                    op = Tokens.GreaterThan;
                    break;
                case BinaryOperatorType.GreaterThanOrEqual:
                    op = Tokens.GreaterEqual;
                    break;
                case BinaryOperatorType.InEquality:
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
                    _outputFormatter.Space();
                    _outputFormatter.PrintToken(Tokens.LessThan);
                    _outputFormatter.PrintToken(Tokens.GreaterThan);
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
                    return null;
                case BinaryOperatorType.NullCoalescing:
                    _outputFormatter.PrintText("IIf(");
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
                    _outputFormatter.PrintText(" Is Nothing, ");
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
                    _outputFormatter.PrintToken(Tokens.Comma);
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
                    _outputFormatter.PrintToken(Tokens.CloseParenthesis);
                    return null;
                case BinaryOperatorType.LessThan:
                    op = Tokens.LessThan;
                    break;
                case BinaryOperatorType.LessThanOrEqual:
                    op = Tokens.LessEqual;
                    break;
            }

            _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(op);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);

            return null;
        }

        public object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(parenthesizedExpression.Expression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitInvocationExpression(InvocationExpression invocationExpression, object data)
        {
            _nodeTracker.TrackedVisit(invocationExpression.TargetObject, data);
            if (invocationExpression.TypeArguments != null && invocationExpression.TypeArguments.Count > 0)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                _outputFormatter.PrintToken(Tokens.Of);
                _outputFormatter.Space();
                AppendCommaSeparatedList(invocationExpression.TypeArguments);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(invocationExpression.Arguments);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data)
        {
            _outputFormatter.PrintIdentifier(identifierExpression.Identifier);
            return null;
        }

        public object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data)
        {
            _nodeTracker.TrackedVisit(typeReferenceExpression.TypeReference, data);
            return null;
        }

        public object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data)
        {
            switch (unaryOperatorExpression.Op)
            {
                case UnaryOperatorType.Not:
                case UnaryOperatorType.BitNot:
                    _outputFormatter.PrintToken(Tokens.Not);
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    return null;

                case UnaryOperatorType.Decrement:
                    _outputFormatter.PrintText("System.Threading.Interlocked.Decrement(");
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintText(")");
                    return null;

                case UnaryOperatorType.Increment:
                    _outputFormatter.PrintText("System.Threading.Interlocked.Increment(");
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintText(")");
                    return null;

                case UnaryOperatorType.Minus:
                    _outputFormatter.PrintToken(Tokens.Minus);
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    return null;

                case UnaryOperatorType.Plus:
                    _outputFormatter.PrintToken(Tokens.Plus);
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    return null;

                case UnaryOperatorType.PostDecrement:
                    _outputFormatter.PrintText("System.Math.Max(System.Threading.Interlocked.Decrement(");
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintText("),");
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintText(" + 1)");
                    return null;

                case UnaryOperatorType.PostIncrement:
                    _outputFormatter.PrintText("System.Math.Max(System.Threading.Interlocked.Increment(");
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintText("),");
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintText(" - 1)");
                    return null;

                case UnaryOperatorType.Star:
                    _outputFormatter.PrintToken(Tokens.Times);
                    return null;
                case UnaryOperatorType.BitWiseAnd:
                    _outputFormatter.PrintToken(Tokens.AddressOf);
                    return null;
                default:
                    Error("unknown unary operator: " + unaryOperatorExpression.Op.ToString(), unaryOperatorExpression.StartLocation);
                    return null;
            }
        }

        public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
        {
            int op = 0;
            bool unsupportedOpAssignment = false;
            switch (assignmentExpression.Op)
            {
                case AssignmentOperatorType.Assign:
                    op = Tokens.Assign;
                    break;
                case AssignmentOperatorType.Add:
                    op = Tokens.PlusAssign;
                    if (IsEventHandlerCreation(assignmentExpression.Right))
                    {
                        _outputFormatter.PrintToken(Tokens.AddHandler);
                        _outputFormatter.Space();
                        _nodeTracker.TrackedVisit(assignmentExpression.Left, data);
                        _outputFormatter.PrintToken(Tokens.Comma);
                        _outputFormatter.Space();
                        _outputFormatter.PrintToken(Tokens.AddressOf);
                        _outputFormatter.Space();
                        _nodeTracker.TrackedVisit(GetEventHandlerMethod(assignmentExpression.Right), data);
                        return null;
                    }
                    break;
                case AssignmentOperatorType.Subtract:
                    op = Tokens.MinusAssign;
                    if (IsEventHandlerCreation(assignmentExpression.Right))
                    {
                        _outputFormatter.PrintToken(Tokens.RemoveHandler);
                        _outputFormatter.Space();
                        _nodeTracker.TrackedVisit(assignmentExpression.Left, data);
                        _outputFormatter.PrintToken(Tokens.Comma);
                        _outputFormatter.Space();
                        _outputFormatter.PrintToken(Tokens.AddressOf);
                        _outputFormatter.Space();
                        _nodeTracker.TrackedVisit(GetEventHandlerMethod(assignmentExpression.Right), data);
                        return null;
                    }
                    break;
                case AssignmentOperatorType.Multiply:
                    op = Tokens.TimesAssign;
                    break;
                case AssignmentOperatorType.Divide:
                    op = Tokens.DivAssign;
                    break;
                case AssignmentOperatorType.ShiftLeft:
                    op = Tokens.ShiftLeftAssign;
                    break;
                case AssignmentOperatorType.ShiftRight:
                    op = Tokens.ShiftRightAssign;
                    break;

                case AssignmentOperatorType.ExclusiveOr:
                    op = Tokens.Xor;
                    unsupportedOpAssignment = true;
                    break;
                case AssignmentOperatorType.Modulus:
                    op = Tokens.Mod;
                    unsupportedOpAssignment = true;
                    break;
                case AssignmentOperatorType.BitwiseAnd:
                    op = Tokens.And;
                    unsupportedOpAssignment = true;
                    break;
                case AssignmentOperatorType.BitwiseOr:
                    op = Tokens.Or;
                    unsupportedOpAssignment = true;
                    break;
            }

            _nodeTracker.TrackedVisit(assignmentExpression.Left, data);
            _outputFormatter.Space();

            if (unsupportedOpAssignment)
            { // left = left OP right
                _outputFormatter.PrintToken(Tokens.Assign);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(assignmentExpression.Left, data);
                _outputFormatter.Space();
            }

            _outputFormatter.PrintToken(op);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(assignmentExpression.Right, data);

            return null;
        }

        public object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
        {
            UnsupportedNode(sizeOfExpression);
            return null;
        }

        public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.GetType);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(typeOfExpression.TypeReference, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
        {
            // assigning nothing to a generic type in VB compiles to a DefaultValueExpression
            _outputFormatter.PrintToken(Tokens.Nothing);
            return null;
        }

        public object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.TypeOf);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(typeOfIsExpression.Expression, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Is);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(typeOfIsExpression.TypeReference, data);
            return null;
        }

        public object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.AddressOf);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(addressOfExpression.Expression, data);
            return null;
        }

        public object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
        {
            UnsupportedNode(anonymousMethodExpression);
            return null;
        }

        public object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
        {
            UnsupportedNode(checkedExpression);
            return _nodeTracker.TrackedVisit(checkedExpression.Expression, data);
        }

        public object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
        {
            UnsupportedNode(uncheckedExpression);
            return _nodeTracker.TrackedVisit(uncheckedExpression.Expression, data);
        }

        public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
        {
            UnsupportedNode(pointerReferenceExpression);
            return null;
        }

        public object VisitCastExpression(CastExpression castExpression, object data)
        {
            if (castExpression.CastType == CastType.Cast)
            {
                return PrintCast(Tokens.DirectCast, castExpression);
            }
            if (castExpression.CastType == CastType.TryCast)
            {
                return PrintCast(Tokens.TryCast, castExpression);
            }
            switch (castExpression.CastTo.SystemType)
            {
                case "System.Boolean":
                    _outputFormatter.PrintToken(Tokens.CBool);
                    break;
                case "System.Byte":
                    _outputFormatter.PrintToken(Tokens.CByte);
                    break;
                case "System.SByte":
                    _outputFormatter.PrintToken(Tokens.CSByte);
                    break;
                case "System.Char":
                    _outputFormatter.PrintToken(Tokens.CChar);
                    break;
                case "System.DateTime":
                    _outputFormatter.PrintToken(Tokens.CDate);
                    break;
                case "System.Decimal":
                    _outputFormatter.PrintToken(Tokens.CDec);
                    break;
                case "System.Double":
                    _outputFormatter.PrintToken(Tokens.CDbl);
                    break;
                case "System.Int16":
                    _outputFormatter.PrintToken(Tokens.CShort);
                    break;
                case "System.Int32":
                    _outputFormatter.PrintToken(Tokens.CInt);
                    break;
                case "System.Int64":
                    _outputFormatter.PrintToken(Tokens.CLng);
                    break;
                case "System.UInt16":
                    _outputFormatter.PrintToken(Tokens.CUShort);
                    break;
                case "System.UInt32":
                    _outputFormatter.PrintToken(Tokens.CInt);
                    break;
                case "System.UInt64":
                    _outputFormatter.PrintToken(Tokens.CLng);
                    break;
                case "System.Object":
                    _outputFormatter.PrintToken(Tokens.CObj);
                    break;
                case "System.Single":
                    _outputFormatter.PrintToken(Tokens.CSng);
                    break;
                case "System.String":
                    _outputFormatter.PrintToken(Tokens.CStr);
                    break;
                default:
                    return PrintCast(Tokens.CType, castExpression);
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(castExpression.Expression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        private object PrintCast(int castToken, CastExpression castExpression)
        {
            _outputFormatter.PrintToken(castToken);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(castExpression.Expression, null);
            _outputFormatter.PrintToken(Tokens.Comma);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(castExpression.CastTo, null);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
        {
            UnsupportedNode(stackAllocExpression);
            return null;
        }

        public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
        {
            _nodeTracker.TrackedVisit(indexerExpression.TargetObject, data);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(indexerExpression.Indexes);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Me);
            return null;
        }

        public object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.MyBase);
            return null;
        }

        public object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.New);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(objectCreateExpression.CreateType, data);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(objectCreateExpression.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.New);
            _outputFormatter.Space();
            PrintTypeReferenceWithoutArray(arrayCreateExpression.CreateType);

            if (arrayCreateExpression.Arguments.Count > 0)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                AppendCommaSeparatedList(arrayCreateExpression.Arguments);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
                PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 1);
            }
            else
            {
                PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 0);
            }

            _outputFormatter.Space();

            if (arrayCreateExpression.ArrayInitializer.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
                _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
            }
            else
            {
                _nodeTracker.TrackedVisit(arrayCreateExpression.ArrayInitializer, data);
            }
            return null;
        }

        public object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
            this.AppendCommaSeparatedList(arrayInitializerExpression.CreateExpressions);
            _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
            return null;
        }

        public object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
        {
            _nodeTracker.TrackedVisit(fieldReferenceExpression.TargetObject, data);
            _outputFormatter.PrintToken(Tokens.Dot);
            _outputFormatter.PrintIdentifier(fieldReferenceExpression.FieldName);
            return null;
        }

        public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
        {
            // VB does not need to specify the direction in method calls
            _nodeTracker.TrackedVisit(directionExpression.Expression, data);
            return null;
        }


        public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
        {
            // No representation in VB.NET, but VB conversion is possible.
            _outputFormatter.PrintText("IIf");
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(conditionalExpression.Condition, data);
            _outputFormatter.PrintToken(Tokens.Comma);
            _nodeTracker.TrackedVisit(conditionalExpression.TrueExpression, data);
            _outputFormatter.PrintToken(Tokens.Comma);
            _nodeTracker.TrackedVisit(conditionalExpression.FalseExpression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        #endregion
        #endregion


        private void OutputModifier(ParameterModifiers modifier, Location position)
        {
            switch (modifier)
            {
                case ParameterModifiers.None:
                case ParameterModifiers.In:
                    _outputFormatter.PrintToken(Tokens.ByVal);
                    break;
                case ParameterModifiers.Out:
                    Error("Out parameter converted to ByRef", position);
                    _outputFormatter.PrintToken(Tokens.ByRef);
                    break;
                case ParameterModifiers.Params:
                    _outputFormatter.PrintToken(Tokens.ParamArray);
                    break;
                case ParameterModifiers.Ref:
                    _outputFormatter.PrintToken(Tokens.ByRef);
                    break;
                case ParameterModifiers.Optional:
                    _outputFormatter.PrintToken(Tokens.Optional);
                    break;
                default:
                    Error(String.Format("Unsupported modifier : {0}", modifier), position);
                    break;
            }
            _outputFormatter.Space();
        }

        private void OutputModifier(Modifiers modifier)
        {
            OutputModifier(modifier, false);
        }

        private void OutputModifier(Modifiers modifier, bool forTypeDecl)
        {
            if ((modifier & Modifiers.Public) == Modifiers.Public)
            {
                _outputFormatter.PrintToken(Tokens.Public);
                _outputFormatter.Space();
            }
            else if ((modifier & Modifiers.Private) == Modifiers.Private)
            {
                _outputFormatter.PrintToken(Tokens.Private);
                _outputFormatter.Space();
            }
            else if ((modifier & (Modifiers.Protected | Modifiers.Internal)) == (Modifiers.Protected | Modifiers.Internal))
            {
                _outputFormatter.PrintToken(Tokens.Protected);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Friend);
                _outputFormatter.Space();
            }
            else if ((modifier & Modifiers.Internal) == Modifiers.Internal)
            {
                _outputFormatter.PrintToken(Tokens.Friend);
                _outputFormatter.Space();
            }
            else if ((modifier & Modifiers.Protected) == Modifiers.Protected)
            {
                _outputFormatter.PrintToken(Tokens.Protected);
                _outputFormatter.Space();
            }

            if ((modifier & Modifiers.Static) == Modifiers.Static)
            {
                _outputFormatter.PrintToken(Tokens.Shared);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.Virtual) == Modifiers.Virtual)
            {
                _outputFormatter.PrintToken(Tokens.Overridable);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.Abstract) == Modifiers.Abstract)
            {
                _outputFormatter.PrintToken(forTypeDecl ? Tokens.MustInherit : Tokens.MustOverride);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.Override) == Modifiers.Override)
            {
                _outputFormatter.PrintToken(Tokens.Overloads);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Overrides);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.New) == Modifiers.New)
            {
                _outputFormatter.PrintToken(Tokens.Shadows);
                _outputFormatter.Space();
            }

            if ((modifier & Modifiers.Sealed) == Modifiers.Sealed)
            {
                _outputFormatter.PrintToken(forTypeDecl ? Tokens.NotInheritable : Tokens.NotOverridable);
                _outputFormatter.Space();
            }

            if ((modifier & Modifiers.ReadOnly) == Modifiers.ReadOnly)
            {
                _outputFormatter.PrintToken(Tokens.ReadOnly);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.WriteOnly) == Modifiers.WriteOnly)
            {
                _outputFormatter.PrintToken(Tokens.WriteOnly);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.Const) == Modifiers.Const)
            {
                _outputFormatter.PrintToken(Tokens.Const);
                _outputFormatter.Space();
            }
            if ((modifier & Modifiers.Partial) == Modifiers.Partial)
            {
                _outputFormatter.PrintToken(Tokens.Partial);
                _outputFormatter.Space();
            }

            if ((modifier & Modifiers.Extern) == Modifiers.Extern)
            {
                // not required in VB
            }

            // TODO : Volatile
            if ((modifier & Modifiers.Volatile) == Modifiers.Volatile)
            {
                Error("'Volatile' modifier not convertable", Location.Empty);
            }

            // TODO : Unsafe
            if ((modifier & Modifiers.Unsafe) == Modifiers.Unsafe)
            {
                Error("'Unsafe' modifier not convertable", Location.Empty);
            }
        }

        public void AppendCommaSeparatedList<T>(ICollection<T> list) where T : class, INode
        {
            if (list != null)
            {
                int i = 0;
                foreach (T node in list)
                {
                    _nodeTracker.TrackedVisit(node, null);
                    if (i + 1 < list.Count)
                    {
                        _outputFormatter.PrintToken(Tokens.Comma);
                        _outputFormatter.Space();
                        if ((i + 1) % 6 == 0)
                        {
                            _outputFormatter.PrintLineContinuation();
                            _outputFormatter.Indent();
                            _outputFormatter.PrintText("\t");
                        }
                    }
                    i++;
                }
            }
        }

        private void VisitAttributes(ICollection attributes, object data)
        {
            if (attributes == null || attributes.Count <= 0)
            {
                return;
            }
            foreach (AttributeSection section in attributes)
            {
                _nodeTracker.TrackedVisit(section, data);
            }
        }


        private static bool IsEventHandlerCreation(Expression expr)
        {
            if (expr is ObjectCreateExpression)
            {
                ObjectCreateExpression oce = (ObjectCreateExpression)expr;
                if (oce.Parameters.Count == 1)
                {
                    return oce.CreateType.SystemType.EndsWith("Handler");
                }
            }
            return false;
        }

        // can only get called if IsEventHandlerCreation returned true for the expression
        private static Expression GetEventHandlerMethod(Expression expr)
        {
            ObjectCreateExpression oce = (ObjectCreateExpression)expr;
            return oce.Parameters[0];
        }
    }
}
