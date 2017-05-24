using System;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using System.IO.MemoryMappedFiles;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;


// code from https://github.com/angelovstanton/AutomateThePlanet/blob/master/CSharp-Series/MSBuildTcpIPLogger/TcpIpLogger.cs

namespace Loggers
{

    public enum Command
    {
        MST, MSTL, TFSWN, TFSWD, TFGL, DISCONNECT, KILL, BUILD, DEL, PARSE
    }

    public class TcpIpLogger : Logger
    {
        #region Private Fields

        private IDictionary<string, string> paramaterBag;
        private static NetworkStream networkStream;
        private TcpClient clientSocketWriter;
       // private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region ILogger Members
        public override void Initialize(IEventSource eventSource)
        {
            try
            {
                Console.WriteLine("MSBuild initialization starts...");
                this.InitializeParameters();
                this.SubscribeToEvents(eventSource);
                //log.Info("Initialize MS Build Logger!");
                string ipStr = GetParameterValue("ip");
                IPAddress ipServer = IPAddress.Parse(ipStr);
                int port = int.Parse(GetParameterValue("port"));
                //log.InfoFormat("MS Build Logger port to write {0}", port);

                clientSocketWriter = new System.Net.Sockets.TcpClient();
                clientSocketWriter.Connect(ipServer, port);
                networkStream = clientSocketWriter.GetStream();
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
               // log.Error("Exception in MS Build logger", ex);
            }
        }

        public override void Shutdown()
        {
            
            clientSocketWriter.GetStream().Close();
            clientSocketWriter.Close();
        }

        #endregion

        protected virtual void InitializeParameters()
        {
            try
            {
                this.paramaterBag = new Dictionary<string, string>();
                //log.Info("Initialize Logger params");
                if (!string.IsNullOrEmpty(Parameters))
                {
                    foreach (string paramString in this.Parameters.Split(";".ToCharArray()))
                    {
                        string[] keyValue = paramString.Split("=".ToCharArray());
                        if (keyValue == null || keyValue.Length < 2)
                        {
                            continue;
                        }
                        this.ProcessParam(keyValue[0].ToLower(), keyValue[1]);
                    }
                }
            }
            catch (Exception e)
            {
                throw new LoggerException("Unable to initialize parameters; message=" + e.Message, e);
            }
        }

