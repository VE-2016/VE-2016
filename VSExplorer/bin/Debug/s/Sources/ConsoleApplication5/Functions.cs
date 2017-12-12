using MakeFiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApplication5.MakeFileParser;

namespace ConsoleApplication5
{
    public class Functions
    {

        static public string iffunction(string s)
        {
            return "if";
        }

        static public Dictionary<String, MakeFileLineProcessor.def> defs;

        static public Dictionary<String, MakeFileLineProcessor.target> targs;

        static public string function(string s, MakeFileLineProcessor lp, bool run = false)
        {

            

            string c = s.Replace("$", "");

            c = c.Replace("(", "");

            c = c.Replace(")", "");

            string[] cc = c.Split(",".ToCharArray());

            Console.WriteLine("Action " + c + " to be performed");

            c = c.Trim();

            if (c.StartsWith("eval"))
                c = c.Replace("eval", "").Trim();

            if (c.StartsWith("call"))
            {

                string d = c.Replace("call", "").Trim();

                string[] pp = d.Split(",".ToCharArray());


                Console.WriteLine(pp[0] + " - " + pp[1]);

                if (defs.ContainsKey(pp[0]))
                {
                    Console.WriteLine("Function found");

                    MakeFileLineProcessor.def dd = defs[pp[0]];

                    string g = dd.function.Replace("$(1)", pp[1]);

                    if (pp.Length >= 3)
                        g = g.Replace("$(2)", pp[2]);

                    g = g.Replace("@", "");

                    if (run == false)
                    {
                        g = g.Replace("\\", "!@#$");
                        string[] code = g.Split("\n\\".ToCharArray());
                        lp.processLines(code, true);
                    }
                    else
                    {

                        string[] code = g.Split("\n\\".ToCharArray());
                        g = g.Replace("\\", "");
                        Functions.Call(g);
                       

                    }

                   


                }

            }

            return "";
        }

        public static string Call(string s)
        {

           

            s = s.Replace("\\", "\\\\");


            Console.WriteLine("Function shell called - \n" + s);

            string r = "";

            if (s != "")
            {

                string cmds = "";

                make.parser.cmd.R = new ArrayList();

                make.parser.cmd.handled = false;

                make.parser.cmd.output = "";

                //make.parser.cmd.SendCommand("echo start \n" + "(" + s + " | xargs echo)" + "\necho end\n echo end\n");

                make.parser.cmd.SendCommand("\necho start\n(" + s + " | xargs echo; echo end;)\n echo end;\n echo end");

                // CMD.SendCommand( s);

                while (make.parser.cmd.handled == false) ;

                //System.Threading.Tasks.Task.Delay(20000);
                //CMD.SendCommand("echo end");
                //System.Threading.Tasks.Task.Delay(2000);



                //if (CMD.R.Count >= 1)
                {
                    cmds = make.parser.cmd.output; //CMD.R[0] as string;
                    cmds = cmds.Replace("start\n", "");
                    cmds = cmds.Replace("end\n", "");
                    //cmds = cmds.Replace("\n", "");
                    Console.WriteLine(cmds);
                    r = cmds;
                    File.AppendAllText("logger", "\nConsole - " + r);

                }
            }

            return r;

        }

        public static string[] RemoveEmpty(string[] cc)
        {

            List<string> d = new List<string>();
            foreach (string s in cc)
                if (s != null)
                    if(s != "")
                    d.Add(s);
            return d.ToArray();


        }
        static public string ExtractArray(string c)
        {
            List<string> d = new List<string>();
            int N = c.Length;
            int i = 0;
            while(i < N)
            {

                char b = c[i];

                if(b != '$')
                {
                    int s = i++;
                    while (i < N && c[i] != ' ' && c[i] != '\t')
                        i++;
                    string g = c.Substring(s, i - s);
                    d.Add(g);
                    i++;
                }
                else
                {
                    int s = i++;
                    int p = 0;
                    while (i < N)
                    {
                        if (c[i] == '(')
                            p++;
                        if (c[i] == ')')
                            p--;
                        if (p == 0)
                            break;
                        i++;
                    }
                    int v = i - s + 2;
                    if (v + s >= N)
                        v--;
                    string g = c.Substring(s, v);
                    d.Add(g);
                    i++;
                }

                d.Add("\t");

                while (i < N && (c[i] == ' ' || c[i] == '\t'))
                    i++;

            }

            string r = "";
            foreach (string s in d)
                r += s;
            return r;
            
        }
        static public MakeFileLineProcessor make { get; set; }

        static public List<string> RemoveEmptyStrings(string[] cc)
        {

            List<string> d = new List<string>();
            foreach (string s in cc)
                if (s.Trim() != "")
                    d.Add(s);
            return d;
        }

        static public string Expand(string s)
        {

            File.AppendAllText("logger", "\nExpand for - " + s);

            string v = ConsoleApplication5.make.GetVariable(s);

            string r = "";

            if (v != "")
            {

                string[] cc = v.Split(" ".ToCharArray());

                cc = RemoveEmpty(cc);

                foreach (string c in cc)
                {
                    if (c.Contains("$"))
                    {
                        string b = Expand(c);

                        //if (b.Contains("$"))
                        //    b = ConsoleApplication5.make.Substitute(b);

                        if (b == "")
                            b = c;

                        r += " " + b;

                    }
                    else r += " " + c;
                }
            }

            return r;

        }

