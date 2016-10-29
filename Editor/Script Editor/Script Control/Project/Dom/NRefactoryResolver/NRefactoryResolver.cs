// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 2496 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using AIMS.Libraries.Scripting.NRefactory.Ast;
using AIMS.Libraries.Scripting.NRefactory.Visitors;
using NR = AIMS.Libraries.Scripting.NRefactory;

namespace AIMS.Libraries.Scripting.Dom.NRefactoryResolver
{
    public class NRefactoryResolver : IResolver
    {
        private ICompilationUnit _cu;
        private IClass _callingClass;
        private IMember _callingMember;
        private AIMS.Libraries.Scripting.NRefactory.Visitors.LookupTableVisitor _lookupTableVisitor;
        private IProjectContent _projectContent;

        private NR.SupportedLanguage _language;

        private int _caretLine;
        private int _caretColumn;

        public NR.SupportedLanguage Language
        {
            get
            {
                return _language;
            }
        }

        public IProjectContent ProjectContent
        {
            get
            {
                return _projectContent;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _projectContent = value;
            }
        }

        public ICompilationUnit CompilationUnit
        {
            get
            {
                return _cu;
            }
        }

        public IClass CallingClass
        {
            get
            {
                return _callingClass;
            }
        }

        public IMember CallingMember
        {
            get
            {
                return _callingMember;
            }
        }

        public int CaretLine
        {
            get
            {
                return _caretLine;
            }
        }

        public int CaretColumn
        {
            get
            {
                return _caretColumn;
            }
        }

        private LanguageProperties _languageProperties;

        public LanguageProperties LanguageProperties
        {
            get
            {
                return _languageProperties;
            }
        }

        public NRefactoryResolver(IProjectContent projectContent, LanguageProperties languageProperties)
        {
            if (projectContent == null)
                throw new ArgumentNullException("projectContent");
            if (languageProperties == null)
                throw new ArgumentNullException("languageProperties");
            _languageProperties = languageProperties;
            _projectContent = projectContent;
            if (languageProperties is LanguageProperties.CSharpProperties)
            {
                _language = NR.SupportedLanguage.CSharp;
            }
            else if (languageProperties is LanguageProperties.VBNetProperties)
            {
                _language = NR.SupportedLanguage.VBNet;
            }
            else
            {
                throw new NotSupportedException("The language " + languageProperties.ToString() + " is not supported in the resolver");
            }
        }

        [Obsolete("Use the IProjectContent, LanguageProperties overload instead to support .cs files inside vb projects or similar.")]
        public NRefactoryResolver(IProjectContent projectContent)
            : this(projectContent, projectContent.Language)
        { }

        private Expression ParseExpression(string expression)
        {
            Expression expr = SpecialConstructs(expression);
            if (expr == null)
            {
                using (NR.IParser p = NR.ParserFactory.CreateParser(_language, new System.IO.StringReader(expression)))
                {
                    expr = p.ParseExpression();
                }
            }
            return expr;
        }

        private string GetFixedExpression(ExpressionResult expressionResult)
        {
            string expression = expressionResult.Expression;
            if (expression == null)
            {
                expression = "";
            }
            expression = expression.TrimStart();

            return expression;
        }

        public bool Initialize(string fileName, int caretLineNumber, int caretColumn)
        {
            _caretLine = caretLineNumber;
            _caretColumn = caretColumn;

            ParseInformation parseInfo = HostCallback.GetParseInformation(fileName);
            if (parseInfo == null)
            {
                return false;
            }

            _cu = parseInfo.MostRecentCompilationUnit;

            if (_cu != null)
            {
                _callingClass = _cu.GetInnermostClass(_caretLine, caretColumn);
                if (_cu.ProjectContent != null)
                {
                    this.ProjectContent = _cu.ProjectContent;
                }
            }
            _callingMember = GetCurrentMember();
            return true;
        }

