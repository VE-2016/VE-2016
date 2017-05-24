using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace MSTestLogger
{
    [ExtensionUri("logger://MessageLogger/v1")]
    [FriendlyName("MessageLogger")]
    public class DiscoveryMessage : IMessageLogger
    {
        public void SendMessage(TestMessageLevel b, string s) 
        {
            File.WriteAllText("test.test", "Discovery");

        }
        
    }


    /// <summary>

    /// Logger for sending output to the console.

    /// </summary>

    [ExtensionUri("logger://SimpleConsoleLogger/v1")] /// Uri used to uniquely identify the console logger. 

    [FriendlyName("MSTestLogger")] /// Alternate user friendly string to uniquely identify the logger.

    public class MSTestLogger : ITestLogger

    {
        private static NetworkStream networkStream;
        private TcpClient clientSocketWriter;
        List<TestResult> testResults = new List<TestResult>();
        string testRunDirectory { get; set; }
        DateTime testRunStarted = DateTime.Now;
        string content = "";
        /// <summary>
        /// Initializes the Test Logger.
        /// </summary>
        /// <param name="events">Events that can be registered for.</param>
        /// <param name="testRunDirectory">Test Run Directory</param>

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {


            this.testRunDirectory = testRunDirectory;
            var fs = File.Create("test.test");
            fs.Close();
            // Register for the events.
            events.TestRunMessage += TestMessageHandler;
            events.TestResult += TestResultHandler;
            events.TestRunComplete += TestRunCompleteHandler;
            

            string ipStr = "127.0.0.1";
            IPAddress ipServer = IPAddress.Parse(ipStr);
            int port = 6543;
            
            clientSocketWriter = new System.Net.Sockets.TcpClient();
            clientSocketWriter.Connect(ipServer, port);
            networkStream = clientSocketWriter.GetStream();
            Thread.Sleep(1000);


        }
        public void Shutdown()
        {

            clientSocketWriter.GetStream().Close();
            clientSocketWriter.Close();
        }
        /// <summary>
        /// Called when a test message is received.
        /// </summary>
        private void TestMessageHandler(object sender, TestRunMessageEventArgs e)
        {
            content += e.Message;
            switch (e.Level)
            {
                case TestMessageLevel.Informational:
                    Console.WriteLine("Information: " + e.Message);
                    break;
                case TestMessageLevel.Warning:
                    Console.WriteLine("Warning: " + e.Message);
                    break;
                case TestMessageLevel.Error:
                    Console.WriteLine("Error: " + e.Message);
                    break;
                default:
                    break;
            }
            SendMessage(0, "test event " + e.Message);
            File.AppendAllText("test.test", e.Message);
        }
        /// <summary>
        /// Called when a test result is received.
        /// </summary>
        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
            content += "\n" + e.Result.StartTime + " " + e.Result.Outcome;
            string name = !string.IsNullOrEmpty(e.Result.DisplayName) ? e.Result.DisplayName : e.Result.TestCase.FullyQualifiedName;
            if (e.Result.Outcome == TestOutcome.Skipped)
            {
                Console.WriteLine(name + " Skipped");
            }
            else if (e.Result.Outcome == TestOutcome.Failed)
            {
                Console.WriteLine(name + " Failed");
                if (!String.IsNullOrEmpty(e.Result.ErrorStackTrace))
                {
                    Console.WriteLine(e.Result.ErrorStackTrace);
                }
            }
            else if (e.Result.Outcome == TestOutcome.Passed)
            {
                Console.WriteLine(name + " Passed");

            }
            SendMessage(0, name + " " + e.Result.ComputerName + " - " + e.Result.ToString());
            File.AppendAllText("test.test", e.Result.ToString());
            testResults.Add(e.Result);
        }
        private void SendMessage(int ID, string line)
        {

            Byte[] sendBytes = Encoding.ASCII.GetBytes("$ " + line + "\n");

            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();

            //Thread.Sleep(3000);
         
        }

        /// <summary>
        /// Called when a test run is completed.
        /// </summary>
        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            string s = "Total Executed: " + e.TestRunStatistics.ExecutedTests + "\n";
            s +=  "Total Passed: " + e.TestRunStatistics[TestOutcome.Passed] + "\n";
            s += "Total Failed: " + e.TestRunStatistics[TestOutcome.Failed] + "\n";
            s += "Total Skippeds: " + e.TestRunStatistics[TestOutcome.Skipped] + "\n";
            SendMessage(0, s);
            File.WriteAllText("test.test", s);
            //Shutdown();
            content += "\n" + s;
            var trxOutputWriter = new MsTestTrxXmlWriter(testResults, e, testRunStarted);

            trxOutputWriter.WriteTrxOutput(testRunDirectory, content);
        }
    }
    public static class UnitTestIdGenerator
    {
        private static readonly HashAlgorithm provider = new SHA1CryptoServiceProvider();

        /// <summary>
        /// Calculates a hash of the string and copies the first 128 bits of the hash to a new Guid.
        /// </summary>
        internal static Guid GuidFromString(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte[] hash = provider.ComputeHash(Encoding.Unicode.GetBytes(data));

            byte[] toGuid = new byte[16];
            Array.Copy(hash, toGuid, 16);

            return new Guid(toGuid);
        }
    }
    public class MsTestTrxXmlWriter
    {
        private const string adapterTypeName = "Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        private readonly IList<TestResult> testResults;

        private readonly TestRunCompleteEventArgs completeEventArgs;

        private readonly DateTime testRunStarted;

        private readonly Dictionary<TestResult, Guid> executionIds = new Dictionary<TestResult, Guid>();

        private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        public MsTestTrxXmlWriter(IList<TestResult> testResults, TestRunCompleteEventArgs completeEventArgs, DateTime testRunStarted)
        {
            this.testResults = testResults;
            this.completeEventArgs = completeEventArgs;
            this.testRunStarted = testRunStarted;
        }

        public void WriteTrxOutput(string outputDirectoryPath, string content)
        {
            Console.WriteLine("Starting to generate trx XML output.");

            var testRunId = Guid.NewGuid();

            //      BinaryFormatter serializer =
            //new BinaryFormatter();
            //      Stream writer = new FileStream("filename.xml", FileMode.CreateNew);
            //      serializer.Serialize(writer, testResults[0]);
            //      writer.Close();

            RunTests r = new RunTests();
            r.RunId = testRunId.ToString();
            r.Name = $"{Environment.UserName}@{Environment.MachineName} {DateTime.UtcNow}";
            r.RunUser = $@"{Environment.UserDomainName}\{Environment.UserName}";
            r.results = new List<RunTestResult>();
            foreach(TestResult rr in testResults)
            {
                RunTestResult s = new RunTestResult();

        //        s.ComputerName = Environment.MachineName;
        //        //s.Duration = result.Duration;
        //        s.EndTime = rr.EndTime;
        //public string RunId 
        //public string Outcome 
        //public string StartTime 
        //public string FullyQualifiedName 
        //public string DisplayName 
        //public string Output 

    }

    XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", null),
                new XElement(ns + "TestRun",
                    new XAttribute("id", testRunId.ToString()),
                    new XAttribute("name", $"{Environment.UserName}@{Environment.MachineName} {DateTime.UtcNow}"),
                    new XAttribute("runUser", $@"{Environment.UserDomainName}\{Environment.UserName}"),
                    new XElement("Results",
                        testResults.Select(result => new XElement("UnitTestResult",
                            new XAttribute("computerName", Environment.MachineName),
                            new XAttribute("duration", result.Duration.ToString()),
                            new XAttribute("endTime", result.EndTime.ToString("o")),
                            new XAttribute("executionId", GetExecutionId(result)),
                            new XAttribute("outcome", result.Outcome == TestOutcome.Skipped ? "Inconclusive" : result.Outcome.ToString()),
                            new XAttribute("startTime", result.StartTime.ToString("o")),
                            new XAttribute("testId", result.TestCase.FullyQualifiedName),
                            new XAttribute("testListId", testRunId.ToString()),
                            new XAttribute("testName", result.TestCase.DisplayName),
                            new XAttribute("testType", testRunId.ToString()),
                            GetOutputElement(result))))));
            
                    //new XElement("ResultSumary",
                    //    new XAttribute("outcome", completeEventArgs.IsAborted ? "Aborted" : completeEventArgs.IsCanceled ? "Canceled" : "Completed"),
                    //    new XElement("Counters",
                    //        new XAttribute("aborted", 0),
                    //        new XAttribute("completed", 0),
                    //        new XAttribute("disconnected", 0),
                    //        new XAttribute("error", 0),
                    //        new XAttribute("executed", testResults.Count(r => r.Outcome != TestOutcome.Skipped)),
                    //        new XAttribute("failed", testResults.Count(r => r.Outcome == TestOutcome.Failed)),
                    //        new XAttribute("inconclusive", testResults.Count(r => r.Outcome == TestOutcome.Skipped || r.Outcome == TestOutcome.NotFound || r.Outcome == TestOutcome.None)),
                    //        new XAttribute("inProgress", 0),
                    //        new XAttribute("notExecuted", testResults.Count(r => r.Outcome == TestOutcome.Skipped)),
                    //        new XAttribute("notRunnable", 0),
                    //        new XAttribute("passed", testResults.Count(r => r.Outcome == TestOutcome.Passed)),
                    //        new XAttribute("passedButRunAborted", 0),
                    //        new XAttribute("pending", 0),
                    //        new XAttribute("timeout", 0),
                    //        new XAttribute("total", testResults.Count),
                    //        new XAttribute("warning", 0))),
                    //  new XElement("TestDefinitions",
                    //    testResults.Select(result => new XElement("UnitTest",
                    //        new XAttribute("id", UnitTestIdGenerator.GuidFromString(result.TestCase.FullyQualifiedName)),
                    //        new XAttribute("name", result.TestCase.DisplayName),
                    //        new XAttribute("storage", result.TestCase.Source),
                    //        new XElement("Description", GetDescription(result)),
                    //        new XElement("Execution", new XAttribute("id", GetExecutionId(result))),
                    //        new XElement("Properties",
                    //            GetPropertyAttributes(result).Select(p => new XElement("Property",
                    //                new XElement("Key", p.Name),
                    //                new XElement("Value", p.Value)))),
                    //        new XElement("TestCategory",
                    //            GetTestCategory(result).Select(c => new XElement("TestCategoryItem", c.TestCategories.First()))),
                    //        new XElement("TestMethod",
                    //            new XAttribute("adapterTypeName", adapterTypeName),
                    //            new XAttribute("className", GetClassFullName(result)),
                    //            new XAttribute("codeBase", result.TestCase.Source),
                    //            new XAttribute("name", result.TestCase.DisplayName))))),
                    //  new XElement("TestEntries",
                    //    testResults.Select(result => new XElement("TestEntry",
                    //        new XAttribute("executionId", GetExecutionId(result)),
                    //        new XAttribute("testId", UnitTestIdGenerator.GuidFromString(result.TestCase.DisplayName)),
                    //        new XAttribute("testListId", testRunId.ToString())))),
                    //  new XElement("TestLists",
                    //    new XElement("TestList", new XAttribute("id", testRunId.ToString()), new XAttribute("name", "Test list"))),
                    //  new XElement("Times",
                    //    new XAttribute("creation", testRunStarted.ToString("o")),
                    //    new XAttribute("finish", DateTime.Now.ToString("o")),
                    //    new XAttribute("queueing", testRunStarted.ToString("o")),
                    //    new XAttribute("start", testRunStarted.ToString("o")))
                    //));

            // We only want to have the xmlns present on the root tag, and not on the descendants, so we're cleaning all the namespaces.
            CleanXmlNamespaces(doc);

            Console.WriteLine("XML generation done, saving the trx files.");

            var trxResultFilePath = Path.Combine(
                outputDirectoryPath,
                String.Format(
                    "{0}_{1} {2}.trx",
                    Environment.UserName,
                    Environment.MachineName,
                    DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss")));

            File.WriteAllText(trxResultFilePath, doc.ToString());

            //Console.WriteLine("Results File: {0}", trxResultFilePath);

            
            File.WriteAllText(Path.Combine(outputDirectoryPath, "testResultsMsTestLogger.trx"), content.ToString());
        }

        private static void CleanXmlNamespaces(XDocument doc)
        {
            if (doc == null || doc.Root == null)
            {
                return;
            }

            foreach (var node in doc.Root.Descendants())
            {
                if (node.Name.NamespaceName == "")
                {
                    node.Attributes("xmlns").Remove();
                    node.Name = node.Parent.Name.Namespace + node.Name.LocalName;
                }
            }
        }

        /// <summary>
        /// Returns the Output tag containing both the normal test output, and the error message (if there was any).
        /// </summary>
        private XElement GetOutputElement(TestResult result)
        {
            var element = new XElement("Output",
                new XElement("StdOut",
                    String.Join(Environment.NewLine, result.Messages.Select(m => m.Text).ToArray())));

            if (!String.IsNullOrEmpty(result.ErrorMessage) || !String.IsNullOrEmpty(result.ErrorStackTrace))
            {
                element.Add(new XElement("ErrorInfo",
                    new XElement("Message", result.ErrorMessage),
                    new XElement("StackTrace", result.ErrorStackTrace)));
            }

            return element;
        }

        /// <summary>
        /// Returns the execution id for the given test.
        /// </summary>
        /// <remarks>
        /// The execution ids can be generated randomly, but we have to use the same id for a given test in multiple places in the XML.
        /// Hence the ids are stored in a dictionary for every test.
        /// </remarks>
        private Guid GetExecutionId(TestResult result)
        {
            if (!executionIds.ContainsKey(result))
            {
                executionIds.Add(result, Guid.NewGuid());
            }

            return executionIds[result];
        }

        /// <summary>
        /// Loads the assembly from <paramref name="path" /> and stores its reference so that we don't load an assembly multiple times.
        /// </summary>
        private Assembly GetAssembly(string path)
        {
            if (!assemblies.ContainsKey(path))
            {
                assemblies.Add(path, Assembly.LoadFrom(path));
            }

            return assemblies[path];
        }

        /// <summary>
        /// Returns the full description text of the unit test.
        /// </summary>
        /// <remarks>
        /// The description text (specified with <see cref="Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute" />) is not present
        /// in the object model provided in Microsoft.VisualStudio.TestPlatform.ObjectModel, so we have to look it up in the unit test assembly.
        /// </remarks>
        private string GetDescription(TestResult test)
        {
            var assembly = GetAssembly(test.TestCase.Source);

            var className = test.TestCase.FullyQualifiedName.Substring(0, test.TestCase.FullyQualifiedName.LastIndexOf('.'));
            var methodName = test.TestCase.FullyQualifiedName.Substring(test.TestCase.FullyQualifiedName.LastIndexOf('.') + 1);

            var type = assembly.GetType(className);

            var method = type.GetMethod(methodName);

            var attributes = method.GetCustomAttributes<DescriptionAttribute>().ToList();

            if (attributes.Any())
            {
                return attributes.First().Description;
            }

            return test.TestCase.DisplayName;
        }

        /// <summary>
        /// Returns the list of TestPropertyAttributes specified for <paramref name="test" />.
        /// </summary>
        /// <remarks>
        /// The information in the TestPropertyAttributes (<see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute" />) is not present
        /// in the object model provided in Microsoft.VisualStudio.TestPlatform.ObjectModel, so we have to look it up in the unit test assembly.
        /// </remarks>
        private IEnumerable<TestPropertyAttribute> GetPropertyAttributes(TestResult test)
        {
            var assembly = GetAssembly(test.TestCase.Source);

            var className = test.TestCase.FullyQualifiedName.Substring(0, test.TestCase.FullyQualifiedName.LastIndexOf('.'));
            var methodName = test.TestCase.FullyQualifiedName.Substring(test.TestCase.FullyQualifiedName.LastIndexOf('.') + 1);

            var type = assembly.GetType(className);

            var method = type.GetMethod(methodName);

            return method.GetCustomAttributes<TestPropertyAttribute>();
        }

        /// <summary>
        /// Returns the list of TestCategoryAttributes specified for <paramref name="test" />.
        /// </summary>
        /// <remarks>
        /// The information in the TestCategoryAttributes (<see cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute" />) is not present
        /// in the object model provided in Microsoft.VisualStudio.TestPlatform.ObjectModel, so we have to look it up in the unit test assembly.
        /// </remarks>
        private IEnumerable<TestCategoryAttribute> GetTestCategory(TestResult test)
        {
            var assembly = GetAssembly(test.TestCase.Source);

            var className = test.TestCase.FullyQualifiedName.Substring(0, test.TestCase.FullyQualifiedName.LastIndexOf('.'));
            var methodName = test.TestCase.FullyQualifiedName.Substring(test.TestCase.FullyQualifiedName.LastIndexOf('.') + 1);

            var type = assembly.GetType(className);

            var method = type.GetMethod(methodName);

            return method.GetCustomAttributes<TestCategoryAttribute>();
        }

        /// <summary>
        /// Returns the fully qualified assembly name of the unit test class.
        /// </summary>
        /// <remarks>
        /// This information  is not present in the object model provided in Microsoft.VisualStudio.TestPlatform.ObjectModel, so we have to look it up in the unit test assembly.
        /// </remarks>
        private string GetClassFullName(TestResult test)
        {
            var assembly = GetAssembly(test.TestCase.Source);

            var className = test.TestCase.FullyQualifiedName.Substring(0, test.TestCase.FullyQualifiedName.LastIndexOf('.'));

            var type = assembly.GetType(className);

            return type.AssemblyQualifiedName;
        }
    }
    public class RunTests
    {

        public string RunId { get; set; }
        public string Name { get; set; }
        public string RunUser { get; set; }

        public List<RunTestResult> results { get; set; }
    }
    public class RunTestResult
    {

        public string ComputerName { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string RunId { get; set; }
        public string Outcome { get; set; }
        public DateTime StartTime { get; set; }
        public string FullyQualifiedName { get; set; }
        public string DisplayName { get; set; }
        public string Output { get; set; }

    }
}

