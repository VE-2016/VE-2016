using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MakeFiles;
using static ConsoleApplication5.MakeFileParser.MakeFileLineProcessor;
using System.Threading;

namespace ConsoleApplication5
{


    public class make
    {

        public string name { get; set; }

        public string content { get; set; }

        public List<string> source { get; set; }

        public static MakeFileParser parser { get; set; }

        public List<make> makes { get; set; }

        public void Load(string s)
        {
            source = Merge(s);

            Merge(source);

            content = Extract(s);

	    content = Extract(s);

            source = Merge(content);

        }

        public make()
        {

        }


        public make(target g) : base()
        {
            if (g == null)
            {
                return;
            }

            makes = new List<make>();

            List<string> gg = g.Expands();

            gg = RemoveEmpty(gg);



            //gg = target.MergeFunctions(gg);

            foreach (string s in gg)
            {
                make mk = new make(s);
                makes.Add(mk);
            }



        }

        public static string[] RemoveEmpty(string[] cc)
        {

            List<string> d = new List<string>();
            foreach (string s in cc)
                if (s != null)
                    if (s != "")
                        d.Add(s);
            return d.ToArray();


        }
        public static List<string> RemoveEmpty(List<string> cc)
        {

            List<string> d = new List<string>();
            foreach (string s in cc)
                if (s != null)
                    if (s != "")
                        d.Add(s);
            return d;


        }

        public static string[] SplitAndKeep(string s, string seperator)
        {

            List<string> d = new List<string>();

            string[] obj = s.Split(new string[] { seperator }, StringSplitOptions.None);

            for (int i = 0; i < obj.Length; i++)
            {
                string result = i == obj.Length - 1 ? obj[i] : seperator + obj[i];
                d.Add(result);
            }
            return d.ToArray();
        }

        public static string Prune(string[] cc)
        {

            StringBuilder sb = new StringBuilder();

            int i = 0;

            int prev = -1;

            while (i < cc.Length)
            {
                string c = cc[i];


                int preves = c.TakeWhile(Char.IsWhiteSpace).Count();

                if (preves > prev)
                {
                    c = c.Replace("\t", "");
                    sb.Append(c);
                }


                else sb.Append(c);

                prev = preves;

                i++;
            }


            return sb.ToString();
        }

        public virtual string ToSource()
        {

            string s = "";


            

            if (makes == null || makes.Count <= 0) 
                s += content + "\n";
            else
                foreach (make mk in makes)
                {


                    s += mk.ToSource() + "\n";


                }



            return s;
        }

        public make(string s) : base()
        {

            s = s.Trim();

            content = s;




            if (content.Trim().StartsWith("$") == false)
            {
                makes = new List<make>();

                content = Substitute(content.Trim(), makes);

                return;
            }

            //source = make.Merge(content);

            //source = make.Merge(source);

            content = make.Extract(s.Trim());

            if (content.Trim().StartsWith("if"))
            {

                makes = new List<make>();

                ifmake mk = new ifmake(content);

                makes.Add(mk);

                content = mk.ToSource();

            }
            else if (content.Trim().StartsWith("foreach"))
            {

                makes = new List<make>();

                formake mk = new formake(content);

                makes.Add(mk);

                content = mk.ToSource();

            }
            else
            {
                //           content = make.Extract(s);

                content = s.Trim();

               

                makes = new List<make>();

                content = Substitute(content, makes);

                //  foreach (string c in source)
                {
                    //if (c == "")
                    //    continue;
                    //string b = Substitute(content);
                    //make mk = new make(b);
                    //makes.Add(mk);
                }

            }
        }


        virtual public void Expand()
        {

        }

        public static string AsVariable(string c)
        {

            string s = "$(" + c + ")";

            return s;



        }


        public static string Extract(string s)
        {

            int d = s.IndexOf("$");
            if (d < 0)
                return s;
            string c = s.Remove(d, 1);
            int l = c.IndexOf("(");
            if (l < 0)
                return s;
            c = c.Remove(l, 1);
            int r = c.LastIndexOf(")");
            if (r < 0)
                return s;
            c = c.Remove(r, 1);

            return c;
        }

        

        //public static string GetVariable(string s)
        //{

        //    if (parser.values.ContainsKey(s))
        //        return parser.values[s];
        //    else return "";

        //}

        public static void SaveLocals(ArrayList L)
        {

            for(int i = 1; i < 10; i++)
            {
                string c = i.ToString();
                c = make.GetVariable(c);

                if (c == "")
                    return;
                L.Add(c);
            }

        }
        public static void RestorLocals(ArrayList L)
        {

            for (int i = 1; i < L.Count; i++)
            {
                string c = L[i - 1] as string;
                parser.values[i.ToString()] = c;
            }

        }
        static public string Substitute(string c, List<make> makes = null)
        {

            if (makes == null)
                makes = new List<make>();

            StringBuilder sb = new StringBuilder();

            int s = c.IndexOf("$(");

            if (s < 0)
                return c;

            int g = 0;

            int b = 0;

            while (s >= 0)
            {

                int r = s + 2;

                int p = 1;

                while (r < c.Length && p != 0)
                {
                    if (c[r] == '(')
                        p++;
                    else if (c[r] == ')')
                        p--;

                    r++;
                }

                string d = c.Substring(b, s);

                sb.Append(d);

                if (p != 0)
                    return c;

                string v = c.Substring(s, r - s);

                string bb = v.Substring(2, v.Length - 2 - 1).Trim(); ;

                if (v.Contains(",") || v.Contains(" "))
                {


                    if (bb.StartsWith("if"))
                    {



                        ifmake ifs = new ifmake(bb);

                        makes.Add(ifs);

                        v = ifs.ToSource();

                        v = Substitute(v, makes);

                        //sb.Append(ifs.ToSource());

                        sb.Append(v);

                    }
                    else if (bb.StartsWith("call"))
                    {

                        ArrayList L = new ArrayList();

             

                        string start = v;


                        SaveLocals(L);

                        string[] cs = bb.Replace("call", "").Trim().Split(",".ToCharArray());


                        if (parser.defs.ContainsKey(cs[0].ToUpper()))
                        {
                            string cc = ((def)parser.defs[cs[0].ToUpper()]).function;

                            v = cc;
                        }
                        else if (parser.values.ContainsKey(cs[0]))
                        {
                            string cc = (parser.values[cs[0]]);

                            v = cc;
                        }

                        

                        if (cs.Length > 1)
                        {

                            string gs = Substitute(cs[1]);

                            if (gs.Contains("$(1)"))
                                gs = gs.Replace("$(1)", GetVariable("1"));
                            if (gs.Contains("$(2)"))
                                gs = gs.Replace("$(2)", GetVariable("2"));

                            gs = Substitute(gs);

                            gs = Extract(gs);

                           

                            if (parser.values.ContainsKey(gs))
                                gs = parser.values[gs];


                            parser.values["1"] = gs;


                        }
                        if (cs.Length > 2)
                        {

                            string gs = Substitute(cs[2]);

                            if (gs.Contains("$(1)"))
                                gs = gs.Replace("$(1)", GetVariable("1"));
                            if (gs.Contains("$(2)"))
                                gs = gs.Replace("$(2)", GetVariable("2"));

                            gs = Substitute(gs);

                            gs = Extract(gs);

                            if (parser.values.ContainsKey(gs))
                                gs = parser.values[gs];


                            parser.values["2"] = gs;

                            bb.Replace(cs[2], gs);


                        }
                        if (cs.Length > 3)
                        {

                            string gs = Substitute(cs[3]);

                            if (gs.Contains("$(1)"))
                                gs = gs.Replace("$(1)", GetVariable("1"));
                            if (gs.Contains("$(2)"))
                                gs = gs.Replace("$(2)", GetVariable("2"));

                            gs = Substitute(gs);

                            gs = Extract(gs);


                            if (parser.values.ContainsKey(gs))
                                gs = parser.values[gs];


                            parser.values["3"] = gs;


                        }
                        if(v != start)
                        v = Substitute(v, makes);

                        RestorLocals(L);

                        sb.Append(v);

                    }
                    else
                    {
                        v = Evaluate(v);

                        sb.Append(v);


                    }
                }
                else
                {
                    string cs = bb.Trim();

                    if (parser.values.ContainsKey(cs))
                    {
                        string cc = (parser.values[cs]);

                        cc = Substitute(cc);

                        v = cc;


                    }
                    sb.Append(v);

                }


                if (r >= c.Length)
                {
                    c = "";
                    //c = c.Substring(r - 1, 1); //c = c.Substring(r - 2 , c.Length - r + 2 );
                }
                else
                    c = c.Substring(r , c.Length - r); // c = c.Substring(r + 2, c.Length - r - 1 - 1);

                s = c.IndexOf("$(");



                if (s < 0)
                {
                    sb.Append(c);

                    break;
                }
            }

            return sb.ToString();

        }

