using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VSProvider;

namespace VSParsers
{
    public class Syntaxer
    {
        public Syntaxer()
        {
            var options = new CSharpCompilationOptions(
          OutputKind.WindowsApplication,
          optimizationLevel: OptimizationLevel.Debug,
          allowUnsafe: true);
            //cc = CSharpCompilation.Create("form", options : options).AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location)).AddSyntaxTrees(CSharpSyntaxTree.ParseText("", null, "void"));
        }

        public static SyntaxTree AddReferencesToSyntaxTree(SyntaxTree syntaxTree)
        {
            var c = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            c.Reverse();
            StringBuilder br = new StringBuilder();
            string content = syntaxTree.ToString();
            br.Append(content);
            foreach (var b in c)
            {
                int index = content.LastIndexOf("\n", b.SpanStart);
                string margin = "";
                if (index >= 0)
                {
                    margin = content.Substring(index, b.SpanStart - index);
                }
                br.Insert(b.SpanStart, "//References - " + margin);
            }
            SyntaxTree s = CSharpSyntaxTree.ParseText(br.ToString());
            return s;
        }

        public static List<MethodDeclarationSyntax> MethodMembers(VSSolution vs, SyntaxTree syntaxTree)
        {
            var c = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            c.Reverse();
            StringBuilder br = new StringBuilder();
            string content = syntaxTree.ToString();
            if (content.IndexOf("References") >= 0)
                return c;
            br.Append(content);
            foreach (var b in c)
            {
                int index = content.LastIndexOf("\n", b.SpanStart);
                string margin = "";
                if (index >= 0)
                {
                    //margin = content.Substring(index, b.SpanStart - index);
                    index++;
                }
                else index = b.SpanStart;
                br.Insert(index, "//References - \n" + margin);
            }
            SyntaxTree s = CSharpSyntaxTree.ParseText(br.ToString(), CSharpParseOptions.Default, syntaxTree.FilePath);
            vs.UpdateSyntaxTree(syntaxTree.FilePath, s.ToString());
            c = s.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
            return c;
        }

        public CSharpCompilation cc { get; set; }

        public Dictionary<int, Diagnostic> hc = new Dictionary<int, Diagnostic>();

        public DataGridView dg { get; set; }

        public Dictionary<int, Diagnostic> ResolveAt(TextLocation location, string content, string filename, VSProject vp)
        {
            ArrayList E = new ArrayList();

            if (dg == null)
                return null;

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(content, null, filename);

            //CSharpCompilation cc = CSharpCompilation.Create("form").AddReferences(
            //                                        MetadataReference.CreateFromFile(
            //                                            typeof(object).Assembly.Location))
            //                                   .AddSyntaxTrees(syntaxTree);

            // foreach (MetadataReference r in cc.References)
            //     MessageBox.Show(r.Display);

            List<SyntaxTree> syntax = cc.SyntaxTrees.Select(x => x).Where(x => x.FilePath == filename).ToList();
            if (syntax.Count > 0)
            {
                cc = cc.ReplaceSyntaxTree(syntax[0], syntaxTree);
            }
            else
            {
                cc = cc.AddSyntaxTrees(syntaxTree);
            }
            SemanticModel semanticModel = null;
            try
            {
                semanticModel = cc.GetSemanticModel(syntaxTree);
            }
            catch (Exception ex)
            {
            }
            string s = "";
            int i = 0;

            foreach (Diagnostic d in syntaxTree.GetDiagnostics())
            {
                int hcc = d.GetHashCode();

                E.Add(hcc);

                if (hc.ContainsKey(hcc))
                    continue;

                //s += (d.Descriptor.Category + " - " + d.Descriptor.Id + " - " + d.GetMessage() + " ");

                //foreach (var p in d.Properties)
                //{
                //    s+= (p.Key + " - " + p.Value + "\n");
                //}
                //IntErrors ie = new IntErrors();

                string file = "";
                string line = "";

                if (d.Location != Microsoft.CodeAnalysis.Location.None)
                {
                    if (d.Location.SourceTree != null)
                    {
                        file = d.Location.SourceTree.FilePath;// syntaxTree.FilePath;
                        FileLinePositionSpan c = d.Location.GetLineSpan();
                        line = c.StartLinePosition.Line.ToString();
                    }
                }

                ////else
                ////{
                ////    ie.file = "";
                ////    ie.e = new Error(ErrorType.Error, d.GetMessage(), new DomRegion(new TextLocation(0, 0)));
                ////}

                //ie.id = d.Id;
                //ie.Category = d.Severity.ToString();
                ////if (ie.Category == "Error")
                ////    MessageBox.Show(d.GetMessage());
                //ie.Message = d.GetMessage();
                //ie.vp = vp;
                //E.Add(ie);

                //int rowId = dg.Rows.Add();
                //DataGridViewRow row = dg.Rows[rowId];
                //row.InheritedStyle.BackColor = Color.FromKnownColor(KnownColor.Control);

                //row.Cells[0].Value = "";
                //row.Cells[1].Value = new Bitmap(Properties.Resources.Cancel_256x, 20,20);
                //row.Cells[2].Value = d.Descriptor.Id;
                //row.Cells[3].Value = d.GetMessage();
                //row.Cells[4].Value = Path.GetFileNameWithoutExtension(vp.FileName);
                //row.Cells[5].Value = Path.GetFileName(file);
                //row.Cells[6].Value = line;
                //row.Cells[7].Value = "project";

                hc.Add(hcc, d);
            }
            ArrayList F = new ArrayList();
            foreach (int hcc in hc.Keys)
                if (E.IndexOf(hcc) < 0)
                    F.Add(hcc);
            foreach (int hcc in F)
                if (hc.ContainsKey(hcc))
                    hc.Remove(hcc);

            return hc;
        }

