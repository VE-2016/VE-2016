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

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinExplorer
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

            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                csharpCodeTextBox.SelectAll();
                CSharpParseButtonClick(null, null);
                resolveButton.UseWaitCursor = true;
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        _builtInLibs.Value.ToString();
                        BeginInvoke(new Action(delegate { resolveButton.UseWaitCursor = false; }));
                    });
            }
        }

        private SyntaxTree _syntaxTree;

        private void CSharpParseButtonClick(object sender, EventArgs e)
        {
            _syntaxTree = new CSharpParser().Parse(csharpCodeTextBox.Text, "demo.cs");
            csharpTreeView.Nodes.Clear();
            foreach (var element in _syntaxTree.Children)
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
            csharpCodeTextBox.Text = _syntaxTree.GetText();
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

        private Lazy<IList<IUnresolvedAssembly>> _builtInLibs = new Lazy<IList<IUnresolvedAssembly>>(
            delegate
            {
                Assembly[] assemblies = {
                    typeof(object).Assembly, // mscorlib
					typeof(Uri).Assembly, // System.dll
					typeof(System.Linq.Enumerable).Assembly, // System.Core.dll
					typeof(System.Xml.XmlDocument).Assembly, // System.Xml.dll
					typeof(System.Drawing.Bitmap).Assembly, // System.Drawing.dll
					typeof(Form).Assembly, // System.Windows.Forms.dll
					typeof(ICSharpCode.NRefactory.TypeSystem.IProjectContent).Assembly,
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

        private void ResolveButtonClick(object sender, EventArgs e)
        {
            IProjectContent project = new CSharpProjectContent();
            var unresolvedFile = _syntaxTree.ToTypeSystem();
            project = project.AddOrUpdateFiles(unresolvedFile);
            project = project.AddAssemblyReferences(_builtInLibs.Value);

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
                result = ResolveAtLocation.Resolve(compilation, unresolvedFile, _syntaxTree, location);
                if (result == null)
                {
                    MessageBox.Show("Could not find a resolvable node at the caret location.");
                    return;
                }
            }
            using (var dlg = new SemanticTreeDialog(result))
                dlg.ShowDialog();
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
            var unresolvedFile = _syntaxTree.ToTypeSystem();
            project = project.AddOrUpdateFiles(unresolvedFile);
            project = project.AddAssemblyReferences(_builtInLibs.Value);

            ICompilation compilation = project.CreateCompilation();
            CSharpAstResolver resolver = new CSharpAstResolver(compilation, _syntaxTree, unresolvedFile);

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
            fr.FindReferencesInFile(searchScopes, unresolvedFile, _syntaxTree, compilation, callback, CancellationToken.None);

            MessageBox.Show("Found " + referenceCount + " references to " + entity.FullName);
        }
    }
}