        static public string[] Substitute(string c, string cc)
        {

            StringBuilder sb = new StringBuilder();

            int s = c.IndexOf("$(");

            if (s < 0)
                return c.Split(",".ToCharArray());

            int g = 0;

            int b = 0;

            while (s >= 0)
            {

                int r = s + 2;

                int p = 1;

                while (r < c.Length && p != 0)
                {
                    if (c[r] == '(')
                        p++;
                    else if (c[r] == ')')
                        p--;

                    r++;
                }

                string d = c.Substring(b, s);


                sb.Append(d);

                if (p != 0)
                    return c.Split(",".ToCharArray());

                string v = c.Substring(s, r - s + 1);

                //if (d.Contains(",") || d.Contains(" "))
                {
                    v = v.Replace(",", "!@#$").Replace("\t", "");
                    //v = Substitute(v);
                    // v = "";
                    sb.Append(v);
                }

                c = c.Substring(r, c.Length - r);


                //if (r + 2 >= c.Length)
                //    c = c.Substring(r - 2, c.Length - r + 2);
                //else
                //    c = c.Substring(r + 2, c.Length - r - 1 - 1);

                s = c.IndexOf("$(");

                if (s < 0)
                {
                    sb.Append(c);
                    break;
                }
            }

            string cs = sb.ToString();

            string[] pp = cs.Split(",".ToCharArray());

            List<string> bb = new List<string>();

            foreach (string vv in pp)
            {
                string csc = vv.Replace("!@#$", ",");
                bb.Add(csc);
            }



            return bb.ToArray();

        }
        static public string[] Separators(string c)
        {



            int r = 1;

            int p = 0;

            bool delim = false;

            bool funs = false;

            List<string> data = new List<string>();

            List<int> pos = new List<int>();

            while (r < c.Length && delim == false)
            {
                if (c[r - 1] == '$' && c[r] == '(')
                {

                    p++;
                }
                else
                {

                    {
                        if (c[r] == '(')
                            p++;
                        else if (c[r] == ')')
                        {
                            p--;

                        }
                    }

                }



                if (p == 0)
                    if (c[r] == ',')
                    {
                        pos.Add(r);
                        if (pos.Count == 2)
                            delim = true;
                    }


                r++;
            }

            string first = c.Substring(0, pos[0]);
            string second = c.Substring(pos[0] + 1, pos[1] - pos[0] - 1);
            string third = c.Substring(pos[1] + 1, c.Length - pos[1] - 1);

            data.Add(first);
            data.Add(second);
            data.Add(third);

            return data.ToArray();



        }

        static public List<string> Merge(List<string> s)
        {

            List<string> d = new List<string>();

            int i = 0;
            while (i < s.Count)
            {

                string c = s[i].Trim();


                int p = 0;

                string line = "";

                if (c.StartsWith("$("))
                {
                    p++;

                    int r = 2;

                    while (i < s.Count && p != 0)
                    {

                        while (r < c.Length && p != 0)
                        {
                            if (c[r] == '(')
                                p++;
                            else if (c[r] == ')')
                                p--;

                            r++;
                        }

                        if (p == 0)
                        {
                            line += "\t" + c;
                            d.Add(line);
                            break;
                        }
                        else
                        {
                            line += "\t" + c;
                            c = s[++i].Trim();
                            r = 0;
                        }


                    }

                }
                else d.Add(c);

                i++;
            }



            return d;
        }
        static public List<string> Merge(string content)
        {

            List<string> d = new List<string>();


            string[] s = content.Split("\t".ToCharArray());

            int i = 0;
            while (i < s.Length)
            {

                string c = s[i].Trim();


                int p = 0;

                string line = "";

                if (c.StartsWith("$("))
                {
                    p++;

                    //line = c;

                    int r = 2;

                    while (i < s.Length && p != 0)
                    {

                        while (r < c.Length && p != 0)
                        {
                            if (c[r] == '(')
                                p++;
                            else if (c[r] == ')')
                                p--;

                            r++;
                        }

                        if (p == 0)
                        {
                            line += c;
                            d.Add(line);
                            break;
                        }
                        else
                        {
                            line += c;
                            c = s[++i].Trim();
                            r = 0;
                        }


                    }

                }
                else d.Add(c);

                i++;
            }



            return d;
        }

        static public string GetVariable(string s)
        {

            string c = s.Replace("$", "");
            c = c.Replace("(", "");
            c = c.Replace(")", "");
            c = c.Trim();

            if (parser.values.ContainsKey(c))
                return parser.values[c];
            else return "";


        }
        static public string GetDefine(string s)
        {

            string c = s.Replace("$", "");
            c = c.Replace("(", "");
            c = c.Replace(")", "");
            c = c.Trim();

            if (parser.defs.ContainsKey(c.ToUpper()))
                return c;
            else return "";


        }
        public static string cs_value(string cs)
        {


            string s = cs.Replace("value", "").Trim();


            string g = GetVariable(s);

            if (g == "")
                g = GetDefine(s);


            return g;



        }

        public static string cs_patsubst(string s)
        {
            Console.WriteLine("Function patsubst called - " + s);

            s = s.Replace("patsubst", "").Trim();

            string[] cc = s.Split(",".ToCharArray());

            string rep = cc[1];

            string text = cc[2];

            string[] dd = text.Replace("\n", " ").Split(" ".ToCharArray());

            string result = "";

            foreach (string b in dd)
            {

                string g = rep.Replace("%", b);

                result += g + " ";


            }

            return result;
        }


        public static string cs_filter_out(string s)
        {
            Console.WriteLine("Function filter-out called - " + s);

            s = s.Replace("filter-out", "").Trim();

            string[] cc = s.Split(",".ToCharArray());

            string[] pats = cc[0].Split("\n".ToCharArray());

            string[] texts = cc[1].Split("\n".ToCharArray());

            string result = "";

            foreach (string b in texts)
            {

                bool found = false;

                foreach (string g in pats)
                {
                    if (g == b)
                    {
                        found = true;
                        break;
                    }
                    if (found == false)
                        result += b + " ";
                }


            }
            return result;
        }


        public static string cs_wildcard(string s)
        {


            // if (s.Contains("$"))
            //     s = findVariables(s);

            s = s.Replace("wildcard", "").Trim();

            if (s.Contains("$"))
                s = Substitute(s);

            string result = "";

            if (s.StartsWith("/"))
                s = s.Substring(1);

            if (s.IndexOf("/") == 1)
                s = s[0] + ":/" + s.Substring(2);

            string pattern = s;

            if (s.Contains("\\") || s.Contains("/"))
            {
                string[] cc = s.Split("\\/".ToCharArray());

                if (cc.Length > 1)
                    pattern = cc[cc.Length - 1];
                else
                    pattern = cc[0];
            }

            string dir = s.Replace(pattern, "");

            if (dir == "" || dir == "\\")
                dir = AppDomain.CurrentDomain.BaseDirectory;

            if (Directory.Exists(dir) == false)
                Console.WriteLine("No dir found");
            if (File.Exists(s))
                result = s;
            if (Directory.Exists(s))
                result = s;

            string[] dirs = Directory.GetFiles(@dir, pattern);

            foreach (string p in dirs)
                result += p + " ";

            return result;

        }

        public static string cs_sort(string s)
        {

            s = s.Replace("sort", "").Trim();

            if (s.Contains("$"))
                s = Substitute(s);

            string result = "";


            string[] cc = s.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            List<string> d = new List<string>();

            foreach(string c in cc)
            {
                if (d.Contains(c) == false)
                    d.Add(c);
            }
            foreach (string c in d)
            {
                result += c + " ";
            }

            return result;

        }


        public static string Evaluate(string s)
        {

            string c = Extract(s.Trim());

            string []cc = c.Split(" ".ToCharArray());

            if (c.StartsWith("wildcard"))
                return cs_wildcard(c);
            else if (c.StartsWith("sort"))
                return cs_sort(c);
            return s;

        }

    }

    public class formake : make
    {

        public formake(string s) : base()
        {

            content = s;

            makes = new List<make>();

            string[] cc = Separators(content);

            pl = cc[0];

            ps = cc[1];

            ac = cc[2];


        }


        string pl = "";

        string ps = "";

        string ac = "";

        public override string ToSource()
        {

            //return "foreach - " + content;

            //Console.WriteLine("foreach called");

            string result = "";

            string action = "";

            string[] cc = Separators(content);

            string c = cc[0];

            c = c.Replace("foreach", "").Trim();

            c = AsVariable(c);

            result = Substitute(cc[1]);

            action = Substitute(cc[2]);

            string cs = "";


            string[] rr = result.Split(" ".ToCharArray());

            rr = RemoveEmpty(rr);


            foreach(string s in rr)
            {

                cs += action.Replace(c, s) + "\n";



            }
            

            return cs;
            

            return result + " - " + action;


            //c = "$(" + c + ")";

            //string r = cc[1];

            //r = GetVariable(r.Trim());

            //string d = cc[2];

            //string[] rr = r.Split(VALUE_DELIMITER.ToCharArray());

            //foreach (string w in rr)
            //    if (w != "")
            //    {
            //        d = cc[2];
            //        if (run == false)
            //            d = d.Replace(c, w);
            //        else d = d.Replace(c, w + "d");

            //        /// Console.WriteLine("foreach function call -> " + " " + d);
            //        /// 
            //        MakeFileLineProcessor lp = new MakeFileLineProcessor(values, parser);
            //        Functions.function(findVariables(d), lp, run);

            //    }

        }


    }

