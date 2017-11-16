// VSSolution



using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using VSParsers;

namespace VSProvider
{
	public class Program
	{
		public static void Mains()
		{
			string file = "C:\\MSBuildProjects-beta\\VStudio.sln";
			VSSolutionLoader rw = new VSSolutionLoader();
			System.Windows.Forms.TreeView vv = rw.LoadProject(file);

			VSSolution vs = vv.Tag as VSSolution;

			MessageBox.Show("VSolution loaded.." + vs.Name);

			vs.CompileSolution();
		}
	}


    public class VSSolution
    {
        
        private static readonly Type s_SolutionParser;
        private static readonly MethodInfo s_SolutionParser_parseSolution;
        //private static readonly PropertyInfo s_SolutionParser_projects;
        private static readonly FieldInfo s_SolutionParser_projects;
        private static readonly PropertyInfo s_SolutionParser_solutionReader;

        public List<VSProject> projects;
        public string solutionFileName;

        public Dictionary<string, VSProject> dd { get; set; }

        static VSSolution()
        {

            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            //s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            

            
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionFile, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_projects = s_SolutionParser.GetField("_projects", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static);
        }

        public VSSolution()
        {
        }
        public Solution SolutionParsed { get; set; }

        public void SetAutoResetEvent(bool b)
        {
            foreach (VSProject vp in Projects)
                if (!b)
                    vp.auto.Reset();
                else vp.auto.Set();
        }
        public void SetAutoResetEventForProjectName(string ProjectName, bool b)
        {
            var vp = GetProjectbyName(ProjectName);
            if (vp == null)
                return;
            if (!b)
                vp.auto.Reset();
            else vp.auto.Set();

        }
        public ManualResetEvent SynchronizedEventForCompileFile(string file)
        {
            var b = GetProjectbyCompileItem(file);
            if (b == null)
                MessageBox.Show("No project for " + file);
            return b.auto;
        }
        public VSProject GetProjectbyName(string name)
        {
            foreach (VSProject p in projects)
                if (p.Name == name)
                    return p;

            return null;
        }

        public static event EventHandler<OpenFileEventArgs> OpenFile;

        public void LoadFileFromContent(string content, VSProject vp, string name, ISymbol symbol = null)
        {
            OpenFileEventArgs args = new OpenFileEventArgs();
            args.Threshold = 10;
            args.filename = name;
            args.content = content;
            args.vp = vp;
            args.symbol = symbol;

            if(OpenFile != null)
                OpenFile(null, args);
        }
        public void LoadFileFromProject(string content, VSProject vp, string name, ISymbol symbol = null, ReferenceLocation? Location = null, TextSpan? Span = null)
        {
            OpenFileEventArgs args = new OpenFileEventArgs();
            args.Threshold = 10;
            args.filename = name;
            args.content = content;
            args.vp = vp;
            args.symbol = symbol;
            args.Location = Location;
            args.Span = Span;
            
            if(OpenFile != null)
                 OpenFile(null,args);

        }
        public static event EventHandler<OpenReferenceEventArgs> OpenReferences;

        public void OpenReferencesTool(string content, VSProject vp, string name, ISymbol symbol = null)
        {
            OpenReferenceEventArgs args = new OpenReferenceEventArgs();
            
            args.filename = name;
            args.content = content;
            args.vp = vp;
            args.symbol = symbol;

            if (OpenReferences != null)
                OpenReferences(null, args);
        }
        public static event EventHandler<OpenReferenceEventArgs> OpenCallHierarchy;

        public void OpenCallHierarchyTool(string content, VSProject vp, string name, ISymbol symbol = null)
        {
            OpenReferenceEventArgs args = new OpenReferenceEventArgs();

            args.filename = name;
            args.content = content;
            args.vp = vp;
            args.symbol = symbol;

            if (OpenCallHierarchy != null)
                OpenCallHierarchy(null, args);
        }
        public static event EventHandler<OpenSearchEventArgs> OpenSearch;

        public void OpenSearchTool(string filename, SearchDomain searchDomain = SearchDomain.file)
        {
            OpenSearchEventArgs args = new OpenSearchEventArgs();

            args.filename = filename;
            args.searchDomain = searchDomain;
        
            if (OpenSearch != null)
                OpenSearch(null, args);
        }
        public static event EventHandler<OpenSyntaxGraphEventArgs> OpenSyntaxGraph;

