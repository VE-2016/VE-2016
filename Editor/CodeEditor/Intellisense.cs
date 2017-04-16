using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSParsers;
using VSProvider;

namespace AIMS.Libraries.CodeEditor
{
    public class IntError
    {
        public CSharpUnresolvedFile c { get; set; }

        public Error e { get; set; }

        public VSProject vp { get; set; }
    }

    public class Intellisense
    {
        public ArrayList errors { get; set; }

        public Dictionary<int, Diagnostic> hc { get; set; }
        public Tuple<VSProject, Error> dict { get; set; }

        public event EventHandler ContentChanged;

        public Intellisense()
        {
            errors = new ArrayList();
        }
        private static object s_lockMethod = new object();
        public void Content()
        {
            lock (s_lockMethod)
            {
                if (ContentChanged != null)
                    ContentChanged(this, new EventArgs());
            }
        }
        public void LoadErrors(ArrayList E, VSProject vp)
        {
            ArrayList L = new ArrayList();
            foreach (VSParsers.IntErrors b in E/*errors*/)
            {
                //if (b.vp as VSProject != vp)
                    L.Add(b);
            }
            //foreach (CSharpUnresolvedFile e in E)
            //{
            //    if (e.Errors != null)
            //    {
            //        foreach (Error c in e.Errors)
            //        {
            //            IntError ie = new IntError();
            //            ie.c = e;
            //            ie.e = c;
            //            ie.vp = vp;
            //            L.Add(ie);
            //        }
            //    }
            //}
            errors = L;

            Content();
        }
    }
}
