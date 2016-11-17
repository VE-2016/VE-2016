using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime.Utilities;
using Microsoft.Diagnostics.Runtime.Utilities.Pdb;
using System.Collections;

namespace ProfilerNET
{

    public class Profiler
    {
        public static void Profile(ClrRuntime runtime)
        {
            {
               

                foreach (ClrThread thread in runtime.Threads)
                {
                    Console.WriteLine("Thread {0:x}:", thread.OSThreadId);

                    foreach (ClrStackFrame frame in thread.StackTrace)
                    {
                        if (frame.Kind == ClrStackFrameType.Runtime)
                        {
                            Console.WriteLine("{0,12:x} {1,12:x} {2}", frame.InstructionPointer, frame.StackPointer, frame.DisplayString);
                        }
                        else
                        {
                            FileAndLineNumber info = frame.GetSourceLocation();
                            Console.WriteLine("{0,12:x} {1,12:x} {2} [{3} @ {4}]", frame.InstructionPointer, frame.StackPointer, frame.DisplayString, info.File, info.Line);
                        }
                    }
                }
            }
        }
        public static Node GetReference(ClrRuntime runtime, ulong obj)
        {

            try
            {
                // Create a ClrRuntime instance from the dump and dac location.  The ClrRuntime
                // object represents a version of CLR loaded in the process.  It contains data
                // such as the managed threads in the process, the AppDomains in the process,
                // the managed heap, and so on.
                

                // To check whether obj is actually a valid object, we attempt to get its type.
                // If the type is null then we do not have a valid object.
                ClrHeap heap = runtime.GetHeap();
                ClrType type = heap.GetObjectType(obj);
                if (type == null)
                {
                    Console.WriteLine("{0:X} is not a valid object.", obj);
                    return null;
                }

                // Set up initial state.
                m_considered = new ObjectSet(heap);
                m_targets[obj] = new Node(obj, type);

                // Enumerating all roots in the process takes a while.  We do it once and full this list.
                // (Note we actually also get static variables as roots instead of simply pinned object
                // arrays.)
                List<ClrRoot> roots = FillRootDictionary(heap);

                foreach (var root in roots)
                {

                    if (m_completedRoots.Contains(root) || m_considered.Contains(root.Object))
                        continue;

                    Node path = FindPathToTarget(heap, root);
                    if (path != null)
                    {
                        PrintOnePath(path);
                        return path;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception:");
                Console.WriteLine(ex);
            }

            return null;
        }

        public static Node FindPathToTarget(ClrHeap heap, ClrRoot root)
        {
            ClrType type = heap.GetObjectType(root.Object);
            if (type == null)
                return null;

            List<ulong> refList = new List<ulong>();
            List<int> offsetList = new List<int>();

            Node curr = new Node(root.Object, type);
            while (curr != null)
            {
                if (curr.Children == null)
                {
                    refList.Clear();
                    offsetList.Clear();

                    curr.Type.EnumerateRefsOfObject(curr.Object, delegate (ulong child, int offset)
                    {
                        if (child != 0)
                        {
                            refList.Add(child);
                            offsetList.Add(offset);
                        }
                    });

                    curr.Children = refList.ToArray();
                    curr.Offsets = offsetList.ToArray();
                }
                else
                {
                    if (curr.Curr < curr.Children.Length)
                    {
                        ulong nextObj = curr.Children[curr.Curr];
                        int offset = curr.Offsets[curr.Curr];
                        curr.Curr++;

                        if (m_considered.Contains(nextObj))
                            continue;

                        m_considered.Add(nextObj);

                        Node next = null;
                        if (m_targets.TryGetValue(nextObj, out next))
                        {
                            curr.Next = next;
                            next.Prev = curr;
                            next.Offset = offset;

                            while (curr.Prev != null)
                            {
                                m_targets[curr.Object] = curr;
                                curr = curr.Prev;
                            }

                            m_targets[curr.Object] = curr;
                            return curr;
                        }

                        type = heap.GetObjectType(nextObj);

                        if (type != null && type.ContainsPointers)
                        {
                            curr = new Node(nextObj, type, curr);
                            curr.Offset = offset;
                        }
                    }
                    else
                    {
                        curr = curr.Prev;

                        if (curr != null)
                            curr.Next = null;
                    }
                }
            }

            return null;
        }

        public static void PrintOnePath(Node path)
        {

            Console.WriteLine("Roots:");
            for (var node = path; node != null; node = node.Next)
            {
                List<ClrRoot> roots;
                if (m_rootDict.TryGetValue(node.Object, out roots))
                {
                    foreach (var root in roots)
                    {
                        m_completedRoots.Add(root);
                        Console.WriteLine("{0,12:X} -> {1,12:X} {2}", root.Address, root.Object, root.Name);
                    }
                }
            }

            Console.WriteLine();
            for (var node = path; node != null; node = node.Next)
            {
                Console.WriteLine("-> {0,12:X} {1}", node.Object, node.Type.Name);
            }
        }

        private static List<ClrRoot> FillRootDictionary(ClrHeap heap)
        {
            List<ClrRoot> roots = new List<ClrRoot>(heap.EnumerateRoots());
            foreach (var root in roots)
            {
                List<ClrRoot> list;
                if (!m_rootDict.TryGetValue(root.Object, out list))
                {
                    list = new List<ClrRoot>();
                    m_rootDict[root.Object] = list;
                }

                list.Add(root);
            }
            return roots;
        }

        static HashSet<ClrRoot> m_completedRoots = new HashSet<ClrRoot>();
        static Dictionary<ulong, List<ClrRoot>> m_rootDict = new Dictionary<ulong, List<ClrRoot>>();
        static Dictionary<ulong, Node> m_targets = new Dictionary<ulong, Node>();
        static ObjectSet m_considered;

        

        #region Helpers
        public static bool TryParseArgs(string[] args, out ulong obj, out string dump, out string dac)
        {
            obj = 0;
            dump = null;
            dac = null;
            bool foundObj = false;

            foreach (string arg in args)
            {
                if (!foundObj)
                {
                    foundObj = true;
                    try
                    {
                        obj = Convert.ToUInt64(arg, 16);
                    }
                    catch
                    {
                        Console.WriteLine("Invalid object: '{0}'", arg);
                        return false;
                    }
                }
                else if (dump == null)
                {
                    dump = arg;
                }
                else if (dac == null)
                {
                    dac = arg;
                }
                else
                {
                    Console.WriteLine("Too many arguments.");
                    return false;
                }
            }

            return dump != null;
        }


     

        public static void Usage()
        {
            string fn = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Console.WriteLine("Usage: {0} object crash.dmp [dac_file_name]", fn);
        }


        #endregion
    }

    public class ObjectSet
    {
        struct Entry
        {
            public ulong High;
            public ulong Low;
            public int Index;
        }

        public ObjectSet(ClrHeap heap)
        {
            m_shift = IntPtr.Size == 4 ? 3 : 4;
            int count = heap.Segments.Count;

            m_data = new BitArray[count];
            m_entries = new Entry[count];

            for (int i = 0; i < count; ++i)
            {
                var seg = heap.Segments[i];


                m_data[i] = new BitArray(GetBitOffset(seg.Length));
                m_entries[i].Low = seg.Start;
                m_entries[i].High = seg.End;
                m_entries[i].Index = i;
            }
        }

        public void Add(ulong value)
        {
            if (value == 0)
            {
                m_zero = true;
                return;
            }

            int index = GetIndex(value);
            if (index == -1)
                return;

            int offset = GetBitOffset(value - m_entries[index].Low);

            m_data[index].Set(offset, true);
        }

        public bool Contains(ulong value)
        {
            if (value == 0)
                return m_zero;


            int index = GetIndex(value);
            if (index == -1)
                return false;

            int offset = GetBitOffset(value - m_entries[index].Low);

            return m_data[index][offset];
        }

        public int Count
        {
            get
            {
                // todo, this is nasty.
                int count = 0;
                foreach (var set in m_data)
                    foreach (bool bit in set)
                        if (bit) count++;

                return count;
            }
        }

        private int GetBitOffset(ulong offset)
        {
            
            return GetBitOffset((int)offset);
        }

        private int GetBitOffset(int offset)
        {
            return offset >> m_shift;
        }

        private int GetIndex(ulong value)
        {
            int low = 0;
            int high = m_entries.Length - 1;

            while (low <= high)
            {
                int mid = (low + high) >> 1;
                if (value < m_entries[mid].Low)
                    high = mid - 1;
                else if (value > m_entries[mid].High)
                    low = mid + 1;
                else
                    return mid;
            }

            // Outside of the heap.
            return -1;
        }

        BitArray[] m_data;
        Entry[] m_entries;
        int m_shift;
        bool m_zero;
    }

    public class Node
    {
        public Node Next;
        public Node Prev;
        public ulong Object;
        public ClrType Type;
        public int Offset;
        public ulong[] Children;
        public int[] Offsets;
        public int Curr;

        public Node(ulong obj, ClrType type, Node prev = null)
        {
            Object = obj;
            Prev = prev;
            Type = type;

            if (type == null)
                throw new ArgumentNullException("type");

            if (prev != null)
                prev.Next = this;
        }
    }
}

