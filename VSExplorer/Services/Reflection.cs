using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;
using System.IO;

namespace WinExplorer.Services
{
    public class Reflection
    {
        public static Type[] AssemblyTypes(string file)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(file);
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                }
                return null;
            }
            finally
            {
            }
            var types = assembly.GetTypes();

            foreach(Type t in types)
            {
                if (t.Name == "VSExplorerSolution_Test")
                {
                    var attributes = t.GetCustomAttributes();
                    foreach (var a in attributes)
                    {
                        //MessageBox.Show(a.ToString());
                        foreach (MethodInfo m in t.GetMethods())
                        {
                            var ab = m.GetCustomAttributes();
                            foreach (var ac in ab)
                            {
                                MessageBox.Show(ac.GetType().Name);
                                var tt = ac.GetType();
                                if (ac.GetType().Name == "TestMethodAttribute")
                                    MessageBox.Show( "--- " + ac.ToString());
                            }
                        }
                    }
                }
            }
                      


            //var methods = assembly.GetTypes()
            //          .SelectMany(t => t.GetMethods())
            //          .Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), true).Length > 0)
            //          .ToArray();

            // Obtain a reference to a method known to exist in assembly.
            //MethodInfo Method = SampleAssembly.GetTypes()[0].GetMethod("Method1");
            //// Obtain a reference to the parameters collection of the MethodInfo instance.
            //ParameterInfo[] Params = Method.GetParameters();
            //// Display information about method parameters.
            //// Param = sParam1
            ////   Type = System.String
            ////   Position = 0
            ////   Optional=False
            //foreach (ParameterInfo Param in Params)
            //{
            //    Console.WriteLine("Param=" + Param.Name.ToString());
            //    Console.WriteLine("  Type=" + Param.ParameterType.ToString());
            //    Console.WriteLine("  Position=" + Param.Position.ToString());
            //    Console.WriteLine("  Optional=" + Param.IsOptional.ToString());
            //}
            return assembly.GetTypes();
            
        }
        public static List<MethodInfo> AttributesOfMethods(string file, string typename, string attributename)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFile(file);
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                }
                return null;
            }
            finally
            {
            }

            List<MethodInfo> methods = new List<MethodInfo>();

            var types = assembly.GetTypes();

            foreach (Type t in types)
            {
                if (t.Name == typename/*"VSExplorerSolution_Test"*/)
                {
                    //var attributes = t.GetCustomAttributes();
                    //foreach (var a in attributes)
                    {
                        //MessageBox.Show(a.ToString());
                        foreach (MethodInfo m in t.GetMethods())
                        {
                            var ab = m.GetCustomAttributes();
                            foreach (var ac in ab)
                            {
                                //          MessageBox.Show(ac.GetType().Name);
                                //          var tt = ac.GetType();
                                if (ac.GetType().Name == /*"TestMethodAttribute"*/attributename)
                                    // MessageBox.Show("--- " + ac.ToString());
                                    methods.Add(m);

                            }
                        }
                    }
                }
            }
            return methods;
        }
        public static List<MethodInfo> AttributesOfMethodsFromTypes(string file, List<Type> types, string attributename)
        {
            List<MethodInfo> methods = new List<MethodInfo>();


            foreach (Type t in types)
            {
//                if (t.Name == typename/*"VSExplorerSolution_Test"*/)
                {
                    //var attributes = t.GetCustomAttributes();
                    //foreach (var a in attributes)
                    {
                        //MessageBox.Show(a.ToString());
                        foreach (MethodInfo m in t.GetMethods())
                        {
                            var ab = m.GetCustomAttributes();
                            foreach (var ac in ab)
                            {
                                //          MessageBox.Show(ac.GetType().Name);
                                //          var tt = ac.GetType();
                                if (ac.GetType().Name == /*"TestMethodAttribute"*/attributename)
                                    // MessageBox.Show("--- " + ac.ToString());
                                    methods.Add(m);

                            }
                        }
                    }
                }
            }
            return methods;
        }

        public static List<Type> AttributesOfTypes(string file, string attributename)
        {
            var current = Directory.GetCurrentDirectory();

            var working = Path.GetDirectoryName(file);

            Directory.SetCurrentDirectory(working);
            Assembly assembly;
            try
            {
                //assembly = Assembly.LoadFile(file);
                assembly = Assembly.LoadFrom(file);
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                }
                return null;
            }
            finally
            {
            }

            List<Type> Types = new List<Type>();

            try
            {

                var types = assembly.GetTypes();

                foreach (Type t in types)
                {
                    //    if (t.Name == typename/*"VSExplorerSolution_Test"*/)
                    {
                        //var attributes = t.GetCustomAttributes();
                        //foreach (var a in attributes)
                        {
                            //MessageBox.Show(a.ToString());
                            //foreach (MethodInfo m in t.GetMethods())
                            {
                                var ab = t.GetCustomAttributes();
                                foreach (var ac in ab)
                                {
                                    //          MessageBox.Show(ac.GetType().Name);
                                    //          var tt = ac.GetType();
                                    if (ac.GetType().Name == /*"TestMethodAttribute"*/attributename)
                                        // MessageBox.Show("--- " + ac.ToString());
                                        Types.Add(t);

                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {

            }
            Directory.SetCurrentDirectory(current);
            return Types;
        }
        public static List<Type> Attributes<T>(Type[] types)
        {
            var attributes = new List<Type>();
            attributes = types.Where(t => t.IsDefined(typeof(T))).ToList();
            return attributes;
        }
        public static List<Type> Attributes<T>(Type type)
        {
            var attributes = new List<Type>();
            attributes = Enumerable.Repeat(type, 1).Where(t => t.IsDefined(typeof(T))).ToList();
            return attributes;
        }
        //public static List<MethodInfo> AttributesOfMethods<T>(Type type)
        //{
        //    var attributes = new List<MethodInfo>();
        //    attributes = type.GetMethods().Select(m => m).Where(m => m.GetCustomAttributes().OfType<T>().Any()).ToList();

        //    return attributes;
        //}
    }
}
