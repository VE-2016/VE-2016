// VSSolution

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VSProvider
{
    public class VSSolution
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        private static readonly Type s_SolutionParser;
        private static readonly MethodInfo s_SolutionParser_parseSolution;
        private static readonly PropertyInfo s_SolutionParser_projects;
        private static readonly PropertyInfo s_SolutionParser_solutionReader;

        public List<VSProject> projects;
        public string solutionFileName;

        public Dictionary<string, VSProject> dd { get; set; }

        static VSSolution()
        {
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public VSSolution()
        {
        }

        public VSProject GetProjectbyName(string name)
        {
            foreach (VSProject p in projects)
                if (p.Name == name)
                    return p;

            return null;
        }

        public VSProject GetProjectbyExec(string name)
        {
            foreach (VSProject p in projects)
                if (p.GetProjectExec() == name)
                    return p;

            return null;
        }

        public VSProject GetProjectbyFileName(string name)
        {
            foreach (VSProject p in projects)
                if (p.FileName == name)
                    return p;

            return null;
        }

        public VSProject GetProjectbyCompileItem(string name)
        {
            foreach (VSProject p in projects)
            {
                ArrayList C = p.GetCompileItems();

                foreach (string file in C)
                    if (file == name)
                        return p;
            }

            return null;
        }

        public VSProject MainVSProject { get; set; }

        public VSSolution(string solutionFileName)
        {
            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }

            var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            var streamReader = new StreamReader(solutionFileName);
            {
                s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
                s_SolutionParser_parseSolution.Invoke(solutionParser, null);
            }

            this.solutionFileName = solutionFileName;

            projects = new List<VSProject>();
            var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);

            for (int i = 0; i < array.Length; i++)
            {
                projects.Add(new VSProject(this, array.GetValue(i)));
            }

            streamReader.Close();

            streamReader.Dispose();
        }

        public VSProject GetVSProject(string file)
        {
            file = file.Replace("\\\\", "\\");

            foreach (VSProject p in this.Projects)
            {
                if (file == p.FileName)
                    return p;

                ArrayList L = p.GetCompileItems();

                int i = L.IndexOf(file);

                if (i >= 0)
                    return p;
            }

            return null;
        }

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(solutionFileName);
            }
        }

        public IEnumerable<VSProject> Projects
        {
            get
            {
                return projects;
            }
            set
            {
                projects = (System.Collections.Generic.List<VSProject>)value;
            }
        }

        public string SolutionPath
        {
            get
            {
                var file = new FileInfo(solutionFileName);

                return file.DirectoryName;
            }
        }

        public void GetAllTypes(Compilation cc, INamespaceSymbol c, List<INamespaceOrTypeSymbol> b)
        {
            foreach (INamespaceSymbol ns in c.GetNamespaceMembers())
            {
                b.Add(ns);
                foreach (INamedTypeSymbol s in ns.GetTypeMembers())
                {
                    b.Add(s);
                }
                GetAllTypes(cc, ns, b);
            }
        }

        public List<INamespaceOrTypeSymbol> GetAllTypes(Compilation cc)
        {
            List<INamespaceOrTypeSymbol> b = new List<INamespaceOrTypeSymbol>();
            try
            {
                foreach (INamespaceSymbol ns in cc.GlobalNamespace.GetNamespaceMembers())
                {
                    b.Add(ns);
                    foreach (INamedTypeSymbol s in ns.GetTypeMembers())
                    {
                        b.Add(s);
                    }
                    GetAllTypes(cc, ns, b);
                }
            }
            catch (Exception ex)
            {
            }
            return b;
        }

        public Dictionary<string, List<INamespaceOrTypeSymbol>> nts = new Dictionary<string, List<INamespaceOrTypeSymbol>>();

        public List<INamespaceOrTypeSymbol> GetAllTypes(string FileName)
        {
            if (nts.ContainsKey(FileName))
                return nts[FileName];

            Compilation cc = comp[FileName];
            List<INamespaceOrTypeSymbol> b = GetAllTypes(cc);
            nts[FileName] = b;

            var d = b.Select(s => s).Where(t => t.GetAttributes() != null).ToList();

            return b;
        }

        public Dictionary<string, Compilation> comp { get; set; }

        public Dictionary<string, List<INamespaceOrTypeSymbol>> named { get; set; }

        private void LoadSolution(Task<Microsoft.CodeAnalysis.Solution> solution)
        {
        }

        private MSBuildWorkspace workspace { get; set; }

        public async void CompileSolution()
        {
            //bool success = true;

            // int count = 0;

            //EmitResult result = null;

            comp = new Dictionary<string, Compilation>();

            named = new Dictionary<string, List<INamespaceOrTypeSymbol>>();

            workspace = null;
            //Microsoft.CodeAnalysis.Solution solution = null;
            ProjectDependencyGraph projectGraph = null;

            //try
            //{
            Microsoft.CodeAnalysis.Solution solution = null;
            workspace = MSBuildWorkspace.Create();
            solution = await workspace.OpenSolutionAsync(solutionFileName);

            // solutions.ContinueWith(LoadSolution);

            projectGraph = solution.GetProjectDependencyGraph();
            //Dictionary<string, Stream> assemblies = new Dictionary<string, Stream>();
            //}
            //catch (ReflectionTypeLoadException ex)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (Exception exSub in ex.LoaderExceptions)
            //    {
            //        sb.AppendLine(exSub.Message);
            //        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
            //        if (exFileNotFound != null)
            //        {
            //            if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
            //            {
            //                sb.AppendLine("Fusion Log:");
            //                sb.AppendLine(exFileNotFound.FusionLog);
            //            }
            //        }
            //        sb.AppendLine();
            //    }
            //    string errorMessage = sb.ToString();
            //    //Display or log the error based on your application.
            //}

            foreach (ProjectId projectId in projectGraph.GetTopologicallySortedProjects())
            {
                Compilation projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;

                Microsoft.CodeAnalysis.Project project = solution.GetProject(projectId);

                comp.Add(project.FilePath, projectCompilation);

                List<INamespaceOrTypeSymbol> ns = GetAllTypes(project.FilePath);
                named.Add(project.FilePath, ns);
                //var type = ns.Select(s => s).Where(t => t.Name.ToLower() == name.ToLower()).First().GetMembers().OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                //return type;
                //List<INamedTypeSymbol> b = GetAllTypes(projectCompilation);

                //foreach (INamedTypeSymbol ts in b)
                //{
                //    var d = ts.GetMembers().Select(s => s).Where(t => t.Kind == SymbolKind.Method).ToList();

                //    foreach (IMethodSymbol bb in d.ToArray())
                //    {
                //        if (bb.IsExtensionMethod)
                //        {
                //            if (bb.Parameters[0].Type.Name.Contains("char"))

                //                MessageBox.Show(bb.Name);
                //        }

                //    }

                //}
            }
        }

        private static SyntaxNode GetNode(SyntaxTree tree)
        {
            var syntaxRoot = tree.GetRoot();
            var TypeDefinition = syntaxRoot.DescendantNodes().OfType<TypeDeclarationSyntax>()
                .First();
            return TypeDefinition;
        }

        private List<ISymbol> BaseClassMembers(ISymbol symbol)
        {
            List<ISymbol> s = new List<ISymbol>();
            ITypeSymbol b = ((ITypeSymbol)symbol).BaseType;
            while (b != null)
            {
                if (b.BaseType == null)
                    break;
                s.AddRange(b.GetMembers().Where(x => s.FirstOrDefault(y => y.Name == x.Name) == null).ToList());
                b = ((ITypeSymbol)b).BaseType;
            }
            return s;
        }

        private List<ISymbol> BaseClassMembers(List<ISymbol> symbols)
        {
            List<ISymbol> s = new List<ISymbol>();

            foreach (ISymbol symbol in symbols)
            {
                if (symbol is ITypeSymbol)
                {
                    ITypeSymbol b = ((ITypeSymbol)symbol).BaseType;

                    while (b != null)
                    {
                        if (b.BaseType == null)
                            break;
                        s.AddRange(b.GetMembers().Where(x => s.FirstOrDefault(y => y.Name == x.Name) == null).ToList());
                        b = ((ITypeSymbol)b).BaseType;
                    }
                }
            }
            return s;
        }

        private string ExtractMethodName(string name)
        {
            string names = name;

            int s = name.IndexOf("(");

            names = names.Substring(s + 1, name.Length - s - 1);

            return names;
        }

        private enum methods
        {
            start,
            end,
            none
        }

        private List<ISymbol> GetBuiltInTypeCompletion(VSProject vp, string name, string names, methods me)
        {
            var b = named[vp.FileName].Select(s => s).Where(t => t.Name.ToLower() == names.ToLower()).FirstOrDefault();
            if (b == null)
                return null;
            var symbols = b.GetMembers().OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
            if (symbols != null && symbols.Count() > 0)
            {
                var d = symbols;

                if (d != null)
                {
                    var property = d
                        .ToList()
                        .Where(s => s.Kind == SymbolKind.Property && s.Name == name)
                        .Cast<IPropertySymbol>()
                        .FirstOrDefault();

                    if (property != null)
                    {
                        //d = property.Type;
                        var p = property.Type.GetMembers();
                        return property.Type.GetMembers().Concat(BaseClassMembers(property.Type)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                    }
                    var method = d
                   .ToList()
                   .Where(s => s.Kind == SymbolKind.Method && s.Name == name)
                   .Cast<IMethodSymbol>()
                   .FirstOrDefault();
                    if (method != null)
                    {
                        if (me == methods.start)
                            return symbols.ToList();

                        return method.ReturnType.GetMembers().Concat(BaseClassMembers(method.ReturnType)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                    }
                    var events = d
                     .ToList()
                     .Where(s => s.Kind == SymbolKind.Event && s.Name == name)
                     .Cast<IEventSymbol>()
                     .FirstOrDefault();
                    if (events != null)
                    {
                        List<ISymbol> c = new List<ISymbol>();
                        c.Add(events);
                        return c;
                    }

                    var field = d
                   .ToList()
                   .Where(s => s.Kind == SymbolKind.Field)
                   .Cast<IFieldSymbol>()
                   .FirstOrDefault();

                    if (field != null)
                    {
                        var fieldType = field.Type;
                        return fieldType.GetMembers().Concat(BaseClassMembers(fieldType)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                    }
                }
                return new List<ISymbol>();
            }
            return new List<ISymbol>();
        }

        public List<ISymbol> GetCodeCompletion(VSProject vp, string filename, string content, int offset, string name, string names)
        {
            List<ISymbol> b = new List<ISymbol>();

            Compilation cc = comp[vp.FileName];

            SyntaxTree syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();

            SyntaxTree syntaxTreeUdated = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, filename);

            cc = cc.ReplaceSyntaxTree(syntaxTree, syntaxTreeUdated);

            syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();

            methods me = methods.none;

            if (syntaxTree == null)
                return b;

            var semanticModel = cc.GetSemanticModel(syntaxTree);

            if (name == "this")
            {
                var nv = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).FirstOrDefault();
                if (nv != null)
                    if (nv.ContainingType != null)
                        return nv.ContainingType.GetMembers().ToList();
                return semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).ToList();
            }
            else if (name != "")
            {
                if (name.Contains("(") && name.Contains(")") && name.StartsWith("."))
                {
                    name = ExtractMethodName(name);
                    me = methods.end;
                }
                else if (name.Contains("("))
                {
                    name = ExtractMethodName(name);
                    me = methods.start;
                }
                var symbols = semanticModel.LookupSymbols(offset/*, includeReducedExtensionMethods: true*/).Select(s => s).Where(t => t.Name == name);

                if (symbols.Count() <= 0)
                {
                    var np = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).FirstOrDefault();
                    if (np != null)
                        if (np.ContainingType != null)
                            symbols = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).First().ContainingType.GetMembers().Select(s => s).Where(t => t.Name == name).ToList();
                    if (symbols.Count() <= 0)
                    {
                        var nv = named[vp.FileName].Select(s => s).Where(t => t.Kind == SymbolKind.NamedType && t.Name == name).FirstOrDefault();
                        if (nv != null)
                        {
                            symbols = nv.GetMembers().ToList();
                            return symbols.ToList();
                        }
                    }
                }
                if (symbols != null && symbols.Count() > 0 && symbols.FirstOrDefault() != null && symbols.FirstOrDefault().ContainingType != null)
                {
                    var d = symbols.FirstOrDefault();

                    if (d != null && d.ContainingType != null)
                    {
                        var property = d.ContainingType.GetMembers()
                            .ToList()
                            .Where(s => s.Kind == SymbolKind.Property && s.Name == name)
                            .Cast<IPropertySymbol>()
                            .FirstOrDefault();

                        if (property != null)
                        {
                            d = property.Type;
                            var p = property.Type.GetMembers();
                            return property.Type.GetMembers().Concat(BaseClassMembers(property.Type)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                        }
                        var method = d.ContainingType.GetMembers()
                       .ToList()
                       .Where(s => s.Kind == SymbolKind.Method && s.Name == name)
                       .Cast<IMethodSymbol>()
                       .FirstOrDefault();
                        if (method != null)
                        {
                            if (me == methods.start)
                                return symbols.ToList();

                            d = method.ReturnType;
                            return method.ReturnType.GetMembers().Concat(BaseClassMembers(method.ReturnType)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                        }
                        var events = d.ContainingType.GetMembers()
                         .ToList()
                         .Where(s => s.Kind == SymbolKind.Event && s.Name == name)
                         .Cast<IEventSymbol>()
                         .FirstOrDefault();
                        if (events != null)
                        {
                            //if (me == methods.start)
                            //    return symbols.ToList();

                            List<ISymbol> c = new List<ISymbol>();
                            c.Add(events);
                            return c;

                            //return method.ReturnType.GetMembers().Concat(BaseClassMembers(method.ReturnType)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                        }

                        var field = d.ContainingType.GetMembers()
                       .ToList()
                       .Where(s => s.Kind == SymbolKind.Field)
                       .Cast<IFieldSymbol>()
                       .FirstOrDefault();

                        if (field != null)
                        {
                            var fieldType = field.Type;
                            return fieldType.GetMembers().Concat(BaseClassMembers(fieldType)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                        }
                    }
                    return semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).First().ContainingType.GetMembers().Select(s => s).Where(t => t.Name == name).ToList();
                    var ts = semanticModel.LookupSymbols(offset).Select(s => s).Where(t => t.Name == name).FirstOrDefault().ContainingType;
                    return ts.GetMembers().ToList().Union(BaseClassMembers(ts)).ToList();
                }
                else
                {
                    if (names.Contains("."))
                    {
                        string[] dnames = names.Split(".".ToCharArray());

                        names = dnames[dnames.Length - 1];

                        var br = GetBuiltInTypeCompletion(vp, name, names, me);
                        if (br != null && br.Count > 0)
                            return br;
                    }
                    var types = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).First().ContainingType.GetMembers().Select(s => s).Where(t => t.Name == name).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                    //      types.AddRange(named[vp.FileName].Select(s => s).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList());

                    var ns = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.Namespace).ToList();
                    types.AddRange(ns);
                    return types;
                }
            }

            var type = semanticModel.LookupSymbols(offset).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
            var ps = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.Namespace).ToList();
            type.AddRange(ps);
            return type;
        }

        public List<INamespaceOrTypeSymbol> GetProjectTypes(string filename)
        {
            return named[filename].Select(s => s).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
        }

        public Dictionary<VSProject, ArrayList> GetDataSources()
        {
            Dictionary<VSProject, ArrayList> dict = new Dictionary<VSProject, ArrayList>();
            foreach (VSProject vp in this.projects)
            {
                vp.GetDataSources(dict);
            }

            return dict;
        }

        public void Dispose()
        {
        }

        public void RestorePackages(bool restore)
        {
            foreach (VSProject vp in this.projects)
            {
                vp.SetRestorePackages(restore);
            }
        }

        public enum OutputType
        {
            exec,
            dlls,
            both
        }

        public ArrayList GetExecutables(OutputType output = OutputType.dlls)
        {
            ArrayList E = new ArrayList();
            foreach (VSProject p in projects)
            {
                string s = p.GetProjectExec();

                if (Directory.Exists(s) == false)
                {
                    if (output == OutputType.dlls)
                    {
                        if (Path.GetExtension(s) != ".exe")
                            if (s != "")
                                E.Add(s);
                    }
                    else if (output == OutputType.exec)
                    {
                        if (Path.GetExtension(s) == ".exe")
                            if (s != "")
                                E.Add(s);
                    }
                    else
                    {
                        if (s != "")
                            E.Add(s);
                    }
                }
            }

            return E;
        }

        public ArrayList GetDependencyOrder(ArrayList T)
        {
            ArrayList L = new ArrayList();

            foreach (VSProject p in this.projects)
            {
                bool added = false;

                ArrayList D = p.GetProjectReferences("ProjectReference");

                L.Add(D);

                T.Add(D);

                int i = 0;
                foreach (ArrayList d in T)
                {
                    if (d.Count <= D.Count)
                    {
                        T.Insert(i, D);
                        added = true;
                        break;
                    }
                    i++;
                }
                if (added == false)
                    T.Add(D);
            }

            return L;
        }

        public string GetProject(string name)
        {
            var solution = VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.Project p in solution.Projects)
            {
                string s = Path.GetDirectoryName(solutionFileName) + "\\" + p.Path;

                if (s == name)
                {
                    return p.content;
                }
            }

            return "";
        }

        public void RemoveProject(string name)
        {
            string content = GetProject(name);

            string text = File.ReadAllText(solutionFileName);

            if (content != "")

                text = text.Replace(content, "");

            File.WriteAllText(solutionFileName, text);
        }

        public ArrayList GetSolutionPlatforms()
        {
            ArrayList L = new ArrayList();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.GlobalSection se in s.Global)
            {
                if (se.Name == "SolutionConfigurationPlatforms")
                {
                    foreach (string p in se.Entries.Keys)
                        L.Add(p);
                }
            }

            return L;
        }

        public Dictionary<string, string> GetProjectGuids()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.Project p in s.Projects)
            {
                if (dict.ContainsKey(p.Guid.ToString()) == true)
                    continue;

                dict.Add(p.Guid.ToString().ToUpper(), p.Name);
            }

            return dict;
        }

        public ArrayList GetProjectPlatforms()
        {
            ArrayList L = new ArrayList();

            VSProvider.Solution s = (VSProvider.Solution)VSProvider.SolutionParser.Parse(solutionFileName);

            foreach (VSProvider.GlobalSection se in s.Global)
            {
                if (se.Name == "ProjectConfigurationPlatforms")
                {
                    foreach (string p in se.Entries.Keys)
                        L.Add(p);
                }
            }

            return L;
        }

        public static ArrayList GetSubstringByString(int start, string a, string b, string c)
        {
            ArrayList S = new ArrayList();

            int s = 0;

            while (s >= 0)
            {
                s = c.IndexOf(a, start);

                int g = (c.IndexOf(b, start) - c.IndexOf(a, start) - a.Length);

                if (s < 0)
                    return S;
                if (g < 0)
                    return S;

                string d = c.Substring((s + a.Length), g);

                start = s + g + b.Length;

                S.Add(d);
            }

            return S;
        }

        public static string GetSubstring(ref int start, string a, string b, string c)
        {
            string w = "";

            int s = 0;

            s = c.IndexOf(a, start);

            if (s < 0)
                return w;

            int g = (c.IndexOf(b, s + 2) - c.IndexOf(a, start) - a.Length);

            if (g < 0)
                return w;

            string d = c.Substring((s + a.Length), g);

            start = s + g + b.Length;

            return d;
        }

        public static ArrayList GetVSProjects(string file)
        {
            ArrayList C = new ArrayList();

            string content = File.ReadAllText(file);

            ArrayList S = GetSubstringByString(0, "Project(", "EndProject\r", content);

            ArrayList F = new ArrayList();

            foreach (string s in S)
            {
                string[] cc = s.Split("\n".ToCharArray());

                string c = cc[0];

                vsproject v = new vsproject();

                v.content = content;

                v.project = s;

                v.Load(c);

                F.Add(v);
            }

            return F;
        }

        public static void CreateVSolutionFile(string file, string sf)
        {
            string content = File.ReadAllText(file);

            FileStream b = File.Create(sf);
            b.Close();

            File.WriteAllText(sf, content);
        }

        public void MockProject()
        {
            ArrayList S = VSSolution.GetVSProjects(solutionFileName);

            string content = File.ReadAllText(solutionFileName);

            foreach (vsproject v in S)
            {
                v.content = content;

                string n = v.name.Replace("\"", "");

                n = n.Trim();

                v.name = v.name.Replace(n, '_' + n);

                v.file = v.file.Replace(n + ".", '_' + n + ".");

                string g = v.GetProject();

                g = v.ModifyProject(g);

                content = g;
            }

            string file = solutionFileName;

            string name = Path.GetFileName(file).Trim();

            file = file.Replace(name, "_" + name);

            File.WriteAllText(file, content);
        }
    }

    public class vsproject
    {
        public static string pattern = "Project(\"{%g}\") = \"%n\", \"%f\", \"{%s}\"";

        public string content { get; set; }

        public string project { get; set; }

        public string guid { get; set; }
        public string name { get; set; }
        public string file { get; set; }
        public string guids { get; set; }

        public string Extract(string g)
        {
            g = g.Replace("\"", "");
            g = g.Replace("{", "");
            g = g.Replace("}", "");
            g = g.Replace("(", "");
            g = g.Replace(")", "");
            g = g.Replace("Project", "");

            return g.Trim();
        }

        public void Load(string s)
        {
            project = s;

            string[] cc = s.Split("\n".ToCharArray());

            string c = cc[0];

            cc = c.Split("=".ToCharArray());

            string g = Extract(cc[0]);

            //g = g.Replace("\"", "");
            //g = g.Replace("{", "");
            //g = g.Replace("}", "");
            //g = g.Replace("(", "");
            //g = g.Replace(")", "");
            //g = g.Replace("Project", "");

            guid = g;

            string[] dd = cc[1].Split(",".ToCharArray());
            name = Extract(dd[0]);
            file = Extract(dd[1]);
            guids = Extract(dd[2]);
        }

        public void RemoveProject()
        {
            content = content.Replace(project, "");
        }

        public string GetProject()
        {
            string s = pattern;
            s = s.Replace("%g", guid);
            s = s.Replace("%n", name);
            s = s.Replace("%f", file);
            s = s.Replace("%s", guids);

            return s;
        }

        public string ModifyProject(string s)
        {
            content = content.Replace(project, s);

            return content;
        }
    }

    public class projects
    {
        public ArrayList P { get; set; }
    }
}