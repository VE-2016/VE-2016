using System.Collections;

namespace AIMS.Libraries.CodeEditor.Core
{   /// <summary>
    /// Summary description for MemHandler.
    /// </summary>
    public class MemHandler
    {
        private static ArrayList s_mHeap = new ArrayList();

        public static void Add(GDIObject item)
        {
            s_mHeap.Add(item);
        }

        public static void Remove(GDIObject item)
        {
            if (s_mHeap.Contains(item))
            {
                s_mHeap.Remove(item);
            }
        }

        public static GDIObject[] Items
        {
            get
            {
                ArrayList al = new ArrayList();

                foreach (GDIObject go in s_mHeap)
                {
                    if (go != null)
                        al.Add(go);
                }

                GDIObject[] gos = new GDIObject[al.Count];
                al.CopyTo(0, gos, 0, al.Count);
                return gos;
            }
        }

        public static void DestroyAll()
        {
            foreach (GDIObject go in Items)
            {
                go.Dispose();
            }
        }
    }
}