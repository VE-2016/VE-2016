using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace WinExplorer
{
    public class Module
    {
        public static Dictionary<string, Module> modules = new Dictionary<string, Module>();

        public static Dictionary<string, Module> templates = new Dictionary<string, Module>();

        public Dictionary<string, Command> dicts = new Dictionary<string, Command>();

        public OrderedDictionary dict = new OrderedDictionary();

        public ArrayList temp { get; set; }

        public string Name { get; set; }

        public static void Initialize()
        {
            Module d = new Module();

            d.Name = "Standard";

            Module.RegisterModule("Standard", d);

            Command c = new gui.Command_NavigateBackward(ExplorerForms.ef);
            d.dict.Add("Navigate Backward", c);
            d.temp.Add("Navigate Backward");
            c = new gui.Command_NavigateForward(ExplorerForms.ef);
            d.dict.Add("Navigate Forward", c);
            d.temp.Add("Navigate Forward");
            d.temp.Add("Separator");

            Module dd = new Module();

            dd.Name = "NewItems";

            Module.RegisterModule("NewItems", dd);

            gui.Command_Module cc = new gui.Command_Module(ExplorerForms.ef);
            cc.Names = "Add Item";
            cc.image = ve_resource.NewItem_16x;

            d.dict.Add("Add Item", cc);
            d.temp.Add("Add Item");

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dd.dict.Add("New Project", c);
            c = new gui.Command_NewWebSite(ExplorerForms.ef);
            dd.dict.Add("New Web Site", c);

            c = new gui.Command_OpenFile(ExplorerForms.ef);
            d.dict.Add("Open File", c);
            d.temp.Add("Open File");
            c = new gui.Command_Save(ExplorerForms.ef);
            d.dict.Add("Save", c);
            d.temp.Add("Save");
            c = new gui.Command_SaveAll(ExplorerForms.ef);
            d.dict.Add("Save All", c);
            d.temp.Add("Save All");
            d.temp.Add("Separator");

            c = new gui.Command_Cut(ExplorerForms.ef);
            d.dict.Add("Cut", c);
            c = new gui.Command_Copy(ExplorerForms.ef);
            d.dict.Add("Copy", c);
            c = new gui.Command_Paste(ExplorerForms.ef);
            d.dict.Add("Paste", c);
            c = new gui.Command_Undo(ExplorerForms.ef);
            d.dict.Add("Undo", c);
            d.temp.Add("Undo");
            c = new gui.Command_Redo(ExplorerForms.ef);
            d.dict.Add("Redo", c);
            d.temp.Add("Redo");
         
            d.temp.Add("Separator");

            gui.Command_Gui gc = new gui.Command_Gui(ExplorerForms.ef);
            gc.Names = "Solution Configurations";
            d.dict.Add("Solution Configurations", gc);
            d.temp.Add("Solution Configurations");

            gc = new gui.Command_SolutionPlaform(ExplorerForms.ef);
            gc.Names = "Solution Platforms";
            d.dict.Add("Solution Platforms", gc);
            d.temp.Add("Solution Platforms");

            //gc = new gui.Command_Gui(ExplorerForms.ef);
            //gc.Names = "Startup Projects";
            //d.dict.Add("Startup Projects", gc);
            //d.temp.Add("Startup Projects");

            gc = new gui.Command_SolutionRun(ExplorerForms.ef);
            gc.Names = "Startup Projects";
            d.dict.Add("Startup Projects", gc);
            d.temp.Add("Startup Projects");

            c = new gui.Command_RunProject(ExplorerForms.ef);
            d.dict.Add("Start", c);
            d.temp.Add("Start");

            d.temp.Add("Separator");

            gui.Command_Launcher lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Find In Files";
            d.dict.Add("Find In Files", lc);
            d.temp.Add("Find In Files");

            gc = new gui.Command_Find(ExplorerForms.ef);
            gc.Names = "Find";
            d.dict.Add("Find", gc);

            c = new gui.Command_SolutionExplorer(ExplorerForms.ef);
            d.dict.Add("Solution Explorer", c);

            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Properties";
            d.dict.Add("Properties", lc);

            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Start Page";
            d.dict.Add("Start Page", lc);

            Module dr = new Module();

            dr.Name = "Debug";

            Module.RegisterModule("Debug", dr);

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dr.dict.Add("Start", c);
            dr.temp.Add("Start");

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dr.dict.Add("Step In", c);
            dr.temp.Add("Step In");
            c = new gui.Command_NewWebSite(ExplorerForms.ef);
            dr.dict.Add("Step Out", c);
            dr.temp.Add("Step Out");
        }

        public static void InitializeTemplates(string template)
        {
            Module d = new Module();

            d.Name = template;

            Module.RegisterTemplate(template, d);

            Command c = new gui.Command_NavigateBackward(ExplorerForms.ef);
            d.dict.Add("Navigate Backward", c);
            d.temp.Add("Navigate Backward");
            c = new gui.Command_NavigateForward(ExplorerForms.ef);
            d.dict.Add("Navigate Forward", c);
            d.temp.Add("Navigate Forward");
            d.temp.Add("Separator");

            Module dd = new Module();

            dd.Name = "NewItems";

            Module.RegisterTemplate("NewItems", dd);

            gui.Command_Module cc = new gui.Command_Module(ExplorerForms.ef);
            cc.Names = "Add Item";
            //cc.image = ve_resource.NewItem_16x;

            d.dict.Add("Add Item", cc);
            d.temp.Add("Add Item");

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dd.dict.Add("New Project", c);
            c = new gui.Command_NewWebSite(ExplorerForms.ef);
            dd.dict.Add("New Web Site", c);

            c = new gui.Command_OpenFile(ExplorerForms.ef);
            d.dict.Add("Open File", c);
            d.temp.Add("Open File");
            c = new gui.Command_Save(ExplorerForms.ef);
            d.dict.Add("Save", c);
            d.temp.Add("Save");
            c = new gui.Command_SaveAll(ExplorerForms.ef);
            d.dict.Add("Save All", c);
            d.temp.Add("Save All");
            d.temp.Add("Separator");

            c = new gui.Command_Cut(ExplorerForms.ef);
            d.dict.Add("Cut", c);
            c = new gui.Command_Copy(ExplorerForms.ef);
            d.dict.Add("Copy", c);
            c = new gui.Command_Paste(ExplorerForms.ef);
            d.dict.Add("Paste", c);
            c = new gui.Command_Undo(ExplorerForms.ef);
            d.dict.Add("Undo", c);
            d.temp.Add("Undo");

            c = new gui.Command_Redo(ExplorerForms.ef);
            d.dict.Add("Redo", c);
            d.temp.Add("Redo");
            d.temp.Add("Separator");

            gui.Command_Gui gc = new gui.Command_SolutionConfiguration(ExplorerForms.ef);
            gc.Names = "Solution Configurations";
            d.dict.Add("Solution Configurations", gc);
            d.temp.Add("Solution Configurations");

            gc = new gui.Command_SolutionPlaform(ExplorerForms.ef);
            gc.Names = "Solution Platforms";
            d.dict.Add("Solution Platforms", gc);
            d.temp.Add("Solution Platforms");

            gc = new gui.Command_SolutionRun(ExplorerForms.ef);
            gc.Names = "Startup Projects";
            d.dict.Add("Startup Projects", gc);
            d.temp.Add("Startup Projects");
            
            c = new gui.Command_RunProject(ExplorerForms.ef);
            d.dict.Add("Start", c);
            d.temp.Add("Start");

            d.temp.Add("Separator");

            gui.Command_Launcher lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Find In Files";
            d.dict.Add("Find In Files", lc);
            d.temp.Add("Find In Files");

            gc = new gui.Command_Find(ExplorerForms.ef);
            gc.Names = "Find";
            d.dict.Add("Find", gc);

            c = new gui.Command_SolutionExplorer(ExplorerForms.ef);
            d.dict.Add("Solution Explorer", c);

            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Properties";
            d.dict.Add("Properties", lc);

            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Start Page";
            d.dict.Add("Start Page", lc);

            Module dr = new Module();

            dr.Name = "Debug";

            Module.RegisterTemplate("Debug", dr);

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dr.dict.Add("Start", c);
            dr.temp.Add("Start");

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dr.dict.Add("Step In", c);
            dr.temp.Add("Step In");
            c = new gui.Command_NewWebSite(ExplorerForms.ef);
            dr.dict.Add("Step Out", c);
            dr.temp.Add("Step Out");
        }

        public static void Template(string template)
        {
            Module d = new Module();

            d.Name = template;

            Module.RegisterModule(template, d);

            Command c = new gui.Command_NavigateBackward(ExplorerForms.ef);
            d.dict.Add("Navigate Backward", c);
            d.temp.Add("Navigate Backward");
            c = new gui.Command_NavigateForward(ExplorerForms.ef);
            d.dict.Add("Navigate Forward", c);
            d.temp.Add("Navigate Forward");
            d.temp.Add("Separator");

            Module dd = new Module();

            dd.Name = "NewItems";

            Module.RegisterModule("NewItems", dd);

            gui.Command_Module cc = new gui.Command_Module(ExplorerForms.ef);
            cc.Names = "Add Item";
            cc.image = ve_resource.NewItem_16x;

            d.dict.Add("Add Item", cc);
            d.temp.Add("Add Item");

            c = new gui.Command_NewProject(ExplorerForms.ef);
            dd.dict.Add("New Project", c);
            c = new gui.Command_NewWebSite(ExplorerForms.ef);
            dd.dict.Add("New Web Site", c);

            c = new gui.Command_OpenFile(ExplorerForms.ef);
            d.dict.Add("Open File", c);
            d.temp.Add("Open File");
            c = new gui.Command_Save(ExplorerForms.ef);
            d.dict.Add("Save", c);
            d.temp.Add("Save");
            c = new gui.Command_SaveAll(ExplorerForms.ef);
            d.dict.Add("Save All", c);
            d.temp.Add("Save All");
            d.temp.Add("Separator");

            c = new gui.Command_Cut(ExplorerForms.ef);
            d.dict.Add("Cut", c);
            c = new gui.Command_Copy(ExplorerForms.ef);
            d.dict.Add("Copy", c);
            c = new gui.Command_Paste(ExplorerForms.ef);
            d.dict.Add("Paste", c);
            c = new gui.Command_Undo(ExplorerForms.ef);
            d.dict.Add("Undo", c);
            d.temp.Add("Undo");

            c = new gui.Command_Redo(ExplorerForms.ef);
            d.dict.Add("Redo", c);
            d.temp.Add("Redo");
            d.temp.Add("Separator");

            gui.Command_Gui gc = new gui.Command_SolutionConfiguration(ExplorerForms.ef);
            gc.Names = "Solution Configurations";
            d.dict.Add("Solution Configurations", gc);
            d.temp.Add("Solution Configurations");

            gc = new gui.Command_SolutionPlaform(ExplorerForms.ef);
            gc.Names = "Solution Platforms";
            d.dict.Add("Solution Platforms", gc);
            d.temp.Add("Solution Platforms");

            gc = new gui.Command_SolutionRun(ExplorerForms.ef);
            gc.Names = "Startup Projects";
            d.dict.Add("Startup Projects", gc);
            d.temp.Add("Startup Projects");
            

            c = new gui.Command_RunProject(ExplorerForms.ef);
            d.dict.Add("Start", c);
            d.temp.Add("Start");

            d.temp.Add("Separator");

            c = new gui.Command_FindInFiles(ExplorerForms.ef);
            d.dict.Add("Find In Files", c);
            d.temp.Add("Find In Files");

            c = new gui.Command_Find(ExplorerForms.ef);
            d.dict.Add("Find", c);

            c = new gui.Command_SolutionExplorer(ExplorerForms.ef);
            d.dict.Add("Solution Explorer", c);

            gui.Command_Launcher lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Properties";
            d.dict.Add("Properties", lc);

            lc = new gui.Command_Launcher(ExplorerForms.ef);
            lc.Names = "Start Page";
            d.dict.Add("Start Page", lc);
        }

        static public string ToString(Module module)
        {
            string s = "";
            s = module.Name + "\n";

            s += "!@#$%\n";
            foreach (string c in module.dict.Keys)
            {
                //string d = c + "\t" + (module.dict[c] as Command).Name + "\t" + (module.dict[c] as Command).Names;

                Command cmd = (Command)module.dict[c];

                string d = c + "\t" + cmd.ToStrings();// Name + "\t" + (module.dict[c] as Command).Names;

                s += d + "\n";
            }
            s += "!@#$%\n";
            foreach (string c in module.temp)
            {
                string d = c;

                s += d + "\n";
            }

            return s;
        }

        static public Module FromString(string s)
        {
            Module module = new Module();

            string[] cc = s.Split(new string[] { "!@#$%\n" }, StringSplitOptions.None);

            if (cc.Length < 3)
                return module;

            string c = cc[0].Trim();

            module.Name = c;

            string[] dd = cc[1].Split("\n".ToCharArray());

            module.dict = new OrderedDictionary();

            foreach (string d in dd)
            {
                if (d == "")
                    continue;
                string[] bb = d.Split("\t".ToCharArray());

                Command cmd = Command.FromName(bb[0]);

                if (cmd == null)
                    cmd = Command.FromName(bb[1]);

                cmd.FromStrings(d);

                //if(bb.Length > 2)
                //cmd.Names = bb[2];

                module.dict.Add(bb[0], cmd);
            }

            module.temp = new ArrayList();

            dd = cc[2].Split("\n".ToCharArray());

            foreach (string d in dd)
            {
                if (d == "")
                    continue;
                module.temp.Add(d);
            }

            return module;
        }

        static public string ToString()
        {
            string s = "";

            foreach (string c in modules.Keys)
            {
                s += ToString(modules[c]) + "%$#@!";
            }

            return s;
        }

        static public Dictionary<string, Module> LoadFromString(string s)
        {
            Dictionary<string, Module> dd = new Dictionary<string, Module>();

            string[] cc = s.Split(new string[] { "%$#@!" }, StringSplitOptions.None);

            foreach (string c in cc)
            {
                if (c == "")
                    continue;

                Module module = FromString(c);

                dd.Add(module.Name, module);
            }

            modules = dd;

            return dd;
        }

        static public void RegisterModule(string name, Module module)
        {
            if (modules.ContainsKey(name) == false)
                modules.Add(name, module);
        }

        static public void RegisterTemplate(string name, Module module)
        {
            if (templates.ContainsKey(name) == false)
                templates.Add(name, module);
        }

        static public Module GetModule(string name)
        {
            if (modules.ContainsKey(name) == true)
                return modules[name];
            else
                return null;
        }

        static public bool GetTemplateItem(string name, string item)
        {
            if (templates.ContainsKey(name) == true)
            {
                if (templates[name].dict.Contains(item))
                    return true;
            }
            else return true;

            return false;
        }

        public Module()
        {
            //dict = new Dictionary<string, Command>();

            dict = new OrderedDictionary();

            temp = new ArrayList();
        }

        public ArrayList GetModulesNames()
        {
            ArrayList L = new ArrayList();

            if (dict == null)
                return L;

            foreach (string s in dict.Keys)
                L.Add(s);

            return L;
        }
    }
}