        public static void OpenSyntaxGraphTool(XElement xe)
        {
            OpenSyntaxGraphEventArgs args = new OpenSyntaxGraphEventArgs();

            args.xe = xe;
           

            if (OpenSyntaxGraph != null)
                OpenSyntaxGraph(null, args);
        }
        public void LoadErrors(VSProject vp, List<Diagnostic> errors)
        {
            OpenFileEventArgs args = new OpenFileEventArgs();
           
            args.Errors = errors;
            args.vp = vp;

            if (Errors != null)
                Errors(null, args);

        }

        public static event EventHandler<OpenFileEventArgs> Errors;

        public VSProject GetProjectbyExec(string name)
        {
            foreach (VSProject p in projects)
                if (p.GetProjectExec() == name)
                    return p;

            return null;
        }
        public string GetProjectExec(string name)
        {
            VSProject p = GetProjectbyName(name);
            if(p != null)
            return p.GetProjectExec();

            
            return "";
        }
        public VSProject GetProjectbyFileName(string name)
        {
            foreach (VSProject p in projects)
                if (p.FileName == name)
                    return p;

            return null;
        }
        object obs = new object();

        public VSProject GetProjectbyCompileItem(string name)
        {
            //lock (obs)
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                name = Path.GetFullPath(name);
                foreach (VSProject p in projects)
                {
                    ArrayList C = p.GetCompileItems();

                    foreach (string file in C)
                        if (file.ToLower().Trim().Equals( name.ToLower().Trim()))
                        {

                            return p;
                        }
                }

                return null;
            }
        }

        public VSProject MainVSProject { get; set; }

        public SolutionFile sf { get; set; }

        public VSSolution(string solutionFileName)
        {
            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }

            var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            //var streamReader = new StreamReader(solutionFileName);
            //{
            //    s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
            //    s_SolutionParser_parseSolution.Invoke(solutionParser, null);
            //}

            sf = (SolutionFile)s_SolutionParser_parseSolution.Invoke(solutionParser, new[] { solutionFileName });

            this.solutionFileName = solutionFileName;

            projects = new List<VSProject>();
            //var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
            //var array = (Array)s_SolutionParser_projects.GetValue(sf);
            var array = (Dictionary<string, ProjectInSolution>)s_SolutionParser_projects.GetValue(sf);
            //for (int i = 0; i < array.Keys.Count; i++)
            //{
            //    projects.Add(new VSProject(this, array.GetValue(i)));
            //}


