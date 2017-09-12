//using AndersLiu.Reflector.Program.UI.AssemblyTreeNode;

using GACProject;
using ICSharpCode.NRefactory.CSharp;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace VSProvider

{
    public class BasicFileLogger : Logger
    {
        private int _indent;

        private StreamWriter _streamWriter;

        public ArrayList E = new ArrayList();

        public ArrayList W = new ArrayList();

        public ArrayList Es = new ArrayList();

        public ArrayList Ws = new ArrayList();

        private RichTextBox rb { get; set; }

        /// <summary>
        /// Initialize is guaranteed to be called by MSBuild at the start of the build
        /// before any events are raised.
        /// </summary>
        public override void Initialize(IEventSource eventSource)
        {
            // The name of the log file should be passed as the first item in the
            // "parameters" specification in the /logger switch.  It is required
            // to pass a log file to this logger. Other loggers may have zero or more than
            // one parameters.
            //if (null == Parameters)
            //{
            //    throw new LoggerException("Log file was not set.");
            //}
            //string[] parameters = Parameters.Split(';');

            //E = new ArrayList();

            //W = new ArrayList();

            rb = VSProvider.cmds.rb;

            string s = AppDomain.CurrentDomain.BaseDirectory;

            string logFile = s + "\\" + "logger.txt";// parameters[0];

            //if (String.IsNullOrEmpty(logFile))
            //{
            //    throw new LoggerException("Log file was not set.");
            //}

            //if (parameters.Length > 1)
            //{
            //    throw new LoggerException("Too many parameters passed.");
            //}
            
            try
            {
                // Open the file
                _streamWriter = new StreamWriter(logFile, true);
            }
            catch (Exception ex)
            {
                if
                (
                    ex is UnauthorizedAccessException
                    || ex is ArgumentNullException
                    || ex is PathTooLongException
                    || ex is DirectoryNotFoundException
                    || ex is NotSupportedException
                    || ex is ArgumentException
                    || ex is SecurityException
                    || ex is IOException
                )
                {
                    throw new LoggerException("Failed to create log file: " + ex.Message);
                }
                else
                {
                    // Unexpected failure
                    throw;
                }
            }

            // For brevity, we'll only register for certain event types. Loggers can also
            // register to handle TargetStarted/Finished and other events.
            eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStarted);
            eventSource.TaskStarted += new TaskStartedEventHandler(eventSource_TaskStarted);
            // eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);

            eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);

            eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningRaised);
            eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorRaised);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(eventSource_ProjectFinished);
        }

        /// <summary>
        /// Shutdown() is guaranteed to be called by MSBuild at the end of the build, after all
        /// events have been raised.
        /// </summary>
        public override void Shutdown()
        {
            // Done logging, let go of the file
            _streamWriter.Close();
        }

        private void eventSource_ErrorRaised(object sender, Microsoft.Build.Framework.BuildErrorEventArgs e)
        {
            Es.Add(e);

            //E.Add(e.Message);
            //E.Add(e.Code);
            //E.Add(e.File);
            //E.Add(e.LineNumber.ToString());
            //E.Add(e.EndLineNumber.ToString());
            //E.Add(e.ColumnNumber.ToString());
            //E.Add(e.EndColumnNumber.ToString());

            // BuildErrorEventArgs adds LineNumber, ColumnNumber, File, amongst other parameters
            string line = String.Format(": ERROR {0}({1},{2}): ", e.File, e.LineNumber, e.ColumnNumber);
            WriteLineWithSenderAndMessage(line, e);
        }

        private void eventSource_MessageRaised(object sender, Microsoft.Build.Framework.BuildMessageEventArgs e)
        {
            // BuildMessageEventArgs adds Importance to BuildEventArgs
            // Let's take account of the verbosity setting we've been passed in deciding whether to log the message
            if ((e.Importance == MessageImportance.High && IsVerbosityAtLeast(LoggerVerbosity.Minimal))
                || (e.Importance == MessageImportance.Normal && IsVerbosityAtLeast(LoggerVerbosity.Normal))
                || (e.Importance == MessageImportance.Low && IsVerbosityAtLeast(LoggerVerbosity.Detailed))
                )
            {
                WriteLineWithSenderAndMessage(String.Empty, e);
            }
        }

        private void eventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            // The regular message string is good enough here too.
            _indent--;
            WriteLine(String.Empty, e);
        }

        private void eventSource_ProjectStarted(object sender, Microsoft.Build.Framework.ProjectStartedEventArgs e)
        {
            // ProjectStartedEventArgs adds ProjectFile, TargetNames
            // Just the regular message string is good enough here, so just display that.
            //WriteLine(String.Empty, e);
            //indent++;

            if (rb == null)
                return;
            string s = " > " + "Build of project " + e.ProjectFile + " started";
            rb.AppendText("\n" + s);
        }

        private void eventSource_TaskStarted(object sender, TaskStartedEventArgs e)
        {
            // TaskStartedEventArgs adds ProjectFile, TaskFile, TaskName
            // To keep this log clean, this logger will ignore these events.

            if (rb == null)
                return;
            string s = " > " + "Build task  " + e.TaskName + " started";
            rb.AppendText("\n" + s);
        }

        private void eventSource_WarningRaised(object sender, Microsoft.Build.Framework.BuildWarningEventArgs e)
        {
            Ws.Add(e);

            W.Add(e.Message);
            W.Add(e.Code);
            W.Add(e.File);
            W.Add(e.LineNumber.ToString());
            W.Add(e.EndLineNumber.ToString());
            W.Add(e.ColumnNumber.ToString());
            W.Add(e.EndColumnNumber.ToString());

            // BuildWarningEventArgs adds LineNumber, ColumnNumber, File, amongst other parameters
            string line = String.Format(": Warning {0}({1},{2}): ", e.File, e.LineNumber, e.ColumnNumber);
            WriteLineWithSenderAndMessage(line, e);
        }

        /// <summary>
        /// Just write a line to the log
        /// </summary>
        private void WriteLine(string line, Microsoft.Build.Framework.BuildWarningEventArgs e)
        {
            for (int i = _indent; i > 0; i--)
            {
                _streamWriter.Write("\t");
            }
            _streamWriter.WriteLine(line + e.Message);
        }

        /// <summary>
        /// Just write a line to the log
        /// </summary>
        private void WriteLine(string line, Microsoft.Build.Framework.BuildMessageEventArgs e)
        {
            for (int i = _indent; i > 0; i--)
            {
                _streamWriter.Write("\t");
            }
            _streamWriter.WriteLine(line + e.Message);
        }

        /// <summary>
        /// Just write a line to the log
        /// </summary>
        private void WriteLine(string line, Microsoft.Build.Framework.ProjectFinishedEventArgs e)
        {
            for (int i = _indent; i > 0; i--)
            {
                _streamWriter.Write("\t");
            }
            _streamWriter.WriteLine(line + e.Message);
        }

        /// <summary>
        /// Just write a line to the log
        /// </summary>
        private void WriteLine(string line, Microsoft.Build.Framework.ProjectStartedEventArgs e)
        {
            for (int i = _indent; i > 0; i--)
            {
                _streamWriter.Write("\t");
            }
            _streamWriter.WriteLine(line + e.Message);
        }

        /// <summary>
        /// Just write a line to the log
        /// </summary>
        private void WriteLine(string line, Microsoft.Build.Framework.BuildErrorEventArgs e)
        {
            for (int i = _indent; i > 0; i--)
            {
                _streamWriter.Write("\t");
            }
            _streamWriter.WriteLine(line + e.Message);
        }

        /// <summary>
        /// Write a line to the log, adding the SenderName and Message
        /// (these parameters are on all MSBuild event argument objects)
        /// </summary>
        private void WriteLineWithSenderAndMessage(string line, Microsoft.Build.Framework.BuildWarningEventArgs e)
        {
            if (0 == String.Compare(e.SenderName, "MSBuild", true /*ignore case*/))
            {
                // Well, if the sender name is MSBuild, let's leave it out for prettiness
                WriteLine(line, e);
            }
            else
            {
                WriteLine(e.SenderName + ": " + line, e);
            }
        }

        /// Write a line to the log, adding the SenderName and Message
        /// (these parameters are on all MSBuild event argument objects)
        /// </summary>
        private void WriteLineWithSenderAndMessage(string line, Microsoft.Build.Framework.BuildErrorEventArgs e)
        {
            if (0 == String.Compare(e.SenderName, "MSBuild", true /*ignore case*/))
            {
                // Well, if the sender name is MSBuild, let's leave it out for prettiness
                WriteLine(line, e);
            }
            else
            {
                WriteLine(e.SenderName + ": " + line, e);
            }
        }

        /// Write a line to the log, adding the SenderName and Message
        /// (these parameters are on all MSBuild event argument objects)
        /// </summary>
        private void WriteLineWithSenderAndMessage(string line, Microsoft.Build.Framework.BuildMessageEventArgs e)
        {
            if (0 == String.Compare(e.SenderName, "MSBuild", true /*ignore case*/))
            {
                // Well, if the sender name is MSBuild, let's leave it out for prettiness
                WriteLine(line, e);
            }
            else
            {
                WriteLine(e.SenderName + ": " + line, e);
            }
        }
    }

    public class msbuilder
    {
        //public ICSharpCode.SharpDevelop.Project.Solution p = null;

        public string filepath { get; set; }

        public ArrayList PL { get; set; }

        static public ArrayList astresolver(ICSharpCode.NRefactory.TypeSystem.ICompilation cp, IEnumerable<ICSharpCode.NRefactory.CSharp.SyntaxTree> s, IEnumerable<ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile> uf)
        {
            ArrayList L = new ArrayList();

            StringBuilder b = new StringBuilder();

            IEnumerator r = uf.GetEnumerator();

            ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile file;

            r.MoveNext();

            foreach (var c in s)
            {
                file = (ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile)r.Current;

                var astResolver = new ICSharpCode.NRefactory.CSharp.Resolver.CSharpAstResolver(cp, c, file);

                b.AppendLine(file.ToString());

                foreach (var invocation in c.Descendants.OfType<InvocationExpression>())
                {
                    // Retrieve semantics for the invocation:
                    var rr = astResolver.Resolve(invocation) as ICSharpCode.NRefactory.CSharp.Resolver.CSharpInvocationResolveResult;
                    if (rr == null)
                    {
                        // Not an invocation resolve result - could be a UnknownMemberResolveResult instead
                        continue;
                    }

                    b.AppendLine(rr.ToString());

                    L.Add(invocation);

                    if (rr.Member.FullName != "System.String.IndexOf")
                    {
                        // Invocation isn't a string.IndexOf call
                        continue;
                    }
                    if (rr.Member.Parameters.First().Type.FullName != "System.String")
                    {
                        // Ignore the overload that accepts a char, as that doesn't take a StringComparison.
                        // (looking for a char always performs the expected ordinal comparison)
                        continue;
                    }
                    if (rr.Member.Parameters.Last().Type.FullName == "System.StringComparison")
                    {
                        // Already using the overload that specifies a StringComparison
                        continue;
                    }
                    Console.WriteLine(invocation.GetRegion() + ": " + invocation.ToString());

                    // file.IndexOfInvocations.Add(invocation);
                }

                r.MoveNext();

                file = (ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile)r.Current;
            }

            string buffer = b.ToString();

            File.WriteAllText("code.txt", buffer);

            return L;
        }

        static public BasicFileLogger build_alls(string path)
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            BasicFileLogger logger = new BasicFileLogger();

            logger.Parameters = b + "\\" + "logger.txt";

            VSProvider.VSSolution vs = null;

            try
            {
                vs = new VSSolution(Path.GetFullPath(path));
            }
            catch (Exception e) { }

            if (vs == null)
            {
                var solution = SolutionParser.Parse(path);

                vs = LoadProjects(solution, path);
            }

            //var solution = Onion.SolutionParser.Parser.SolutionParser.Parse(Path.GetFullPath(path));

            foreach (VSProject ps in vs.Projects)
            //foreach (Onion.SolutionParser.Parser.Model.Project pg in solution.Projects)
            {
                try
                {
                    if (ps.ProjectType == "SolutionFolder")
                        continue;
                    Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(ps.FileName);

                    pc.Build(logger);

                    pc.ProjectCollection.UnloadAllProjects();
                }
                catch (Exception e)
                {
                }
                finally
                {
                }
            }

            return logger;
        }

        static public BasicFileLogger build_alls_project(string path)
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            BasicFileLogger logger = new BasicFileLogger();

            logger.Parameters = b + "\\" + "logger.txt";

            //VSProvider.VSSolution vs = new VSSolution(Path.GetFullPath(path));
            //foreach (VSProject ps in vs.Projects)
            {
                try
                {
                    //if (ps.projecttype == "solutionfolder")
                    //    continue;
                    Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(path);

                    pc.MarkDirty();

                    pc.Build(logger);

                    pc.ProjectCollection.UnloadAllProjects();
                }
                catch (Exception e)
                {
                }
                finally
                {
                }
            }

            return logger;
        }

        static public void checkprojectreferences(string path, string projectname)
        {
            //         VSProvider.VSSolution vs = new VSSolution(Path.GetFullPath(path));

            //Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)vs.Projects.Single(p => string.Equals(p.Name, projectname, StringComparison.OrdinalIgnoreCase));

            path = Path.GetDirectoryName(path);

            path = Path.GetFullPath(path);

            ICSharpCode.NRefactory.CSharp.CSharpParser parse = new ICSharpCode.NRefactory.CSharp.CSharpParser();

            string[] tree = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            //ICSharpCode.NRefactory.CSharp.SyntaxTree

            IEnumerable<ICSharpCode.NRefactory.CSharp.SyntaxTree> s = tree.Select(f => parse.Parse(File.ReadAllText(f), f));

            //   ICSharpCode.NRefactory.CSharp.SyntaxTree g = new ICSharpCode.NRefactory.CSharp.SyntaxTree();

            //      ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile b = g.ToTypeSystem();

            // ICSharpCode.NRefactory.CSharp.SyntaxTree s;

            IEnumerable<ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile> gf = tree.Select(f => parse.Parse(File.ReadAllText(f), f).ToTypeSystem());

            //IEnumerable<ICSharpCode.NRefactory.TypeSystem.IUnresolvedFile> t = tree.Select(f => parse.Parse(File.ReadAllText(f), f).ToTypeSystem());

            //ICSharpCode.NRefactory.CSharp.CSharpProjectContent project2 = new ICSharpCode.NRefactory.CSharp.CSharpProjectContent();

            ICSharpCode.NRefactory.TypeSystem.IProjectContent pc = new ICSharpCode.NRefactory.CSharp.CSharpProjectContent();

            pc = pc.AddOrUpdateFiles(gf);

            //project2.AddOrUpdateFiles(new string[3]);

            ICSharpCode.NRefactory.TypeSystem.ICompilation cp = pc.CreateCompilation();

            // ICSharpCode.NRefactory.TypeSystem.IType type = cp.FindType(ICSharpCode.NRefactory.CSharp.TypeSystem.KnownTypeCode.Object);

            astresolver(cp, s, gf);
        }

        static public IEnumerable<ICSharpCode.NRefactory.CSharp.Attribute> GetAttributes(TypeDeclaration typeDeclaration)
        {
            return typeDeclaration.Members
                .SelectMany(member => member
                    .Attributes
                    .SelectMany(attr => attr.Attributes));
        }

        static public MainTest mf { get; set; }

        static public MainTest GetProjectDefinitions(string path)
        {
            if (mf == null)
                mf = new MainTest();

            mf.AnalyzeCSharpProject(path);

            return mf;
        }

        static public MainTest GetClassDefinition(string path, string file, ClassMapper mapper)
        {
            if (mapper == null)
                mapper = new ClassMapper();

            MainTest mf = new MainTest();

            mf.mapper = mapper;

            mf.doloaddefaults(file);

            //mf.AnalyzeCSharpProject(file);

            return mf;
        }

        static public void GetClasses()
        {
            //StreamReader reader = new StreamReader(@"..\..\demo.cs");
            //var tex = reader.ReadToEnd();

            //var syntaxTree = new CSharpParser().Parse(tex, tex);

            //var testClass = syntaxTree.Descendants.OfType<TypeDeclaration>().Single(x => x.ClassType == ICSharpCode.NRefactory.CSharp.TypeSystem.MemberTypeOrNamespaceReference);

            //var testClassAttributes = testClass.Attributes.SelectMany(x => x.Attributes).ToArray();
        }

        static public VSProject getvsproject(string paths, int ids)
        {
            string path = Path.GetFullPath(paths);

            VSProvider.VSSolution vs = new VSSolution(Path.GetFullPath(path));

            int i = 0;
            foreach (VSProject p in vs.Projects)
            {
                if (i == 5) return p;

                i++;
            }

            return null;
        }

        public static void OpenProject()
        {
        }

        public void AddRootNode(TreeView msv, string file, int i)
        {
            //int i = msv.Nodes.Count;

            string pp = Path.GetFileNameWithoutExtension(file) + "(" + i.ToString() + " projects)";

            msv.BeginUpdate();

            TreeNodeCollection tnc = msv.Nodes;

            TreeNode gs = new TreeNode();
            gs.ImageKey = "Solution_8308_24";
            gs.SelectedImageKey = "Solution_8308_24";
            gs.Text = pp;

            for (int k = 0; k < i; k++)
            {
                TreeNode bb = msv.Nodes[0];

                msv.Nodes.Remove(bb);

                gs.Nodes.Add(bb);
            }

            msv.Nodes.Clear();

            msv.Nodes.Add(gs);

            msv.EndUpdate();
        }

        static public VSProject LoadProjectItems(VSProject pp, string path)
        {
            string folder = Path.GetDirectoryName(path);

            Microsoft.Build.Evaluation.Project pc = null;

            try
            {
                pc = new Microsoft.Build.Evaluation.Project(path);
            }
            catch (Exception e)
            {
                return pp;
            }

            List<VSProjectItem> list = new List<VSProjectItem>();

            foreach (Microsoft.Build.Evaluation.ProjectItem b in pc.Items)
            {
                string g = Path.GetFullPath(path).Replace(Path.GetFileName(path), "");

                string p = b.EvaluatedInclude.Replace(g, "");

                VSProjectItem v = new VSProjectItem();

                v.ItemType = b.ItemType;

                v.fileName = g + "\\" + b.EvaluatedInclude;

                v.Include = p;

                list.Add(v);
            }

            pp.Items = (IEnumerable<VSProjectItem>)list;

            pc.ProjectCollection.UnloadAllProjects();

            return pp;
        }

        static public VSSolution LoadProjects(VSProvider.ISolution s, string path)
        {
            string folder = Path.GetDirectoryName(path);

            VSSolution vs = new VSSolution();

            List<VSProject> list = new List<VSProject>();

            foreach (VSProvider.Project b in s.Projects)
            {
                //string g = Path.GetFullPath(ps.FileName).Replace(Path.GetFileName(ps.FileName), "");

                VSProject v = new VSProject();

                v.Name = b.Name;

                v.FileName = folder + "\\" + b.Path;

                list.Add(v);

                LoadProjectItems(v, v.FileName);
            }

            vs.Projects = (IEnumerable<VSProject>)list;

            return vs;
        }

        public ProgressBar pgs { get; set; }

        private ArrayList FD { get; set; }

        public Dictionary<string, string> dc { get; set; }

        public int NestedProject(VSProvider.ISolution s, TreeNode master)
        {
            VSProvider.GlobalSection[] obs = s.Global.ToList().ToArray();

            VSProvider.Project[] p = s.Projects.ToArray();

            VSProvider.GlobalSection g = GetSection(obs, "NestedProjects");

            if (g == null)
                return 0;

            foreach (string entry in g.Entries.Keys)
            {
                Guid guid = new Guid(entry);

                //FindProject(p, guid);

                VSProvider.Project pp = FindProject(p, guid);

                if (pp == null)
                    continue;

                TreeNode node = FindNodeExact(master, pp.Name);

                string entrys = g.Entries[entry];

                if (entrys == null)
                    continue;

                guid = new Guid(entrys);

                VSProvider.Project pps = FindProject(p, guid);

                if (pps == null)
                    continue;

                TreeNode nodes = FindNodeExact(master, pps.Name);

                if (nodes == null)
                    continue;

                Move2Node(master, node, nodes);
            }

            Merge2Clean(master);

            MoveFolderNodes(master);

            //MoveResourcesNodes(master);

            return g.Entries.Count();
        }

        public void Move2Node(TreeNode master, TreeNode node, TreeNode nodes)
        {
            if (node.Equals(nodes))
                return;

            master.Nodes.Remove(node);

            node.ImageKey = "CSharpProject_SolutionExplorerNode_24";
            node.SelectedImageKey = "CSharpProject_SolutionExplorerNode_24";

            nodes.Nodes.Insert(nodes.Nodes.Count - 1, node);

            nodes.ImageKey = "Folder_6222";
            nodes.SelectedImageKey = "Folder_6222";

            ArrayList R = FindNodes2Project(nodes, "");
            if (R.Count > 0)
            {
                int i = 0;
                while (i < R.Count)
                {
                    TreeNode ns = R[i] as TreeNode;
                    nodes.Nodes.Remove(ns);
                    nodes.Nodes.Insert(nodes.Nodes.Count, ns);

                    //MoveFolderNodes(nodes);

                    i++;
                }
            }
        }

        public void Merge2Clean(TreeNode master)
        {
            foreach (TreeNode node in master.Nodes)

                MoveNode(node, node.Text);
        }

        private VSProvider.GlobalSection GetSection(VSProvider.GlobalSection[] obs, string name)
        {
            foreach (VSProvider.GlobalSection g in obs)
            {
                if (g.Name == name)
                    return g;
            }

            return null;
        }

        public VSProvider.Project FindProject(VSProvider.Project[] p, Guid guid)
        {
            foreach (VSProvider.Project pp in p)
            {
                if (pp.Guid == guid)
                    return pp;
            }

            return null;
        }

        public TreeNode FindNode(ArrayList L, string name)
        {
            if (L == null)
                return null;

            foreach (TreeNode node in L)
            {
                if (node.Text == name)
                    return node;
            }

            return null;
        }

        public void ProjectCleanUp(TreeNode master)
        {
            ResourceNode(master);

            EmptyRemoveNode(master);

            MoveNode(master, "None");

            MoveNode(master, "EmbeddedResource");

            SetExtensions(master);
        }

        //"resource_32xMD"
        public void MoveFolderNodes(TreeNode master)
        {
            int p = 0;

            ArrayList L = Nodes2Array(master);

            SortNodes(master);

            //ArrayList R = FindNodes(L, "Properties");
            //if (R.Count == 1)
            //{
            //    TreeNode node = R[0] as TreeNode;
            //    master.Nodes.Remove(node);
            //    master.Nodes.Insert(0, node);
            //    p++;

            //}
            //R = FindNodes(L, "Reference");
            //if (R.Count == 1)
            //{
            //    TreeNode node = R[0] as TreeNode;
            //    master.Nodes.Remove(node);
            //    master.Nodes.Insert(p, node);
            //    p++;
            //}

            ArrayList R = FindNodes2Image(master, "Folder_6222");
            R.Reverse();
            if (R.Count > 0)
            {
                int i = 0;
                while (i < R.Count)
                {
                    TreeNode nodes = R[i] as TreeNode;
                    master.Nodes.Remove(nodes);
                    master.Nodes.Insert(0, nodes);

                    //MoveFolderNodes(nodes);

                    i++;
                }
            }

            TreeNode ns = FindNodeExact(master, "Reference");
            if (ns != null)
            {
                //TreeNode node = R[0] as TreeNode;
                master.Nodes.Remove(ns);
                master.Nodes.Insert(0, ns);
                //p++;
            }

            ns = FindNodeExact(master, "Properties");
            if (ns != null)
            {
                //TreeNode node = R[0] as TreeNode;
                master.Nodes.Remove(ns);
                master.Nodes.Insert(0, ns);
                //p++;
            }
        }

        public void SortMainNodes(TreeNode master)
        {
            ArrayList R = FindNodes2Project(master, "");
            if (R.Count > 0)
            {
                int i = 0;
                while (i < R.Count)
                {
                    TreeNode ns = R[i] as TreeNode;
                    master.Nodes.Remove(ns);
                    master.Nodes.Insert(master.Nodes.Count - 1, ns);
                    i++;
                }
            }
        }

        public void MoveResourcesNodes(TreeNode master, int p)
        {
            ArrayList L = Nodes2Array(master);

            L = SortNodes(L);

            int i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;

                //if (node.ImageKey == "Folder_6222")
                //{
                //    master.Nodes.Remove(node);
                //    master.Nodes.Insert(p, node);
                //}
                //else
                if (node.ImageKey == "resource_32xMD")
                {
                    if (node.Text != "Properties" && node.Text != "Reference")
                    {
                        master.Nodes.Remove(node);
                        master.Nodes.Insert(master.Nodes.Count - 1, node);
                    }
                }

                MoveResourcesNodes(node, 2);

                i++;
            }
        }

        public int GetNodeCount(TreeNode master, string name)
        {
            ArrayList L = Nodes2Array(master, name);

            return L.Count;
        }

        public void SortNodes(TreeNode master)
        {
            ArrayList L = Nodes2Array(master);

            L = SortNodes(L);

            //ArrayList S = new ArrayList();

            master.Nodes.Clear();

            foreach (TreeNode node in L)
            {
                master.Nodes.Add(node);
            }
        }

        public ArrayList SortNodes(ArrayList L)
        {
            ArrayList S = new ArrayList();

            foreach (TreeNode node in L)
            {
                S.Add(node.Text);
            }

            S.Sort();

            //S.Reverse();

            ArrayList N = new ArrayList();

            foreach (string s in S)
            {
                foreach (TreeNode nodes in L)
                {
                    if (nodes.Text == s)
                    {
                        N.Add(nodes);
                        break;
                    }
                }
            }

            return N;
        }

        public void SetExtensions(TreeNode node)
        {
            if (dc != null)
            {
                string[] s = node.Text.Split(".".ToCharArray());
                if (s.Count() > 1)
                {
                    int N = s.Count();

                    string exts = "." + s[N - 1];

                    if (dc.ContainsKey(exts))
                    {
                        string dd = (string)dc[exts];
                        if (dd != "")
                        {
                            node.ImageKey = dd;
                            node.SelectedImageKey = dd;
                        }
                    }
                }
            }

            foreach (TreeNode nodes in node.Nodes)
                SetExtensions(nodes);
        }

        public void ResourceNode(TreeNode master)
        {
            TreeNode nodes = FindNode(master, "Resources");

            if (nodes == null)
                return;

            foreach (TreeNode node in nodes.Nodes)
            {
                node.ImageKey = "resource_32xMD";
                node.SelectedImageKey = "resource_32xMD";
            }
        }

        public void EmptyRemoveNode(TreeNode master)
        {
            TreeNode nodes = FindNode(master, "Properties");

            if (nodes == null)
                return;

            ArrayList L = Nodes2Array(nodes);

            int i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;
                if (node.Text == "")
                    nodes.Nodes.Remove(node);

                i++;
            }
        }

        public void MoveNode(TreeNode master, string name)
        {
            TreeNode node = FindNodeExact(master, name);

            if (node == null)
                return;

            int i = 0;

            ArrayList L = new ArrayList();

            foreach (TreeNode ns in node.Nodes)
            {
                L.Add(ns);
            }

            while (i < L.Count)
            {
                TreeNode ng = L[i] as TreeNode;

                node.Nodes.Remove(ng);

                master.Nodes.Add(ng);

                i++;
            }

            master.Nodes.Remove(node);
        }

        public void ProjectForms(TreeNode master)
        {
            ArrayList L = Designers(master);

            TreeNode nodes = FindNode(master, "EmbeddedResource");

            int i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;

                string[] s = node.Text.Split(".".ToCharArray());

                TreeNode ns = FindNode(master, s[0]);

                TreeNode ems = null;

                if (nodes != null)
                    ems = FindNode(nodes, s[0]);

                TreeNode ng = new TreeNode();
                ng.Text = s[0] + ".cs";

                ng.ImageKey = "Formtag_5766_24";
                ng.SelectedImageKey = "Formtag_5766_24";

                if (ems != null)
                    nodes.Nodes.Remove(ems);
                master.Nodes.Remove(ns);
                master.Nodes.Remove(node);

                if (ems != null)
                    ng.Nodes.Add(ems);
                ng.Nodes.Add(ns);
                ng.Nodes.Add(node);

                master.Nodes.Add(ng);

                i++;
            }
        }

        public void ProjectSubForms(TreeNode master, string folder)
        {
            ArrayList L = Designers(master);

            TreeNode nodes = FindNode(master, "EmbeddedResource");

            int i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;

                string names = node.Text.Replace(folder + "\\", "");

                string[] s = names.Split(".".ToCharArray());

                ArrayList N = FindNodes(master, s[0]);

                N.Remove(node);

                TreeNode ns = null;

                if (N.Count > 0)

                    ns = N[0] as TreeNode;

                TreeNode ems = null;

                if (nodes != null)
                    ems = FindNode(nodes, s[0]);

                TreeNode ng = new TreeNode();
                if (ns != null)
                    ng.Text = ns.Text;

                ng.ImageKey = "Formtag_5766_24";
                ng.SelectedImageKey = "Formtag_5766_24";

                if (ems != null)
                    nodes.Nodes.Remove(ems);
                if (ns != null)
                    master.Nodes.Remove(ns);

                master.Nodes.Remove(node);

                if (ems != null)
                    ng.Nodes.Add(ems);
                //ng.Nodes.Add(ns);
                ng.Nodes.Add(node);

                master.Nodes.Add(ng);

                i++;
            }
        }

        public TreeNode FindNode(TreeNode master, string name)
        {
            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper().Contains(name) == true)
                    return node;
            }

            return null;
        }

        public TreeNode FindNodeExact(TreeNode master, string name)
        {
            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper().Equals(name) == true)
                    return node;
            }

            return null;
        }

        public ArrayList FindNodes(TreeNode master, string name)
        {
            ArrayList L = new ArrayList();

            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper().Contains(name) == true)
                    L.Add(node);
            }

            return L;
        }

        public ArrayList FindNodes(ArrayList N, string name)
        {
            ArrayList L = new ArrayList();

            name = name.ToUpper();

            foreach (TreeNode node in N)
            {
                if (node.Text.ToUpper() == name)
                    L.Add(node);
            }

            return L;
        }

        public ArrayList FindNodes2Image(TreeNode master, string name)
        {
            ArrayList L = new ArrayList();

            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.ImageKey.ToUpper() == name)
                    L.Add(node);
            }

            return L;
        }

        public ArrayList FindNodes2Project(TreeNode master, string name)
        {
            ArrayList L = new ArrayList();

            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Tag == null)
                    continue;

                //CreateView_Solution.ProjectItemInfo p = node.Tag as CreateView_Solution.ProjectItemInfo;

                //if (p.psi == null)
                //    L.Add(node);
            }

            return L;
        }

        public ArrayList Designers(TreeNode master)
        {
            ArrayList L = new ArrayList(0);

            foreach (TreeNode node in master.Nodes)
            {
                string text = node.Text;

                if (text.ToLower().Contains(".designer.") == true)
                    L.Add(node);
            }
            return L;
        }

        public void ProjectOrder(TreeNode master, ArrayList FD, TreeNode REF, ArrayList nds)
        {
            if (REF != null)
            {
                master.Nodes.Remove(REF);
                master.Nodes.Insert(0, REF);
            }

            int c = 0;

            for (int i = 0; i < FD.Count; i++)
            {
                TreeNode node = FD[i] as TreeNode;

                if (node.Text == "Properties")
                {
                    node.ImageKey = "properties_16xLG";
                    node.SelectedImageKey = "properties_16xLG";

                    master.Nodes.Remove(node);

                    master.Nodes.Insert(0, node);
                }
                else
                {
                    master.Nodes.Remove(node);
                    master.Nodes.Insert(i + 1, node);

                    ExtractSubFolders(master, node, node.Text);
                }

                c++;
            }

            for (int i = 0; i < nds.Count; i++)
            {
                TreeNode node = nds[i] as TreeNode;

                node.ImageKey = "Folder_6222";
                node.SelectedImageKey = "Folder_6222";

                master.Nodes.Remove(node);

                RemoveEmptyNodes(node);

                TreeNode nodes = FindNode(master, node.Text);

                if (nodes != null && node.Text == nodes.Text)
                {
                    if (nodes != null)
                    {
                        TreeNode ns = node;

                        if (ns != null)
                        {
                            ArrayList L = Nodes2Array(ns);

                            int N = L.Count;

                            int k = 0;
                            while (k < N)
                            {
                                TreeNode ng = L[k] as TreeNode;

                                node.Nodes.Remove(ng);

                                if (ng.Text != "")
                                    nodes.Nodes.Add(ng);

                                k++;
                            }
                        }

                        //master.Nodes.Remove(node);
                    }
                }
                else master.Nodes.Insert(c + 1, node);

                c++;

                //ExtractSubFolders(master, nds, nds.Text);
            }
        }

        public void RemoveEmptyNodes(TreeNode node)
        {
            if (node.Nodes.Count < 1)
                return;

            if (node.Nodes.Count > 1)
                return;
            TreeNode nodes = node.Nodes[0];

            if (nodes.Text == "")
                node.Nodes.Clear();
        }

        public ArrayList Nodes2Array(TreeNode node, string name)
        {
            ArrayList L = new ArrayList();

            if (node == null)
                return L;

            int i = 0;

            while (i < node.Nodes.Count)
            {
                if (node.Text == name)
                    L.Add(node.Nodes[i]);

                i++;
            }

            return L;
        }

        public ArrayList Nodes2Array(TreeNode node)
        {
            ArrayList L = new ArrayList();

            if (node == null)
                return L;

            int i = 0;

            while (i < node.Nodes.Count)
            {
                L.Add(node.Nodes[i]);

                i++;
            }

            return L;
        }

        public void ExtractSubFolders(TreeNode master, TreeNode node, string folder)
        {
            Dictionary<string, ArrayList> dict = new Dictionary<string, ArrayList>();

            ArrayList FD = new ArrayList();

            ArrayList PL = new ArrayList();

            foreach (TreeNode ns in node.Nodes)
            {
                string s = ns.Text;

                if (folder != "")
                    s = s.Replace(folder, "");
                if (s.StartsWith("\\"))
                    s = s.Remove(0, 1);
                //               s = s.Replace("- compile","");

                ns.Text = s;

                string[] cc = s.Split("\\".ToCharArray());

                if (cc.Count() >= 2)
                {
                    if (dict.ContainsKey(cc[0]))
                    {
                        ArrayList L = dict[cc[0]];

                        L.Add(ns);
                    }
                    else
                    {
                        PL = new ArrayList();
                        PL.Add(ns);
                        dict.Add(cc[0], PL);
                    }
                }
            }

            if (dict.Keys.Count < 1)
                return;

            int c = 0;

            foreach (string key in dict.Keys)
            {
                TreeNode nods = new TreeNode();
                nods.Text = key;
                nods.ImageKey = "Folder_6222";
                nods.SelectedImageKey = "Folder_6222";
                ArrayList L = dict[key];

                int i = 0;
                while (i < L.Count)
                {
                    TreeNode ng = L[i] as TreeNode;

                    //ng.ImageKey = "resource_32xMD";
                    //ng.SelectedImageKey = "resource_32xMD";

                    node.Nodes.Remove(ng);

                    if (ng.Text != "" && ng.Text != null)

                        nods.Nodes.Add(ng);

                    i++;
                }

                node.Nodes.Insert(c, nods);

                //if(key != folder)
                ExtractSubFolders(master, nods, key);

                c++;
            }
        }

        public static void SetTreeView(TreeView tv)
        {
            if (tv.ImageKey != "Folder_6222")
            {
                tv.ImageKey = "CSharpProject_SolutionExplorerNode_24";
                tv.SelectedImageKey = "CSharpProject_SolutionExplorerNode_24";
            }
            foreach (TreeNode node in tv.Nodes)
            {
                if (node.Nodes != null)
                    SetTreeNode(node, 1);
            }
        }

        public static void SetTreeNode(TreeNode node, int r)
        {
            if (r <= 2)
            {
                if (node.ImageKey != "Folder_6222")
                {
                    node.ImageKey = "CSharpProject_SolutionExplorerNode_24";
                    node.SelectedImageKey = "CSharpProject_SolutionExplorerNode_24";
                }
            }
            else if (node.ImageKey == "FolderOpen_7140_24")
            {
                node.ImageKey = "CSharpProject_SolutionExplorerNode_24";
                node.SelectedImageKey = "CSharpProject_SolutionExplorerNode_24";
            }

            foreach (TreeNode nodes in node.Nodes)
            {
                if (node.Nodes != null)
                    SetTreeNode(nodes, r + 1);
            }
        }
    }

    public class MySimpleLogger : Logger
    {
        public override void Initialize(Microsoft.Build.Framework.IEventSource eventSource)
        {
            //Register for the ProjectStarted, TargetStarted, and ProjectFinished events
            eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStarted);
            eventSource.TargetStarted += new TargetStartedEventHandler(eventSource_TargetStarted);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(eventSource_ProjectFinished);
        }

        private void eventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            Console.WriteLine("Project Finished: " + e.ProjectFile);
        }

        private void eventSource_ProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            Console.WriteLine("Project Started: " + e.ProjectFile);
        }

        private void eventSource_TargetStarted(object sender, TargetStartedEventArgs e)
        {
            if (Verbosity == LoggerVerbosity.Detailed)
            {
                Console.WriteLine("Target Started: " + e.TargetName);
            }
        }
    }

    //public class Solutions
    //{
    //    public Solution solution;

    //    public void dotest2(string f, string g)
    //    {
    //        //LoadSolution(f);
    //        //solution.FileName = f;//@"d:\projects\MyProject\MySolution.sln";

    //        //solution.UpdateMSBuildProperties();

    //        // ProjectPropertyInstance property = solution.MSBuildProjectCollection.GetGlobalProperty("SolutionDir");
    //        // string solutionDir = property.EvaluatedValue;

    //        //string expectedSolutionDir = g;//@"d:\projects\MyProject\";
    //        //Assert.AreEqual(expectedSolutionDir, solutionDir);
    //    }

    //    private void LoadSolution(string f)
    //    {
    //        //IProjectChangeWatcher fw = MockRepository.GenerateStub<IProjectChangeWatcher>();
    //        //solution = new Solution(fw);
    //        //solution = Solution.Load(f);
    //    }

    //    //[Test]
    //    //public void UpdateMSBuildProperties_SolutionHasFileName_SolutionDefinesSolutionDirMSBuildPropertyWithDirectoryEndingInForwardSlash()
    //    //{
    //    //    CreateSolution();
    //    //    solution.FileName = @"d:\projects\MyProject\MySolution.sln";

    //    //    solution.UpdateMSBuildProperties();

    //    //    ProjectPropertyInstance property = solution.MSBuildProjectCollection.GetGlobalProperty("SolutionDir");
    //    //    string solutionDir = property.EvaluatedValue;

    //    //    string expectedSolutionDir = @"d:\projects\MyProject\";
    //    //    Assert.AreEqual(expectedSolutionDir, solutionDir);
    //    //}
    //    //[Test]
    //    //public void dotest(string f, string g)
    //    //{
    //    //    CreateSolution();
    //    //    solution.FileName = f;//@"d:\projects\MyProject\MySolution.sln";

    //    //    solution.UpdateMSBuildProperties();

    //    //    ProjectPropertyInstance property = solution.MSBuildProjectCollection.GetGlobalProperty("SolutionDir");
    //    //    string solutionDir = property.EvaluatedValue;

    //    //    string expectedSolutionDir = g;//@"d:\projects\MyProject\";
    //    //    Assert.AreEqual(expectedSolutionDir, solutionDir);
    //    //}
    //}

    ////public class Compiler
    ////{
    ////    private static string locationOfMSBuilldEXE = "";
    ////    public static void Build(string msbuildFileName)
    ////    {
    ////        ConsoleLogger logger = new ConsoleLogger(LoggerVerbosity.Normal);
    ////        BuildManager manager = BuildManager.DefaultBuildManager;

    ////        ProjectInstance projectInstance = new ProjectInstance(msbuildFileName);
    ////        var result = manager.Build(
    ////            new BuildParameters()
    ////            {
    ////                DetailedSummary = true,
    ////                Loggers = new List<ILogger>() { logger }
    ////            },
    ////            new BuildRequestData(projectInstance, new string[] { "Build" }));
    ////        var buildResult = result.ResultsByTarget["Build"];
    ////        var buildResultItems = buildResult.Items;

    ////        string s = "";
    ////    }
    ////}

    public enum TypeCode
    {
        SByte,
        Int16,
        Int32,
        Int64,
        Byte,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        IntPtr,
        UIntPtr,
        Void,
        Boolean,
        Char,
        String,
        TypedReference,
        ValueType,
        NotModulePrimitive,
        @bool,
        @byte,
        @sbyte,
        @char,
        @string,
        @decimal,
        @double,
        @float,
        @int,
        @uint,
        @long,
        @ulong,
        @ushort
    }

    public class TypeBuilder
    {
        public VSSolution vs { get; set; }

        public Dictionary<string, string> dc { get; set; }

        public void AddBuiltInTypes()
        {
            string[] cc = { "System.Object", "System.MarshalByRefObject" };

            foreach (string cs in cc)
            {
                if (maps.ContainsKey(cs) == false)
                {
                    Type obs = Type.GetType(cs);

                    if (obs != null)
                    {
                        ClassMapper m = new ClassMapper();
                        m.classname = cs;

                        if (obs.BaseType != null)
                        {
                            m.baseclasses = new ArrayList();
                            m.baseclasses.Add(obs.BaseType.FullName);
                        }

                        MethodInfo[] bb = obs.GetMethods();

                        foreach (MethodInfo b in bb)
                        {
                            if (m.methods == null)
                                m.methods = new ArrayList();
                            MethodCreator mc = new MethodCreator();
                            mc.name = b.Name;
                            mc.SetInnerName();
                            m.methods.Add(mc);
                        }

                        maps.Add(cs, m);

                        dict.Add(cs, cs);
                    }
                }
            }
            var values = Enum.GetValues(typeof(TypeCode)).Cast<TypeCode>();

            foreach (var val in values)
            {
                if (dict.ContainsKey(val.ToString()) == true)
                    return;

                dict.Add(val.ToString(), val.ToString());
            }

            dict.Add("System.Int16", "System.Int16");
            dict.Add("System.Int32", "System.Int32");
            dict.Add("System.Int64", "System.Int64");
            dict.Add("System.UInt16", "System.UInt16");
            dict.Add("System.UInt32", "System.UInt32");
            dict.Add("System.UInt64", "System.UInt64");
            dict.Add("System.Boolean", "System.Boolean");
        }

        public TreeView LoadSolution(VSSolution _vs)
        {
            vs = _vs;

            TreeView tt = new TreeView();

            foreach (VSProject p in vs.Projects)
            {
                TreeNode node = new TreeNode();
                node.Text = p.Name;
                node.Tag = p;

                if (dc != null)
                {
                    string exts = Path.GetExtension(p.FileName);

                    if (dc.ContainsKey(exts))
                    {
                        string dd = (string)dc[exts];
                        if (dd != "")
                        {
                            node.ImageKey = dd;
                            node.SelectedImageKey = dd;
                        }
                    }
                }

                //cc.Nodes.Add(node);
                tt.Nodes.Add(node);

                AddProjectReferences(p, node);

                //AddProjectTypes(p, node);
            }

            return tt;
        }

        public TreeView LoadRefTypes(VSProject p)
        {
            TreeView tt = new TreeView();

            // AddProjectReferences(p, node);

            //     AddProjectTypes(p, node);

            return tt;
        }

        public TreeNode AddProjectReferences(VSProject p, TreeNode node)
        {
            TreeNode nodes = new TreeNode();
            nodes.Text = "ProjectReferences";

            nodes.ImageKey = "Folder_6222";
            nodes.SelectedImageKey = "Folder_6222";

            ArrayList L = p.GetReferences("Reference");

            foreach (string s in L)
            {
                TreeNode ns = new TreeNode();
                ns.Text = s;
                nodes.Nodes.Add(ns);
                ns.ImageKey = "reference_16xLG";
                ns.SelectedImageKey = "reference_16xLG";
            }
            TreeNode nn = new TreeNode();
            nn.Text = "mscorlib";
            nodes.Nodes.Add(nn);
            nn.ImageKey = "reference_16xLG";
            nn.SelectedImageKey = "reference_16xLG";

            node.Nodes.Add(nodes);

            return nodes;
        }

        //public void AddProjectTypes(VSProject p, TreeNode node)
        //{
        //    string file = p.GetProjectExec();

        //    TreeNode nodes = TreeNodeBuilder.BuildUnitNodes(file);

        //    TreeNode ng = RebuildNode(nodes, file);

        //    node.Nodes.Add(ng);

        //    //node.Nodes.Add(nodes);
        //}

        //public ArrayList mappers { get; set; }

        //public void AddTypes(TreeNode node, string file)
        //{
        //    TreeNode nodes = TreeNodeBuilder.BuildUnitNodes(file);

        //    TreeNode ng = RebuildNode(nodes, file);

        //    node.Nodes.Add(ng);

        //    if (mappers == null)
        //        mappers = new ArrayList();

        //    mappers.Add(ng.Tag);

        //    //node.Nodes.Add(nodes);
        //}

        public Dictionary<string, string> dict { get; set; }

        public Dictionary<string, string> dlls { get; set; }

        public Dictionary<string, ClassMapper> maps { get; set; }

        private Object _thisLock = new Object();

        //public TreeNode RebuildNode(TreeNode nodes, string file)
        //{
        //    TreeNode ns = new TreeNode();
        //    ns.Text = nodes.Text;
        //    ns.ImageKey = "Class_489_24";
        //    ns.SelectedImageKey = "Class_489_24";

        //    if (nodes.Nodes.Count <= 0)
        //        return ns;

        //    TreeNode ng = nodes.Nodes[0];

        //    ArrayList mappers = new ArrayList();

        //    ns.Tag = mappers;

        //    foreach (TreeNode b in ng.Nodes)
        //    {
        //        if (b.Text == "")
        //            continue;

        //        ns.Nodes.Add(b);

        //        ClassMapper mapper = new ClassMapper();

        //        mapper.filename = file;

        //        mapper.classname = b.Text;

        //        mappers.Add(mapper);

        //        mapper.mappers = new ArrayList();

        //        foreach (TreeNode g in b.Nodes)
        //        {
        //            AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> bb = g.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

        //            string s = bb.GetTypeName();

        //            string n = bb.GetTypeFullName();

        //            ClassMapper m = new ClassMapper();

        //            mapper.mappers.Add(m);

        //            m.classname = n;

        //            m.baseclasses = bb.B;

        //            if (maps.ContainsKey(n) == false)
        //                maps.Add(n, m);

        //            lock (thisLock)
        //            {
        //                if (dict.ContainsKey(s) == false)
        //                    dict.Add(s, n);

        //                if (dlls.ContainsKey(n) == false)
        //                    dlls.Add(n, file);
        //            }

        //            foreach (TreeNode ne in g.Nodes)
        //            {
        //                AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> bg = ne.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

        //                var obs = ne.Tag;

        //                AndersLiu.Reflector.Program.UI.AssemblyTreeNode.FieldNodeTag<TreeNode> dd = obs as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.FieldNodeTag<TreeNode>;

        //                if (dd != null)
        //                {
        //                    if (m.fields == null)
        //                        m.fields = new ArrayList();

        //                    FieldCreator me = new FieldCreator();

        //                    me.name = dd.GetName();

        //                    me.SetInnerName();

        //                    me.typename = dd.GetTypeName();

        //                    m.fields.Add(me);
        //                }

        //                AndersLiu.Reflector.Program.UI.AssemblyTreeNode.MethodNodeTag<TreeNode> dp = obs as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.MethodNodeTag<TreeNode>;

        //                if (dp != null)
        //                {
        //                    if (m.methods == null)
        //                        m.methods = new ArrayList();

        //                    MethodCreator me = new MethodCreator();

        //                    me.name = dp.GetName();

        //                    me.SetInnerName();

        //                    m.methods.Add(me);
        //                }

        //                AndersLiu.Reflector.Program.UI.AssemblyTreeNode.PropertyNodeTag<TreeNode> pp = obs as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.PropertyNodeTag<TreeNode>;

        //                if (pp != null)
        //                {
        //                    if (m.properties == null)
        //                        m.properties = new ArrayList();

        //                    PropertyCreator me = new PropertyCreator();

        //                    me.name = pp.GetName();

        //                    me.typename = pp.GetTypeName();

        //                    me.SetInnerName();

        //                    m.properties.Add(me);
        //                }
        //            }
        //        }
        //    }

        //    return ns;
        //}

        //public void GetBaseTypes(TreeNode node)
        //{
        //    AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode> T = node.Tag as AndersLiu.Reflector.Program.UI.AssemblyTreeNode.TypeNodeTag<TreeNode>;

        //    if (T.B == null)
        //        return;

        //    if (T.B.Count <= 0)
        //        return;

        //    foreach (string s in T.B)
        //    {
        //        TreeNode nodes = new TreeNode();
        //        nodes.Text = s;
        //        nodes.ImageKey = "Class_489_24";
        //        nodes.SelectedImageKey = "Class_489_24";
        //        node.Nodes.Add(nodes);
        //    }
        //}

        public string FindBaseType(string b)
        {
            string bs = "";

            if (dict == null)
                return "";

            if (dict.ContainsKey(b) == false)
                return "";

            string bb = dict[b];

            if (dlls.ContainsKey(bb) == true)
            {
                bs = dlls[bb];
            }

            return bs;
        }

        public void GetProjectReference(VSProject p, string asm, TreeNode node)
        {
            if (p == null)
                return;

            Dictionary<string, string> d = GACForm.dicts;

            string file = Path.GetDirectoryName(p.FileName);

            if (d.ContainsKey(asm))
            {
                file = d[asm];
            }
            else
            {
                string hint = p.GetReferenceHint(asm);

                if (hint == "")
                    return;

                file = file + "\\" + hint;
            }

            //AddTypes(node, file);
        }

        public void GetProjectReference(string asm, TreeNode node)
        {
            string file = asm;
            // AddTypes(node, file);
        }

        public void GetReference(string asm, TreeNode node)
        {
            string file = "";

            Dictionary<string, string> d = GACForm.dicts;

            asm = "mscorlib";

            //if (d.ContainsKey(asm))
            //{
            //    file = d[asm];

            //}

            //Assembly assem = Assembly.LoadFrom(file);
            //System.Type []types = assem.GetTypes();

            if (d.ContainsKey(asm))
            {
                file = d[asm];
            }
            else return;

            //AddTypes(node, file);
        }
    }

    public class cmds
    {
        [DllImport("kernel32.dll")]
        private static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

        public delegate void CmdEvent(object sender, object param);

        public event CmdEvent OnCmdEvent;

        public cmds()
        {
            this.OnCmdEvent += new CmdEvent(Foo_OnMyEvent);
        }

        private void Foo_OnMyEvent(object sender, object param)
        {
            if (this.OnCmdEvent != null)
            {
                //do something
            }
        }

        public void RaiseEvent(string file)
        {
            object param = file;

            this.OnCmdEvent(this, param);
        }

        static public void start(string file)
        {
            Process ExternalProcess = new Process();
            ExternalProcess.StartInfo.FileName = file;
            ExternalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            ExternalProcess.Start();
            ExternalProcess.WaitForExit();
        }

        static public RichTextBox rb { get; set; }

        private static void p_Exited(object sender, EventArgs e)
        {
            Process p = sender as Process;
            if (p != null)
            {
                if (rb != null)
                    rb.Invoke(new Action(() => { rb.AppendText("\n > Exited with code " + p.ExitCode); }));
            }
            else
               if (rb != null)
                rb.Invoke(new Action(() => { rb.AppendText("\n > Exited "); }));
        }

        static public async void RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);
            //if (!string.IsNullOrEmpty("/c " + arguments))
            {
                process.StartInfo.Arguments = "/c " + arguments;
            }

            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.EnableRaisingEvents = true;
            process.Exited += p_Exited;

            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            {
                //Console.WriteLine(e.Data);
                stdOutput.Append(args.Data);
                if (rb != null)
                    rb.AppendText(args.Data);
            };
            string stdError = null;
            try
            {
                process.Start();

                //stdOutput .Append( await process.StandardOutput.ReadToEndAsync());
                //if (rb != null)
                //    rb.AppendText(stdOutput.ToString());
                //stdError = await process.StandardError.ReadToEndAsync();
                StreamReader reader = process.StandardOutput;

                StreamReader error = process.StandardError;

                //string output = await reader.ReadToEndAsync();
                //if (rb != null)
                //    rb.AppendText(output);

                //string error = p.StandardError.ReadToEnd();
                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    process.WaitForExit();
                });

                ProcessModuleCollection pmc = process.Modules;
                foreach (ProcessModule pm in pmc)
                    if (rb != null)
                        rb.AppendText("\n" + ">  " + Path.GetFileName(filename) + " : Loaded  " + pm.ModuleName + "   " + pm.FileName);

                if (rb != null)
                    rb.AppendText("\n");

                ProcessThreadCollection currentThreads = process.Threads;

                foreach (ProcessThread thread in currentThreads)
                {
                    if (rb != null)
                        rb.AppendText("\n" + ">  Thread - " + thread.Id);
                }

                string output = reader.ReadToEnd();
                if (rb != null)
                    rb.AppendText(output);

                if (rb != null)
                    rb.AppendText("\n");

                string errors = error.ReadToEnd();
                if (rb != null)
                    rb.AppendText(errors);

                if (rb != null)
                    rb.AppendText("\n");

                foreach (ProcessThread thread in currentThreads)
                {
                    uint ec;
                    GetExitCodeThread(new IntPtr(thread.Id), out ec);

                    if (rb != null)
                        rb.AppendText("\n" + "The thread  " + "0x" + thread.Id.ToString("X").ToLower() + " has exited with code " + ec + "  (0x" + ec.ToString("X").ToLower() + ").");
                }
            }
            catch (Exception e)
            {
                //throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);

                return;
            }

            if (process.ExitCode == 0)
            {
                return;
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                // throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);

                return;
            }
        }

        static public async void RunExternalExes(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filename);

            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            //process.EnableRaisingEvents = true;
            //process.Exited += p_Exited;

            //var stdOutput = new StringBuilder();
            //process.OutputDataReceived += (sender, args) =>
            //{
            //    //Console.WriteLine(e.Data);
            //    stdOutput.Append(args.Data);
            //    if (rb != null)
            //        rb.AppendText(args.Data);
            //};

            string stdError = null;
            try
            {
                process.Start();

                //stdOutput .Append( await process.StandardOutput.ReadToEndAsync());
                //if (rb != null)
                //    rb.AppendText(stdOutput.ToString());
                //stdError = await process.StandardError.ReadToEndAsync();
                StreamReader reader = process.StandardOutput;

                StreamReader error = process.StandardError;

                //string output = await reader.ReadToEndAsync();
                //if (rb != null)
                //    rb.AppendText(output);

                //string error = p.StandardError.ReadToEnd();
                //    await System.Threading.Tasks.Task.Factory.StartNew(() =>
                //   {
                process.WaitForExit();
                //    });

                string output = reader.ReadToEnd();
                if (rb != null)
                    rb.AppendText(output);

                if (rb != null)
                    rb.AppendText("\n");

                string errors = error.ReadToEnd();
                if (rb != null)
                    rb.AppendText(errors);

                if (rb != null)
                    rb.AppendText("\n");
            }
            catch (Exception e)
            {
                //throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);

                return;
            }
        }

        static public string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }
    }
}
//public static class BuildEngineExtensions
//{
//    const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;