        public List<INamedTypeSymbol> GetAllTypes()
        {
            List<INamedTypeSymbol> b = new List<INamedTypeSymbol>();
            try
            {
                foreach (INamespaceSymbol ns in cc.GlobalNamespace.GetNamespaceMembers())
                {
                    foreach (INamedTypeSymbol s in ns.GetTypeMembers())
                    {
                        b.Add(s);
                    }
                    GetAllTypes(ns, b);
                }
            }
            catch (Exception ex)
            {
            }
            return b;
        }

        public void GetAllTypes(INamespaceSymbol c, List<INamedTypeSymbol> b)
        {
            foreach (INamespaceSymbol ns in c.GetNamespaceMembers())
            {
                foreach (INamedTypeSymbol s in ns.GetTypeMembers())
                {
                    b.Add(s);
                }
                GetAllTypes(ns, b);
            }
        }

        public void GetDeclaredTypes()
        {
            //List<ISymbol> ls = new List<ISymbol>();
            //foreach (Document d in p.Documents)
            //{
            //    SemanticModel m = d.GetSemanticModelAsync().Result;
            //    List<ClassDeclarationSyntax> lc = d.GetSyntaxRootAsync().Result.DescendantNodes().OfType<ClassDeclaractionSyntax>().ToList();
            //    foreach (var c in lc)
            //    {
            //        ISymbol s = m.GetDeclaredSymbol(c);
            //        ls.Add(s);
            //    }
            //}
        }

        static public void Execute(RichTextBox rb)
        {
            //    var projectFileName = "";
            //    var project = ProjectCollection.GlobalProjectCollection.
            //                  GetLoadedProjects(projectFileName).Single();
            //    var compilation = CSharpCompilation.Create(
            //                          project.GetPropertyValue("AssemblyName"),
            //                          syntaxTrees: project.GetItems("Compile").Select(
            //                            c => SyntaxFactory.ParseCompilationUnit(
            //                                     c.EvaluatedInclude).SyntaxTree),
            //                          references: project.GetItems("Reference")
            //                                             .Select(
            //                            r => new MetadataFileReference
            //                                         (r.EvaluatedInclude)));
            string c = File.ReadAllText("..\\..\\ExplorerForms.cs");

            //    c =         @" using System;
            //using System.Collections.Generic;
            //using System.Text;

            //namespace HelloWorld
            //{
            //    class Program
            //    {
            //        static void Main(string[] args)
            //        {
            //            Console.WriteLine(""Hello, World!"");
            //        }
            //    }
            //}";

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(c, null, "file");

            //    List<string> usings = new List<string>()
            //{
            //    "System.IO", "System"
            //};
            //    List<MetadataFileReference> references = new List<MetadataFileReference>()
            //{
            //    new MetadataFileReference(typeof(object).Assembly.Location),
            //};

            CSharpCompilation cc = CSharpCompilation.Create("form").AddReferences(
                                                    MetadataReference.CreateFromFile(
                                                        typeof(object).Assembly.Location))
                                               .AddSyntaxTrees(syntaxTree);

            SyntaxTree syntax = cc.SyntaxTrees.Select(x => x).Where(x => x.FilePath == "file").ToList()[0];

            var semanticModel = cc.GetSemanticModel(syntaxTree);

            //int i = 0;
            //while(i < c.Length)
            //{
            //    string cs = c[i].ToString();
            //        rb.AppendText(i.ToString() + " : " + cs.ToString() + " - " + "\n");
            //    i++;
            //}

            // Ask for symbols at postion 88 i.e. after the declaration
            // of variable 'j' in the supplied source.
            var symbols = semanticModel.LookupSymbols(827);
            rb.AppendText(semanticModel.ToString() + "\n");
            foreach (var symbol in symbols)
            {
                if (symbol.Kind == Microsoft.CodeAnalysis.SymbolKind.Local)
                    rb.AppendText(symbol.Name + "\n");
            }
            foreach (Diagnostic d in semanticModel.GetDiagnostics())
            {
                rb.AppendText(d.Descriptor.Category + " - " + d.Descriptor.Id + " - " + d.GetMessage() + "\n");

                foreach (var p in d.Properties)
                {
                    rb.AppendText(p.Key + " - " + p.Value + "\n");
                }
            }
            ////adding the usings this way also produces the same error
            //CompilationUnitSyntax root = (CompilationUnitSyntax)syntaxTree.GetRoot();
            //root = root.AddUsings(usings.Select(u => SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(u))).ToArray());
            //syntaxTree = CSharpSyntaxTree.Create(root);
        }
    }

