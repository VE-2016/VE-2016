using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinExplorer
{
    public class MainApp
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        private static void Init()
        {
            //// Create receiver, command, and invoker
            //Receiver receiver = new Receiver();
            //Command command = new ConcreteCommand(receiver);
            //Invoker invoker = new Invoker();

            //// Set and execute command
            //invoker.SetCommand(command);
            //invoker.ExecuteCommand();

            //// Wait for user
            //Console.ReadKey();
        }
    }

    ///// <summary>
    ///// The 'Command' abstract class
    ///// </summary>
    //public abstract class Command
    //{

    //    public static bool running = false;

    //    public static int counter = 0;

    //    protected Receiver receiver;

    //    public string Name { get; set; }

    //    public Image image { get; set; }

    //    public Keys[] keyboard { get; set; }

    //    public string Module { get; set; }

    //    public ExplorerForms ef { get; set; }

    //    static public Command FromName(string name)
    //    {
    //        ExplorerForms ef = ExplorerForms.ef;

    //        if(name == "OpenForm")
    //            return new Command_OpenForm(ef);
    //        if (name == "OpenForm")
    //            return new Command_OpenForm(ef);
    //        guis.Add("GetTemplates", new Command_GetTemplates(ef));
    //        guis.Add("AddTemplate", new Command_AddTemplate(ef));
    //        guis.Add("GetProject", new Command_GetProject(ef));
    //        guis.Add("SetProject", new Command_SetProject(ef));
    //        guis.Add("CloseForm", new Command_CloseForm(ef));
    //        guis.Add("LoadProjectTemplate", new Command_LoadProjectTemplate(ef));
    //        guis.Add("MSBuild", new Command_MSBuild(ef));
    //        guis.Add("Config", new Command_Config(ef));
    //        guis.Add("Platform", new Command_Platform(ef));
    //        guis.Add("MainProject", new Command_SetMainProject(ef));
    //        guis.Add("OpenDocument", new Command_OpenDocument(ef));
    //        guis.Add("LoadSolution", new Command_LoadSolution(ef));
    //        guis.Add("AddProjectItem", new Command_AddProjectItem(ef));
    //        guis.Add("CloseAllDocuments", new Command_CloseAllDocuments(ef));
    //        guis.Add("TakeSnapshot", new Command_TakeSnapshot(ef));
    //        guis.Add("Customize", new Command_Customize(ef));
    //        guis.Add("Solution Platforms", new Command_Customize(ef));
    //        guis.Add("Solution Configurations", new Command_Customize(ef));
    //        guis.Add("Find", new Command_Find(ef));
    //    }
       

    //    // Constructor
    //    public Command(ExplorerForms ef)
    //    {
    //        this.ef = ef;
    //        //this.receiver = receiver;
    //    }

    //    public abstract object Execute(object obs = null);

    //    public virtual object Configure(object obs)
    //    {
    //        return obs;
    //    }
    //}

    /// <summary>
    /// The 'ConcreteCommand' class
    /// </summary>
    //public class ConcreteCommand : Command
    //{
    //    // Constructor
    //    public ConcreteCommand(Receiver receiver) :
    //      base(receiver)
    //    {
    //    }

    //    public override void Execute()
    //    {
    //        receiver.Action();
    //    }
    //}

    /// <summary>
    /// The 'Receiver' class
    /// </summary>
    public class Receiver
    {
        public void Action()
        {
            Console.WriteLine("Called Receiver.Action()");
        }
    }

    /// <summary>
    /// The 'Invoker' class
    /// </summary>
    public class Invoker
    {
        private Command _command;

        public void SetCommand(Command command)
        {
            _command = command;
        }

        public void ExecuteCommand()
        {
            _command.Execute();
        }
    }
}