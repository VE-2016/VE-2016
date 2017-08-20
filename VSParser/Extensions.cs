using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace WindowsFormsApp3
{
    public static partial class ISymbolExtensions
    {
        //        public static string ToNameDisplayString(this Microsoft.CodeAnalysis.ISymbol symbol)
        //        {
        //            return symbol.ToDisplayString(SymbolDisplayFormats.NameFormat);
        //        }

        //        public static string ToSignatureDisplayString(this ISymbol symbol)
        //        {
        //            return symbol.ToDisplayString(SymbolDisplayFormats.SignatureFormat);
        //        }

        public static List<INamedTypeSymbol> BaseTypes(this ITypeSymbol symbol)
        {
            List<INamedTypeSymbol> b = new List<INamedTypeSymbol>();
            while (symbol.BaseType != null)
            {

                b.Add(symbol.BaseType);

                symbol = symbol.BaseType;
            }

            return b;

        }


        //        public static SymbolVisibility GetResultantVisibility(this ISymbol symbol)
        //        {
        //            // Start by assuming it's visible.
        //            var visibility = SymbolVisibility.Public;

        //            switch (symbol.Kind)
        //            {
        //                case SymbolKind.Alias:
        //                    // Aliases are uber private.  They're only visible in the same file that they
        //                    // were declared in.
        //                    return SymbolVisibility.Private;

        //                case SymbolKind.Parameter:
        //                    // Parameters are only as visible as their containing symbol
        //                    return GetResultantVisibility(symbol.ContainingSymbol);

        //                case SymbolKind.TypeParameter:
        //                    // Type Parameters are private.
        //                    return SymbolVisibility.Private;
        //            }

        //            while (symbol != null && symbol.Kind != SymbolKind.Namespace)
        //            {
        //                switch (symbol.DeclaredAccessibility)
        //                {
        //                    // If we see anything private, then the symbol is private.
        //                    case Accessibility.NotApplicable:
        //                    case Accessibility.Private:
        //                        return SymbolVisibility.Private;

        //                    // If we see anything internal, then knock it down from public to
        //                    // internal.
        //                    case Accessibility.Internal:
        //                    case Accessibility.ProtectedAndInternal:
        //                        visibility = SymbolVisibility.Internal;
        //                        break;

        //                        // For anything else (Public, Protected, ProtectedOrInternal), the
        //                        // symbol stays at the level we've gotten so far.
        //                }

        //                symbol = symbol.ContainingSymbol;
        //            }

        //            return visibility;
        //        }

        //        public static ISymbol OverriddenMember(this ISymbol symbol)
        //        {
        //            switch (symbol.Kind)
        //            {
        //                case SymbolKind.Event:
        //                    return ((IEventSymbol)symbol).OverriddenEvent;

        //                case SymbolKind.Method:
        //                    return ((IMethodSymbol)symbol).OverriddenMethod;

        //                case SymbolKind.Property:
        //                    return ((IPropertySymbol)symbol).OverriddenProperty;
        //            }

        //            return null;
        //        }

        //        public static ImmutableArray<ISymbol> ExplicitInterfaceImplementations(this ISymbol symbol)
        //        {
        //            switch (symbol)
        //            {
        //                case IEventSymbol @event: return ImmutableArray<ISymbol>.CastUp(@event.ExplicitInterfaceImplementations);
        //                case IMethodSymbol method: return ImmutableArray<ISymbol>.CastUp(method.ExplicitInterfaceImplementations);
        //                case IPropertySymbol property: return ImmutableArray<ISymbol>.CastUp(property.ExplicitInterfaceImplementations);
        //                default: return ImmutableArray.Create<ISymbol>();
        //            }
        //        }

        //        public static bool IsOverridable(this ISymbol symbol)
        //        {
        //            // Members can only have overrides if they are virtual, abstract or override and is not
        //            // sealed.
        //            return symbol?.ContainingType?.TypeKind == TypeKind.Class &&
        //                   (symbol.IsVirtual || symbol.IsAbstract || symbol.IsOverride) &&
        //                   !symbol.IsSealed;
        //        }

        //        public static bool IsImplementableMember(this ISymbol symbol)
        //        {
        //            if (symbol != null &&
        //                symbol.ContainingType != null &&
        //                symbol.ContainingType.TypeKind == TypeKind.Interface)
        //            {
        //                if (symbol.Kind == SymbolKind.Event)
        //                {
        //                    return true;
        //                }

        //                if (symbol.Kind == SymbolKind.Property)
        //                {
        //                    return true;
        //                }

        //                if (symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).MethodKind == MethodKind.Ordinary)
        //                {
        //                    return true;
        //                }
        //            }

        //            return false;
        //        }

        //        public static INamedTypeSymbol GetContainingTypeOrThis(this ISymbol symbol)
        //        {
        //            if (symbol is INamedTypeSymbol namedType)
        //            {
        //                return namedType;
        //            }

        //            return symbol.ContainingType;
        //        }

        //        public static bool IsPointerType(this ISymbol symbol)
        //        {
        //            return symbol is IPointerTypeSymbol;
        //        }

        //        public static bool IsErrorType(this ISymbol symbol)
        //        {
        //            return (symbol as ITypeSymbol)?.IsErrorType() == true;
        //        }

        //        public static bool IsModuleType(this ISymbol symbol)
        //        {
        //            return (symbol as ITypeSymbol)?.IsModuleType() == true;
        //        }

        //        public static bool IsInterfaceType(this ISymbol symbol)
        //        {
        //            return (symbol as ITypeSymbol)?.IsInterfaceType() == true;
        //        }

        //        public static bool IsArrayType(this ISymbol symbol)
        //        {
        //            return symbol?.Kind == SymbolKind.ArrayType;
        //        }

        //        public static bool IsTupleType(this ISymbol symbol)
        //        {
        //            return (symbol as ITypeSymbol)?.IsTupleType ?? false;
        //        }

        //        public static bool IsAnonymousFunction(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.AnonymousFunction;
        //        }

        //        public static bool IsKind(this ISymbol symbol, SymbolKind kind)
        //        {
        //            return symbol.MatchesKind(kind);
        //        }

        //        public static bool MatchesKind(this ISymbol symbol, SymbolKind kind)
        //        {
        //            return symbol?.Kind == kind;
        //        }

        //        public static bool MatchesKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2)
        //        {
        //            return symbol != null
        //                && (symbol.Kind == kind1 || symbol.Kind == kind2);
        //        }

        //        public static bool MatchesKind(this ISymbol symbol, SymbolKind kind1, SymbolKind kind2, SymbolKind kind3)
        //        {
        //            return symbol != null
        //                && (symbol.Kind == kind1 || symbol.Kind == kind2 || symbol.Kind == kind3);
        //        }

        //        public static bool MatchesKind(this ISymbol symbol, params SymbolKind[] kinds)
        //        {
        //            return symbol != null
        //                && kinds.Contains(symbol.Kind);
        //        }

        //        public static bool IsReducedExtension(this ISymbol symbol)
        //        {
        //            return symbol is IMethodSymbol && ((IMethodSymbol)symbol).MethodKind == MethodKind.ReducedExtension;
        //        }

        //        public static bool IsExtensionMethod(this ISymbol symbol)
        //        {
        //            return symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).IsExtensionMethod;
        //        }

        //        public static bool IsLocalFunction(this ISymbol symbol)
        //        {
        //            return symbol != null && symbol.Kind == SymbolKind.Method && ((IMethodSymbol)symbol).MethodKind == MethodKind.LocalFunction;
        //        }

        //        public static bool IsModuleMember(this ISymbol symbol)
        //        {
        //            return symbol != null && symbol.ContainingSymbol is INamedTypeSymbol && symbol.ContainingType.TypeKind == TypeKind.Module;
        //        }

        //        public static bool IsConstructor(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Constructor;
        //        }

        //        public static bool IsStaticConstructor(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.StaticConstructor;
        //        }

        //        public static bool IsDestructor(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Destructor;
        //        }

        //        public static bool IsUserDefinedOperator(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.UserDefinedOperator;
        //        }

        //        public static bool IsConversion(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Conversion;
        //        }

        //        public static bool IsOrdinaryMethod(this ISymbol symbol)
        //        {
        //            return (symbol as IMethodSymbol)?.MethodKind == MethodKind.Ordinary;
        //        }

        //        public static bool IsOrdinaryMethodOrLocalFunction(this ISymbol symbol)
        //        {
        //            if (!(symbol is IMethodSymbol method))
        //            {
        //                return false;
        //            }

        //            return method.MethodKind == MethodKind.Ordinary
        //                || method.MethodKind == MethodKind.LocalFunction;
        //        }

        //        public static bool IsDelegateType(this ISymbol symbol)
        //        {
        //            return symbol is ITypeSymbol && ((ITypeSymbol)symbol).TypeKind == TypeKind.Delegate;
        //        }

        //        public static bool IsAnonymousType(this ISymbol symbol)
        //        {
        //            return symbol is INamedTypeSymbol && ((INamedTypeSymbol)symbol).IsAnonymousType;
        //        }

        //        public static bool IsNormalAnonymousType(this ISymbol symbol)
        //        {
        //            return symbol.IsAnonymousType() && !symbol.IsDelegateType();
        //        }

        //        public static bool IsAnonymousDelegateType(this ISymbol symbol)
        //        {
        //            return symbol.IsAnonymousType() && symbol.IsDelegateType();
        //        }

        //        public static bool IsAnonymousTypeProperty(this ISymbol symbol)
        //            => symbol is IPropertySymbol && symbol.ContainingType.IsNormalAnonymousType();

        //        public static bool IsTupleField(this ISymbol symbol)
        //            => symbol is IFieldSymbol && symbol.ContainingType.IsTupleType;

        //        public static bool IsIndexer(this ISymbol symbol)
        //        {
        //            return (symbol as IPropertySymbol)?.IsIndexer == true;
        //        }

        //        public static bool IsWriteableFieldOrProperty(this ISymbol symbol)
        //        {
        //            var fieldSymbol = symbol as IFieldSymbol;
        //            if (fieldSymbol != null)
        //            {
        //                return !fieldSymbol.IsReadOnly
        //                    && !fieldSymbol.IsConst;
        //            }

        //            var propertySymbol = symbol as IPropertySymbol;
        //            if (propertySymbol != null)
        //            {
        //                return !propertySymbol.IsReadOnly;
        //            }

        //            return false;
        //        }

        //        public static ITypeSymbol GetMemberType(this ISymbol symbol)
        //        {
        //            switch (symbol.Kind)
        //            {
        //                case SymbolKind.Field:
        //                    return ((IFieldSymbol)symbol).Type;
        //                case SymbolKind.Property:
        //                    return ((IPropertySymbol)symbol).Type;
        //                case SymbolKind.Method:
        //                    return ((IMethodSymbol)symbol).ReturnType;
        //                case SymbolKind.Event:
        //                    return ((IEventSymbol)symbol).Type;
        //            }

        //            return null;
        //        }

        //        public static int GetArity(this ISymbol symbol)
        //        {
        //            switch (symbol.Kind)
        //            {
        //                case SymbolKind.NamedType:
        //                    return ((INamedTypeSymbol)symbol).Arity;
        //                case SymbolKind.Method:
        //                    return ((IMethodSymbol)symbol).Arity;
        //                default:
        //                    return 0;
        //            }
        //        }

        //        public static ISymbol GetOriginalUnreducedDefinition(this ISymbol symbol)
        //        {
        //            if (symbol.IsReducedExtension())
        //            {
        //                // note: ReducedFrom is only a method definition and includes no type arguments.
        //                symbol = ((IMethodSymbol)symbol).GetConstructedReducedFrom();
        //            }

        //            if (symbol.IsFunctionValue())
        //            {
        //                var method = symbol.ContainingSymbol as IMethodSymbol;
        //                if (method != null)
        //                {
        //                    symbol = method;

        //                    if (method.AssociatedSymbol != null)
        //                    {
        //                        symbol = method.AssociatedSymbol;
        //                    }
        //                }
        //            }

        //            if (symbol.IsNormalAnonymousType() || symbol.IsAnonymousTypeProperty())
        //            {
        //                return symbol;
        //            }

        //            var parameter = symbol as IParameterSymbol;
        //            if (parameter != null)
        //            {
        //                var method = parameter.ContainingSymbol as IMethodSymbol;
        //                if (method?.IsReducedExtension() == true)
        //                {
        //                    symbol = method.GetConstructedReducedFrom().Parameters[parameter.Ordinal + 1];
        //                }
        //            }

        //            return symbol?.OriginalDefinition;
        //        }

        //        public static bool IsFunctionValue(this ISymbol symbol)
        //        {
        //            return symbol is ILocalSymbol && ((ILocalSymbol)symbol).IsFunctionValue;
        //        }

        //        public static bool IsThisParameter(this ISymbol symbol)
        //            => symbol?.Kind == SymbolKind.Parameter && ((IParameterSymbol)symbol).IsThis;

        //        public static ISymbol ConvertThisParameterToType(this ISymbol symbol)
        //        {
        //            if (symbol.IsThisParameter())
        //            {
        //                return ((IParameterSymbol)symbol).Type;
        //            }

        //            return symbol;
        //        }

        //        public static bool IsParams(this ISymbol symbol)
        //        {
        //            var parameters = symbol.GetParameters();
        //            return parameters.Length > 0 && parameters[parameters.Length - 1].IsParams;
        //        }

        //        public static ImmutableArray<IParameterSymbol> GetParameters(this ISymbol symbol)
        //        {
        //            switch (symbol)
        //            {
        //                case IMethodSymbol m: return m.Parameters;
        //                case IPropertySymbol nt: return nt.Parameters;
        //                default: return ImmutableArray.Create<IParameterSymbol>();
        //            }
        //        }

        //        public static ImmutableArray<ITypeParameterSymbol> GetTypeParameters(this ISymbol symbol)
        //        {
        //            switch (symbol)
        //            {
        //                case IMethodSymbol m: return m.TypeParameters;
        //                case INamedTypeSymbol nt: return nt.TypeParameters;
        //                default: return ImmutableArray.Create<ITypeParameterSymbol>();
        //            }
        //        }

        //        public static ImmutableArray<ITypeSymbol> GetTypeArguments(this ISymbol symbol)
        //        {
        //            switch (symbol)
        //            {
        //                case IMethodSymbol m: return m.TypeArguments;
        //                case INamedTypeSymbol nt: return nt.TypeArguments;
        //                default: return ImmutableArray.Create<ITypeSymbol>();
        //            }
        //        }

        //        public static ImmutableArray<ITypeSymbol> GetAllTypeArguments(this ISymbol symbol)
        //        {
        //            var results = ArrayBuilder<ITypeSymbol>.GetInstance();
        //            results.AddRange(symbol.GetTypeArguments());

        //            var containingType = symbol.ContainingType;
        //            while (containingType != null)
        //            {
        //                results.AddRange(containingType.GetTypeArguments());
        //                containingType = containingType.ContainingType;
        //            }

        //            return results.ToImmutableAndFree();
        //        }

        //        public static bool IsAttribute(this ISymbol symbol)
        //        {
        //            return (symbol as ITypeSymbol)?.IsAttribute() == true;
        //        }





        //        public static bool IsStaticType(this ISymbol symbol)
        //        {
        //            return symbol != null && symbol.Kind == SymbolKind.NamedType && symbol.IsStatic;
        //        }

        //        public static bool IsNamespace(this ISymbol symbol)
        //        {
        //            return symbol?.Kind == SymbolKind.Namespace;
        //        }

        //        public static bool IsOrContainsAccessibleAttribute(this ISymbol symbol, ISymbol withinType, IAssemblySymbol withinAssembly)
        //        {
        //            var alias = symbol as IAliasSymbol;
        //            if (alias != null)
        //            {
        //                symbol = alias.Target;
        //            }

        //            var namespaceOrType = symbol as INamespaceOrTypeSymbol;
        //            if (namespaceOrType == null)
        //            {
        //                return false;
        //            }



        //            // PERF: Avoid allocating a lambda capture as this method is recursive
        //            foreach (var namedType in namespaceOrType.GetTypeMembers())
        //            {
        //                if (namedType.IsOrContainsAccessibleAttribute(withinType, withinAssembly))
        //                {
        //                    return true;
        //                }
        //            }

        //            return false;
        //        }

        //        public static IEnumerable<IPropertySymbol> GetValidAnonymousTypeProperties(this ISymbol symbol)
        //        {
        //           // System.Diagnostics.Contracts.Contract.ThrowIfFalse(symbol.IsNormalAnonymousType());
        //            return ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>().Where(p => p.CanBeReferencedByName);
        //        }

        //        public static Accessibility ComputeResultantAccessibility(this ISymbol symbol, ITypeSymbol finalDestination)
        //        {
        //            if (symbol == null)
        //            {
        //                return Accessibility.Private;
        //            }

        //            switch (symbol.DeclaredAccessibility)
        //            {
        //                default:
        //                    return symbol.DeclaredAccessibility;
        //                case Accessibility.ProtectedAndInternal:
        //                    return symbol.ContainingAssembly.GivesAccessTo(finalDestination.ContainingAssembly)
        //                        ? Accessibility.ProtectedAndInternal
        //                        : Accessibility.Internal;
        //                case Accessibility.ProtectedOrInternal:
        //                    return symbol.ContainingAssembly.GivesAccessTo(finalDestination.ContainingAssembly)
        //                        ? Accessibility.ProtectedOrInternal
        //                        : Accessibility.Protected;
        //            }
        //        }

        //        /// <returns>
        //        /// Returns true if symbol is a local variable and its declaring syntax node is 
        //        /// after the current position, false otherwise (including for non-local symbols)
        //        /// </returns>
        //        public static bool IsInaccessibleLocal(this ISymbol symbol, int position)
        //        {
        //            if (symbol.Kind != SymbolKind.Local)
        //            {
        //                return false;
        //            }

        //            // Implicitly declared locals (with Option Explicit Off in VB) are scoped to the entire
        //            // method and should always be considered accessible from within the same method.
        //            if (symbol.IsImplicitlyDeclared)
        //            {
        //                return false;
        //            }

        //            var declarationSyntax = symbol.DeclaringSyntaxReferences.Select(r => r.GetSyntax()).FirstOrDefault();
        //            return declarationSyntax != null && position < declarationSyntax.SpanStart;
        //        }




        //        private const int TypeLibTypeFlagsFHidden = 0x0010;
        //        private const int TypeLibFuncFlagsFHidden = 0x0040;
        //        private const int TypeLibVarFlagsFHidden = 0x0040;

        //        private static bool IsBrowsingProhibitedByTypeLibAttributeWorker(
        //            ISymbol symbol, ImmutableArray<AttributeData> attributes, List<IMethodSymbol> attributeConstructors, int hiddenFlag)
        //        {
        //            foreach (var attribute in attributes)
        //            {
        //                if (attribute.ConstructorArguments.Length == 1)
        //                {
        //                    foreach (var constructor in attributeConstructors)
        //                    {
        //                        if (attribute.AttributeConstructor == constructor)
        //                        {
        //                            var actualFlags = 0;

        //                            // Check for both constructor signatures. The constructor that takes a TypeLib*Flags reports an int argument.
        //                            var argumentValue = attribute.ConstructorArguments.First().Value;

        //                            if (argumentValue is int i)
        //                            {
        //                                actualFlags = i;
        //                            }
        //                            else if (argumentValue is short sh)
        //                            {
        //                                actualFlags = sh;
        //                            }
        //                            else
        //                            {
        //                                continue;
        //                            }

        //                            if ((actualFlags & hiddenFlag) == hiddenFlag)
        //                            {
        //                                return true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            return false;
        //        }





        //        public static bool IsEventAccessor(this ISymbol symbol)
        //        {
        //            var method = symbol as IMethodSymbol;
        //            return method != null &&
        //                (method.MethodKind == MethodKind.EventAdd ||
        //                 method.MethodKind == MethodKind.EventRaise ||
        //                 method.MethodKind == MethodKind.EventRemove);
        //        }

        //        public static bool IsFromSource(this ISymbol symbol)
        //            => symbol.Locations.Any() && symbol.Locations.All(location => location.IsInSource);

        //        public static bool IsNonImplicitAndFromSource(this ISymbol symbol)
        //            => !symbol.IsImplicitlyDeclared && symbol.IsFromSource();



        //        public static ITypeSymbol GetSymbolType(this ISymbol symbol)
        //        {
        //            var localSymbol = symbol as ILocalSymbol;
        //            if (localSymbol != null)
        //            {
        //                return localSymbol.Type;
        //            }

        //            var fieldSymbol = symbol as IFieldSymbol;
        //            if (fieldSymbol != null)
        //            {
        //                return fieldSymbol.Type;
        //            }

        //            var propertySymbol = symbol as IPropertySymbol;
        //            if (propertySymbol != null)
        //            {
        //                return propertySymbol.Type;
        //            }

        //            var parameterSymbol = symbol as IParameterSymbol;
        //            if (parameterSymbol != null)
        //            {
        //                return parameterSymbol.Type;
        //            }

        //            var aliasSymbol = symbol as IAliasSymbol;
        //            if (aliasSymbol != null)
        //            {
        //                return aliasSymbol.Target as ITypeSymbol;
        //            }

        //            return symbol as ITypeSymbol;
        //        }



        //        /// <summary>
        //        /// If the <paramref name="symbol"/> is a method symbol, returns <see langword="true"/> if the method's return type is "awaitable", but not if it's <see langword="dynamic"/>.
        //        /// If the <paramref name="symbol"/> is a type symbol, returns <see langword="true"/> if that type is "awaitable".
        //        /// An "awaitable" is any type that exposes a GetAwaiter method which returns a valid "awaiter". This GetAwaiter method may be an instance method or an extension method.
        //        /// </summary>
        //        public static bool IsAwaitableNonDynamic(this ISymbol symbol, SemanticModel semanticModel, int position)
        //        {
        //            IMethodSymbol methodSymbol = symbol as IMethodSymbol;
        //            ITypeSymbol typeSymbol = null;

        //            if (methodSymbol == null)
        //            {
        //                typeSymbol = symbol as ITypeSymbol;
        //                if (typeSymbol == null)
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                if (methodSymbol.ReturnType == null)
        //                {
        //                    return false;
        //                }
        //            }

        //            // otherwise: needs valid GetAwaiter
        //            var potentialGetAwaiters = semanticModel.LookupSymbols(position,
        //                                                                   container: typeSymbol ?? methodSymbol.ReturnType.OriginalDefinition,
        //                                                                   name: WellKnownMemberNames.GetAwaiter,
        //                                                                   includeReducedExtensionMethods: true);
        //            var getAwaiters = potentialGetAwaiters.OfType<IMethodSymbol>().Where(x => !x.Parameters.Any());
        //            return getAwaiters.Any(VerifyGetAwaiter);
        //        }

        //        private static bool VerifyGetAwaiter(IMethodSymbol getAwaiter)
        //        {
        //            var returnType = getAwaiter.ReturnType;
        //            if (returnType == null)
        //            {
        //                return false;
        //            }

        //            // bool IsCompleted { get }
        //            if (!returnType.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == WellKnownMemberNames.IsCompleted && p.Type.SpecialType == SpecialType.System_Boolean && p.GetMethod != null))
        //            {
        //                return false;
        //            }

        //            var methods = returnType.GetMembers().OfType<IMethodSymbol>();

        //            // NOTE: (vladres) The current version of C# Spec, §7.7.7.3 'Runtime evaluation of await expressions', requires that
        //            // NOTE: the interface method INotifyCompletion.OnCompleted or ICriticalNotifyCompletion.UnsafeOnCompleted is invoked
        //            // NOTE: (rather than any OnCompleted method conforming to a certain pattern).
        //            // NOTE: Should this code be updated to match the spec?

        //            // void OnCompleted(Action) 
        //            // Actions are delegates, so we'll just check for delegates.
        //            if (!methods.Any(x => x.Name == WellKnownMemberNames.OnCompleted && x.ReturnsVoid && x.Parameters.Length == 1 && x.Parameters.First().Type.TypeKind == TypeKind.Delegate))
        //            {
        //                return false;
        //            }

        //            // void GetResult() || T GetResult()
        //            return methods.Any(m => m.Name == WellKnownMemberNames.GetResult && !m.Parameters.Any());
        //        }




        //    }
        //    public static class SymbolDisplayFormats
        //    {
        //        /// <summary>
        //        /// Standard format for displaying to the user.
        //        /// </summary>
        //        /// <remarks>
        //        /// No return type.
        //        /// </remarks>
        //        public static readonly SymbolDisplayFormat NameFormat =
        //            new SymbolDisplayFormat(
        //                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        //                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        //                propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
        //                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
        //                memberOptions: SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeExplicitInterface,
        //                parameterOptions:
        //                    SymbolDisplayParameterOptions.IncludeParamsRefOut |
        //                    SymbolDisplayParameterOptions.IncludeExtensionThis |
        //                    SymbolDisplayParameterOptions.IncludeType |
        //                    SymbolDisplayParameterOptions.IncludeName,
        //                miscellaneousOptions:
        //                    SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        //                    SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        //        /// <summary>
        //        /// Contains enough information to determine whether two symbols have the same signature.
        //        /// </summary>
        //        public static readonly SymbolDisplayFormat SignatureFormat =
        //            new SymbolDisplayFormat(
        //                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
        //                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        //                propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
        //                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
        //                memberOptions:
        //                    SymbolDisplayMemberOptions.IncludeParameters |
        //                    SymbolDisplayMemberOptions.IncludeContainingType |
        //                    SymbolDisplayMemberOptions.IncludeExplicitInterface |
        //                    SymbolDisplayMemberOptions.IncludeType,
        //                kindOptions:
        //                    SymbolDisplayKindOptions.IncludeMemberKeyword,
        //                parameterOptions:
        //                    SymbolDisplayParameterOptions.IncludeParamsRefOut |
        //                    SymbolDisplayParameterOptions.IncludeExtensionThis |
        //                    SymbolDisplayParameterOptions.IncludeType);
        //    }
        //    public enum SymbolVisibility
        //    {
        //        Public,
        //        Internal,
        //        Private,
        //    }
        //    public class ArrayBuilder<T> : IReadOnlyCollection<T>, IReadOnlyList<T>
        //    {
        //        #region DebuggerProxy

        //        private sealed class DebuggerProxy
        //        {
        //            private readonly ArrayBuilder<T> _builder;

        //            public DebuggerProxy(ArrayBuilder<T> builder)
        //            {
        //                _builder = builder;
        //            }


        //            public T[] A
        //            {
        //                get
        //                {
        //                    var result = new T[_builder.Count];
        //                    for (int i = 0; i < result.Length; i++)
        //                    {
        //                        result[i] = _builder[i];
        //                    }

        //                    return result;
        //                }
        //            }
        //        }

        //        #endregion

        //        private readonly ImmutableArray<T>.Builder _builder;

        //        private readonly ObjectPool<ArrayBuilder<T>> _pool;

        //        public ArrayBuilder(int size)
        //        {
        //            _builder = ImmutableArray.CreateBuilder<T>(size);
        //        }

        //        public ArrayBuilder() :
        //            this(8)
        //        { }

        //        private ArrayBuilder(ObjectPool<ArrayBuilder<T>> pool) :
        //            this()
        //        {
        //            _pool = pool;
        //        }

        //        /// <summary>
        //        /// Realizes the array.
        //        /// </summary>
        //        public ImmutableArray<T> ToImmutable()
        //        {
        //            return _builder.ToImmutable();
        //        }

        //        public int Count
        //        {
        //            get
        //            {
        //                return _builder.Count;
        //            }
        //            set
        //            {
        //                _builder.Count = value;
        //            }
        //        }

        //        public T this[int index]
        //        {
        //            get
        //            {
        //                return _builder[index];
        //            }

        //            set
        //            {
        //                _builder[index] = value;
        //            }
        //        }

        //        /// <summary>
        //        /// Write <paramref name="value"/> to slot <paramref name="index"/>. 
        //        /// Fills in unallocated slots preceding the <paramref name="index"/>, if any.
        //        /// </summary>
        //        public void SetItem(int index, T value)
        //        {
        //            while (index > _builder.Count)
        //            {
        //                _builder.Add(default(T));
        //            }

        //            if (index == _builder.Count)
        //            {
        //                _builder.Add(value);
        //            }
        //            else
        //            {
        //                _builder[index] = value;
        //            }
        //        }

        //        public void Add(T item)
        //        {
        //            _builder.Add(item);
        //        }

        //        public void Insert(int index, T item)
        //        {
        //            _builder.Insert(index, item);
        //        }

        //        public void EnsureCapacity(int capacity)
        //        {
        //            if (_builder.Capacity < capacity)
        //            {
        //                _builder.Capacity = capacity;
        //            }
        //        }

        //        public void Clear()
        //        {
        //            _builder.Clear();
        //        }

        //        public bool Contains(T item)
        //        {
        //            return _builder.Contains(item);
        //        }

        //        public int IndexOf(T item)
        //        {
        //            return _builder.IndexOf(item);
        //        }

        //        public int IndexOf(T item, IEqualityComparer<T> equalityComparer)
        //        {
        //            return _builder.IndexOf(item, 0, _builder.Count, equalityComparer);
        //        }

        //        public int IndexOf(T item, int startIndex, int count)
        //        {
        //            return _builder.IndexOf(item, startIndex, count);
        //        }

        //        public int FindIndex(Predicate<T> match)
        //            => FindIndex(0, this.Count, match);

        //        public int FindIndex(int startIndex, Predicate<T> match)
        //            => FindIndex(startIndex, this.Count - startIndex, match);

        //        public int FindIndex(int startIndex, int count, Predicate<T> match)
        //        {
        //            int endIndex = startIndex + count;
        //            for (int i = startIndex; i < endIndex; i++)
        //            {
        //                if (match(_builder[i]))
        //                {
        //                    return i;
        //                }
        //            }

        //            return -1;
        //        }

        //        public void RemoveAt(int index)
        //        {
        //            _builder.RemoveAt(index);
        //        }

        //        public void RemoveLast()
        //        {
        //            _builder.RemoveAt(_builder.Count - 1);
        //        }

        //        public void ReverseContents()
        //        {
        //            _builder.Reverse();
        //        }

        //        public void Sort()
        //        {
        //            _builder.Sort();
        //        }

        //        public void Sort(IComparer<T> comparer)
        //        {
        //            _builder.Sort(comparer);
        //        }

        //        public void Sort(Comparison<T> compare)
        //            => Sort(Comparer<T>.Create(compare));

        //        public void Sort(int startIndex, IComparer<T> comparer)
        //        {
        //            _builder.Sort(startIndex, _builder.Count - startIndex, comparer);
        //        }

        //        public T[] ToArray()
        //        {
        //            return _builder.ToArray();
        //        }

        //        public void CopyTo(T[] array, int start)
        //        {
        //            _builder.CopyTo(array, start);
        //        }

        //        public T Last()
        //        {
        //            return _builder[_builder.Count - 1];
        //        }

        //        public T First()
        //        {
        //            return _builder[0];
        //        }

        //        public bool Any()
        //        {
        //            return _builder.Count > 0;
        //        }

        //        /// <summary>
        //        /// Realizes the array.
        //        /// </summary>
        //        public ImmutableArray<T> ToImmutableOrNull()
        //        {
        //            if (Count == 0)
        //            {
        //                return default(ImmutableArray<T>);
        //            }

        //            return this.ToImmutable();
        //        }

        //        /// <summary>
        //        /// Realizes the array, downcasting each element to a derived type.
        //        /// </summary>
        //        public ImmutableArray<U> ToDowncastedImmutable<U>()
        //            where U : T
        //        {
        //            if (Count == 0)
        //            {
        //                return ImmutableArray<U>.Empty;
        //            }

        //            var tmp = ArrayBuilder<U>.GetInstance(Count);
        //            foreach (var i in this)
        //            {
        //                tmp.Add((U)i);
        //            }

        //            return tmp.ToImmutableAndFree();
        //        }

        //        /// <summary>
        //        /// Realizes the array and disposes the builder in one operation.
        //        /// </summary>
        //        public ImmutableArray<T> ToImmutableAndFree()
        //        {
        //            var result = this.ToImmutable();
        //            this.Free();
        //            return result;
        //        }

        //        public T[] ToArrayAndFree()
        //        {
        //            var result = this.ToArray();
        //            this.Free();
        //            return result;
        //        }

        //        #region Poolable

        //        // To implement Poolable, you need two things:
        //        // 1) Expose Freeing primitive. 
        //        public void Free()
        //        {
        //            var pool = _pool;
        //            if (pool != null)
        //            {
        //                // According to the statistics of a C# compiler self-build, the most commonly used builder size is 0.  (808003 uses).
        //                // The distant second is the Count == 1 (455619), then 2 (106362) ...
        //                // After about 50 (just 67) we have a long tail of infrequently used builder sizes.
        //                // However we have builders with size up to 50K   (just one such thing)
        //                //
        //                // We do not want to retain (potentially indefinitely) very large builders 
        //                // while the chance that we will need their size is diminishingly small.
        //                // It makes sense to constrain the size to some "not too small" number. 
        //                // Overall perf does not seem to be very sensitive to this number, so I picked 128 as a limit.
        //                if (_builder.Capacity < 128)
        //                {
        //                    if (this.Count != 0)
        //                    {
        //                        this.Clear();
        //                    }

        //                    pool.Free(this);
        //                    return;
        //                }
        //                else
        //                {
        //                    pool.ForgetTrackedObject(this);
        //                }
        //            }
        //        }

        //        // 2) Expose the pool or the way to create a pool or the way to get an instance.
        //        //    for now we will expose both and figure which way works better
        //        private static readonly ObjectPool<ArrayBuilder<T>> s_poolInstance = CreatePool();
        //        public static ArrayBuilder<T> GetInstance()
        //        {
        //            var builder = s_poolInstance.Allocate();

        //            return builder;
        //        }

        //        public static ArrayBuilder<T> GetInstance(int capacity)
        //        {
        //            var builder = GetInstance();
        //            builder.EnsureCapacity(capacity);
        //            return builder;
        //        }

        //        public static ArrayBuilder<T> GetInstance(int capacity, T fillWithValue)
        //        {
        //            var builder = GetInstance();
        //            builder.EnsureCapacity(capacity);

        //            for (int i = 0; i < capacity; i++)
        //            {
        //                builder.Add(fillWithValue);
        //            }

        //            return builder;
        //        }

        //        public static ObjectPool<ArrayBuilder<T>> CreatePool()
        //        {
        //            return CreatePool(128); // we rarely need more than 10
        //        }

        //        public static ObjectPool<ArrayBuilder<T>> CreatePool(int size)
        //        {
        //            ObjectPool<ArrayBuilder<T>> pool = null;
        //            pool = new ObjectPool<ArrayBuilder<T>>(() => new ArrayBuilder<T>(pool), size);
        //            return pool;
        //        }

        //        #endregion

        //        public Enumerator GetEnumerator()
        //        {
        //            return new Enumerator(this);
        //        }

        //        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        //        {
        //            return GetEnumerator();
        //        }

        //        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //        {
        //            return GetEnumerator();
        //        }

        //        internal Dictionary<K, ImmutableArray<T>> ToDictionary<K>(Func<T, K> keySelector, IEqualityComparer<K> comparer = null)
        //        {
        //            if (this.Count == 1)
        //            {
        //                var dictionary1 = new Dictionary<K, ImmutableArray<T>>(1, comparer);
        //                T value = this[0];
        //                dictionary1.Add(keySelector(value), ImmutableArray.Create(value));
        //                return dictionary1;
        //            }

        //            if (this.Count == 0)
        //            {
        //                return new Dictionary<K, ImmutableArray<T>>(comparer);
        //            }

        //            // bucketize
        //            // prevent reallocation. it may not have 'count' entries, but it won't have more. 
        //            var accumulator = new Dictionary<K, ArrayBuilder<T>>(Count, comparer);
        //            for (int i = 0; i < Count; i++)
        //            {
        //                var item = this[i];
        //                var key = keySelector(item);
        //                if (!accumulator.TryGetValue(key, out var bucket))
        //                {
        //                    bucket = ArrayBuilder<T>.GetInstance();
        //                    accumulator.Add(key, bucket);
        //                }

        //                bucket.Add(item);
        //            }

        //            var dictionary = new Dictionary<K, ImmutableArray<T>>(accumulator.Count, comparer);

        //            // freeze
        //            foreach (var pair in accumulator)
        //            {
        //                dictionary.Add(pair.Key, pair.Value.ToImmutableAndFree());
        //            }

        //            return dictionary;
        //        }

        //        public void AddRange(ArrayBuilder<T> items)
        //        {
        //            _builder.AddRange(items._builder);
        //        }

        //        public void AddRange<U>(ArrayBuilder<U> items) where U : T
        //        {
        //            _builder.AddRange(items._builder);
        //        }

        //        public void AddRange(ImmutableArray<T> items)
        //        {
        //            _builder.AddRange(items);
        //        }

        //        public void AddRange(ImmutableArray<T> items, int length)
        //        {
        //            _builder.AddRange(items, length);
        //        }

        //        public void AddRange<S>(ImmutableArray<S> items) where S : class, T
        //        {
        //            AddRange(ImmutableArray<T>.CastUp(items));
        //        }

        //        public void AddRange(T[] items, int start, int length)
        //        {
        //            for (int i = start, end = start + length; i < end; i++)
        //            {
        //                Add(items[i]);
        //            }
        //        }

        //        public void AddRange(IEnumerable<T> items)
        //        {
        //            _builder.AddRange(items);
        //        }

        //        public void AddRange(params T[] items)
        //        {
        //            _builder.AddRange(items);
        //        }

        //        public void AddRange(T[] items, int length)
        //        {
        //            _builder.AddRange(items, length);
        //        }

        //        public void Clip(int limit)
        //        {

        //            _builder.Count = limit;
        //        }

        //        public void ZeroInit(int count)
        //        {
        //            _builder.Clear();
        //            _builder.Count = count;
        //        }

        //        public void AddMany(T item, int count)
        //        {
        //            for (int i = 0; i < count; i++)
        //            {
        //                Add(item);
        //            }
        //        }

        //        public void RemoveDuplicates()
        //        {
        //            var set = PooledHashSet<T>.GetInstance();

        //            int j = 0;
        //            for (int i = 0; i < Count; i++)
        //            {
        //                if (set.Add(this[i]))
        //                {
        //                    this[j] = this[i];
        //                    j++;
        //                }
        //            }

        //            Clip(j);
        //            set.Free();
        //        }

        //        public ImmutableArray<S> SelectDistinct<S>(Func<T, S> selector)
        //        {
        //            var result = ArrayBuilder<S>.GetInstance(Count);
        //            var set = PooledHashSet<S>.GetInstance();

        //            foreach (var item in this)
        //            {
        //                var selected = selector(item);
        //                if (set.Add(selected))
        //                {
        //                    result.Add(selected);
        //                }
        //            }

        //            set.Free();
        //            return result.ToImmutableAndFree();
        //        }
        //        public struct Enumerator : IEnumerator<T>
        //        {
        //            private readonly ArrayBuilder<T> _builder;
        //            private int _index;

        //            public Enumerator(ArrayBuilder<T> builder)
        //            {
        //                _builder = builder;
        //                _index = -1;
        //            }

        //            public T Current
        //            {
        //                get
        //                {
        //                    return _builder[_index];
        //                }
        //            }

        //            public bool MoveNext()
        //            {
        //                _index++;
        //                return _index < _builder.Count;
        //            }

        //            public void Dispose()
        //            {
        //            }

        //            object System.Collections.IEnumerator.Current
        //            {
        //                get
        //                {
        //                    return this.Current;
        //                }
        //            }

        //            public void Reset()
        //            {
        //                _index = -1;
        //            }
        //        }
        //    }
        //}
        //    public class ObjectPool<T> where T : class
        //    {

        //        public struct Element
        //        {
        //            public T Value;
        //        }

        //        /// <remarks>
        //        /// Not using System.Func{T} because this file is linked into the (debugger) Formatter,
        //        /// which does not have that type (since it compiles against .NET 2.0).
        //        /// </remarks>
        //        public delegate T Factory();

        //        // Storage for the pool objects. The first item is stored in a dedicated field because we
        //        // expect to be able to satisfy most requests from it.
        //        private T _firstItem;
        //        private readonly Element[] _items;

        //        // factory is stored for the lifetime of the pool. We will call this only when pool needs to
        //        // expand. compared to "new T()", Func gives more flexibility to implementers and faster
        //        // than "new T()".
        //        private readonly Factory _factory;




        //        public ObjectPool(Factory factory)
        //            : this(factory, Environment.ProcessorCount * 2)
        //        { }

        //        public ObjectPool(Factory factory, int size)
        //        {

        //            _factory = factory;
        //            _items = new Element[size - 1];
        //        }

        //        private T CreateInstance()
        //        {
        //            var inst = _factory();
        //            return inst;
        //        }

        //        /// <summary>
        //        /// Produces an instance.
        //        /// </summary>
        //        /// <remarks>
        //        /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
        //        /// Note that Free will try to store recycled objects close to the start thus statistically 
        //        /// reducing how far we will typically search.
        //        /// </remarks>
        //        internal T Allocate()
        //        {
        //            // PERF: Examine the first element. If that fails, AllocateSlow will look at the remaining elements.
        //            // Note that the initial read is optimistically not synchronized. That is intentional. 
        //            // We will interlock only when we have a candidate. in a worst case we may miss some
        //            // recently returned objects. Not a big deal.
        //            T inst = _firstItem;
        //            if (inst == null || inst != Interlocked.CompareExchange(ref _firstItem, null, inst))
        //            {
        //                inst = AllocateSlow();
        //            }


        //            return inst;
        //        }

        //        private T AllocateSlow()
        //        {
        //            var items = _items;

        //            for (int i = 0; i < items.Length; i++)
        //            {
        //                // Note that the initial read is optimistically not synchronized. That is intentional. 
        //                // We will interlock only when we have a candidate. in a worst case we may miss some
        //                // recently returned objects. Not a big deal.
        //                T inst = items[i].Value;
        //                if (inst != null)
        //                {
        //                    if (inst == Interlocked.CompareExchange(ref items[i].Value, null, inst))
        //                    {
        //                        return inst;
        //                    }
        //                }
        //            }

        //            return CreateInstance();
        //        }

        //        /// <summary>
        //        /// Returns objects to the pool.
        //        /// </summary>
        //        /// <remarks>
        //        /// Search strategy is a simple linear probing which is chosen for it cache-friendliness.
        //        /// Note that Free will try to store recycled objects close to the start thus statistically 
        //        /// reducing how far we will typically search in Allocate.
        //        /// </remarks>
        //        internal void Free(T obj)
        //        {
        //            Validate(obj);
        //            ForgetTrackedObject(obj);

        //            if (_firstItem == null)
        //            {
        //                // Intentionally not using interlocked here. 
        //                // In a worst case scenario two objects may be stored into same slot.
        //                // It is very unlikely to happen and will only mean that one of the objects will get collected.
        //                _firstItem = obj;
        //            }
        //            else
        //            {
        //                FreeSlow(obj);
        //            }
        //        }

        //        private void FreeSlow(T obj)
        //        {
        //            var items = _items;
        //            for (int i = 0; i < items.Length; i++)
        //            {
        //                if (items[i].Value == null)
        //                {
        //                    // Intentionally not using interlocked here. 
        //                    // In a worst case scenario two objects may be stored into same slot.
        //                    // It is very unlikely to happen and will only mean that one of the objects will get collected.
        //                    items[i].Value = obj;
        //                    break;
        //                }
        //            }
        //        }

        //        /// <summary>
        //        /// Removes an object from leak tracking.  
        //        /// 
        //        /// This is called when an object is returned to the pool.  It may also be explicitly 
        //        /// called if an object allocated from the pool is intentionally not being returned
        //        /// to the pool.  This can be of use with pooled arrays if the consumer wants to 
        //        /// return a larger array to the pool than was originally allocated.
        //        /// </summary>

        //        internal void ForgetTrackedObject(T old, T replacement = null)
        //        {
        //        }




        //        private void Validate(object obj)
        //        {

        //            var items = _items;
        //            for (int i = 0; i < items.Length; i++)
        //            {
        //                var value = items[i].Value;
        //                if (value == null)
        //                {
        //                    return;
        //                }


        //            }
        //        }
        //    }
        //    public class PooledHashSet<T> : HashSet<T>
        //    {
        //        private readonly ObjectPool<PooledHashSet<T>> _pool;

        //        private PooledHashSet(ObjectPool<PooledHashSet<T>> pool)
        //        {
        //            _pool = pool;
        //        }

        //        public void Free()
        //        {
        //            this.Clear();
        //            _pool?.Free(this);
        //        }

        //        // global pool
        //        private static readonly ObjectPool<PooledHashSet<T>> s_poolInstance = CreatePool();

        //        // if someone needs to create a pool;
        //        public static ObjectPool<PooledHashSet<T>> CreatePool()
        //        {
        //            ObjectPool<PooledHashSet<T>> pool = null;
        //            pool = new ObjectPool<PooledHashSet<T>>(() => new PooledHashSet<T>(pool), 128);
        //            return pool;
        //        }

        //        public static PooledHashSet<T> GetInstance()
        //        {
        //            var instance = s_poolInstance.Allocate();

        //            return instance;
        //        }
    }
}