    public class tarmake : make
    {

        public ifeq ifeqs { get; set; }

        public tarmake(ifeq q) : base()
        {

            content = "";

            makes = new List<make>();

            ifeqs = q;

          

        }

        public void Add(make mk)
        {

            makes.Add(mk);
        }

        public tarmake(string s, string []cc = null) : base()
        {

            content = "";

            makes = new List<make>();

            if(cc != null)

            foreach(string c in cc)
            {
                make mk = new make(c);

                makes.Add(mk);

                content += "\t" + c;

            }

            

            if (s.StartsWith("ifdef") || s.StartsWith("ifndef") ||  s.StartsWith("ifneq") ||  s.StartsWith("ifeq"))
            {


                ifeq qs = parser.proc.readIfeq(s);

                tarmake tar = new tarmake(qs);

                makes.Add(tar);

            }

        }
        public override string ToSource()
        {

            string s = "";

            if(ifeqs != null)
            s = parser.proc.checkCondition(ifeqs);




            if (makes == null || makes.Count <= 0)
                s += content + "\n";
            else
                foreach (make mk in makes)
                {


                    s += mk.ToSource() + "\n";


                }



            return s;
        }
    }

    public class ifmake : make
    {

        public ifmake(string s) : base()
        {


            nc = new List<make>();

            tc = new List<make>();

            content = s;

            makes = new List<make>();

            string[] cc = Separators(content);


            if (cc.Length == 3)
            {

                conds = cc[0];


                string[] gg = SplitAndKeep(cc[1], "\t");

                string v = Prune(gg).Replace("\r", "");

                gg = v.Split("\t".ToCharArray());

                gg = RemoveEmpty(gg);
                foreach (string g in gg)
                    tc.Add(new make(g));

                gg = cc[2].Split("\t".ToCharArray());
                gg = RemoveEmpty(gg);
                foreach (string g in gg)
                    nc.Add(new make(g));

                return;

            }



            bool tcs = true;

            source = make.Merge(content);

            source = make.Merge(source);

            int i = 1;
            while (i < source.Count)
            {
                string bb = source[i];

                bb = Substitute(bb, makes);

                if (bb == ",")
                {
                    tcs = false;
                }
                else if (bb.StartsWith("$"))
                {

                    make mk = new make(bb);
                    tc.Add(mk);

                }
                else
                {

                    make mk = new make(bb);

                    if (tcs == true)
                        tc.Add(mk);
                    else
                        nc.Add(mk);
                }

                i++;
            }


            //foreach (string c in source)
            //{
            //    if (c == "")
            //        continue;
            //    make mk = new make(c);
            //    makes.Add(mk);
            //}


        }

        public override string ToSource()
        {

            string s = "";

            if (conds != null)
            {
                foreach (make mk in tc)
                {
                    //if(mk.makes != null && mk.makes.Count > 0)
                    //{
                    //    foreach (make mks in mk.makes)
                    //        s +=  mks.ToSource() + "\n";

                    //}
                    //   else
                    s += mk.content + "\n";

                }

            }
            else
            {
                foreach (make mk in nc)
                {

                    s += mk.content + "\n";

                }
            }
            return s;
        }

        public string conds { get; set; }

        public List<make> nc { get; set; }

        public List<make> tc { get; set; }

        public override void Expand()
        {


        }


    }

    public class Settings
    {


        public bool debug = true;

        public string dir { get; set; }

        public string Makefile { get; set; }

        public string target { get; set; }


        public ArrayList E { get; set; }

        public string PWD { get; set; }

        public Settings()
        {
            E = new ArrayList();
        }

    }

    /**
     * Parses the make files and finds the appropriate section a given module.
     */
    public class MakeFileParser
    {

        public static Settings settings { get; set; }


        //public static String VALUE_DELIMITER = "\n";
        public static String VALUE_DELIMITER = " ";
        private string makeFile;
        public Dictionary<String, String> values;

        public Dictionary<String, MakeFileLineProcessor.target> tgs;

        public Dictionary<String, MakeFileLineProcessor.def> defs;

        public Dictionary<String, MakeFileLineProcessor.target> targs;

        public Dictionary<String, String> preds;

        public List<string> overrides;

        public MakeFileLineProcessor proc { get; set; }

        public CMD cmd { get; set; }

        /**
         * Create a parser for a given make file and module name.
         * <p>
         * A make file may contain multiple modules.
         *
         * @param makeFile The make file to parse.
         */
        public MakeFileParser(string makeFile)
        {

            if (File.Exists("logger") == false) {
                FileStream s = File.Create("logger");
                s.Close();
            }

            File.AppendAllText("logger", "Application started...");

            this.makeFile = File.ReadAllText(makeFile);

            cmd = new CMD();

            File.AppendAllText("logger", "\nApplication - read completed");
        }
        public IEnumerable<String> getValues(String key)
        {
            String str = values[key];
            if (str == null)
            {
                return null;
            }

            string[] s = str.Split(VALUE_DELIMITER.ToCharArray());

            return s;

            //return Splitter.on(VALUE_DELIMITER).trimResults().omitEmptyStrings().split(str);
        }

        string[] cc { get; set; }

        int current = -1;

        /**
         * Extracts the relevant portion of the make file and converts into key value pairs. <p> Since
         * each make file may contain multiple build targets (modules), this method will determine which
         * part is the correct portion for the given module name.
         */
        public void parse()
        {

            string cmds = "";

            CMD.running = false;

            System.Threading.Tasks.Task.Run(new Action(() => { cmds = make.parser.cmd.runCommandShell("C:\\MinGW\\msys\\1.0\\bin\\sh.exe", "--login", true/*"cmd.exe"*/); }));

            while (CMD.running == false) ;

            //cmds = CMD.runCommandShell("cmd.exe");

            values = new Dictionary<string, string>();

            overrides = new List<string>();

            defs = new Dictionary<string, MakeFileLineProcessor.def>();

            targs = new Dictionary<string, MakeFileLineProcessor.target>();

            Functions.defs = defs;

            Functions.targs = targs;

            Console.WriteLine("Parsing... ");

            LoadPredefines();

            //Console.ReadKey();

            File.AppendAllText("logger", "\nApplication shell before start");

            make.parser.cmd.handled = false;
            //CMD.SendCommand("echo start\n" + "C:\\MinGW\\msys\\1.0\\bin\\starter.bat" + "\necho end");
            make.parser.cmd.SendCommand("echo start\n\necho end");
            while (make.parser.cmd.handled == false) ;
            make.parser.cmd.output = "";

            if (settings.dir != null)
            {
                string pc = settings.dir.Replace("\\", "/");
                
                make.parser.cmd.output = "";
                make.parser.cmd.handled = false;
                make.parser.cmd.SendCommand("echo start\n cd ~\ncd " + pc + "\npwd\ntrap\necho end");
                //parser.cmd.SendCommand("echo start\n cd /d "  + settings.dir + "\npwd\ntrap\necho end");
                while (make.parser.cmd.handled == false) ;
                //File.AppendAllText("logger", "\nDirectory output " + make.parser.cmd.output);
                make.parser.cmd.output = "";
            }

            //File.AppendAllText("logger", "\nDirectory " + settings.dir);

            //Console.ReadKey();

            //File.AppendAllText("logger", "\nApplication shell has been read");

            MakeFileLineProcessor p = new MakeFileLineProcessor(values, this);

            proc = p;

            cc = makeFile.Split("\n".ToCharArray());

            //File.AppendAllText("logger", "\nApplication file lines has been read " + cc.Length.ToString());

            int i = 0;
            current = 0;
            while (current < cc.Length)
            {
                string s = cc[current];

                Console.WriteLine("Processing - " + s);

               

                p.processLine(s);

                current++;
            }
            //File.AppendAllText("logger", "\nApplication - read " + current.ToString());


            tgs = new Dictionary<string, target>();

            ArrayList L = new ArrayList();

            foreach (string s in targs.Keys)
            {
                L.Add(s);
            }



            foreach (string s in L)
            {

                string c = p.findVariables(s);
                if(tgs.ContainsKey(c) == false)
                tgs.Add(c, targs[s]);
                if (s != c)
                    if(targs.ContainsKey(c) == false)
                    targs.Add(c, targs[s]);
            }

        }
        //Files.readLines(makeFile, Charset.forName("UTF-8"), new MakeFileLineProcessor());

            public void LoadPredefines()
        {

            string file = AppDomain.CurrentDomain.BaseDirectory;
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            if (path.Contains(file) == false)
            {
                path += ";" + file;
                Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.User);
            }

            values.Add("MAKE",  ("ConsoleApplication5.exe " + "PWD=" + settings.dir).Replace("\\","\\\\"));
            //-C C:\\Users\\TEMP\\OpenBLAS - 0.2.19 - f Makefiles blas

            values.Add("CC", "gcc");

            preds = new Dictionary<string, string>();

            preds.Add("$@", "");
            preds.Add("$^", "");
            preds.Add("$%", "");
            preds.Add("$<", "");
            preds.Add("$?", "");
            preds.Add("$+", "");
            preds.Add("$|", "");
            preds.Add("$*", "");

            preds.Add("$(@F)", "");
            preds.Add("$(@D)", "");