        /// <summary>
        /// Method that will process the parameter value. If either <code>name</code> or
        /// <code>value</code> is empty then this parameter will not be processed.
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value of the parameter</param>
        protected virtual void ProcessParam(string name, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    //add to param bag so subclasses have easy method to fetch other parameter values
                    //log.Info("Process Logger params");
                    AddToParameters(name, value);
                }
            }
            catch (LoggerException)
            {
                throw;
            }
            //catch (Exception e)
            //{
            //    string message = String.Concat("Unable to process parameters;[name=", name, ",value=", value, " message=", e.Message)
            //    //throw new LoggerException(message, e);
            //}
        }

        /// <summary>
        /// Adds the given name & value to the <code>_parameterBag</code>.
        /// If the bag already contains the name as a key, this value will replace the previous value.
        /// </summary>
        /// <param name="name">name of the parameter</param>
        /// <param name="value">value for the parameter</param>
        protected virtual void AddToParameters(string name, string value)
        {
            //log.Info("Add new item to Logger params");
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (value == null)
            {
                throw new ArgumentException("value");
            }

            string paramKey = name.ToUpper();
            try
            {
                if (paramaterBag.ContainsKey(paramKey))
                {
                    paramaterBag.Remove(paramKey);
                }

                paramaterBag.Add(paramKey, value);
            }
            catch (Exception e)
            {
                throw new LoggerException("Unable to add to parameters bag", e);
            }
        }

        /// <summary>
        /// This can be used to get the values of parameter that this class is not aware of.
        /// If the value is not present then string.Empty is returned.
        /// </summary>
        /// <param name="name">name of the parameter to fetch</param>
        /// <returns></returns>
        protected virtual string GetParameterValue(string name)
        {
            //log.Info("Get parameter value from logger params");
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            string paramName = name.ToUpper();

            string value = null;
            if (paramaterBag.ContainsKey(paramName))
            {
                value = paramaterBag[paramName];
            }

            return value;
        }

        /// <summary>
        /// Will return a collection of parameters that have been defined.
        /// </summary>
        protected virtual ICollection<string> DefiniedParameters
        {
            get
            {
                ICollection<string> value = null;
                if (paramaterBag != null)
                {
                    value = paramaterBag.Keys;
                }

                return value;
            }
        }

        private void SubscribeToEvents(IEventSource eventSource)
        {
            eventSource.BuildStarted +=
                new BuildStartedEventHandler(this.BuildStarted);
            eventSource.BuildFinished +=
                new BuildFinishedEventHandler(this.BuildFinished);
            eventSource.ProjectStarted +=
                new ProjectStartedEventHandler(this.ProjectStarted);
            eventSource.ProjectFinished +=
                new ProjectFinishedEventHandler(this.ProjectFinished);
            eventSource.TargetStarted +=
                new TargetStartedEventHandler(this.TargetStarted);
            eventSource.TargetFinished +=
                new TargetFinishedEventHandler(this.TargetFinished);
            eventSource.TaskStarted +=
                new TaskStartedEventHandler(this.TaskStarted);
            eventSource.TaskFinished +=
                new TaskFinishedEventHandler(this.TaskFinished);
            eventSource.ErrorRaised +=
                new BuildErrorEventHandler(this.BuildError);
            eventSource.WarningRaised +=
                new BuildWarningEventHandler(this.BuildWarning);
            eventSource.MessageRaised +=
                new BuildMessageEventHandler(this.BuildMessage);
        }

        #region Logging handlers


        //MemoryMappedFile mmf { get; set; }

        //MemoryMappedViewStream stream { get; set; }

        //BinaryWriter writer { get; set; }

        //void WriteMessage(int ID, string message)
        //{

        //    // using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("testmap", 10000))
        //    // {

        //    if (mmf == null)
        //    {
                
        //        mmf = MemoryMappedFile.CreateOrOpen("testmap", 1000000);


        //    }

        //    bool mutexCreated;
        //    string s;
        //    Mutex mutex = new Mutex(true, "testmapmutex", out mutexCreated);

            
        //    mutex.WaitOne();


        //    if (stream == null) stream = mmf.CreateViewStream();

            


        //    if (writer == null)
        //        writer = new System.IO.BinaryWriter(stream);


        //    writer.Write(ID);

        //    writer.Write(message);

        //    Console.WriteLine("Message written...");

        //    mutex.ReleaseMutex();
            

        //}

        private void BuildStarted(object sender, BuildStartedEventArgs e)
        {
            string message = FormatMessage(e);
            //WriteMessage(123456, message);
            SendMessage(0, FormatMessage(e));
        }

        private void BuildFinished(object sender, BuildFinishedEventArgs e)
        {
            string message = FormatMessage(e);
           // WriteMessage(1, message);
            SendMessage(1, FormatMessage(e));
            //SendMessage("END$$");
        }

        private void ProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            string message = FormatMessage(e);
            //WriteMessage(2, message);
            SendMessage(2, FormatMessage(e));
        }

        private void ProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            string message = FormatMessage(e);
            //WriteMessage(3, message);
            SendMessage(3, FormatMessage(e));
            //SendMessage("END$$");
        }

        private void TargetStarted(object sender, TargetStartedEventArgs e)
        {
            //SendMessage(FormatMessage(e));
        }

        private void TargetFinished(object sender, TargetFinishedEventArgs e)
        {
            //SendMessage(FormatMessage(e));
        }

        private void TaskStarted(object sender, TaskStartedEventArgs e)
        {
            //SendMessage(FormatMessage(e));
        }

        private void TaskFinished(object sender, TaskFinishedEventArgs e)
        {
            //SendMessage(FormatMessage(e));
        }

        private void BuildError(object sender, BuildErrorEventArgs e)
        {
            string message = FormatErrorMessage(e);
            SendMessage(4, message);
        }

        private void BuildWarning(object sender, BuildWarningEventArgs e)
        {
            string message = FormatWarningMessage(e);
            SendMessage(5, message);
        }

        private void BuildMessage(object sender, BuildMessageEventArgs e)
        {
            //SendMessage(FormatMessage(e));
        }

        #endregion

        private void SendMessage(string line)
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(line);
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();
            //log.InfoFormat("MS Build logger send to server the message {0}", line);
        }
        private void SendMessage(int ID, string line)
        {
         
            Byte[] sendBytes = Encoding.ASCII.GetBytes("$" + '1' + ID.ToString() + '1' + line + "\n");
 
            networkStream.Write(sendBytes, 0, sendBytes.Length);
            networkStream.Flush();
            //log.InfoFormat("MS Build logger send to server the message {0}", line);
        }

        private static string FormatMessage(BuildStatusEventArgs e)
        {
            return string.Format("{0}:{1}$$", e.HelpKeyword, e.Message);
        }

        private static string FormatMessage(LazyFormattedBuildEventArgs e)
        {
            return string.Format("{0}:{1}$$", e.HelpKeyword, e.Message);
        }
        private static string FormatErrorMessage(BuildErrorEventArgs e)
        {
            string helpKeyword = e.HelpKeyword;

            if (e.HelpKeyword == "" || e.HelpKeyword == null)
                helpKeyword = "-";
                
                 
            return string.Format("${0}${1}${2}${3}${4}${5}", helpKeyword, e.Message, e.File, e.ColumnNumber, e.LineNumber, e.ProjectFile);
        }
        private static string FormatWarningMessage(BuildWarningEventArgs e)
        {
            string helpKeyword = e.HelpKeyword;

            if (e.HelpKeyword == "" || e.HelpKeyword == null)
                helpKeyword = "-";


            return string.Format("${0}${1}${2}${3}${4}${5}", helpKeyword, e.Message.Replace("$",""), e.File, e.ColumnNumber, e.LineNumber, e.ProjectFile);
        }
    }
}
