using ICSharpCode.NRefactory.CSharp;

//using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Newtonsoft.Json;

//using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VSProvider;

namespace SE_Project
{
    public class BasicFileLogger : Logger
    {
        private int _indent;

        private StreamWriter _streamWriter;

        public ArrayList E = new ArrayList();

        public ArrayList W = new ArrayList();

        public ArrayList Es = new ArrayList();

        public ArrayList Ws = new ArrayList();

        public ArrayList Me = new ArrayList();

        private RichTextBox rb { get; set; }

        private bool _running = false;

        public BasicFileLogger()
        {
            // Verbosity = LoggerVerbosity.Diagnostic;
        }

        public override void Initialize(IEventSource eventSource)
        {
        }

        /// <summary>
        /// Initialize is guaranteed to be called by MSBuild at the start of the build
        /// before any events are raised.
        /// </summary>
        //public override void Initialize(IEventSource eventSource)
        //{
        //        eventSource.ProjectStarted += new ProjectStartedEventHandler(eventSource_ProjectStarted);
        //        eventSource.TaskStarted += new TaskStartedEventHandler(eventSource_TaskStarted);
        //        // eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);
        //        eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);
        //        eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningRaised);
        //        eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorRaised);
        //        eventSource.ProjectFinished += new ProjectFinishedEventHandler(eventSource_ProjectFinished);
        //        //eventSource.AnyEventRaised += EventSource_AnyEventRaised;
        //        _running = true;

        //    rb = VSProvider.cmds.rb;

        //    Verbosity = LoggerVerbosity.Detailed;
        //}

        ///// <summary>
        ///// Shutdown() is guaranteed to be called by MSBuild at the end of the build, after all
        ///// events have been raised.
        ///// </summary>
        //public override void Shutdown()
        //{
        //}

        public void Append(string s)
        {
            rb.Invoke(new Action(() => { rb.AppendText("\n" + s); }));
        }

        private void eventSource_ErrorRaised(object sender, Microsoft.Build.Framework.BuildErrorEventArgs e)
        {
            Es.Add(e);

            if (rb == null)
                return;
            string s = " > " + "Error in Build of project " + e.ProjectFile + "  " + e.Message;
            //rb.AppendText("\n" + s);
            Append("\n" + s);
        }

        private void eventSource_MessageRaised(object sender, Microsoft.Build.Framework.BuildMessageEventArgs e)
        {
            if (e.LineNumber != 0)
                Me.Add(e);
        }

        private void eventSource_ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            if (rb == null)
                return;
            string s = " > " + "Build of project " + e.ProjectFile + " finished";
            //rb.AppendText("\n" + s);
            Append("\n" + s);
        }

        private void eventSource_ProjectStarted(object sender, Microsoft.Build.Framework.ProjectStartedEventArgs e)
        {
            if (rb == null)
                return;
            string s = "1>------- " + " Build started: Project : " + Path.GetFileNameWithoutExtension(e.ProjectFile);
            //rb.AppendText("\n" + s);
            Append("\n" + s);
            s = "1>Build started " + e.Timestamp;
            //rb.AppendText("\n" + s);
            Append("\n" + s);
        }

        private void eventSource_TaskStarted(object sender, TaskStartedEventArgs e)
        {
            //if (rb == null)
            //    return;
            //string s = " > " + "Build task  " + e.TaskName + " started";
            //rb.AppendText("\n" + s);
        }

