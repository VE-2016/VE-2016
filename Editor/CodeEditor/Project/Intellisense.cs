using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Tuple<VSProject, Error> dict { get; set; }

        public event EventHandler ContentChanged;

        public Intellisense()
        {

            errors = new ArrayList();
        }

        public void Content()
        {
            if (ContentChanged != null)
                ContentChanged(this, new EventArgs());
        }
        public void LoadErrors(ArrayList E, VSProject vp)
        {
            ArrayList L = new ArrayList();
            foreach(IntError b in errors)
            {
                if (b.vp as VSProject != vp)
                    L.Add(b);
            }
            foreach(CSharpUnresolvedFile e in E)
            {
                if (e.Errors != null)
                {
                    foreach (Error c in e.Errors)
                    {
                        //Tuple<VSProject, Error> b = new Tuple<VSProject, Error>(vp, c);
                        IntError ie = new IntError();
                        ie.c = e;
                        ie.e = c;
                        ie.vp = vp;
                        L.Add(ie);
                    }
                }
            }
            errors = L;
            Content();
        }

    }
}