        public ResolveResult Resolve(ExpressionResult expressionResult,
                                     int caretLineNumber,
                                     int caretColumn,
                                     string fileName,
                                     string fileContent)
        {
            string expression = GetFixedExpression(expressionResult);

            if (!Initialize(fileName, caretLineNumber, caretColumn))
                return null;

            Expression expr = null;
            if (_language == NR.SupportedLanguage.VBNet)
            {
                if (expression.Length == 0 || expression[0] == '.')
                {
                    return WithResolve(expression, fileContent);
                }
                else if ("global".Equals(expression, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new NamespaceResolveResult(null, null, "");
                }
            }
            if (expr == null)
            {
                expr = ParseExpression(expression);
                if (expr == null)
                {
                    return null;
                }
                if (expressionResult.Context.IsObjectCreation)
                {
                    Expression tmp = expr;
                    while (tmp != null)
                    {
                        if (tmp is IdentifierExpression)
                            return ResolveInternal(expr, ExpressionContext.Type);
                        if (tmp is FieldReferenceExpression)
                            tmp = (tmp as FieldReferenceExpression).TargetObject;
                        else
                            break;
                    }
                    expr = ParseExpression("new " + expression);
                    if (expr == null)
                    {
                        return null;
                    }
                }
            }

            if (expressionResult.Context.IsAttributeContext)
            {
                return ResolveAttribute(expr);
            }

            RunLookupTableVisitor(fileContent);

            ResolveResult rr = CtrlSpaceResolveHelper.GetResultFromDeclarationLine(_callingClass, _callingMember as IMethodOrProperty, _caretLine, caretColumn, expressionResult);
            if (rr != null) return rr;

            return ResolveInternal(expr, expressionResult.Context);
        }

        private ResolveResult WithResolve(string expression, string fileContent)
        {
            RunLookupTableVisitor(fileContent);

            WithStatement innermost = null;
            if (_lookupTableVisitor.WithStatements != null)
            {
                foreach (WithStatement with in _lookupTableVisitor.WithStatements)
                {
                    if (IsInside(new NR.Location(_caretColumn, _caretLine), with.StartLocation, with.EndLocation))
                    {
                        innermost = with;
                    }
                }
            }
            if (innermost != null)
            {
                if (expression.Length > 1)
                {
                    Expression expr = ParseExpression(DummyFindVisitor.dummyName + expression);
                    if (expr == null) return null;
                    DummyFindVisitor v = new DummyFindVisitor();
                    expr.AcceptVisitor(v, null);
                    if (v.result == null) return null;
                    v.result.TargetObject = innermost.Expression;
                    return ResolveInternal(expr, ExpressionContext.Default);
                }
                else
                {
                    return ResolveInternal(innermost.Expression, ExpressionContext.Default);
                }
            }
            else
            {
                return null;
            }
        }
        private class DummyFindVisitor : AbstractAstVisitor
        {
            internal const string dummyName = "___withStatementExpressionDummy";
            internal FieldReferenceExpression result;
            public override object VisitFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression, object data)
            {
                IdentifierExpression ie = fieldReferenceExpression.TargetObject as IdentifierExpression;
                if (ie != null && ie.Identifier == dummyName)
                    result = fieldReferenceExpression;
                return base.VisitFieldReferenceExpression(fieldReferenceExpression, data);
            }
        }

        public INode ParseCurrentMember(string fileContent)
        {
            CompilationUnit cu = ParseCurrentMemberAsCompilationUnit(fileContent);
            if (cu != null && cu.Children.Count > 0)
            {
                TypeDeclaration td = cu.Children[0] as TypeDeclaration;
                if (td != null && td.Children.Count > 0)
                {
                    return td.Children[0];
                }
            }
            return null;
        }

        public CompilationUnit ParseCurrentMemberAsCompilationUnit(string fileContent)
        {
            System.IO.TextReader content = ExtractCurrentMethod(fileContent);
            if (content != null)
            {
                NR.IParser p = NR.ParserFactory.CreateParser(_language, content);
                p.Parse();
                return p.CompilationUnit;
            }
            else
            {
                return null;
            }
        }

        private void RunLookupTableVisitor(string fileContent)
        {
            _lookupTableVisitor = new LookupTableVisitor(_language);

            if (_callingMember != null)
            {
                CompilationUnit cu = ParseCurrentMemberAsCompilationUnit(fileContent);
                if (cu != null)
                {
                    _lookupTableVisitor.VisitCompilationUnit(cu, null);
                }
            }
        }

        public void RunLookupTableVisitor(INode currentMemberNode)
        {
            _lookupTableVisitor = new LookupTableVisitor(_language);
            currentMemberNode.AcceptVisitor(_lookupTableVisitor, null);
        }

        private string GetAttributeName(Expression expr)
        {
            if (expr is IdentifierExpression)
            {
                return (expr as IdentifierExpression).Identifier;
            }
            else if (expr is FieldReferenceExpression)
            {
                TypeVisitor typeVisitor = new TypeVisitor(this);
                FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
                IReturnType type = fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType;
                if (type is TypeVisitor.NamespaceReturnType)
                {
                    return type.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
                }
            }
            return null;
        }

        private IClass GetAttribute(string name)
        {
            if (name == null)
                return null;
            IClass c = SearchClass(name);
            if (c != null)
            {
                if (c.IsTypeInInheritanceTree(c.ProjectContent.SystemTypes.Attribute.GetUnderlyingClass()))
                    return c;
            }
            return SearchClass(name + "Attribute");
        }

