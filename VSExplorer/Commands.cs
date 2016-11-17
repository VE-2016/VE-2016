using System;

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

    /// <summary>
    /// The 'Command' abstract class
    /// </summary>
    public abstract class Command
    {

        public static bool running = false;

        public static int counter = 0;

        protected Receiver receiver;

        public ExplorerForms ef { get; set; }

        // Constructor
        public Command(ExplorerForms ef)
        {
            this.ef = ef;
            //this.receiver = receiver;
        }

        public abstract object Execute(object obs = null);
    }

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