            foreach(string s in settings.E)
            {

                string[] cc = s.Split("=".ToCharArray());

                cc = make.RemoveEmpty(cc);

                if (cc.Length <= 1)
                    continue;
                if(values.ContainsKey(cc[0]) == false)
                values.Add(cc[0],cc[1].Replace("\"",""));

            }

        }

        public void Append(string[] g)
        {

            int N = cc.Length + g.Length;

            string[] gg = new string[N];

            int i = 0;
            foreach (string s in cc)
            {
                gg[i++] = s;

                if (i == current + 1)
                    foreach (string w in g)
                        gg[i++] = w;

            }

            cc = gg;


        }

        public void SkipBlank()
        {
            int c = current;

            c++;

            while (cc[c].Trim() == "" && c < cc.Length - 1)
                c++;

            current = c--;
        }

        public string GetNextLine(bool add = true)
        {
            if (add == true)
            {
                current++;
                if (current >= cc.Length)
                    return "EOF";
                else return cc[current];
            }
            else
            {

                if (current >= cc.Length - 1)
                    return "EOF";
                else return cc[current + 1];
            }
        }
        public bool ReadNextLine(string starts)
        {


            if (current - 1 >= cc.Length)
                return false;
            if (cc[current + 1].StartsWith(starts))
                return true;
            if (cc[current + 1].Trim().StartsWith(starts))
                return true;
            else return false;

        }
        public String toString()
        {
            string s = "";
            foreach (string c in values.Keys)
                s += c + "\t" + values[c] + "\n";
            return s;
        }
        public class MakeFileLineProcessor
        {
            private StringBuilder lineBuffer;

            public Dictionary<string, string> values { get; set; }


            public MakeFileParser parser { get; set; }

            public bool run = false;

            public MakeFileLineProcessor(Dictionary<string, string> values, MakeFileParser parser)
            {
                this.values = values;
                this.parser = parser;
                Functions.make = this;
            }

            int current = -1;

            string[] cc { get; set; }


            public void SkipBlank()
            {
                if (cc == null)
                {
                    parser.SkipBlank();
                    return;
                }
                int c = current;

                c++;

                while (cc[c].Trim() == "" && c < cc.Length - 1)
                    c++;

                current = c--;
            }

            public string GetNextLine(bool add = true)
            {

                if (cc == null)
                    return parser.GetNextLine(add);

                if(add == true)
                current++;
                if (current >= cc.Length)
                    return "EOF";
                else return cc[current];

            }

            public bool ReadNextLine(string starts)
            {

                if (cc == null)
                    return parser.ReadNextLine(starts);

                if (current - 1 >= cc.Length)
                    return false;
                if (cc[current + 1].StartsWith(starts))
                    return true;
                if (cc[current + 1].EndsWith("\r") && cc[current + 1][0] == ' ')
                    return true;
                if (cc[current + 1].Trim().StartsWith(starts))
                    return true;
                else return false;

            }
         
            // Keep a list of LOCAL_ variables to clear when CLEAR_VARS is encountered.
            private HashSet<String> localVars = new HashSet<String>();

            public void processLines(string[] g, bool append = true)
            {

                cc = g;
                current = 0;
                while (current < g.Length)
                {
                    string s = g[current];

                    Console.WriteLine("Processing - " + s);

                    processLine(s, append);

                    current++;
                }

                cc = null;
            }

            public bool processLine(String line, bool append = false)
            {
                
                String trimmed = line.Trim();

                //        String trimmed = line;
                // Skip comments.
                if (trimmed != "" && trimmed[0] == '#')
                {
                    return true;
                }
                //if (append == false)
                {
                    appendPartialLine(trimmed, append);
                    if (append == false)
                        if (trimmed != "" && trimmed[trimmed.Length - 1] == '\\')
                        {
                            // This is a partial line.  Do not process yet.
                            return true;
                        }
                }
                String completeLine = lineBuffer.ToString().Trim();
                // Reset the line buffer.
                lineBuffer = null;
                if (isNullOrEmpty(completeLine))
                {
                    return true;
                }
                processKeyValuePairs(completeLine);
                return true;
            }

            public static string[] SplitAndTrim(string text, string separator)
            {
                if (string.IsNullOrWhiteSpace(text))
                {
                    return null;
                }

                return text.Split(separator.ToCharArray(), 2).Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();


            }


            public static bool isNullOrEmpty(string s)
            {

                if (s == null || s == "")
                    return true;
                return false;

            }
            public static bool isEmpty(string s)
            {

                if (s == "")
                    return true;
                return false;

            }

