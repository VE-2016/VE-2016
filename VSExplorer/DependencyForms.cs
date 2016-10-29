using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using VSProvider;

namespace WinExplorer
{
    public partial class DependencyForms : Form
    {
        public DependencyForms()
        {
            InitializeComponent();
            cb = checkedListBox1;
            cc = comboBox1;
            lb = listBox1;
        }

        public VSSolution vs { get; set; }

        public CheckedListBox cb { get; set; }

        public ComboBox cc { get; set; }

        public ListBox lb { get; set; }

        public void LoadProjects(VSSolution _vs)
        {
            vs = _vs;

            cb.Items.Clear();

            cc.Items.Clear();

            foreach (VSProject p in vs.Projects)
            {
                cb.Items.Add(p.Name, false);

                cc.Items.Add(p.Name);
            }
            GetDependencyOrder();

            foreach (int pp in R)
            {
                string name = "";
                foreach (KeyValuePair<string, int> pair in dict)
                    if (pp.Equals(pair.Value)) name = pair.Key;

                lb.Items.Add(name);
            }
        }

        private ArrayList D { get; set; }

        private int[,] pp { get; set; }

        private Dictionary<string, int> dict { get; set; }

        private ArrayList R { get; set; }

        public void GetDependencyOrder()
        {
            D = new ArrayList();

            dict = new Dictionary<string, int>();
            int i = 0;
            foreach (VSProject p in vs.Projects)
            {
                dict.Add(Path.GetFileNameWithoutExtension(p.FileName), i);
                i++;
            }

            int r = vs.Projects.Count();

            rr = r;

            ArrayList L = vs.GetDependencyOrder(D);

            int c = vs.Projects.Count() + 1 + 1;

            if (c <= 1)
                return;

            pp = new int[c, c];
            {
                for (int d = 0; d < vs.projects.Count(); d++)
                    for (int j = 0; j < vs.projects.Count(); j++)
                        pp[d, j] = -1;
            }

            i = 0;
            foreach (VSProject p in vs.Projects)
            {
                ArrayList B = L[i] as ArrayList;
                foreach (string s in B)
                {
                    int b = dict[s];
                    pp[i, b] = 1;
                }
                i++;
            }

            i = 0;
            while (i < vs.Projects.Count())
            {
                for (int j = 0; j < vs.Projects.Count(); j++)
                {
                    if (pp[i, j] >= 0)
                        for (int k = 0; k < vs.projects.Count(); k++)
                            if (pp[j, k] >= 0)
                                pp[i, k] = 1;
                }

                i++;
            }

            for (int d = 0; d < vs.projects.Count(); d++)
                for (int j = 0; j < vs.projects.Count(); j++)
                    if (pp[d, j] >= 0)
                        pp[d, r] += 1;

            ArrayList E = new ArrayList();

            R = new ArrayList();

            for (int g = 0; g < r; g++)
            {
                ArrayList PD = new ArrayList();

                for (int d = 0; d < r; d++)
                {
                    if (pp[d, r] == g)
                    {
                        PD.Add(d);
                        pp[i, r + 1] = 0;
                    }
                }

                ArrayList F = new ArrayList();

                for (int b = 0; b < PD.Count; b++)
                {
                    ArrayList B = Deps((int)PD[b], r);

                    RemoveExisting(R, B);

                    B.Insert(0, PD[b]);

                    F.Add(B);
                }

                foreach (ArrayList K in F)
                {
                    if (K.Count <= 1)
                        addDeps(R, K, E);
                }
            }
        }

        public int rr = 0;

        public void RemoveExisting(ArrayList D, ArrayList P)
        {
            foreach (int d in D)
            {
                int b = P.IndexOf(d);

                if (b >= 0)
                    P.RemoveAt(b);
            }
        }

        public static void addDeps(ArrayList E, ArrayList D, ArrayList P)
        {
            int b = (int)D[0];

            int i = E.IndexOf(b);
            if (i < 0)
                E.Add(b);

            D.RemoveAt(0);

            foreach (int d in D)
            {
                int c = P.IndexOf(d);
                if (c < 0)
                    P.Add(d);
            }
        }

        public static void addDep(ArrayList E, int d)
        {
            int i = E.IndexOf(d);
            if (i < 0)
                E.Add(d);
        }

        public ArrayList Deps(int p, int r)
        {
            ArrayList D = new ArrayList();
            for (int i = 0; i < r; i++)
                if (pp[p, i] >= 0)
                    D.Add(i);
            return D;
        }

        public static bool hasDep(int p, int d)
        {
            return true;
        }

        public void ReloadProject()
        {
            int i = cc.SelectedIndex;
            if (i < 0)
                return;
            string name = cc.Items[i].ToString();

            VSProject p = GetProject(name);

            if (p == null)
                return;

            ArrayList L = new ArrayList();

            //foreach(VSProject p in vs.Projects)

            L = p.GetProjectReferences("ProjectReference");

            i = 0;
            while (i < cb.Items.Count)
            {
                cb.SetItemChecked(i, false);

                string names = cb.Items[i].ToString();

                int c = L.IndexOf(names);

                if (c >= 0)
                {
                    cb.SetItemChecked(i, true);
                }

                i++;
            }
        }

        public VSProject GetProject(string name)
        {
            foreach (VSProject p in vs.Projects)
            {
                if (p.Name == name)
                    return p;
            }

            return null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadProject();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = cc.SelectedIndex;
            if (i < 0)
                return;
            string name = cc.Items[i].ToString();

            VSProject p = GetProject(name);

            if (p == null)
                return;

            ArrayList L = new ArrayList();

            ArrayList R = new ArrayList();

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (VSProject ps in vs.Projects)
            {
                dict.Add(Path.GetFileNameWithoutExtension(ps.FileName), ps.FileName);
            }

            i = 0;
            while (i < cb.Items.Count)
            {
                if (cb.GetItemChecked(i) == true)
                {
                    string names = cb.Items[i].ToString();

                    //int c = L.IndexOf(names);

                    //string file = Path.GetFileNameWithoutExtension(names);

                    //if (c >= 0)
                    {
                        //cb.SetItemChecked(i, true);

                        L.Add(dict[names]);
                    }
                }

                i++;
            }

            p.SetProjectReferences(L);
        }
    }
}