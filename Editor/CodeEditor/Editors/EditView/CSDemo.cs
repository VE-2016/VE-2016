// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.Completion;
using System.Text.RegularExpressions;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using System.Xml;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using System.Collections;
using VSProvider;
//using Mono.CSharp;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    /// <summary>
    /// Description of CSDemo.
    /// </summary>
    public partial class CSDemo : UserControl
    {
        public CSDemo()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //if (LicenseManager.UsageMode != LicenseUsageMode.Designtime) {
            //	csharpCodeTextBox.SelectAll();
            //	CSharpParseButtonClick(null, null);
            //	resolveButton.UseWaitCursor = true;
            //	ThreadPool.QueueUserWorkItem(
            //		delegate {
            //			builtInLibs.Value.ToString();
            //			BeginInvoke(new Action(delegate { resolveButton.UseWaitCursor = false; }));
            //		});
            //}
        }

        private static SyntaxTree s_syntaxTree;



        public static void Parse(string sourceCode)
        {
            s_syntaxTree = new CSharpParser().Parse(sourceCode, "demo.cs");
            //csharpTreeView.Nodes.Clear();
            //foreach (var element in syntaxTree.Children)
            //{
            //    csharpTreeView.Nodes.Add(MakeTreeNode(element));
            //}
            //SelectCurrentNode(csharpTreeView.Nodes);
            //resolveButton.Enabled = true;
            //findReferencesButton.Enabled = true;
        }

        public IProjectContent pctx { get; set; }

        public ICompilation cmp { get; set; }

        public Dictionary<string, ITypeDefinition> dict { get; set; }

        public void AddProjectFiles(ArrayList F)
        {
            pctx = new CSharpProjectContent();
            s_builtInLibs.Value.ToString();
            var refs = s_builtInLibs.Value;// new List<IUnresolvedAssembly> { mscorlib.Value, systemCore.Value, systemAssembly.Value };
            pctx = pctx.AddAssemblyReferences(refs);

            foreach (string s in F)
            {
                var doc = new ReadOnlyDocument(s);
                //var location = doc.GetLocation(offset);

                string parsedText = File.ReadAllText(s);
                // TODO: Why there are different values in test cases?


                var syntaxTree = new CSharpParser().Parse(parsedText, s);
                syntaxTree.Freeze();
                var unresolvedFile = syntaxTree.ToTypeSystem();



                var mb = new DefaultCompletionContextProvider(doc, unresolvedFile);

                pctx = pctx.AddOrUpdateFiles(unresolvedFile);

                cmp = pctx.CreateCompilation();

                //IEnumerable<EntityDeclaration> b = syntaxTree.GetTypes();
            }

            cmp = pctx.CreateCompilation();

            T = cmp.GetAllTypeDefinitions();

            dict = new Dictionary<string, ITypeDefinition>();

            foreach (ITypeDefinition d in T)
            {
                if (dict.ContainsKey(d.FullName) == false)
                    dict.Add(d.FullName, d);
            }
        }

        public IEnumerable<ITypeDefinition> T { get; set; }

        public IEnumerable<ICompletionData> GetCodeComplete(string filename, string editorText, int offset, int X, int Y) // not the best way to put in the whole string every time
        {
            var doc = new ReadOnlyDocument(editorText);
            //TextLocation b = new TextLocation(Y, X);
            var location = doc.GetLocation(offset);
            //var location = doc.GetLocation(0);
            //offset = doc.GetOffset(X, Y);

            string parsedText = editorText; // TODO: Why there are different values in test cases?


            var syntaxTree = new CSharpParser().Parse(parsedText, filename);
            syntaxTree.Freeze();
            var unresolvedFile = syntaxTree.ToTypeSystem();

            var mb = new DefaultCompletionContextProvider(doc, unresolvedFile);


            //pctx = new CSharpProjectContent();
            //builtInLibs.Value.ToString();
            //var refs = builtInLibs.Value;// new List<IUnresolvedAssembly> { mscorlib.Value, systemCore.Value, systemAssembly.Value };
            //pctx = pctx.AddAssemblyReferences(refs);




            pctx = pctx.AddOrUpdateFiles(unresolvedFile);

            //  CSharpCompilerSettings s = pctx.CompilerSettings; 

            cmp = pctx.CreateCompilation();


            var resolver3 = unresolvedFile.GetResolver(cmp, location);
            var completionContext = new CSharpCompletionContext(doc, offset, pctx);
            var completionFactory = new CSharpCompletionContext.CSharpCompletionDataFactory2(resolver3.CurrentTypeResolveContext, completionContext);
            var engine = new CSharpCompletionEngine(doc, mb, completionFactory, pctx, resolver3.CurrentTypeResolveContext);



            //MemberLookup b= resolver3.CreateMemberLookup();

            //resolver3.Compilation.GetAllTypeDefinitions();


            //T = cmp.GetAllTypeDefinitions();


            //    T = resolver3.Compilation.TypeResolveContext.CurrentAssembly.GetAllTypeDefinitions();


            // var completionFactory = new CSharpCompletionContext.CSharpCompletionDataFactory2(resolver3.CurrentUsingScope, completionContext);
            // var engine = new CSharpCompletionEngine(doc, mb, completionFactory, pctx, resolver3.CurrentUsingScope);

            engine.EolMarker = Environment.NewLine;
            engine.EditorBrowsableBehavior = EditorBrowsableBehavior.IncludeAdvanced;
            engine.FormattingPolicy = FormattingOptionsFactory.CreateEmpty();
            engine.AutomaticallyAddImports = true;
            var data = engine.GetCompletionData(offset, controlSpace: false);




            return data;
        }

        public ArrayList GetErrors()
        {
            ArrayList L = new ArrayList();
            foreach (CSharpUnresolvedFile c in pctx.Files)
            {
                L.Add(c);
            }
            return L;
        }

        public ResolveResult GetCurrentMember(string filename, string editorText, int offset, int X, int Y)
        {
            try
            {
                var doc = new ReadOnlyDocument(editorText);
                // var location = doc.GetLocation(offset);
                TextLocation b = new TextLocation(Y, X);
                var location = b;// doc.GetLocation(offset);

                string parsedText = editorText;

                var syntaxTree = new CSharpParser().Parse(parsedText, filename);
                syntaxTree.Freeze();
                var unresolvedFile = syntaxTree.ToTypeSystem();




                var mb = new DefaultCompletionContextProvider(doc, unresolvedFile);

                pctx = pctx.AddOrUpdateFiles(unresolvedFile);

                cmp = pctx.CreateCompilation();

                var resolver3 = unresolvedFile.GetResolver(cmp, location);

                ResolveResult result = ResolveAtLocation.Resolve(cmp, unresolvedFile, syntaxTree, location);

                return result;
            }
            catch (Exception e) { }

            return null;
        }

        public ITypeDefinition GetAllTypes(string name)
        {
            foreach (string s in dict.Keys)
            {
                if (s.EndsWith(name))
                    return dict[s];
            }



            return null;
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        public string TypeToString(ITypeDefinition name)
        {
            string c = "";


            Type tt = GetType(name.FullTypeName.ReflectionName);

            if (tt != null)
            {
                c = "// " + tt.Assembly.GetName() + "\n\n";



                foreach (AssemblyName a in tt.Assembly.GetReferencedAssemblies())
                {
                    c += "using " + a.Name + "\n";
                }
            }

            c += "\nnamespace " + name.Namespace + "\n{\n";


            if (name.IsPublic)
                c += "public";
            else if (name.IsProtected)
                c += "protected";
            else if (name.IsPrivate)
                c += "private";

            if (name.IsAbstract)
                c += " abstract ";



            c += " " + "class ";

            c += name.FullTypeName;

            if (name.DirectBaseTypes != null)
            {
                c += " : ";

                int i = 0;
                foreach (IType t in name.DirectBaseTypes)
                {
                    if (i > 0)
                        c += ", ";
                    c += " " + t.Name;
                    i++;
                }
            }

            c += " \n{\n\n";

            //foreach (IMember m in name.Members)
            //{
            //    if (m.IsPublic)
            //        c += "public ";
            //    else if (m.IsProtected)
            //        c += "protected ";
            //    else if (m.IsPrivate)
            //        c += "private ";

            //    c += m.Name + "\n";
            //}

            //foreach(ITypeDefinition dd in name.GetAllBaseTypeDefinitions()){

            foreach (IMember m in name.Members)
            {
                if (m.IsAbstract)
                    c += "abstract ";

                if (m.IsSealed)
                    c += "sealed ";

                if (m.IsInternal)
                    c += "internal ";

                if (m.IsPublic)
                    c += "public ";
                else if (m.IsProtected)
                    c += "protected ";
                else if (m.IsPrivate)
                    c += "private ";

                if (m.IsOverride)
                    c += "override ";

                if (m.IsVirtual)
                    c += "virtual ";


                if ((Type)m.GetType() == typeof(DefaultResolvedEvent))
                {
                    c += "event ";

                    c += ((DefaultResolvedEvent)m).ReturnType.Name + " ";
                }

                if ((Type)m.GetType() == typeof(DefaultResolvedProperty))
                {
                    c += ((DefaultResolvedProperty)m).ReturnType.Name + " ";
                }
                else if ((Type)m.GetType() == typeof(DefaultResolvedField))
                {
                    c += ((DefaultResolvedField)m).ReturnType.Name + " ";
                }


                if ((Type)m.GetType() == typeof(DefaultResolvedMethod))
                {
                    if (((DefaultResolvedMethod)m).IsConstructor == false)
                    {
                        if (((DefaultResolvedMethod)m).ReturnType.Name == "Void")

                            c += ((DefaultResolvedMethod)m).ReturnType.Name.ToLower() + " ";
                        else
                            c += ((DefaultResolvedMethod)m).ReturnType.Name + " ";

                        c += m.Name;
                    }
                    else c += name.Name;
                }
                else

                    c += m.Name;

                if ((Type)m.GetType() == typeof(DefaultResolvedEvent))
                {
                    c += "; ";
                }
                if ((Type)m.GetType() == typeof(DefaultResolvedField))
                {
                    c += "; ";
                }


                if ((Type)m.GetType() == typeof(DefaultResolvedProperty))
                {
                    c += " {";

                    if (((DefaultResolvedProperty)m).CanGet)
                        c += " get;";
                    if (((DefaultResolvedProperty)m).CanSet)
                        c += " set;";

                    c += " }";
                }

                if ((Type)m.GetType() == typeof(DefaultResolvedMethod))
                {
                    c += "(";

                    IMethod b = (IMethod)m;

                    foreach (IParameter p in b.Parameters)
                    {
                        c += p.Type.Name;
                        c += " ";
                        c += p.Name;
                        c += ",";
                    }

                    if (c.EndsWith(","))
                        c = c.Remove(c.Length - 1);

                    c += ");";
                }

                c += "\n";
            }

            //}

            c += "\n}\n\n}\n";





            return c;
        }


        public IEnumerable<ICompletionData> DoCodeComplete(string editorText, int offset) // not the best way to put in the whole string every time
        {
            var doc = new ReadOnlyDocument(editorText);
            var location = doc.GetLocation(offset);


            string parsedText = editorText; // TODO: Why there are different values in test cases?


            var syntaxTree = new CSharpParser().Parse(parsedText, "program.cs");
            syntaxTree.Freeze();
            var unresolvedFile = syntaxTree.ToTypeSystem();

            var mb = new DefaultCompletionContextProvider(doc, unresolvedFile);


            pctx = new CSharpProjectContent();
            s_builtInLibs.Value.ToString();
            var refs = s_builtInLibs.Value;// new List<IUnresolvedAssembly> { mscorlib.Value, systemCore.Value, systemAssembly.Value };
            pctx = pctx.AddAssemblyReferences(refs);
            pctx = pctx.AddOrUpdateFiles(unresolvedFile);

            var cmp = pctx.CreateCompilation();

            var resolver3 = unresolvedFile.GetResolver(cmp, location);
            var completionContext = new CSharpCompletionContext(doc, offset, pctx);
            var completionFactory = new CSharpCompletionContext.CSharpCompletionDataFactory2(resolver3.CurrentTypeResolveContext, completionContext);
            var engine = new CSharpCompletionEngine(doc, mb, completionFactory, pctx, resolver3.CurrentTypeResolveContext);


            engine.EolMarker = Environment.NewLine;
            engine.FormattingPolicy = FormattingOptionsFactory.CreateMono();

            var data = engine.GetCompletionData(offset, controlSpace: false);
            return data;
        }

        private void CSharpParseButtonClick(object sender, EventArgs e)
        {
            s_syntaxTree = new CSharpParser().Parse(csharpCodeTextBox.Text, "demo.cs");
            csharpTreeView.Nodes.Clear();
            foreach (var element in s_syntaxTree.Children)
            {
                csharpTreeView.Nodes.Add(MakeTreeNode(element));
            }
            SelectCurrentNode(csharpTreeView.Nodes);
            resolveButton.Enabled = true;
            findReferencesButton.Enabled = true;
        }

        private TreeNode MakeTreeNode(AstNode node)
        {
            TreeNode t = new TreeNode(GetNodeTitle(node));
            t.Tag = node;
            foreach (AstNode child in node.Children)
            {
                t.Nodes.Add(MakeTreeNode(child));
            }
            return t;
        }

        private string GetNodeTitle(AstNode node)
        {
            StringBuilder b = new StringBuilder();
            b.Append(node.Role.ToString());
            b.Append(": ");
            b.Append(node.GetType().Name);
            bool hasProperties = false;
            foreach (PropertyInfo p in node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.Name == "NodeType" || p.Name == "IsNull" || p.Name == "IsFrozen" || p.Name == "HasChildren")
                    continue;
                if (p.PropertyType == typeof(string) || p.PropertyType.IsEnum || p.PropertyType == typeof(bool))
                {
                    if (!hasProperties)
                    {
                        hasProperties = true;
                        b.Append(" (");
                    }
                    else
                    {
                        b.Append(", ");
                    }
                    b.Append(p.Name);
                    b.Append(" = ");
                    try
                    {
                        object val = p.GetValue(node, null);
                        b.Append(val != null ? val.ToString() : "**null**");
                    }
                    catch (TargetInvocationException ex)
                    {
                        b.Append("**" + ex.InnerException.GetType().Name + "**");
                    }
                }
            }
            if (hasProperties)
                b.Append(")");
            return b.ToString();
        }

        private bool SelectCurrentNode(TreeNodeCollection c)
        {
            int selectionStart = csharpCodeTextBox.SelectionStart;
            int selectionEnd = selectionStart + csharpCodeTextBox.SelectionLength;
            foreach (TreeNode t in c)
            {
                AstNode node = t.Tag as AstNode;
                if (node != null
                    && selectionStart >= GetOffset(csharpCodeTextBox, node.StartLocation)
                    && selectionEnd <= GetOffset(csharpCodeTextBox, node.EndLocation))
                {
                    if (selectionStart == selectionEnd
                        && (selectionStart == GetOffset(csharpCodeTextBox, node.StartLocation)
                            || selectionStart == GetOffset(csharpCodeTextBox, node.EndLocation)))
                    {
                        // caret is on border of this node; don't expand
                        csharpTreeView.SelectedNode = t;
                    }
                    else
                    {
                        t.Expand();
                        if (!SelectCurrentNode(t.Nodes))
                            csharpTreeView.SelectedNode = t;
                    }
                    return true;
                }
            }
            return false;
        }

        private void CSharpGenerateCodeButtonClick(object sender, EventArgs e)
        {
            csharpCodeTextBox.Text = s_syntaxTree.GetText();
        }

        private int GetOffset(TextBox textBox, TextLocation location)
        {
            // TextBox uses 0-based coordinates, TextLocation is 1-based
            return textBox.GetFirstCharIndexFromLine(location.Line - 1) + location.Column - 1;
        }

        private TextLocation GetTextLocation(TextBox textBox, int offset)
        {
            int line = textBox.GetLineFromCharIndex(offset);
            int col = offset - textBox.GetFirstCharIndexFromLine(line);
            return new TextLocation(line + 1, col + 1);
        }

        private void CSharpTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            AstNode node = e.Node.Tag as AstNode;
            if (node != null)
            {
                if (node.StartLocation.IsEmpty || node.EndLocation.IsEmpty)
                {
                    csharpCodeTextBox.DeselectAll();
                }
                else
                {
                    int startOffset = GetOffset(csharpCodeTextBox, node.StartLocation);
                    int endOffset = GetOffset(csharpCodeTextBox, node.EndLocation);
                    csharpCodeTextBox.Select(startOffset, endOffset - startOffset);
                }
            }
        }

        private static Lazy<IList<IUnresolvedAssembly>> s_builtInLibs = new Lazy<IList<IUnresolvedAssembly>>(
            delegate
            {
                Assembly[] assemblies = {
                    typeof(object).Assembly, // mscorlib
					typeof(Uri).Assembly, // System.dll
                    typeof( System.Runtime.InteropServices.AssemblyRegistrationFlags).Assembly,
                    typeof(System.Linq.Enumerable).Assembly, // System.Core.dll
                    typeof(System.Collections.ArrayList).Assembly, // System.Core.dll
                    typeof(System.Xml.XmlDocument).Assembly, // System.Xml.dll
					typeof(System.Drawing.Bitmap).Assembly, // System.Drawing.dll
					typeof(Form).Assembly, // System.Windows.Forms.dll
					typeof(ICSharpCode.NRefactory.TypeSystem.IProjectContent).Assembly,
                    typeof(Microsoft.Build.Utilities.Logger).Assembly,
                    typeof(Microsoft.Build.Framework.LoggerVerbosity).Assembly,
                    typeof(System.Runtime.CompilerServices.CallConvCdecl).Assembly,
};
                IUnresolvedAssembly[] projectContents = new IUnresolvedAssembly[assemblies.Length];
                Stopwatch total = Stopwatch.StartNew();
                Parallel.For(
                    0, assemblies.Length,
                    delegate (int i)
                    {
                        Stopwatch w = Stopwatch.StartNew();
                        ICSharpCode.NRefactory.TypeSystem.CecilLoader loader = new CecilLoader();
                        projectContents[i] = loader.LoadAssemblyFile(assemblies[i].Location);
                        Debug.WriteLine(Path.GetFileName(assemblies[i].Location) + ": " + w.Elapsed);
                    });
                Debug.WriteLine("Total: " + total.Elapsed);
                return projectContents;
            });


        public ResolveResult Resolve(TextLocation location, string content, string filename)
        {
            // IProjectContent project = new CSharpProjectContent();
            var syntaxTree = new CSharpParser().Parse(content, filename);
            var unresolvedFile = syntaxTree.ToTypeSystem();
            pctx = pctx.AddOrUpdateFiles(unresolvedFile);
            //project = project.AddAssemblyReferences(builtInLibs.Value);

            cmp = pctx.CreateCompilation();



            ResolveResult result;


            //TextLocation location = GetTextLocation(csharpCodeTextBox, csharpCodeTextBox.SelectionStart);
            result = ResolveAtLocation.Resolve(cmp, unresolvedFile, syntaxTree, location);
            if (result == null)
            {
                //MessageBox.Show("Could not find a resolvable node at the caret location.");
                return result;
            }

            return result;
        }
        public ArrayList ResolveAt(TextLocation location, string content, string filename, VSProject vp)
        {
            // IProjectContent project = new CSharpProjectContent();
            var syntaxTree = new CSharpParser().Parse(content, filename);
            var unresolvedFile = syntaxTree.ToTypeSystem();
            syntaxTree.Freeze();
            pctx = pctx.AddOrUpdateFiles(unresolvedFile);
            //project = project.AddAssemblyReferences(builtInLibs.Value);



            cmp = pctx.CreateCompilation();

            //ResolveResult res = csd.Resolve(new TextLocation(p.Y, p.X), Document.Text, FileName);


            ArrayList E = new ArrayList();

            ResolveResult result;

            CSharpUnresolvedFile c = unresolvedFile;

            E.Add(c);



            CSharpAstResolver resolver = new CSharpAstResolver(cmp, syntaxTree, unresolvedFile);

            foreach (AstNode node in syntaxTree.DescendantNodesAndSelf())
            {
                ResolveResult r = resolver.Resolve(node);
                if (r.IsError)
                {
                    if (r.GetType() == typeof(ErrorResolveResult))
                        continue;




                    // UnknownMemberResolveResult b = (UnknownMemberResolveResult)r;




                    DomRegion d = r.GetDefinitionRegion();

                    TextLocation n = node.StartLocation;

                    TextLocation g = new TextLocation(d.BeginLine + n.Line, d.BeginColumn + n.Column);

                    string message = r.ToString().Replace("[", "");
                    message = message.Replace("]", "");
                    message = message.Replace("?", "");
                    message = message.Replace("ResolveResult", "");

                    Error e = new Error(ErrorType.Error, message, g);



                    //IntError ie = new IntError();
                    //ie.vp = null;
                    //ie.c = unresolvedFile;
                    //ie.e = new Error(ErrorType.Error, r.ToString());
                    //ie.vp = vp;
                    c.Errors.Add(e);
                }
            }

            return E;

            ////TextLocation location = GetTextLocation(csharpCodeTextBox, csharpCodeTextBox.SelectionStart);
            //result = ResolveAtLocation.Resolve(cmp, unresolvedFile, syntaxTree, location);
            //if (result == null)
            //{

            //    return null;
            //}

            //if (result.IsError)
            //{
            //    Error e = new Error(ErrorType.Error, result.ToString());

            //    IntError ie = new IntError();
            //    ie.vp = null;
            //    ie.c = unresolvedFile;
            //    ie.e = new Error(ErrorType.Error, result.ToString());
            //    ie.vp = vp;
            //    return ie;
            //}

            //return null;
        }
        private void ResolveButtonClick(object sender, EventArgs e)
        {
            IProjectContent project = new CSharpProjectContent();
            var unresolvedFile = s_syntaxTree.ToTypeSystem();
            project = project.AddOrUpdateFiles(unresolvedFile);
            project = project.AddAssemblyReferences(s_builtInLibs.Value);

            ICompilation compilation = project.CreateCompilation();

            ResolveResult result;
            //if (csharpTreeView.SelectedNode != null)
            //{
            //	var selectedNode = (AstNode)csharpTreeView.SelectedNode.Tag;
            //	CSharpAstResolver resolver = new CSharpAstResolver(compilation, syntaxTree, unresolvedFile);
            //	result = resolver.Resolve(selectedNode);
            //	// CSharpAstResolver.Resolve() never returns null
            //} else
            {
                TextLocation location = GetTextLocation(csharpCodeTextBox, csharpCodeTextBox.SelectionStart);
                result = ResolveAtLocation.Resolve(compilation, unresolvedFile, s_syntaxTree, location);
                if (result == null)
                {
                    MessageBox.Show("Could not find a resolvable node at the caret location.");
                    return;
                }
            }
            //using (var dlg = new SemanticTreeDialog(result))
            //	dlg.ShowDialog();
        }

        private void CSharpCodeTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                e.Handled = true;
                csharpCodeTextBox.SelectAll();
            }
        }

        private void CsharpCodeTextBoxTextChanged(object sender, EventArgs e)
        {
            resolveButton.Enabled = false;
            findReferencesButton.Enabled = false;
        }

        private void FindReferencesButtonClick(object sender, EventArgs e)
        {
            if (csharpTreeView.SelectedNode == null)
                return;

            IProjectContent project = new CSharpProjectContent();
            var unresolvedFile = s_syntaxTree.ToTypeSystem();
            project = project.AddOrUpdateFiles(unresolvedFile);
            project = project.AddAssemblyReferences(s_builtInLibs.Value);

            ICompilation compilation = project.CreateCompilation();
            CSharpAstResolver resolver = new CSharpAstResolver(compilation, s_syntaxTree, unresolvedFile);

            AstNode node = (AstNode)csharpTreeView.SelectedNode.Tag;
            IEntity entity;
            MemberResolveResult mrr = resolver.Resolve(node) as MemberResolveResult;
            TypeResolveResult trr = resolver.Resolve(node) as TypeResolveResult;
            if (mrr != null)
            {
                entity = mrr.Member;
            }
            else if (trr != null)
            {
                entity = trr.Type.GetDefinition();
            }
            else
            {
                return;
            }

            FindReferences fr = new FindReferences();
            int referenceCount = 0;
            FoundReferenceCallback callback = delegate (AstNode matchNode, ResolveResult result)
            {
                Debug.WriteLine(matchNode.StartLocation + " - " + matchNode + " - " + result);
                referenceCount++;
            };

            var searchScopes = fr.GetSearchScopes(entity);
            Debug.WriteLine("Find references to " + entity.ReflectionName);
            fr.FindReferencesInFile(searchScopes, unresolvedFile, s_syntaxTree, compilation, callback, CancellationToken.None);

            MessageBox.Show("Found " + referenceCount + " references to " + entity.FullName);
        }
    }
    public sealed class CSharpCompletionContext
    {
        public readonly ICSharpCode.NRefactory.Editor.IDocument OriginalDocument;
        public readonly int OriginalOffset;
        public readonly string OriginalUsings;
        public readonly string OriginalVariables;

        public readonly int Offset;
        public readonly ICSharpCode.NRefactory.Editor.IDocument Document;
        public readonly ICompilation Compilation;
        public readonly IProjectContent ProjectContent;
        public readonly CSharpResolver Resolver;
        public readonly CSharpTypeResolveContext TypeResolveContextAtCaret;
        public readonly ICompletionContextProvider CompletionContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCompletionContext"/> class.
        /// </summary>
        /// <param name="document">The document, make sure the FileName property is set on the document.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="projectContent">Content of the project.</param>
        /// <param name="usings">The usings.</param>
        /// <param name="variables">The variables</param>
        public CSharpCompletionContext(IDocument document, int offset, IProjectContent projectContent, string usings = null, string variables = null)
        {
            OriginalDocument = document;
            OriginalOffset = offset;
            OriginalUsings = usings;
            OriginalVariables = variables;

            //if the document is a c# script we have to soround the document with some code.
            Document = PrepareCompletionDocument(document, ref offset, usings, variables);
            Offset = offset;

            var syntaxTree = new CSharpParser().Parse(Document, "Document.FileName");
            syntaxTree.Freeze();
            var unresolvedFile = syntaxTree.ToTypeSystem();

            ProjectContent = projectContent.AddOrUpdateFiles(unresolvedFile);
            //note: it's important that the project content is used that is returned after adding the unresolved file
            Compilation = ProjectContent.CreateCompilation();

            var location = Document.GetLocation(Offset);
            Resolver = unresolvedFile.GetResolver(Compilation, location);
            TypeResolveContextAtCaret = unresolvedFile.GetTypeResolveContext(Compilation, location);
            CompletionContextProvider = new DefaultCompletionContextProvider(Document, unresolvedFile);
        }

        private static Regex s_replaceRegex = new Regex("[^a-zA-Z0-9_]");
        private static ICSharpCode.NRefactory.Editor.IDocument PrepareCompletionDocument(ICSharpCode.NRefactory.Editor.IDocument document, ref int offset, string usings = null, string variables = null)
        {
            if (String.IsNullOrEmpty(document.FileName))
                return document;

            //if the code is just a script it it will contain no namestpace, class and method structure and so the code completion will not work properly
            // for it to work we have to suround the code with the appropriate code structure
            //we only process the file if its a .csx file
            var fileExtension = Path.GetExtension(document.FileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(document.FileName);
            if (String.IsNullOrEmpty(fileExtension) || String.IsNullOrEmpty(fileNameWithoutExtension))
                return document;

            if (fileExtension.ToLower() == ".csx")
            {
                string classname = s_replaceRegex.Replace(fileNameWithoutExtension, "");
                classname = classname.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

                string header = String.Empty;
                header += (usings ?? "") + Environment.NewLine;
                header += "public static class " + classname + " {" + Environment.NewLine;
                header += "public static void Main() {" + Environment.NewLine;
                header += (variables ?? "") + Environment.NewLine;

                string footer = "}" + Environment.NewLine + "}" + Environment.NewLine;

                string code = header + document.Text + Environment.NewLine + footer;

                offset += header.Length;

                return new ReadOnlyDocument(new StringTextSource(code), document.FileName);
            }
            return document;
        }
        public class CSharpCompletionDataFactory2 : ICompletionDataFactory, IParameterCompletionDataFactory
        {
            private readonly CSharpTypeResolveContext _contextAtCaret;
            private readonly CSharpCompletionContext _context;

            public CSharpCompletionDataFactory2(CSharpTypeResolveContext contextAtCaret, CSharpCompletionContext context)
            {
                Debug.Assert(contextAtCaret != null);
                _contextAtCaret = contextAtCaret;
                _context = context;
            }

            #region ICompletionDataFactory implementation
            ICompletionData ICompletionDataFactory.CreateEntityCompletionData(IEntity entity)
            {
                return new EntityCompletionData(entity);
            }

            ICompletionData ICompletionDataFactory.CreateEntityCompletionData(IEntity entity, string text)
            {
                return new EntityCompletionData(entity)
                {
                    CompletionText = text,
                    DisplayText = text
                };
            }

            ICompletionData ICompletionDataFactory.CreateTypeCompletionData(IType type, bool showFullName, bool isInAttributeContext, bool addForTypeCreation)
            {
                var typeDef = type.GetDefinition();
                if (typeDef != null)
                    return new EntityCompletionData(typeDef);
                else
                {
                    string name = showFullName ? type.FullName : type.Name;
                    if (isInAttributeContext && name.EndsWith("Attribute") && name.Length > "Attribute".Length)
                    {
                        name = name.Substring(0, name.Length - "Attribute".Length);
                    }
                    return new CompletionData(name);
                }
            }

            ICompletionData ICompletionDataFactory.CreateMemberCompletionData(IType type, IEntity member)
            {
                return new CompletionData(type.Name + "." + member.Name);
            }

            ICompletionData ICompletionDataFactory.CreateLiteralCompletionData(string title, string description, string insertText)
            {
                return new CompletionData(title)
                {
                    Description = description,
                    CompletionText = insertText ?? title,
                    //Image = CompletionImage.Literal.BaseImage,
                    Priority = 2
                };
            }

            ICompletionData ICompletionDataFactory.CreateNamespaceCompletionData(INamespace name)
            {
                return new CompletionData(name.Name)
                {
                    //Image = CompletionImage.NamespaceImage,
                };
            }

            ICompletionData ICompletionDataFactory.CreateVariableCompletionData(IVariable variable)
            {
                return new VariableCompletionData(variable);
            }

            ICompletionData ICompletionDataFactory.CreateVariableCompletionData(ITypeParameter parameter)
            {
                return new CompletionData(parameter.Name);
            }

            ICompletionData ICompletionDataFactory.CreateEventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterDefinition, IUnresolvedMember currentMember, IUnresolvedTypeDefinition currentType)
            {
                return new CompletionData("TODO: event creation");
            }

            ICompletionData ICompletionDataFactory.CreateNewOverrideCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IMember m)
            {
                return new OverrideCompletionData(declarationBegin, m, _contextAtCaret);
            }

            ICompletionData ICompletionDataFactory.CreateNewPartialCompletionData(int declarationBegin, IUnresolvedTypeDefinition type, IUnresolvedMember m)
            {
                return new CompletionData("TODO: partial completion");
            }

            IEnumerable<ICompletionData> ICompletionDataFactory.CreateCodeTemplateCompletionData()
            {
                yield break;
            }

            IEnumerable<ICompletionData> ICompletionDataFactory.CreatePreProcessorDefinesCompletionData()
            {
                yield return new CompletionData("DEBUG");
                yield return new CompletionData("TEST");
            }

            ICompletionData ICompletionDataFactory.CreateImportCompletionData(IType type, bool useFullName, bool addForTypeCreation)
            {
                ITypeDefinition typeDef = type.GetDefinition();
                if (typeDef != null)
                    return new ImportCompletionData(typeDef, _contextAtCaret, useFullName);
                else
                    throw new InvalidOperationException("Should never happen");
            }

            ICompletionData ICompletionDataFactory.CreateFormatItemCompletionData(string format, string description, object example)
            {
                throw new NotImplementedException();
            }

            ICompletionData ICompletionDataFactory.CreateXmlDocCompletionData(string tag, string description = null, string tagInsertionText = null)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region IParameterCompletionDataFactory implementation
            private IParameterDataProvider CreateMethodDataProvider(int startOffset, IEnumerable<IParameterizedMember> methods)
            {
                throw new NotImplementedException();
            }

            IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type)
            {
                return CreateMethodDataProvider(startOffset, type.GetConstructors());
            }

            IParameterDataProvider IParameterCompletionDataFactory.CreateConstructorProvider(int startOffset, IType type, AstNode thisInitializer)
            {
                return CreateMethodDataProvider(startOffset, type.GetConstructors());
            }

            IParameterDataProvider IParameterCompletionDataFactory.CreateMethodDataProvider(int startOffset, IEnumerable<IMethod> methods)
            {
                return CreateMethodDataProvider(startOffset, methods);
            }

            IParameterDataProvider IParameterCompletionDataFactory.CreateDelegateDataProvider(int startOffset, IType type)
            {
                return CreateMethodDataProvider(startOffset, new[] { type.GetDelegateInvokeMethod() });
            }

            public IParameterDataProvider CreateIndexerParameterDataProvider(int startOffset, IType type, IEnumerable<IProperty> accessibleIndexers, AstNode resolvedNode)
            {
                throw new NotImplementedException();
                //return CreateMethodDataProvider(startOffset, accessibleIndexers);
            }

            IParameterDataProvider IParameterCompletionDataFactory.CreateTypeParameterDataProvider(int startOffset, IEnumerable<IType> types)
            {
                return null;
            }

            public IParameterDataProvider CreateTypeParameterDataProvider(int startOffset, IEnumerable<IMethod> methods)
            {
                return CreateMethodDataProvider(startOffset, methods);
            }
            #endregion

        }
    }
    internal class EntityCompletionData : CompletionData, IEntityCompletionData
    {
        private readonly IEntity _entity;
        private static readonly CSharpAmbience s_csharpAmbience = new CSharpAmbience();

        public IEntity Entity
        {
            get { return _entity; }
        }

        public EntityCompletionData(IEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            _entity = entity;
            IAmbience ambience = new CSharpAmbience();
            ambience.ConversionFlags = entity is ITypeDefinition ? ConversionFlags.ShowTypeParameterList : ConversionFlags.None;
            DisplayText = entity.Name;
            CompletionText = ambience.ConvertSymbol(entity);
            ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
            if (entity is ITypeDefinition)
            {
                // Show fully qualified Type name
                ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedTypeNames;
            }
            //Image = AvalonEdit.CodeCompletion.CompletionImage.GetImage(entity);
        }

        #region Description & Documentation
        private string _description;
        public override string Description
        {
            get
            {
                if (_description == null)
                {
                    _description = GetText(Entity);
                    if (HasOverloads)
                    {
                        _description += " (+" + OverloadedData.Count() + " overloads)";
                    }
                    _description += Environment.NewLine + XmlDocumentationToText(Entity.Documentation);
                }
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Converts a member to text.
        /// Returns the declaration of the member as C# or VB code, e.g.
        /// "public void MemberName(string parameter)"
        /// </summary>
        private static string GetText(IEntity entity)
        {
            IAmbience ambience = s_csharpAmbience;
            ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
            if (entity is ITypeDefinition)
            {
                // Show fully qualified Type name
                ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedTypeNames;
            }
            if (entity is IMethod)
            {
                //if the method is an extension method we wanna see the whole method for the description
                //the original method (not reduced) can be obtained by calling ReducedFrom
                var reducedFromMethod = ((IMethod)entity).ReducedFrom;
                if (reducedFromMethod != null)
                    entity = reducedFromMethod;
            }
            return ambience.ConvertEntity(entity);
        }

        public static string XmlDocumentationToText(string xmlDoc)
        {
            //.Diagnostics.Debug.WriteLine(xmlDoc);
            StringBuilder b = new StringBuilder();
            try
            {
                using (XmlTextReader reader = new XmlTextReader(new StringReader("<root>" + xmlDoc + "</root>")))
                {
                    reader.XmlResolver = null;
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Text:
                                b.Append(reader.Value);
                                break;
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "filterpriority":
                                        reader.Skip();
                                        break;
                                    case "returns":
                                        b.AppendLine();
                                        b.Append("Returns: ");
                                        break;
                                    case "param":
                                        b.AppendLine();
                                        b.Append(reader.GetAttribute("name") + ": ");
                                        break;
                                    case "remarks":
                                        b.AppendLine();
                                        b.Append("Remarks: ");
                                        break;
                                    case "see":
                                        if (reader.IsEmptyElement)
                                        {
                                            b.Append(reader.GetAttribute("cref"));
                                        }
                                        else
                                        {
                                            reader.MoveToContent();
                                            if (reader.HasValue)
                                            {
                                                b.Append(reader.Value);
                                            }
                                            else
                                            {
                                                b.Append(reader.GetAttribute("cref"));
                                            }
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }
                return b.ToString();
            }
            catch (XmlException)
            {
                return xmlDoc;
            }
        }

        #endregion
    } //end class EntityCompletionData

    internal class CompletionData : ICompletionData
    {
        protected CompletionData()
        { }

        public CompletionData(string text)
        {
            DisplayText = CompletionText = text;
        }

        public string TriggerWord { get; set; }
        public int TriggerWordLength { get; set; }

        #region NRefactory ICompletionData implementation
        public CompletionCategory CompletionCategory { get; set; }
        public string DisplayText { get; set; }
        public virtual string Description { get; set; }
        public string CompletionText { get; set; }
        public DisplayFlags DisplayFlags { get; set; }

        public bool HasOverloads
        {
            get { return _overloadedData.Count > 0; }
        }

        private readonly List<ICompletionData> _overloadedData = new List<ICompletionData>();
        public IEnumerable<ICompletionData> OverloadedData
        {
            get { return _overloadedData; }
        }

        public void AddOverload(ICompletionData data)
        {
            if (_overloadedData.Count == 0)
                _overloadedData.Add(this);
            _overloadedData.Add(data);
        }
        #endregion

        #region AvalonEdit ICompletionData implementation

        //public System.Windows.Media.ImageSource Image { get; set; }

        public virtual void Complete(ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            //textArea.Document.Replace(completionSegment, this.CompletionText);
        }

        public object Content
        {
            get { return DisplayText; }
        }

        //public object Description
        //{
        //    get { return "Description"; }
        //}

        private double _priority = 1;
        public virtual double Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public string Text
        {
            get { return this.CompletionText; }
        }
        #endregion

        #region Equals, ToString, GetHashCode...
        public override string ToString()
        {
            return DisplayText;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CompletionData;
            return other != null && DisplayText == other.DisplayText;
        }

        public override int GetHashCode()
        {
            return DisplayText.GetHashCode();
        }
        #endregion
    } //end class CompletionData
    internal class VariableCompletionData : CompletionData, IVariableCompletionData
    {
        public VariableCompletionData(IVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");
            Variable = variable;

            IAmbience ambience = new CSharpAmbience();
            DisplayText = variable.Name;
            Description = ambience.ConvertVariable(variable);
            CompletionText = Variable.Name;
            //this.Image = ICSharpCode.AvalonEdit.CodeCompletion.CompletionImage.Field.BaseImage;
        }

        public IVariable Variable { get; private set; }
    } //end class VariableCompletionData
    internal class OverrideCompletionData : EntityCompletionData
    {
        private readonly int _declarationBegin;
        private readonly CSharpTypeResolveContext _contextAtCaret;

        public OverrideCompletionData(int declarationBegin, IMember m, CSharpTypeResolveContext contextAtCaret)
            : base(m)
        {
            _declarationBegin = declarationBegin;
            _contextAtCaret = contextAtCaret;
            var ambience = new CSharpAmbience
            {
                ConversionFlags =
                    ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowParameterList |
                    ConversionFlags.ShowParameterNames
            };
            this.CompletionText = ambience.ConvertSymbol(m);
        }

        #region Complete Override
        public override void Complete(ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            if (_declarationBegin > completionSegment.Offset)
            {
                //base.Complete(textArea, completionSegment, insertionRequestEventArgs);
                return;
            }
            var b = new TypeSystemAstBuilder(new CSharpResolver(_contextAtCaret))
            {
                ShowTypeParameterConstraints = false,
                GenerateBody = true
            };

            var entityDeclaration = b.ConvertEntity(this.Entity);
            entityDeclaration.Modifiers &= ~(Modifiers.Virtual | Modifiers.Abstract);
            entityDeclaration.Modifiers |= Modifiers.Override;

            if (!this.Entity.IsAbstract)
            {
                // modify body to call the base method
                if (this.Entity.SymbolKind == SymbolKind.Method)
                {
                    var baseCall = new BaseReferenceExpression().Invoke(this.Entity.Name, ParametersToExpressions(this.Entity));
                    var body = entityDeclaration.GetChildByRole(Roles.Body);
                    body.Statements.Clear();
                    if (((IMethod)this.Entity).ReturnType.IsKnownType(KnownTypeCode.Void))
                        body.Statements.Add(new ExpressionStatement(baseCall));
                    else
                        body.Statements.Add(new ReturnStatement(baseCall));
                }
                else if (this.Entity.SymbolKind == SymbolKind.Indexer || this.Entity.SymbolKind == SymbolKind.Property)
                {
                    Expression baseCall;
                    if (this.Entity.SymbolKind == SymbolKind.Indexer)
                        baseCall = new BaseReferenceExpression().Indexer(ParametersToExpressions(this.Entity));
                    else
                        baseCall = new BaseReferenceExpression().Member(this.Entity.Name);
                    var getterBody = entityDeclaration.GetChildByRole(PropertyDeclaration.GetterRole).Body;
                    if (!getterBody.IsNull)
                    {
                        getterBody.Statements.Clear();
                        getterBody.Add(new ReturnStatement(baseCall.Clone()));
                    }
                    var setterBody = entityDeclaration.GetChildByRole(PropertyDeclaration.SetterRole).Body;
                    if (!setterBody.IsNull)
                    {
                        setterBody.Statements.Clear();
                        setterBody.Add(new AssignmentExpression(baseCall.Clone(), new IdentifierExpression("value")));
                    }
                }
            }

            //var document = textArea.Document;
            //StringWriter w = new StringWriter();
            //var formattingOptions = FormattingOptionsFactory.CreateSharpDevelop();
            //var segmentDict = SegmentTrackingOutputFormatter.WriteNode(w, entityDeclaration, formattingOptions, textArea.Options);

            //string newText = w.ToString().TrimEnd();
            //document.Replace(declarationBegin, completionSegment.EndOffset - declarationBegin, newText);
            //var throwStatement = entityDeclaration.Descendants.FirstOrDefault(n => n is ThrowStatement);
            //if (throwStatement != null)
            //{
            //    var segment = segmentDict[throwStatement];
            //    textArea.Selection = new RectangleSelection(textArea, new TextViewPosition(textArea.Document.GetLocation(declarationBegin + segment.Offset)), new TextViewPosition(textArea.Document.GetLocation(declarationBegin + segment.Offset + segment.Length)));
            //}

            ////format the inserted code nicely
            //var formatter = new CSharpFormatter(formattingOptions);
            //formatter.AddFormattingRegion(new DomRegion(document.GetLocation(declarationBegin), document.GetLocation(declarationBegin + newText.Length)));
            //var syntaxTree = new CSharpParser().Parse(document);
            //formatter.AnalyzeFormatting(document, syntaxTree).ApplyChanges();
        }

        private IEnumerable<Expression> ParametersToExpressions(IEntity entity)
        {
            foreach (var p in ((IParameterizedMember)entity).Parameters)
            {
                if (p.IsRef || p.IsOut)
                    yield return new DirectionExpression(p.IsOut ? FieldDirection.Out : FieldDirection.Ref, new IdentifierExpression(p.Name));
                else
                    yield return new IdentifierExpression(p.Name);
            }
        }
        #endregion
    }//end class OverrideCompletionData
    internal class ImportCompletionData : EntityCompletionData
    {
        private string _insertUsing;
        private string _insertionText;

        public ImportCompletionData(ITypeDefinition typeDef, CSharpTypeResolveContext contextAtCaret, bool useFullName)
            : base(typeDef)
        {
            this.Description = "using " + typeDef.Namespace + ";";
            if (useFullName)
            {
                var astBuilder = new TypeSystemAstBuilder(new CSharpResolver(contextAtCaret));
                _insertionText = astBuilder.ConvertType(typeDef).ToString(null);
            }
            else
            {
                _insertionText = typeDef.Name;
                _insertUsing = typeDef.Namespace;
            }
        }
    } //end class ImportCompletionData
}
