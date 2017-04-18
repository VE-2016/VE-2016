using Microsoft.Diagnostics.Runtime.Interop;
using System;

namespace DumpFileAnalyzer
{
    internal class OutputCallbacks : IDebugOutputCallbacks2 // IDebugOutputCallbacksWide
    {
        public int Output(DEBUG_OUTPUT Mask, string Text)
        {
            switch (Mask)
            {
                case DEBUG_OUTPUT.DEBUGGEE:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case DEBUG_OUTPUT.PROMPT:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case DEBUG_OUTPUT.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case DEBUG_OUTPUT.EXTENSION_WARNING:
                case DEBUG_OUTPUT.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case DEBUG_OUTPUT.SYMBOLS:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.Write(Text);
            return 0;
        }

        public int GetInterestMask(out DEBUG_OUTCBI Mask)
        {
            Mask = DEBUG_OUTCBI.TEXT;
            return 0;
        }

        public int Output2(DEBUG_OUTCB Which, DEBUG_OUTCBF Flags, ulong Arg, string Text)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Text);
            return 0;
        }
    }

    internal class EventCallbacks : IDebugEventCallbacksWide
    {
        private readonly IDebugControl6 _control;
        public bool BreakpointHit;
        public bool StateChanged;
        //private readonly ModuleEventHandler ModuleLoadEvent;

        public EventCallbacks(IDebugControl6 control)
        {
            _control = control;
        }

        public int GetInterestMask(out DEBUG_EVENT Mask)
        {
            Mask = DEBUG_EVENT.BREAKPOINT | DEBUG_EVENT.CHANGE_DEBUGGEE_STATE
| DEBUG_EVENT.CHANGE_ENGINE_STATE | DEBUG_EVENT.CHANGE_SYMBOL_STATE |
            DEBUG_EVENT.CREATE_PROCESS | DEBUG_EVENT.CREATE_THREAD | DEBUG_EVENT.EXCEPTION | DEBUG_EVENT.EXIT_PROCESS |
            DEBUG_EVENT.EXIT_THREAD | DEBUG_EVENT.LOAD_MODULE |
                  DEBUG_EVENT.SESSION_STATUS | DEBUG_EVENT.SYSTEM_ERROR |
                  DEBUG_EVENT.UNLOAD_MODULE;

            return 0;
        }

        public int Breakpoint(IDebugBreakpoint2 Bp)
        {
            BreakpointHit = true;
            StateChanged = true;
            return (int)DEBUG_STATUS.BREAK;
        }

        public int Exception(ref EXCEPTION_RECORD64 Exception, uint FirstChance)
        {
            BreakpointHit = true;
            return (int)DEBUG_STATUS.BREAK;
        }

        public int CreateProcess(ulong ImageFileHandle, ulong Handle, ulong BaseOffset, uint ModuleSize, string ModuleName, string ImageName, uint CheckSum, uint TimeDateStamp, ulong InitialThreadHandle, ulong ThreadDataOffset, ulong StartOffset)
        {
            return 1;

            IDebugBreakpoint bp;
            _control.AddBreakpoint(DEBUG_BREAKPOINT_TYPE.CODE, uint.MaxValue, out bp);
            bp.SetOffset(StartOffset);
            bp.SetFlags(DEBUG_BREAKPOINT_FLAG.ENABLED);
            //bp.SetCommandWide(".echo Stopping on process attach");

            return (int)DEBUG_STATUS.GO;// NO_CHANGE;
        }

        public int CreateThread(ulong Handle, ulong DataOffset, ulong StartOffset)
        {
            return 0;
        }

        public int ExitProcess(uint ExitCode)
        {
            return 0;
        }

        public int ExitThread(uint ExitCode)
        {
            return 0;
        }

        public int LoadModule(ulong ImageFileHandle, ulong BaseOffset, uint ModuleSize, string ModuleName, string ImageName, uint CheckSum, uint TimeDateStamp)
        {
            return 0;
        }

        public int SessionStatus(DEBUG_SESSION Status)
        {
            return 0;
        }

        public int SystemError(uint Error, uint Level)
        {
            return 0;
        }

        public int UnloadModule(string ImageBaseName, ulong BaseOffset)
        {
            return 0;
        }

        public int ChangeDebuggeeState(DEBUG_CDS Flags, ulong Argument)
        {
            return 0;
        }

        public int ChangeEngineState(DEBUG_CES Flags, ulong Argument)
        {
            return 0;
        }

        public int ChangeSymbolState(DEBUG_CSS Flags, ulong Argument)
        {
            return 0;
        }
    }
}