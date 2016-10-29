using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

//using Mono.Reflection;
namespace apps
{
    internal class EventPublisher
    {
        public event EventHandler TestEvent;

        public void RaiseEvent()
        {
            TestEvent(this, EventArgs.Empty);
        }
    }

    internal class Test
    {
        private void HandleEvent(object sender, EventArgs e)
        {
            Console.WriteLine("HandleEvent called");
        }

        static public void Main2()
        {
            // Find the handler method
            Test test = new Test();
            EventPublisher publisher = new EventPublisher();
            MethodInfo method = typeof(Test).GetMethod
                ("HandleEvent", BindingFlags.NonPublic | BindingFlags.Instance);

            // Subscribe to the event
            EventInfo eventInfo = typeof(EventPublisher).GetEvent("TestEvent");
            Type type = eventInfo.EventHandlerType;
            Delegate handler = Delegate.CreateDelegate(type, test, method);

            // Raise the event
            eventInfo.AddEventHandler(publisher, handler);
            publisher.RaiseEvent();
        }
    }

    public class reflect
    {
        public reflect()
        {
        }

        public Type D = null;

        public ListBox lb = null;

        public TextBox tb0 = null;

        public ArrayList pp = null;

        public TextBox tex = null;

        public TextBox tex2 = null;

        public ArrayList p = null;

        static public ArrayList GetProperties(object b, ListBox lb)
        {
            //reflect.PopulatePart_properties(

            //IEnumerable ea = reflect.EnumerateFields(b.buffer[0]);

            //ArrayList L = reflect.ie_to_ar(ea);

            //L = reflect.get_fields(L);

            ArrayList p = new ArrayList();

            if (b == null)
                return p;

            Type T = null;

            Type D = typeof(Type);

            string a = D.FullName;

            string c = b.GetType().FullName;

            if (c != "System.RuntimeType")

                // T = (Type)b;
                T = b.GetType();
            else
            {
                //lb.Items.Clear();
                //lb.Items.Add(b.GetType().FullName.ToString());
                //return p;

                T = (Type)b;
            }
            ArrayList L = reflect.getProperties(T);

            string[] g = reflect.get_full_names2(typeof(PropertyInfo), L);

            //c.add(g);

            p.AddRange(L.ToArray());

            utils.helpers.get_strs_contents(lb, g);

            return p;
        }

        public string filename = "";

        public void ListBoxIndexChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;

            int index = 0;

            index = lb.SelectedIndex;

            if (index < 0)
                return;

            filename = lb.Items[index].ToString();

            if (tb0 == null)
                return;

            tb0.Text = filename;
        }

        public void TreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            TreeNode node = e.Node;

            if (node.Tag == null)
                return;

            object obs = (object)node.Tag;

            //if (obs.GetType() == typeof(Type))
            //    D = (Type)obs;

            if (lb == null)
                return;

            p = GetProperties(obs, lb);

            tex.Text = obs.GetType().FullName;

            tex.Tag = obs;