        static string CheckExtension(string f, string e)
        {

            string[] cc = f.Split(".".ToCharArray());

            string file = "";

            int i = 0;
            while (i < cc.Length - 1)
                file += cc[i++] + ".";
            file += e;
            return file;
        }

        static string GetImpliciteRule(Dictionary<string, MakeFileLineProcessor.target> d, string v, out string res)
        {

            if (v.Contains("$"))
                v = make.findVariables(v);

            string[] cc = v.Split(".".ToCharArray());

            string c = cc[cc.Length - 1];

         

            foreach(string r in d.Keys)
            {

                //string name = "";

                

                string s = r;

                if (s.StartsWith("."))
                    s = s.Remove(0, 1);



                string[] vv = s.Split(".".ToCharArray());

                string g = vv[vv.Length - 1];

                if (g != c)
                    continue;
                if (vv.Length <= 1)
                    continue;
                string f = vv[vv.Length - 2];

                string file = CheckExtension(v, f);
                if (File.Exists(file) == false)
                    continue;
                

                Console.WriteLine("File found " + file);

                res = file;

                return r;

            }

            res = "";

            return "";
        }

        static public string target(string s, string su = "")
        {

            File.AppendAllText("logger", "\nTarget start - " + s);

           
            if (s == null || s == "")
                return "";

            if (s.Contains("$") && targs.ContainsKey(s) == false)
            {



                string v = Expand(s);

                if (v != "")
                    s = v;

                su = make.findVariables(s);



            }
            else
            {



                su = su.Trim();

                if (targs.ContainsKey(s) == false)
                {
                    bool istarget = false;
                    if (su != "")
                    {
                        if (targs.ContainsKey(su))
                        {
                            s = su;
                            istarget = true;

                        }
                        else s = su;
                    }

                    {
                        if (istarget == false)
                        {
                            string c = s.Replace("$(", "").Replace(")", "").Trim();
                            if (c.StartsWith("shell"))
                            {

                                Functions.Call(c);
                                return su;

                            }
                            //return su;
                        }

                    }

                }

            }

            File.AppendAllText("logger", "\nTarget reading - " + s);

            

            List<string> tt = new List<string>();

            if (s.Contains(" "))
            {
                string[] cs = s.Split(" ".ToCharArray());
                foreach (string v in cs)
                    tt.Add(v);
                tt = ConsoleApplication5.make.RemoveEmpty(tt);


            }
            else tt.Add(s);

            File.AppendAllText("logger", "\nTarget iterate");

            foreach (string c in tt)
            {

                s = c;

                if (targs.ContainsKey(s) == false)
                    continue;

                MakeFileLineProcessor.target target = targs[s];

                List<string> r = target.Expand();

                r = MakeFileLineProcessor.target.MergeFunctions(r);

                string u = target.prereq;

                string p = ExtractArray(u);

                //p = make.findVariables(p);

                List<string> files = new List<string>();

                string[] cc = p.Replace("\r", "").Trim().Split("\t".ToCharArray());

                cc = RemoveEmpty(cc);

                foreach (string b in cc)
                //if (b.Trim() != "")

                {
                //   if (make.parser.targs.ContainsKey(b) == false)
                //        if (b.Contains(".") == false)
                //            continue;

                    if (make.parser.targs.ContainsKey(b))
                    {

                        string file = Functions.target(b);
                        if (file.Trim() != "")
                            files.Add(file);

                        continue;
                    }


                    string name = Expand(b);

                    if(File.Exists(name))
                        if (File.Exists(b))
                        {



                        }

                    //if (make.parser.targs.ContainsKey(b) == false)
                    //    continue;

                    
                    string[] bb = name.Split(" ".ToCharArray());

                    bb = RemoveEmpty(bb);

                    foreach (string v in bb)
                    {
                        string vv = make.findVariables(v);

                        if (make.parser.targs.ContainsKey(vv) == false)
                        {

                            string res = "";
                            string rs = GetImpliciteRule(make.parser.targs, vv, out res);
                            if (rs.Trim() == "")
                                continue;
                            MakeFileLineProcessor.target tg = make.parser.targs[rs];

                            string rec = tg.recp[0];

                            string rule = rec.Replace("$@", vv);

                            rule = make.findVariables(rule);

                            rule = rule.Replace("$<", res);

                           

                            Functions.Call(rule);

                        }
                        else
                        {
                            string file = Functions.target(vv);
                            if (file.Trim() != "")
                                files.Add(file);
                        }
                    }

                    if (make.parser.targs.ContainsKey(b) == false)
                    {

                        string res = "";

                       

                        string rs = GetImpliciteRule(make.parser.targs, b, out res);
                        if (rs.Trim() == "")
                            continue;
                        MakeFileLineProcessor.target tg = make.parser.targs[rs];

                        string rec = tg.recp[0];

                        string rule = rec.Replace("$@", b);

                        rule = make.findVariables(rule);

                        rule = rule.Replace("$<", res);



                        Functions.Call(rule);

                    }

                }



                
                if (r.Count > 1 || (r.Count == 1 && target.prereq != ""))
                r.RemoveAt(0);

                if(target.tar != null)
                {

                    tarmake tar = target.tar;

                    string v = tar.ToSource();

                    string []m = v.Split("\n".ToCharArray());

                    m = RemoveEmpty(m);

                    r.Clear();

                    foreach (string g in m)
                        if(g.StartsWith("ln") == false)
                        r.Add(g);
                    string content = "";
                    foreach (string g in r)
                        content += "\n" + g;

                    r.Clear();
                    r.Add(content);

                }

                foreach (string d in r)
                {
                    File.AppendAllText("logger", "\nRecipe reading - " + d);

                    string name = target.name;

                    if (name.Contains("$"))
                        name = make.findVariables(name);

                    if (d.Trim().StartsWith("$(MAKE)"))
                    {

                        string g = d.Trim().Replace("$(MAKE)", "").Trim();

                        g = make.findVariables(g);

                        Functions.target(g);

                        File.AppendAllText("logger", "\nCall - " + g);
                    }
                    else
                    {

                        if (d.Trim().StartsWith("$(if") || d.Trim().StartsWith("($("))
                        {

                            make mk = new ConsoleApplication5.make(d);
                            string cs = mk.ToSource();
                            Functions.Call(cs);

                        }
                        else if (d.Trim().StartsWith("$(") || d.Trim().StartsWith("($("))
                        {

                            File.AppendAllText("logger", "\nTarget call - " + d);

                            List<string> bb = MakeFileLineProcessor.target.MergeFunctions(d);
                            List<string> dd = MakeFileLineProcessor.target.MergeFunctions(bb);

                            make mk = new make(d);

                            make.run = true;
                            string g = d;
                            if (g.Contains("$(@F)"))
                                g = g.Replace("$(@F)", name);
                            g = g.Replace("\\", "");
                            g = make.findVariables(g);
                            if (g.Contains("$<"))
                            {

                                string[] dc = target.recp[0].Split(" ".ToCharArray());
                                dc = RemoveEmpty(dc);
                                g = g.Replace("$<", make.findVariables(dc[0]));

                            }
                            if (g.Contains("$^"))
                            {

                                
                                g = g.Replace("$^", make.findVariables(target.prereq));

                            }
                            g = g.Replace("-DQUAD_PRECISION", "");

                            g = g.Trim().Replace("$@", name);

                            g = g.Replace("\n", "");

                            Console.WriteLine(g);

                           // if(g.Trim().StartsWith("gcc"))
                           //     g += " -UDOUBLE -UCOMPLEX -DDOUBLE -DCOMPLEX -DXDOUBLE -DXCOMPLEX -Wno-error";

                            Functions.Call(g);
                            
                            make.run = false;

                            File.AppendAllText("logger", "\nCall - " + g);

                            //Functions.Call(g);



                        }
                        else if (d.Trim().StartsWith("@"))

                        {
                            string g = d.Trim();
                            make.run = true;
                            
                            make.run = false;


                            //string[] bc = d.Split("\n@".ToCharArray());

                            //foreach (string f in bc)
                            {

                              //  g = f;

                                g = g.Replace("$$", "$");

                                g = g.Replace("\\\\", "\\");

                                //g = g.Replace("\r", "");

                                //g = g.Replace("\n", "");

                                if (g.Contains("$(@F)"))
                                    g = g.Replace("$(@F)", target.name);

                                g = g.Trim().Replace("$@", target.name);

                                g = g.Trim().Replace("@-", "");

                                g = g.Trim().Replace("@", "");

                                g = make.findVariables(g);

                                g = g.Replace("$$", "$");

                                Functions.Call(g);

                                Console.WriteLine(g);

                                File.AppendAllText("logger", "\nCall - " + g);


                            }
                        }
                        else
                        {

                           

                            string g = d.Replace("$$", "$");

                            if (g.Contains("$(@F)")) //
                                g = g.Replace("$(@F)", target.name); //


                            g = g.Trim().Replace("$@", target.name);

                            g = g.Trim().Replace("@", ""); //

                            g = g.Replace("\\", "");

                            g = make.substituteVariables(g, true);

                            if (g.Contains("$^"))
                            {

                                string[] dc = target.recp[0].Split(" ".ToCharArray());
                                g = g.Replace("$<", target.prerequ);

                            }

                            g = g.Replace("\n", " ");

                            //g = g.Replace("\\", "");

                            //g = g.Replace("/", "\\");

                            //g = g.Replace(" then ", "then echo ok; ");

                            //g = g.Replace("exit 1", "then echo no; ");

                            Console.WriteLine("Recipe " + g);

                            if (g != "")
                                if (g.Contains("PKG") == false)
                                    Functions.Call(g);

                            File.AppendAllText("logger", "\nCall - " + g);

                        }
                    }
                }
            }
            return "";
        }
    }
}