//    public static IEnumerable GetEnvironmentVariable(this IBuildEngine buildEngine, string key, bool throwIfNotFound)
//    {
//        var projectInstance = GetProjectInstance(buildEngine);

//        var items = projectInstance.Items
//            .Where(x => string.Equals(x.ItemType, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
//        if (items.Count > 0)
//        {
//            return items.Select(x => x.EvaluatedInclude);
//        }


//        var properties = projectInstance.Properties
//            .Where(x => string.Equals(x.Name, key, StringComparison.InvariantCultureIgnoreCase)).ToList();
//        if (properties.Count > 0)
//        {
//            return properties.Select(x => x.EvaluatedValue);
//        }

//        if (throwIfNotFound)
//        {
//            throw new Exception(string.Format("Could not extract from '{0}' environmental variables.", key));
//        }

//        return Enumerable.Empty();
//    }

//    static ProjectInstance GetProjectInstance(IBuildEngine buildEngine)
//    {
//        var buildEngineType = buildEngine.GetType();
//        var targetBuilderCallbackField = buildEngineType.GetField("targetBuilderCallback", bindingFlags);
//        if (targetBuilderCallbackField == null)
//        {
//            throw new Exception("Could not extract targetBuilderCallback from " + buildEngineType.FullName);
//        }
//        var targetBuilderCallback = targetBuilderCallbackField.GetValue(buildEngine);
//        var targetCallbackType = targetBuilderCallback.GetType();
//        var projectInstanceField = targetCallbackType.GetField("projectInstance", bindingFlags);
//        if (projectInstanceField == null)
//        {
//            throw new Exception("Could not extract projectInstance from " + targetCallbackType.FullName);
//        }
//        return (ProjectInstance)projectInstanceField.GetValue(targetBuilderCallback);
//    }
//}

//public override bool Execute()
//{
//    string projectFile = BuildEngine.ProjectFileOfTaskNode;

//    Engine buildEngine = new Engine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory());

//    Project project = new Project(buildEngine);
//    project.Load(projectFile);
//    foreach (var o in project.EvaluatedProperties)
//    {
//        // Use properties
//    }

//    // Do what you want

//    return true;
//}