            tex2.Text = node.Text;
        }

        static public void load_assembly(string s, TreeView tv)
        {
            try
            {
                Assembly ase = Assembly.LoadFile(s);
                //reflect.PopulateTree);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        static public void load_control_hier(Control c, TreeView tv, TreeNode node)
        {
            ArrayList L = new ArrayList();

            L = utils.helpers.getallcontrols(c, L, tv, null);
        }

        static public void load_control_hier(Control c, TreeView tv)
        {
            ArrayList L = new ArrayList();

            L = utils.helpers.getallcontrols(c, L);
        }

        static public void load_control(Control c, TreeView tv)
        {
            ArrayList L = new ArrayList();

            L = utils.helpers.getallcontrols(c, L);

            MessageBox.Show(L.Count.ToString() + " Controls detected");

            reflect.PopulateTreeObject(L, tv);

            //Type T = c.GetType();

            //T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            //T.GetMembers();

            //T.GetProperties();

            //T.GetMethods();

            //T.GetConstructors();

            //T.GetEvents();

            //T.GetNestedTypes();

            //T.GetEnumNames();

            //T.GetInterfaces();
        }

        //static public void PopulateTreeObject(ArrayList L, TreeView tv)
        //{
        //    tv.Nodes.Clear();

        //    //TreeNode newNode = new TreeNode("object tree");
        //    //newNode.Tag = L;
        //    foreach (Control c in L)
        //    {
        //        string name = c.Name.ToString();

        //        if (name == "")
        //            name = c.GetType().FullName.ToString();

        //        TreeNode ns = new TreeNode(name);

        //        ns.Tag = c;

        //        tv.Nodes.Add(ns);

        //        PopulatePart_methods(ns, c.GetType());

        //        PopulatePart_fields(ns, c.GetType());

        //        PopulatePart_properties(ns, c.GetType());

        //        PopulatePart_members(ns, c.GetType());

        //        PopulatePart_events(ns, c.GetType());

        //        PopulatePart_nestedtypes(ns, c.GetType());

        //    }
        //}

        private void PopulateTree(TreeView tv, Assembly ase)
        {
            TreeNode newNode = new TreeNode(ase.GetName().Name);
            newNode.Tag = ase;
            tv.Nodes.Add(newNode);

            foreach (Module mod in ase.GetModules())
            {
                AddModule(mod, newNode);
            }
        }

        static public void load_object(Object obj, TreeView tv)
        {
            ArrayList L = new ArrayList();

            Type T = obj.GetType();

            //L = utils.helpers.getallcontrols(c, L);

            //MessageBox.Show(L.Count.ToString() + " Controls detected");

            PopulateTree_Type(T, tv);

            //Type T = c.GetType();

            //T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            //T.GetMembers();

            //T.GetProperties();

            //T.GetMethods();

            //T.GetConstructors();

            //T.GetEvents();

            //T.GetNestedTypes();

            //T.GetEnumNames();

            //T.GetInterfaces();
        }

        //static public Type[] get_forms(Type a)
        //{
        //    //foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System.") || !a.FullName.StartsWith("Microsoft.")))
        //    //{
        //    //    var types = a.GetTypes().Where(t => (typeof(Form).IsAssignableFrom(t) || typeof(UserControl).IsAssignableFrom(t)) && t.IsClass && t.FullName.StartsWith("YourNamespace."));

        //    //}

        //    //foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System.") || !a.FullName.StartsWith("Microsoft.")))
        //    //{
        //    var types = a.GetFields().Where(t => (typeof(Form).IsAssignableFrom(t) || typeof(UserControl).IsAssignableFrom(t)));

        //    FieldInfo[] T = types;

        //    return T;

        //    //}

        //}

        private IEnumerable<Component> EnumerateComponents()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Component).IsAssignableFrom(field.FieldType)
                   let component = (Component)field.GetValue(this)
                   where component != null
                   select component;
        }

        static public string[] get_full_names(ArrayList L)
        {
            int N = L.Count;

            string[] s = new string[N];
            int i = 0;
            foreach (Control c in L)
            {
                s[i] = c.Name;
                i++;
            }

            return s;
        }

        static public string[] get_full_names2(ArrayList L)
        {
            int N = L.Count;

            string[] s = new string[N];
            int i = 0;
            foreach (FieldInfo c in L)
            {
                s[i] = c.Name;
                i++;
            }

            return s;
        }

        static public string[] get_full_names2(Type T, ArrayList L)
        {
            int N = L.Count;

            string[] s = new string[N];

            if (T == typeof(FieldInfo))
            {
                int i = 0;

                foreach (FieldInfo c in L)
                {
                    s[i] = c.Name;
                    i++;
                }
            }
            else if (T == typeof(PropertyInfo))
            {
                int i = 0;

                foreach (PropertyInfo c in L)
                {
                    s[i] = c.Name;
                    i++;
                }
            }

            return s;
        }

        static public ArrayList get_fields(ArrayList ms)
        {
            ArrayList L = new ArrayList();

            foreach (FieldInfo f in ms)
            {
                if (f.FieldType.IsSubclassOf(typeof(Form)))
                    L.Add(f);
            }

            return L;
        }

        public IEnumerable<Control> EnumerateControls()
        {
            return from field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Control).IsAssignableFrom(field.FieldType)
                   let component = (Control)field.GetValue(this)
                   where component != null
                   select component;
        }

        static public IEnumerable<Control> EnumerateControls(object p)
        {
            return from field in p.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Control).IsAssignableFrom(field.FieldType)
                   let component = (Control)field.GetValue(p)
                   where component != null
                   select component;
        }

        static public IEnumerable<FieldInfo> EnumerateFields(object p)
        {
            return from field in p.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Control).IsAssignableFrom(field.FieldType)
                   //let component = (Control)field.GetValue(p)
                   //where component != null
                   //select component;
                   select field;
        }

        static public IEnumerable<FieldInfo> EnumerateFieldsw(Type p)
        {
            return from field in p.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   where typeof(Control).IsAssignableFrom(field.FieldType)
                   //let component = (Control)field.GetValue(p)
                   //where component != null
                   //select component;
                   select field;
        }

        static public ArrayList ie_to_ar(IEnumerable e)
        {
            ArrayList L = new ArrayList();

            foreach (var _item in e)
            {
                L.Add(_item);
            }

            return L;
        }

        static public void FillControls(Control control, List<Control> AllControls)
        {
            String controlName = "";
            controlName = control.Name;

            foreach (Control c in control.Controls)
            {
                controlName = c.Name;
                if ((control.Controls.Count > 0))
                {
                    AllControls.Add(c);
                    FillControls(c, AllControls);
                }
            }
        }

        public void WriteTrace(object sender, EventArgs e, string eventName)
        {
            Control c = (Control)sender;
            //   Console.WriteLine("Control: " + f.Name + ", Event: " + eventName);
        }

        public void SubscribeEvent(Control control, string eventName)
        {
            EventInfo eInfo = control.GetType().GetEvent(eventName);

            if (eInfo != null)
            {
                // create a dummy, using a closure to capture the eventName parameter
                // this is to make use of the C# closure mechanism
                EventHandler dummyDelegate = (s, e) => WriteTrace(s, e, eventName);

                // Delegate.Method returns the MethodInfo for the delegated method
                Delegate realDelegate = Delegate.CreateDelegate(eInfo.EventHandlerType, dummyDelegate.Target, dummyDelegate.Method);

                eInfo.AddEventHandler(control, realDelegate);
            }
        }

        public static T GetFieldValue<T>(object obj, string fieldName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var field = obj.GetType().GetField(fieldName, BindingFlags.Public |
                                                          BindingFlags.NonPublic |
                                                          BindingFlags.Instance);

            if (field == null)
                throw new ArgumentException("fieldName", "No such field was found.");

            if (!typeof(T).IsAssignableFrom(field.FieldType))
                throw new InvalidOperationException("Field type and requested type are not compatible.");

            return (T)field.GetValue(obj);
        }

        public void SubscribeEvents(Control control)
        {
            Type controlType = control.GetType();
            FieldInfo[] fields = controlType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // MethodInfo method = typeof("Trace").GetMethod("WriteTrace");

            // "button1" hardcoded for the sample
            FieldInfo f = controlType.GetField("button1", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // "Click" hardcoded for the sample
            EventInfo eInfo = f.FieldType.GetEvent("Click");

            Type t = f.FieldType;
            object o = Activator.CreateInstance(t);

            f.GetValue(o);

            if (eInfo != null)
            {
                //EventHandler dummyDelegate = (s, e) => WriteTrace(s, e, eInfo.Name);
                //Delegate realDelegate = Delegate.CreateDelegate(eInfo.EventHandlerType, dummyDelegate.Target, dummyDelegate.Method);
                //eInfo.AddEventHandler(o, realDelegate); // Why can I refer to the variable button1 ???
            }
        }

        static public FieldInfo[] getFields(Type T)
        {
            FieldInfo[] ms = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            return ms;
        }

        static public void PopulatePart_methods(TreeNode nw, Type T)
        {
            MethodInfo[] ms = T.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Methods");

            ng.Tag = T;

            nw.Nodes.Add(ng);

            int i = 0;

            foreach (MethodInfo m in ms)
            {
                TreeNode np = new TreeNode(m.Name.ToString());

                ng.Nodes.Add(np);

                np.Tag = m;

                try
                {
                    //DelegateDecompiler.MethodBodyDecompiler mbd = new DelegateDecompiler.MethodBodyDecompiler(m);

                    //LambdaExpression e = mbd.Decompile();

                    //string s = e.ToString();

                    //tb.AppendText("\r\r\r\r" + s);
                }
                catch (Exception ex) { };

                i++;

                if (i > 3)
                    break;
            }
        }

        static public void PopulatePart_nestedtypes(TreeNode nw, Type T)
        {
            Type[] ms = T.GetNestedTypes(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Nested Types");

            ng.Tag = T;

            nw.Nodes.Add(ng);

            foreach (Type m in ms)
            {
                TreeNode np = new TreeNode(m.Name.ToString());

                ng.Nodes.Add(np);

                np.Tag = m;
            }
        }

        static public void PopulatePart_events(TreeNode nw, Type T)
        {
            EventInfo[] ms = T.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Events");

            ng.Tag = T;

            nw.Nodes.Add(ng);

            foreach (EventInfo m in ms)
            {
                TreeNode np = new TreeNode(m.Name.ToString());

                ng.Nodes.Add(np);

                np.Tag = m;
            }
        }

        static public void PopulatePart_properties(TreeNode nw, Type T)
        {
            PropertyInfo[] ms = T.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Properties");

            ng.Tag = T;

            nw.Nodes.Add(ng);

            foreach (PropertyInfo m in ms)
            {
                TreeNode np = new TreeNode(m.Name.ToString());

                ng.Nodes.Add(np);

                ng.Tag = m;
            }
        }

        static public ArrayList get_FieldInfos(ArrayList b, string[] g)
        {
            ArrayList L = new ArrayList();

            foreach (string s in g)
            {
                foreach (FieldInfo p in b)
                {
                    if (p.Name == s)
                    {
                        L.Add(p);

                        break;
                    }
                }
            }

            return L;
        }

        static public ArrayList get_PropertyInfos(ArrayList b, string[] g)
        {
            ArrayList L = new ArrayList();

            foreach (string s in g)
            {
                foreach (PropertyInfo p in b)
                {
                    if (p.Name == s)
                    {
                        L.Add(p);

                        break;
                    }
                }
            }

            return L;
        }

        static public ArrayList getProperties(Type T)
        {
            PropertyInfo[] ms = T.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Properties");

            //ng.Tag = T;

            //nw.Nodes.Add(ng);

            ArrayList L = new ArrayList();

            foreach (PropertyInfo m in ms)
            {
                //  TreeNode np = new TreeNode(m.Name.ToString());

                //  ng.Nodes.Add(np);

                //ng.Tag = m;

                L.Add(m);
            }

            return L;
        }

        static public void PopulatePart_fields(TreeNode nw, Type T)
        {
            FieldInfo[] ms = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Fields");

            ng.Tag = T;

            nw.Nodes.Add(ng);

            foreach (FieldInfo m in ms)
            {
                TreeNode np = new TreeNode(m.Name.ToString());

                ng.Nodes.Add(np);

                ng.Tag = m;
            }
        }

        static public void PopulatePart_members(TreeNode nw, Type T)
        {
            MemberInfo[] ms = T.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            TreeNode ng = new TreeNode("Members");

            ng.Tag = T;

            nw.Nodes.Add(ng);

            foreach (MemberInfo m in ms)
            {
                TreeNode np = new TreeNode(m.Name.ToString());

                ng.Nodes.Add(np);

                ng.Tag = m;
            }
        }

        //type.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)

        static public void PopulateTree_Type(Type c, TreeView tv)
        {
            tv.Nodes.Clear();

            //TreeNode newNode = new TreeNode("object tree");
            //newNode.Tag = L;
            //foreach (Control c in L)
            {
                string name = c.Name.ToString();

                if (name == "")
                    name = c.GetType().FullName.ToString();

                TreeNode ns = new TreeNode(name);

                ns.Tag = c;

                tv.Nodes.Add(ns);

                PopulatePart_methods(ns, c);

                PopulatePart_fields(ns, c);

                PopulatePart_properties(ns, c);

                PopulatePart_members(ns, c);

                PopulatePart_events(ns, c);

                PopulatePart_nestedtypes(ns, c);
            }
        }

        static public void PopulateTreeObject(ArrayList L, TreeView tv)
        {
            tv.Nodes.Clear();

            //TreeNode newNode = new TreeNode("object tree");
            //newNode.Tag = L;
            foreach (Control c in L)
            {
                string name = c.Name.ToString();

                if (name == "")
                    name = c.GetType().FullName.ToString();

                TreeNode ns = new TreeNode(name);

                ns.Tag = c;

                tv.Nodes.Add(ns);

                PopulatePart_methods(ns, c.GetType());

                PopulatePart_fields(ns, c.GetType());

                PopulatePart_properties(ns, c.GetType());

                PopulatePart_members(ns, c.GetType());

                PopulatePart_events(ns, c.GetType());

                PopulatePart_nestedtypes(ns, c.GetType());
            }
        }

        //private void PopulateTree(TreeView tv, Assembly ase)
        //{
        //    TreeNode newNode = new TreeNode(ase.GetName().Name);
        //    newNode.Tag = ase;
        //    tv.Nodes.Add(newNode);

        //    foreach (Module mod in ase.GetModules())
        //    {
        //        AddModule(mod, newNode);
        //    }
        //}

        private void AddModule(Module mod, TreeNode parent)
        {
            TreeNode newNode = new TreeNode(mod.Name);
            newNode.Tag = mod;
            parent.Nodes.Add(newNode);

            foreach (Type t in mod.GetTypes())
            {
                AddType(t, newNode);
            }
        }

        private void AddType(Type t, TreeNode parent)
        {
            TreeNode newNode = new TreeNode(t.Name);
            newNode.Tag = t;
            TreeNode curType;
            TreeNode curMember;

            curType = new TreeNode("Constructors");
            foreach (ConstructorInfo constructor in t.GetConstructors())
            {
                curMember = new TreeNode(constructor.Name);
                curMember.Tag = constructor;
                curType.Nodes.Add(curMember);
            }
            newNode.Nodes.Add(curType);

            curType = new TreeNode("Methods");
            foreach (MethodInfo method in t.GetMethods())
            {
                string methodString = method.Name + "( ";
                int count = method.GetParameters().Length;

                foreach (ParameterInfo param in method.GetParameters())
                {
                    methodString += param.ParameterType;
                    if (param.Position < count - 1)
                        methodString += ", ";
                }
                methodString += " )";
                curMember = new TreeNode(methodString);
                curMember.Tag = method;
                curType.Nodes.Add(curMember);
            }
            newNode.Nodes.Add(curType);

            curType = new TreeNode("Properties");
            foreach (PropertyInfo property in t.GetProperties())
            {
                curMember = new TreeNode(property.Name);
                curMember.Tag = property;
                curType.Nodes.Add(curMember);
            }
            newNode.Nodes.Add(curType);

            curType = new TreeNode("Fields");
            foreach (FieldInfo field in t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField))
            {
                string fieldInfo = field.FieldType.Name;
                fieldInfo += " " + field.Name;
                curMember = new TreeNode(fieldInfo);
                curMember.Tag = field;
                curType.Nodes.Add(curMember);
            }
            newNode.Nodes.Add(curType);

            curType = new TreeNode("Events");
            foreach (EventInfo curEvent in t.GetEvents())
            {
                string eventInfo = curEvent.Name;
                eventInfo += " Delegate Type=" + curEvent.EventHandlerType;
                curMember = new TreeNode(eventInfo);
                curMember.Tag = curEvent;
                curType.Nodes.Add(curMember);
            }
            newNode.Nodes.Add(curType);

            parent.Nodes.Add(newNode);
        }

        private PropertyGrid pg = null;

        private void tv_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                pg.SelectedObject = e.Node.Tag;
            }
            else
            {
                pg.SelectedObject = null;
            }
        }

        public static class ReflectionHelper
        {
            private static Hashtable serializationLists = new Hashtable();

            public static List<FieldInfo> GetSerializableFieldInformation(Type _type)
            {
                if (serializationLists.ContainsKey(_type))
                {
                    return serializationLists[_type] as List<FieldInfo>;
                }

                //if (_type.IsSubclassOf(typeof(DependencyObject)))
                //{
                //    List<FieldInfo> fieldInformation = new List<FieldInfo>();
                //    Type typeRover = _type;

                //    while (typeRover != null && typeRover.BaseType != typeof(DependencyObject))
                //    {
                //        // Retrieve all instance fields.
                //        // This will present us with quite a lot of stuff, such as backings for
                //        // auto-properties, events, etc.
                //        FieldInfo[] fields = typeRover.GetFields
                //            (BindingFlags.Instance | BindingFlags.Public |
                //                            BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                //        foreach (FieldInfo fiRover in fields)
                //        {
                //            if (fiRover.IsNotSerialized)
                //                continue;

                //            // Make sure we don't serialize events here,
                //            // because that will drag along half of
                //            // your forms, views, presenters or whatever is used for user interaction....
                //            if (fiRover.FieldType.IsSubclassOf(typeof(MulticastDelegate)) ||
                //                fiRover.FieldType.IsSubclassOf(typeof(Delegate)))
                //                continue;

                //            fieldInformation.Add(fiRover);
                //        }

                //        typeRover = typeRover.BaseType;
                //    }

                //    serializationLists.Add(_type, fieldInformation);
                //    return fieldInformation;
                //}

                return null;
            }
        }

        [Serializable]
        public class properter : Object
        {
            public PropertyInfo pr = null;

            public Type D = null;

            [NonSerialized]
            public object obs = null;

            public object v = null;

            public string name { get; set; }

            public string ase { get; set; }

            public string fqn { get; set; }

            public void setase()
            {
                if (ase == "")
                    return;

                if (obs == null)
                    return;

                ase = obs.GetType().FullName;

                asename = obs.GetType().AssemblyQualifiedName;
            }

            public string asename { get; set; }

            public string setname(ArrayList q)
            {
                string g = "";

                Type T = obs.GetType();

                //if (T == typeof(Control))
                {
                    Control c = (Control)obs;

                    g = c.Name;

                    name = g;

                    asename = T.AssemblyQualifiedName;
                }

                utils.helpers.fqn n = utils.helpers.fqn.getfqn(q, (Control)obs);

                if (n.c != null)
                    fqn = n.fq;

                return g;
            }

            public object getControl(ArrayList q, object obs)
            {
                //ArrayList L = new ArrayList();
                //L = utils.helpers.getallcontrols(p, L);

                Control cc = ((Control)obs);

                foreach (utils.helpers.fqn f in q)
                {
                    if (f.fq == fqn)
                        return f.c;
                }

                return null;
            }

            public void setcontrol(PropertyInfo pr, Control c, object v)
            {
                pr.SetValue(c, v, null);
            }

            public void setobjectvalue()
            {
                v = pr.GetValue(obs, new object[0]);
            }

            //T CreateType<T>() where T : new()
            //{
            //    return new T();
            //}

            public void createinstance()
            {
                Type T = Type.GetType(asename);

                obs = Activator.CreateInstance(T, new object[0]);
            }
        }
    }
}