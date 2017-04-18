//---------------------------------------------------------------------
//  This file is part of the CLR Managed Debugger (mdbg) Sample.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//---------------------------------------------------------------------
using Microsoft.Samples.Tools.Mdbg;
using Microsoft.Samples.Tools.Mdbgs;
using System;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

// This is declared in the assemblyrefs file
//[assembly:System.Runtime.InteropServices.ComVisible(false)]
#pragma warning disable 618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
#pragma warning restore 618

// Main entry point to the managed debugger.
public class Bootstap
{
    [MTAThread]
    public static int Mains(string[] args)
    {
        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "/?":
                case "-?":
                    Console.WriteLine(usageString);
                    return 0;
            }
        }

        MDbgShell shell = new MDbgShell();
        return shell.Start(args);
    }

    [STAThread]
    public static void Main2(string[] args)
    {
        Thread thread = new Thread(new ParameterizedThreadStart(ThreadWorker.Run));
        thread.SetApartmentState(ApartmentState.MTA);
        thread.Start(args);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm(args));
    }

    private const string usageString =
@"
Usage: mdbg [program [ arguments... ] ]
       mdbg !command1 [!command2 !command3 ... ]

  When program name is entered on the command line, the debugger
  automatically starts debugging such program.

  Arguments starting with ! are interpreted as debugger commands.

Examples:
  mdbg myProgram.exe

  mdbg !run myProgram.exe !step !go !kill !quit
";
}