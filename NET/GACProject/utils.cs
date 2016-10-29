using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GACManagerApi;

using System.Windows.Forms;

namespace GACProject
{
    public class TreeViews : System.Windows.Forms.TreeView
    {
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName,
                                                string pszSubIdList);

        protected override void CreateHandle()
        {
            base.CreateHandle();
            SetWindowTheme(this.Handle, "explorer", null);
        }
    }


    //C#
    // Implements the manual sorting of items by column.
    internal class ListViewItemComparer : IComparer
    {
        private int _col;

        private SortOrder _sorting;
        public ListViewItemComparer()
        {
            _col = 0;
        }
        public ListViewItemComparer(int column, SortOrder s)
        {
            _col = column;
            _sorting = s;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[_col].Text,
            ((ListViewItem)y).SubItems[_col].Text);

            if (_sorting == SortOrder.Descending)
                returnVal = -returnVal;

            return returnVal;
        }
    }

    public class utils
    {
        static async public Task<ArrayList> EnumerateAssemblies()
        {
            ArrayList L = new ArrayList();

            //  Start a stopwatch.
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            //  Create an assembly cache enumerator.
            var assemblyCacheEnum = new AssemblyCacheEnumerator(null);

            //  Enumerate the assemblies.
            var assemblyName = assemblyCacheEnum.GetNextAssembly();


            while (assemblyName != null)
            {
                //  Create the assembly description.
                var desc = new AssemblyDescription(assemblyName);



                //  Create an assembly view model.
                //onAssemblyEnumerated(desc);
                assemblyName = assemblyCacheEnum.GetNextAssembly();

                L.Add(desc);
            }

            //  Stop the stopwatch.
            //stopwatch.Stop();

            //  Return the elapsed time.
            //return stopwatch.Elapsed;

            return L;
        }
    }
}

