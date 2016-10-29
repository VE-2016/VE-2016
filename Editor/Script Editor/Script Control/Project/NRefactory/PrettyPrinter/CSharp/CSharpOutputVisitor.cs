// <file>
//     
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2522 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Parser;
using AIMS.Libraries.Scripting.NRefactory.Parser.CSharp;

namespace AIMS.Libraries.Scripting.NRefactory.PrettyPrinter
{
    public sealed class CSharpOutputVisitor : IOutputAstVisitor
    {
        private Errors _errors = new Errors();
        private CSharpOutputFormatter _outputFormatter;
        private PrettyPrintOptions _prettyPrintOptions = new PrettyPrintOptions();
        private NodeTracker _nodeTracker;
        private bool _printFullSystemType;

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

        public PrettyPrintOptions Options
        {
            get { return _prettyPrintOptions; }
        }

        public IOutputFormatter OutputFormatter
        {
            get
            {
                return _outputFormatter;
            }
        }

        public NodeTracker NodeTracker
        {
            get
            {
                return _nodeTracker;
            }
        }

        public CSharpOutputVisitor()
        {
            _outputFormatter = new CSharpOutputFormatter(_prettyPrintOptions);
            _nodeTracker = new NodeTracker(this);
        }

        private void Error(INode node, string message)
        {
            _outputFormatter.PrintText(" // ERROR: " + message + Environment.NewLine);
            _errors.Error(node.StartLocation.Y, node.StartLocation.X, message);
        }

        private void NotSupported(INode node)
        {
            Error(node, "Not supported in C#: " + node.GetType().Name);
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
            if (TypeReference.PrimitiveTypesCSharpReverse.TryGetValue(typeString, out primitiveType))
                return primitiveType;
            else
                return typeString;
        }

        private void PrintTemplates(List<TemplateDefinition> templates)
        {
            if (templates.Count == 0) return;
            _outputFormatter.PrintToken(Tokens.LessThan);
            for (int i = 0; i < templates.Count; i++)
            {
                if (i > 0) PrintFormattedComma();
                _outputFormatter.PrintIdentifier(templates[i].Name);
            }
            _outputFormatter.PrintToken(Tokens.GreaterThan);
        }

        public object VisitTypeReference(TypeReference typeReference, object data)
        {
            if (typeReference == TypeReference.ClassConstraint)
            {
                _outputFormatter.PrintToken(Tokens.Class);
            }
            else if (typeReference == TypeReference.StructConstraint)
            {
                _outputFormatter.PrintToken(Tokens.Struct);
            }
            else if (typeReference == TypeReference.NewConstraint)
            {
                _outputFormatter.PrintToken(Tokens.New);
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
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

        private void PrintArrayRank(int[] rankSpecifier, int startRankIndex)
        {
            for (int i = startRankIndex; i < rankSpecifier.Length; ++i)
            {
                _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
                if (_prettyPrintOptions.SpacesWithinBrackets)
                {
                    _outputFormatter.Space();
                }
                for (int j = 0; j < rankSpecifier[i]; ++j)
                {
                    _outputFormatter.PrintToken(Tokens.Comma);
                }
                if (_prettyPrintOptions.SpacesWithinBrackets)
                {
                    _outputFormatter.Space();
                }
                _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
            }
        }

        private void PrintTypeReferenceWithoutArray(TypeReference typeReference)
        {
            if (typeReference.IsGlobal)
            {
                _outputFormatter.PrintText("global::");
            }
            if (typeReference.Type == null || typeReference.Type.Length == 0)
            {
                _outputFormatter.PrintText("void");
            }
            else if (typeReference.SystemType == "System.Nullable" && typeReference.GenericTypes != null
                     && typeReference.GenericTypes.Count == 1 && !typeReference.IsGlobal)
            {
                _nodeTracker.TrackedVisit(typeReference.GenericTypes[0], null);
                _outputFormatter.PrintText("?");
            }
            else
            {
                if (typeReference.SystemType.Length > 0)
                {
                    if (_printFullSystemType || typeReference.IsGlobal)
                    {
                        _outputFormatter.PrintIdentifier(typeReference.SystemType);
                    }
                    else
                    {
                        _outputFormatter.PrintText(ConvertTypeString(typeReference.SystemType));
                    }
                }
                else
                {
                    _outputFormatter.PrintText(typeReference.Type);
                }
                if (typeReference.GenericTypes != null && typeReference.GenericTypes.Count > 0)
                {
                    _outputFormatter.PrintToken(Tokens.LessThan);
                    AppendCommaSeparatedList(typeReference.GenericTypes);
                    _outputFormatter.PrintToken(Tokens.GreaterThan);
                }
            }
            for (int i = 0; i < typeReference.PointerNestingLevel; ++i)
            {
                _outputFormatter.PrintToken(Tokens.Times);
            }
        }

        public object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data)
        {
            _nodeTracker.TrackedVisit(innerClassTypeReference.BaseType, data);
            _outputFormatter.PrintToken(Tokens.Dot);
            return VisitTypeReference((TypeReference)innerClassTypeReference, data);
        }

        #region Global scope
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
        private void PrintFormattedComma()
        {
            if (_prettyPrintOptions.SpacesBeforeComma)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.Comma);
            if (_prettyPrintOptions.SpacesAfterComma)
            {
                _outputFormatter.Space();
            }
        }