            private void processKeyValuePairs(String line)
            {


                if (line.StartsWith("ifeq ($(USE_OPENMP), 1)"))
                    line = line;
                
                if (line.StartsWith("ifeq ($(ARCH), $(filter $(ARCH),mips64 mips))"))
                    line = line;

                if (line.StartsWith("ifndef GOTOBLAS_MAKEFILE"))
                    line = line;

                if (line.Trim().StartsWith("export"))
                    line = line.Replace("export", "").Trim();

                if (line.Trim().StartsWith("include"))
                {
                    processinclude(line);
                    //return;
                }
                else if (line.Trim().StartsWith("define"))
                {
                    processdef(line);
                }
                else if (line.Trim().StartsWith("ifeq") || line.Trim().StartsWith("ifneq") || line.Trim().StartsWith("ifdef") || line.Trim().StartsWith("ifndef"))
                {
                    if (line.Trim().StartsWith("ifdef") || line.Trim().StartsWith("ifndef"))
                        line = line;
                    processifeq(line);
                }
                else
if (line.Trim().StartsWith("$"))
                {

                    if (line.Contains(":") && line.Contains(":=") == false)
                    {

                        int left = line.IndexOf("(");
                        int right = line.IndexOf(")");
                        int eq = line.IndexOf(":");


                        if (left < right && right < eq)
                        {
                            processtarget(line);
                            //return;

                        }

                        //processtarget(line);


                    }
                    else
                    {

                        int left = line.IndexOf("(");
                        int right = line.IndexOf(")");
                        int eq = line.IndexOf("=");


                        if (left > right || right > eq)
                        {
                            tokenizeValue(line);
                            // return;
                        }
                    }
                }

                if (line.Contains(":") && line.Contains(":=") == false || line.Contains("::"))
                {

                    //int left = line.IndexOf("(");
                    //int right = line.IndexOf(")");
                    //int eq = line.IndexOf(":");


                    //if (left < right && right < eq)
                    {
                        processtarget(line);
                        //return;
                    }

                }

                //ArrayList valuesArr = tokenizeValue(line);

                

                if (line.Contains("="))
                {
                    String[] arr = null;

                    if (line.Contains("+="))
                    {


                        arr = line.Split("+=".ToCharArray());

                        arr = make.RemoveEmpty(arr);

                        arr[0] = arr[0].Replace(":", "");


                    } else
                    if (line.Contains("="))
                    {


                        arr = SplitAndTrim(line, "=");

                        if(arr.Length > 1)
                        if(arr[1].StartsWith("$("))
                            if(arr[1].EndsWith(")"))
                        if (arr[1].Contains(":"))
                            arr[1] = SubstituteVars(arr[1]);


                        arr[0] = arr[0].Replace(":", "");

                        
                    }
                    //else 
                    //{
                    //    arr = SplitAndTrim(line, "\\+="); 
                    //}

                    if (arr.Length > 2)
                    {
                        int g = line.IndexOf(":=");

                        if (g >= 0)
                        {

                            arr = new string[2];

                            arr[0] = line.Substring(0, g - 1).Trim();

                            arr[1] = line.Substring(g - 1, line.Length - g + 1).Trim();

                            //Console.WriteLine("Malformed line " + line);

                        }
                    }

                    bool tokenize = true;

                    if (arr.Length >= 2)
                    {
                        string g = "";
                        if (arr[1].Trim().StartsWith("["))
                        {

                            //g = arr[1].Replace("[", "");
                            //g = arr[1].Replace("]", "");

                            //g = "$(shell " + g + ")";

                            //arr[1] = g;

                            tokenize = false;

                        }

                        
                    }

                    if (arr[0].Contains("override"))
                    {
                        arr[0] = arr[0].Replace("override", "").Trim();

                        if (parser.overrides.Contains(arr[0]) == false)
                            parser.overrides.Add(arr[0]);

                    }
                    else if (parser.overrides.Contains(arr[0].Trim()) == true)
                    {
                        return;
                    }

                    {
                        // Store the key in case the line continues
                        String key = arr[0].Trim();
                        if (arr.Length == 2)
                        {


                            bool append = false;

                            if (line.Contains("+="))
                                append = true;

                            if (line.Contains(":=") == false)
                                tokenize = false;

                            // There may be multiple values on one line.
                            ArrayList valuesArr = tokenizeValue(arr[1]);

                            if (tokenize == false)
                            {
                                if (arr.Length >= 1)
                                    if (values.ContainsKey(key) == false)
                                    {

                                        
                                        values.Add(key.Trim(), arr[1]);

                                    }
                                else if(append == true)
                                    {

                                        appendValue(key.Trim(), arr[1].Trim(), append);

                                    }
                            }

                            else
                            {
                                if (valuesArr.Count > 1)
                                    append = true;
                                foreach (String value in valuesArr)
                                {
                                    appendValue(key.Trim(), value, append);
                                }

                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Skipping line " + line);
                }
            }

            public string SubstituteVars(string v)
            {
                string s = "";

                if (v.StartsWith("$"))
                {

                    string[] cc = v.Split(":".ToCharArray());

                    string c = cc[0].Substring(2, cc[0].Length - 2);


                    string b = c;

                    if (values.ContainsKey(c))
                        b = values[c];
                    else return "";

                    string[] bf = b.Split(" ".ToCharArray());

                    string[] bb = cc[1].Split("=".ToCharArray());

                    bb[1] = bb[1].Substring(0, bb[1].Length - 1);

                    string a = bb[0];// findVariables(bb[0]);

                    string d = bb[1];// findVariables(bb[1]);

                    foreach(string g in bf)
                    {


                        string w = g;// findVariables(g);
                            
                            w = w.Replace(a, d);



                        s += " " + w; 
                    }

                }
                return s;
            }

            public enum ifq
            {
                ifeq,
                ifneq,
                ifdef,
                ifndef
            }

            public string checkCondition(ifeq q, List<string> r = null)
            {

                string c = q.ifeqs;

                ifq qs = ifq.ifeq;

                if (c.StartsWith("ifneq"))
                    qs = ifq.ifneq;
                else if (c.StartsWith("ifeq"))
                    qs = ifq.ifeq;
                else if (c.StartsWith("ifdef"))
                    qs = ifq.ifdef;
                else qs = ifq.ifndef;

                string g = c.Replace("ifeq", "");
                g = g.Replace("ifneq", "");
                g = g.Replace("ifdef", "");
                g = g.Replace("ifndef", "");

                int s0 = g.IndexOf("(");
                int s1 = g.LastIndexOf(")");

                string f = g;

                if (s1 != -1 && s0 != -1)
                {
                    if (s1 <= s0)
                        return "";
                    else
                    f = g.Substring(s0 + 1, s1 - s0 - 1);

                }

                string[] dd = f.Split(",".ToCharArray());

                string value = "";

                string key = "";

                if (dd.Length >= 2)
                {
                    
                    ArrayList valuesArr = tokenizeValue(dd[0]);

                    if (valuesArr.Count <= 0)
                        return "";

                    value = valuesArr[0] as string;

                    key = dd[1].Trim();

                }
                else
                {
                    value = GetVariable(g).Trim() ;

                    
                }

                List<object> b = q.ifeqc;


                if (value == "" && key == "")
                {
                    if (qs != ifq.ifeq)
                        b = q.ifeql;
                    else b = q.ifeqc;
                    if (qs == ifq.ifndef)
                        b = q.ifeqc;
                    if (qs == ifq.ifdef)
                        b = q.ifeql;
                }
                else if (value != key/*dd[1]*/)
                {
                    if (qs == ifq.ifeq)
                        b = q.ifeql;
                    else b = q.ifeqc;
                    if (qs == ifq.ifndef)
                        b = q.ifeql;
                    if (qs == ifq.ifdef)
                        b = q.ifeqc;
                }
            
                else
                {
                    if (qs != ifq.ifeq)
                        b = q.ifeql;
                    else b = q.ifeqc;
                    if (qs == ifq.ifdef)
                        b = q.ifeqc;
                    if (qs == ifq.ifndef)
                        b = q.ifeql;
                }

                

                foreach (object bb in b)
                {
                    if (bb == "")
                        continue;
                    if (bb.GetType() == typeof(string))
                    {
                        if (r == null) processLine(bb as string);
                        else
                        r.Add(bb as string);
                    }
                    else
                        checkCondition(bb as ifeq, r);
                }


                return "";
            }

            static string PathToWindows(string p)
            {

                string s = p;

                if (p[0] == '/')
                    s = p.Remove(0, 1);

                if (s[1] != ':')
                    s = s.Insert(1, ":");

                return s;

            }

            string processinclude(string line)
            {

                string s = line.Replace("include", "").Trim();

                if (s.StartsWith("$"))
                {

                    s = findVariables(s);
                }

                string[] cc = s.Split(" ".ToCharArray());

                foreach (string d in cc)
                {

                    if (d == "")
                        continue;

                    string file = d;

                    //file = PathToWindows(file);
                    if(file.Contains("..") == false)
                    file = file.Replace("./", "");

                    if (File.Exists(file) == false)
                        continue;

                    

                    string[] g = File.ReadAllLines(file);

                    if (g != null)

                        parser.Append(g);

                }

                return "";
            }

            string processdef(string line)
            {



                def w = new def();

                line = findVariables(line);

                w.name = line.ToUpper().Replace("DEFINE", "").Trim();

                string next = parser.GetNextLine();


                while (next.Trim().StartsWith("endef") == false)
                {

                    w.function += next + "\n";

                    next = GetNextLine();

                }
                if (parser.defs.ContainsKey(w.name) == false)
                    parser.defs.Add(w.name, w);

                return next;
            }

            public string content { get; set; }

            string processtarget(string line)
            {

                target w = new target();

                StringBuilder sb = new StringBuilder();


                string[] lines = line.Split(";".ToCharArray());


                string[] cc = line.Split(":".ToCharArray());

                string nextline = "";

                if(lines.Length > 1)
                {

                    line = lines[0];
                    cc = line.Split(":".ToCharArray());
                    nextline = lines[1];
                }
                
                if (line.Contains("::"))
                {
                    cc = line.Split("::".ToCharArray());
                    cc = make.RemoveEmpty(cc);
                    w.isDoubleColon = true;
                }

                //cc[0] = findVariables(cc[0]);

                w.name = cc[0].Trim();

                string[] bb = w.name.Split(" ".ToCharArray());

                bb = make.RemoveEmpty(bb);

                //if (w.name == "config.h")
                //    bb = bb;


                //if(w.name == ".PHONY")
                //{
                //    string init = GetNextLine();

                //    cc = init.Split(":".ToCharArray());

                //    w.name = cc[0];

                //}

                if (cc.Length > 1)
                {
                    w.prereq = cc[1];
                    w.prerequ = findVariables(cc[1]);
                }



                //SkipBlank();

                if (ReadNextLine("\t") || GetNextLine(false).Trim().StartsWith("if") == true || GetNextLine(false).Trim().StartsWith("#") == true || nextline != "")
                {

                    string text = "";

                    if (nextline != "")
                    {
                        text = nextline;
                        
                    }
                    else
                    text = GetNextLine(false);

                    if(text.StartsWith("#") == true)
                    {

                        text = GetNextLine();

                    }




                    if (text.Trim().StartsWith("if"))
                    {

                        text = GetNextLine();

                        if (w.tar == null)
                        {

                            tarmake makes = new tarmake(text, w.recp.ToArray());

                            w.tar = makes;
                        }
                        else
                        {
                            tarmake makes = new tarmake(text);

                            w.tar.Add(makes);

                        }
                        


                    }

                    string next = text;

                    if(nextline == "")

                    next = GetNextLine();

                    bool append = false;

                    while (next.StartsWith("\t") == true || (next.EndsWith("\r") && Char.IsLetterOrDigit(next[0]) == false) || next.Trim().StartsWith("if") || nextline != "")
                    {

                        nextline = "";

                        //if (next.Trim().StartsWith("define"))
                        //{

                        //    string name = next.Replace("define", "").Trim();

                        //    ReadDefines(name);

                        //}
                        //else
                        {
                            if (append == true)
                            {
                                if(w.tar != null)
                                {

                                    if (next.Trim().StartsWith("if"))
                                    {
                                        tarmake m = new tarmake(next);
                                        w.tar.Add(m);
                                    }
                                    else
                                    {
                                        //make m = new make(w.recp[w.recp.Count - 1].Replace("\t", "") + next.Trim() + " ");
                                        make m = new make(next.Trim() + " ");
                                        w.tar.Add(m);
                                    }
                                    


                                }
                                else w.recp[w.recp.Count - 1] = w.recp[w.recp.Count - 1].Replace("\t", "") + next.Trim() + " ";
                                //sb.Append(next.Trim() + " ");
                                sb.Append(next + " ");
                            }
                            else
                            {

                                if (next.Trim().StartsWith("if") && w.tar == null)
                                {

                                    //next = GetNextLine();

                                    if (w.tar == null)
                                    {

                                        tarmake makes = new tarmake(next, w.recp.ToArray());

                                        w.tar = makes;
                                    }
                                    else
                                    {
                                        tarmake makes = new tarmake(next);

                                        w.tar.Add(makes);

                                    }



                                }

                                else 
                                if (w.tar != null)
                                {
                                    if (next.Trim().StartsWith("if"))
                                    {
                                        tarmake m = new tarmake(next.Replace("\t", ""));
                                        w.tar.Add(m);
                                    }
                                    else
                                    {
                                        make m = new make(next.Replace("\t", ""));
                                        w.tar.Add(m);
                                    }
                                }
                                else
                                w.recp.Add(next.Replace("\t", ""));
                                sb.AppendLine(next.Replace("\t", ""));
                            }

                            if (next.Trim().EndsWith("\\"))
                            {
                                append = true;
                            }
                            else
                            {
                                append = false;
                            }
                        }

                        //SkipBlank();

                        next = GetNextLine(false);

                        if (next == "EOF")
                            break;

                       

                        if (next.StartsWith("#") == true)
                        {

                            next = GetNextLine();
                            next = GetNextLine(false);

                        }

                        if (ReadNextLine("\t") == false && next.Trim().StartsWith("if") == false)
                            break;

                      

                        next = GetNextLine();

                    }

                }

                

                w.content = sb.ToString();

                foreach (string s in bb)
                {

                    if (parser.targs.ContainsKey(s/*w.name*/) == false)
                    {
                        target t = target.Create(w, s);
                        parser.targs.Add(t.name, t);

                    }
                    else
                    {

                        target b = parser.targs[s /*w.name*/];
                        target t = target.Create(w, s);
                        b.next = t;//w



                    }

                }
                return "";
            }

            void ReadDefines(string name)
            {


                def defs = new def();
                defs.name = name;


                {
                    while (ReadNextLine("endef") == false)
                    {

                        string next = GetNextLine();


                        defs.function += next + "\n";



                    }

                    parser.defs.Add(defs.name, defs);


                }

            }

            string processifeq(string line)
            {

                if (line.Contains("ifdef FUNCTION_PROFILE") == true)
                    line = line;

                List<string> r = new List<string>();

                ifeq q = readIfeq(line);

                checkCondition(q, r);

                parser.Append(r.ToArray());

                return "g";
            }

            public ifeq readIfeq(string line)
            {

                if (line.Trim().StartsWith("ifeq ($(NO_LAPACK), 1)"))
                    line = line;

                ifeq w = new ifeq();

                w.ifeqs = line;

                string next = parser.GetNextLine();//.Trim();

                bool yes = true;

                bool elseif = false;

                while (next.StartsWith("endif") == false)
                {

                    if (next.StartsWith("define"))
                    {
                        next = processdef(next);

                    }
                    if (next.StartsWith("else"))
                    {
                        yes = false;

                        next = next.Replace("else", "");//.Trim();

                        if(next.Trim() == "")
                            do
                            {
                                next = parser.GetNextLine();//.Trim();
                            }
                            while (next == "" || next.StartsWith("#"));

                        if (next.StartsWith("ifeq") || next.StartsWith("ifneq") || next.StartsWith("ifdef") || next.StartsWith("ifndef"))
                        {
                            elseif = true;
                        }
                    }

                    if (next.StartsWith("ifeq") || next.StartsWith("ifneq") ||  next.StartsWith("ifdef") || next.StartsWith("ifndef"))
                    {
                        ifeq qq = readIfeq(next);
                        if (yes == true)
                        {
                            w.ifeqc.Add(qq);
                        }
                        else
                        {
                            w.ifeql.Add(qq);
                        }
                    }
                    else
                    {
                        if (yes == true)
                        {
                            w.ifeqc.Add(next);
                        }
                        else
                        {
                            w.ifeql.Add(next);
                        }
                    }

                    if (elseif == true)
                        break;

                    do
                    {
                        next = parser.GetNextLine();//.Trim();
                    }
                    while (next == "" || next.StartsWith("#"));


                }


                return w;
            }


            public class def
            {

                public string name { get; set; }

                public string function { get; set; }

            }
            public class target
            {

                public string name { get; set; }

                public string prereq { get; set; }

                public string prerequ { get; set; }

                public List<string> recp { get; set; }

                public bool isDoubleColon = false;

                public target next { get; set; }

                public tarmake tar { get; set; }

                public string content { get; set; }
                public target()
                {
                    prereq = "";
                    recp = new List<string>();
                }
                public target(string s)
                {
                    prereq = "";
                    content = s;
                    recp = new List<string>();
                }

                static public target Create(target t, string name)
                {

                    target tt = new target();
                    tt.name = name;
                    tt.prereq = t.prereq;
                    tt.prerequ = t.prerequ;
                    tt.tar = t.tar;
                    tt.recp = new List<string>();
                    tt.recp.Capacity = t.recp.Capacity;
                    foreach (string s in t.recp)
                        tt.recp.Add(s);
                    return tt;
                }


                static public List<string> MergeFunctions(List<string> s)
                {

                    List<string> d = new List<string>();

                    int i = 0;
                    while (i < s.Count)
                    {

                        //string c = s[i].Trim();
                        string c = s[i];


                        int p = 0;

                        string line = "";

                        if (c.StartsWith("$("))
                        {
                            p++;

                            //line = c;

                            int r = 2;

                            while (i < s.Count && p != 0)
                            {

                                while (r < c.Length && p != 0)
                                {
                                    if (c[r] == '(')
                                        p++;
                                    else if (c[r] == ')')
                                        p--;

                                    r++;
                                }

                                if (p == 0)
                                {
                                    line += "\t" + c;
                                    d.Add(line);
                                    break;
                                }
                                else
                                {
                                    line += "\t" + c;
                                    c = s[++i];
                                    r = 0;
                                }


                            }

                        }
                        else d.Add(c);

                        i++;
                    }



                    return d;
                }
                static public List<string> MergeFunctions(string content)
                {

                    List<string> d = new List<string>();


                    string[] s = content.Split("\t".ToCharArray());

                    int i = 0;
                    while (i < s.Length)
                    {

                        string c = s[i].Trim();


                        int p = 0;

                        string line = "";

                        if (c.StartsWith("$("))
                        {
                            p++;

                            //line = c;

                            int r = 2;

                            while (i < s.Length && p != 0)
                            {

                                while (r < c.Length && p != 0)
                                {
                                    if (c[r] == '(')
                                        p++;
                                    else if (c[r] == ')')
                                        p--;

                                    r++;
                                }

                                if (p == 0)
                                {
                                    line += c;
                                    d.Add(line);
                                    break;
                                }
                                else
                                {
                                    line += c;
                                    c = s[++i].Trim();
                                    r = 0;
                                }


                            }

                        }
                        else d.Add(c);

                        i++;
                    }



                    return d;
                }



                public List<string> Expand()
                {



                    prereq = prereq.Replace("!@#$", "\\");

                    int i = 0;
                    while (i < recp.Count)
                    {
                        recp[i] = recp[i].Replace("!@#$", "\\");
                        i++;

                    }

                    List<string> d = new List<string>();

                    //string c = prerequ;
                    if (prereq != "")
                        recp.Insert(0, prereq);

                    i = 0;
                    while (i < recp.Count)
                    {
                        string s = recp[i];

                        string c = "";

                        //if (s.Contains(":"))

                        while (s.EndsWith("\\") || s.EndsWith("\\\r"))
                        {

                            s = s.Replace("\\", "");
                            c += s + " ";
                            i++;
                            if (i >= recp.Count)
                                break;
                            s = recp[i];

                        }
                        c += s + " ";

                        d.Add(c);

                        i++;


                    }

                    if (prereq != "")
                    {
                        if (prereq.EndsWith("\\"))
                        {
                            prereq = d[0];
                            d.RemoveAt(0);
                        }
                    }

                    return d;

                }
                public List<string> Expands()
                {

                    string[] rec = content.Split("\n".ToCharArray());

                    List<string> recp = new List<string>(rec);

                    if (prereq != null)
                        prereq = prereq.Replace("!@#$", "\\");

                    int i = 0;
                    while (i < recp.Count)
                    {
                        recp[i] = recp[i].Replace("!@#$", "\\");
                        i++;

                    }

                    List<string> d = new List<string>();

                    //string c = prerequ;
                    if (prereq != "")
                        recp.Insert(0, prereq);

                    i = 0;
                    while (i < recp.Count)
                    {
                        string s = recp[i];

                        string c = "";

                        //if (s.Contains(":"))

                        int v = 0;
                        while (s.Trim().EndsWith("\\") || s.EndsWith("\\\r") || s.EndsWith("\\\r"))
                        {

                            s = s.Replace("\\", "");
                            c += s + " ";
                            i++;

                            if (i >= recp.Count)
                                break;
                            s = recp[i];

                        }
                        c += s + " ";

                        d.Add(c);

                        i++;


                    }

                    if (prereq != "")
                    {
                        if (prereq.EndsWith("\\"))
                        {
                            prereq = d[0];
                            d.RemoveAt(0);
                        }
                    }

                    d = target.MergeFunctions(d);

                    return d;

                }

            }

            public class ifeq
            {
                public ifeq q;
                public string ifeqs { get; set; }
                public List<object> ifeqc = new List<object>();
                public List<object> ifeql = new List<object>();

            }

            public void appendPartialLine(String line, bool append = false)
            {
                if (lineBuffer == null)
                {
                    lineBuffer = new StringBuilder();
                }
                else
                {
                    lineBuffer.Append(" ");
                }
                if (line.EndsWith("\\") && append == false)
                {
                    lineBuffer.Append(line.Substring(0, line.Length - 1).Trim());
                }
                else
                {
                    lineBuffer.Append(line);
                }
            }
            private ArrayList tokenizeValue(String rawValue)
            {
                String value = rawValue.Trim();
                ArrayList result = new ArrayList();
                if (isEmpty(value))
                {
                    return result;
                }
                // Value may contain function calls such as "$(call all-java-files-under)" or refer
                // to variables such as "$(my_var)"
                value = findVariables(value);
                String[] tokens = value.Split(" ".ToCharArray());
                result.AddRange(tokens);
                //if (tokens.Length > 2)
                //    result.Insert(0, "function");
                return result;
            }
            public String findVariables(String value, bool defines = false)
            {

                string content = value;

                if(value.Length == 1)
                {

                    //if (values.ContainsKey(value))
                    //    return values[value];
                    //else
                        return value;


                }

                int variableStart = value.IndexOf("$(");
                // Keep going until we substituted all variables.
                while (variableStart > -1)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(value.Substring(0, variableStart));
                    // variable found
                    int variableEnd = findClosingParen(value, variableStart);
                    if (variableEnd > variableStart)
                    {
                        string s = value.Substring(variableStart + 2, variableEnd - variableStart - 2);
                        String result = substituteVariables(s, defines);
                        if (s.Trim().StartsWith("shell"))
                        {
                            Console.WriteLine("Function shell called - " + result);

                            s = result;

                            s = s.Replace("shell", "").Trim();

                            if (s != "")
                            {

                                string cmds = "";

                                make.parser.cmd.R = new ArrayList();

                                make.parser.cmd.handled = false;

                                make.parser.cmd.output = "";

                                if (s.StartsWith("sed"))
                                    s = s;

                                string es = "err_report() {  echo 'end' }\ntrap 'err_report' ERR\n";


                                //make.parser.cmd.SendCommand("\necho start\n(\n" + s + " )\n echo end");

                                make.parser.cmd.SendCommand("\necho start\n(" + s + " | xargs echo; echo end;)\n echo end; \n echo end;");



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
                                    result = cmds;

                                }
                            }
                        }
                        else
                        if (s.Trim().StartsWith("if"))
                        {
                            Console.WriteLine("Function if called - " + result);

                            if (result.Contains("wget"))
                                s = s;

                            s = result;

                            s = s.Replace("if", "").Trim();



                            List<string> bb = target.MergeFunctions(s);

                            List<string> dd = target.MergeFunctions(bb);

                            string[] cc = s.Split(",".ToCharArray());


                            if (cc[0].Trim() != "" || result.Contains("wget"))
                                result = cc[1];
                            else
                            {
                                //if (cc.Length >= 3)
                                //    result = cc[2];
                                //else
                                result = "";
                            }

                        }
                        else
                        if (s.Trim().StartsWith("patsubst"))
                        {
                            Console.WriteLine("Function patsubst called - " + result);

                            s = result;

                            s = s.Replace("patsubst", "").Trim();

                            string[] cc = s.Split(",".ToCharArray());


                            string rep = cc[1];

                            string text = cc[2];

                            string[] dd = text.Replace("\n", " ").Split(" ".ToCharArray());


                            result = "";

                            foreach (string b in dd)
                            {

                                string g = rep.Replace("%", b);

                                result += g + " ";


                            }


                        }
                        else
                        if (s.Trim().StartsWith("filter-out"))
                        {
                            Console.WriteLine("Function filter-out called - " + result);

                            s = result;

                            s = s.Replace("filter-out", "").Trim();

                            string[] cc = s.Split(",".ToCharArray());

                            string[] pats = cc[0].Split("\n".ToCharArray());

                            string[] texts = cc[1].Split("\n".ToCharArray());

                            result = "";

                            foreach (string b in texts)
                            {

                                bool found = false;

                                foreach (string g in pats)
                                {
                                    if (g == b)
                                    {
                                        found = true;
                                        break;
                                    }
                                    if (found == false)
                                        result += b + " ";
                                }


                            }

                        }
                        else if (s.Trim().StartsWith("addprefix"))
                        {
                            Console.WriteLine("Function addprefix called - " + result);

                            if (s.Contains("$"))
                                s = findVariables(s);

                            s = s.Replace("addprefix", "").Trim();

                            string[] cc = s.Split(",".ToCharArray());

                            result = "";

                            if (cc.Length >= 2)
                            {


                                string prefix = cc[0];

                                string[] dd = cc[1].Replace("\n", " ").Split(" ".ToCharArray());

                                foreach (string c in dd)

                                    result += (prefix + c + " ");


                            }


                        }
                        else
                        if (s.Trim().StartsWith("filter"))
                        {
                            Console.WriteLine("Function filter called - " + result);

                            s = result;

                            s = s.Replace("filter", "").Trim();

                            string[] cc = s.Split(",".ToCharArray());

                            if (cc[1] == cc[0])

                                result = cc[0];
                            else
                                result = "";



                        }
                        else
                        if (s.Trim().StartsWith("value"))
                        {

                            s = result;

                            s = s.Replace("value", "").Trim();


                            string g = GetVariable(s);

                            if (g == "")
                                g = GetDefine(s);

                            result = g;


                        }
                        else
                        if (s.Trim().StartsWith("wildcard"))
                        {
                            //Console.WriteLine("Function wildcard called - " + result);

                            if (s.Contains("$"))
                                s = findVariables(s);

                            s = result;

                            s = s.Replace("wildcard", "").Trim();

                            if (s.Contains("$"))
                                s = findVariables(s);



                            //string[] dirs = Directory.GetFiles(@"c:\", "c*");



                            if (s.StartsWith("/"))
                                s = s.Substring(1);

                            if (s.IndexOf("/") == 1)
                                s = s[0] + ":/" + s.Substring(2);


                            string pattern = s;

                            if (s.Contains("\\") || s.Contains("/"))
                            {
                                string[] cc = s.Split("\\/".ToCharArray());

                                if (cc.Length > 1)
                                    pattern = cc[cc.Length - 1];
                                else
                                    pattern = cc[0];
                            }

                            string dir = s.Replace(pattern, "");

                            if (dir == "" || dir == "\\")
                                dir = AppDomain.CurrentDomain.BaseDirectory;

                            if (Directory.Exists(dir) == false)
                                Console.WriteLine("No dir found");
                            if (File.Exists(s))
                                result = s;
                            if (Directory.Exists(s))
                                result = s;

                            string[] dirs = Directory.GetFiles(@dir, pattern);

                            result = "";

                            foreach (string p in dirs)
                                result += p + " ";

                        }
                        else if (s.Trim().StartsWith("call"))
                        {
                            if (defines == true)
                            {

                                string r = s.Trim().Replace("call", "").Trim();

                                string[] cc = r.Split(",".ToCharArray());

                                string c = cc[0];

                                if (parser.defs.ContainsKey(c) == true)
                                {

                                    string g = parser.defs[c].function;

                                    if (cc.Length >= 1)
                                        g = g.Replace("$(1)", cc[1]);

                                    r = r.Replace(c, g);

                                    result = r;

                                }
                                else if (parser.values.ContainsKey(c))
                                {

                                    string g = parser.values[c];

                                    if (cc.Length >= 1)
                                        g = g.Replace("$(1)", cc[1]);

                                    r = r.Replace(c, g);

                                    result = r;



                                }

                            }
                        }
                        sb.Append(result);
                    }
                    else
                    {
                        Console.WriteLine("Malformed variable reference in make file: " + value);
                        return content;
                    }
                    if (variableEnd + 1 < value.Length)
                    {
                        sb.Append(value.Substring(variableEnd + 1));
                    }
                    value = sb.ToString();
                    variableStart = value.IndexOf("$(");
                }
                return value;
            }
            private int findClosingParen(String value, int startIndex)
            {
                int openParenCount = 0;
                for (int i = startIndex; i < value.Length; i++)
                {
                    char ch = value[i];
                    if (ch == ')')
                    {
                        openParenCount--;
                        if (openParenCount == 0)
                        {
                            return i;
                        }
                    }
                    else if (ch == '(')
                    {
                        openParenCount++;
                    }
                }
                return -1;
            }
            /**
             * Look for and handle $(...) variables.
             */

            public string GetVariable(string s)
            {

                string c = s.Replace("$", "");
                c = c.Replace("(", "");
                c = c.Replace(")", "");
                c = c.Trim();

                if (values.ContainsKey(c))
                    return values[c];
                else return "";


            }
            public string GetDefine(string s)
            {

                string c = s.Replace("$", "");
                c = c.Replace("(", "");
                c = c.Replace(")", "");
                c = c.Trim();

                if (parser.defs.ContainsKey(c.ToUpper()))
                    return c;
                else return "";


            }
            public string substituteDefines(string rawValue)
            {

                return "";




            }


            public String substituteVariables(String rawValue, bool defines = false)
            {
                if (isEmpty(rawValue))
                {
                    return rawValue;
                }
                String value = rawValue;
                if (value.StartsWith("call all-java-files-under"))
                {
                    // Ignore the call and function, keep the args.
                    value = value.Substring(25).Trim();
                }
                else if(value.Length == 1)
                {
                    if (values.ContainsKey(value))
                        return values[value];
                    else
                    return value;
                }
          
                else if (value.StartsWith("call"))
                {
                    Console.WriteLine("call for " + rawValue);

                    string[] cc = rawValue.Split(" ".ToCharArray());

                    cc = rawValue.Replace("call", "").Trim().Split(",".ToCharArray());

                    if (cc.Length > 1)
                    {
                        string s = GetVariable(cc[0]);
                        if (s != "")
                            value = rawValue.Replace(cc[0], s);

                    }
                   // cc = rawValue.Replace("call", "").Trim().Split(",".ToCharArray());

                    if(cc.Length == 2 && cc[0].StartsWith("$") == false)
                    {

                        string first = GetVariable(cc[0]);

                        // string second = GetVariable(cc[1]);

                        string second = cc[1];

                        value = first +  second;

                        return value;

                    }


                    if (cc.Length > 1)
                    {
                        string s = GetVariable(cc[1]);
                        if (s != "")
                        {
                            value = rawValue.Replace(cc[1], s);
                           
                        }
                        values["1"] = s;
                    }
                    if (cc.Length > 2)
                    {
                        
                        string s = GetVariable(cc[2]);
                        if (s != "")
                            value = value.Replace(cc[2], s);
                        values["2"] = s;
                    }
                }
                else
                                if (rawValue.Trim().StartsWith("value"))
                {


                    string s = rawValue.Replace("value", "").Trim();


                    string g = GetVariable(s);

                    if (g == "")
                        g = GetDefine(s);

                    value = g;

                    return g;

                }
                else if (value.StartsWith("foreach"))
                {
                    Console.WriteLine("foreach called");

                    string[] cc = value.Split(",".ToCharArray(), 3);

                    string c = cc[0];

                    c = c.Replace("foreach", "").Trim();

                    c = "$(" + c + ")";

                    string r = cc[1];

                    r = GetVariable(r.Trim());

                    string d = cc[2];

                    string[] rr = r.Split(VALUE_DELIMITER.ToCharArray());

                    foreach (string w in rr)
                        if (w != "")
                        {
                            d = cc[2];
                            if (run == false)
                                d = d.Replace(c, w);
                            else d = d.Replace(c, w + "d");

                            /// Console.WriteLine("foreach function call -> " + " " + d);
                            /// 
                            MakeFileLineProcessor lp = new MakeFileLineProcessor(values, parser);
                            Functions.function(findVariables(d), lp, run);

                        }

                }
                // Check for single variable
                if (value.IndexOf(' ') == -1)
                {
                    // Substitute.
                    if (values.ContainsKey(value) == false)
                        return "";
                    else return values[value];
                }
                else
                {
                    return findVariables(value, defines);
                }
            }

            public Object getResult()
            {
                return null;
            }
            /**
             * Add a value to the hash map. If the key already exists, will append instead of
             * over-writing the existing value.
             *
             * @param key The hashmap key
             * @param newValue The value to append.
             */
            private void appendValue(String key, String newValue, bool append)
            {
                String value = "";

                newValue = findVariables(newValue);

                if (key.StartsWith("$"))
                    key = findVariables(key);

                if (values.ContainsKey(key.Trim()) == false)

                {
                    values[key.Trim()] = newValue;
                }
                else
                {
                    string c = values[key.Trim()];

                    if (c.Trim() != newValue.Trim())
                    {

                        string v = values[key.Trim()];

                        string[] vv = v.Split(VALUE_DELIMITER.ToCharArray());

                        if (append == true)
                        {

                            bool found = false;

                            foreach (string g in vv)
                                if (g == newValue.Trim())
                                {
                                    found = true;
                                    break;
                                }
                            if (found == false)
                                values[key.Trim()] = values[key.Trim()] + VALUE_DELIMITER + newValue;

                        }
                        else values[key.Trim()] = newValue;

                    }
                }
            }
        }


