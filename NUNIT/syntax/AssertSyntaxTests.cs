// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using NUnit.Framework.Constraints;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSProvider;
using WinExplorer;

namespace NUnit.Framework.Tests
{
    [TestFixture]
    public class VSExplorerTest
    {
        private VSSolution vs { get; set; }

        private TreeView b { get; set; }

        private ArrayList F { get; set; }


        ArrayList FolderSearch(string folder, string ext)
        {
            try
            {
                F = new ArrayList();

                foreach (string d in Directory.GetDirectories(folder))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        if (f.EndsWith(ext))
                            F.Add(f);
                        
                    }
                    FolderSearch(d, ext);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return F;
        }

        [Test]
        public void LoadSolutionTests()
        {

            string folder = "..\..\..\..";

            string folders = AppDomain.CurrentDomain.BaseDirectory + folder;
            
              if(File.Exists("cs-syntax.conf") == true)
            {

                string c = File.ReadAllText("cs-syntax.conf");

                if (Path.IsPathRooted(c) == true)
                    folders = c;
                else
                {

                    string s = AppDomain.CurrentDomain.BaseDirectory + c;

                    if (Directory.Exists(s) == true)
                        folders = s;


                }

            }
            

            FolderSearch(folders, ".sln");


            foreach (string file in F)
            {

                //string file = "..\\..\\..\\VStudio.sln";
                b = VSExplorer.LoadProject(file);
                Assert.That(b, Is.Not.Null);

                vs = b.Tag as VSSolution;
                Assert.That(vs, Is.Not.Null);

            }
        }

        public void MockProject()
        {
            ArrayList S = VSSolution.GetVSProjects(vs.solutionFileName);

            string content = File.ReadAllText(vs.solutionFileName);

            foreach (vsproject v in S)
            {
                v.content = content;

                string n = v.name.Replace("\"", "");

                n = n.Trim();

                v.name = v.name.Replace(n, '_' + n);

                v.file = v.file.Replace(n + ".", '_' + n + ".");

                string g = v.GetProject();

                g = v.ModifyProject(g);

                content = g;
            }

            string file = vs.solutionFileName;

            string name = Path.GetFileName(file).Trim();

            file = file.Replace(name, "_" + name);

            File.WriteAllText(file, content);
        }

        [Test]
        public void ChangeSolution()
        {
            Microsoft.Build.Evaluation.Project pc = null;

            if (vs == null)
                return;

            foreach (VSProject p in vs.projects)
            {
                if (File.Exists(p.FileName) == false)
                    continue;

                if(p == null)
                    continue;
                
                pc = p.LoadProjectToMemory();

                p.Shuffle(pc);
            }

            MockProject();
        }

        private Dictionary<string, ArrayList> dict { get; set; }


        public void GetSubTypes(Microsoft.Build.Evaluation.Project pc, VSProject vp)
        {
            dict = vp.GetSubtypes(pc);
        }

        [Test]
        public void CheckSolution()
        {
            Microsoft.Build.Evaluation.Project pc = null;

             if (vs == null)
                return;
            
            foreach (VSProject p in vs.projects)
            {
                pc = p.LoadProjectToMemory();
                GetSubTypes(pc, p);

                msbuilder_alls bb = new msbuilder_alls();

                TreeNode node = bb.GetProjectNode(b, p);

                Assert.That(node, Is.Not.Null);

                int i = 0;
                foreach (string s in dict.Keys)
                {
                    ArrayList L = dict[s];

                    foreach (Microsoft.Build.Evaluation.ProjectItem pp in L)
                    {
                        string type = pp.ItemType;
                        string include = pp.EvaluatedInclude;

                        TreeNode ng = bb.FindNodesIncludes(node, include);

                        Assert.That(ng, Is.Not.Null);

                        if (ng != null)
                            i++;
                    }
                }
            }
        }
    }
}
