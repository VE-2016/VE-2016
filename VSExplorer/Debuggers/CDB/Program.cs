using Microsoft.Diagnostics.Runtime.Interop;
using System;

namespace DumpFileAnalyzer
{
    internal class Program
    {
        private static void Main2(string[] args)
        {
            //if(args.Length < 1) {
            //	Console.WriteLine("Usage: dumpfileanalyzer <dump file path>");
            //	return;
            //}

            //var target = DataTarget.LoadCrashDump("dump.dmp", CrashDumpReader.DbgEng);

            //var client = target.DebuggerInterface as IDebugClient5;
            //var control = client as IDebugControl6;

            //ulong extHandle;
            //control.AddExtensionWide(@"folders\kdexts.dll", 0, out extHandle);

            //Console.Title = "Analyzing " + args[0];

            Guid guid = typeof(IDebugClient).GUID;

            // create debug client object
            object obj;
            CheckHr((int)Microsoft.Diagnostics.Runtime.NativeMethods.DebugCreate(ref guid, out obj));
            var client = obj as IDebugClient5;
            var control = client as IDebugControl6;

            var events = new EventCallbacks(control);
            client.SetEventCallbacksWide(events);
            client.SetOutputCallbacksWide(new OutputCallbacks());

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                control.SetInterrupt(DEBUG_INTERRUPT.ACTIVE);
            };

            uint pid;
            if (uint.TryParse("ConsoleApplication5.exe", out pid))
            {
                // start debugging by attaching to a process

                //Process.GetProcessesByName("notepad");

                CheckHr(client.AttachProcess(0, pid, DEBUG_ATTACH.DEFAULT));
            }
            else
            {
                // start debugging by creating and attaching to a process

                CheckHr(client.CreateProcessAndAttachWide(0, "Console.exe", (DEBUG_CREATE_PROCESS)DEBUG_PROCESS.ONLY_THIS_PROCESS, 0, DEBUG_ATTACH.DEFAULT));
            }

            CheckHr(control.WaitForEvent(DEBUG_WAIT.DEFAULT, uint.MaxValue));

            DEBUG_STATUS status;
            int hr;

            while (true)
            {
                CheckHr(control.GetExecutionStatus(out status));
                if (status == DEBUG_STATUS.NO_DEBUGGEE)
                {
                    Console.WriteLine("No Target");
                    break;
                }

                if (status == DEBUG_STATUS.GO || status == DEBUG_STATUS.STEP_BRANCH ||
          status == DEBUG_STATUS.STEP_INTO ||
          status == DEBUG_STATUS.STEP_OVER)
                {
                    hr = control.WaitForEvent(DEBUG_WAIT.DEFAULT, uint.MaxValue);
                    continue;
                }

                if (events.StateChanged)
                {
                    Console.WriteLine();
                    events.StateChanged = false;
                    if (events.BreakpointHit)
                    {
                        control.OutputCurrentState(DEBUG_OUTCTL.THIS_CLIENT,
      DEBUG_CURRENT.DEFAULT);
                        events.BreakpointHit = false;
                    }
                }

                control.OutputPromptWide(DEBUG_OUTCTL.THIS_CLIENT, null);
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Gray;
                string command = Console.ReadLine();

                //command = "pc";

                //Console.WriteLine(command);
                control.ExecuteWide(DEBUG_OUTCTL.THIS_CLIENT, command,
          DEBUG_EXECUTE.DEFAULT);
            }

            //         string input;
            //do {
            //	Console.ForegroundColor = ConsoleColor.Green;
            //	Console.Write("> ");
            //	input = Console.ReadLine();

            //	control.ExecuteWide(DEBUG_OUTCTL.THIS_CLIENT, input, DEBUG_EXECUTE.DEFAULT);

            //	Console.WriteLine();
            //} while(input != "q");
        }

        private static bool CheckHr(int hresult)
        {
            return true;
        }
    }
}