        public static void PrintUsage()
        {


            Console.WriteLine("Program call:");
            Console.WriteLine("Make -C <> -f target:");



        }

        public static void Main(String[] args)
        {

            Settings settings = new Settings();

            settings.dir  = AppDomain.CurrentDomain.BaseDirectory;

            int i = 0;
            if (args != null && args.Length > 0)
            {
                string text = "";
                while (i < args.Length)
                {
                    string s = args[i];

                    text += " " + s;
                    
                    i++;
                }
                File.AppendAllText("C:\\Users\\user\\log.txt", "\nParameters " + text);
                i = 0;
                while (i < args.Length)
                {
                    string s = args[i];

                    if (s.ToLower().StartsWith("=") == false)
                    {

                        if (s.Contains("=") == true)
                        {

                            string[] cc = s.Split("=".ToCharArray());

                            if (cc.Length >= 2)
                            {

                                cc = make.RemoveEmpty(cc);

                                settings.E.Add(s);

                                i++;

                                continue;
                            }

                        }

                    }
                    i++;
                }

                if (settings.E != null)
                {

                    foreach (string g in settings.E)
                    {
                        if (g.StartsWith("PWD="))
                        {
                            string pwd = g.Replace("PWD=", "");
                            settings.PWD = pwd.Replace("\\","\\\\");
                        }
                    }

                }
                i = 0;
                while(i < args.Length)
                {
                    string s = args[i];

                    //if (s.ToLower().StartsWith("=") == false)
                    //{

                    //    if (s.Contains("=") == true)
                    //    {

                    //        string[] cc = s.Split("=".ToCharArray());

                    //        if (cc.Length >= 2)
                    //        {

                    //            cc = make.RemoveEmpty(cc);

                    //            settings.E.Add(s);

                    //            i++;

                    //            continue;
                    //        }

                    //    }

                    //}

                    if (s.ToLower() == "-b" || s.ToLower() == "-m")
                    {


                    }
                    else
                    if (s.ToLower() == "-d" || s.ToLower().StartsWith("--debug"))
                    {

                        settings.debug = true;
                    }
                    else
                    if (s.ToLower() == "-e" || s.ToLower().StartsWith("--environment-overrides"))
                    {


                    }
                    else if (s.ToLower() == "-c" || s.ToLower().StartsWith("--directory="))
                    {
                        if (s.ToLower() == "-c")
                        {

                            string f = AppDomain.CurrentDomain.BaseDirectory;

                            string file = args[++i];
                            if(Path.IsPathRooted(file))
                            settings.dir = file;
                            else
                                settings.dir = settings.PWD + "\\" + file;

                            settings.dir = Path.GetFullPath(settings.dir).Replace("\\","\\\\");
                            FileStream fs = null;
                            if (File.Exists("C:\\Users\\user\\log.txt") == false)
                            {
                                fs = File.Create("C:\\Users\\user\\log.txt");
                                fs.Close();
                            }
                            File.AppendAllText("C:\\Users\\user\\log.txt", "\nDirectory will be changed " + settings.dir + "\nPWD " + settings.PWD + "\nFile " + file);

                            if (Directory.Exists(settings.dir) == false)
                            {
 //                               try
                                {
                                    if (File.Exists("C:\\Users\\user\\log.txt") == false)
                                    {
                                        fs = File.Create("C:\\Users\\user\\log.txt");
                                        fs.Close();
                                    }
                                    File.AppendAllText("C:\\Users\\user\\log.txt","\nDirectory not found " + settings.dir);
                                    //Console.WriteLine("Directory not found " + settings.dir);
                                    //Console.WriteLine("Press any key to finish");
                                    //Console.ReadKey();
                                }
//                                catch(Exception e) { }
                                return;
                            }

                            Directory.SetCurrentDirectory(settings.dir);
                        }

                    }
                    else if (s.ToLower() == "-f" || s.ToLower().StartsWith("--file=") || s.ToLower().StartsWith("--makefile="))
                    {
                        if (s == "-f")
                            settings.Makefile = args[++i];

                    }
                    else
                    if (s.ToLower() == "-h" || s.ToLower().StartsWith("--help"))
                    {


                    }
                    else
                    if (s.ToLower() == "-i" || s.ToLower().StartsWith("--ignore-errors"))
                    {


                    }
                    else if (s.ToLower() == "-n" || s.ToLower().StartsWith("--just-print") || s.ToLower().StartsWith("--dry-run") || s.ToLower().StartsWith("--recon"))
                    {


                    }
                    else
                    if (s.ToLower() == "-p" || s.ToLower().StartsWith("--print-data-base"))
                    {


                    }
                    else
                    if (s.ToLower() == "-q" || s.ToLower().StartsWith("--question"))
                    {


                    }
                    else
                    if (s.ToLower() == "-s" || s.ToLower().StartsWith("--silent") || s.ToLower().StartsWith("--quiet"))
                    {


                    }
                    else {
                        settings.target = s;

                    }

                    i++;
                }

            }

            MakeFileParser.settings = settings;


            string Makefile = "Makefile";

            if(settings.Makefile != null)
            Makefile = settings.Makefile;// "Makefiles";

            if(File.Exists(Makefile) == false)
            {
                PrintUsage();
                return;
            }

            MakeFileParser parser = new MakeFileParser(Makefile);

            make.parser = parser;

            try
            {
                parser.parse();

                Functions.target(settings.target);

                //Functions.target("build-only-default-octave");

                //Functions.target("$(STAMP_DIR)/default-octave");
            }
            catch (Exception e)
            {
                e.StackTrace.ToString();
                File.AppendAllText("logger", "\nApplication error " + e.StackTrace.ToString());
            }

            //Thread.Sleep(90000);

           // while (true) ;

		int bbb = 10;

            Console.WriteLine(parser.toString());
        }
    }

}
