using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;


using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AIMS.Libraries.CodeEditor.Syntax

{
    /// <summary>
    /// Allows caching values for a specific compilation.
    /// A CacheManager consists of a for shared instances (shared among all threads working with that resolve context).
    /// </summary>
    /// <remarks>This class is - non - thread-safe</remarks>
    public class CacheManager
    {

        public Dictionary<object, object> sharedDicts = new Dictionary<object, object>();

        public Dictionary<object, object> sharedDict = new Dictionary<object, object>();
        // There used to be a thread-local dictionary here, but I removed it as it was causing memory
        // leaks in some use cases.


        public Dictionary<object, object> FindShared(string name)
        {

            if (sharedDicts.ContainsKey(name))
                return sharedDicts[name] as Dictionary<object, object>;

            return null;

        }

        public void AddShared(string name, Dictionary<object, object> d)
        {


            if (sharedDicts.ContainsKey(name))
                return;

            sharedDicts.Add(name, d);


        }

        public object GetShared(object key)
        {
            object value;
            sharedDict.TryGetValue(key, out value);
            return value;
        }

        public void SetShared(object key, object value)
        {
            sharedDict[key] = value;
        }
    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class ClassMapper
    {

        public CacheManager wm {get; set;}


        public bool HasBeenCreated = false;

        public ArrayList mappers { get; set; }

        /// <summary>
        /// The attributes
        /// </summary>
        [NonSerialized]
        public ArrayList attributes = null;

        /// <summary>
        /// The syntax
        /// </summary>
        [NonSerialized]
        public SyntaxTree syntax = null;

        /// <summary>
        /// Gets or sets the classcode.
        /// </summary>
        /// <value>
        /// The classcode.
        /// </value>
        public string classcode { get; set; }

        /// <summary>
        /// Gets or sets the classname.
        /// </summary>
        /// <value>
        /// The classname.
        /// </value>
        public string classname { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        public ArrayList events { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public ArrayList fields { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string filename { get; set; }

        public IMember member { get; set; }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public ArrayList members { get; set; }

        /// <summary>
        /// Gets or sets the methods.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        public ArrayList methods { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public ArrayList properties { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Addevents the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>
        public EventCreator addevent(IEvent m, AstNode ast)
        {
            if (events == null)
                events = new ArrayList();

            EventCreator me = new EventCreator();

            me.name = m.FullName;

            me.typename = m.ReturnType.FullName;

            me.ast = ast;

            me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

            me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

            events.Add(me);

            return me;
        }

        /// <summary>
        /// Addfields the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>



        /// <summary>
        /// Addfields the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>
        public FieldCreator addfield(IField m, AstNode ast)
        {
            if (fields == null)
                fields = new ArrayList();

            FieldCreator me = new FieldCreator();

            me.name = m.FullName;

            me.typename = m.ReturnType.FullName;

            me.Namespace = m.Namespace;

            me.ast = ast;

            me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

            me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

            me.SetInnerName();

            fields.Add(me);

            return me;
        }

        /// <summary>
        /// Addmembers the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>
        //public MethodCreator addmember(IMember m, AstNode ast, bool isConstructor)
        //{
        //    if (members == null)
        //        members = new ArrayList();

        //    MethodCreator me = new MethodCreator();

        //    me.ctor = isConstructor;

        //    me.name = m.FullName;

            

        //    //me.typename = m.ReturnType.FullName;

        //    me.ast = ast;

        //    me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

        //    me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

        //    me.SetInnerName();

        //    members.Add(me);

        //    return me;
        //}

        /// <summary>
        /// Addmembers the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>
        public MemberCreator addmember(IMember m, AstNode ast)
        {
            if (members == null)
                members = new ArrayList();

            MemberCreator me = new MemberCreator();

           // me.ctor = isConstructor;

            me.name = m.FullName;



            me.typename = m.ReturnType.FullName;

            me.ast = ast;

            me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

            me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

            me.SetInnerName();

            members.Add(me);

            return me;
        }


        /// <summary>
        /// Addmethods the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>
        public MethodCreator addmethod(IMethod m, AstNode ast, bool isctor)
        {
            if (methods == null)
                methods = new ArrayList();

            MethodCreator me = new MethodCreator();

            me.ctor = isctor;

            me.name = m.FullName;

            me.ast = ast;

            me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

            me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

            me.SetInnerName();

            int i = 0;
            while (i < methods.Count)
            {
                MethodCreator c = methods[i] as MethodCreator;
                if (c.end.Y > me.start.Y)
                {
                    methods.Insert(i, me);
                    return me;
                }
                i++;
            }
            methods.Add(me);

            
            return me;
        }


        public FieldCreator GetFieldCreator(int Y)
        {

            if (fields == null)
                return null;

            int i = 0;
            while (i < fields.Count)
            {
                FieldCreator c = fields[i] as FieldCreator;

                if (c.start.Y <= Y && c.end.Y >= Y)
                    return c;

                i++;
            }

            return null;
        }


        public MethodCreator GetMethodCreator(int Y)
        {

            if (methods == null)
                return null;

            int i = 0;
            while (i < methods.Count)
            {
                MethodCreator c = methods[i] as MethodCreator;

                if (c.start.Y <= Y && c.end.Y >= Y)
                    return c;

                i++;
            }

            return null;
        }

        /// <summary>
        /// Addproperties the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="ast">The ast.</param>
        /// <returns></returns>
        public PropertyCreator addproperty(IProperty m, AstNode ast)
        {
            if (properties == null)
                properties = new ArrayList();

            PropertyCreator me = new PropertyCreator();

            me.name = m.FullName;

            me.typename = m.ReturnType.FullName;

            me.ast = ast;

            me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

            me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

            properties.Add(me);

            return me;
        }

        public ArrayList GetFieldNames()
        {
            ArrayList F = new ArrayList();
            if(fields != null)
            foreach (FieldCreator f in fields)
            {
                F.Add(f.InnerName);
            }

            return F;
        }

        public ArrayList GetMethodsNames()
        {
            ArrayList F = new ArrayList();
            if (fields != null)
                foreach (MethodCreator f in methods)
                {
                    F.Add(f.InnerName);
                }

            return F;
        }

        /// <summary>
        /// Codes this instance.
        /// </summary>
        public void code()
        {
            /// <summary>
            /// Definicja zadań
            /// </summary>
            //protected override void InitializeTasks()
            //{
            //    base.InitializeTasks();
            //    //definicja kategorii
            //    SpTaskCategory fileCategory = new SpTaskCategory("file", "File", TaskCategories);
            //    SpTaskCategory helpCategory = new SpTaskCategory("help", "Help", TaskCategories);
            //    SpTaskCategory toolsCategory = new SpTaskCategory("tools", "Tools", TaskCategories);
            //    //zadanie manage layouts
            //    SpTask layoutManagerTask = new SpTask("layoutManager", "&Layout manager", TaskCategories["tools"]);
            //    layoutManagerTask.Execute += new EventHandler(layoutManagerTask_Execute);
            //    //zadanie Exit
            //    SpTask exitTask = new SpTask("exit", "E&xit", TaskCategories["file"]);
            //    exitTask.Execute += new EventHandler(exitTask_Execute);
            //    exitTask.Image = SpResource.Exit.ToBitmap();
            //    //zadanie About
            //    SpTask aboutTask = new SpTask("about", "&About", TaskCategories["help"]);
            //    aboutTask.Execute += new EventHandler(aboutTask_Execute);
            //    aboutTask.Image = Resource1.About;
            //    //zadanie Appearance
            //    SpTask appearanceManagerTask = new SpTask("appearanceManager", "&Appearance manager", TaskCategories["tools"]);
            //    appearanceManagerTask.Execute += new EventHandler(appearanceManagerTask_Execute);
            //    //zadanie Binding Manager
            //    SpTask bindingManagerTask = new SpTask("bindingManager", "&Binding manager", TaskCategories["tools"]);
            //    bindingManagerTask.Execute += new EventHandler(bindingManagerTask_Execute);
            //    //zadanie Binding Manager Alternative
            //    SpTask bindingManagerAltTask = new SpTask("bindingManagerAlt", "&Binding manager alternative", TaskCategories["tools"]);
            //    bindingManagerAltTask.Execute += new EventHandler(bindingManagerAltTask_Execute);
            //    SpTask loadProjectTask = new SpTask("loadProject", "Load project", TaskCategories["file"]);
            //    loadProjectTask.Execute += new EventHandler(loadProjectTask_Execute);
            //    //zadanie robocze
            //    roboczeTask = new SpTask("roboczeTask", "robocze", viewCategory);
            //    roboczeTask.Execute += new EventHandler(roboczeTask_Execute);
            //    //dodanie komponentu
            //    SpTask addComponent = new SpTask("mainAddComp", "Add component", viewCategory);
            //    addComponent.Image = Resource1.dialog_more;
            //    addComponent.Execute += new EventHandler(addComponent_Execute);
            //    //dodanie komponentu z szablonu
            //    SpTask addTplComponent = new SpTask("mainAddTplComp", "Add component from tpl", viewCategory);
            //    addTplComponent.Execute += new EventHandler(addTplComponent_Execute);
            //    //dodanie projektu z szablonu
            //    SpTask addTplProject = new SpTask("mainAddTplProject", "Add project from tpl", viewCategory);
            //    addTplProject.Execute += new EventHandler(addTplProject_Execute);
            //    //usuniecie wszystkich komponentów
            //    SpTask closeAllCompTask = new SpTask("closeAllComp", "Close all components", viewCategory);
            //    closeAllCompTask.Execute += new EventHandler(closeAllTask_Execute);
            //    closeAllCompTask.Image = Resource1.edit_clear_4;
            //    //settigns
            //    SpTask settingsTask = new SpTask("applicationSettings", "Application settings", TaskCategories["tools"]);
            //    settingsTask.Image = SpResource.Settings_icon;
            //    settingsTask.Execute += new EventHandler(settingsTask_Execute);
            //}
        }

        public IMember getmember(EntityDeclaration ent)
        {
            foreach (IMember m in members)
            {
                if (m.FullName == ent.Name)
                    return m;
            }

            return null;
        }

        /// <summary>
        /// Loads the attributes.
        /// </summary>
        /// <param name="tdd">The TDD.</param>
        public void loadAttributes(TypeDeclaration tdd)
        {
            attributes = new ArrayList();

            foreach (EntityDeclaration memb in tdd.Members)
            {
                foreach (AstNode a in memb.Attributes)
                    attributes.Add(a);
            }
        }
    }

    public class ClassVisitor : DepthFirstAstVisitor
    {
        public readonly CSharpAstResolver Resolver;

        public ClassVisitor(CSharpAstResolver resolver)
        {
            Resolver = resolver;
        }

        public StringBuilder b { get; set; }

        public ICSharpCode.NRefactory.Utils.CacheManager cm { get; set; }

        public string filename { get; set; }

        public ClassMapper mapper { get; set; }

        public ArrayList mappers { get; set; }

        public bool HasBeenCreated = false;

        public SyntaxTree syntax { get; set; }

        public CacheManager wm { get; set; }

        public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            string f = fieldDeclaration.ToString();

            b.Append("Field declaration : " + f + "\n");
        }

        public override void VisitMemberType(MemberType memberType)
        {
            string f = memberType.ToString();
            b.Append("MemberType : " + f + "\n");
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
        {
            ResolveResult r = Resolver.Resolve(namespaceDeclaration);

            Namespace = namespaceDeclaration.Name;

            base.VisitNamespaceDeclaration(namespaceDeclaration);
        }


        public string Namespace { get; set; }

        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {

            if (mappers == null)
            {
                mappers = new ArrayList();
                mappers.Add(mapper);
                mapper.HasBeenCreated = false;
             
            }

            if (mapper.HasBeenCreated == true)
            {

                if (mappers == null)
                    mappers = new ArrayList();
                mappers.Add(mapper);
                mapper = new ClassMapper();
                mapper.HasBeenCreated = false;   

            }

            mapper.HasBeenCreated = true;

            mapper.loadAttributes(typeDeclaration);

            Dictionary<string, object> d = new Dictionary<string, object>();

            Console.WriteLine(typeDeclaration.Name + " : ");
            b.Append(typeDeclaration.Name + " : ");
            b.Append(typeDeclaration.GetRegion().ToString());
            b.Append(typeDeclaration.StartLocation.ToString() + "\n");
            b.Append(Resolver.UnresolvedFile.FileName);


            d.Add("Namespace", Namespace);
            d.Add("Name", typeDeclaration.Name);
            d.Add("Region", typeDeclaration.GetRegion().ToString());
            d.Add("Location", typeDeclaration.ToString());
            d.Add("FileName", Resolver.UnresolvedFile.FileName.ToString());
            d.Add("FullFileName", filename);

            mapper.classname = typeDeclaration.Name;

            mapper.classcode = typeDeclaration.Region.ToString();

            mapper.start = new Point(typeDeclaration.StartLocation.Column, typeDeclaration.StartLocation.Line);

            mapper.end = new Point(typeDeclaration.EndLocation.Column, typeDeclaration.EndLocation.Line);

            if (wm != null)
                wm.SetShared(typeDeclaration.Name, d);

            var typeResolveResult = Resolver.Resolve(typeDeclaration);

            if (typeResolveResult.IsError == true)
                return;

            mapper.Namespace = typeResolveResult.Type.Namespace;

            ITypeDefinition def = typeResolveResult.Type.GetDefinition();

            b.Append(def.Accessibility.ToString() + " : ");

            foreach (ITypeParameter p in def.TypeParameters)
            {
                b.Append("\t" + p.Name + " : ");
            }

            foreach (IProperty property in typeResolveResult.Type.GetProperties(property => true))
            {
                //Console.Write("\t" + method.Name + " : ");
                //b.Append("\t" + method.Name + " : ");

                //d.Add("Men", method.Name);
                //d.Add("Loc", method.Region.ToString());
                //d.Add("Reg", method.BodyRegion.ToString());

                AstNode ast = syntax.GetNodeContaining(property.Region.Begin, property.Region.End);

                PropertyCreator fc = mapper.addproperty(property, ast);
            }

            foreach (IMember member in typeResolveResult.Type.GetMembers(member => true))
            {
                //Console.Write("\t" + method.Name + " : ");
                //b.Append("\t" + method.Name + " : ");

                //d.Add("Men", method.Name);
                //d.Add("Loc", method.Region.ToString());
                //d.Add("Reg", method.BodyRegion.ToString());

                

                AstNode ast = syntax.GetNodeContaining(member.Region.Begin, member.Region.End);

                MemberCreator fc = mapper.addmember(member, ast);
            }

            foreach (IMethod member in typeResolveResult.Type.GetConstructors(null))
            {
    

                AstNode ast = syntax.GetNodeContaining(member.Region.Begin, member.Region.End);

                MethodCreator fc = mapper.addmethod(member, ast, true);
            }


            foreach (IField field in typeResolveResult.Type.GetFields(field => true))
            {
                //Console.Write("\t" + method.Name + " : ");
                //b.Append("\t" + method.Name + " : ");

                //d.Add("Men", method.Name);
                //d.Add("Loc", method.Region.ToString());
                //d.Add("Reg", method.BodyRegion.ToString());

                AstNode ast = syntax.GetNodeContaining(field.Region.Begin, field.Region.End);

                FieldCreator fc = mapper.addfield(field, ast);
            }

            List<ResolveResult> r = typeResolveResult.GetChildResults().ToList();

            foreach (IMethod method in typeResolveResult.Type.GetMethods(method => true))
            {
                //Console.Write("\t" + method.Name + " : ");
                //b.Append("\t" + method.Name + " : ");

                //d.Add("Men", method.Name);
                //d.Add("Loc", method.Region.ToString());
                //d.Add("Reg", method.BodyRegion.ToString());

                AstNode ast = syntax.GetNodeContaining(method.Region.Begin, method.Region.End);

                MethodCreator mc = mapper.addmethod(method, ast, false);

                if (method.Parameters.Count == 0)
                {
                    Console.WriteLine("void");
                    b.Append("void");
                    // d.Add("Par", "void");
                }
                else
                {
                    IParameter[] parameters = method.Parameters.ToArray();

                    string[] p = new string[method.Parameters.Count];
                    int i = 0;
                    foreach (var parameter in method.Parameters)
                    {
                        Console.WriteLine(parameter + " ");
                        b.Append(parameter + " ");
                        p[i++] = parameter.ToString();

                        mc.createparameter(ast, parameter);
                    }

                    //   d.Add("Par", p);

                    Console.WriteLine();
                    b.AppendLine();
                }
            }

            foreach (IEvent ev in typeResolveResult.Type.GetEvents(ev => true))
            {
                //Console.Write("\t" + ev.Name + " : ");
                //b.Append("\t" + ev.Name + " : ");

                //d.Add("Men", method.Name);
                //d.Add("Loc", method.Region.ToString());
                //d.Add("Reg", method.BodyRegion.ToString());

                AstNode ast = syntax.GetNodeContaining(ev.Region.Begin, ev.Region.End);

                EventCreator mc = mapper.addevent(ev, ast);

                //IAttribute v = ev.GetAttribute(, true);

                //if (ev.Parameters.Count == 0)
                //{
                //    Console.WriteLine("void");
                //    b.Append("void");
                //    d.Add("Par", "void");
                //}
                //else
                //{
                //    IParameter[] parameters = method.Parameters.ToArray();

                //    string[] p = new string[method.Parameters.Count];
                //    int i = 0;
                //    foreach (var parameter in method.Parameters)
                //    {
                //        Console.WriteLine(parameter + " ");
                //        b.Append(parameter + " ");
                //        p[i++] = parameter.ToString();

                //        mc.createparameter(ast, parameter);

                //    }

                //    d.Add("Par", p);

                //    Console.WriteLine();
                //    b.AppendLine();
                //}
            }

            base.VisitTypeDeclaration(typeDeclaration);
        }
    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class EventCreator
    {
        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The attribute
        /// </summary>
        [NonSerialized]
        public IAttribute attribute = null;

        /// <summary>
        /// The parameters
        /// </summary>
        [NonSerialized]
        public ArrayList parameters = null;

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public string access { get; set; }

        /// <summary>
        /// Gets or sets the attrs.
        /// </summary>
        /// <value>
        /// The attrs.
        /// </value>
        public string attrs { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the PMS.
        /// </summary>
        /// <value>
        /// The PMS.
        /// </value>
        public ArrayList pms { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Gets or sets the typename.
        /// </summary>
        /// <value>
        /// The typename.
        /// </value>
        public string typename { get; set; }

        /// <summary>
        /// Gets or sets the visible.
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        public string visible { get; set; }

        /// <summary>
        /// Createparameters the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public ParameterCreator createparameter(AstNode a, IParameter p)
        {
            ParameterCreator pc = new ParameterCreator(a, p);

            if (parameters == null)
                parameters = new ArrayList();

            parameters.Add(pc);

            return pc;
        }

        /// <summary>
        /// Geteventstrings this instance.
        /// </summary>
        /// <returns></returns>
        public string geteventstring()
        {
            StringBuilder b = new StringBuilder();

            string s = "";

            b.Append(attrs);

            b.Append(signature);

            b.Append(body);

            return b.ToString();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class eventwriter
    {
        /// <summary>
        /// The alls
        /// </summary>
        public string[] alls = {"[SpBindableEvent(\"Zdarzenie generowane po wybraniu pozycji na liście wyboru. W obiekcie kontekst przekazwyna jest zawartość tekstowa wybranej pozycji\", typeof(String))]\n",
                                   "public event SpBindableEventHandler SelectedItemChanged;\n",
                                   //"public BindExampleSender()",
                                   //"{",
                                   //"InitializeComponent();",
                                   //"}",
                                   "private void listBox1_SelectedIndexChanged(object sender, EventArgs e)\n",
                                   "{if (SelectedItemChanged != null) {SelectedItemChanged(this, new SpBindableEventArgs(listBox1.SelectedItem))}"};

        /// <summary>
        /// The attributes
        /// </summary>
        public string attributes = null;

        /// <summary>
        /// The body
        /// </summary>
        public string body = null;

        /// <summary>
        /// The events
        /// </summary>
        public string events = null;

        /// <summary>
        /// The signature
        /// </summary>
        public string signature = null;

        /// <summary>
        /// Addnamespaces the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public string addnamespace(string c, string code)
        {
            string cc = "namespace " + c + "{\n";

            cc = cc + code + " \n";

            cc = cc + "}\n";

            return cc;
        }

        /// <summary>
        /// Getattrses this instance.
        /// </summary>
        /// <returns></returns>
        public string getattrs()
        {
            return attributes;
        }

        /// <summary>
        /// Getbodies this instance.
        /// </summary>
        /// <returns></returns>
        public string getbody()
        {
            return body;
        }

        /// <summary>
        /// Getcodes this instance.
        /// </summary>
        /// <returns></returns>
        public string getcode()
        {
            string code = "public class test {\n";

            string body = string.Concat(alls);

            code = string.Concat(code, body);

            body = "\n}";

            code = string.Concat(code, body);

            code = addnamespace("mynamespace", code);

            return code;
        }

        /// <summary>
        /// Geteventses this instance.
        /// </summary>
        /// <returns></returns>
        public string getevents()
        {
            return events;
        }

        /// <summary>
        /// Getsignatures this instance.
        /// </summary>
        /// <returns></returns>
        public string getsignature()
        {
            return signature;
        }

        /// <summary>
        /// Setattrses this instance.
        /// </summary>
        public void setattrs()
        {
            attributes = alls[0];
        }

        /// <summary>
        /// Setbodies this instance.
        /// </summary>
        public void setbody()
        {
            body = alls[3];
        }

        /// <summary>
        /// Seteventses this instance.
        /// </summary>
        public void setevents()
        {
            events = alls[1];
        }

        /// <summary>
        /// Setsignatures this instance.
        /// </summary>
        public void setsignature()
        {
            signature = alls[2];
        }
    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class FieldCreator
    {
        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The parameters
        /// </summary>
        [NonSerialized]
        public ArrayList parameters = null;

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public string access { get; set; }

        /// <summary>
        /// Gets or sets the attrs.
        /// </summary>
        /// <value>
        /// The attrs.
        /// </value>
        public string attrs { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string filename { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the PMS.
        /// </summary>
        /// <value>
        /// The PMS.
        /// </value>
        public ArrayList pms { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Gets or sets the typename.
        /// </summary>
        /// <value>
        /// The typename.
        /// </value>
        public string typename { get; set; }

        /// <summary>
        /// Gets or sets the visible.
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        public string visible { get; set; }

        /// <summary>
        /// Createparameters the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        /// 

        public string InnerName { get; set; }
        public ParameterCreator createparameter(AstNode a, IParameter p)
        {
            ParameterCreator pc = new ParameterCreator(a, p);

            if (parameters == null)
                parameters = new ArrayList();

            parameters.Add(pc);

            return pc;
        }

        /// <summary>
        /// Getfieldstrings this instance.
        /// </summary>
        /// <returns></returns>
        public string getfieldstring()
        {
            StringBuilder b = new StringBuilder();

            string s = "";

            b.Append(attrs);

            b.Append(signature);

            b.Append(body);

            return b.ToString();
        }

        public void SetInnerName()
        {

            if (name == "")
                return;

            if (name.EndsWith(".ctor") == true)
                name = name.Replace(".ctor", "");

            string[] s = name.Split(".".ToCharArray());

            string c = s[s.Length - 1];

            InnerName = c;

        }

    }

    public class MainTest
    {
        public ICSharpCode.NRefactory.Utils.CacheManager cm = new ICSharpCode.NRefactory.Utils.CacheManager();

        public SyntaxTree syntax = null;

        public CacheManager wm = new CacheManager();

        public StringBuilder b { get; set; }

        public ClassMapper mapper { get; set; }

        public ArrayList mappers { get; set; }

        static public ClassMapper analysecode(string sourceText, string file)
        {
            StringBuilder b = new StringBuilder();

            ClassMapper mapper = new ClassMapper();

            mapper.filename = file;

            string filename = file;

            //string sourceText = File.ReadAllText(filename);

            var parser = new CSharpParser();
            SyntaxTree syntaxTree = parser.Parse(sourceText, file);
            var pc = new CSharpProjectContent();
            pc = (CSharpProjectContent)pc.AddOrUpdateFiles(syntaxTree.ToTypeSystem());
            ICompilation compilation = pc.CreateCompilation();
            var resolver = new CSharpAstResolver(compilation, syntaxTree, syntaxTree.ToTypeSystem());

            var classVisitor = new ClassVisitor(resolver);
            classVisitor.filename = filename;
            classVisitor.b = b;
            classVisitor.wm = new CacheManager();
            classVisitor.mapper = mapper;

            classVisitor.syntax = syntaxTree;

            syntaxTree.AcceptVisitor(classVisitor);

            mapper.syntax = syntaxTree;

            mapper.wm = classVisitor.wm;

            mapper.mappers = classVisitor.mappers;

            return mapper;
        }

    

        public void AnalyzeCSharpFile(string path)
        {
            mapper = new ClassMapper();

            if (mappers == null)
                mappers = new ArrayList();

            mappers.Add(mapper);

            mapper.filename = path;

            string filename = path;

            string sourceText = File.ReadAllText(filename);

            CSharpParser parser = new CSharpParser();
            SyntaxTree syntaxTree = parser.Parse(sourceText, Path.GetFileNameWithoutExtension(filename));
            var pc = new CSharpProjectContent();
            pc = (CSharpProjectContent)pc.AddOrUpdateFiles(syntaxTree.ToTypeSystem());
            ICompilation compilation = pc.CreateCompilation();
            var resolver = new CSharpAstResolver(compilation, syntaxTree, syntaxTree.ToTypeSystem());

            var classVisitor = new ClassVisitor(resolver);
            classVisitor.filename = path;
            classVisitor.b = b;
            classVisitor.wm = wm;
            classVisitor.mapper = mapper;

            classVisitor.syntax = syntaxTree;

            syntaxTree.AcceptVisitor(classVisitor);

            syntax = syntaxTree;
        }

        public void AnalyzeCSharpProject(string path)
        {
            mappers = new ArrayList();

            b = new StringBuilder();

            path = Path.GetDirectoryName(path);

            path = Path.GetFullPath(path);

            ICSharpCode.NRefactory.CSharp.CSharpParser parse = new ICSharpCode.NRefactory.CSharp.CSharpParser();

            string[] tree = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            int i = 0;

            foreach (string s in tree)
            {
                AnalyzeCSharpFile(s);

                //if (i > 2000)
                //    break;

                i++;
            }

            //string p = b.ToString();

            //File.WriteAllText("code-class.txt", p);

            // ICSharpCode.NRefactory.Utils.FastSerializer fs = new ICSharpCode.NRefactory.Utils.FastSerializer();

            // Persist to file
            FileStream stream = File.Create("dict.txt");
            BinaryFormatter formatter = new BinaryFormatter();
            Console.WriteLine("Serializing vector");
            formatter.Serialize(stream, wm.sharedDict);
            stream.Close();
        }

        static public IEnumerable<EntityDeclaration> getentity(SyntaxTree syntax, string ents)
        {
            string classname = ents;

            IEnumerable<EntityDeclaration> doc = syntax.GetTypes(true);

            foreach (EntityDeclaration ent in doc)
            {
                if (ent.Name == classname)
                {

                    return doc;
                }
            }

            return doc;
        }

        static public void AnalyzeSyntaxTree(SyntaxTree syntax, ClassMapper mapper)
        {

            string ents = mapper.classname;


            IEnumerable<EntityDeclaration> doc = getentity(syntax, ents);

            if (doc == null)
                return;

            foreach (EntityDeclaration ent in doc)
            {

                AstNode asc = ent as AstNode;

                if (doc.Count() >= 1)
                {

                    if (asc.GetType() == typeof(DelegateDeclaration))
                    {
                        DelegateDeclaration d = (DelegateDeclaration)asc;

                    }
                    else if (asc.GetType() == typeof(TypeDeclaration))
                    {

                        TypeDeclaration d = (TypeDeclaration)asc;

                        if (d.ClassType == ClassType.Interface)
                        {

                        }
                        else if (d.ClassType == ClassType.Enum)
                        {

                        }

                    }

                }

                foreach (AstNode ast in asc.Descendants)
                {
                    if (ast.NodeType.ToString() == "MemberType" || ast.NodeType.ToString() == "Member")
                    {


                        Type T = ast.GetType();

                        if (ast.GetType() == typeof(MethodDeclaration))
                        {
                            MethodDeclaration dec2 = (MethodDeclaration)ast;

                            string method = dec2.Name;

                            MethodCreator g = null;


                            if(mapper.methods != null)
                            foreach (MethodCreator mc in mapper.methods)
                            {

                                //string[] s = mc.name.Split(".".ToCharArray());

                                string s = mc.InnerName;

                                if (method == s)
                                {

                                    g = mc;
                                    //MessageBox.Show("Method found - " + mc.name);


                                }

                            }

                            foreach (Statement s in dec2.Body.Statements)
                            {

                                if (g == null)
                                    continue;

                                g.addstatement(s, dec2);



                                if (s.GetType() == typeof(VariableDeclarationStatement))
                                {

                                    VariableDeclarationStatement d = (VariableDeclarationStatement)s;
 
                                    string types = d.Type.ToString();

                                    if (g == null)
                                        continue;

                                    VarDeclCreator dc = new VarDeclCreator();

                                    foreach( VariableInitializer c in  d.Variables)
                                    {
                                        dc.name = c.Name;
                                        dc.SetInnerName();
                                        break;
                                    }
                                        
                                    dc.type = types;

                                    g.decls.Add(dc);

                                }

                            }




                            string pp = "( ";

                            foreach (ParameterDeclaration p in dec2.Parameters)
                            {
                                pp += p.Name + " ,";
                            }




                            Modifiers m = dec2.Modifiers;


                        }

                        else if (ast.GetType() == typeof(ConstructorDeclaration))
                        {
                            ConstructorDeclaration dec2 = (ConstructorDeclaration)ast;



                            string pp = "( ";

                            foreach (ParameterDeclaration p in dec2.Parameters)
                            {
                                pp += p.Name + " ,";
                            }


                        }


                        else if (ast.GetType() == typeof(FieldDeclaration))
                        {
                            FieldDeclaration dec3 = (FieldDeclaration)ast;



                            string pp = "";

                            foreach (VariableInitializer v in dec3.Variables)
                            {
                                pp += v.Name + " ";
                            }




                        }
                        else if (ast.GetType() == typeof(ConstructorDeclaration))
                        {
                            ConstructorDeclaration dec4 = (ConstructorDeclaration)ast;



                            string pp = "";

                            foreach (ParameterDeclaration v in dec4.Parameters)
                            {
                                pp += v.Name + " ";
                            }





                        }
                        else if (ast.GetType() == typeof(PropertyDeclaration))
                        {
                            PropertyDeclaration dec5 = (PropertyDeclaration)ast;



                            string pp = "";

                            Accessor ac = dec5.Getter;

                            ac = dec5.Setter;




                        }

                        //nods.Nodes.Add(nv);


                        // string bc = getnodebyrole(ast, "TypeMember", nv);
                    }
                }
            }

        }


        public void AnalyzeCSharpProjectFile(string path, string file)
        {
            mappers = new ArrayList();

            b = new StringBuilder();

            path = Path.GetDirectoryName(path);

            path = Path.GetFullPath(path);

            //ICSharpCode.NRefactory.CSharp.CSharpParser parse = new ICSharpCode.NRefactory.CSharp.CSharpParser();

            //string[] tree = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            //int i = 0;

            //foreach (string s in tree)
            //{
            AnalyzeCSharpFile(file);

            //    if (i > 2000)
            //        break;

            //    i++;

            //}

            string p = b.ToString();

            File.WriteAllText("code-class.txt", p);

            // ICSharpCode.NRefactory.Utils.FastSerializer fs = new ICSharpCode.NRefactory.Utils.FastSerializer();

            // Persist to file
            FileStream stream = File.Create("dict.txt");
            BinaryFormatter formatter = new BinaryFormatter();
            Console.WriteLine("Serializing vector");
            formatter.Serialize(stream, wm.sharedDict);
            stream.Close();
        }

        public void doloaddefaults(string path)
        {
            b = new StringBuilder();
            //path = Path.GetDirectoryName(path);
            path = Path.GetFullPath(path);

            AnalyzeCSharpFile(path);
        }
    }

    //public class AddMethodVisitor : CSharpOutputVisitor
    //{
    //    //public AddMethodVisitor(IOutputFormatter formatter, CSharpFormattingOptions formattingPolicy) : base(formatter, formattingPolicy)
    //    //{
    //    //}

    //    public void dosomething()
    //    {
    //    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class MemberCreator
    {

        public MemberCreator()
        {
            decls = new ArrayList();
        }

        //public bool ctor = false;

        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The parameters
        /// </summary>
        [NonSerialized]
        public ArrayList parameters = null;

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public string access { get; set; }

        /// <summary>
        /// Gets or sets the attrs.
        /// </summary>
        /// <value>
        /// The attrs.
        /// </value>
        public string attrs { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string filename { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the PMS.
        /// </summary>
        /// <value>
        /// The PMS.
        /// </value>
        public ArrayList pms { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Gets or sets the typename.
        /// </summary>
        /// <value>
        /// The typename.
        /// </value>
        public string typename { get; set; }

        /// <summary>
        /// Gets or sets the visible.
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        /// 
        public string InnerName { get; set; }
        public ArrayList decls { get; set; }
        public string visible { get; set; }

        public bool ctor = false;

        /// <summary>
        /// Createparameters the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public ParameterCreator createparameter(AstNode a, IParameter p)
        {
            ParameterCreator pc = new ParameterCreator(a, p);

            if (parameters == null)
                parameters = new ArrayList();

            parameters.Add(pc);

            return pc;
        }

        /// <summary>
        /// Getmemberstrings this instance.
        /// </summary>
        /// <returns></returns>
        public string getmemberstring()
        {
            StringBuilder b = new StringBuilder();

            string s = "";

            b.Append(attrs);

            b.Append(signature);

            b.Append(body);

            return b.ToString();
        }

        public void SetInnerName()
        {

            if (name == "")
                return;

            if (name.EndsWith(".ctor"))
                name.Replace(".ctor", "");

            string[] s = name.Split(".".ToCharArray());

            string c = s[s.Length - 1];

            InnerName = c;

          
        }
    }

    public class VarDeclCreator
    {

        public string name { get; set; }

        public string InnerName { get; set; }

        public string type { get; set; }

        //public string InnerName { get; set; }

        public void SetInnerName()
        {

            if (name == "")
                return;
            string[] s = name.Split(".".ToCharArray());

            string c = s[s.Length - 1];

            InnerName = c;

        }


    }


    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class MethodCreator
    {

        public MethodCreator()
        {
            decls = new ArrayList();

            states = new ArrayList();
        }

        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The parameters
        /// </summary>
        [NonSerialized]
        public ArrayList parameters = null;

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public string access { get; set; }

        /// <summary>
        /// Gets or sets the attrs.
        /// </summary>
        /// <value>
        /// The attrs.
        /// </value>
        public string attrs { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string filename { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the PMS.
        /// </summary>
        /// <value>
        /// The PMS.
        /// </value>
        public ArrayList pms { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Gets or sets the visible.
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        public string visible { get; set; }

        /// <summary>
        /// Createparameters the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public ArrayList decls { get; set; }

        public bool ctor = false;

        public ParameterCreator createparameter(AstNode a, IParameter p)
        {
            ParameterCreator pc = new ParameterCreator(a, p);

            if (parameters == null)
                parameters = new ArrayList();

            parameters.Add(pc);

            return pc;
        }

        /// <summary>
        /// Getmethodstrings this instance.
        /// </summary>
        /// <returns></returns>
        public string getmethodstring()
        {
            StringBuilder b = new StringBuilder();

            string s = "";

            b.Append(attrs);

            b.Append(signature);

            b.Append(body);

            return b.ToString();
        }

        public string InnerName { get; set; }

              public void SetInnerName()
        {

            if (name == "")
                return;

            if(name.EndsWith(".ctor") == true)
                name = name.Replace(".ctor","");

            string[] s = name.Split(".".ToCharArray());

            string c = s[s.Length - 1];

            InnerName = c;

        }

              public ArrayList states { get; set; }


              public StatementCreator addstatement(Statement m, AstNode ast)
              {
                  if (states == null)
                      states = new ArrayList();

                  StatementCreator me = new StatementCreator();

                  
                  
                  me.body = m.ToString();

                  me.name = "";

                  me.Namespace = "";

                  me.ast = ast;

                  me.start = new Point(m.Region.BeginColumn, m.Region.BeginLine);

                  me.end = new Point(m.Region.EndColumn, m.Region.EndLine);

                  states.Add(me);

                  return me;
              }

              public StatementCreator GetStatementCreator(int Y)
              {

                  if (states == null)
                      return null;

                  int i = 0;
                  while (i < states.Count)
                  {
                      StatementCreator c = states[i] as StatementCreator;

                      

                      if (c.start.Y <= Y && c.end.Y >= Y)
                          return c;

                      i++;
                  }

                  return null;
              }

              public ArrayList GetParamNames()
              {
                  ArrayList P = new ArrayList();
                  if(parameters != null)
                  foreach (ParameterCreator p in parameters)
                      P.Add(p.name);

                  return P;

              }

    }


    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class StatementCreator
    {

        public StatementCreator()
        {
            states = new ArrayList();
        }

        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The parameters
        /// </summary>
        [NonSerialized]
        public ArrayList parameters = null;

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public string access { get; set; }

        /// <summary>
        /// Gets or sets the attrs.
        /// </summary>
        /// <value>
        /// The attrs.
        /// </value>
        public string attrs { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string filename { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the PMS.
        /// </summary>
        /// <value>
        /// The PMS.
        /// </value>
        public ArrayList pms { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Gets or sets the visible.
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        public string visible { get; set; }

        /// <summary>
        /// Createparameters the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public ArrayList states { get; set; }

        public bool ctor = false;

        public ParameterCreator createparameter(AstNode a, IParameter p)
        {
            ParameterCreator pc = new ParameterCreator(a, p);

            if (parameters == null)
                parameters = new ArrayList();

            parameters.Add(pc);

            return pc;
        }

        /// <summary>
        /// Getmethodstrings this instance.
        /// </summary>
        /// <returns></returns>
        public string getmethodstring()
        {
            StringBuilder b = new StringBuilder();

            string s = "";

            b.Append(attrs);

            b.Append(signature);

            b.Append(body);

            return b.ToString();
        }

        public string InnerName { get; set; }

        public void SetInnerName()
        {

            if (name == "")
                return;

            if (name.EndsWith(".ctor") == true)
                name = name.Replace(".ctor", "");

            string[] s = name.Split(".".ToCharArray());

            string c = s[s.Length - 1];

            InnerName = c;

        }

        public string ContainsWord(ArrayList F, string word)
        {

            foreach (string s in F)
            {
                if(s == word)
                if (body.Contains(s))
                    return s;
            }

            return "";
        }

    }

    
    
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class ParameterCreator
    {
        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The attrs
        /// </summary>
        [NonSerialized]
        public ArrayList attrs = null;

        public string name { get; set; }

        public string types { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCreator"/> class.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        public ParameterCreator(AstNode a, IParameter p)
        {

            name = p.Name;

            types = p.Type.FullName;


            ast = a;

            attrs = new ArrayList();
            foreach (IAttribute at in p.Attributes)
                attrs.Add(at);
        }
    }

    // }
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class PropertyCreator
    {
        /// <summary>
        /// The ast
        /// </summary>
        [NonSerialized]
        public AstNode ast = null;

        /// <summary>
        /// The parameters
        /// </summary>
        [NonSerialized]
        public ArrayList parameters = null;

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public string access { get; set; }

        /// <summary>
        /// Gets or sets the attrs.
        /// </summary>
        /// <value>
        /// The attrs.
        /// </value>
        public string attrs { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string body { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public Point end { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string filename { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the PMS.
        /// </summary>
        /// <value>
        /// The PMS.
        /// </value>
        public ArrayList pms { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        public string signature { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Point start { get; set; }

        /// <summary>
        /// Gets or sets the typename.
        /// </summary>
        /// <value>
        /// The typename.
        /// </value>
        public string typename { get; set; }

        /// <summary>
        /// Gets or sets the visible.
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        public string visible { get; set; }

        /// <summary>
        /// Createparameters the specified a.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public ParameterCreator createparameter(AstNode a, IParameter p)
        {
            ParameterCreator pc = new ParameterCreator(a, p);

            if (parameters == null)
                parameters = new ArrayList();

            parameters.Add(pc);

            return pc;
        }

        /// <summary>
        /// Getpropertystrings this instance.
        /// </summary>
        /// <returns></returns>
        public string getpropertystring()
        {
            StringBuilder b = new StringBuilder();

            string s = "";

            b.Append(attrs);

            b.Append(signature);

            b.Append(body);

            return b.ToString();
        }
    }
}