    public class IntErrors
    {
        public string file { get; set; }

        public Error e { get; set; }

        public string Category { get; set; }

        public string id { get; set; }

        public string Message { get; set; }

        public Location lc { get; set; }

        public VSProject vp { get; set; }
    }

    public class Finder
    {
        public int MAX(int x, int y)
        {
            return x > y ? x : y;
        }

        public int MIN(int x, int y)
        {
            return x < y ? x : y;
        }

        //const int XSIZE = 100;

        public ArrayList found { get; set; }

        private void OUTPUT(int s)
        {
            //Console.WriteLine(s);

            found.Add(s);
        }

        /* Computing of the maximal suffix for <= */

        private int maxSuf(char[] x, int m, ref int p)
        {
            int ms, j, k;
            char a, b;

            ms = -1;
            j = 0;
            k = p = 1;
            while (j + k < m)
            {
                a = x[j + k];
                b = x[ms + k];
                if (a < b)
                {
                    j += k;
                    k = 1;
                    p = j - ms;
                }
                else
                    if (a == b)
                    if (k != p)
                        ++k;
                    else
                    {
                        j += p;
                        k = 1;
                    }
                else
                { /* a > b */
                    ms = j;
                    j = ms + 1;
                    k = p = 1;
                }
            }
            return (ms);
        }

        /* Computing of the maximal suffix for >= */

        private int maxSufTilde(char[] x, int m, ref int p)
        {
            int ms, j, k;
            char a, b;

            ms = -1;
            j = 0;
            k = p = 1;
            while (j + k < m)
            {
                a = x[j + k];
                b = x[ms + k];
                if (a > b)
                {
                    j += k;
                    k = 1;
                    p = j - ms;
                }
                else
                    if (a == b)
                    if (k != p)
                        ++k;
                    else
                    {
                        j += p;
                        k = 1;
                    }
                else
                { /* a < b */
                    ms = j;
                    j = ms + 1;
                    k = p = 1;
                }
            }
            return (ms);
        }

        public bool Compare(byte[] b1, byte[] b2)
        {
            return Encoding.ASCII.GetString(b1) == Encoding.ASCII.GetString(b2);
        }

        public byte[] GetBytes(char[] b, int offset, int length)
        {
            byte[] bb = new byte[length];

            for (int i = 0; i < length; i++)
                bb[i] = Convert.ToByte(b[i + offset]);

            return bb;
        }

        /* Two Way string matching algorithm. */

        public ArrayList TW(char[] x, int m, char[] y, int n)
        {
            found = new ArrayList();

            int i, j, ell, memory, per;

            int p = 0;
            int q = 0;

            /* Preprocessing */
            i = maxSuf(x, m, ref p);
            j = maxSufTilde(x, m, ref q);
            if (i > j)
            {
                ell = i;
                per = p;
            }
            else
            {
                ell = j;
                per = q;
            }

            /* Searching */
            //if (memcmp(x, x + per, ell + 1) == 0)
            byte[] b = GetBytes(x, 0, ell + 1);
            byte[] c = GetBytes(x, per, ell + 1);
            if (Compare(b, c) == true)
            {
                j = 0;
                memory = -1;
                while (j <= n - m)
                {
                    i = MAX(ell, memory) + 1;
                    while (i < m && x[i] == y[i + j])
                        ++i;
                    if (i >= m)
                    {
                        i = ell;
                        while (i > memory && x[i] == y[i + j])
                            --i;
                        if (i <= memory)
                            OUTPUT(j);
                        j += per;
                        memory = m - per - 1;
                    }
                    else
                    {
                        j += (i - ell);
                        memory = -1;
                    }
                }
            }
            else
            {
                per = MAX(ell + 1, m - ell - 1) + 1;
                j = 0;
                while (j <= n - m)
                {
                    i = ell + 1;
                    while (i < m && x[i] == y[i + j])
                        ++i;
                    if (i >= m)
                    {
                        i = ell;
                        while (i >= 0 && x[i] == y[i + j])
                            --i;
                        if (i < 0)
                            OUTPUT(j);
                        j += per;
                    }
                    else
                        j += (i - ell);
                }
            }

            return found;
        }

        private int mains()
        {
            return 0;
        }
    }

    public class SearchEngine
    {
    }

    public enum SearchDomain
    {
        file,
        openfiles,
        project,
        solution,
        projectwithexternals,
        solutionwithexternals,
        block
    }
}