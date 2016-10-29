using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VSProvider;


namespace AIMS.Libraries.CodeEditor
{
    public class TypeExplorer
    {
        public TreeView tw { get; set; }

        static public VSProvider.MainTest mf { get; set; }

        public void LoadType()
        {
            VSProject p = GetVSProject();

            if (p == null)
                return;


            string name = p.GetDefNamespace();

            MainTest mf = TypeExplorer.mf;

            Dictionary<object, object> dict = null;


            if (mf == null)
            {
                mf = msbuilder.GetProjectDefinitions(p.FileName);

                dict = mf.wm.sharedDict;

                mf.wm.AddShared(name, dict);
            }

            dict = mf.wm.FindShared(name);

            if (dict == null)
            {
                mf = msbuilder.GetProjectDefinitions(p.FileName);

                dict = mf.wm.sharedDict;

                mf.wm.AddShared(name, dict);
            }

            //if (dict.ContainsKey(text))
            {
                //Dictionary<string, object> d = dict[text] as Dictionary<string, object>;

                //string files = d["FullFileName"] as string;

                //eo.LoadFile(files, "", null, null);

                //if (sv.dic != null)
                //{

                //    if (sv.dic.ContainsKey(files) == false)
                //        return;

                //    TreeNode nodes = sv.dic[files];

                //    nodes.EnsureVisible();

                //    sv._SolutionTreeView.SelectedNode = nodes;

                //}
            }
        }

        public VSProject GetVSProject()
        {
            return new VSProject();
        }
    }
}