        private void eventSource_WarningRaised(object sender, Microsoft.Build.Framework.BuildWarningEventArgs e)
        {
            Ws.Add(e);

            if (rb == null)
                return;
            string s = " > " + "Warning in Build of project " + e.ProjectFile + "  " + e.Message;
            //rb.AppendText("\n" + s);
            Append("\n" + s);
        }
    }

    public class msbuilder_alls
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

        static public BasicFileLogger build_alls(string path, bool rebuild)
        {
            string b = AppDomain.CurrentDomain.BaseDirectory;

            BasicFileLogger logger = new BasicFileLogger();

            // logger.Parameters = b + "\\" + "logger.txt";

            VSProvider.VSSolution vs = null;

            try
            {
                vs = new VSSolution(Path.GetFullPath(path));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

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

                    ArrayList bc = ps.GetCompileItems();

                    Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(ps.FileName);

                    pc.MarkDirty();

                    DateTime d = File.GetLastWriteTime(bc[0] as string);

                    if (rebuild)
                    {
                        File.SetLastWriteTime(bc[0] as string, DateTime.Now);
                    }

                    //pc.Build(logger);

                    if (rebuild)
                        File.SetLastWriteTime(bc[0] as string, d);

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

            // logger.Parameters = b + "\\" + "logger.txt";

            //VSProvider.VSSolution vs = new VSSolution(Path.GetFullPath(path));
            //foreach (VSProject ps in vs.Projects)
            {
                try
                {
                    //if (ps.projecttype == "solutionfolder")
                    //    continue;
                    Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(path);

                    pc.MarkDirty();

                    //pc.Build(logger);

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

        //msbuild.exe YourMSBuildProject.csproj /t:Rebuild /l:TcpIpLogger, C:\Loggers.dll,; Ip=”127.0.0.1″;Port=1200″;

        //    static void TCPClient()
        //{
        //    TcpClient clientSocketWriter = new System.Net.Sockets.TcpClient();
        //    clientSocketWriter.Connect(ipServer, port);
        //    networkStream = clientSocketWriter.GetStream();

        //}

        public class Server
        {
            private static readonly TcpListener listener = new TcpListener(IPAddress.Any, 6787);

            static public bool running = false;

            
            public Server()
            {
                //this.ef = efs;
                listener.Start();
                Console.WriteLine("Started MSBuild TCP server.");

                running = true;

                while (running)
                {
                    Console.WriteLine("Waiting for connection...");

                    var client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // each connection has its own thread
                    new Thread(ServeData).Start(client);
                }
            }

            private void ServeData(object clientSocket)
            {
                Console.WriteLine("Started msbuild thread  " + Thread.CurrentThread.ManagedThreadId);

                

               

                running = true;

                var rnd = new Random();
                try
                {
                    var client = (TcpClient)clientSocket;
                    var stream = client.GetStream();
                    var msg = new byte[1000000];
                    int i = 0;
                    ArrayList es = new ArrayList();
                    ArrayList ws = new ArrayList();
                    ArrayList me = new ArrayList();

                    string prev = null;

                    while (running)
                    {
                        
                        {
                           

                            int c = stream.Read(msg, 0, msg.Length);

                            string s = Encoding.ASCII.GetString(msg, 0, c);

                            s = s.Replace("\r", "");

                            string[] cc = s.Trim().Split("\n".ToCharArray());

                            if (prev != null)
                            {
                                cc[0] = prev + cc[0];
                                prev = null;
                            }

                            if (s.EndsWith("\n") == false)
                            {
                                prev = cc[cc.Length - 1];
                                cc[cc.Length - 1] = null;
                            }
                            else prev = null;

                            //Console.WriteLine("MSBuild data received: " + s);

                            foreach (string b in cc)
                            {
                                if (b == null)
                                    continue;
                                if (b.StartsWith("$") == false)
                                    continue;
                                //return string.Format("{0}:{1}:{2}:{3}:{4}:{5}$$", e.HelpKeyword, e.Message, e.File, e.ColumnNumber, e.LineNumber, e.ProjectFile);
                                if (b[2] == '4' || b[2] == '5')
                                {
                                    string[] dd = b.Trim().Replace("\r", "").Split("$".ToCharArray());

                                    if (dd.Length < 6)
                                        continue;

                                    //string[] db = dd;

                                    string[] gg = new string[dd.Length];
                                    int p = 0;
                                    foreach (string w in dd)
                                    {
                                        if (p == 0)
                                            if (w == "")
                                                continue;

                                        gg[p++] = w;
                                    }

                                    dd = gg;

                                    if (b[2] == '4')
                                    {
                                        BuildErrorEventArgs be = new BuildErrorEventArgs("", dd[0], dd[3], Convert.ToInt32(dd[5]), Convert.ToInt32(dd[4]), Convert.ToInt32(dd[5]), Convert.ToInt32(dd[4]), dd[2], dd[1], "");
                                        be.ProjectFile = dd[6];
                                        es.Add(be);
                                    }
                                    else
                                    {
                                        BuildWarningEventArgs be = new BuildWarningEventArgs("", dd[0], dd[3], Convert.ToInt32(dd[5]), Convert.ToInt32(dd[4]), Convert.ToInt32(dd[5]), Convert.ToInt32(dd[4]), dd[2], dd[1], "");
                                        be.ProjectFile = dd[6];
                                        ws.Add(be);
                                    }

                                    i++;
                                }
                                //else if (b[2] == '0' || b[2] == '1')
                                //    Console.WriteLine(b);
                                //Console.WriteLine(b);
                                else

                               

                                if (i > 10)
                                {
                                    
                                    es.Clear();
                                    ws.Clear();
                                    me.Clear();

                                    i = 0;
                                }
                            }
                        }
                        //if (i > 10)
                        {
                            
                            es.Clear();
                            ws.Clear();
                            me.Clear();
                        }

                        // wait until the next update - I made the wait time so small 'cause I was bored :)
                        //Thread.Sleep(new TimeSpan(0, 0, rnd.Next(1, 5)));
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Socket exception in thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e);
                }
            }

            public void Close()
            {
                listener.Stop();
            }
        }

        public enum Build
        {
            rebuild,
            build,
            clean,
            none
        }

        public class MSBuild
        {
            public string msbuildPath { get; set; }

            public string IP { get; set; }
            public string Port { get; set; }
            public string LoggerClass { get; set; }
            public string LoggerDLL { get; set; }
            public string LoggerParameters { get; set; }
            public string Platform { get; set; }
            public string Configuration { get; set; }

            public string MSBuildFile { get; set; }

            public Build build = Build.none;

            public string GetMSBuild()
            {
                //    string additionalArguments = String.Concat(msbuildProjPath, " ", additionalArgs, " ",
                //"/fileLoggerParameters:LogFile=MsBuildLog.txt;append=true;Verbosity=normal;Encoding=UTF-8 /l:YourDll.MsBuildLogger.TcpIpLogger,",
                //currentAssemblyFullpath, ";Ip=", "ipAddressSettings.GetIPAddress()", ";Port=", "ipAddressSettings.Port", ";");

                string s = "\"" + MSBuildFile + "\"";

                //string s = " /t:Rebuild ";// MSBuildFile;

                if (build == Build.rebuild)
                    s += " /t:Rebuild ";
                else if (build == Build.build)
                    s += " /t:Build ";
                else if (build == Build.clean)
                    s += " /t:Clean ";
                else s += " /t:Rebuild ";

                // MSBuild.exe sample.sln / t:Clean / p:Configuration = Release; Platform = Any CPU

                string p = "";

                if (Platform != null)
                    p += " /p:Configuration=" + "\"" + Platform + "\"";

                s += p;

                if (Configuration != null)
                    //                    p = " /p:Platform=" + "\"Any CPU\"";// Configuration;
                    p = " /p:Platform=" + "\"" + Configuration + "\"";
                if (MSBuildFile.EndsWith("sln") == false)
                    if (Configuration == "Any CPU")
                        p = " /p:Platform=" + "\"AnyCPU\"";// Configuration;

                s += p;

                s += " " + LoggerParameters;
                s += " " + "/logger:" + "\"" + LoggerDLL + "\"";
                //s += "," + LoggerDLL;
                s += ";Ip=" + IP;
                //s += ";Port=" + Port + ";";
                s += ";Port=" + Port + " ";
                //s += " " + MSBuildFile;

                return s;
            }
        }

        private static string MSBUILD_PATH { get; set; }

        static public string BuildMsBuildAdditionalArguments(string msbuildProjPath, string ipAddressSettings,
            string additionalArgs, string currentAssemblyFullpath)
        {
            string additionalArguments = String.Concat(msbuildProjPath, " ", additionalArgs, " ",
            "/fileLoggerParameters:LogFile=MsBuildLog.txt;append=true;Verbosity=normal;Encoding=UTF-8 /l:YourDll.MsBuildLogger.TcpIpLogger,",
            currentAssemblyFullpath, ";Ip=", "ipAddressSettings.GetIPAddress()", ";Port=", "ipAddressSettings.Port", ";");
            return additionalArguments;
        }

        static public Process ExecuteMsbuildProject(string msbuildProjPath, string ipAddressSettings, string additionalArgs = "")
        {
            string currentAssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string currentAssemblyFullpath = String.Format(currentAssemblyLocation, "YourDll.dll");
            string additionalArguments = "";
            //(msbuildProjPath, ipAddressSettings, additionalArgs, currentAssemblyFullpath);

            ProcessStartInfo procStartInfo = new ProcessStartInfo(MSBUILD_PATH, additionalArguments);
            procStartInfo.RedirectStandardOutput = false;
            //procStartInfo.UseShellExecute = true;
            //procStartInfo.CreateNoWindow = false;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            return proc;
        }

        static public Process ExecuteMsbuildProject(MSBuild msbuild)
        {
            string args = msbuild.GetMSBuild();

            ProcessStartInfo procStartInfo = new ProcessStartInfo(msbuild.msbuildPath, args);
            procStartInfo.RedirectStandardOutput = false;
            //procStartInfo.UseShellExecute = true;
            //procStartInfo.CreateNoWindow = false;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.RedirectStandardError = true;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            proc.WaitForExit();

            

            string stderr = proc.StandardError.ReadToEnd();

            Console.WriteLine(stderr);

            msbuilder_alls.Server.running = false;

            return proc;
        }
        public class MSTest
        {
            public string msTestPath { get; set; }

            public string IP { get; set; }
            public string Port { get; set; }
            public string LoggerClass { get; set; }
            public string LoggerDLL { get; set; }

            public string MSTestFile { get; set; }

            public string file_library { get; set; }

            public string folder_library { get; set; }

            public void SetTestLibrary(string file)
            {
                string file_tested = file;

                if (!File.Exists(msTestPath))
                    return;
                string content = File.ReadAllText(msTestPath);

                
                file_library = Path.GetFileName(file);

                folder_library = Path.GetDirectoryName(file);

                file = msTestPath;

                string filename = Path.GetFileName(file);

                file = file.Replace(filename, filename.Remove(0, 1));
                
                msTestPath = file;

                file_library = Path.GetFileName(file);

                content = content.Replace("{####}", folder_library);

                content = content.Replace("{#####}", file_tested);

                File.WriteAllText(msTestPath, content);
            }
            
            public string GetMSTest()
            {
                return "";
            }
            
            public List<string> GetDiscoveredTests()
            {
                List<string> b = new System.Collections.Generic.List<string>();

                //string folder_library = AppDomain.CurrentDomain.BaseDirectory + "\\Extensions";

                if (!File.Exists(folder_library + "\\" + "results.vs"))
                    return b;

                string c = File.ReadAllText(folder_library + "\\" + "results.vs");

                string[] cc = c.Split("\n".ToCharArray());

                bool found = false;

                foreach (string d in cc)
                {
                    
                    if (d.StartsWith("The following Tests are available"))
                    {
                        found = true;
                        continue;
                    }
                    if (found)
                        if (!string.IsNullOrEmpty(d))
                    b.Add(d);

                }
                return b;
            }
        }
        static public Process ExecuteMsTests(MSTest mstest)
        {
            string args = mstest.GetMSTest();

            ProcessStartInfo procStartInfo = new ProcessStartInfo(mstest.msTestPath, args);
            procStartInfo.RedirectStandardOutput = false;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = false;
            procStartInfo.RedirectStandardError = true;
            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            proc.WaitForExit();

           

            string stderr = proc.StandardError.ReadToEnd();

            Console.WriteLine(stderr);

            msbuilder_alls.Server.running = false;

            return proc;
        }
        static public void checkprojectreferences(string path, string projectname)
        {
            //         VSProvider.VSSolution vs = new VSSolution(Path.GetFullPath(path));

            //Microsoft.Build.Evaluation.Project project = (Microsoft.Build.Evaluation.Project)vs.Projects.Single(p => string.Equals(p.Name, projectname, StringComparison.OrdinalIgnoreCase));

            path = Path.GetDirectoryName(path);

            path = Path.GetFullPath(path);

            ICSharpCode.NRefactory.CSharp.CSharpParser parse = new ICSharpCode.NRefactory.CSharp.CSharpParser();

            string[] tree = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            IEnumerable<ICSharpCode.NRefactory.CSharp.SyntaxTree> s = tree.Select(f => parse.Parse(File.ReadAllText(f), f));

            IEnumerable<ICSharpCode.NRefactory.CSharp.TypeSystem.CSharpUnresolvedFile> gf = tree.Select(f => parse.Parse(File.ReadAllText(f), f).ToTypeSystem());

            ICSharpCode.NRefactory.TypeSystem.IProjectContent pc = new ICSharpCode.NRefactory.CSharp.CSharpProjectContent();

            pc = pc.AddOrUpdateFiles(gf);

            ICSharpCode.NRefactory.TypeSystem.ICompilation cp = pc.CreateCompilation();

            astresolver(cp, s, gf);
        }

        static public IEnumerable<ICSharpCode.NRefactory.CSharp.Attribute> GetAttributes(TypeDeclaration typeDeclaration)
        {
            return typeDeclaration.Members
                .SelectMany(member => member
                    .Attributes
                    .SelectMany(attr => attr.Attributes));
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
            ProjectItemInfo p = new ProjectItemInfo();
            p.vs = msv.Tag as VSSolution;
            p.SubType = "SolutionNode";
            gs.Tag = p;

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

                if (v.ItemType == "_ExplicitReference")
                    continue;
                if (v.ItemType == "PropertyPageSchema")
                    continue;
                if (v.ItemType == "AppxManifestMetadata")
                    continue;
                if (v.ItemType == "DesktopDebuggerPages")
                    continue;
                if (v.ItemType == "AppHostDebuggerPages")
                    continue;
                if (v.ItemType == "DebuggerPages")
                    continue;

                v.fileName = g + "\\" + b.EvaluatedInclude;

                if (b.HasMetadata("SubType") == true)
                    v.SubType = b.GetMetadataValue("SubType");

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

            vs.solutionFileName = path;

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


        public static string GetDependentResource(string data, string resource, string depends)
        {
            string[] cc = data.Split("\\".ToCharArray());

            //if (!depends.EndsWith(".resx"))
            //{

            //    string d = data.Replace(cc[cc.Length - 1], "");

            //    return d + resource;
            //}
            //else
            {
                string d = data.Replace(cc[cc.Length - 1], "");

                return d + depends;
            }
        }

        public static int projects = 0;

        public ProgressBar pgs { get; set; }

        private ArrayList FD { get; set; }

        public Dictionary<string, string> dc { get; set; }

        public ArrayList PD { get; set; }

        public Dictionary<string, TreeNode> DD { get; set; }

        public VSSolution tester(TreeView tv, string paths, string thisproject)
        {
            PL = new ArrayList();

            PD = new ArrayList();

            DD = new Dictionary<string, TreeNode>();

            string path = Path.GetFullPath(paths);

            VSProvider.VSSolution vs = null;

            var solution = VSProvider.SolutionParser.Parse(paths);

            projects = 0;

            try
            {
                vs = new VSSolution(Path.GetFullPath(path));
            }
            catch (Exception e) { }

            if (vs == null)
            {
                vs = LoadProjects(solution, path);
            }

            Dictionary<string, object> d = new Dictionary<string, object>();

            tv.Tag = vs;
            //tv.Tag = d;
            tv.Nodes.Clear();

            ArrayList nodes = new ArrayList();

            TreeNode REF = null;

            try
            {
                if (pgs != null)
                {
                    //pgs.Invoke(new Action(() => { pgs.Maximum = 100 * vs.Projects.Count(); pgs.Maximum = 100 * vs.Projects.Count(); pgs.Value = 0; }));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            };
            Dictionary<string, TreeNode> DSN;
            foreach (VSProject ps in vs.Projects)
            {
                ArrayList NL = new ArrayList();

                DSN = new Dictionary<string, TreeNode>();

                if (thisproject != "")
                    if (thisproject != ps.FileName)
                        continue;

                ps.vs = vs;

                ArrayList FDD = new ArrayList();

                ArrayList EMB = new ArrayList();

                ArrayList CDD = new ArrayList();

                ArrayList DDD = new ArrayList();

                REF = null;

                FD = new ArrayList();

                string file = ps.FileName;

                string f = "";

                string[] fs = null;

                TreeNode t = null;

                if (file.Contains("."))
                    projects++;

                f = Path.GetFileNameWithoutExtension(file);

                fs = f.Split("\\".ToCharArray());

                t = new TreeNode(f);

                t.ImageKey = "Folder";
                t.SelectedImageKey = "FolderOpen";

                if (dc != null)
                {
                    string exts = Path.GetExtension(file);

                    if (dc.ContainsKey(exts))
                    {
                        string dd = (string)dc[exts];
                        if (dd != "")
                        {
                            t.ImageKey = dd;
                            t.SelectedImageKey = dd;
                        }
                    }
                }

                string pp = file;

                nodes.Add(t);

                try
                {
                    pp = Path.GetFullPath(file);

                    pp = pp.Replace("\\\\", "\\");
                }
                catch (Exception ex)
                {
                }

                ProjectItemInfo prs = new ProjectItemInfo();

                prs.vs = vs;

                prs.ps = ps;

                prs.IsProjectNode = true;

                prs.Guid = ps.ProjectGuid;

                VSProjectItem prc = new VSProjectItem();
                prc.ItemType = "Project";
                prc.SubType = "Project";
                prs.psi = prc;

                prs.isItem = false;

                PL.Add(prs);

                d[pp] = t;

                t.Tag = prs;

                tv.Nodes.Add(t);

                string nm = ps.GetType().FullName;

                ArrayList dirs = new ArrayList();

                ArrayList nds = new ArrayList();

                if (ps.FileName.EndsWith(".vbproj"))
                {
                    t.ImageKey = "VBProject_SolutionExplorerNode";
                    t.SelectedImageKey = "VBProject_SolutionExplorerNode";
                }

                if (ps.FileName.EndsWith(".wixproj"))
                {
                    t.Text += " (incompatible project)";
                }
                else
                if (ps.ProjectType == "Unknown" && ps.FileName.EndsWith(".shproj"))
                {
                    try
                    {
                        Dictionary<string, string> globalProperties = new Dictionary<string, string>();
                        Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(ps.FileName, globalProperties, "15.0");

                        List<VSProjectItem> list = new List<VSProjectItem>();

                        if (ps.Items != null)
                            list = new List<VSProjectItem>(ps.Items);

                        foreach (Microsoft.Build.Evaluation.ProjectItem b in pc.Items)
                        {
                            if (b.ItemType != "Compile")
                                continue;
                                                       
                            string g = Path.GetFullPath(ps.FileName).Replace(Path.GetFileName(ps.FileName), "");

                            string p = b.EvaluatedInclude.Replace(g, "");

                            VSProjectItem v = new VSProjectItem();

                            v.ItemType = b.ItemType;

                            v.fileName = b.EvaluatedInclude;

                            v.Include = p;

                            list.Add(v);
                        }

                        ps.Items = (IEnumerable<VSProjectItem>)list;

                        TreeNode n = t;
                        n.Text = ps.Name;
                        n.ImageKey = "shared";
                        n.SelectedImageKey = "shared";

                        ProjectItemInfo pr0 = new ProjectItemInfo();
                        pr0.vs = vs;
                        pr0.ps = ps;
                        pr0.IsProjectNode = true;
                        pr0.Guid = ps.ProjectGuid;
                        n.Tag = pr0;

                        pc.ProjectCollection.UnloadAllProjects();
                    }
                    catch (Exception e) { MessageBox.Show("shared error " + " + ps.Name " + e.Message.ToString()); }
                }
                else
                if (ps.ProjectType == "SolutionFolder")
                {
                    //Microsoft.Build.Evaluation.Project pc = new Microsoft.Build.Evaluation.Project(ps.FileName);

                    TreeNode n = t;
                    n.Text = ps.Name;

                    ProjectItemInfo pr0 = new ProjectItemInfo();
                    pr0.vs = vs;
                    pr0.ps = ps;
                    pr0.Guid = ps.ProjectGuid;
                    pr0.SubType = "SolutionFolder";

                    n.Tag = pr0;

                    t.ImageKey = "Folder";
                    t.SelectedImageKey = "FolderOpen";
                    n.ImageKey = "Folder";
                    n.SelectedImageKey = "FolderOpen";

                    foreach (VSProvider.Project pg in solution.Projects)
                    {
                        if (pg.Name == ps.Name)
                        {
                            if (pg.ProjectSection != null)
                                if (pg.ProjectSection.Entries != null)

                                    foreach (string obs in pg.ProjectSection.Entries.Keys)
                                    {
                                        TreeNode me = new TreeNode(obs);

                                        Guid guid;
                                        bool cc = Guid.TryParse(obs, out guid);
                                        if (cc == false)
                                            continue;

                                        string filepath = Path.GetFullPath(ps.GetProjectFolder() + "\\" + obs as string);

                                        filepath = filepath.Replace(ps.GetProjectFolder() + "\\", "");

                                        me.ImageKey = "resource_32xMD";
                                        me.SelectedImageKey = "resource_32xMD";

                                        ProjectItemInfo pr2 = new ProjectItemInfo();
                                        pr2.vs = vs;
                                        pr2.ps = ps;
                                        pr2.psi = new VSProjectItem();
                                        pr2.psi.ItemType = "NestedProject";
                                        pr2.psi.Include = filepath;
                                        pr2.psi.fileName = filepath;
                                        pr2.Guid = guid.ToString();

                                        if (Path.IsPathRooted(filepath))
                                            filepath = Path.GetFileName(filepath);

                                        me.Text = filepath;

                                        me.Tag = pr2;

                                        t.Nodes.Add(me);
                                    }
                        }
                    }
                }

                PD.Add(t);

                if (DD.ContainsKey(ps.FileName) == false)
                    DD.Add(ps.FileName, t);

                foreach (VSProjectItem pi in ps.Items)
                {


                    //if (pi.Include.Contains("C:\\Program Files (x86)\\MSBuild\\14.0\\bin\\amd64\\CSharp.ProjectItemsSchema.xaml"))
                    //    MessageBox.Show("found");

                    ProjectItemInfo pr3 = new ProjectItemInfo();
                    pr3.vs = vs;
                    pr3.ps = ps;
                    pr3.psi = pi;
                    pr3.Include = pi.Include;

      
                    string g = pi.fileName;

                    string dependentupon = "";

                    pi.GetSubType(out dependentupon);

                    string[] bc = pi.Include.Split("\\".ToCharArray());

                    string dependsupon = bc[bc.Length - 1];

                    if (DSN.ContainsKey(pi.Include/*dependsupon*/))
                    {
                        //TreeNode b = DSN[pi.Include/*dependsupon*/];
                        //b.Tag = pr3;

                        TreeNode ng = new TreeNode();
                        ng.Text = pi.Include;
                        TreeNode b = DSN[pi.Include/*dependentupon*/];
                        ng.Tag = pr3;
                        b.Nodes.Add(ng);

                        continue;
                    }
                    else
                    if (dependentupon != "")
                    {
                        dependentupon = GetDependentResource(pi.Include, dependsupon, dependentupon);
                        if (!DSN.ContainsKey(dependentupon))
                        {
                            TreeNode ns = new TreeNode();
                            ns.Text = pi.Include;// dependentupon;
                            ns.Tag = pr3;

                            DSN.Add(dependentupon, ns);

                            if (dependentupon != pi.Include/*dependsupon*/)
                            {
                                ProjectItemInfo pr4 = new ProjectItemInfo();
                                pr4.vs = vs;
                                pr4.ps = ps;
                                pr4.psi = pi;
                                pr4.Include = dependentupon;// pi.Include;

                                TreeNode ng = new TreeNode();
                                ng.Text = dependentupon;// pi.Include;
                                ng.Tag = pr4;
                                ns.Nodes.Add(ng);
                            }
                        }
                        else
                        {
                            TreeNode ng = new TreeNode();
                            ng.Text = pi.Include;
                            TreeNode b = DSN[dependentupon];
                            ng.Tag = pr3;
                            b.Nodes.Add(ng);
                        }
                        pi.SubType = "resx";
                        continue;
                    }
                    else
                    if (pi.SubTypes != "")
                    {
                        if (!DSN.ContainsKey(pi.Include/*dependsupon*/))
                        {
                            TreeNode ns = new TreeNode();
                            ns.Text = pi.Include;
                            ns.Tag = pr3;

                            DSN.Add(pi.Include/*dependsupon*/, ns);

                            continue;
                        }
                        else
                        {
                            TreeNode b = DSN[pi.Include/*dependsupon*/];
                            b.Tag = pr3;
                            continue;
                        }
                    }

                    if (pi.Info != null)
                    {
                        pi.Include = pi.Info;
                        pr3.Include = pi.Info;
                    }

                  
                    if (pi.ItemType == "Content")
                    {
                        CDD.Add(pi);
                        if (pi.Info != null)
                        {
                            pi.Include = pi.Info;
                        }
                        pi.fileName = ps.FileName;
                    }
                    else
                    if (pi.ItemType == "None" && pi.Info != null)
                    {
                        
                        pi.Include = pi.Info;
                        pi.fileName = ps.FileName;
                        CDD.Add(pi);
                    }
                    else
                    if (pi.ItemType == "None" || pi.ItemType == "Resource")
                    {
                        pi.fileName = ps.FileName;
                        if (Path.GetExtension(pi.Include) == ".sh")
                            continue;
                        if (Path.GetExtension(pi.Include) == ".json")
                            CDD.Add(pi);
                        if (Path.GetExtension(pi.Include) == ".txt")
                            CDD.Add(pi);
                        if (Path.GetExtension(pi.Include) == ".vsixmanifest")
                            CDD.Add(pi);
                        if (Path.GetExtension(pi.Include) == ".datasource")
                            DDD.Add(pi);

                        
                        TreeNode ns = new TreeNode();
                        ns.Text = pi.Include;
                        ns.Tag = pr3;
                        //ns.ImageKey = "new";
                        t.Nodes.Add(ns);
                    }
                    else
                    if (pi.ItemType == "file")
                    {
                        if (g == null)
                        {
                            g = pi.Include;
                            pi.fileName = pi.Include;
                        }

                        string gs = Path.GetFileName(g);

                        gs = gs + " - " + pi.ItemType;

                        TreeNode t2 = new TreeNode(gs);

                        t2.ImageKey = "Folder";
                        t2.SelectedImageKey = "FolderOpen";

                        t2.Tag = pr3;

                        t.Nodes.Add(t2);
                    }
                    else
                        if (pi.ItemType == "Compile")
                    {

                       // if (pi.Include.Contains("Build\\version.cs"))
                       //     MessageBox.Show("bv");


                        TreeNode res = t;

                        pi.GetSubType(out dependentupon);

                        //res.Tag = pr3;

                        string gs = Path.GetFileName(g);

                        string filapath = "";

                        if (Path.IsPathRooted(pi.Include) == true)
                            filepath = pi.Include;
                        else
                            filepath = Path.GetFullPath(ps.GetProjectFolder() + "\\" + pi.Include);

                        filepath = filepath.Replace(ps.GetProjectFolder() + "\\", "");

                        
                        if (pi.Info != null)
                        {
                            filepath = pi.Info;
                            
                        }

                        string[] cc = filepath.Split("\\".ToCharArray());

                        if (cc.Count() > 1)
                        {
                            bool fnds = false;

                            gs = pi.Include;// + " - " + pi.ItemType;

                            string folder = cc[0];

                            foreach (TreeNode node in FD)
                            {
                                string fss = node.Text;
                                if (fss == folder)
                                {
                                    t = node;
                                    fnds = true;
                                    break;
                                }
                            }

                            if (fnds == false)
                            {
                                string name = "";
                                int i = 0;
                                while (i < cc.Length - 1)
                                {
                                    name = name + cc[i] + "\\";
                                    i++;
                                }

                                name = name.Remove(name.Length - 1);

                                TreeNode bbb = new TreeNode(folder);
                                bbb.ImageKey = "Folder";
                                bbb.SelectedImageKey = "FolderOpen";
                                ProjectItemInfo ppb = new ProjectItemInfo();
                                ppb.vs = pr3.vs;
                                ppb.ps = pr3.ps;
                                VSProjectItem ppp = new VSProjectItem();
                                ppp.SubType = "Folder";
                                ppp.ItemType = "Folder";

                                ppp.Include = cc[0];
                                ppb.psi = ppp;
                                ppb.Include = cc[0];
                        
                                t.Nodes.Add(bbb);

                                bbb.Tag = ppb;

                                FD.Add(bbb);
                            }
                        }

                        TreeNode t2 = new TreeNode(gs);

                        t2.ImageKey = "CSharpFile_SolutionExplorerNode";
                        t2.SelectedImageKey = "CSharpFile_SolutionExplorerNode";

                        if (pi.SubTypes != "")
                            if (pi.SubTypes == "UserControl")
                            {
                                t2.ImageKey = "usercontrol";
                                t2.SelectedImageKey = "usercontrol";
                            }
                            else if (pi.SubTypes != "")
                                if (pi.SubTypes == "Component")
                                {
                                    t2.ImageKey = "component";
                                    t2.SelectedImageKey = "component";
                                }
                                else if (pi.SubTypes != "")
                                    if (pi.SubTypes == "Form")
                                    {
                                        t2.ImageKey = "forms";
                                        t2.SelectedImageKey = "forms";
                                    }
                        t2.Tag = pr3;

                        if (g == null)
                            g = pi.Include;

                        //string pc = Path.GetFullPath(g);

                        //pc = pc.Replace("\\\\", "\\");

                        //d[pc] = t2;

                        t.Nodes.Add(t2);

                        t = res;
                    }
                    else if (pi.ItemType == "Folder" ||  pi.ItemType == "WCFMetadata")
                    {
                        
                        FDD.Add(pi.Include);
                    }
                    else if (pi.ItemType == "EmbeddedResource")
                    {
                        if (!pi.Include.EndsWith(".resx") || dependentupon == "")
                        {
                            pi.fileName = ps.FileName;
                      
                            TreeNode ns = new TreeNode();
                            ns.Text = pi.Include;
                            ns.Tag = pr3;
                            t.Nodes.Add(ns);
                            continue;
                        }

                        EMB.Add(pi.Include);
                    }
                    else
                    {
                        if (pi.ItemType == "ProjectReference")
                            pi.ItemType = "Reference";
                        TreeNode N = null;
                        bool found = false;
                        foreach (TreeNode n in t.Nodes)
                        {
                            if (n.Text == pi.ItemType)
                            {
                                N = n;

                                found = true;

                                break;
                            }
                        }

                        TreeNode t3 = N;

                        string[] pw = pi.Include.Split("\\".ToCharArray());

                        if (pi.ItemType != "Reference")
                            if (pw.Length > 1)
                            {
                                int c = 0;
                                bool re = false;
                                //foreach (string pws in dirs)
                                {
                                    //if (pw[0] == pws)
                                    {
                                        TreeNode ns = FindNode(nds, pw[0]);

                                        if (ns != null)
                                        {
                                            TreeNode node = new TreeNode(pi.Include.Replace(pw[0] + "\\", ""));

                                            ns.Nodes.Add(node);

                                            node.Tag = pr3;

                                            node.ImageKey = "filesource";
                                            node.SelectedImageKey = "filesource";

                                            re = true;
                                        }
                                    }

                                    c++;
                                }

                                if (re == false)
                                {
                                    TreeNode ns2 = new TreeNode(pw[0]);

                                    ns2.ImageKey = "Folder";
                                    ns2.SelectedImageKey = "FolderOpen";

                                    ProjectItemInfo pps = new ProjectItemInfo();

                                    prs.vs = vs;
                                    prs.ps = ps;

                                    prs.psi = new VSProjectItem();
                                    prs.psi.SubType = "Folder";
                                    prs.psi.Include = pw[0];
                                    prs.psi.ItemType = "Folder";

                                    pps.psi = prs.psi;
                                    ns2.Tag = pps;

                                    TreeNode node2 = new TreeNode(pw[1]);

                                    node2.Tag = pr3;

                                    ns2.Nodes.Add(node2);

                                    if (t3 != null)
                                        t3.Nodes.Add(ns2);
                                    else t.Nodes.Add(ns2);

                                    if (pw[0] == "" || pw[0] == null)
                                    {
                                        dirs.Add(pi.ItemType + " - " + pi.fileName);
                                        node2.Text = pi.ItemType + " - " + pi.fileName;
                                    }
                                    else
                                        dirs.Add(pw[0]);

                                    nds.Add(ns2);
                                    if (pi.ItemType == "None")
                                    {
                                        string[] cc = pi.Include.Split("\\".ToCharArray());
                                        TreeNode t2 = new TreeNode();
                                        t2.Text = cc[cc.Length - 1];

                                        t2.ImageKey = "filesource";
                                        t2.SelectedImageKey = "filesource";

                                        t2.Tag = pr3;

                                        if (g == null)
                                            g = pi.Include;

                                        string pc = Path.GetFullPath(g);

                                        pc = pc.Replace("\\\\", "\\");

                                        d[pc] = t2;

                                        t.Nodes.Add(t2);

                                        NL.Add(t2);
                                    }
                                }

                                continue;
                            }

                        if (found == true)
                        {
                            string dd = "";
                            try
                            {
                                dd = Path.GetFileName(g);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            string b = pi.Include;

                            TreeNode t4 = new TreeNode(b);

                            t4.Tag = pr3;

                            t4.ImageKey = "filesource";
                            t4.SelectedImageKey = "filesource";

                            if (pi.ItemType.ToString() == "Reference" || pi.ItemType.ToString() == "ProjectReference")
                            {
                                t4.ImageKey = "reference_16xLG";
                                t4.SelectedImageKey = "reference_16xLG";
                            }

                            t3.Nodes.Add(t4);
                            if (g != null)
                            {
                                try
                                {
                                    string pc = Path.GetFullPath(Path.GetFileName(g));

                                    pc = pc.Replace("\\", "\\\\");

                                    d[pc] = t4;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                };
                            }
                            continue;
                        }
                        else
                        {
                            TreeNode ts = new TreeNode(pi.ItemType);
                            t.Nodes.Add(ts);

                            ts.Tag = pr3;

                            ts.ImageKey = "resource_32xMD";
                            ts.SelectedImageKey = "resource_32xMD";

                            if (pi.ItemType.ToString() == "Reference" || pi.ItemType.ToString() == "ProjectReference")
                            {
                                REF = ts;

                                ts.ImageKey = "reference_16xLG";
                                ts.SelectedImageKey = "reference_16xLG";
                            }
                            else
                            {
                                bool df = false;
                                foreach (TreeNode nc in nds)
                                {
                                    if (nc.Text == pi.ItemType)
                                        df = true;
                                }
                                if (df == false)
                                    nds.Add(ts);
                            }

                            string da = "";

                            try
                            {
                                da = Path.GetFileName(g);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            if (da == null || da == "")

                                da = pi.Include;

                            TreeNode tp = new TreeNode(da);

                            tp.Tag = pr3;

                            tp.ImageKey = "filesource";
                            tp.SelectedImageKey = "filesource";

                            if (pi.ItemType.ToString() == "Reference" || pi.ItemType.ToString() == "ProjectReference")
                            {
                                tp.ImageKey = "reference_16xLG";
                                tp.SelectedImageKey = "reference_16xLG";
                            }

                            ts.Nodes.Add(tp);

                            tp.Tag = pr3;

                            try
                            {
                                if (g != null)
                                {
                                    string pc = Path.GetFullPath(Path.GetFileName(g));
                                    pc = pc.Replace("\\\\", "\\");
                                    d[pc] = tp;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            };
                        }
                    }
                }

                tv.BeginUpdate();
                //ArrayList G = new ArrayList();
                //FindNodesByInclude(t, "AsynchronousMessage_13464_32.bmp", G);

                ProjectOrder(t, FD, REF, nds);

                ProjectForms(EMB, t);

                foreach (TreeNode node in t.Nodes)
                {
                    ProjectSubForms(EMB, t, node, node.Text);
                }

              
                ProjectCleanUp(t);
               
                AppendEmptyFolders(t, FDD, vs, ps);
                
                MoveFolderNodes(t);
                
                MoveEmbeddedResources(t, EMB);
                
                MoveOtherNodes(tv, t, NL);

                MoveProjectOtherNodes(EMB, tv, t);
                
                MoveContent(t, CDD);

                MoveContent(t, DDD);

                MoveContent(t, DSN);

                ProjectCleanUp(t);
              
                MoveContent(t, "Resources");
                //if (ps.Name == "VSExplorer")
                //    MessageBox.Show("ve");
                MoveContentTo(t);
                ArrayList G = new ArrayList();
                FindNodesByInclude(t, "Formatting\\XmlLiterals.resx", G);
                RemoveDuplicates(t);
                G = new ArrayList();
                FindNodesByInclude(t, "Formatting\\XmlLiterals.resx", G);
                SortFolderNodes(t);

                tv.EndUpdate();

                if (thisproject != "")
                    if (thisproject == ps.FileName)
                        return vs;
            }

            tv.SuspendLayout();

            SetOutput(vs);

            AddRootNode(tv, vs.Name, vs.Projects.Count());

            int pcp = NestedProject(solution, tv.Nodes[0]);

            foreach (TreeNode ns in tv.Nodes[0].Nodes)
                SortMainNodes(ns);

            _projectCount = 0;

            SortProjectNodes(tv.Nodes[0]);

            string projectCounts = Path.GetFileNameWithoutExtension(vs.Name) + "(" + projects + " projects)";
            if (projects == 1)
                projectCounts = Path.GetFileNameWithoutExtension(vs.Name) + "(" + projects + " project)";
            tv.Nodes[0].Text = projectCounts;

            foreach (TreeNode node in PD)
            {
                ProjectItemInfo p = node.Tag as ProjectItemInfo;
                if (p != null)
                {
                    VSProject vp = p.ps;

                    string s = vp.GetPropertyBase("IsPortable");
                    if (s != null)
                        if (s.ToLower() == "true")
                            node.Text += " (Portable)";
                }
            }

            tv.ResumeLayout();

            return vs;
        }

        public void loggers(TreeNode node, string s)
        {
            if (node.Text != "MSBuildTask.Shared")
                return;

            MessageBox.Show("Node " + node.Nodes.Count);

            return;

            ArrayList NPs = new ArrayList();
            FindNodeExactTree(node, "MSBuildTask.Shared", NPs);

            MessageBox.Show("found in test start" + NPs.Count);

            if (NPs.Count > 0)
            {
                TreeNode np = NPs[0] as TreeNode;
                MessageBox.Show("test nodes " + np.Nodes.Count + " iteration" + np.ToString());
            }
        }

        public bool IsFolder(TreeNode node)
        {
            ProjectItemInfo p = node.Tag as ProjectItemInfo;
            if (p == null)
                return false;
            if (p.psi == null)
                return false;
            if (p.psi.ItemType == "Folder")
                return true;
            return false;
        }

        public string GetFileName(TreeNode node)
        {
            ProjectItemInfo p = node.Tag as ProjectItemInfo;
            if (p == null)
                return "";
            if (p.ps == null)
                return "";
            return p.ps.FileName;
        }

        /// <summary>
        /// Appends the empty folders.
        /// </summary>
        /// <param name="master">The master.</param>
        /// <param name="L">The l.</param>
        /// <param name="vs">The vs.</param>
        /// <param name="pp">The pp.</param>
        public void AppendEmptyFolders(TreeNode master, ArrayList L, VSSolution vs, VSProject pp)
        {
            foreach (string s in L)
            {
                string[] cc = s.Split("\\".ToCharArray());

                string folders = "";

                TreeNode ns = master;

                foreach (string names in cc)
                {
                    string name = names.Trim();

                    if (name == "")
                        continue;

                    if (folders == "")
                        folders = name;
                    else folders = folders + "\\" + name;

                    TreeNode node = FindNodeExact(ns, name);

                    if (node == null)
                    {
                        TreeNode nodes = new TreeNode();
                        nodes.Text = name;
                        nodes.ImageKey = "Folder";
                        nodes.SelectedImageKey = "FolderOpen";

                        ProjectItemInfo p = new ProjectItemInfo();
                        p.vs = vs;
                        p.ps = pp;
                        p.psi = new VSProjectItem();
                        p.psi.SubType = "Folder";
                        p.psi.Include = folders;
                        p.psi.ItemType = "Folder";
                        p.Include = folders;

                        nodes.Tag = p;

                        if (ns == null)
                        {
                            master.Nodes.Add(nodes);
                            ns = nodes;
                        }
                        else
                        {
                            ns.Nodes.Add(nodes);
                            ns = nodes;
                        }
                    }
                    else
                    {
                        ns = node;
                    }
                }
            }
        }

        public int NestedProject(VSProvider.ISolution s, TreeNode master)
        {
            VSProvider.GlobalSection[] obs = s.Global.ToList().ToArray();

            VSProvider.Project[] p = s.Projects.ToArray();

            VSProvider.GlobalSection g = GetSection(obs, "NestedProjects");

            if (g == null)
                return 0;

            //bool found = false;

            //int change = 0;

            int i = 0;
            foreach (string entry in g.Entries.Keys)
            {
                Guid guid = new Guid(entry);

                VSProvider.Project pp = FindProject(p, guid);

                if (pp == null)
                    continue;

                ArrayList N = new ArrayList();

                TreeNode node = null;

                FindNodeExactTree(master, pp.Name, N);

                foreach (TreeNode ng in N)
                {
                    ProjectItemInfo pc = ng.Tag as ProjectItemInfo;
                    if (pc != null)
                    {
                        //            if (pc.psi == null)
                        if (pc.Guid == entry)
                            node = ng;
                    }
                }

                //if (node == null)
                //    MessageBox.Show("no node");

                string entrys = g.Entries[entry];

                if (entrys == null)
                    continue;

                //if (entrys == "{3F40F71B-7DCF-44A1-B15C-38CA34824143}")
                //    MessageBox.Show("Compilers");

                //if (entry == "{3F40F71B-7DCF-44A1-B15C-38CA34824143}")
                //    MessageBox.Show("Compilers");

                guid = new Guid(entrys);

                VSProvider.Project pps = FindProject(p, guid);

                if (pps == null)
                    continue;

                N = new ArrayList();

                FindNodeExactTree(master, pps.Name, N);

                TreeNode nodes = null;

                foreach (TreeNode ng in N)
                {
                    ProjectItemInfo pc = ng.Tag as ProjectItemInfo;
                    if (pc != null)
                    {
                        //            if (pc.psi == null)
                        if (pc.Guid == entrys)
                            nodes = ng;
                    }
                }

                if (nodes == null)
                    continue;

                Move2Node(master, node, nodes);

                i++;
            }

            Merge2Clean(master);

            MoveFolderNodes(master);

            return g.Entries.Count();
        }

        private bool isNested(TreeNode nodes, TreeNode node)
        {
            TreeNode ns = node.Parent;

            while (ns != null)
            {
                if (ns.Parent == nodes)
                {
                    nodes.Nodes.Remove(ns);

                    break;
                }
                ns = ns.Parent;
            }

            ns = nodes.Parent;

            while (ns != null)
            {
                if (ns.Parent == node)
                {
                    nodes.Nodes.Remove(ns);

                    break;
                }

                ns = ns.Parent;
            }

            return false;
        }

        public void Move2Node(TreeNode master, TreeNode node, TreeNode nodes)
        {
            if (node == null)
                return;

            if (node.Equals(nodes))
                return;

            if (node.Parent == null)
                return;

            if (nodes.Equals(master))
                return;

            if (node.Equals(master))
                return;

            if (node.Parent == nodes)
                return;

            //            if (isNested(node, nodes))
            //                return;

            //master.Nodes.Remove(node);

            node.Parent.Nodes.Remove(node);

            string exts = Path.GetExtension(node.Text);

            if (dc.ContainsKey(exts))
            {
                node.ImageKey = dc[exts];// "CSharpProject_SolutionExplorerNode_24";
                node.SelectedImageKey = dc[exts];// "CSharpProject_SolutionExplorerNode_24";
            }
            else
            {
                node.ImageKey = "CSharpProject_SolutionExplorerNode_24";
                node.SelectedImageKey = "CSharpProject_SolutionExplorerNode_24";
            }
            //isNested(nodes, node);

            nodes.Nodes.Insert(nodes.Nodes.Count - 1, node);

            nodes.ImageKey = "Folder";
            nodes.SelectedImageKey = "Folder";

            ArrayList R = FindNodes2Project(nodes, "");
            if (R.Count > 0)
            {
                int i = 0;
                while (i < R.Count)
                {
                    TreeNode ns = R[i] as TreeNode;
                    nodes.Nodes.Remove(ns);
                    nodes.Nodes.Insert(nodes.Nodes.Count, ns);

                    i++;
                }
            }

            ArrayList N = new ArrayList();
            ArrayList F = new ArrayList();

            bool nested = false;

            foreach (TreeNode ns in nodes.Nodes)
            {
                ProjectItemInfo p = ns.Tag as ProjectItemInfo;

                if (node.Equals(ns))
                    nested = true;

                if (p.IsProjectNode == true)
                    N.Add(ns);
                else F.Add(ns);
            }

            if (nested == true)
                return;

            N = SortNodes(N, false);

            nodes.Nodes.Clear();

            foreach (TreeNode ns in F)
            {
                string[] cc = ns.Text.Split("\\".ToCharArray());
                ns.Text = cc[cc.Length - 1];
                nodes.Nodes.Add(ns);
            }
            foreach (TreeNode ns in N)
                nodes.Nodes.Add(ns);
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

        public void MoveContent(TreeNode master, ArrayList CDD)
        {
            foreach (VSProjectItem pi in CDD)
            {
                string Include = pi.Include;

                string c = Include;

                if (Include == null)
                    continue;

                string guid = "";

                TreeNode nodes = FindNodesIncludeExact(master, Include);

                ProjectItemInfo p = null;

                if (nodes != null)
                {
                    p = nodes.Tag as ProjectItemInfo;

                    if (p != null)
                    {
                        guid = p.Guid;
                        p.psi.fileName = pi.fileName;
                    }
                }

                nodes = master;

                string[] cc = c.Split("\\".ToCharArray());

                string d = "";

                //if (Include.Contains("ApplicationIcon"))
                //    MessageBox.Show("");

                int i = 0;
                foreach (string s in cc)
                {
                    if (i == 0)
                        d = s;
                    else
                        d += "\\" + s;

                    //TreeNode ns = FindNodesIncludeExact(nodes, d);

                    TreeNode ns = FindNodesIncludeExact(nodes, s);

                    if (ns == null)
                    {
                        ns = new TreeNode();
                        ns.Text = s;
                        ProjectItemInfo pp = new ProjectItemInfo();
                        pp.ps = new VSProject();
                        pp.psi = new VSProjectItem();
                        pp.psi.Include = d;
                        ns.Tag = pp;
                        pp.psi.fileName = pi.fileName;
                        pp.Include = d;

                        //ns.SelectedImageKey = "Folder";
                        //ns.ImageKey = "Folder";

                        //nodes.ImageKey = "Folder";
                        //nodes.SelectedImageKey = "FolderOpen";

                        //master.Nodes.Remove(node);

                        string[] bb = s.Split(".".ToCharArray());

                        if (bb.Length <= 1)
                            pp.psi.ItemType = "Folder";
                        if (i < cc.Length - 1)
                        {
                            ns.SelectedImageKey = "Folder";
                            ns.ImageKey = "Folder";
                        }
                        nodes.Nodes.Add(ns);
                    }

                    nodes = ns;
                    i++;
                }
            }
        }

        public void MoveContent(TreeNode master, Dictionary<string, TreeNode> D)
        {
            //foreach (string cs in D.Keys)
            {
                //    master.Nodes.Add(D[cs]);
            }

            foreach (string cs in D.Keys)
            {
                //master.Nodes.Add(D[cs]);
                TreeNode ng = D[cs];
                ProjectItemInfo pc = ng.Tag as ProjectItemInfo;
                VSProjectItem pi = pc.psi;

                string Include = pc.Include;// pi.Include;

                //if (Include == "Editors\\Dialogs\\Settings.resx")
                //    MessageBox.Show("OK");
                //if (Include == "Editors\\Dialogs\\Settings.cs")
                //    MessageBox.Show("OK");

                string c = Include;

                if (Include == null)
                    continue;

                string guid = "";

                TreeNode nodes = FindNodesIncludeExact(master, Include);

                ProjectItemInfo p = null;

                if (nodes != null)
                {
                    p = nodes.Tag as ProjectItemInfo;

                    if (p != null)
                    {
                        guid = p.Guid;
                        p.psi.fileName = pi.fileName;
                    }
                }

                nodes = master;

                string[] cc = c.Split("\\".ToCharArray());

                string d = "";

                //if (Include.Contains("ApplicationIcon"))
                //    MessageBox.Show("");

                int i = 0;
                foreach (string s in cc)
                {
                    if (s.Contains("."))
                        continue;

                    if (i == 0)
                        d = s;
                    else
                        d += "\\" + s;

                    //TreeNode ns = FindNodesIncludeExact(nodes, d);

                    TreeNode ns = FindNodesIncludeExact(nodes, s);

                    if (ns == null)
                    {
                        ns = new TreeNode();
                        ns.Text = s;
                        ProjectItemInfo pp = new ProjectItemInfo();
                        pp.ps = new VSProject();
                        pp.psi = new VSProjectItem();
                        pp.psi.Include = d;
                       // pp.Include = pc.Include; 
                        ns.Tag = pp;
                        pp.psi.fileName = pi.fileName;

                        //ns.SelectedImageKey = "Folder";
                        //ns.ImageKey = "Folder";

                        //nodes.ImageKey = "Folder";
                        //nodes.SelectedImageKey = "FolderOpen";

                        //master.Nodes.Remove(node);

                        string[] bb = s.Split(".".ToCharArray());

                        if (bb.Length <= 1)
                            pp.psi.ItemType = "Folder";
                        if (i < cc.Length - 1)
                        {
                            ns.SelectedImageKey = "Folder";
                            ns.ImageKey = "Folder";
                        }
                        nodes.Nodes.Add(ns);
                    }

                    nodes = ns;
                    i++;
                }

                nodes.Nodes.Add(ng);
            }
        }

        public void MoveContent(TreeNode master, string name)
        {
            TreeNode b = FindNode(master, name);

            if (b == null)
                return;

            ArrayList N = new ArrayList();

            foreach (TreeNode ng in b.Nodes)
            {
                if (ng == null)
                    continue;

                //if (ng.Text.Equals("Icons"))
                //{
                //    MessageBox.Show("");
                //    continue;
                //}

                ProjectItemInfo p = ng.Tag as ProjectItemInfo;

                if (p == null)
                    continue;

                string guid = p.Guid;

                if (p.psi == null)
                    continue;

                if (p.psi.ItemType == "Folder")
                    continue;

                string Include = p.psi.Include.Trim();

                if (Include == null)
                    continue;

                TreeNode nodes = master;// FindNodesIncludeExact(master, Include);

                string[] cc = Include.Split("\\".ToCharArray());

                string d = "";

                int i = 0;
                foreach (string s in cc)
                {
                    if (i == 0)
                        d = s;
                    else
                        d += "\\" + s;

                    //TreeNode ns = FindNodesIncludeExact(nodes, d);

                    TreeNode ns = FindNodesIncludeExact(nodes, s);

                    if (ns == null)
                    {
                        ns = new TreeNode();
                        ns.Text = s;
                        ProjectItemInfo pp = new ProjectItemInfo();
                        pp.Guid = guid;
                        pp.ps = new VSProject();
                        pp.psi = new VSProjectItem();
                        pp.psi.Include = d;
                        ns.Tag = pp;
                        ns.SelectedImageKey = "Folder";
                        ns.ImageKey = "Folder";

                        if (i < cc.Length - 1)
                        {
                            pp.psi.ItemType = "Folder";
                            ns.SelectedImageKey = "Folder";
                            ns.ImageKey = "Folder";
                            nodes.Nodes.Add(ns);
                        }
                        else
                        {
                            string[] bb = s.Split(".".ToCharArray());

                            //pp.psi.ItemType = "resourcefile";
                            //ns.SelectedImageKey = "resourcefile";

                            if (bb.Length <= 1)
                            {
                                pp.psi.ItemType = "Folder";
                                ns.SelectedImageKey = "Folder";
                                ns.ImageKey = "Folder";
                            }
                            else
                                SetExtensions(ns);
                            N.Add(ng);
                            nodes.Nodes.Add(ns);
                        }
                    }

                    nodes = ns;
                    i++;
                }
            }

            foreach (TreeNode n in N)
                if(n.Parent.Text == n.Text)
                    
                n.Parent.Nodes.Remove(n);
        }

        public void MoveContentTo(TreeNode master)
        {
            ArrayList P = Nodes2Array(master);

            foreach (TreeNode node in P)
            {

                
                if (node == null)
                    continue;
                ProjectItemInfo p = node.Tag as ProjectItemInfo;
                if (p == null)
                    continue;
                if(p.psi.ItemType == "None" || p.psi.ItemType == "EmbeddedResource")
                {
                    master.Nodes.Remove(node);
                    string s = p.Include;
                    string []cc = s.Split("\\".ToCharArray());
                    string name = "";
                    TreeNode nodes = master;
                  
                    int i = 0;
                    foreach (string d in cc)
                    {
                        if (name != "")
                            name += "\\";
                        name += d;
                        TreeNode ns = FindNodesIncludeExact(nodes, d);
                        if (ns == null)
                        {
                            ns = new TreeNode();
                            ns.Text = d;
                            ProjectItemInfo pp = new ProjectItemInfo();
                            pp.ps = new VSProject();
                            pp.psi = new VSProjectItem();
                            pp.psi.Include = name;
                            ns.Tag = pp;
                         //   ns.SelectedImageKey = "Folder";
                         //   ns.ImageKey = "Folder";
                            pp.Include = name;

                            if (i < cc.Length - 1)
                            {
                                pp.psi.ItemType = "Folder";
                                ns.SelectedImageKey = "Folder";
                                ns.ImageKey = "Folder";
                                nodes.Nodes.Add(ns);
                            }
                            else
                            {
                                string[] bb = d.Split(".".ToCharArray());

                                //pp.psi.ItemType = "resourcefile";
                                //ns.SelectedImageKey = "resourcefile";

                                if (bb.Length <= 1)
                                {
                                    pp.psi.ItemType = "Folder";
                                    ns.SelectedImageKey = "Folder";
                                    ns.ImageKey = "Folder";
                                }
                                
                                nodes.Nodes.Add(ns);
                                SetExtensions(ns);
                            }

                            nodes = ns;
                        }
                        else nodes = ns;
                        i++;
                    }
                    
                }
            }
        }

        //public void MoveNodeContent(TreeNode master)
        //{
        //    ArrayList N = new ArrayList();

        //    Nodes2Array(master, N);

        //    foreach (TreeNode ng in N)
        //    {
        //        if (ng == null)
        //            continue;

        //        //MoveNodeContent(ng);

        //        //string Include = c;

        //        ProjectItemInfo p = ng.Tag as ProjectItemInfo;

        //        if (p != null)
        //        {
        //            if (p.psi != null)
        //            {
        //                if (p.psi.ItemType != "Folder")
        //                {
        //                    string Include = p.psi.Include;

        //                    //if (Include.Contains("ApplicationIcon"))
        //                    //    MessageBox.Show("");

        //                    TreeNode nodes = FindNodesIncludeExact(master, Include);

        //                    nodes = master;

        //                    string[] cc = Include.Split("\\".ToCharArray());

        //                    string d = "";

        //                    int i = 0;
        //                    foreach (string s in cc)
        //                    {
        //                        if (i == 0)
        //                            d = s;
        //                        else
        //                            d += "\\" + s;

        //                        TreeNode ns = FindNodesIncludeExact(nodes, d);

        //                        if (ns == null)
        //                        {
        //                            ns = new TreeNode();
        //                            ns.Text = s;
        //                            ProjectItemInfo pp = new ProjectItemInfo();
        //                            pp.ps = new VSProject();
        //                            pp.psi = new VSProjectItem();
        //                            pp.psi.Include = d;
        //                            ns.Tag = pp;

        //                            //nodes.ImageKey = "Folder";
        //                            //nodes.SelectedImageKey = "FolderOpen";

        //                            //master.Nodes.Remove(node);

        //                            if (i < cc.Length - 1)
        //                            {
        //                                ns.SelectedImageKey = "Folder";// "filesource";
        //                                ns.ImageKey = "Folder";// "filesource";
        //                                nodes.Nodes.Add(ns);
        //                            }
        //                            else if (i == cc.Length - 1)
        //                            {
        //                                if (ng != nodes)
        //                                {
        //                                    master.Nodes.Remove(ng);

        //                                    nodes.Nodes.Add(ng);

        //                                }

        //                            }
        //                            else if (i == cc.Length - 1)
        //                            {
        //                                if(nodes != null)
        //                                if (ng != nodes)
        //                                {
        //                                    master.Nodes.Remove(ng);

        //                                    nodes.Nodes.Add(ng);

        //                                }

        //                            }

        //                        }
        //                        //            if(ns != null)
        //                        nodes = ns;

        //                        i++;
        //                    }
        //                }
        //            }
        //        }

        //    }

        //}

        public void ProjectCleanUp(TreeNode master)
        {
            ResourceNode(master);

            EmptyRemoveNode(master);

            MoveNode(master, "None");

            MoveNode(master, "EmbeddedResource");

            SetExtensions(master);
        }

        public void MoveEmbeddedResources(TreeNode master, ArrayList EMB)
        {
            foreach (string s in EMB)
            {
                string[] cc = s.Split("\\".ToCharArray());

                if (cc.Length > 1)
                    continue;

                string c = cc[cc.Length - 1];

                if (c.EndsWith(".resx"))
                    continue;

                TreeNode node = new TreeNode();
                node.Text = c;
                ProjectItemInfo p = new ProjectItemInfo();

                p.psi = new VSProjectItem();
                p.psi.Include = s;
                p.ps = null;
                p.Include = s;

                node.Tag = p;

                master.Nodes.Add(node);
            }
        }

        public void MoveFolderNodes(TreeNode master)
        {
            int p = 0;

            ArrayList L = Nodes2Array(master);

            SortNodes(master);

            ArrayList R = FindNodes2Image(master, "Folder");
            R.Reverse();
            if (R.Count > 0)
            {
                int i = 0;
                while (i < R.Count)
                {
                    TreeNode nodes = R[i] as TreeNode;
                    master.Nodes.Remove(nodes);
                    master.Nodes.Insert(0, nodes);

                    i++;
                }
            }

            TreeNode ns = FindNodeExact(master, "Reference");
            if (ns != null)
            {
                master.Nodes.Remove(ns);
                master.Nodes.Insert(0, ns);
            }

            ns = FindNodeExact(master, "Properties");
            if (ns != null)
            {
                master.Nodes.Remove(ns);
                master.Nodes.Insert(0, ns);
            }
        }

        public void SortMainNodes(TreeNode master)
        {
            ArrayList R = FindNodes2Project(master, "");

            R = SortNodes(R, false);

            if (R.Count > 0)
            {
                int i = 0;
                while (i < R.Count)
                {
                    TreeNode ns = R[i] as TreeNode;
                    master.Nodes.Remove(ns);
                    master.Nodes.Insert(master.Nodes.Count, ns);//-1
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

        public void MoveOtherNodes(TreeView v, TreeNode master, ArrayList NL)
        {
            v.BeginUpdate();

            int i = 0;
            while (i < NL.Count)
            {
                TreeNode node = NL[i++] as TreeNode;

                ProjectItemInfo p = node.Tag as ProjectItemInfo;

                if (p == null)
                    continue;
                if (p.psi == null)
                    continue;

                if (p.psi.Include == null)
                    continue;

                string Include = p.psi.Include;

                TreeNode nodes = FindNodesInclude(master, Include);

                if (nodes == null)
                    continue;

                nodes.ImageKey = "Folder";
                nodes.SelectedImageKey = "FolderOpen";

                master.Nodes.Remove(node);

                node.SelectedImageKey = "filesource";
                node.ImageKey = "filesource";

                nodes.Nodes.Add(node);
            }

            v.EndUpdate();
        }

        public TreeNode FindResource(ArrayList EMB, TreeNode node)
        {
            ProjectItemInfo pp = node.Tag as ProjectItemInfo;

            ProjectItemInfo pnp = null;

            ProjectItemInfo pc = null;

            foreach (string b in EMB)
            {
                string name = b.Replace(".resx", "");
                string file = pp.psi.Include.Replace(".cs", "");
                file = file.Replace(".Designer", "");
                file = file.Replace(".designer", "");

                name = name.ToLower();
                file = file.ToLower();

                if (name == file)
                {
                    pc = new ProjectItemInfo();
                    pc.vs = pp.vs;
                    pc.ps = pp.ps;
                    pc.psi = new VSProjectItem();
                    pc.psi.Include = b;
                    pc.psi.ItemType = "EmbeddedResource";
                    pc.psi.SubType = "EmbeddedResource";

                    TreeNode nb = new TreeNode();

                    string[] bc = b.Split("\\".ToCharArray());

                    nb.Text = bc[bc.Length - 1];

                    //nb.Text = b;

                    nb.ImageKey = "filesource";
                    nb.SelectedImageKey = "filesource";
                    nb.Tag = pc;
                    return nb;
                }
            }

            return null;
        }

        public void MoveProjectOtherNodes(ArrayList EMB, TreeView v, TreeNode master)
        {
            v.BeginUpdate();

            ArrayList NL = Nodes2Array(master);

            int i = 0;
            while (i < NL.Count)
            {
                TreeNode node = NL[i++] as TreeNode;

                ProjectItemInfo p = node.Tag as ProjectItemInfo;

                if (p == null)
                    continue;
                if (p.psi == null)
                    continue;
                if (p.psi.Include == null)
                    continue;

                string Include = p.psi.Include;

                TreeNode nodes = FindNodesInclude(master, Include);

                if (nodes != null && nodes != master)
                {
                    master.Nodes.Remove(node);

                    nodes.Nodes.Add(node);

                    MergeTheSame(node);

                    TreeNode b = FindResource(EMB, node);

                    TreeNode ng = null;

                    if (node.Nodes.Count > 0)
                        ng = node.Nodes[0];

                    if (b != null)
                        node.Nodes.Add(b);

                    // MergeTheSameForms(node);

                    int index = Include.LastIndexOf("\\");
                    if (index > 0)
                    {
                        node.Text = Include.Substring(index + 1, Include.Length - index - 1);
                        if (ng != null)
                            ng.Text = Include.Substring(index + 1, Include.Length - index - 1);
                    }
                }
                else
                {
                    int index = Include.LastIndexOf("\\");
                    if (index > 0)
                    {
                        Include = Include.Substring(0, index);

                        nodes = FindNodesInclude(master, Include);

                        if (nodes == null)
                            continue;
                        if (nodes == master)
                            continue;

                        node.Text = node.Text.Replace(Include, "");

                        master.Nodes.Remove(node);
                        nodes.Nodes.Add(node);
                    }
                }
            }

            v.EndUpdate();
        }

        public int GetNodeCount(TreeNode master, string name)
        {
            ArrayList L = Nodes2Array(master, name);

            return L.Count;
        }

        public void SortNodes(TreeNode master)
        {
            ArrayList L = Nodes2Array(master);

            //L = SortNodes(L);

           
            master.Nodes.Clear();

            foreach (TreeNode node in L)
            {
                if (master.Nodes.Contains(node) == false)
                    master.Nodes.Add(node);
            }
        }

        private int _projectCount = 0;

        public void SortProjectNodes(TreeNode master)
        {
            //        if (master.Text == "AnalyzerDriver")
            //            MessageBox.Show("d");

            ArrayList L = Nodes2Array(master);

            L = SortNodes(L);

            ArrayList A = new ArrayList();

            foreach (TreeNode ns in A)
            {
                //               if (ns.Text == "AnalyzerDriver")
                //                   MessageBox.Show("d");

                string text = ns.Text.Trim();

                if (Path.IsPathRooted(text))
                {
                    string[] cc = text.Split("\\".ToCharArray());

                    if (cc.Length > 1)
                    {
                        text = cc[cc.Length - 1];
                        ns.Text = text;
                    }
                }
            }

            //ArrayList S = new ArrayList();

            master.Nodes.Clear();

            foreach (TreeNode node in L)
            {
                master.Nodes.Remove(node);
                master.Nodes.Add(node);
            }

            L = Nodes2Array(master);

            ArrayList P = new ArrayList();

            foreach (TreeNode node in L)
            {
                ProjectItemInfo p = node.Tag as ProjectItemInfo;
                //if (p != null)
                //   // if (p.psi != null)
                //        if (p.IsProjectNode == true)
                //            P.Add(node);
                if (p.IsProjectNode == true)
                {
                    P.Add(node);
                    if (p.ps != null)
                        if (p.ps.FileName != null)
                            if (p.ps.FileName.EndsWith(".shproj"))
                                A.Add(node);
                    SetProjectExtension(node);
                    _projectCount++;
                }
            }

            P = SortNodes(P, true);

            A = SortNodes(A, false);

            foreach (TreeNode node in P)
            {
                master.Nodes.Remove(node);
                master.Nodes.Insert(0, node);

                ProjectItemInfo p = node.Tag as ProjectItemInfo;
                if (p != null)

                    if (p.psi != null)
                        if (p.psi.ItemType == "Project")
                        {
                            if (p.ps.Error != null)
                                node.Text += " (unavailble)";
                        }

                SortProjectNodes(node);
            }

            foreach (TreeNode node in A)
            {
                master.Nodes.Remove(node);
                master.Nodes.Add(node);
            }

            L = Folders2Array(master);

            L = SortNodes(L, true);

            //A = SortNodes(A, true);

            foreach (TreeNode node in L)
            {
                master.Nodes.Remove(node);

                string text = node.Text;

                if (Path.IsPathRooted(text) == true)
                {
                    string[] cc = text.Split("\\".ToCharArray());

                    if (cc.Length > 1)
                    {
                        text = cc[cc.Length - 1];
                        node.Text = text;
                    }
                }
                else
                {
                    master.Nodes.Insert(0, node);
                    SortProjectNodes(node);
                }
            }

            TreeNode ng = FindNodeExact(master, "Reference");

            if (ng != null)
            {
                master.Nodes.Remove(ng);
                master.Nodes.Insert(0, ng);

                foreach (TreeNode ns in ng.Nodes)
                {
                    string text = ns.Text;

                    string[] cc = text.Split(",".ToCharArray());

                    text = cc[0];

                    ns.Text = text;

                    if (Path.IsPathRooted(text))
                    {
                        ns.Text = Path.GetFileName(text);
                    }

                    ProjectItemInfo p = ns.Tag as ProjectItemInfo;
                    if (p != null)
                    {
                        if (p.psi != null)
                        {
                            string include = p.psi.Include;

                            if (include.EndsWith("proj"))
                            {
                                VSSolution vs = p.vs;

                                VSProject vp = p.ps;

                                string c = Path.GetDirectoryName(vp.FileName) + "\\" + include;

                                c = Path.GetFullPath(c);

                                if (vs.dd.ContainsKey(c) == true)
                                {
                                    VSProject b = vs.dd[c];//vs.GetProjectbyFileName(c);

                                    if (b != null)
                                    {
                                        string s = b.OutputName;
                                        ns.Text = Path.GetFileName(s);
                                        ns.ImageKey = "reference_16xLG";
                                        ns.SelectedImageKey = "reference_16xLG";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            ng = FindNodeExact(master, "Properties");

            if (ng != null)
            {
                master.Nodes.Remove(ng);
                master.Nodes.Insert(0, ng);
            }
        }

        public class json
        {
            public object supports { get; set; }

            public object dependencies { get; set; }

            public object frameworks { get; set; }
        }

        public void SetOutput(VSSolution vs)
        {
            vs.dd = new Dictionary<string, VSProject>();

            foreach (VSProject vp in vs.projects)
            {
                vs.dd[vp.FileName] = vp;

                if (DD.ContainsKey(vp.FileName) == false)
                    continue;

                TreeNode node = DD[vp.FileName];

                //if (vp.Name == "Composition")
                {
                    //MessageBox.Show("c");

                    TreeNode ns = FindNodeExact(node, "Reference");

                    ProjectItemInfo p = null;

                    ArrayList L = vp.GetItems("_ExplicitReference", false);

                    if (L.Count > 0)
                    {
                        if (ns == null)
                        {
                            ns = new TreeNode("Reference");
                            p = new ProjectItemInfo();
                            p.vs = vs;
                            p.ps = vp;
                            p.psi = new VSProjectItem();
                            ns.ImageKey = "reference_16xLG";
                            ns.SelectedImageKey = "reference_16xLG";

                            ns.Tag = p;
                            node.Nodes.Add(ns);
                        }

                        foreach (string c in L)
                        {
                            TreeNode ng = new TreeNode(c);
                            p = new ProjectItemInfo();
                            p.vs = vs;
                            p.ps = vp;
                            p.psi = new VSProjectItem();
                            p.psi.ItemType = "Reference";
                            p.psi.Include = c;

                            ng.ImageKey = "reference_16xLG";
                            ng.SelectedImageKey = "reference_16xLG";
                            ng.Tag = p;
                            ns.Nodes.Add(ng);
                        }
                    }

                    ArrayList P = vp.GetItems("Content", true);

                    foreach (string c in P)
                    {
                        if (File.Exists(c) == false)
                            continue;
                        if (c.EndsWith(".json") == false)
                            continue;

                        string content = File.ReadAllText(c);

                        json js = JsonConvert.DeserializeObject<json>(content);

                        if (js.dependencies == null)
                            continue;

                        Dictionary<string, string> g = JsonConvert.DeserializeObject<Dictionary<string, string>>((string)js.dependencies.ToString());

                        foreach (string s in g.Keys)
                        {
                            if (ns == null)
                            {
                                ns = new TreeNode("Reference");

                                p = new ProjectItemInfo();
                                {
                                    p.vs = vs;
                                    p.ps = vp;
                                    p.psi = new VSProjectItem();
                                    ns.ImageKey = "reference_16xLG";
                                    ns.SelectedImageKey = "reference_16xLG";

                                    ns.Tag = p;
                                    node.Nodes.Add(ns);
                                }
                            }

                            TreeNode ng = new TreeNode(s);
                            p = new ProjectItemInfo();
                            p.vs = vs;
                            p.ps = vp;
                            p.psi = new VSProjectItem();
                            p.psi.ItemType = "Reference";
                            p.psi.Include = s;

                            ng.ImageKey = "nuget";
                            ng.SelectedImageKey = "nuget";
                            ng.Tag = p;
                            ns.Nodes.Add(ng);
                        }
                    }
                }
                //string s = vp.GetProjectExec();
                //s = Path.GetFileName(s);
                //vp.OutputName = s;
            }
        }

        static public void SortFolderNodes(TreeNode master)
        {
            string name = master.ImageKey;

            if (name == "Folder")
            {
                ArrayList L = Nodes2Array(master);

                ArrayList F = new ArrayList();

                ArrayList N = new ArrayList();

                foreach (TreeNode node in L)
                {
                    if (node.ImageKey == "Folder")
                    {
                        F.Add(node);
                    }
                    else
                    {
                        N.Add(node);
                    }

                    master.Nodes.Remove(node);
                }

                N = SortNodes(N);

                foreach (TreeNode node in N)
                {
                    master.Nodes.Add(node);
                }

                F = SortNodes(F, true);

                foreach (TreeNode node in F)
                {
                    master.Nodes.Remove(node);

                    string text = node.Text;

                    if (Path.IsPathRooted(text.Trim() + "\\") == true)
                        continue;

                    master.Nodes.Insert(0, node);
                    node.ImageKey = "Folder";
                    node.SelectedImageKey = "Folder";
                }
            }

            foreach (TreeNode ns in master.Nodes)
                SortFolderNodes(ns);
        }

        static public void LoadNonFolders(TreeNode node, ArrayList L)
        {
            foreach (TreeNode nodes in node.Nodes)
            {
                if (nodes.ImageKey != "Folder")
                    L.Add(node);

                LoadNonFolders(nodes, L);
            }
        }

        static public void SortProjectNodes(TreeView master)
        {
            ArrayList L = Nodes2Array(master);

            L = SortNodes(L);

            //ArrayList S = new ArrayList();

            master.Nodes.Clear();

            ArrayList N = new ArrayList();

            foreach (TreeNode node in L)
            {
                // master.Nodes.Add(node);

                if (node.ImageKey != "Folder")
                    N.Add(node);

                //LoadNonFolders(node, N);
            }

            //= Folders2Array(master);

            //L = SortNodes(L);

            //foreach (TreeNode node in L)
            //{
            //    master.Nodes.Remove(node);
            //    master.Nodes.Insert(0, node);
            //}

            N = SortNodes(N);

            master.Nodes.Clear();

            foreach (TreeNode node in N)
            {
                master.Nodes.Add(node);
            }
        }

        static public ArrayList SortNodes(ArrayList L)
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

        static public ArrayList SortNodes(ArrayList L, bool reverse)
        {
            ArrayList S = new ArrayList();

            foreach (TreeNode node in L)
            {
                S.Add(node.Text);
            }

            S.Sort();

            if (reverse == true)

                S.Reverse();

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
            string[] gg = node.Text.Split("\\".ToCharArray());

            if (gg.Length > 0)
                node.Text = gg[gg.Length - 1];

            if (dc != null)
            {
                string[] s = node.Text.Split(".".ToCharArray());
                if (s.Count() > 1)
                {
                    int N = s.Count();

                    string exts = "." + s[N - 1];

                    exts = exts.ToLower();

                    ProjectItemInfo p = node.Tag as ProjectItemInfo;

                    if (exts != ".resx")

                    {
                        if (dc.ContainsKey(exts))
                        {
                            string dd = (string)dc[exts];
                            if (dd != "")
                            {
                                if (p != null)
                                    if (p.psi.ItemType == "Reference")
                                        return;

                                if (exts == ".cs")
                                {
                                    if (p != null)
                                    {
                                        if (p.psi != null)
                                            if (p.psi.SubTypes == "Component")
                                            {
                                                node.ImageKey = "component";
                                                node.SelectedImageKey = "component";
                                            }
                                            else if (p.psi.SubTypes == "UserControl")
                                            {
                                                node.ImageKey = "usercontrol";
                                                node.SelectedImageKey = "usercontrol";
                                            }
                                            else if (p.psi.SubTypes == "Form" || p.psi.SubTypes == "Forms")
                                            {
                                                node.ImageKey = "forms";
                                                node.SelectedImageKey = "forms";
                                            }
                                            else if (node.Text.ToLower().Contains("designer"))
                                            {
                                                node.ImageKey = "filesource";
                                                node.SelectedImageKey = "filesource";
                                            }
                                            else
                                            {
                                                node.ImageKey = dd;
                                                node.SelectedImageKey = dd;
                                            }
                                    }
                                }
                                else
                                {
                                    node.ImageKey = dd;
                                    node.SelectedImageKey = dd;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (p.psi.SubType == "resx")
                        {
                            node.ImageKey = "filesource";
                            node.SelectedImageKey = "filesource";
                        }
                        else
                        {
                            node.ImageKey = "resourcefile";
                            node.SelectedImageKey = "resourcefile";
                        }
                    }
                }
                //else
                //{
                //    string filename = GetFileName(node);
                //    if (filename == "")
                //        return;
                //    string exts = Path.GetExtension(filename);
                //    if (exts == "")
                //        return;
                //    if (exts == null)
                //        return;

                //    if (dc.ContainsKey(exts))
                //    {
                //        string dd = (string)dc[exts];
                //        if (dd != "")
                //        {
                //            node.ImageKey = dd;
                //            node.SelectedImageKey = dd;
                //        }
                //    }
                //    //node.ImageKey = "Folder";
                //    //node.SelectedImageKey = "Folder";
                //}
            }

            foreach (TreeNode nodes in node.Nodes)
                SetExtensions(nodes);
        }

        public void SetProjectExtension(TreeNode node)
        {
            if (dc == null)
                return;
            {
                string filename = GetFileName(node);
                if (filename == "")
                    return;
                string exts = Path.GetExtension(filename);
                if (exts == "")
                    return;
                if (exts == null)
                    return;

                if (dc.ContainsKey(exts))
                {
                    string dd = (string)dc[exts];
                    if (dd != "")
                    {
                        node.ImageKey = dd;
                        node.SelectedImageKey = dd;
                    }
                }
                //node.ImageKey = "Folder";
                //node.SelectedImageKey = "Folder";
            }
        }

        public void ResourceNode(TreeNode master)
        {
            TreeNode ns = FindNode(master, "Properties");

            if (ns != null)
            {
                ns.ImageKey = "properties_16xLG";
                ns.SelectedImageKey = "properties_16xLG";
            }

            TreeNode nodes = FindNode(master, "Resources");

            if (nodes == null)
                return;

            foreach (TreeNode node in nodes.Nodes)
            {
                string[] bb = node.Text.Split(".".ToCharArray());
                if (bb.Length > 1)
                {
                    node.ImageKey = "filesource";
                    node.SelectedImageKey = "filesource";
                }
                else
                {
                    node.ImageKey = "Folder";
                    node.SelectedImageKey = "Folder";
                }
            }
        }

        public void RemoveDuplicates(TreeNode node)
        {
            ArrayList L = Nodes2Array(node);

            int N = L.Count;

            int i = 0;
            while (i < L.Count)
            {
                TreeNode ns = L[i] as TreeNode;

                ProjectItemInfo p0 = ns.Tag as ProjectItemInfo;

                int j = i + 1;
                while (j < L.Count)
                {
                    TreeNode ng = L[j] as TreeNode;

                    ProjectItemInfo p1 = ng.Tag as ProjectItemInfo;

                    if (p0.Include == p1.Include)
                    {

                        if (ng.Text == ns.Text)
                            if(!ng.Equals(ns))
                            L.Remove(ng);

                        if (ng.Text == "")
                            L.Remove(ng);
                        if (ng.Text == "..")
                            L.Remove(ng);
                        //if (ng.Text == "bin")
                        //    L.Remove(ng);
                        if (ng.Text == "obj")
                            L.Remove(ng);
                        if (ng.Text.Length > 1)
                            if (ng.Text[1] == ':')
                                L.Remove(ng);
                    }
                    j++;
                }

                RemoveDuplicates(ns);

                i++;
            }

            if (L.Count == N)
                return;

            node.Nodes.Clear();
            i = 0;
            while (i < L.Count)
            {
                node.Nodes.Add(L[i] as TreeNode);
                i++;
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
                if (node.Text == "..")
                    nodes.Nodes.Remove(node);

                i++;
            }
            L = Nodes2Array(master);

            i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;
                if (node.Text == "")
                    nodes.Nodes.Remove(node);
                if (node.Text == "..")
                    if (node.Nodes.Count == 0)
                        nodes.Nodes.Remove(node);

                i++;
            }
        }

        public void MoveNode(TreeNode master, string name)
        {
            TreeNode node = FindNodeExact(master, name);

            if (node == null)
                return;

            string guid = "";
            ProjectItemInfo p = master.Tag as ProjectItemInfo;
            if (p != null)
                guid = p.Guid;
            string guids = "";
            ProjectItemInfo pp = node.Tag as ProjectItemInfo;
            if (pp != null)
                guids = pp.Guid;
            if (guid != "" && guids != "")
                if (guid != guids)
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

        public void ProjectForms(ArrayList EMB, TreeNode master)
        {
            ArrayList L = Designers(master);

            TreeNode nodes = FindNode(master, "EmbeddedResource");

            int i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;

                ProjectItemInfo nc = node.Tag as ProjectItemInfo;

                if (node.ImageKey == "Folder")
                {
                    i++;
                    continue;
                }

                string[] s = node.Text.Split(".".ToCharArray());

                TreeNode ns = FindNode(master, s[0]);

                //if (ns.ImageKey == "resourcefile")
                //{
                //    i++;
                //    continue;
                //}

                ProjectItemInfo nsc = ns.Tag as ProjectItemInfo;

                TreeNode ems = null;

                //if (nodes != null)
                //    ems = FindNode(nodes, s[0]);

                ProjectItemInfo pp = node.Tag as ProjectItemInfo;

                ProjectItemInfo pc = null;

                foreach (string b in EMB)
                {
                    string name = b.Replace(".resx", "");
                    string file = pp.psi.Include.Replace(".Designer.cs", "");

                    if (name == file)
                    {
                        pc = new ProjectItemInfo();
                        pc.vs = pp.vs;
                        pc.ps = pp.ps;
                        pc.psi = new VSProjectItem();
                        pc.psi.Include = b;
                        pc.psi.SubType = "EmbeddedResource";
                        pc.Include = b;

                        TreeNode nb = new TreeNode();
                        nb.Text = b;
                        nb.ImageKey = "filesource";// "resource_32xMD";
                        nb.SelectedImageKey = "filesource";// "resource_32xMD";
                        nb.Tag = pc;

                        ems = nb;
                    }
                }

                TreeNode ng = new TreeNode();

                ProjectItemInfo prs = new ProjectItemInfo();
                prs.psi = new VSProjectItem();
                prs.psi.SubTypes = pp.psi.SubTypes;
                prs.psi.Include = pp.psi.Include;
                prs.psi.SubType = "Form";
                prs.Include = pp.psi.Include;
                // if (pp.SubType != "")
                //     prs.psi.SubType = pp.SubType;

                if (nc != null)
                {
                    prs.ps = nc.ps;
                    prs.vs = nc.vs;
                }

                ng.Tag = prs;

                ng.Text = s[0] + ".cs";

                ng.ImageKey = "forms";
                ng.SelectedImageKey = "forms";

                if (nsc.psi.SubTypes == "UserControl")
                {
                    ng.ImageKey = "usercontrol";
                    ng.SelectedImageKey = "usercontrol";
                }

                if (prs.psi.SubTypes == "UserControl")
                {
                    ng.ImageKey = "usercontrol";
                    ng.SelectedImageKey = "usercontrol";
                }

                if (ems != null)
                    if (ns != null)
                        if (ems.Text == ns.Text)
                            ns = null;

                //if (ns.Text == "Resources")
                //{
                //    Type T = ns.Tag.GetType();
                //    ns = null;

                //}

                if (ns.Tag.GetType() == typeof(ProjectItemInfo))
                {
                    ProjectItemInfo psi = ns.Tag as ProjectItemInfo;

                    if (psi.psi.ItemType == "Folder")
                        ns = null;
                }

                if (node != null)
                    if (ems != null)
                        if (node.Text == ems.Text)
                            ems = null;

                if (node != null)
                    if (ns != null)
                        if (node.Text == ns.Text)
                            ns = null;

                //if(ems != null)
                //nodes.Nodes.Remove(ems);
                if (ns != null)
                    master.Nodes.Remove(ns);
                master.Nodes.Remove(node);

                if (ems != null)
                    ng.Nodes.Add(ems);

                if (ns != null)
                    ng.Nodes.Add(ns);

                ng.Nodes.Add(node);

                if (ng.Nodes.Count == 2)
                {
                    bool recursive = false;
                    bool isresx = false;
                    bool isdesigner = false;

                    string resfile = "";

                    TreeNode rec = null;

                    foreach (TreeNode nw in ng.Nodes)
                    {
                        if (nw.Text.Trim().EndsWith(".resx") == true)
                        {
                            resfile = nw.Text;
                            if (isresx == true)
                            {
                                rec = nw;
                                recursive = true;
                            }
                            isresx = true;
                        }
                        if (nw.Text.ToLower().Trim().EndsWith("designer.cs") == true)
                            isdesigner = true;
                    }

                    if (isresx == true)
                        if (isdesigner == true)
                        {
                            ng.ImageKey = "resourcefile";
                            ng.SelectedImageKey = "resourcefile";
                            ng.Text = resfile;
                        }

                    //if (recursive == true)
                    //{
                    //    rec.Parent.Nodes.Remove(rec);
                    //    ng = rec;

                    //}
                }

                master.Nodes.Add(ng);

                i++;
            }
        }

        public void ProjectSubForms(ArrayList EMB, TreeNode upper, TreeNode master, string folder)
        {
            ArrayList L = Designers(master);

            //TreeNode nodes = FindNode(master, "EmbeddedResource");

            //TreeNode nodes = null;// FindNode(masters, "EmbeddedResource");

            ArrayList P = Nodes2Array(master);

            if (master.Text.EndsWith(".cs") == true)
                return;

            if (master.Text.EndsWith(".resx") == true)
                return;

            foreach (TreeNode ng in P)
            {
                ProjectSubForms(EMB, master, ng, ng.Text);
            }

            int i = 0;
            while (i < L.Count)
            {
                TreeNode node = L[i] as TreeNode;

                string names = node.Text.Replace(folder + "\\", "");

                //string filename = names.Replace(".cs", "");
                string filename = names.Replace(".Designer", "");
                filename = filename.Replace(".designer", "");

                string[] s = names.Split(".".ToCharArray());

                ArrayList N = FindNodesExact(master, filename, node);

                //ArrayList N = FindNodes(master, s[0]);

                N.Remove(node);

                TreeNode ns = null;

                if (N.Count > 0)
                {
                    ns = N[0] as TreeNode;

                    //if (node.ImageKey == "resourcefile")
                    //{
                    //    i++;
                    //    continue;
                    //}
                }

                TreeNode ems = null;

                //if(N.Count > 1)
                //ems = N[1] as TreeNode;

                //if (nodes != null)
                //    ems = FindNode(nodes, s[0]);

                ProjectItemInfo pp = node.Tag as ProjectItemInfo;

                ProjectItemInfo pnp = null;

                if (ns != null)
                    pnp = ns.Tag as ProjectItemInfo;

                ProjectItemInfo pc = null;

                foreach (string b in EMB)
                {
                    string name = b.Replace(".resx", "");
                    string file = pp.psi.Include.Replace(".cs", "");
                    file = file.Replace(".Designer", "");
                    file = file.Replace(".designer", "");

                    name = name.ToLower();
                    file = file.ToLower();

                    if (name == file)
                    {
                        pc = new ProjectItemInfo();
                        pc.Include = b;
                        pc.vs = pp.vs;
                        pc.ps = pp.ps;
                        pc.psi = new VSProjectItem();
                        pc.psi.Include = b;
                        pc.psi.ItemType = "EmbeddedResource";
                        pc.psi.SubType = "EmbeddedResource";

                        TreeNode nb = new TreeNode();

                        string[] bc = b.Split("\\".ToCharArray());

                        nb.Text = bc[bc.Length - 1];

                        //nb.Text = b;

                        nb.ImageKey = "filesource";
                        nb.SelectedImageKey = "filesource";
                        nb.Tag = pc;

                        ems = nb;
                    }
                }

                //else
                //{
                //    ArrayList B = FindNodes(master, s[0]);

                //    foreach(TreeNode bc in B)
                //        if(bc != ns )
                //            if(bc != node)
                //    ems = bc;
                //}

                string[] cc = node.Text.Split(".".ToCharArray());

                TreeNode ng = new TreeNode();
                ng.ImageKey = "forms";
                ng.SelectedImageKey = "forms";

                string named = pp.psi.Include.Replace(".resx", "");
                named = named.Replace(".cs", "");
                named = named.Replace(".Designer", "");
                named = named.Replace(".designer", "");

                ProjectItemInfo pcc = new ProjectItemInfo();
                pcc.vs = pp.vs;
                pcc.ps = pp.ps;
                pcc.psi = new VSProjectItem();
                pcc.psi.Include = pp.psi.Include; // named;
                pcc.Include = pp.psi.Include;
                pcc.psi.SubTypes = pp.psi.SubTypes;
                //pcc.psi.SubType = pp.SubType;

                if (pp.psi.SubTypes == "UserControl")
                {
                    ng.ImageKey = "usercontrol";
                    ng.SelectedImageKey = "usercontrol";
                }
                if (pp.psi.SubTypes == "Component")
                {
                    ng.ImageKey = "component";
                    ng.SelectedImageKey = "component";
                }
                if (pnp != null)
                    if (pnp.psi.SubTypes == "UserControl")
                    {
                        ng.ImageKey = "usercontrol";
                        ng.SelectedImageKey = "usercontrol";
                    }
                pcc.psi.ItemType = "Forms";
                pcc.psi.SubType = "Forms";

                ng.Tag = pcc;

                if (ems != null)
                    master.Nodes.Remove(ems);
                //if(nodes != null)
                //nodes.Nodes.Remove(ems);
                if (ns != null)
                    master.Nodes.Remove(ns);

                master.Nodes.Remove(node);

                upper.Nodes.Remove(node);

                node.Nodes.Clear();

                if (ems != null)
                {
                    if (node != null)
                        if (node.Text != ems.Text)
                        {
                            ng.Nodes.Add(ems);
                            ems.Nodes.Clear();
                        }
                }
                if (ns != null)
                {
                    if (ems != null) if (ns.Text == ems.Text) continue;
                    if (node.Text == ns.Text) continue;
                    {
                        ng.Nodes.Add(ns);
                        ns.Nodes.Clear();
                    }
                }

                if (ns != null)
                    ng.Text = cc[0];
                else
                    if (ems != null)
                    ng.Text = cc[0];
                else ng.Text = cc[0];

                ng.Nodes.Add(node);
                node.Nodes.Clear();

                if (ng.Nodes.Count <= 1)
                {
                    ng.Text = ng.Nodes[0].Text;
                    ng.Nodes.Clear();
                }
                if (ng.Nodes.Count == 2)
                {
                    bool isresx = false;
                    bool isdesigner = false;
                    //TreeNode nw = null;

                    string resfile = "";

                    foreach (TreeNode nc in ng.Nodes)
                    {
                        if (nc.Text.Trim().ToLower().EndsWith(".resx") == true)
                        {
                            //if (isresx == true)
                            //    nw = nc;
                            isresx = true;
                            resfile = nc.Text;
                        }
                        if (nc.Text.ToLower().Trim().EndsWith("designer.cs") == true)
                            isdesigner = true;
                    }

                    if (isresx == true)
                        if (isdesigner == true)
                        {
                            ng.ImageKey = "resourcefile";
                            ng.SelectedImageKey = "resourcefile";
                            ng.Text = resfile;
                        }
                    //if (nw != null)
                    //    ng = nw;
                }

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

        public static TreeNode FindNodeExact(TreeNode master, string name)
        {
            name = name.ToUpper().Trim();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper().Trim() == name)
                    return node;
            }

            return null;
        }

        public TreeNode FindNodeExactTree(TreeNode master, string name)
        {
            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper() == name)
                    return node;
                TreeNode ng = FindNodeExactTree(node, name);
                if (ng != null)
                    return ng;
            }

            return null;
        }

        public void FindNodeExactTree(TreeNode master, string name, ArrayList N)
        {
            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper() == name)
                    N.Add(node);
                FindNodeExactTree(node, name, N);
            }
        }

        public TreeNode FindNodesInclude(TreeNode master, string Include)
        {
            Include = Include.ToUpper();

            string[] cc = Include.Split("\\".ToCharArray());

            int i = 0;
            foreach (string c in cc)
            {
                if (i >= cc.Length - 1)
                    return master;
                master = FindNodeExact(master, c);

                if (master == null)
                    return master;
                i++;
            }

            return null;
        }

        public TreeNode FindNodesIncludeExact(TreeView v, string Include)
        {
            Include = Include.ToUpper();

            string[] cc = Include.Split("\\".ToCharArray());

            TreeNode ns = null;

            foreach (TreeNode master in v.Nodes)
            {
                ns = master;
                int i = 0;
                foreach (string c in cc)
                {
                    i++;

                    ns = FindNodeExact(ns, c);

                    if (ns == null)
                        continue;

                    if (i >= cc.Length)
                        if (ns != null)
                            return ns;
                }
            }
            return null;
        }

        public TreeNode GetProjectNode(TreeNode n, VSProject vp)
        {
            foreach (TreeNode ns in n.Nodes)
            {
                ProjectItemInfo p = ns.Tag as ProjectItemInfo;

                if (p != null)
                {
                    if(p.IsProjectNode)
                    if (p.ps != null)
                    {
                        if (p.ps.Name == vp.Name)
                                return ns;
                    }
                }
                TreeNode ng = GetProjectNode(ns, vp);

                if (ng != null)
                    return ng;
            }

            return null;
        }

        public TreeNode GetProjectNode(TreeView v, VSProject vp)
        {
            foreach (TreeNode ns in v.Nodes)
            {
                ProjectItemInfo p = ns.Tag as ProjectItemInfo;

                if (p != null)
                {
                    if(p.IsProjectNode)
                        

                    if (p.ps != null)
                    {
                        if (p.ps.Name == vp.Name)
                            return ns;
                    }
                }

                TreeNode ng = GetProjectNode(ns, vp);

                if (ng != null)
                    return ng;
            }

            return null;
        }

        public TreeNode FindNodesIncludeExact(TreeNode master, string Include)
        {
            Include = Include.ToUpper();

            string[] cc = Include.Split("\\".ToCharArray());

            int i = 0;
            foreach (string c in cc)
            {
                master = FindNodeExact(master, c);

                if (master == null)
                    return master;

                i++;

                if (i >= cc.Length)
                    return master;
            }

            return null;
        }

        public TreeNode FindNodesIncludes(TreeNode master, string Include)
        {
            Include = Include.ToUpper().Trim();

            ProjectItemInfo p = master.Tag as ProjectItemInfo;

            if (p != null)
            {
                if (p.Include != null)
                {
                    string include = p.Include.ToUpper().Trim();

                    if (include.Contains(Include))
                        return master;
                }
            }

                foreach (TreeNode ns in master.Nodes)
            {

                p = ns.Tag as ProjectItemInfo;

                if (p != null)
                {
                    if (p.Include != null)
                    {
                        string include = p.Include.ToUpper().Trim();

                        if (include.Contains( Include))
                            return ns;
                    }

                    TreeNode ng = FindNodesIncludes(ns, Include);

                    if (ng != null)
                        return ng;
                }
            }
            return null;
        }

        public ArrayList FindNodesExacts(TreeNode master, string name, ArrayList L)
        {
            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.ToUpper().Contains(name) == true)
                    L.Add(node);

                FindNodesExacts(node, name, L);
            }

            return L;
        }
        public ArrayList FindNodesByInclude(TreeView master, string name, ArrayList L)
        {
            name = name.ToUpper();

            ProjectItemInfo p0 = master.Tag as ProjectItemInfo;

            if (!string.IsNullOrEmpty(p0.Include))
                if (p0.Include.ToUpper() == name)
                L.Add(master);

            foreach (TreeNode node in master.Nodes)
            {
                // if (node.Text.ToUpper().Contains(name) == true)
                //     L.Add(node);

                ProjectItemInfo p1 = node.Tag as ProjectItemInfo;
                if (!string.IsNullOrEmpty(p1.Include))
                    if (p1.Include.ToUpper() == name)
                    L.Add(master);

                FindNodesByInclude(node, name, L);
            }

            return L;
        }
        public ArrayList FindNodesByInclude(TreeNode master, string name, ArrayList L)
        {
            name = name.ToUpper();

            ProjectItemInfo p0 = master.Tag as ProjectItemInfo;

            if(!string.IsNullOrEmpty(p0.Include))
            if (p0.Include.ToUpper().Contains(name))
                L.Add(master);

            foreach (TreeNode node in master.Nodes)
            {
               // if (node.Text.ToUpper().Contains(name) == true)
               //     L.Add(node);

                ProjectItemInfo p1= node.Tag as ProjectItemInfo;

                if (!string.IsNullOrEmpty(p1.Include))
                    if (p1.Include.ToUpper().Contains(name))
                    L.Add(master);

                FindNodesByInclude(node, name, L);
            }

            return L;
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

        public ArrayList FindNodesExact(TreeNode master, string name, TreeNode nodes)
        {
            string[] c = name.Split(".".ToCharArray());

            string names = name.Replace(" - Compile", ""); ;// c[0].Replace(" - Compile", "");

            ArrayList L = new ArrayList();

            name = name.ToUpper();

            foreach (TreeNode node in master.Nodes)
            {
                if (node == nodes)
                    continue;

                string text = node.Text;

                string[] cc = text.Split(".".ToCharArray());

                string g = text.Replace(" - Compile", ""); //cc[0].Replace(" - Compile", "");

                if (g.ToUpper() == names.ToUpper())
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

                ProjectItemInfo p = node.Tag as ProjectItemInfo;

                if (p.psi == null)
                    L.Add(node);
            }

            return L;
        }

        public ArrayList Designers(TreeNode master)
        {
            ArrayList L = new ArrayList(0);

            ArrayList B = new ArrayList();

            foreach (TreeNode node in master.Nodes)
            {
                if (node.Text.Contains("\\"))
                    continue;

                string text = node.Text;

                string[] bb = node.Text.Split(".".ToCharArray());

                string name = bb[0];

                if (text.ToLower().Contains(".designer.") == true)
                {
                    L.Add(node);

                    B.Add(name);

                    if (text.ToLower().EndsWith(".resx") == true)
                    {
                        string[] c = text.ToLower().Split(".".ToCharArray());

                        ArrayList N = FindNodes(master, c[0].ToLower());

                        foreach (TreeNode ns in N)
                        {
                            //    if (ns == null)
                            //        continue;

                            //    string texts = ns.Text.Replace(" - Compile", "");

                            //    string[] cc = texts.Split(".".ToCharArray());

                            //    if(cc[0] == c[0])

                            if (ns.Text.ToLower().EndsWith(".resx") == false)
                            {
                                L.Add(ns);
                                break;
                            }
                        }
                    }
                    //else
                    //{
                    //    ProjectItemInfo p = node.Tag as ProjectItemInfo;

                    //    if (p == null)
                    //        continue;

                    //    VSProjectItem pi = p.psi;

                    //    if (pi == null)
                    //        continue;

                    //    if (pi.SubTypes != null)
                    //        if (pi.SubTypes != "")
                    //        {
                    //            if (pi.SubTypes == "Form" || pi.SubTypes == "UserControl" || pi.SubTypes == "Component")

                    //                L.Add(node);
                    //        }
                    //}
                }
            }

            foreach (TreeNode node in master.Nodes)
            {
                int i = L.LastIndexOf(node);

                if (i >= 0)
                    continue;
                bool d = false;
                foreach (string s in B)
                    if (node.Text.Trim().StartsWith(s) == true)
                        d = true;

                if (d == true)
                    continue;

                ProjectItemInfo p = node.Tag as ProjectItemInfo;

                if (p == null)
                    continue;

                VSProjectItem pi = p.psi;

                if (pi == null)
                    continue;

                if (pi.SubTypes != null)
                    if (pi.SubTypes != "")
                    {
                        if (pi.SubTypes == "Form" || pi.SubTypes == "UserControl" || pi.SubTypes == "Component")

                            L.Add(node);
                    }
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

                node.ImageKey = "Folder";
                node.SelectedImageKey = "FolderOpen";

                if (node.Text == "Forms")
                {
                    master.Nodes.Remove(node);
                    continue;
                }

                //if (node.Text == "Parser")
                //    MessageBox.Show("");

                master.Nodes.Remove(node);

                RemoveEmptyNodes(node);

                TreeNode nodes = FindNode(master, node.Text);

                //TreeNode nodes = FindNodeExact(master, node.Text);

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

                if (nodes != null)
                    ExtractSubFolders(master, nodes, nodes.Text);
                //              else
                //                  ExtractSubFolders(master, node, node.Text);
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

        static public ArrayList Nodes2Array(TreeNode node)
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

        static public ArrayList Nodes2Array(TreeNode node, ArrayList N)
        {
            if (node == null)
                return N;

            int i = 0;

            while (i < node.Nodes.Count)
            {
                if (node.Nodes[i].Text != "Reference")
                    if (node.Nodes[i].Text != "Properties")
                    {
                        N.Add(node.Nodes[i]);

                        Nodes2Array(node.Nodes[i], N);
                    }
                i++;
            }

            return N;
        }

        static public ArrayList Nodes2Array(TreeView node)
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

        static public ArrayList Folders2Array(TreeNode node)
        {
            ArrayList L = new ArrayList();

            if (node == null)
                return L;

            int i = 0;

            while (i < node.Nodes.Count)
            {
                if (node.Nodes[i].ImageKey == "Folder")
                    L.Add(node.Nodes[i]);

                i++;
            }

            return L;
        }

        static public ArrayList Folders2Array(TreeView node)
        {
            ArrayList L = new ArrayList();

            if (node == null)
                return L;

            int i = 0;

            while (i < node.Nodes.Count)
            {
                if (node.Nodes[i].ImageKey == "Folder")
                    L.Add(node.Nodes[i]);

                i++;
            }

            return L;
        }

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public void ExtractSubFolders(TreeNode master, TreeNode node, string folder)
        {
            //if(node.Text.Contains("VBNet"))
            //MessageBox.Show("Found");
            //if (folder.StartsWith("Parser"))
            //    MessageBox.Show("Found");

            Dictionary<string, ArrayList> dict = new Dictionary<string, ArrayList>();

            ArrayList FD = new ArrayList();

            ArrayList PL = new ArrayList();

            foreach (TreeNode ns in node.Nodes)
            {
                string s = ns.Text;

                //if (s.Contains("VBNetParser"))
                //    MessageBox.Show("");

                string[] cc = s.Split("\\".ToCharArray());

                //if(cc.Length > 1)
                if (folder != "")
                    if (s.StartsWith(folder))
                        s = ReplaceFirst(s, folder + "\\", "");

                if (s.StartsWith("\\"))
                    s = s.Remove(0, 1);

                s = s.Replace("- Compile", "");

                string[] bc = s.Split("\\".ToCharArray());

                ns.Text = bc[bc.Length - 1];

                cc = s.Split("\\".ToCharArray());

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
            {
                return;
            }

            int c = 0;

            foreach (string key in dict.Keys)
            {
                //if (key == "Parser5")
                //    MessageBox.Show("Parser5");

                TreeNode nods = new TreeNode();
                nods.Text = key;
                nods.ImageKey = "Folder";
                nods.SelectedImageKey = "FolderOpen";

                ProjectItemInfo prs = node.Tag as ProjectItemInfo;

                ProjectItemInfo pc = new ProjectItemInfo();

                string name = "";

                if (prs != null)
                {
                    pc.vs = prs.vs;
                    pc.ps = prs.ps;
                    if (prs.psi.Include != "")
                        name = prs.psi.Include.Replace(prs.psi.Name, "");
                }
                pc.psi = new VSProjectItem();
                pc.psi.SubType = "Folder";
                pc.psi.ItemType = "Folder";

                if (prs.psi.SubType == "Folder")

                    pc.psi.Include = prs.psi.Include + "\\" + key;
                else
                {
                    if (name != "")
                        if (name.EndsWith("\\") == false)
                            name = name + "\\";
                    pc.psi.Include = name + key;
                }

                nods.Tag = pc;

                ArrayList L = dict[key];

                int i = 0;
                while (i < L.Count)
                {
                    TreeNode ng = L[i] as TreeNode;

                    TreeNode ns = LoadSubfolders(ng, nods, key, node);

                    if (ns == null)
                    {
                        string[] bc = ng.Text.Split("\\".ToCharArray());

                        ng.Text = bc[bc.Length - 1];

                        node.Nodes.Remove(ng);

                        if (ng.Text != "" && ng.Text != null)

                            nods.Nodes.Add(ng);
                    }

                    i++;
                }

                node.Nodes.Insert(c, nods);

                if (key != folder)
                {
                    ExtractSubFolders(master, nods, key);
                    MergeTheSame(nods);
                }
                else
                {
                    ExtractSubFolders(master, node, node.Text);
                    MergeTheSame(node);
                }
                c++;
            }

            MergeTheSame(node);
        }

        public TreeNode LoadSubfolders(TreeNode ng, TreeNode nods, string key, TreeNode nodes)
        {
            ProjectItemInfo prs = ng.Tag as ProjectItemInfo;

            if (prs == null)
                return ng;

            if (prs.psi == null)
                return ng;

            string Include = prs.psi.Include;

            ProjectItemInfo pp = nods.Tag as ProjectItemInfo;

            string bb = pp.psi.Include;

            Include = ReplaceFirst(Include, bb + "\\", "");

            ng.Text = Include;

            string[] cc = Include.Split("\\".ToCharArray());

            if (cc.Length <= 1)
                return null;

            string name = bb;

            TreeNode nc = nods;

            TreeNode first = nods;

            int i = 0;
            while (i < cc.Length - 1)
            {
                TreeNode b = new TreeNode();
                b.SelectedImageKey = "FolderOpen";
                b.ImageKey = "Folder";

                b.Text = cc[i];

                name += "\\" + cc[i];

                TreeNode s = FindNodesInclude(nods, name);

                if (s != null)

                    b = s;
                else
                {
                    first.Nodes.Add(b);

                    ProjectItemInfo pc = new ProjectItemInfo();
                    pc.psi = new VSProjectItem();
                    pc.psi.SubType = "Folder";
                    pc.psi.ItemType = "Folder";
                    pc.psi.Include = name;

                    pc.ps = pp.ps;

                    b.Tag = pc;
                }
                nc = b;

                first = b;

                i++;
            }

            bb = prs.psi.Include;

            //  Include = bb.Replace(name + "\\", "");

            ng.Text = bb.Replace(name + "\\", "");

            nodes.Nodes.Remove(ng);

            first.Nodes.Add(ng);

            return ng;
        }

        public void MergeTheSame(TreeNode node)
        {
            ArrayList L = Nodes2Array(node);

            int i = 0;
            while (i < L.Count)
            {
                TreeNode ns = L[i] as TreeNode;

                string c = ns.Text.Trim().Replace(" - Compile", "");

                ArrayList N = FindNodesExact(node, c, null);

                //ArrayList N = FindNodes(node, ns.Text);

                if (N.Count > 1)

                {
                    int g = 0;
                    while (g < N.Count)
                    {
                        TreeNode ng = N[g] as TreeNode;

                        //if(ng.Text.Contains(".cs") == false)

                        if (ng != ns)
                        {
                            node.Nodes.Remove(ng);

                            foreach (TreeNode b in ng.Nodes)
                                ns.Nodes.Add(b);
                        }

                        g++;
                    }
                }

                MergeTheSame(ns);

                i++;
            }
        }

        public void MergeTheSameForms(TreeNode node)
        {
            if (node.Nodes.Count != 1)
                return;

            TreeNode nodes = node.Nodes[0];

            string[] cc = node.Text.Split("\\".ToCharArray());

            string c = cc[cc.Length - 1];

            string[] bb = nodes.Text.Split("\\".ToCharArray());

            string b = bb[bb.Length - 1];

            c = Path.GetFileNameWithoutExtension(c);

            b = Path.GetFileNameWithoutExtension(b);

            if (c != b)
                return;

            ArrayList N = Nodes2Array(nodes);

            node.Nodes.Remove(nodes);

            foreach (TreeNode ns in N)
            {
                nodes.Nodes.Remove(ns);
                node.Nodes.Add(ns);
            }
        }

        public static void SetTreeView(TreeView tv)
        {
            if (tv.ImageKey != "Folder")
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
                if (node.ImageKey != "Folder"/*"Folder"*/)
                {
                    node.ImageKey = "CSharpProject_SolutionExplorerNode_24";
                    node.SelectedImageKey = "CSharpProject_SolutionExplorerNode_24";
                }
            }
            else if (node.ImageKey == "Folder" /*"FolderOpen_7140_24"*/)
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

    public class VSExplorer
    {
        public void LoadSolution(string file)
        {
        }

        static public ImageList addImage(Bitmap bmp, string name, ImageList imgL)
        {
            if (imgL == null)
                imgL = new ImageList();

            if (bmp != null)
            {
                imgL.Images.Add(name, (Image)bmp);
            }

            return imgL;
        }

        public static Dictionary<string, object> Load(string ase, string res)
        {
            Dictionary<string, object> dicAttributes = new Dictionary<string, object>();

            ArrayList L = new ArrayList();

            ArrayList V = new ArrayList();

            Assembly assem = Assembly.LoadFile(ase);

            string[] rs = assem.GetManifestResourceNames();

            Stream r = assem.GetManifestResourceStream(res/*"SE-Project.resources-vs.resources"*/);

            System.Resources.ResourceReader mg = new System.Resources.ResourceReader(r);

            foreach (var item in mg)
            {
                dicAttributes.Add(((System.Collections.DictionaryEntry)(item)).Key.ToString(), ((System.Collections.DictionaryEntry)(item)).Value);

                L.Add(((System.Collections.DictionaryEntry)(item)).Key.ToString());

                V.Add(((System.Collections.DictionaryEntry)(item)).Value);
            }

            return dicAttributes;
        }

        static public ImageList CreateImageList(string s)
        {
            Dictionary<string, object> dict;

            Dictionary<string, object> dict2;

            Dictionary<string, object> dict3;

            ImageList imageList1 = new ImageList();

            if (!File.Exists(s + "\\" + "SE-Project.exe"))
                throw new FileNotFoundException("File does not exists - " + s + "\\" + "SE-Project.exe");

            dict = null;
            if (dict == null)
            {
                dict = Load(s + "\\" + "SE-Project.exe", "SE-Project.resources-vs.resources");

                foreach (string str in dict.Keys)
                {
                    if (dict[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict[str], str, imageList1);
                }
            }

            dict2 = null;
            if (dict2 == null)
            {
                dict2 = Load(s + "\\" + "SE-Project.exe", "SE-Project.resource-vsc.resources");

                foreach (string str in dict2.Keys)
                {
                    if (dict2[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict2[str], str, imageList1);
                }
            }
            dict3 = null;
            if (dict3 == null)
            {
                dict3 = Load(s + "\\" + "SE-Project.exe", "SE-Project.Resources_refs.resources");

                foreach (string str in dict3.Keys)
                {
                    if (dict3[str].GetType() == typeof(Bitmap))

                        addImage((Bitmap)dict3[str], str, imageList1);
                }

                //addImage(resource_alls.Folder, "Folder", imageList1);
                //addImage(resource_alls.FolderOpen, "FolderOpen", imageList1);

       
            }

            return imageList1;
        }

        static public Dictionary<string, string> dc { get; set; }

        static public VSSolution vs { get; set; }

        static public Dictionary<string, string> GetDC()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            d.Add(".vcproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".vcxproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".csproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".shfbproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wxs", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wixproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".cpp", "CPPFile_SolutionExplorerNode_24");
            //d.Add(".h", "Include_13490_24");
            //d.Add(".txt", "resource_32xMD");
            //d.Add(".rtf", "resource_32xMD");
            //d.Add(".targets", "resource_32xMD");
            //d.Add(".dll", "Library_6213");
            //d.Add(".xml", "XMLFile_828_16x");
            //d.Add(".xaml", "XMLFile_828_16x");
            //d.Add(".cur", "cursor_16xLG");
            //d.Add(".shproj", "shared");

            return d;
        }

        public void loads_vs_dict()
        {
            if (dc == null)
                dc = new Dictionary<string, string>();
            else
                return;

            Dictionary<string, string> d = dc;

            d.Add(".vcproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".vcxproj", "CPPProject_SolutionExplorerNode_24");
            d.Add(".csproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".shfbproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wxs", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".wixproj", "CSharpProject_SolutionExplorerNode_24");
            d.Add(".cpp", "CPPFile_SolutionExplorerNode_24");
            d.Add(".h", "Include_13490_24");
            d.Add(".txt", "resource_32xMD");
            d.Add(".rtf", "resource_32xMD");
            d.Add(".targets", "resource_32xMD");
            d.Add(".dll", "Library_6213");
            d.Add(".xml", "XMLFile_828_16x");
            d.Add(".xaml", "XMLFile_828_16x");
            d.Add(".cur", "cursor_16xLG");
            d.Add(".shproj", "shared");
            d.Add(".cd", "shared");

            d.Add("ASSEMBLY", "ASSEMBLY");
            d.Add("CLASS", "CLASS");
            d.Add("CLASS_FRIEND", "CLASS_FRIEND");
            d.Add("CLASS_PRIVATE", "CLASS_PRIVATE");
            d.Add("CLASS_PROTECTED", "CLASS_PROTECTED");
            d.Add("CLASS_SEALED", "CLASS_SEALED");
            d.Add("CONSTANT", "CONSTANT");
            d.Add("CONSTANT_FRIEND", "CONSTANT_FRIEND");
            d.Add("CONSTANT_PRIVATE", "CONSTANT_PRIVATE");
            d.Add("CONSTANT_PROTECTED", "CONSTANT_PROTECTED");
            d.Add("CONSTANT_SEALED", "CONSTANT_SEALED");
            d.Add("DELEGATE", "DELEGATE");
            d.Add("DELEGATE_FRIEND", "DELEGATE_FRIEND");
            d.Add("DELEGATE_PRIVATE", "DELEGATE_PRIVATE");
            d.Add("DELEGATE_PROTECTED", "DELEGATE_PROTECTED");
            d.Add("DELEGATE_SEALED", "DELEGATE_SEALED");
            d.Add("ENUM", "ENUM");
            d.Add("ENUM_FRIEND", "ENUM_FRIEND");
            d.Add("ENUM_PROTECTED", "ENUM_PROTECTED");
            d.Add("ENUM_SEALED", "ENUM_SEALED");
            d.Add("EVENT", "EVENT");
            d.Add("EVENT_FRIEND", "EVENT_FRIEND");
            d.Add("EVENT_PRIVATE", "EVENT_PRIVATE");
            d.Add("EVENT_PROTECTED", "EVENT_PROTECTED");
            d.Add("EVENT_SEALED", "EVENT_SEALED");
            d.Add("FIELD", "FIELD");
            d.Add("FIELD_FRIEND", "FIELD_FRIEND");
            d.Add("FIELD_PRIVATE", "FIELD_PRIVATE");
            d.Add("FIELD_PROTECTED", "FIELD_PROTECTED");
            d.Add("FIELD_SEALED", "FIELD_SEALED");
            d.Add("INTERFACE", "INTERFACE");
            d.Add("INTERFACE_FRIEND", "INTERFACE_FRIEND");
            d.Add("INTERFACE_PRIVATE", "INTERFACE_PRIVATE");
            d.Add("INTERFACE_PROTECTED", "INTERFACE_PROTECTED");
            d.Add("INTERFACE_SEALED", "INTERFACE_SEALED");
            d.Add("METHOD", "METHOD");
            d.Add("METHOD_FRIEND", "METHOD_FRIEND");
            d.Add("METHOD_PRIVATE", "METHOD_PRIVATE");
            d.Add("METHOD_PROTECTED", "METHOD_PROTECTED");
            d.Add("METHOD_SEALED", "METHOD_SEALED");
            d.Add("METHOD_EXTENSION", "METHOD_EXTENSION");
            d.Add("METHODOVERLOAD", "METHODOVERLOAD");
            d.Add("METHODOVERLOAD_FRIEND", "METHODOVERLOAD_FRIEND");
            d.Add("METHODOVERLOAD_PRIVATE", "METHODOVERLOAD_PRIVATE");
            d.Add("METHODOVERLOAD_PROTECTED", "METHODOVERLOAD_PROTECTED");
            d.Add("METHODOVERLOAD_SEALED", "METHODOVERLOAD_SEALED");
            d.Add("NAMESPACE", "NAMESPACE");
            d.Add("NAMESPACE_FRIEND", "NAMESPACE_FRIEND");
            d.Add("NAMESPACE_PRIVATE", "NAMESPACE_PRIVATE");
            d.Add("NAMESPACE_PROTECTED", "NAMESPACE_PROTECTED");
            d.Add("NAMESPACE_SEALED", "NAMESPACE_SEALED");
            d.Add("PROPERTIES", "PROPERTIES");
            d.Add("PROPERTIES_FRIEND", "PROPERTIES_FRIEND");
            d.Add("PROPERTIES_PRIVATE", "PROPERTIES_PRIVATE");
            d.Add("PROPERTIES_PROTECTED", "PROPERTIES_PROTECTED");
            d.Add("PROPERTIES_SEALED", "PROPERTIES_SEALED");
            d.Add("STRUCTURE", "STRUCTURE");
            d.Add("STRUCTURE_FRIEND", "STRUCTURE_FRIEND");
            d.Add("STRUCTURE_PRIVATE", "STRUCTURE_PRIVATE");
            d.Add("STRUCTURE_PROTECTED", "STRUCTURE_PROTECTED");
            d.Add("STRUCTURE_SEALED", "STRUCTURE_SEALED");
            d.Add("VALUETYPE", "VALUETYPE");
            d.Add("VALUETYPE_FRIEND", "VALUETYPE_FRIEND");
            d.Add("VALUETYPE_PRIVATE", "VALUETYPE_PRIVATE");
            d.Add("VALUETYPE_PROTECTED", "VALUETYPE_PROTECTED");
            d.Add("VALUETYPE_SEALED", "VALUETYPE_SEALED");
        }

        static public TreeView LoadProject(string file, string bb = "")
        {
            string s = AppDomain.CurrentDomain.BaseDirectory;

            if (bb != "")
                s = bb;

            ImageList imageList1 = CreateImageList(s);

            string b = AppDomain.CurrentDomain.BaseDirectory;

            TreeView msv = new TreeView();

            msv.Nodes.Clear();

            msv.ImageList = imageList1;

            msbuilder_alls ms = new msbuilder_alls();

            string p = Path.GetDirectoryName(file);

            Directory.SetCurrentDirectory(p);

            string g = Path.GetFileName(file);

            dc = GetDC();

            ms.dc = dc;

            VSSolution vs = ms.tester(msv, g, "");

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            if (ms.PL == null)
                return msv;

            foreach (ProjectItemInfo prs in ms.PL)
            {
                TreeNode nodes = new TreeNode();
                nodes.Text = prs.ps.Name;

                nodes.Tag = prs;
            }

            foreach (VSProject vp in vs.Projects)
            {
                vp.vs = vs;
            }

            return msv;
        }

        static public void Loads2(TreeView tv, TreeView s)
        {
            //sv._SolutionTreeView.ImageList = tv.ImageList;

            Font font = new Font(s.Font, FontStyle.Bold);

            if (tv == null)
                return;

            //s.BeginInvoke(new Action(() =>
            {
                s.Nodes.Clear();

                s.BeginUpdate();

                s.ImageList = tv.ImageList;

                s.Tag = tv.Tag;

                s.EndUpdate();
            };

            if (tv == null)
                return;

            int N = tv.Nodes.Count;

            ArrayList L = new ArrayList();

            int i = 0;
            while (i < N)
            {
                L.Add(tv.Nodes[i]);

                i++;
            }

            i = 0;
            while (i < N)
            {
                TreeNode ns = L[i] as TreeNode;
                object[] obs = new object[3];
                obs[0] = s;
                obs[1] = tv;
                obs[2] = ns;
                populateTreeNode(s, tv, ns);

                //ns.NodeFont = font;
                //tv.Nodes.Remove(ns);
                //s.Nodes.Add(ns);

                i++;
            }
        }

        static private void populateTreeNode(TreeView s, TreeView tv, TreeNode ns)
        {
            s.BeginInvoke(new Action(() =>
            {
                s.BeginUpdate();
                tv.Nodes.Remove(ns);
                s.Nodes.Add(ns);
                s.EndUpdate();
            }));
        }
    }
    public class ProjectItemInfo : object
    {
        [Category("File")]
        public string filepath { get; set; }
        [Category("File")]
        //public ClassMapper mapper { get; set; }
        //[Category("File")]
        public VSProvider.Project pg { get; set; }


        [Category("Projects")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VSProject ps { get; set; }
        [Category("Projects")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VSProjectItem psi { get; set; }
        [Category("Projects")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public VSSolution vs { get; set; }

        public bool isItem = true;

        public string SubType { get; set; }

        public bool IsProjectNode { get; set; }

        public string Guid { get; set; }

        public ProjectItemInfo()
        {
            IsProjectNode = false;
        }

        public bool IsItem()
        {
            return isItem;
        }
        public string Include { get; set; }
    }
}