        public object VisitAttributeSection(AttributeSection attributeSection, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            if (!string.IsNullOrEmpty(attributeSection.AttributeTarget))
            {
                _outputFormatter.PrintText(attributeSection.AttributeTarget);
                _outputFormatter.PrintToken(Tokens.Colon);
                _outputFormatter.Space();
            }
            Debug.Assert(attributeSection.Attributes != null);
            for (int j = 0; j < attributeSection.Attributes.Count; ++j)
            {
                _nodeTracker.TrackedVisit((INode)attributeSection.Attributes[j], data);
                if (j + 1 < attributeSection.Attributes.Count)
                {
                    PrintFormattedComma();
                }
            }
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitAttribute(AIMS.Libraries.Scripting.NRefactory.Ast.Attribute attribute, object data)
        {
            _outputFormatter.PrintIdentifier(attribute.Name);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            this.AppendCommaSeparatedList(attribute.PositionalArguments);

            if (attribute.NamedArguments != null && attribute.NamedArguments.Count > 0)
            {
                if (attribute.PositionalArguments.Count > 0)
                {
                    PrintFormattedComma();
                }
                for (int i = 0; i < attribute.NamedArguments.Count; ++i)
                {
                    _nodeTracker.TrackedVisit((INode)attribute.NamedArguments[i], data);
                    if (i + 1 < attribute.NamedArguments.Count)
                    {
                        PrintFormattedComma();
                    }
                }
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data)
        {
            _outputFormatter.PrintIdentifier(namedArgumentExpression.Name);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Assign);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(namedArgumentExpression.Expression, data);
            return null;
        }

        public object VisitUsing(Using @using, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Using);
            _outputFormatter.Space();

            _outputFormatter.PrintIdentifier(@using.Name);

            if (@using.IsAlias)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Assign);
                _outputFormatter.Space();
                _printFullSystemType = true;
                _nodeTracker.TrackedVisit(@using.Alias, data);
                _printFullSystemType = false;
            }

            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data)
        {
            foreach (Using u in usingDeclaration.Usings)
            {
                _nodeTracker.TrackedVisit(u, data);
            }
            return null;
        }

        public object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Namespace);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(namespaceDeclaration.Name);

            _outputFormatter.BeginBrace(_prettyPrintOptions.NamespaceBraceStyle);

            _nodeTracker.TrackedVisitChildren(namespaceDeclaration, data);

            _outputFormatter.EndBrace();

            return null;
        }


        private void OutputEnumMembers(TypeDeclaration typeDeclaration, object data)
        {
            for (int i = 0; i < typeDeclaration.Children.Count; i++)
            {
                FieldDeclaration fieldDeclaration = (FieldDeclaration)typeDeclaration.Children[i];
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
                if (i < typeDeclaration.Children.Count - 1)
                {
                    _outputFormatter.PrintToken(Tokens.Comma);
                }
                _outputFormatter.NewLine();
                _nodeTracker.EndNode(fieldDeclaration);
            }
        }

        private TypeDeclaration _currentType = null;

        public object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
        {
            VisitAttributes(typeDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(typeDeclaration.Modifier);
            switch (typeDeclaration.Type)
            {
                case ClassType.Enum:
                    _outputFormatter.PrintToken(Tokens.Enum);
                    break;
                case ClassType.Interface:
                    _outputFormatter.PrintToken(Tokens.Interface);
                    break;
                case ClassType.Struct:
                    _outputFormatter.PrintToken(Tokens.Struct);
                    break;
                default:
                    _outputFormatter.PrintToken(Tokens.Class);
                    break;
            }
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(typeDeclaration.Name);

            PrintTemplates(typeDeclaration.Templates);

            if (typeDeclaration.BaseTypes != null && typeDeclaration.BaseTypes.Count > 0)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Colon);
                _outputFormatter.Space();
                for (int i = 0; i < typeDeclaration.BaseTypes.Count; ++i)
                {
                    if (i > 0)
                    {
                        PrintFormattedComma();
                    }
                    _nodeTracker.TrackedVisit(typeDeclaration.BaseTypes[i], data);
                }
            }

            foreach (TemplateDefinition templateDefinition in typeDeclaration.Templates)
            {
                _nodeTracker.TrackedVisit(templateDefinition, data);
            }

            switch (typeDeclaration.Type)
            {
                case ClassType.Enum:
                    _outputFormatter.BeginBrace(_prettyPrintOptions.EnumBraceStyle);
                    break;
                case ClassType.Interface:
                    _outputFormatter.BeginBrace(_prettyPrintOptions.InterfaceBraceStyle);
                    break;
                case ClassType.Struct:
                    _outputFormatter.BeginBrace(_prettyPrintOptions.StructBraceStyle);
                    break;
                default:
                    _outputFormatter.BeginBrace(_prettyPrintOptions.ClassBraceStyle);
                    break;
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
            _outputFormatter.EndBrace();

            return null;
        }

        public object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data)
        {
            if (templateDefinition.Bases.Count == 0)
                return null;

            _outputFormatter.Space();
            _outputFormatter.PrintText("where");
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(templateDefinition.Name);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Colon);
            _outputFormatter.Space();

            for (int i = 0; i < templateDefinition.Bases.Count; ++i)
            {
                _nodeTracker.TrackedVisit(templateDefinition.Bases[i], data);
                if (i + 1 < templateDefinition.Bases.Count)
                {
                    PrintFormattedComma();
                }
            }
            return null;
        }

        public object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data)
        {
            VisitAttributes(delegateDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(delegateDeclaration.Modifier);
            _outputFormatter.PrintToken(Tokens.Delegate);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(delegateDeclaration.ReturnType, data);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(delegateDeclaration.Name);
            PrintTemplates(delegateDeclaration.Templates);
            if (_prettyPrintOptions.BeforeDelegateDeclarationParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(delegateDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            foreach (TemplateDefinition templateDefinition in delegateDeclaration.Templates)
            {
                _nodeTracker.TrackedVisit(templateDefinition, data);
            }
            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data)
        {
            NotSupported(optionDeclaration);
            return null;
        }
        #endregion

        #region Type level
        public object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data)
        {
            if (!fieldDeclaration.TypeReference.IsNull)
            {
                VisitAttributes(fieldDeclaration.Attributes, data);
                _outputFormatter.Indent();
                OutputModifier(fieldDeclaration.Modifier);
                _nodeTracker.TrackedVisit(fieldDeclaration.TypeReference, data);
                _outputFormatter.Space();
                AppendCommaSeparatedList(fieldDeclaration.Fields);
                _outputFormatter.PrintToken(Tokens.Semicolon);
                _outputFormatter.NewLine();
            }
            else
            {
                for (int i = 0; i < fieldDeclaration.Fields.Count; i++)
                {
                    VisitAttributes(fieldDeclaration.Attributes, data);
                    _outputFormatter.Indent();
                    OutputModifier(fieldDeclaration.Modifier);
                    _nodeTracker.TrackedVisit(fieldDeclaration.GetTypeForField(i), data);
                    _outputFormatter.Space();
                    _nodeTracker.TrackedVisit(fieldDeclaration.Fields[i], data);
                    _outputFormatter.PrintToken(Tokens.Semicolon);
                    _outputFormatter.NewLine();
                }
            }
            return null;
        }

        public object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data)
        {
            _outputFormatter.PrintIdentifier(variableDeclaration.Name);
            if (!variableDeclaration.FixedArrayInitialization.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
                _nodeTracker.TrackedVisit(variableDeclaration.FixedArrayInitialization, data);
                _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
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
            propertyDeclaration.Modifier &= ~Modifiers.ReadOnly;
            OutputModifier(propertyDeclaration.Modifier);
            _nodeTracker.TrackedVisit(propertyDeclaration.TypeReference, data);
            _outputFormatter.Space();
            if (propertyDeclaration.InterfaceImplementations.Count > 0)
            {
                _nodeTracker.TrackedVisit(propertyDeclaration.InterfaceImplementations[0].InterfaceType, data);
                _outputFormatter.PrintToken(Tokens.Dot);
            }
            _outputFormatter.PrintIdentifier(propertyDeclaration.Name);

            _outputFormatter.BeginBrace(_prettyPrintOptions.PropertyBraceStyle);

            _nodeTracker.TrackedVisit(propertyDeclaration.GetRegion, data);
            _nodeTracker.TrackedVisit(propertyDeclaration.SetRegion, data);

            _outputFormatter.EndBrace();
            return null;
        }

        public object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data)
        {
            this.VisitAttributes(propertyGetRegion.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(propertyGetRegion.Modifier);
            _outputFormatter.PrintText("get");
            OutputBlockAllowInline(propertyGetRegion.Block, _prettyPrintOptions.PropertyGetBraceStyle);
            return null;
        }

        public object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data)
        {
            this.VisitAttributes(propertySetRegion.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(propertySetRegion.Modifier);
            _outputFormatter.PrintText("set");
            OutputBlockAllowInline(propertySetRegion.Block, _prettyPrintOptions.PropertySetBraceStyle);
            return null;
        }

        public object VisitEventDeclaration(EventDeclaration eventDeclaration, object data)
        {
            VisitAttributes(eventDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(eventDeclaration.Modifier);
            _outputFormatter.PrintToken(Tokens.Event);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(eventDeclaration.TypeReference, data);
            _outputFormatter.Space();

            if (eventDeclaration.InterfaceImplementations.Count > 0)
            {
                _nodeTracker.TrackedVisit(eventDeclaration.InterfaceImplementations[0].InterfaceType, data);
                _outputFormatter.PrintToken(Tokens.Dot);
            }

            _outputFormatter.PrintIdentifier(eventDeclaration.Name);

            if (!eventDeclaration.Initializer.IsNull)
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Assign);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(eventDeclaration.Initializer, data);
            }

            if (eventDeclaration.AddRegion.IsNull && eventDeclaration.RemoveRegion.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.Semicolon);
                _outputFormatter.NewLine();
            }
            else
            {
                _outputFormatter.BeginBrace(_prettyPrintOptions.PropertyBraceStyle);
                _nodeTracker.TrackedVisit(eventDeclaration.AddRegion, data);
                _nodeTracker.TrackedVisit(eventDeclaration.RemoveRegion, data);
                _outputFormatter.EndBrace();
            }
            return null;
        }

        public object VisitEventAddRegion(EventAddRegion eventAddRegion, object data)
        {
            VisitAttributes(eventAddRegion.Attributes, data);
            _outputFormatter.Indent();
            _outputFormatter.PrintText("add");
            OutputBlockAllowInline(eventAddRegion.Block, _prettyPrintOptions.EventAddBraceStyle);
            return null;
        }

        public object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data)
        {
            VisitAttributes(eventRemoveRegion.Attributes, data);
            _outputFormatter.Indent();
            _outputFormatter.PrintText("remove");
            OutputBlockAllowInline(eventRemoveRegion.Block, _prettyPrintOptions.EventRemoveBraceStyle);
            return null;
        }

        public object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data)
        {
            // VB.NET only
            NotSupported(eventRaiseRegion);
            return null;
        }

        public object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
        {
            VisitAttributes(parameterDeclarationExpression.Attributes, data);
            OutputModifier(parameterDeclarationExpression.ParamModifier, parameterDeclarationExpression);
            _nodeTracker.TrackedVisit(parameterDeclarationExpression.TypeReference, data);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(parameterDeclarationExpression.ParameterName);
            return null;
        }

        public object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
        {
            VisitAttributes(methodDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(methodDeclaration.Modifier);
            _nodeTracker.TrackedVisit(methodDeclaration.TypeReference, data);
            _outputFormatter.Space();
            if (methodDeclaration.InterfaceImplementations.Count > 0)
            {
                _nodeTracker.TrackedVisit(methodDeclaration.InterfaceImplementations[0].InterfaceType, data);
                _outputFormatter.PrintToken(Tokens.Dot);
            }
            _outputFormatter.PrintIdentifier(methodDeclaration.Name);

            PrintMethodDeclaration(methodDeclaration);
            return null;
        }

        private void PrintMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            PrintTemplates(methodDeclaration.Templates);
            if (_prettyPrintOptions.BeforeMethodDeclarationParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(methodDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            foreach (TemplateDefinition templateDefinition in methodDeclaration.Templates)
            {
                _nodeTracker.TrackedVisit(templateDefinition, null);
            }
            OutputBlock(methodDeclaration.Body, _prettyPrintOptions.MethodBraceStyle);
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
                    _outputFormatter.PrintToken(Tokens.Implicit);
                }
                else
                {
                    _outputFormatter.PrintToken(Tokens.Explicit);
                }
            }
            else
            {
                _nodeTracker.TrackedVisit(operatorDeclaration.TypeReference, data);
            }
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Operator);
            _outputFormatter.Space();

            if (operatorDeclaration.IsConversionOperator)
            {
                _nodeTracker.TrackedVisit(operatorDeclaration.TypeReference, data);
            }
            else
            {
                switch (operatorDeclaration.OverloadableOperator)
                {
                    case OverloadableOperatorType.Add:
                        _outputFormatter.PrintToken(Tokens.Plus);
                        break;
                    case OverloadableOperatorType.BitNot:
                        _outputFormatter.PrintToken(Tokens.BitwiseComplement);
                        break;
                    case OverloadableOperatorType.BitwiseAnd:
                        _outputFormatter.PrintToken(Tokens.BitwiseAnd);
                        break;
                    case OverloadableOperatorType.BitwiseOr:
                        _outputFormatter.PrintToken(Tokens.BitwiseOr);
                        break;
                    case OverloadableOperatorType.Concat:
                        _outputFormatter.PrintToken(Tokens.Plus);
                        break;
                    case OverloadableOperatorType.Decrement:
                        _outputFormatter.PrintToken(Tokens.Decrement);
                        break;
                    case OverloadableOperatorType.Divide:
                    case OverloadableOperatorType.DivideInteger:
                        _outputFormatter.PrintToken(Tokens.Div);
                        break;
                    case OverloadableOperatorType.Equality:
                        _outputFormatter.PrintToken(Tokens.Equal);
                        break;
                    case OverloadableOperatorType.ExclusiveOr:
                        _outputFormatter.PrintToken(Tokens.Xor);
                        break;
                    case OverloadableOperatorType.GreaterThan:
                        _outputFormatter.PrintToken(Tokens.GreaterThan);
                        break;
                    case OverloadableOperatorType.GreaterThanOrEqual:
                        _outputFormatter.PrintToken(Tokens.GreaterEqual);
                        break;
                    case OverloadableOperatorType.Increment:
                        _outputFormatter.PrintToken(Tokens.Increment);
                        break;
                    case OverloadableOperatorType.InEquality:
                        _outputFormatter.PrintToken(Tokens.NotEqual);
                        break;
                    case OverloadableOperatorType.IsTrue:
                        _outputFormatter.PrintToken(Tokens.True);
                        break;
                    case OverloadableOperatorType.IsFalse:
                        _outputFormatter.PrintToken(Tokens.False);
                        break;
                    case OverloadableOperatorType.LessThan:
                        _outputFormatter.PrintToken(Tokens.LessThan);
                        break;
                    case OverloadableOperatorType.LessThanOrEqual:
                        _outputFormatter.PrintToken(Tokens.LessEqual);
                        break;
                    case OverloadableOperatorType.Like:
                        _outputFormatter.PrintText("Like");
                        break;
                    case OverloadableOperatorType.Modulus:
                        _outputFormatter.PrintToken(Tokens.Mod);
                        break;
                    case OverloadableOperatorType.Multiply:
                        _outputFormatter.PrintToken(Tokens.Times);
                        break;
                    case OverloadableOperatorType.Not:
                        _outputFormatter.PrintToken(Tokens.Not);
                        break;
                    case OverloadableOperatorType.Power:
                        _outputFormatter.PrintText("Power");
                        break;
                    case OverloadableOperatorType.ShiftLeft:
                        _outputFormatter.PrintToken(Tokens.ShiftLeft);
                        break;
                    case OverloadableOperatorType.ShiftRight:
                        _outputFormatter.PrintToken(Tokens.GreaterThan);
                        _outputFormatter.PrintToken(Tokens.GreaterThan);
                        break;
                    case OverloadableOperatorType.Subtract:
                        _outputFormatter.PrintToken(Tokens.Minus);
                        break;
                    default:
                        Error(operatorDeclaration, operatorDeclaration.OverloadableOperator.ToString() + " is not supported as overloadable operator");
                        break;
                }
            }

            PrintMethodDeclaration(operatorDeclaration);
            return null;
        }

        public object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data)
        {
            throw new InvalidOperationException();
        }

        public object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data)
        {
            VisitAttributes(constructorDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(constructorDeclaration.Modifier);
            if (_currentType != null)
            {
                _outputFormatter.PrintIdentifier(_currentType.Name);
            }
            else
            {
                _outputFormatter.PrintIdentifier(constructorDeclaration.Name);
            }
            if (_prettyPrintOptions.BeforeConstructorDeclarationParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(constructorDeclaration.Parameters);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _nodeTracker.TrackedVisit(constructorDeclaration.ConstructorInitializer, data);
            OutputBlock(constructorDeclaration.Body, _prettyPrintOptions.ConstructorBraceStyle);
            return null;
        }

        public object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data)
        {
            if (constructorInitializer.IsNull)
            {
                return null;
            }
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Colon);
            _outputFormatter.Space();
            if (constructorInitializer.ConstructorInitializerType == ConstructorInitializerType.Base)
            {
                _outputFormatter.PrintToken(Tokens.Base);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.This);
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            AppendCommaSeparatedList(constructorInitializer.Arguments);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data)
        {
            VisitAttributes(indexerDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(indexerDeclaration.Modifier);
            _nodeTracker.TrackedVisit(indexerDeclaration.TypeReference, data);
            _outputFormatter.Space();
            if (indexerDeclaration.InterfaceImplementations.Count > 0)
            {
                _nodeTracker.TrackedVisit(indexerDeclaration.InterfaceImplementations[0].InterfaceType, data);
                _outputFormatter.PrintToken(Tokens.Dot);
            }
            _outputFormatter.PrintToken(Tokens.This);
            _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            AppendCommaSeparatedList(indexerDeclaration.Parameters);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
            _outputFormatter.NewLine();
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
            _outputFormatter.NewLine();
            ++_outputFormatter.IndentationLevel;
            _nodeTracker.TrackedVisit(indexerDeclaration.GetRegion, data);
            _nodeTracker.TrackedVisit(indexerDeclaration.SetRegion, data);
            --_outputFormatter.IndentationLevel;
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data)
        {
            VisitAttributes(destructorDeclaration.Attributes, data);
            _outputFormatter.Indent();
            OutputModifier(destructorDeclaration.Modifier);
            _outputFormatter.PrintToken(Tokens.BitwiseComplement);
            if (_currentType != null)
                _outputFormatter.PrintIdentifier(_currentType.Name);
            else
                _outputFormatter.PrintIdentifier(destructorDeclaration.Name);
            if (_prettyPrintOptions.BeforeConstructorDeclarationParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            OutputBlock(destructorDeclaration.Body, _prettyPrintOptions.DestructorBraceStyle);
            return null;
        }

        public object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data)
        {
            NotSupported(declareDeclaration);
            return null;
        }
        #endregion

        #region Statements

        private void OutputBlock(BlockStatement blockStatement, BraceStyle braceStyle)
        {
            _nodeTracker.BeginNode(blockStatement);
            if (blockStatement.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.Semicolon);
                _outputFormatter.NewLine();
            }
            else
            {
                _outputFormatter.BeginBrace(braceStyle);
                foreach (Statement stmt in blockStatement.Children)
                {
                    _outputFormatter.Indent();
                    if (stmt is BlockStatement)
                    {
                        _nodeTracker.TrackedVisit(stmt, BraceStyle.EndOfLine);
                    }
                    else
                    {
                        _nodeTracker.TrackedVisit(stmt, null);
                    }
                    if (!_outputFormatter.LastCharacterIsNewLine)
                        _outputFormatter.NewLine();
                }
                _outputFormatter.EndBrace();
            }
            _nodeTracker.EndNode(blockStatement);
        }

        private void OutputBlockAllowInline(BlockStatement blockStatement, BraceStyle braceStyle)
        {
            OutputBlockAllowInline(blockStatement, braceStyle, true);
        }

        private void OutputBlockAllowInline(BlockStatement blockStatement, BraceStyle braceStyle, bool useNewLine)
        {
            if (!blockStatement.IsNull
                && (
                    blockStatement.Children.Count == 0
                    || blockStatement.Children.Count == 1
                    && (blockStatement.Children[0] is ExpressionStatement
                        || blockStatement.Children[0] is ReturnStatement
                       )))
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
                _outputFormatter.Space();
                if (blockStatement.Children.Count != 0)
                {
                    bool doIndent = _outputFormatter.DoIndent;
                    bool doNewLine = _outputFormatter.DoNewLine;
                    _outputFormatter.DoIndent = false;
                    _outputFormatter.DoNewLine = false;

                    _nodeTracker.TrackedVisit(blockStatement.Children[0], null);

                    _outputFormatter.DoIndent = doIndent;
                    _outputFormatter.DoNewLine = doNewLine;

                    _outputFormatter.Space();
                }
                _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
                if (useNewLine)
                {
                    _outputFormatter.NewLine();
                }
            }
            else
            {
                OutputBlock(blockStatement, braceStyle);
            }
        }

        public object VisitBlockStatement(BlockStatement blockStatement, object data)
        {
            if (_outputFormatter.TextLength == 0)
            {
                // we are outputting only a code block:
                // do not output braces, just the block's contents
                foreach (Statement stmt in blockStatement.Children)
                {
                    _outputFormatter.Indent();
                    _nodeTracker.TrackedVisit(stmt, null);
                    if (!_outputFormatter.LastCharacterIsNewLine)
                        _outputFormatter.NewLine();
                }
                return null;
            }

            if (data is BraceStyle)
                OutputBlock(blockStatement, (BraceStyle)data);
            else
                OutputBlock(blockStatement, BraceStyle.NextLine);
            return null;
        }

        public object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data)
        {
            _nodeTracker.TrackedVisit(addHandlerStatement.EventExpression, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.PlusAssign);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(addHandlerStatement.HandlerExpression, data);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data)
        {
            _nodeTracker.TrackedVisit(removeHandlerStatement.EventExpression, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.MinusAssign);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(removeHandlerStatement.HandlerExpression, data);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.If);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.NotEqual);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Null);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            _outputFormatter.BeginBrace(BraceStyle.EndOfLine);

            _outputFormatter.Indent();
            _outputFormatter.PrintIdentifier(raiseEventStatement.EventName);
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            this.AppendCommaSeparatedList(raiseEventStatement.Arguments);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.PrintToken(Tokens.Semicolon);

            _outputFormatter.NewLine();
            _outputFormatter.EndBrace();

            return null;
        }

        public object VisitEraseStatement(EraseStatement eraseStatement, object data)
        {
            for (int i = 0; i < eraseStatement.Expressions.Count; i++)
            {
                if (i > 0)
                {
                    _outputFormatter.NewLine();
                    _outputFormatter.Indent();
                }
                _nodeTracker.TrackedVisit(eraseStatement.Expressions[i], data);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Assign);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.Null);
                _outputFormatter.PrintToken(Tokens.Semicolon);
            }
            return null;
        }

        public object VisitErrorStatement(ErrorStatement errorStatement, object data)
        {
            NotSupported(errorStatement);
            return null;
        }

        public object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data)
        {
            NotSupported(onErrorStatement);
            return null;
        }

        public object VisitReDimStatement(ReDimStatement reDimStatement, object data)
        {
            NotSupported(reDimStatement);
            return null;
        }

        public object VisitExpressionStatement(ExpressionStatement expressionStatement, object data)
        {
            _nodeTracker.TrackedVisit(expressionStatement.Expression, data);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
        {
            for (int i = 0; i < localVariableDeclaration.Variables.Count; ++i)
            {
                VariableDeclaration v = (VariableDeclaration)localVariableDeclaration.Variables[i];
                if (i > 0)
                {
                    _outputFormatter.NewLine();
                    _outputFormatter.Indent();
                }
                OutputModifier(localVariableDeclaration.Modifier);
                _nodeTracker.TrackedVisit(localVariableDeclaration.GetTypeForVariable(i) ?? new TypeReference("object"), data);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(v, data);
                _outputFormatter.PrintToken(Tokens.Semicolon);
            }
            return null;
        }

        public object VisitEmptyStatement(EmptyStatement emptyStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitYieldStatement(YieldStatement yieldStatement, object data)
        {
            Debug.Assert(yieldStatement != null);
            Debug.Assert(yieldStatement.Statement != null);
            _outputFormatter.PrintText("yield");
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(yieldStatement.Statement, data);
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
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitIfElseStatement(IfElseStatement ifElseStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.If);
            if (_prettyPrintOptions.IfParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(ifElseStatement.Condition, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            PrintIfSection(ifElseStatement.TrueStatement);

            foreach (ElseIfSection elseIfSection in ifElseStatement.ElseIfSections)
            {
                _nodeTracker.TrackedVisit(elseIfSection, data);
            }

            if (ifElseStatement.HasElseStatements)
            {
                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.Else);
                PrintIfSection(ifElseStatement.FalseStatement);
            }

            return null;
        }

        private void PrintIfSection(List<Statement> statements)
        {
            if (statements.Count != 1 || !(statements[0] is BlockStatement))
            {
                _outputFormatter.Space();
            }
            if (statements.Count != 1)
            {
                _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
            }
            foreach (Statement stmt in statements)
            {
                _nodeTracker.TrackedVisit(stmt, null);
            }
            if (statements.Count != 1)
            {
                _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
            }
            if (statements.Count != 1 || !(statements[0] is BlockStatement))
            {
                _outputFormatter.Space();
            }
        }

        public object VisitElseIfSection(ElseIfSection elseIfSection, object data)
        {
            _outputFormatter.PrintToken(Tokens.Else);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.If);
            if (_prettyPrintOptions.IfParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(elseIfSection.Condition, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            WriteEmbeddedStatement(elseIfSection.EmbeddedStatement);

            return null;
        }

        public object VisitForStatement(ForStatement forStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.For);
            if (_prettyPrintOptions.ForParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _outputFormatter.DoIndent = false;
            _outputFormatter.DoNewLine = false;
            _outputFormatter.EmitSemicolon = false;
            for (int i = 0; i < forStatement.Initializers.Count; ++i)
            {
                INode node = (INode)forStatement.Initializers[i];
                _nodeTracker.TrackedVisit(node, data);
                if (i + 1 < forStatement.Initializers.Count)
                {
                    _outputFormatter.PrintToken(Tokens.Comma);
                }
            }
            _outputFormatter.EmitSemicolon = true;
            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.EmitSemicolon = false;
            if (!forStatement.Condition.IsNull)
            {
                if (_prettyPrintOptions.SpacesAfterSemicolon)
                {
                    _outputFormatter.Space();
                }
                _nodeTracker.TrackedVisit(forStatement.Condition, data);
            }
            _outputFormatter.EmitSemicolon = true;
            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.EmitSemicolon = false;
            if (forStatement.Iterator != null && forStatement.Iterator.Count > 0)
            {
                if (_prettyPrintOptions.SpacesAfterSemicolon)
                {
                    _outputFormatter.Space();
                }

                for (int i = 0; i < forStatement.Iterator.Count; ++i)
                {
                    INode node = (INode)forStatement.Iterator[i];
                    _nodeTracker.TrackedVisit(node, data);
                    if (i + 1 < forStatement.Iterator.Count)
                    {
                        _outputFormatter.PrintToken(Tokens.Comma);
                    }
                }
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.EmitSemicolon = true;
            _outputFormatter.DoNewLine = true;
            _outputFormatter.DoIndent = true;

            WriteEmbeddedStatement(forStatement.EmbeddedStatement);

            return null;
        }

        private void WriteEmbeddedStatement(Statement statement)
        {
            if (statement is BlockStatement)
            {
                _nodeTracker.TrackedVisit(statement, _prettyPrintOptions.StatementBraceStyle);
            }
            else
            {
                ++_outputFormatter.IndentationLevel;
                _outputFormatter.NewLine();
                _nodeTracker.TrackedVisit(statement, null);
                _outputFormatter.NewLine();
                --_outputFormatter.IndentationLevel;
            }
        }

        public object VisitLabelStatement(LabelStatement labelStatement, object data)
        {
            _outputFormatter.PrintIdentifier(labelStatement.Label);
            _outputFormatter.PrintToken(Tokens.Colon);
            return null;
        }

        public object VisitGotoStatement(GotoStatement gotoStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Goto);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(gotoStatement.Label);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitSwitchStatement(SwitchStatement switchStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Switch);
            if (_prettyPrintOptions.SwitchParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(switchStatement.SwitchExpression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
            _outputFormatter.NewLine();
            ++_outputFormatter.IndentationLevel;
            foreach (SwitchSection section in switchStatement.SwitchSections)
            {
                _nodeTracker.TrackedVisit(section, data);
            }
            --_outputFormatter.IndentationLevel;
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
            return null;
        }

        public object VisitSwitchSection(SwitchSection switchSection, object data)
        {
            foreach (CaseLabel label in switchSection.SwitchLabels)
            {
                _nodeTracker.TrackedVisit(label, data);
            }

            ++_outputFormatter.IndentationLevel;
            foreach (Statement stmt in switchSection.Children)
            {
                _outputFormatter.Indent();
                _nodeTracker.TrackedVisit(stmt, data);
                _outputFormatter.NewLine();
            }

            // Check if a 'break' should be auto inserted.
            if (switchSection.Children.Count == 0 ||
                !(switchSection.Children[switchSection.Children.Count - 1] is BreakStatement ||
                  switchSection.Children[switchSection.Children.Count - 1] is ContinueStatement ||
                  switchSection.Children[switchSection.Children.Count - 1] is ReturnStatement))
            {
                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.Break);
                _outputFormatter.PrintToken(Tokens.Semicolon);
                _outputFormatter.NewLine();
            }

            --_outputFormatter.IndentationLevel;
            return null;
        }

        public object VisitCaseLabel(CaseLabel caseLabel, object data)
        {
            _outputFormatter.Indent();
            if (caseLabel.IsDefault)
            {
                _outputFormatter.PrintToken(Tokens.Default);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Case);
                _outputFormatter.Space();
                if (caseLabel.BinaryOperatorType != BinaryOperatorType.None)
                {
                    Error(caseLabel, String.Format("Case labels with binary operators are unsupported : {0}", caseLabel.BinaryOperatorType));
                }
                _nodeTracker.TrackedVisit(caseLabel.Label, data);
            }
            _outputFormatter.PrintToken(Tokens.Colon);
            if (!caseLabel.ToExpression.IsNull)
            {
                PrimitiveExpression pl = caseLabel.Label as PrimitiveExpression;
                PrimitiveExpression pt = caseLabel.ToExpression as PrimitiveExpression;
                if (pl != null && pt != null && pl.Value is int && pt.Value is int)
                {
                    int plv = (int)pl.Value;
                    int prv = (int)pt.Value;
                    if (plv < prv && plv + 12 > prv)
                    {
                        for (int i = plv + 1; i <= prv; i++)
                        {
                            _outputFormatter.NewLine();
                            _outputFormatter.Indent();
                            _outputFormatter.PrintToken(Tokens.Case);
                            _outputFormatter.Space();
                            _outputFormatter.PrintText(i.ToString(NumberFormatInfo.InvariantInfo));
                            _outputFormatter.PrintToken(Tokens.Colon);
                        }
                    }
                    else
                    {
                        _outputFormatter.PrintText(" // TODO: to ");
                        _nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
                    }
                }
                else
                {
                    _outputFormatter.PrintText(" // TODO: to ");
                    _nodeTracker.TrackedVisit(caseLabel.ToExpression, data);
                }
            }
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitBreakStatement(BreakStatement breakStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Break);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitStopStatement(StopStatement stopStatement, object data)
        {
            _outputFormatter.PrintText("System.Diagnostics.Debugger.Break()");
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitResumeStatement(ResumeStatement resumeStatement, object data)
        {
            NotSupported(resumeStatement);
            return null;
        }

        public object VisitEndStatement(EndStatement endStatement, object data)
        {
            _outputFormatter.PrintText("System.Environment.Exit(0)");
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitContinueStatement(ContinueStatement continueStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Continue);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Goto);
            _outputFormatter.Space();
            if (gotoCaseStatement.IsDefaultCase)
            {
                _outputFormatter.PrintToken(Tokens.Default);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Case);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(gotoCaseStatement.Expression, data);
            }
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        private void PrintLoopCheck(DoLoopStatement doLoopStatement)
        {
            _outputFormatter.PrintToken(Tokens.While);
            if (_prettyPrintOptions.WhileParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);

            if (doLoopStatement.ConditionType == ConditionType.Until)
            {
                _outputFormatter.PrintToken(Tokens.Not);
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            }

            if (doLoopStatement.Condition.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.True);
            }
            else
            {
                _nodeTracker.TrackedVisit(doLoopStatement.Condition, null);
            }

            if (doLoopStatement.ConditionType == ConditionType.Until)
            {
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
        }

        public object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data)
        {
            if (doLoopStatement.ConditionPosition == ConditionPosition.None)
            {
                Error(doLoopStatement, String.Format("Unknown condition position for loop : {0}.", doLoopStatement));
            }

            if (doLoopStatement.ConditionPosition == ConditionPosition.Start)
            {
                PrintLoopCheck(doLoopStatement);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Do);
            }

            WriteEmbeddedStatement(doLoopStatement.EmbeddedStatement);

            if (doLoopStatement.ConditionPosition == ConditionPosition.End)
            {
                _outputFormatter.Indent();
                PrintLoopCheck(doLoopStatement);
                _outputFormatter.PrintToken(Tokens.Semicolon);
                _outputFormatter.NewLine();
            }

            return null;
        }

        public object VisitForeachStatement(ForeachStatement foreachStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Foreach);
            if (_prettyPrintOptions.ForeachParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(foreachStatement.TypeReference, data);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(foreachStatement.VariableName);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.In);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(foreachStatement.Expression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            WriteEmbeddedStatement(foreachStatement.EmbeddedStatement);

            return null;
        }

        public object VisitLockStatement(LockStatement lockStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Lock);
            if (_prettyPrintOptions.LockParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(lockStatement.LockExpression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            WriteEmbeddedStatement(lockStatement.EmbeddedStatement);

            return null;
        }

        public object VisitUsingStatement(UsingStatement usingStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Using);
            if (_prettyPrintOptions.UsingParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _outputFormatter.DoIndent = false;
            _outputFormatter.DoNewLine = false;
            _outputFormatter.EmitSemicolon = false;

            _nodeTracker.TrackedVisit(usingStatement.ResourceAcquisition, data);
            _outputFormatter.DoIndent = true;
            _outputFormatter.DoNewLine = true;
            _outputFormatter.EmitSemicolon = true;

            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            WriteEmbeddedStatement(usingStatement.EmbeddedStatement);

            return null;
        }

        public object VisitWithStatement(WithStatement withStatement, object data)
        {
            NotSupported(withStatement);
            return null;
        }

        public object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Try);
            WriteEmbeddedStatement(tryCatchStatement.StatementBlock);

            foreach (CatchClause catchClause in tryCatchStatement.CatchClauses)
            {
                _nodeTracker.TrackedVisit(catchClause, data);
            }

            if (!tryCatchStatement.FinallyBlock.IsNull)
            {
                _outputFormatter.Indent();
                _outputFormatter.PrintToken(Tokens.Finally);
                WriteEmbeddedStatement(tryCatchStatement.FinallyBlock);
            }
            return null;
        }

        public object VisitCatchClause(CatchClause catchClause, object data)
        {
            _outputFormatter.Indent();
            _outputFormatter.PrintToken(Tokens.Catch);

            if (!catchClause.TypeReference.IsNull)
            {
                if (_prettyPrintOptions.CatchParentheses)
                {
                    _outputFormatter.Space();
                }
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                _outputFormatter.PrintIdentifier(catchClause.TypeReference.Type);
                if (catchClause.VariableName.Length > 0)
                {
                    _outputFormatter.Space();
                    _outputFormatter.PrintIdentifier(catchClause.VariableName);
                }
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            WriteEmbeddedStatement(catchClause.StatementBlock);
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
            _outputFormatter.PrintToken(Tokens.Semicolon);
            return null;
        }

        public object VisitFixedStatement(FixedStatement fixedStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Fixed);
            if (_prettyPrintOptions.FixedParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(fixedStatement.TypeReference, data);
            _outputFormatter.Space();
            AppendCommaSeparatedList(fixedStatement.PointerDeclarators);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            WriteEmbeddedStatement(fixedStatement.EmbeddedStatement);
            return null;
        }

        public object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Unsafe);
            WriteEmbeddedStatement(unsafeStatement.Block);
            return null;
        }

        public object VisitCheckedStatement(CheckedStatement checkedStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Checked);
            WriteEmbeddedStatement(checkedStatement.Block);
            return null;
        }

        public object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.Unchecked);
            WriteEmbeddedStatement(uncheckedStatement.Block);
            return null;
        }

        public object VisitExitStatement(ExitStatement exitStatement, object data)
        {
            if (exitStatement.ExitType == ExitType.Function || exitStatement.ExitType == ExitType.Sub || exitStatement.ExitType == ExitType.Property)
            {
                _outputFormatter.PrintToken(Tokens.Return);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.Break);
            }
            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.PrintText(" // TODO: might not be correct. Was : Exit " + exitStatement.ExitType);
            _outputFormatter.NewLine();
            return null;
        }

        public object VisitForNextStatement(ForNextStatement forNextStatement, object data)
        {
            _outputFormatter.PrintToken(Tokens.For);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            if (!forNextStatement.TypeReference.IsNull)
            {
                _nodeTracker.TrackedVisit(forNextStatement.TypeReference, data);
                _outputFormatter.Space();
            }
            _outputFormatter.PrintIdentifier(forNextStatement.VariableName);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Assign);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(forNextStatement.Start, data);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(forNextStatement.VariableName);
            _outputFormatter.Space();
            PrimitiveExpression pe = forNextStatement.Step as PrimitiveExpression;
            if ((pe == null || !(pe.Value is int) || ((int)pe.Value) >= 0)
                && !(forNextStatement.Step is UnaryOperatorExpression))
                _outputFormatter.PrintToken(Tokens.LessEqual);
            else
                _outputFormatter.PrintToken(Tokens.GreaterEqual);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(forNextStatement.End, data);
            _outputFormatter.PrintToken(Tokens.Semicolon);
            _outputFormatter.Space();
            _outputFormatter.PrintIdentifier(forNextStatement.VariableName);
            if (forNextStatement.Step.IsNull)
            {
                _outputFormatter.PrintToken(Tokens.Increment);
            }
            else
            {
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.PlusAssign);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(forNextStatement.Step, data);
            }
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);

            WriteEmbeddedStatement(forNextStatement.EmbeddedStatement);
            return null;
        }
        #endregion

        #region Expressions
        public object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data)
        {
            NotSupported(classReferenceExpression);
            return null;
        }

        private static string ConvertCharLiteral(char ch)
        {
            if (ch == '\'') return "\\'";
            return ConvertChar(ch);
        }

        private static string ConvertChar(char ch)
        {
            switch (ch)
            {
                case '\\':
                    return "\\\\";
                case '\0':
                    return "\\0";
                case '\a':
                    return "\\a";
                case '\b':
                    return "\\b";
                case '\f':
                    return "\\f";
                case '\n':
                    return "\\n";
                case '\r':
                    return "\\r";
                case '\t':
                    return "\\t";
                case '\v':
                    return "\\v";
                default:
                    if (char.IsControl(ch))
                    {
                        return "\\u" + (int)ch;
                    }
                    else
                    {
                        return ch.ToString();
                    }
            }
        }

        private static string ConvertString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in str)
            {
                if (ch == '"')
                    sb.Append("\\\"");
                else
                    sb.Append(ConvertChar(ch));
            }
            return sb.ToString();
        }

        public object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data)
        {
            if (primitiveExpression.Value == null)
            {
                _outputFormatter.PrintToken(Tokens.Null);
                return null;
            }

            object val = primitiveExpression.Value;

            if (val is bool)
            {
                if ((bool)val)
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
                _outputFormatter.PrintText('"' + ConvertString(val.ToString()) + '"');
                return null;
            }

            if (val is char)
            {
                _outputFormatter.PrintText("'" + ConvertCharLiteral((char)val) + "'");
                return null;
            }

            if (val is decimal)
            {
                _outputFormatter.PrintText(((decimal)val).ToString(NumberFormatInfo.InvariantInfo) + "m");
                return null;
            }

            if (val is float)
            {
                _outputFormatter.PrintText(((float)val).ToString(NumberFormatInfo.InvariantInfo) + "f");
                return null;
            }

            if (val is double)
            {
                string text = ((double)val).ToString(NumberFormatInfo.InvariantInfo);
                if (text.IndexOf('.') < 0 && text.IndexOf('E') < 0)
                    _outputFormatter.PrintText(text + ".0");
                else
                    _outputFormatter.PrintText(text);
                return null;
            }

            if (val is IFormattable)
            {
                _outputFormatter.PrintText(((IFormattable)val).ToString(null, NumberFormatInfo.InvariantInfo));
                if (val is uint || val is ulong)
                {
                    _outputFormatter.PrintText("u");
                }
                if (val is long || val is ulong)
                {
                    _outputFormatter.PrintText("l");
                }
            }
            else
            {
                _outputFormatter.PrintText(val.ToString());
            }

            return null;
        }

        private static bool IsNullLiteralExpression(Expression expr)
        {
            PrimitiveExpression pe = expr as PrimitiveExpression;
            if (pe == null) return false;
            return pe.Value == null;
        }

        public object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data)
        {
            // VB-operators that require special representation:
            switch (binaryOperatorExpression.Op)
            {
                case BinaryOperatorType.ReferenceEquality:
                case BinaryOperatorType.ReferenceInequality:
                    if (IsNullLiteralExpression(binaryOperatorExpression.Left) || IsNullLiteralExpression(binaryOperatorExpression.Right))
                    {
                        // prefer a == null to object.ReferenceEquals(a, null)
                        break;
                    }

                    if (binaryOperatorExpression.Op == BinaryOperatorType.ReferenceInequality)
                        _outputFormatter.PrintToken(Tokens.Not);
                    _outputFormatter.PrintText("object.ReferenceEquals");
                    if (_prettyPrintOptions.BeforeMethodCallParentheses)
                    {
                        _outputFormatter.Space();
                    }

                    _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
                    PrintFormattedComma();
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
                    _outputFormatter.PrintToken(Tokens.CloseParenthesis);
                    return null;
                case BinaryOperatorType.Power:
                    _outputFormatter.PrintText("Math.Pow");
                    if (_prettyPrintOptions.BeforeMethodCallParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
                    PrintFormattedComma();
                    _nodeTracker.TrackedVisit(binaryOperatorExpression.Right, data);
                    _outputFormatter.PrintToken(Tokens.CloseParenthesis);
                    return null;
            }
            _nodeTracker.TrackedVisit(binaryOperatorExpression.Left, data);
            switch (binaryOperatorExpression.Op)
            {
                case BinaryOperatorType.Add:
                case BinaryOperatorType.Concat: // translate Concatenation to +
                    if (_prettyPrintOptions.AroundAdditiveOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Plus);
                    if (_prettyPrintOptions.AroundAdditiveOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.Subtract:
                    if (_prettyPrintOptions.AroundAdditiveOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Minus);
                    if (_prettyPrintOptions.AroundAdditiveOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.Multiply:
                    if (_prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Times);
                    if (_prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.Divide:
                case BinaryOperatorType.DivideInteger:
                    if (_prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Div);
                    if (_prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.Modulus:
                    if (_prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Mod);
                    if (_prettyPrintOptions.AroundMultiplicativeOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.ShiftLeft:
                    if (_prettyPrintOptions.AroundShiftOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.ShiftLeft);
                    if (_prettyPrintOptions.AroundShiftOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.ShiftRight:
                    if (_prettyPrintOptions.AroundShiftOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.GreaterThan);
                    _outputFormatter.PrintToken(Tokens.GreaterThan);
                    if (_prettyPrintOptions.AroundShiftOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.BitwiseAnd:
                    if (_prettyPrintOptions.AroundBitwiseOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.BitwiseAnd);
                    if (_prettyPrintOptions.AroundBitwiseOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.BitwiseOr:
                    if (_prettyPrintOptions.AroundBitwiseOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.BitwiseOr);
                    if (_prettyPrintOptions.AroundBitwiseOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.ExclusiveOr:
                    if (_prettyPrintOptions.AroundBitwiseOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Xor);
                    if (_prettyPrintOptions.AroundBitwiseOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.LogicalAnd:
                    if (_prettyPrintOptions.AroundLogicalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.LogicalAnd);
                    if (_prettyPrintOptions.AroundLogicalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.LogicalOr:
                    if (_prettyPrintOptions.AroundLogicalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.LogicalOr);
                    if (_prettyPrintOptions.AroundLogicalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                case BinaryOperatorType.Equality:
                case BinaryOperatorType.ReferenceEquality:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.Equal);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.GreaterThan:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.GreaterThan);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.GreaterThanOrEqual:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.GreaterEqual);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.InEquality:
                case BinaryOperatorType.ReferenceInequality:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.NotEqual);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.LessThan:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.LessThan);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.LessThanOrEqual:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.LessEqual);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;
                case BinaryOperatorType.NullCoalescing:
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    _outputFormatter.PrintToken(Tokens.DoubleQuestion);
                    if (_prettyPrintOptions.AroundRelationalOperatorParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    break;

                default:
                    Error(binaryOperatorExpression, String.Format("Unknown binary operator {0}", binaryOperatorExpression.Op));
                    return null;
            }
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
                _outputFormatter.PrintToken(Tokens.LessThan);
                AppendCommaSeparatedList(invocationExpression.TypeArguments);
                _outputFormatter.PrintToken(Tokens.GreaterThan);
            }

            if (_prettyPrintOptions.BeforeMethodCallParentheses)
            {
                _outputFormatter.Space();
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
                case UnaryOperatorType.BitNot:
                    _outputFormatter.PrintToken(Tokens.BitwiseComplement);
                    break;
                case UnaryOperatorType.Decrement:
                    _outputFormatter.PrintToken(Tokens.Decrement);
                    break;
                case UnaryOperatorType.Increment:
                    _outputFormatter.PrintToken(Tokens.Increment);
                    break;
                case UnaryOperatorType.Minus:
                    _outputFormatter.PrintToken(Tokens.Minus);
                    break;
                case UnaryOperatorType.Not:
                    _outputFormatter.PrintToken(Tokens.Not);
                    break;
                case UnaryOperatorType.Plus:
                    _outputFormatter.PrintToken(Tokens.Plus);
                    break;
                case UnaryOperatorType.PostDecrement:
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintToken(Tokens.Decrement);
                    return null;
                case UnaryOperatorType.PostIncrement:
                    _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
                    _outputFormatter.PrintToken(Tokens.Increment);
                    return null;
                case UnaryOperatorType.Star:
                    _outputFormatter.PrintToken(Tokens.Times);
                    break;
                case UnaryOperatorType.BitWiseAnd:
                    _outputFormatter.PrintToken(Tokens.BitwiseAnd);
                    break;
                default:
                    Error(unaryOperatorExpression, String.Format("Unknown unary operator {0}", unaryOperatorExpression.Op));
                    return null;
            }
            _nodeTracker.TrackedVisit(unaryOperatorExpression.Expression, data);
            return null;
        }

        public object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
        {
            _nodeTracker.TrackedVisit(assignmentExpression.Left, data);
            if (_prettyPrintOptions.AroundAssignmentParentheses)
            {
                _outputFormatter.Space();
            }
            switch (assignmentExpression.Op)
            {
                case AssignmentOperatorType.Assign:
                    _outputFormatter.PrintToken(Tokens.Assign);
                    break;
                case AssignmentOperatorType.Add:
                    _outputFormatter.PrintToken(Tokens.PlusAssign);
                    break;
                case AssignmentOperatorType.Subtract:
                    _outputFormatter.PrintToken(Tokens.MinusAssign);
                    break;
                case AssignmentOperatorType.Multiply:
                    _outputFormatter.PrintToken(Tokens.TimesAssign);
                    break;
                case AssignmentOperatorType.Divide:
                case AssignmentOperatorType.DivideInteger:
                    _outputFormatter.PrintToken(Tokens.DivAssign);
                    break;
                case AssignmentOperatorType.ShiftLeft:
                    _outputFormatter.PrintToken(Tokens.ShiftLeftAssign);
                    break;
                case AssignmentOperatorType.ShiftRight:
                    _outputFormatter.PrintToken(Tokens.GreaterThan);
                    _outputFormatter.PrintToken(Tokens.GreaterEqual);
                    break;
                case AssignmentOperatorType.ExclusiveOr:
                    _outputFormatter.PrintToken(Tokens.XorAssign);
                    break;
                case AssignmentOperatorType.Modulus:
                    _outputFormatter.PrintToken(Tokens.ModAssign);
                    break;
                case AssignmentOperatorType.BitwiseAnd:
                    _outputFormatter.PrintToken(Tokens.BitwiseAndAssign);
                    break;
                case AssignmentOperatorType.BitwiseOr:
                    _outputFormatter.PrintToken(Tokens.BitwiseOrAssign);
                    break;
                case AssignmentOperatorType.Power:
                    _outputFormatter.PrintToken(Tokens.Assign);
                    if (_prettyPrintOptions.AroundAssignmentParentheses)
                    {
                        _outputFormatter.Space();
                    }
                    VisitBinaryOperatorExpression(new BinaryOperatorExpression(assignmentExpression.Left,
                                                                               BinaryOperatorType.Power,
                                                                               assignmentExpression.Right), data);
                    return null;
                default:
                    Error(assignmentExpression, String.Format("Unknown assignment operator {0}", assignmentExpression.Op));
                    return null;
            }
            if (_prettyPrintOptions.AroundAssignmentParentheses)
            {
                _outputFormatter.Space();
            }
            _nodeTracker.TrackedVisit(assignmentExpression.Right, data);
            return null;
        }

        public object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Sizeof);
            if (_prettyPrintOptions.SizeOfParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(sizeOfExpression.TypeReference, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Typeof);
            if (_prettyPrintOptions.TypeOfParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(typeOfExpression.TypeReference, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Default);
            if (_prettyPrintOptions.TypeOfParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(defaultValueExpression.TypeReference, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data)
        {
            _nodeTracker.TrackedVisit(typeOfIsExpression.Expression, data);
            _outputFormatter.Space();
            _outputFormatter.PrintToken(Tokens.Is);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(typeOfIsExpression.TypeReference, data);
            return null;
        }

        public object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data)
        {
            // C# 2.0 can reference methods directly
            return _nodeTracker.TrackedVisit(addressOfExpression.Expression, data);
        }

        public object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Delegate);

            if (anonymousMethodExpression.Parameters.Count > 0 || anonymousMethodExpression.HasParameterList)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                AppendCommaSeparatedList(anonymousMethodExpression.Parameters);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            OutputBlockAllowInline(anonymousMethodExpression.Body, _prettyPrintOptions.MethodBraceStyle, false);
            return null;
        }

        public object VisitCheckedExpression(CheckedExpression checkedExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Checked);
            if (_prettyPrintOptions.CheckedParentheses)
            {
                _outputFormatter.Space();
            }

            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(checkedExpression.Expression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Unchecked);
            if (_prettyPrintOptions.UncheckedParentheses)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            _nodeTracker.TrackedVisit(uncheckedExpression.Expression, data);
            _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            return null;
        }

        public object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data)
        {
            _nodeTracker.TrackedVisit(pointerReferenceExpression.TargetObject, data);
            _outputFormatter.PrintToken(Tokens.Pointer);
            _outputFormatter.PrintIdentifier(pointerReferenceExpression.Identifier);
            return null;
        }

        public object VisitCastExpression(CastExpression castExpression, object data)
        {
            if (castExpression.CastType == CastType.TryCast)
            {
                _nodeTracker.TrackedVisit(castExpression.Expression, data);
                _outputFormatter.Space();
                _outputFormatter.PrintToken(Tokens.As);
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(castExpression.CastTo, data);
            }
            else
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
                _nodeTracker.TrackedVisit(castExpression.CastTo, data);
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
                if (_prettyPrintOptions.SpacesAfterTypecast)
                {
                    _outputFormatter.Space();
                }
                _nodeTracker.TrackedVisit(castExpression.Expression, data);
            }
            return null;
        }

        public object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Stackalloc);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(stackAllocExpression.TypeReference, data);
            _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            _nodeTracker.TrackedVisit(stackAllocExpression.Expression, data);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
            return null;
        }

        public object VisitIndexerExpression(IndexerExpression indexerExpression, object data)
        {
            _nodeTracker.TrackedVisit(indexerExpression.TargetObject, data);
            _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            AppendCommaSeparatedList(indexerExpression.Indexes);
            if (_prettyPrintOptions.SpacesWithinBrackets)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
            return null;
        }

        public object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.This);
            return null;
        }

        public object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.Base);
            return null;
        }

        public object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.New);
            _outputFormatter.Space();
            _nodeTracker.TrackedVisit(objectCreateExpression.CreateType, data);
            if (_prettyPrintOptions.NewParentheses)
            {
                _outputFormatter.Space();
            }
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
                _outputFormatter.PrintToken(Tokens.OpenSquareBracket);
                if (_prettyPrintOptions.SpacesWithinBrackets)
                {
                    _outputFormatter.Space();
                }
                for (int i = 0; i < arrayCreateExpression.Arguments.Count; ++i)
                {
                    if (i > 0) PrintFormattedComma();
                    _nodeTracker.TrackedVisit(arrayCreateExpression.Arguments[i], data);
                }
                if (_prettyPrintOptions.SpacesWithinBrackets)
                {
                    _outputFormatter.Space();
                }
                _outputFormatter.PrintToken(Tokens.CloseSquareBracket);
                PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 1);
            }
            else
            {
                PrintArrayRank(arrayCreateExpression.CreateType.RankSpecifier, 0);
            }

            if (!arrayCreateExpression.ArrayInitializer.IsNull)
            {
                _outputFormatter.Space();
                _nodeTracker.TrackedVisit(arrayCreateExpression.ArrayInitializer, data);
            }
            return null;
        }

        public object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
        {
            Expression target = fieldReferenceExpression.TargetObject;

            if (target is BinaryOperatorExpression || target is CastExpression)
            {
                _outputFormatter.PrintToken(Tokens.OpenParenthesis);
            }
            _nodeTracker.TrackedVisit(target, data);
            if (target is BinaryOperatorExpression || target is CastExpression)
            {
                _outputFormatter.PrintToken(Tokens.CloseParenthesis);
            }
            _outputFormatter.PrintToken(Tokens.Dot);
            _outputFormatter.PrintIdentifier(fieldReferenceExpression.FieldName);
            return null;
        }

        public object VisitDirectionExpression(DirectionExpression directionExpression, object data)
        {
            switch (directionExpression.FieldDirection)
            {
                case FieldDirection.Out:
                    _outputFormatter.PrintToken(Tokens.Out);
                    _outputFormatter.Space();
                    break;
                case FieldDirection.Ref:
                    _outputFormatter.PrintToken(Tokens.Ref);
                    _outputFormatter.Space();
                    break;
            }
            _nodeTracker.TrackedVisit(directionExpression.Expression, data);
            return null;
        }

        public object VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, object data)
        {
            _outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
            this.AppendCommaSeparatedList(arrayInitializerExpression.CreateExpressions);
            _outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
            return null;
        }

        public object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data)
        {
            _nodeTracker.TrackedVisit(conditionalExpression.Condition, data);
            if (_prettyPrintOptions.ConditionalOperatorBeforeConditionSpace)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.Question);
            if (_prettyPrintOptions.ConditionalOperatorAfterConditionSpace)
            {
                _outputFormatter.Space();
            }
            _nodeTracker.TrackedVisit(conditionalExpression.TrueExpression, data);
            if (_prettyPrintOptions.ConditionalOperatorBeforeSeparatorSpace)
            {
                _outputFormatter.Space();
            }
            _outputFormatter.PrintToken(Tokens.Colon);
            if (_prettyPrintOptions.ConditionalOperatorAfterSeparatorSpace)
            {
                _outputFormatter.Space();
            }
            _nodeTracker.TrackedVisit(conditionalExpression.FalseExpression, data);
            return null;
        }

        #endregion
        #endregion

        private void OutputModifier(ParameterModifiers modifier, INode node)
        {
            switch (modifier)
            {
                case ParameterModifiers.None:
                case ParameterModifiers.In:
                    break;
                case ParameterModifiers.Out:
                    _outputFormatter.PrintToken(Tokens.Out);
                    _outputFormatter.Space();
                    break;
                case ParameterModifiers.Params:
                    _outputFormatter.PrintToken(Tokens.Params);
                    _outputFormatter.Space();
                    break;
                case ParameterModifiers.Ref:
                    _outputFormatter.PrintToken(Tokens.Ref);
                    _outputFormatter.Space();
                    break;
                case ParameterModifiers.Optional:
                    Error(node, String.Format("Optional parameters aren't supported in C#"));
                    break;
                default:
                    Error(node, String.Format("Unsupported modifier : {0}", modifier));
                    break;
            }
        }

        private void OutputModifier(Modifiers modifier)
        {
            ArrayList tokenList = new ArrayList();
            if ((modifier & Modifiers.Unsafe) != 0)
            {
                tokenList.Add(Tokens.Unsafe);
            }
            if ((modifier & Modifiers.Public) != 0)
            {
                tokenList.Add(Tokens.Public);
            }
            if ((modifier & Modifiers.Private) != 0)
            {
                tokenList.Add(Tokens.Private);
            }
            if ((modifier & Modifiers.Protected) != 0)
            {
                tokenList.Add(Tokens.Protected);
            }
            if ((modifier & Modifiers.Static) != 0)
            {
                tokenList.Add(Tokens.Static);
            }
            if ((modifier & Modifiers.Internal) != 0)
            {
                tokenList.Add(Tokens.Internal);
            }
            if ((modifier & Modifiers.Override) != 0)
            {
                tokenList.Add(Tokens.Override);
            }
            if ((modifier & Modifiers.Abstract) != 0)
            {
                tokenList.Add(Tokens.Abstract);
            }
            if ((modifier & Modifiers.Virtual) != 0)
            {
                tokenList.Add(Tokens.Virtual);
            }
            if ((modifier & Modifiers.New) != 0)
            {
                tokenList.Add(Tokens.New);
            }
            if ((modifier & Modifiers.Sealed) != 0)
            {
                tokenList.Add(Tokens.Sealed);
            }
            if ((modifier & Modifiers.Extern) != 0)
            {
                tokenList.Add(Tokens.Extern);
            }
            if ((modifier & Modifiers.Const) != 0)
            {
                tokenList.Add(Tokens.Const);
            }
            if ((modifier & Modifiers.ReadOnly) != 0)
            {
                tokenList.Add(Tokens.Readonly);
            }
            if ((modifier & Modifiers.Volatile) != 0)
            {
                tokenList.Add(Tokens.Volatile);
            }
            if ((modifier & Modifiers.Fixed) != 0)
            {
                tokenList.Add(Tokens.Fixed);
            }
            _outputFormatter.PrintTokenList(tokenList);

            if ((modifier & Modifiers.Partial) != 0)
            {
                _outputFormatter.PrintText("partial ");
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
                        PrintFormattedComma();
                    }
                    if ((i + 1) % 10 == 0)
                    {
                        _outputFormatter.NewLine();
                        _outputFormatter.Indent();
                    }
                    i++;
                }
            }
        }
    }
}