        private ResolveResult ResolveAttribute(Expression expr)
        {
            string attributeName = GetAttributeName(expr);
            IClass c = GetAttribute(attributeName);
            if (c != null)
            {
                return new TypeResolveResult(_callingClass, _callingMember, c);
            }
            else if (expr is InvocationExpression)
            {
                InvocationExpression ie = (InvocationExpression)expr;
                attributeName = GetAttributeName(ie.TargetObject);
                c = GetAttribute(attributeName);
                if (c != null)
                {
                    List<IMethod> ctors = new List<IMethod>();
                    foreach (IMethod m in c.Methods)
                    {
                        if (m.IsConstructor && !m.IsStatic)
                            ctors.Add(m);
                    }
                    TypeVisitor typeVisitor = new TypeVisitor(this);
                    return CreateMemberResolveResult(typeVisitor.FindOverload(ctors, null, ie.Arguments, null));
                }
            }
            return null;
        }

        public ResolveResult ResolveInternal(Expression expr, ExpressionContext context)
        {
            TypeVisitor typeVisitor = new TypeVisitor(this);
            IReturnType type;

            if (expr is PrimitiveExpression)
            {
                if (((PrimitiveExpression)expr).Value is int)
                    return new IntegerLiteralResolveResult(_callingClass, _callingMember, _projectContent.SystemTypes.Int32);
            }
            else if (expr is InvocationExpression)
            {
                IMethodOrProperty method = typeVisitor.GetMethod(expr as InvocationExpression);
                if (method != null)
                {
                    return CreateMemberResolveResult(method);
                }
                else
                {
                    // InvocationExpression can also be a delegate/event call
                    ResolveResult invocationTarget = ResolveInternal((expr as InvocationExpression).TargetObject, ExpressionContext.Default);
                    if (invocationTarget == null)
                        return null;
                    type = invocationTarget.ResolvedType;
                    if (type == null)
                        return null;
                    IClass c = type.GetUnderlyingClass();
                    if (c == null || c.ClassType != ClassType.Delegate)
                        return null;
                    // We don't want to show "System.EventHandler.Invoke" in the tooltip
                    // of "EventCall(this, EventArgs.Empty)", we just show the event/delegate for now

                    // but for DelegateCall(params).* completion, we use the delegate's
                    // return type instead of the delegate type itself
                    method = c.Methods.Find(delegate (IMethod innerMethod) { return innerMethod.Name == "Invoke"; });
                    if (method != null)
                        invocationTarget.ResolvedType = method.ReturnType;

                    return invocationTarget;
                }
            }
            else if (expr is IndexerExpression)
            {
                return CreateMemberResolveResult(typeVisitor.GetIndexer(expr as IndexerExpression));
            }
            else if (expr is FieldReferenceExpression)
            {
                FieldReferenceExpression fieldReferenceExpression = (FieldReferenceExpression)expr;
                if (fieldReferenceExpression.FieldName == null || fieldReferenceExpression.FieldName.Length == 0)
                {
                    // NRefactory creates this "dummy" fieldReferenceExpression when it should
                    // parse a primitive type name (int, short; Integer, Decimal)
                    if (fieldReferenceExpression.TargetObject is TypeReferenceExpression)
                    {
                        type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)fieldReferenceExpression.TargetObject).TypeReference, this);
                        if (type != null)
                        {
                            return new TypeResolveResult(_callingClass, _callingMember, type);
                        }
                    }
                }
                type = fieldReferenceExpression.TargetObject.AcceptVisitor(typeVisitor, null) as IReturnType;
                if (type != null)
                {
                    ResolveResult result = ResolveMemberReferenceExpression(type, fieldReferenceExpression);
                    if (result != null)
                        return result;
                }
            }
            else if (expr is IdentifierExpression)
            {
                ResolveResult result = ResolveIdentifier(((IdentifierExpression)expr).Identifier, context);
                if (result != null)
                    return result;
            }
            else if (expr is TypeReferenceExpression)
            {
                type = TypeVisitor.CreateReturnType(((TypeReferenceExpression)expr).TypeReference, this);
                if (type != null)
                {
                    if (type is TypeVisitor.NamespaceReturnType)
                        return new NamespaceResolveResult(_callingClass, _callingMember, type.FullyQualifiedName);
                    IClass c = type.GetUnderlyingClass();
                    if (c != null)
                        return new TypeResolveResult(_callingClass, _callingMember, type, c);
                }
                return null;
            }
            type = expr.AcceptVisitor(typeVisitor, null) as IReturnType;

            if (type == null || type.FullyQualifiedName == "")
            {
                return null;
            }
            if (expr is ObjectCreateExpression)
            {
                List<IMethod> constructors = new List<IMethod>();
                foreach (IMethod m in type.GetMethods())
                {
                    if (m.IsConstructor && !m.IsStatic)
                        constructors.Add(m);
                }

                if (constructors.Count == 0)
                {
                    // Class has no constructors -> create default constructor
                    IClass c = type.GetUnderlyingClass();
                    if (c != null)
                    {
                        return CreateMemberResolveResult(Constructor.CreateDefault(c));
                    }
                }
                IReturnType[] typeParameters = null;
                if (type.IsConstructedReturnType)
                {
                    typeParameters = new IReturnType[type.CastToConstructedReturnType().TypeArguments.Count];
                    type.CastToConstructedReturnType().TypeArguments.CopyTo(typeParameters, 0);
                }
                ResolveResult rr = CreateMemberResolveResult(typeVisitor.FindOverload(constructors, typeParameters, ((ObjectCreateExpression)expr).Parameters, null));
                if (rr != null)
                {
                    rr.ResolvedType = type;
                }
                return rr;
            }
            return new ResolveResult(_callingClass, _callingMember, type);
        }

        private ResolveResult ResolveMemberReferenceExpression(IReturnType type, FieldReferenceExpression fieldReferenceExpression)
        {
            IClass c;
            IMember member;
            if (type is TypeVisitor.NamespaceReturnType)
            {
                string combinedName;
                if (type.FullyQualifiedName == "")
                    combinedName = fieldReferenceExpression.FieldName;
                else
                    combinedName = type.FullyQualifiedName + "." + fieldReferenceExpression.FieldName;
                if (_projectContent.NamespaceExists(combinedName))
                {
                    return new NamespaceResolveResult(_callingClass, _callingMember, combinedName);
                }
                c = GetClass(combinedName);
                if (c != null)
                {
                    return new TypeResolveResult(_callingClass, _callingMember, c);
                }
                if (_languageProperties.ImportModules)
                {
                    // go through the members of the modules
                    foreach (object o in _projectContent.GetNamespaceContents(type.FullyQualifiedName))
                    {
                        member = o as IMember;
                        if (member != null && IsSameName(member.Name, fieldReferenceExpression.FieldName))
                        {
                            return CreateMemberResolveResult(member);
                        }
                    }
                }
                return null;
            }
            member = GetMember(type, fieldReferenceExpression.FieldName);
            if (member != null)
                return CreateMemberResolveResult(member);
            c = type.GetUnderlyingClass();
            if (c != null)
            {
                foreach (IClass baseClass in c.ClassInheritanceTree)
                {
                    List<IClass> innerClasses = baseClass.InnerClasses;
                    if (innerClasses != null)
                    {
                        foreach (IClass innerClass in innerClasses)
                        {
                            if (IsSameName(innerClass.Name, fieldReferenceExpression.FieldName))
                            {
                                return new TypeResolveResult(_callingClass, _callingMember, innerClass);
                            }
                        }
                    }
                }
            }
            return ResolveMethod(type, fieldReferenceExpression.FieldName);
        }

        public TextReader ExtractCurrentMethod(string fileContent)
        {
            if (_callingMember == null)
                return null;
            return ExtractMethod(fileContent, _callingMember, _language, _caretLine);
        }

        /// <summary>
        /// Creates a new class containing only the specified member.
        /// This is useful because we only want to parse current method for local variables,
        /// as all fields etc. are already prepared in the AST.
        /// </summary>
        public static TextReader ExtractMethod(string fileContent, IMember member,
                                               NR.SupportedLanguage language, int caretLine)
        {
            // As the parse information is always some seconds old, the end line could be wrong
            // if the user just inserted a line in the method.
            // We can ignore that case because it is sufficient for the parser when the first part of the
            // method body is ok.
            // Since we are operating directly on the edited buffer, the parser might not be
            // able to resolve invalid declarations.
            // We can ignore even that because the 'invalid line' is the line the user is currently
            // editing, and the declarations he is using are always above that line.


            // The ExtractMethod-approach has the advantage that the method contents do not have
            // do be parsed and stored in memory before they are needed.
            // Previous SharpDevelop versions always stored the SharpRefactory[VB] parse tree as 'Tag'
            // to the AST CompilationUnit.
            // This approach doesn't need that, so one could even go and implement a special parser
            // mode that does not parse the method bodies for the normal run (in the ParserUpdateThread or
            // SolutionLoadThread). That could improve the parser's speed dramatically.

            if (member.Region.IsEmpty) return null;
            int startLine = member.Region.BeginLine;
            if (startLine < 1) return null;
            DomRegion bodyRegion;
            if (member is IMethodOrProperty)
            {
                bodyRegion = ((IMethodOrProperty)member).BodyRegion;
            }
            else if (member is IEvent)
            {
                bodyRegion = ((IEvent)member).BodyRegion;
            }
            else
            {
                return null;
            }
            if (bodyRegion.IsEmpty) return null;
            int endLine = bodyRegion.EndLine;

            // Fix for SD2-511 (Code completion in inserted line)
            if (language == NR.SupportedLanguage.CSharp)
            {
                // Do not do this for VB: the parser does not correct create the
                // ForEachStatement when the method in truncated in the middle
                // VB does not have the "inserted line looks like variable declaration"-problem
                // anyways.
                if (caretLine > startLine && caretLine < endLine)
                    endLine = caretLine;
            }

            int offset = 0;
            for (int i = 0; i < startLine - 1; ++i)
            { // -1 because the startLine must be included
                offset = fileContent.IndexOf('\n', offset) + 1;
                if (offset <= 0) return null;
            }
            int startOffset = offset;
            for (int i = startLine - 1; i < endLine; ++i)
            {
                int newOffset = fileContent.IndexOf('\n', offset) + 1;
                if (newOffset <= 0) break;
                offset = newOffset;
            }
            int length = offset - startOffset;
            string classDecl, endClassDecl;
            if (language == NR.SupportedLanguage.VBNet)
            {
                classDecl = "Class A";
                endClassDecl = "End Class\n";
            }
            else
            {
                classDecl = "class A {";
                endClassDecl = "}\n";
            }
            System.Text.StringBuilder b = new System.Text.StringBuilder(classDecl, length + classDecl.Length + endClassDecl.Length + startLine - 1);
            b.Append('\n', startLine - 1);
            b.Append(fileContent, startOffset, length);
            b.Append(endClassDecl);
            return new System.IO.StringReader(b.ToString());
        }

        #region Resolve Identifier
        private ResolveResult ResolveIdentifier(string identifier, ExpressionContext context)
        {
            ResolveResult result = ResolveIdentifierInternal(identifier);
            if (result is TypeResolveResult)
                return result;

            ResolveResult result2 = null;

            IReturnType t = SearchType(identifier);
            if (t != null)
            {
                result2 = new TypeResolveResult(_callingClass, _callingMember, t);
            }
            else
            {
                if (_callingClass != null)
                {
                    if (_callingMember is IMethod)
                    {
                        foreach (ITypeParameter typeParameter in (_callingMember as IMethod).TypeParameters)
                        {
                            if (IsSameName(identifier, typeParameter.Name))
                            {
                                return new TypeResolveResult(_callingClass, _callingMember, new GenericReturnType(typeParameter));
                            }
                        }
                    }
                    foreach (ITypeParameter typeParameter in _callingClass.TypeParameters)
                    {
                        if (IsSameName(identifier, typeParameter.Name))
                        {
                            return new TypeResolveResult(_callingClass, _callingMember, new GenericReturnType(typeParameter));
                        }
                    }
                }
            }

            if (result == null) return result2;
            if (result2 == null) return result;
            if (context == ExpressionContext.Type)
                return result2;
            return new MixedResolveResult(result, result2);
        }

        private IField CreateLocalVariableField(LocalLookupVariable var, string identifier)
        {
            IReturnType type = GetVariableType(var);
            IField f = new DefaultField.LocalVariableField(type, identifier, new DomRegion(var.StartPos, var.EndPos), _callingClass);
            if (var.IsConst)
            {
                f.Modifiers |= ModifierEnum.Const;
            }
            return f;
        }

        private ResolveResult ResolveIdentifierInternal(string identifier)
        {
            if (_callingMember != null)
            { // LocalResolveResult requires callingMember to be set
                LocalLookupVariable var = SearchVariable(identifier);
                if (var != null)
                {
                    return new LocalResolveResult(_callingMember, CreateLocalVariableField(var, identifier));
                }
                IParameter para = SearchMethodParameter(identifier);
                if (para != null)
                {
                    IField field = new DefaultField.ParameterField(para.ReturnType, para.Name, para.Region, _callingClass);
                    return new LocalResolveResult(_callingMember, field);
                }
                if (IsSameName(identifier, "value"))
                {
                    IProperty property = _callingMember as IProperty;
                    if (property != null && property.SetterRegion.IsInside(_caretLine, _caretColumn))
                    {
                        IField field = new DefaultField.ParameterField(property.ReturnType, "value", property.Region, _callingClass);
                        return new LocalResolveResult(_callingMember, field);
                    }
                }
            }
            if (_callingClass != null)
            {
                IMember member = GetMember(_callingClass.DefaultReturnType, identifier);
                if (member != null)
                {
                    return CreateMemberResolveResult(member);
                }
                ResolveResult result = ResolveMethod(_callingClass.DefaultReturnType, identifier);
                if (result != null)
                    return result;
            }

            // try if there exists a static member in outer classes named typeName
            List<IClass> classes = _cu.GetOuterClasses(_caretLine, _caretColumn);
            foreach (IClass c in classes)
            {
                IMember member = GetMember(c.DefaultReturnType, identifier);
                if (member != null && member.IsStatic)
                {
                    return new MemberResolveResult(_callingClass, _callingMember, member);
                }
            }

            string namespaceName = SearchNamespace(identifier);
            if (namespaceName != null && namespaceName.Length > 0)
            {
                return new NamespaceResolveResult(_callingClass, _callingMember, namespaceName);
            }

            if (_languageProperties.CanImportClasses)
            {
                foreach (IUsing @using in _cu.Usings)
                {
                    foreach (string import in @using.Usings)
                    {
                        IClass c = GetClass(import);
                        if (c != null)
                        {
                            IMember member = GetMember(c.DefaultReturnType, identifier);
                            if (member != null)
                            {
                                return CreateMemberResolveResult(member);
                            }
                            ResolveResult result = ResolveMethod(c.DefaultReturnType, identifier);
                            if (result != null)
                                return result;
                        }
                    }
                }
            }

            if (_languageProperties.ImportModules)
            {
                ArrayList list = new ArrayList();
                CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, _cu, _callingClass);
                foreach (object o in list)
                {
                    IClass c = o as IClass;
                    if (c != null && IsSameName(identifier, c.Name))
                    {
                        return new TypeResolveResult(_callingClass, _callingMember, c);
                    }
                    IMember member = o as IMember;
                    if (member != null && IsSameName(identifier, member.Name))
                    {
                        if (member is IMethod)
                        {
                            return new MethodResolveResult(_callingClass, _callingMember, member.DeclaringType.DefaultReturnType, member.Name);
                        }
                        else
                        {
                            return CreateMemberResolveResult(member);
                        }
                    }
                }
            }

            return null;
        }
        #endregion

        private ResolveResult CreateMemberResolveResult(IMember member)
        {
            if (member == null) return null;
            return new MemberResolveResult(_callingClass, _callingMember, member);
        }

        #region ResolveMethod
        private ResolveResult ResolveMethod(IReturnType type, string identifier)
        {
            if (type == null)
                return null;
            foreach (IMethod method in type.GetMethods())
            {
                if (IsSameName(identifier, method.Name))
                    return new MethodResolveResult(_callingClass, _callingMember, type, identifier);
            }
            if (_languageProperties.SupportsExtensionMethods && _callingClass != null)
            {
                ArrayList list = new ArrayList();
                ResolveResult.AddExtensions(_languageProperties, list, _callingClass, type);
                foreach (IMethodOrProperty mp in list)
                {
                    if (mp is IMethod && IsSameName(mp.Name, identifier))
                    {
                        return new MethodResolveResult(_callingClass, _callingMember, type, identifier);
                    }
                }
            }
            return null;
        }
        #endregion

        private Expression SpecialConstructs(string expression)
        {
            if (_language == NR.SupportedLanguage.VBNet)
            {
                // MyBase and MyClass are no expressions, only MyBase.Identifier and MyClass.Identifier
                if ("mybase".Equals(expression, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new BaseReferenceExpression();
                }
                else if ("myclass".Equals(expression, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new ClassReferenceExpression();
                } // Global is handled in Resolve() because we don't need an expression for that
            }
            else if (_language == NR.SupportedLanguage.CSharp)
            {
                // generic type names are no expressions, only property access on them is an expression
                if (expression.EndsWith(">"))
                {
                    FieldReferenceExpression expr = ParseExpression(expression + ".Prop") as FieldReferenceExpression;
                    if (expr != null)
                    {
                        return expr.TargetObject;
                    }
                }
                return null;
            }
            return null;
        }

        public bool IsSameName(string name1, string name2)
        {
            return _languageProperties.NameComparer.Equals(name1, name2);
        }

        private bool IsInside(NR.Location between, NR.Location start, NR.Location end)
        {
            if (between.Y < start.Y || between.Y > end.Y)
            {
                return false;
            }
            if (between.Y > start.Y)
            {
                if (between.Y < end.Y)
                {
                    return true;
                }
                // between.Y == end.Y
                return between.X <= end.X;
            }
            // between.Y == start.Y
            if (between.X < start.X)
            {
                return false;
            }
            // start is OK and between.Y <= end.Y
            return between.Y < end.Y || between.X <= end.X;
        }

        private IMember GetCurrentMember()
        {
            if (_callingClass == null)
                return null;
            foreach (IMethod method in _callingClass.Methods)
            {
                if (method.Region.IsInside(_caretLine, _caretColumn) || method.BodyRegion.IsInside(_caretLine, _caretColumn))
                {
                    return method;
                }
            }
            foreach (IProperty property in _callingClass.Properties)
            {
                if (property.Region.IsInside(_caretLine, _caretColumn) || property.BodyRegion.IsInside(_caretLine, _caretColumn))
                {
                    return property;
                }
            }
            return null;
        }

        /// <remarks>
        /// use the usings to find the correct name of a namespace
        /// </remarks>
        public string SearchNamespace(string name)
        {
            return _projectContent.SearchNamespace(name, _callingClass, _cu, _caretLine, _caretColumn);
        }

        public IClass GetClass(string fullName)
        {
            return _projectContent.GetClass(fullName);
        }

        /// <remarks>
        /// use the usings and the name of the namespace to find a class
        /// </remarks>
        public IClass SearchClass(string name)
        {
            IReturnType t = SearchType(name);
            return (t != null) ? t.GetUnderlyingClass() : null;
        }

        public IReturnType SearchType(string name)
        {
            return _projectContent.SearchType(new SearchTypeRequest(name, 0, _callingClass, _cu, _caretLine, _caretColumn)).Result;
        }

        #region Helper for TypeVisitor
        #region SearchMethod

        public List<IMethod> SearchMethod(string memberName)
        {
            List<IMethod> methods = SearchMethod(_callingClass.DefaultReturnType, memberName);
            if (methods.Count > 0)
                return methods;

            if (_languageProperties.CanImportClasses)
            {
                foreach (IUsing @using in _cu.Usings)
                {
                    foreach (string import in @using.Usings)
                    {
                        IClass c = _projectContent.GetClass(import, 0);
                        if (c != null)
                        {
                            methods = SearchMethod(c.DefaultReturnType, memberName);
                            if (methods.Count > 0)
                                return methods;
                        }
                    }
                }
            }

            if (_languageProperties.ImportModules)
            {
                ArrayList list = new ArrayList();
                CtrlSpaceResolveHelper.AddImportedNamespaceContents(list, _cu, _callingClass);
                foreach (object o in list)
                {
                    IMethod m = o as IMethod;
                    if (m != null && IsSameName(m.Name, memberName))
                    {
                        methods.Add(m);
                    }
                }
            }
            return methods;
        }

        /// <summary>
        /// Gets the list of methods on the return type that have the specified name.
        /// </summary>
        public List<IMethod> SearchMethod(IReturnType type, string memberName)
        {
            List<IMethod> methods = new List<IMethod>();
            if (type == null)
                return methods;

            bool isClassInInheritanceTree = false;
            if (_callingClass != null)
                isClassInInheritanceTree = _callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());

            foreach (IMethod m in type.GetMethods())
            {
                if (IsSameName(m.Name, memberName)
                    && m.IsAccessible(_callingClass, isClassInInheritanceTree)
                   )
                {
                    methods.Add(m);
                }
            }
            if (methods.Count == 0)
            {
                if (_languageProperties.SupportsExtensionMethods && _callingClass != null)
                {
                    ArrayList list = new ArrayList();
                    ResolveResult.AddExtensions(_languageProperties, list, _callingClass, type);
                    foreach (IMethodOrProperty mp in list)
                    {
                        if (mp is IMethod && IsSameName(mp.Name, memberName))
                        {
                            methods.Add((IMethod)mp);
                        }
                    }
                }
            }

            return methods;
        }
        #endregion

        #region SearchMember
        // no methods or indexer
        public IReturnType SearchMember(IReturnType type, string memberName)
        {
            if (type == null)
                return null;
            IMember member = GetMember(type, memberName);
            if (member == null)
                return null;
            else
                return member.ReturnType;
        }

        public IMember GetMember(IReturnType type, string memberName)
        {
            if (type == null)
                return null;
            bool isClassInInheritanceTree = false;
            if (_callingClass != null)
                isClassInInheritanceTree = _callingClass.IsTypeInInheritanceTree(type.GetUnderlyingClass());
            foreach (IProperty p in type.GetProperties())
            {
                if (IsSameName(p.Name, memberName))
                {
                    return p;
                }
            }
            foreach (IField f in type.GetFields())
            {
                if (IsSameName(f.Name, memberName))
                {
                    return f;
                }
            }
            foreach (IEvent e in type.GetEvents())
            {
                if (IsSameName(e.Name, memberName))
                {
                    return e;
                }
            }
            return null;
        }
        #endregion

        #region DynamicLookup
        /// <remarks>
        /// does the dynamic lookup for the identifier
        /// </remarks>
        public IReturnType DynamicLookup(string identifier)
        {
            ResolveResult rr = ResolveIdentifierInternal(identifier);
            if (rr is NamespaceResolveResult)
            {
                return new TypeVisitor.NamespaceReturnType((rr as NamespaceResolveResult).Name);
            }
            return (rr != null) ? rr.ResolvedType : null;
        }

        private IParameter SearchMethodParameter(string parameter)
        {
            IMethodOrProperty method = _callingMember as IMethodOrProperty;
            if (method == null)
            {
                return null;
            }
            foreach (IParameter p in method.Parameters)
            {
                if (IsSameName(p.Name, parameter))
                {
                    return p;
                }
            }
            return null;
        }

        private IReturnType GetVariableType(LocalLookupVariable v)
        {
            if (v == null)
            {
                return null;
            }
            return TypeVisitor.CreateReturnType(v.TypeRef, this);
        }

        private LocalLookupVariable SearchVariable(string name)
        {
            if (_lookupTableVisitor == null || !_lookupTableVisitor.Variables.ContainsKey(name))
                return null;
            List<LocalLookupVariable> variables = _lookupTableVisitor.Variables[name];
            if (variables.Count <= 0)
            {
                return null;
            }

            foreach (LocalLookupVariable v in variables)
            {
                if (IsInside(new NR.Location(_caretColumn, _caretLine), v.StartPos, v.EndPos))
                {
                    return v;
                }
            }
            return null;
        }
        #endregion
        #endregion

        private IClass GetPrimitiveClass(string systemType, string newName)
        {
            IClass c = _projectContent.GetClass(systemType);
            if (c == null)
                return null;
            DefaultClass c2 = new DefaultClass(c.CompilationUnit, newName);
            c2.ClassType = c.ClassType;
            c2.Modifiers = c.Modifiers;
            c2.Documentation = c.Documentation;
            c2.BaseTypes.AddRange(c.BaseTypes);
            c2.Methods.AddRange(c.Methods);
            c2.Fields.AddRange(c.Fields);
            c2.Properties.AddRange(c.Properties);
            c2.Events.AddRange(c.Events);
            return c2;
        }

        public ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context)
        {
            ArrayList result = new ArrayList();
            if (_language == NR.SupportedLanguage.VBNet)
            {
                foreach (KeyValuePair<string, string> pair in TypeReference.PrimitiveTypesVB)
                {
                    if ("System." + pair.Key != pair.Value)
                    {
                        IClass c = GetPrimitiveClass(pair.Value, pair.Key);
                        if (c != null) result.Add(c);
                    }
                }
                result.Add("Global");
                result.Add("New");
            }
            else
            {
                foreach (KeyValuePair<string, string> pair in TypeReference.PrimitiveTypesCSharp)
                {
                    IClass c = GetPrimitiveClass(pair.Value, pair.Key);
                    if (c != null) result.Add(c);
                }
            }
            ParseInformation parseInfo = HostCallback.GetParseInformation(fileName);
            if (parseInfo == null)
            {
                return null;
            }

            _caretLine = caretLine;
            _caretColumn = caretColumn;

            _lookupTableVisitor = new LookupTableVisitor(_language);

            _cu = parseInfo.MostRecentCompilationUnit;

            if (_cu != null)
            {
                _callingClass = _cu.GetInnermostClass(caretLine, caretColumn);
            }

            _callingMember = GetCurrentMember();
            if (_callingMember != null)
            {
                CompilationUnit parsedCu = ParseCurrentMemberAsCompilationUnit(fileContent);
                if (parsedCu != null)
                {
                    _lookupTableVisitor.VisitCompilationUnit(parsedCu, null);
                }
            }

            CtrlSpaceResolveHelper.AddContentsFromCalling(result, _callingClass, _callingMember);

            foreach (KeyValuePair<string, List<LocalLookupVariable>> pair in _lookupTableVisitor.Variables)
            {
                if (pair.Value != null && pair.Value.Count > 0)
                {
                    foreach (LocalLookupVariable v in pair.Value)
                    {
                        if (IsInside(new NR.Location(caretColumn, caretLine), v.StartPos, v.EndPos))
                        {
                            // convert to a field for display
                            result.Add(CreateLocalVariableField(v, pair.Key));
                            break;
                        }
                    }
                }
            }
            if (_callingMember is IProperty)
            {
                IProperty property = (IProperty)_callingMember;
                if (property.SetterRegion.IsInside(caretLine, caretColumn))
                {
                    result.Add(new DefaultField.ParameterField(property.ReturnType, "value", property.Region, _callingClass));
                }
            }

            CtrlSpaceResolveHelper.AddImportedNamespaceContents(result, _cu, _callingClass);
            return result;
        }
    }
}
