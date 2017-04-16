using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Samples.Tools.Mdbg;
using System.Threading;
using System.IO;

namespace Microsoft.Samples.Tools.Mdbgs
{
    public partial class MainForm : Form
    {

        MemoryStream ms = new MemoryStream();

        ReaderWriter wr = new ReaderWriter();

        Thread thread { get; set; }

        long offset = 0;

        ToolStripContainer ts { get; set; }

        public MainForm(string[] args)
        {

            InitializeComponent();

            ts = toolStripContainer1;

           

            thread = new Thread(new ParameterizedThreadStart(ThreadWorker.Run));
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start(args);

           


                //System.Threading.Tasks.Task.Run(new Action(() =>
                //{

                //    while (true)
                //    {
                //        string m = wr.Read();

                //        if (m != null)
                //            MessageBox.Show("Message received " + m);
                //    }
                //}));
                //System.Threading.Tasks.Task.Run(new Action(() =>
                //{

                //    Thread.Sleep(5000);
                //    wr.Write("Debugger is starting...");
                    

                //}));



            }

        private void button1_Click(object sender, EventArgs e)
        {
            string c = textBox1.Text;

            MDbgShell shell = ThreadWorker.shell;

            shell.Write(c);

            
        }
    }

   
    }
    

public class ReaderWriter
{
    public List<string> lines = new List<string>();
    public object obs = new object();
    private readonly AutoResetEvent waitHandle = new AutoResetEvent(false);
    int current = 0;

    public int Timeout { get; set; }

    

    public ReaderWriter()
    {
        Timeout = 500;
    }

    public void Write(string line)
    {
        lines.Add(line);
        current = lines.Count;
        waitHandle.Set();
        
    }

    public string Read()
    {
        waitHandle.Reset();

        if (waitHandle.WaitOne(Timeout) == true)
        {
            return lines[current - 1];
        }
        else return null;
        
    }


}

public class ThreadWorker
    {


    public static MDbgShell shell { get; set; }

        static public void Run(object obs)
        {
            string[] args = obs as string[];

            string s = Thread.CurrentThread.GetApartmentState().ToString();

            LoadMdb(args);

        }
        public static int LoadMdb(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "/?":
                    case "-?":
                        Console.WriteLine("usageString");
                        return 0;
                }
            }

            shell = new MDbgShell();
            return shell.Start(args);
        }
    }