            foreach (string s in array.Keys)
            {
                projects.Add(new VSProject(this, array[s]));
            }
            //streamReader.Close();
            //streamReader.Dispose();
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
        public VSProject GetVSProjectByName(string name)
        {
            //file = file.Replace("\\\\", "\\");

            foreach (VSProject p in this.Projects)
            {
                if (name == p.Name)
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

        static public void GetAllTypes(Compilation cc, INamespaceSymbol c, List<INamespaceOrTypeSymbol> b)
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

        static public List<INamespaceOrTypeSymbol> GetAllTypes(Compilation cc)
        {
            List<INamespaceOrTypeSymbol> b = new List<INamespaceOrTypeSymbol>();
            try
            {
                foreach (INamespaceSymbol ns in cc.GlobalNamespace.GetNamespaceMembers())
                {
                    b.Add(ns);
                    foreach (INamedTypeSymbol s in ns.GetTypeMembers())
                    {
                        var bytes = Serialized(s);
                        var obs = Deserialize(bytes, s.GetType());
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

        public static byte[] Serialized(object obs)
        {
            var size = Marshal.SizeOf(obs);
            // Both managed and unmanaged buffers required.
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            // Copy object byte-to-byte to unmanaged memory.
            Marshal.StructureToPtr(obs, ptr, false);
            // Copy data from unmanaged memory to managed buffer.
            Marshal.Copy(ptr, bytes, 0, size);
            // Release unmanaged memory.
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }
        public static object Deserialize(byte[] bytes, Type T)
        {
            int size = bytes.Length;
            //var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            var obs = Marshal.PtrToStructure(ptr, T);
            Marshal.FreeHGlobal(ptr);
            return obs;
        }
        public Dictionary<string, List<INamespaceOrTypeSymbol>> nts = new Dictionary<string, List<INamespaceOrTypeSymbol>>();

        public List<INamespaceOrTypeSymbol> GetAllTypes(string FileName)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                List<INamespaceOrTypeSymbol> bs = new List<INamespaceOrTypeSymbol>();
                foreach(string s in nts.Keys)
                {
                    bs.AddRange(nts[s]);
                }
            
                return bs;
            }

            if (nts.ContainsKey(FileName))
                return nts[FileName];

            if (comp == null)
                return new List<INamespaceOrTypeSymbol>();

            if (!comp.ContainsKey(FileName))
                return new List<INamespaceOrTypeSymbol>();

            Compilation cc = comp[FileName];
            List<INamespaceOrTypeSymbol> b = GetAllTypes(cc);
            nts[FileName] = b;

            //var d = b.Select(s => s).Where(t => t.GetAttributes() != null).ToList();

            return b;
        }

        public List<string> TypesNames(string FileName)
        {
            List<string> names = new List<string>();
            VSProject vp = this.GetProjectbyCompileItem(FileName);
            if (vp == null)
                return names;
            string file = vp.FileName;
            var b = GetAllTypes(file);
            
            foreach (var name in b)
                if (!names.Contains(name.Name))
                    names.Add(name.Name);
            return names;
        }

        public ISymbol GetSymbol(ISymbol symbol)
        {

            foreach (Compilation c in comp.Values)
            {
                List<INamespaceOrTypeSymbol> d = GetAllTypes(c);
                //foreach (INamespaceOrTypeSymbol p in d)
                {
                    if (d.Select(s => s).Any(s => s.Equals(symbol)))
                        return d.Select(s => s).Where(p => p.Equals(symbol)).FirstOrDefault();
                            
                }
            }
            return null;
        }

        public VSSolutionLoader rw { get; set; }

        public Dictionary<string, Compilation> comp { get; set; }

        public Dictionary<string, List<INamespaceOrTypeSymbol>> named { get; set; }

        private void LoadSolution(Task<Microsoft.CodeAnalysis.Solution> solution)
        {
        }

        public Microsoft.CodeAnalysis.Solution solution { get; set; }

        public MSBuildWorkspace workspace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void CompileSolution()
        {

            Stopwatch sw = Stopwatch.StartNew();

            //bool success = true;

            // int count = 0;

            //EmitResult result = null;

            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

			comp = new Dictionary<string, Compilation>();

            named = new Dictionary<string, List<INamespaceOrTypeSymbol>>();

            workspace = null;
            //Microsoft.CodeAnalysis.Solution solution = null;
            ProjectDependencyGraph projectGraph = null;

            //try
            //{
            solution = null;
            workspace = MSBuildWorkspace.Create();
            //solution = await workspace.OpenSolutionAsync(solutionFileName);

            solution = workspace.OpenSolutionAsync(solutionFileName).Result;

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
                

                VSProject vv = this.GetProjectbyFileName(project.FilePath);

                if(vv != null)
                {
                    vv.Project = project;
                }

                comp.Add(project.FilePath, projectCompilation);
                
                List<INamespaceOrTypeSymbol> ns = GetAllTypes(project.FilePath);

                
                named.Add(project.FilePath, ns);
                   }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Time compilation: {0}ms", sw.Elapsed.TotalMilliseconds);

       

        }
        public event EventHandler SolutionCompiled;

        EventHandler OnSolutionCompiled;


        public Dictionary<DocumentId, Document> documents { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="solutionFileName"></param>
        public async void CompileSolution(string solutionFileName)
        {


            documents = new Dictionary<DocumentId, Document>();

            Stopwatch sw = Stopwatch.StartNew();

            //bool success = true;

            // int count = 0;

            //EmitResult result = null;

            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

            comp = new Dictionary<string, Compilation>();

            named = new Dictionary<string, List<INamespaceOrTypeSymbol>>();

            //var workspace = null;
            //Microsoft.CodeAnalysis.Solution solution = null;
            ProjectDependencyGraph projectGraph = null;

            //try
            //{
            //var solution = null;
            var workspace = MSBuildWorkspace.Create();
            //solution = await workspace.OpenSolutionAsync(solutionFileName);

            var solution = workspace.OpenSolutionAsync(solutionFileName).Result;

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
               // Stopwatch sw = Stopwatch.StartNew();

                Compilation projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;

                //sw.Stop();

                //System.Diagnostics.Debug.WriteLine("Time taken compilation creation: {0}ms", sw.Elapsed.TotalMilliseconds);

                Microsoft.CodeAnalysis.Project project = solution.GetProject(projectId);

               


                //VSProject vv = this.GetProjectbyFileName(project.FilePath);

                //if (vv != null)
                //{
                //    vv.Project = project;
                //}

                comp.Add(project.FilePath, projectCompilation);

                //List<INamespaceOrTypeSymbol> ns = GetAllTypes(project.FilePath);


                //named.Add(project.FilePath, ns);

                

            }
            sw.Stop();

            System.Diagnostics.Debug.WriteLine("Time taken compilation creation: {0}ms", sw.Elapsed.TotalMilliseconds);


            //MessageBox.Show("Time of compilation - " + sw.Elapsed.TotalMilliseconds);

            if(OnSolutionCompiled != null)
            {
                OnSolutionCompiled(this, new EventArgs());
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

            int s = name.LastIndexOf("(");

            int c = name.LastIndexOf(".");

            int b = name.LastIndexOf(".", c - 1);

            names = names.Substring(b + 1, s - b - 1);

            return names;
        }
        private string ExtractMemberName(string name)
        {
            string names = name;

            int c = name.LastIndexOf(".");

            int b = name.LastIndexOf(".", c - 1);

            names = names.Substring(b + 1, c - b - 1);

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

        public List<ISymbol> GetMembers(string content, bool addProperties = false)
        {
            List<ISymbol> b = new List<ISymbol>();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, "content");
            if (syntaxTree == null)
                return b;
            var root = (CompilationUnitSyntax)syntaxTree.GetRoot();
            var compilation = CSharpCompilation.Create("content")
                              .AddReferences(
                                 MetadataReference.CreateFromFile(
                                   typeof(object).Assembly.Location))
                              .AddSyntaxTrees(syntaxTree);

           // var compilation = comp.Values.First();

           // compilation = compilation.AddSyntaxTrees(syntaxTree);

            var model = compilation.GetSemanticModel(syntaxTree);
            IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>().ToList();
            
            IEnumerable<ConstructorDeclarationSyntax> constructors = root.DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>().ToList();
            IEnumerable<PropertyDeclarationSyntax> properties = root.DescendantNodes()
           .OfType<PropertyDeclarationSyntax>().ToList();
            IEnumerable<OperatorDeclarationSyntax> operators = root.DescendantNodes()
         .OfType<OperatorDeclarationSyntax>().ToList();
            IEnumerable<IndexerDeclarationSyntax> indexers = root.DescendantNodes()
        .OfType<IndexerDeclarationSyntax>().ToList();
            IEnumerable<EventFieldDeclarationSyntax> events = root.DescendantNodes() 
       .OfType<EventFieldDeclarationSyntax>().ToList();

            

            foreach (var p in constructors)

                b.Add(model.GetDeclaredSymbol(p));

            foreach (var p in methods)

                b.Add(model.GetDeclaredSymbol(p));

            if (addProperties)
            {
                foreach (var p in properties)
                    b.Add(model.GetDeclaredSymbol(p));
                foreach (var p in operators)
                    b.Add(model.GetDeclaredSymbol(p));
                foreach (var p in indexers)
                    b.Add(model.GetDeclaredSymbol(p));
                foreach (var p in events)
                {
                    EventFieldDeclarationSyntax ev = p;
                    TypeSyntax type = ev.Declaration.Type;
                    SyntaxToken identifier = ev.Declaration.Variables[0].Identifier;
                    
                 
                    EventDeclarationSyntax evt = SyntaxFactory.EventDeclaration(type, identifier);
                    var df = model.LookupSymbols(p.GetLocation().SourceSpan.Start, null,identifier.Text).FirstOrDefault();
                    
                    
                    b.Add(/*model.GetDeclaredSymbol(p)*/ df);
                }
            }
            //var cs = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();


            //foreach (var c in cs)
            //{

            //    var createCommandList = new List<ISymbol>();


            //    {
            //        b.Add(model.GetDeclaredSymbol(c));
            //    }

            //}
            //var ns = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();


            //foreach (var c in ns)
            //{


            //    {
            //        b.Add(model.GetDeclaredSymbol(c));
            //    }

            //}
            return b;
        }
        public bool HasMembers(VSProject vp, string filename, string content)
        {

            List<ISymbol> b = new List<ISymbol>();

            if (comp == null)
                return false;

            if (vp.FileName == null)
                return false;

            if (!comp.ContainsKey(vp.FileName))
                return false;

            Compilation cc = comp[vp.FileName];

            SyntaxTree syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).FirstOrDefault();

            if (syntaxTree == null)
                return false;
           
           
            var semanticModel = cc.GetSemanticModel(syntaxTree);

            var root = syntaxTree.GetRoot();
            IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>().ToList();

            var cs = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            if (cs.Count > 0)
                return true;
            else return false;
        }
            public List<ISymbol> GetMembers(VSProject vp, string filename, string content)
        {

            List<ISymbol> b = new List<ISymbol>();

            if (comp == null)
                return b;

            if (!comp.ContainsKey(vp.FileName))
                return b;

            Compilation cc = comp[vp.FileName];

            SyntaxTree syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).FirstOrDefault();

            if (syntaxTree == null)
                return b;

            SyntaxTree syntaxTreeUdated = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, filename);

            cc = cc.ReplaceSyntaxTree(syntaxTree, syntaxTreeUdated);

            syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();

            if (syntaxTree == null)
                return b;

            var semanticModel = cc.GetSemanticModel(syntaxTree);

            var root = syntaxTree.GetRoot();
            IEnumerable<MethodDeclarationSyntax> methods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>().ToList();
            
            var cs = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            
            foreach (var c in cs)
            {

                var createCommandList = new List<ISymbol>();
                
                //foreach (var d in c.Members)
                {
                    b.Add(semanticModel.GetDeclaredSymbol(c));
                }

            }
            var ns = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().ToList();

            
            foreach (var c in ns)
            {

                //var createCommandList = new List<ISymbol>();
                
                //foreach (var d in c.Members)
                {
                    b.Add(semanticModel.GetDeclaredSymbol(c));
                }

            }
            return b;

                ////.Select(s => s.GetCompilation()
                //               //                             .GetTypeByMetadataName(className))
                //               //               .FirstOrDefault();
                //var method = @c.GetMembers(methodName)
                //                    .AsList()
                //                    .Where(s => s.Kind == CommonSymbolKind.Method)
                //                    .Cast<MethodSymbol>()
                //                    .FirstOrDefault();
                //var returnType = method.ReturnType as TypeSymbol;
                //var returnTypeProperties = returnType.GetMembers()
                //                                     .AsList()
                //                                     .Where(s => s.Kind == SymbolKind.Property)
                //                                     .Select(s => s.Name);


          

        }

        public ISymbol GetSymbolAtLocation(VSProject vp, string filename, int position, string content = "")
        {
            if (comp == null)
                return null;


            
                Compilation cc = null;
                SyntaxTree syntaxTree = null;
                SemanticModel model = null;

            if (comp == null || vp == null || string.IsNullOrEmpty(vp.FileName))
            {
            }
            else
            {
                cc = comp[vp.FileName];
                syntaxTree = null;
                model = null;



                cc = comp[vp.FileName];
                syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();

                if (!string.IsNullOrEmpty(content))
                {
                    SyntaxTree newSyntaxTree = CSharpSyntaxTree.ParseText(content, null, filename);
                    
                    cc = cc.ReplaceSyntaxTree(syntaxTree, newSyntaxTree);
                    syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();
                }

                model = cc.GetSemanticModel(syntaxTree);

                var d = SymbolFinder.FindSymbolAtPositionAsync(model, position, solution.Workspace).Result;

                return d;
             

            }
            

            return null;
        }
        object obsc = new object();
        public void UpdateSyntaxTree(string FileToLoad, string content)
        {
            lock (obsc)
            {
                Compilation cc = null;
                SyntaxTree syntaxTree = null;
                SemanticModel model = null;

                VSProject vp = GetProjectbyCompileItem(FileToLoad);

                if (comp == null || vp == null || !comp.ContainsKey(vp.FileName))
                {
                    return;
                }
                else
                {

                    cc = comp[vp.FileName];
                    syntaxTree = null;
                    model = null;

                    cc = comp[vp.FileName];
                    syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == FileToLoad).FirstOrDefault();

                    SyntaxTree newSyntaxTree = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, FileToLoad);

                    
                    
                    Document d = solution.GetDocument(syntaxTree);
                   // if(solution.ContainsDocument(d.Id))
                   // solution = solution.RemoveDocument(d.Id);
                    solution = solution.WithDocumentSyntaxRoot(d.Id, newSyntaxTree.GetRoot());

                    var st = solution.GetDocument(d.Id).GetSyntaxTreeAsync().Result;
                    if (syntaxTree != null)
                        cc = cc.ReplaceSyntaxTree(syntaxTree, st);
                    else cc.AddSyntaxTrees(newSyntaxTree);

                    d = solution.GetDocument(d.Id);

                    cc = d.Project.GetCompilationAsync().Result;
                    comp[vp.FileName] = cc;
                }
            }
        }
        public SyntaxTree GetSyntaxTree(string filename)
        {
            VSProject vp = GetProjectbyCompileItem(filename);

            if (vp == null || comp == null)
                return null;

            if (!comp.ContainsKey(vp.FileName))
                return null;

            Compilation c = comp[vp.FileName];

            SyntaxTree s = c.SyntaxTrees.Select(b => b).Where(d => d.FilePath == filename).FirstOrDefault();
            
            return s;
        }
        public SemanticModel GetSemanticModel(string filename)
        {
            VSProject vp = GetProjectbyCompileItem(filename);

            if (vp == null || comp == null)
                return null;

            if (!comp.ContainsKey(vp.FileName))
                return null;

            Compilation c = comp[vp.FileName];

            SyntaxTree s = c.SyntaxTrees.Select(b => b).Where(d => d.FilePath == filename).FirstOrDefault();

            return c.GetSemanticModel(s);
        }
        public string GetSyntaxTreeText(string filename)
        {
            string content = "";

            VSProject vp = GetProjectbyCompileItem(filename);

            if (vp == null)
                return content;

            if(!comp.ContainsKey(vp.FileName))
            return content;

            Compilation c = comp[vp.FileName];

            SyntaxTree s = c.SyntaxTrees.Select(b => b).Where(d => d.FilePath == filename).FirstOrDefault();

            if (s == null)
                return content;
            return s.GetText().ToString();
        }
        public ISymbol GetSymbolAtLocation(int offset, string FileToLoad)
        {
            VSProject vp = GetProjectbyCompileItem(FileToLoad);

            ISymbol s = GetSymbolAtLocation(vp, FileToLoad, offset);
            
            return s;
        }
        public IEnumerable<ReferencedSymbol> FindReferences(int offset, string FileToLoad)
        {
            VSProject vp = GetProjectbyCompileItem(FileToLoad);

            ISymbol s = GetSymbolAtLocation(vp, FileToLoad, offset);
            if (s == null)
                return null;

            IEnumerable<ReferencedSymbol> r = GetAllSymbolReferences(s, FileToLoad, vp);

            if (r != null)
            {
                List<ReferencedSymbol> list = r.ToList();
                
            }
            return r;
        }
        public IEnumerable<ReferencedSymbol> GetAllSymbolReferences(ISymbol symbol, string filename, VSProject vp)
        {
            IEnumerable<ReferencedSymbol> references = null;

            List<ReferencedSymbol> referenced = new List<ReferencedSymbol>();

            if (comp == null)
                return references;

        
            {
                Compilation cc = null;
                SyntaxTree syntaxTree = null;
                SemanticModel model = null;

                if (comp == null || vp == null || string.IsNullOrEmpty(vp.FileName))
                {
                }
                else {
                    cc = comp[vp.FileName];
                    syntaxTree = null;
                    model = null;

                    cc = comp[vp.FileName];
                    syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();
                    model = cc.GetSemanticModel(syntaxTree);

            }
                
                IEnumerable<SyntaxNode> methodInvocations = null;

                ISymbol methodSymbol = null;

                SyntaxNode methodInvocation = null;

                if (symbol is IMethodSymbol)
                {

                    IMethodSymbol m = symbol as IMethodSymbol;

                    if (m.Name == ".ctor")
                    {
                        if (comp == null || vp == null || string.IsNullOrEmpty(vp.FileName))
                            methodSymbol = symbol;
                        else
                        {
                            methodInvocations = syntaxTree.GetRootAsync().Result.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Where(s => s.Identifier.Text == symbol.ContainingType.Name);
                            methodInvocation = methodInvocations.FirstOrDefault();
                            if (methodInvocation == null)
                                return references;
                            methodSymbol = model.GetDeclaredSymbol(methodInvocation);
                        }
                        references = SymbolFinder.FindReferencesAsync(methodSymbol, this.solution).Result;
                        referenced.AddRange(references);

                    }
                    else
                    {
                        if (comp == null || vp == null || string.IsNullOrEmpty(vp.FileName))
                            methodSymbol = symbol;
                        else
                        {
                            methodInvocations = syntaxTree.GetRootAsync().Result.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(s => s.Identifier.Text == symbol.Name);
                            methodInvocation = methodInvocations.FirstOrDefault();
                            if (methodInvocation == null)
                                return references;
                            methodSymbol = model.GetDeclaredSymbol(methodInvocation);
                        }
                        references = SymbolFinder.FindReferencesAsync(methodSymbol, this.solution).Result;
                        referenced.AddRange(references);
                    }


                }
                else
                {
                    references = SymbolFinder.FindReferencesAsync(symbol, this.solution).Result;
                    referenced.AddRange(references);
                }
            }
            return referenced;
            
        }

        public Document GetDocument(string FileName, string filename)
        {

            Compilation cc = comp[FileName];

            SyntaxTree syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();

            return solution.GetDocument(syntaxTree);
        }
        public List<Diagnostic> GetParserErrors(VSProject vp, string filename, string content)
        {
            List<ISymbol> b = new List<ISymbol>();

            Compilation cc = comp[vp.FileName];

            SyntaxTree syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();

            SyntaxTree syntaxTreeUdated = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, filename);

            cc = cc.ReplaceSyntaxTree(syntaxTree, syntaxTreeUdated);

            syntaxTree = cc.SyntaxTrees.Select(s => s).Where(t => t.FilePath == filename).First();
            
            if (syntaxTree == null)
                return new List<Diagnostic>();

            var semanticModel = cc.GetSemanticModel(syntaxTree);

            return semanticModel.GetDiagnostics().ToList();
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
                        return nv.ContainingType.GetMembers().Concat(BaseClassMembers(nv.ContainingType)).ToList().OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).Where(s => s.IsStatic == false).ToList();
                return semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).ToList();
            }
            else if (name != "")
            {
                if (name.Contains("(") && name.EndsWith(")."))
                {
                    name = ExtractMethodName(name);
                    me = methods.end;
                }
                else if (name.EndsWith("."))
                {
                    name = ExtractMemberName(name);
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
                if (symbols != null && symbols.Count() > 0 && symbols.FirstOrDefault() != null && symbols.FirstOrDefault().ContainingType == null)
                {
                    var d = symbols.FirstOrDefault();
                    if(d is INamedTypeSymbol)
                    {
                        INamedTypeSymbol ns = (INamedTypeSymbol)d;
                        return ns.GetMembers().OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                    }
                }
                else
                if (symbols != null && symbols.Count() > 0 && symbols.FirstOrDefault() != null && symbols.FirstOrDefault().ContainingType != null)
                {
                    var d = symbols.FirstOrDefault();

                    if(d != null && (d.Kind == SymbolKind.Parameter || d.Kind == SymbolKind.Local)){

                        if(d is IParameterSymbol)
                        {
                            IParameterSymbol v = (IParameterSymbol)d;
                            return v.Type.GetMembers().Concat(BaseClassMembers(v.Type)).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                        }     
                        if(d is ILocalSymbol)
                        {
                            ILocalSymbol v = (ILocalSymbol)d;
                            return v.Type.GetMembers().OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.First()).ToList();
                        }

                    }
                    else 
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
                            
                            if(property.Type.TypeKind == TypeKind.Enum)

                            return property.Type.GetMembers().ToList();

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
                    var types = semanticModel.LookupNamespacesAndTypes(offset).Select(s => s).Where(t => t.Kind == SymbolKind.NamedType).First().ContainingType.GetMembers().Select(s => s).Where(t => t.Name == name).OrderBy(f => f.Name).GroupBy(f => f.Name, f => f).Select(g => g.FirstOrDefault()).ToList();
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
        public List<LocationSource> FindString(string texttofind, string content, string filename, string files, string block, SearchDomain searchDomain = SearchDomain.file)
        {
            List<LocationSource> locations = new List<LocationSource>(); 

            if (searchDomain == SearchDomain.openfiles)
            {
                if (OpenSearch == null)
                    return locations;
                OpenSearchEventArgs args = new OpenSearchEventArgs();
                args.texttofind = texttofind;
                args.filename = filename;
                args.searchDomain = searchDomain;
                OpenSearch(null, args);
                if (args.openfiles == null)
                    return locations;
                Finder finder = new Finder();
                foreach (string file in args.openfiles)
                {
                    SyntaxTree syntaxTree = GetSyntaxTree(file);
                    if (syntaxTree == null)
                        continue;
                    content = syntaxTree.GetText().ToString();
                    char[] tt = texttofind.ToCharArray();
                    char[] te = content.ToCharArray();
                    
                    var s = finder.TW(tt, tt.Length, te, te.Length);
                    foreach (int c in s)
                    {
                        TextSpan ts = new TextSpan(c, texttofind.Length);
                        //LinePositionSpan lp = new LinePositionSpan();
                        LocationSource ls = LocationSource.Create(ts, syntaxTree);
                        locations.Add(ls);
                    }
                   
                }
                return locations;
            }
            else 
            {
                if(searchDomain == SearchDomain.file)
                {
                    if (string.IsNullOrEmpty(texttofind) || string.IsNullOrEmpty(filename))
                        return locations;
                    char[] tt = texttofind.ToCharArray();
                    char[] te = content.ToCharArray();
                    Finder finder = new Finder();
                    var s = finder.TW(tt, tt.Length, te, te.Length);
                    foreach(int c in s)
                    {
                        TextSpan ts = new TextSpan(c, texttofind.Length);
                        LinePositionSpan lp = new LinePositionSpan();
                        LocationSource ls = LocationSource.Create(ts, null);
                        locations.Add(ls);
                    }
                    return locations;
                }
                else if(searchDomain == SearchDomain.project)
                {
                    VSProject vp = GetProjectbyCompileItem(filename);
                    Finder finder = new Finder();
                    foreach (string FileName in vp.GetCompileItems())
                    {
                        SyntaxTree syntaxTree = GetSyntaxTree(FileName);
                        if (syntaxTree == null)
                            continue;
                        content = syntaxTree.GetText().ToString();
                        char[] tt = texttofind.ToCharArray();
                        char[] te = content.ToCharArray();

                        var s = finder.TW(tt, tt.Length, te, te.Length);
                        foreach (int c in s)
                        {
                            TextSpan ts = new TextSpan(c, texttofind.Length);
                            LinePositionSpan lp = new LinePositionSpan();
                            LocationSource ls = LocationSource.Create(ts, syntaxTree);
                            locations.Add(ls);
                        }
                    }
                    return locations;
                }
                else if (searchDomain == SearchDomain.solution)
                {
                    
                    Finder finder = new Finder();
                    foreach (VSProject vp in Projects)
                    {
                        foreach (string FileName in vp.GetCompileItems())
                        {
                            SyntaxTree syntaxTree = GetSyntaxTree(FileName);
                            if (syntaxTree == null)
                                continue;
                            content = syntaxTree.GetText().ToString();
                            char[] tt = texttofind.ToCharArray();
                            char[] te = content.ToCharArray();

                            var s = finder.TW(tt, tt.Length, te, te.Length);
                            foreach (int c in s)
                            {
                                TextSpan ts = new TextSpan(c, texttofind.Length);
                                //LinePositionSpan lp = new LinePositionSpan();
                                LocationSource ls = LocationSource.Create(ts, syntaxTree);
                                locations.Add(ls);
                            }
                        }
                    }
                    return locations;
                }
            }
            return locations;
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
    public class OpenFileEventArgs : EventArgs
    {
        public int Threshold { get; set; }
        public string filename { get; set; }
        public string content { get; set; }
        public VSProject vp { get; set; }
        public ISymbol symbol { get; set; }
        public List<Diagnostic> Errors { get; set; }
        public ReferenceLocation? Location { get; set; }
        public TextSpan? Span { get; set; }
        
    }
    public class OpenReferenceEventArgs : EventArgs
    {
        
        public string filename { get; set; }
        public string content { get; set; }
        public VSProject vp { get; set; }
        public ISymbol symbol { get; set; }
        public List<Diagnostic> Errors { get; set; }
        public ReferenceLocation? Location { get; set; }
        public int Line { get; set; }
    }
    public class OpenSyntaxGraphEventArgs : EventArgs
    {

        public XElement xe { get; set; }
      
    }
    public class OpenSearchEventArgs : EventArgs
    {
        public string texttofind { get; set; }
        public string filename { get; set; }
        public SearchDomain searchDomain { get; set; }
        public List<string> openfiles { get; set; }
    }
    public class LocationSource
    {
        public LocationSource(TextSpan t, SyntaxTree s)
        {
            textSpan = t;
            Source = s;
        }
        public static LocationSource Create(TextSpan t, SyntaxTree s)
        {
            return new VSProvider.LocationSource(t, s);
        }
        public TextSpan textSpan { get; set; }
        public SyntaxTree Source { get; set; }
    }
}